using Microsoft.AspNet.Identity;
using Speedbird.Controllers;
using System;
using System.Collections.Generic;
//using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
//using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class BanksController : Controller
    {
        protected Repository db;
        protected int newImportId;
        protected int newAbstTableId;
        public BanksController()
        {
            this.db = new Repository(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Fintastic;Integrated Security=True", "System.Data.SqlClient");
        }

        public ActionResult ImportXmlToSql()
        {               
            string fileForImport = @"http://localhost:53040/Eadtsx.xml";
         
            XDocument doc = XDocument.Load(fileForImport);
            fileForImport = fileForImport.TrimStart(@"http://localhost:53040/".ToCharArray());
            newImportId = int.Parse(db.Insert(new ImportLog() { FileName = fileForImport, ImportDate = DateTime.UtcNow }).ToString());
            newAbstTableId = 1;
            
            //truncate the temp table
            db.Execute("truncate table AbstTable");
            //Log the root element
            var root = doc.Elements().First();
            int newId = LogNode(root, 0, 0, "Root","");
            try
            {
                //The child of doc is root, so get the kids of root and log them.
                var rootsKids = root.Elements();
                rootsKids.ToList().ForEach(e => { EAXmlDig(e, 1, newId); });


                //Make the tables based on the XML imported
                var tableDefs = MakeSQLTables();

                //Fill the tables with data from the imported XML
                FillSQLtables(tableDefs);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
                throw e;
            }
            return Json(new object() , JsonRequestBehavior.AllowGet);
        }
        
        public void EAXmlDig(XElement input, int level, int parentId)
        {
            int newId = LogNode(input, level++, parentId, input.HasElements ? "ParentElement" : "Element",db.SingleOrDefault<AbstTable>(parentId).Name ??"");
            
            //Parent Element            
            var elemList = input.Elements().ToList();
            elemList.ForEach(x => {
                EAXmlDig(x, level, newId);                
            });

            if (elemList.Count > 0)
            {
                if (input.LastNode.NodeType.ToString() == "Text")
                {
                    //MixedMode: Parent has elements as well as text
                    db.Insert(new AbstTable
                    {
                        ImportLogId=newImportId,
                        AbstId= newAbstTableId++,
                        recursionLevel = level,
                        Type = GetTypeInt("Text"),
                        ParentTableName = input.Name.LocalName,
                        ParentId = newId,
                        Value = input.LastNode.ToString().Replace("\r\n\t", "")
                    });
                }
            }
        }

        public int LogNode(XElement elem, int level, int parentId, string elemType, string parentName)
        {
            string elemName = elem.Name.ToString().Replace("\r\n\t", "").Replace("{www.microsoft.com/SqlServer/Dts}", "");
            
            //LogParent the parents data
            int newId = (int)db.Insert(new AbstTable
            {
                ImportLogId = newImportId,
                AbstId = newAbstTableId++,
                Name = (elemType == "ParentElement")? $"{elemName}_{level}_{elem.Parent?.Name.LocalName ?? ""}": elemName,
                recursionLevel = level,
                Type = GetTypeInt(elemType),
                ParentTableName = parentName,
                ParentId = parentId,
                Value= elem.HasElements?"": elem.Value
            });

            //Log its attributes
            var attrList = elem.Attributes().ToList();
            if (elem.HasElements)
            {   //Parent element's attributes which will become tables
                attrList.ForEach(x =>
                {
                    db.Insert(new AbstTable
                    {
                        ImportLogId = newImportId,
                        AbstId = newAbstTableId++,
                        Name = x.Name.LocalName,
                        Value = x.Value,
                        recursionLevel = level+1,
                        Type = GetTypeInt("Attribute"),
                        ParentTableName = elemName,
                        ParentId = newId,
                        AttrElemId = newId
                    });
                });
            } else
            {   //Leaf element's attributes that will become fields
                attrList.ForEach(x =>
                {
                    db.Insert(new AbstTable
                    {
                        ImportLogId = newImportId,
                        AbstId = newAbstTableId++,
                        Name = x.Name.LocalName,
                        Value = x.Value,
                        recursionLevel = level,
                        Type = GetTypeInt("Attribute"),
                        ParentTableName = parentName,
                        ParentId = parentId,
                        AttrElemId = newId
                    });
                });
            }

            return newId;
        }

        private List<TableDef> MakeSQLTables()
        {
            //HOMO TABLE: If multiple child leaf elemets share a common name they must be put into a table of their own. This we call a homogeneous table

            //The names of the hetro tables have already been put into the Name_Id_ParentName format when filling AbstTable.
            //Now that we know which the Homo tables are, we must format their names too.
            db.Execute(";WITH HomoLeaves AS " +
                    "(SELECT [Name] AS TblName, recursionLevel, 1 AS IsHomoLeaf, ParentTableName " +
                    "FROM AbstTable WHERE type = 0 AND parentId IN " +
                    "   (SELECT ParentId FROM AbstTable GROUP BY ParentId HAVING count(parentId) > 1)	" +
                    "GROUP BY [Name], recursionLevel, ParentTableName) " +
                "UPDATE C SET [Name] = CONCAT ([Name], '_', C.recursionLevel, '-', C.ParentTableName) " +
                    "FROM AbstTable C JOIN HomoLeaves O ON C.recursionLevel = O.recursionLevel AND C.Name = O.TblName " +
                    "AND C.ParentTableName = O.ParentTableName");

            
            //Fetch the list of potential tables to create: Hetro union Homogeneous tables
            var tableDefs = db.Query<TableDef>($"select [Name] as TblName,recursionLevel, 0 as IsHomoLeaf, ParentTableName as ParentTblName  from AbstTable where ([type]={ (int)EnumFldType.ParentElement} or [type]={(int)EnumFldType.Root}) and " +
                "AbstId in (Select distinct ParentId from AbstTable where type<>1) " + //1 is attributes 
                "group by[Name], recursionLevel, ParentTableName" +
                " UNION " +
                $"select [Name] as TblName,recursionLevel, 1 as IsHomoLeaf, ParentTableName as ParentTblName from AbstTable where type={ (int)EnumFldType.Element} and parentId in " +
                $"(select ParentId from AbstTable group by ParentId having count(parentId)>1 ) group by [Name], recursionLevel, ParentTableName").ToList();

            //Sometimes homoLeaf's PARENT tables dont have kids and so get re-added as homoleaf tables. Here below we remove those duplicate leaf tables
            var HetrotableDefs = tableDefs.Where(t => t.IsHomoLeaf==false).ToList();
            var HomotableDefs = tableDefs.Where(t => t.IsHomoLeaf==true).ToList();
            HomotableDefs = HomotableDefs.Except(HetrotableDefs, new TableDefEqualityComparer()).ToList();
            tableDefs = HetrotableDefs.Concat(HomotableDefs).ToList();

            //Fetch the list of fields for each table //and then get the table parent
            tableDefs.ForEach(t =>
            {
                t.TblFields = db.Fetch<FieldLst>("select Name, Type, max(len([Value])) as MaxFieldLen from " +
                $"(select ISNULL(Name, '{t.TblName}') as Name, Type, Value from AbstTable where ParentTableName = '{t.TblName}' and recursionLevel = {t.RecursionLevel+ 1} " +//n + 1 for child elements
                "union " +
                $"select Name, Type, Value from AbstTable where AttrElemId in (select AbstId from AbstTable where Name = '{t.TblName}' and recursionLevel = {t.RecursionLevel})) as tmpTbl " + //n for attributes
                "group by Name,Type " +
                "having max(len([Value])) > 0");

                //t.ParentTblName = db.ExecuteScalar<string>($"select top 1 CONCAT(ParentTableName,'_',{t.RecursionLevel - 1}) as ParentTableName from AbstTable where Name='{t.TblName}' and recursionLevel={t.RecursionLevel}");
            });

            //Some Parents may have only other parents as their children. They have no leaf or attribute nodes. Hence dont make tables from them.
            //Generate the Create Table statements
            //tableDefs.Where(t => t.FieldCnt > 0).ToList().ForEach(t =>
            tableDefs.ToList().ForEach(t =>
            {
                if (db.Query<string>($"select table_schema from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='{t.TblName}'").Count()==0)
                {
                    StringBuilder createSQL = new StringBuilder($"CREATE TABLE [{t.TblName}] (ImportLogId int NOT NULL, ");

                    createSQL.Append($"[{t.TblName}Id] INT NOT NULL, PRIMARY KEY (ImporLogtId, [{t.TblName}Id]), "); //make composite PK

                    //Fill table fields
                    t.TblFields.ForEach(f =>
                    {
                        createSQL.Append($"[{f.Name}_{f.Type}] VARCHAR(max) NULL, "); //Postfix fields with numbers according to EnumFldType
                    });

                    //make FK to Parent for all below the kids of root. 
                    if (t.RecursionLevel > 0)
                    {
                        //string pureParentName = t.ParentTblName.Substring(0, t.ParentTblName.LastIndexOf('_'));
                        createSQL.Append($"[{t.ParentTblName}_Id] INT NULL, " +
                            $" CONSTRAINT [FK_{t.TblName}-{t.ParentTblName}] FOREIGN KEY ([{t.ParentTblName}_Id]) REFERENCES [{t.ParentTblName}](ImportId,[{t.ParentTblName}Id]), " +
                            $" CONSTRAINT [FK_{t.TblName}-ImportLog] FOREIGN KEY (ImportLogId) REFERENCES [ImportLog](ImportLogId)");
                    }
                    createSQL.Append(" )"); //Closing bracket of create stmt
                    t.CreateSQL = createSQL.ToString(); 
                }
            });

            //With our statements ready, lets now make our tables
            tableDefs.OrderBy(o=>o.RecursionLevel).ToList().ForEach(t => {
                Debug.Print(t.CreateSQL);
                if (t.CreateSQL != null)
                { db.Execute(t.CreateSQL); }
            });

            return tableDefs;
        }

        private void FillSQLtables(List<TableDef> tableDefs)
        {
            //This outerloop takes care of Hetro tables. The homo ones are populates in an inner loop
            tableDefs.Where(t=>t.IsHomoLeaf==false).ToList().ForEach(t => {

                //get the Ids of each row to be inserted
                List<int> insIds = db.Query<int>($"SELECT distinct ParentId from AbstTable where recursionLevel={t.RecursionLevel + 1} and ParentTableName='{t.TblName}' ").ToList();

                foreach (var id in insIds)
                {
                    var insData = new Dictionary<string, string>();
                    Debug.Print($"parentId={id} and type<>{(int)EnumFldType.ParentElement}");

                    if (db.Query<int>($"select COUNT(name) as Cont from AbstTable where parentId={id} and type<>{(int)EnumFldType.ParentElement} group by name having COUNT(name)>1").Any())
                    {//This happens when there are many leaf elements of the same name under a parent
                        //first insert the parent's attributes. TODO: case when this type of parent has hetrogenous kids in addition to the homogeneous ones. 
                        insData = db.Query<AbstTable>("select Name, Value from AbstTable where AttrElemId=ParentId and parentId=@0 and type<>@1", id, (int)EnumFldType.ParentElement).ToDictionary(key => key.Name ?? t.TblName, Val => Val.Value);
                        MakeInsertStmt(t, id, insData);

                        //Next take care of the homogeneous kids
                        var homoLeafTbl = db.First<TableDef>("select distinct Name as TblName, RecursionLevel from AbstTable where parentId=" + id + " and AttrElemId is null");//Gets the homoleaf table name
                        TableDef hlt = tableDefs.First(th => th.IsHomoLeaf == true && th.TblName == homoLeafTbl.TblName && th.RecursionLevel == homoLeafTbl.RecursionLevel);

                        var homoInsIds = db.Query<int>($"select AbstId from AbstTable where parentId={id} and AttrElemId is null").ToList();
                        homoInsIds.ForEach (hi => {
                            insData = db.Query<AbstTable>($"select * from AbstTable where parentId={id} and ParentId<>AttrElemId and AttrElemId={hi}").ToDictionary(key => key.Name ?? t.TblName, Val => Val.Value);
                            MakeInsertStmt(hlt, hi, insData);
                        }) ;
                    }
                    else
                    {
                        insData = db.Query<AbstTable>("select Name, Value from AbstTable where parentId=@0 and type<>@1", id, (int)EnumFldType.ParentElement).ToDictionary(key => key.Name ?? t.TblName, Val => Val.Value);
                        MakeInsertStmt(t, id, insData);
                    }
                }
            });
            

            tableDefs.OrderBy(o => o.RecursionLevel).ToList().ForEach(t => {
                t.InsertSQL.ForEach(sq =>
                {
                    Debug.Print(sq);
                    db.Execute(sq);
                });
            });
        }

        private void MakeInsertStmt(TableDef t, int id, Dictionary<string, string> insData)
        {
            StringBuilder insStmt = new StringBuilder($"INSERT INTO [{t.TblName}] (ImportId , [{t.TblName}Id], ");
            StringBuilder insValues = new StringBuilder($" VALUES ({newImportId}, {id}, "); //Pk from original import table

            t.TblFields.ForEach(f =>
            {
                insStmt.Append($"[{f.Name}_{f.Type}], ");
                insValues.Append($"'{((insData.ContainsKey(f.Name)) ? insData[f.Name].Trim().Replace("'","''") : "")}', ");
            });

            if (t.RecursionLevel == 0)
            {
                insStmt.Replace(",", ")", insStmt.ToString().LastIndexOf(","), 1);
                insValues.Replace(",", ")", insValues.ToString().LastIndexOf(","), 1);
            }
            else
            {
                insStmt.Append($"[{t.ParentTblName}_Id])");
                insValues.Append($"{db.Single<AbstTable>(id).ParentId})");//Fk from original import table
            }
            
            t.InsertSQL.Add(insStmt.Append(insValues.ToString()).ToString());
        }
        
        public int GetTypeInt(string type)
        {
            return (int)System.Enum.Parse(typeof(EnumFldType), type);
        }

        enum EnumFldType
        {
            Element,
            Attribute,
            Text,
            Root,
            ParentElement
        }


        public class FieldLst
        {
            public string Name { get; set; }
            public int Type { get; set; }
            public int MaxFieldLen { get; set; }
        }

        public class TableDef
        {
            public string TblName { get; set; }
            public bool IsHomoLeaf { get; set; }
            public int RecursionLevel { get; set; }
            public string ParentTblName { get; set; }
            public int ParntTblId { get; set; }
            public List<FieldLst> TblFields { get; set; }
            public int FieldCnt { get { return TblFields.Count; }  }
            public string CreateSQL { get; set; }
            public List<string> InsertSQL { get; set; }

            public TableDef()
            {
                InsertSQL = new List<string>();
            }
        }

        public class TableDefEqualityComparer : IEqualityComparer<TableDef>
        {
            public bool Equals(TableDef x, TableDef y)
            {
                bool res = (x.TblName == y.TblName && x.RecursionLevel == y.RecursionLevel && x.ParentTblName==y.ParentTblName);
                Debug.Print($"{res} for {x.TblName}={y.TblName} and {x.RecursionLevel}={y.RecursionLevel} and {x.ParentTblName}={y.ParentTblName}");
                return res;
            }
            public int GetHashCode(TableDef obj)
            {
                return obj.TblName.GetHashCode();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
