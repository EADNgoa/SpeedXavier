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
using System.IO;
using System.Text.RegularExpressions;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class BanksController : Controller
    {
        protected Repository db;
        protected int newImportId;
        protected int newAbstTableId;
        public BanksController()
        {
            this.db = new Repository(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=DtsxToSql;Integrated Security=True", "System.Data.SqlClient");
        }

        public string ImportFilesFromDir()
        {
            var xmlPath = Server.MapPath(@"~/Dtsx/");
            string[] xmlFiles = Directory.GetFiles(xmlPath, "*.xml");
            var SuccessUpload = new List<string>();
            var FailedUpload = new List<string>();

            foreach (string xFile in xmlFiles)
            {
                try
                {
                    string fileName = xFile.Substring(xFile.LastIndexOf("\\") + 1);
                    Debug.Print(fileName.PadRight(30, '*'));
                    ImportXmlToSql(fileName);
                    SuccessUpload.Add(fileName);
                    
                }
                catch (Exception e)
                {
                    FailedUpload.Add(xFile.Substring(xFile.LastIndexOf("\\")) + " because " + e.Message);

                }
            }

            string res = $"The following {SuccessUpload.Count} files were successfully uploaded: {String.Join(", \n ", SuccessUpload)} \n ";
            if (FailedUpload.Count > 0)
                res += $"Errrors were encountered uploading the following {FailedUpload.Count} files: {String.Join(", \n", FailedUpload)} ";
            return res;
        }

        public void ImportXmlToSql(string xFile)
        {
            string fileForImport = @"http://localhost:53040/Dtsx/"+ xFile;
            
         
            XDocument doc = XDocument.Load(fileForImport);
            fileForImport = fileForImport.TrimStart(@"http://localhost:53040/Dtsx/".ToCharArray());
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
            
        }
        
        public void EAXmlDig(XElement input, int level, int Parent_Id)
        {
            int newId = LogNode(input, level++, Parent_Id, input.HasElements ? "ParentElement" : "Element",db.SingleOrDefault<AbstTable>(Parent_Id).Name ??"");
            
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
                        ImportLog_Id=newImportId,
                        Abst_Id= newAbstTableId++,
                        recursionLevel = level,
                        Type = GetTypeInt("Text"),
                        ParentTableName = input.Name.LocalName,
                        Parent_Id = newId,
                        Value = input.LastNode.ToString().Replace("\r\n\t", "")
                    });
                }
            }
        }

        public int LogNode(XElement elem, int level, int Parent_Id, string elemType, string parentName)
        {
            var nsStripper = RemoveBetween(elem.Name.ToString(), '{', '}');
            string elemName = nsStripper.Key;
            string ns = nsStripper.Value;
            //string elemName = elem.Name.ToString().Replace("\r\n\t", "").Replace("{www.microsoft.com/SqlServer/Dts}", "");
            //elem.Attributes().Where(e => e.IsNamespaceDeclaration).Remove();
            //elemName = elem.Name.ToString().Replace("\r\n\t", "").Replace(@"{http://schemas.xmlsoap.org/soap/encoding/}", "");
            //elemName = elem.Name.ToString().Replace("\r\n\t", "").Replace(@"{http://schemas.xmlsoap.org/soap/envelope/}", "");
            //elemName = elem.Name.ToString().Replace("\r\n\t", "").Replace("{www.microsoft.com/sqlserver/dts/tasks/sqltask}", "");

            //LogParent the parents data
            int newId = (int)db.Insert(new AbstTable
            {
                ImportLog_Id = newImportId,
                Abst_Id = newAbstTableId++,
                Name = (elemType == "ParentElement")? $"{elemName}_{level}_{elem.Parent?.Name.LocalName ?? ""}": elemName,
                recursionLevel = level,
                Type = GetTypeInt(elemType),
                ParentTableName = parentName,
                Parent_Id = Parent_Id,
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
                        ImportLog_Id = newImportId,
                        Abst_Id = newAbstTableId++,
                        Name = x.Name.LocalName,
                        Value = x.Value,
                        recursionLevel = level+1,
                        Type = GetTypeInt("Attribute"),
                        ParentTableName = elemName,
                        Parent_Id = newId,
                        AttrElem_Id = newId
                    });
                });
            } else
            {   //Leaf element's attributes that will become fields
                attrList.ForEach(x =>
                {
                    db.Insert(new AbstTable
                    {
                        ImportLog_Id = newImportId,
                        Abst_Id = newAbstTableId++,
                        Name = x.Name.LocalName,
                        Value = x.Value,
                        recursionLevel = level,
                        Type = GetTypeInt("Attribute"),
                        ParentTableName = parentName,
                        Parent_Id = Parent_Id,
                        AttrElem_Id = newId
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
                    "(SELECT [Name] AS TblName, recursionLevel, 1 AS IsHomoLeaf, ParentTableName, Parent_Id " +
                    "FROM AbstTable WHERE type = 0 AND Parent_Id IN " +
                    "   (SELECT Parent_Id FROM AbstTable GROUP BY Parent_Id HAVING count(Parent_Id) > 1)	" +
                    "GROUP BY [Name], recursionLevel, ParentTableName, Parent_Id HAVING count(Name) > 1) " +
                $"UPDATE C SET Type={(int)EnumFldType.HomoElement}, [Name] = CONCAT ([Name], '_', C.recursionLevel, '-', C.ParentTableName) " +
                    "FROM AbstTable C JOIN HomoLeaves O ON C.recursionLevel = O.recursionLevel AND C.Name = O.TblName " +
                    "AND C.ParentTableName = O.ParentTableName and C.Parent_Id=O.Parent_Id");

            
            //Fetch the list of potential tables to create: Hetro union Homogeneous tables
            var tableDefs = db.Query<TableDef>($"select [Name] as TblName,recursionLevel, 0 as IsHomoLeaf, ParentTableName as ParentTblName  from AbstTable where ([type]={ (int)EnumFldType.ParentElement} or [type]={(int)EnumFldType.Root}) and " +
                "Abst_Id in (Select distinct Parent_Id from AbstTable where type<>1) " + //1 is attributes 
                "group by [Name], recursionLevel, ParentTableName" +
                " UNION " +
                $"select [Name] as TblName,recursionLevel, 1 as IsHomoLeaf, ParentTableName as ParentTblName from AbstTable where type={ (int)EnumFldType.HomoElement} and Parent_Id in " +
                $"(select Parent_Id from AbstTable group by Parent_Id having count(Parent_Id)>1 ) group by [Name], recursionLevel, ParentTableName, Parent_Id HAVING count(Name) > 1 order by recursionLevel").ToList();

            //Sometimes homoLeaf's PARENT tables dont have kids and so get re-added as homoleaf tables. Here below we remove those duplicate leaf tables
            var HetrotableDefs = tableDefs.Where(t => t.IsHomoLeaf==false).ToList();
            var HomotableDefs = tableDefs.Where(t => t.IsHomoLeaf==true).ToList();
            HomotableDefs = HomotableDefs.Except(HetrotableDefs, new TableDefEqualityComparer()).ToList();
            tableDefs = HetrotableDefs.Concat(HomotableDefs).ToList();



            //Fetch the list of fields for each table 
            tableDefs.ForEach(t =>
            {
                t.TblFields = db.Fetch<FieldDef>("select Name, Type, max(len([Value])) as MaxFieldLen from " +
                $"(select ISNULL(Name, '{t.TblName}') as Name, Type, Value from AbstTable where ParentTableName = '{t.TblName}' and recursionLevel = {t.RecursionLevel+ 1} and type<>{(int)EnumFldType.HomoElement} " + //n + 1 for child elements
                $" 	and (AttrElem_Id is null or AttrElem_Id NOT in (select Abst_Id from AbstTable where type={(int)EnumFldType.HomoElement})) " +//exclude attributes of homo tables in fields of its parent
                $"union select ISNULL(Name, '{t.TblName}') as Name, Type, Value from AbstTable where Name is null and type={(int)EnumFldType.Text} and ParentTableName = substring('{t.TblName}', 0, CHARINDEX('_', '{t.TblName}')) and recursionLevel = {t.RecursionLevel + 1} and type<>{(int)EnumFldType.HomoElement} " +
                $"union select '{t.TblName}' as Name, Type, Value from AbstTable where ParentTableName='{t.ParentTblName}' and type={(int)EnumFldType.HomoElement} and recursionLevel={t.RecursionLevel} " +//to store the text value in Homofield
                "union " +
                $"select Name, Type, Value from AbstTable where AttrElem_Id in (select Abst_Id from AbstTable where Name = '{t.TblName}' and recursionLevel = {t.RecursionLevel})) as tmpTbl " + //n for attributes
                "group by Name,Type " +
                "having max(len([Value])) > 0");

                //Debug.Print(db.LastSQL);
            });


            
            //Generate the Create Table statements            
            tableDefs.ToList().ForEach(t =>
            {
                if (db.Query<string>($"select table_schema from INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='{t.TblName}'").Count()==0) //Ensure we dont try to make the same table twice (when importing multiple xmls)
                {
                    StringBuilder createSQL = new StringBuilder($"CREATE TABLE [{t.TblName}] (ImportLog_Id int NOT NULL, ");

                    createSQL.Append($"[{t.TblName}_Id] INT NOT NULL, PRIMARY KEY (ImportLog_Id, [{t.TblName}_Id]), "); //make composite PK

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
                            $" CONSTRAINT [FK_{t.TblName}-{t.ParentTblName}] FOREIGN KEY (ImportLog_Id, [{t.ParentTblName}_Id]) REFERENCES [{t.ParentTblName}](ImportLog_Id, [{t.ParentTblName}_Id]), " +
                            $" CONSTRAINT [FK_{t.TblName}-ImportLog] FOREIGN KEY (ImportLog_Id) REFERENCES [ImportLog](ImportLog_Id)");
                    }
                    createSQL.Append(" )"); //Closing bracket of create stmt
                    t.CreateSQL = createSQL.ToString();
                    Debug.Print(t.CreateSQL);
                    db.Execute(t.CreateSQL);
                } else //add new fields if any to the table
                {
                    //First fetch the existing fields in the table
                    var fieldsInDBTable = db.Query<FieldDef>("select col.name as Name, (CASE t.name WHEN 'int' THEN 0 ELSE 1 END ) as Type,  col.max_length as MaxFieldLen " +
                        "from sys.tables as tab inner join sys.columns as col on tab.object_id = col.object_id " +
                        "left join sys.types as t on col.user_type_id = t.user_type_id " +
                        $"where tab.name = '{t.TblName}' " +
                        "order by col.name, Type").ToList();

                    var newFields = t.TblFields.Except<FieldDef>(fieldsInDBTable, new FieldDefEqualityComparer()).ToList();  //make a list of only the new fields                    

                    foreach (var fld in newFields)
                    {
                        string altStmt = $"ALTER TABLE [{t.TblName}] ADD [{fld.Name}_{fld.Type}] VARCHAR(MAX)";
                        t.AlterSQL.Add(altStmt);
                        Debug.Print(altStmt);
                        db.Execute(altStmt);
                    }
                }
            });

            //With our statements ready, lets now make our tables
            //tableDefs.OrderBy(o=>o.RecursionLevel).ToList().ForEach(t => {
            //    if (t.CreateSQL != null)
            //    {
            //        Debug.Print(t.CreateSQL);
            //        //db.Execute(t.CreateSQL);

            //    }
            //    t.AlterSQL.ForEach(sq =>
            //    {
            //        Debug.Print(sq);
            //        //db.Execute(sq);
            //    });
            //});

            return tableDefs;
        }

        private void FillSQLtables(List<TableDef> tableDefs)
        {
            //This outerloop takes care of Hetro tables. The homo ones are populates in an inner loop
            tableDefs.Where(t=>t.IsHomoLeaf==false).ToList().ForEach(t => {

                //get the Ids of each row to be inserted
                List<int> insIds = db.Query<int>($"SELECT distinct Parent_Id from AbstTable where recursionLevel={t.RecursionLevel + 1} and ParentTableName='{t.TblName}' ").ToList();

                foreach (var id in insIds)
                {
                    var insData = new Dictionary<string, string>();
                    //Debug.Print($"Parent_Id={id} and type<>{(int)EnumFldType.ParentElement}");

                    if (db.Query<int>($"select COUNT(name) as Cont from AbstTable where Parent_Id={id} and type<>{(int)EnumFldType.ParentElement} group by name having COUNT(name)>1").Any())
                    {//This happens when there are many leaf elements of the same name under a parent
                        //first insert the parent's data. TODO: case when this type of parent has hetrogenous kids in addition to the homogeneous ones.                         
                        insData=db.Query<AbstTable>("select Name, Value from AbstTable where Parent_Id=@0 and type<>@1 and type<>@2 " +
                            "and (AttrElem_Id is null or AttrElem_Id not in (select Abst_Id from AbstTable where Parent_Id=@0 and Type=@2))", //exclude attributes of homo child tables
                            id, (int)EnumFldType.ParentElement, (int)EnumFldType.HomoElement).ToDictionary(key => key.Name ?? t.TblName, Val => Val.Value);
                        MakeInsertStmt(t, id, insData);

                        //Next take care of the homogeneous kids
                        var homoLeafTbl = db.First<TableDef>($"select distinct Name as TblName, RecursionLevel from AbstTable where type={(int)EnumFldType.HomoElement} and Parent_Id={id} and AttrElem_Id is null");//Gets the homoleaf table name
                        TableDef hlt = tableDefs.FirstOrDefault(th => th.IsHomoLeaf == true && th.TblName == homoLeafTbl.TblName && th.RecursionLevel == homoLeafTbl.RecursionLevel);

                        if (hlt!=null)
                        {
                            var homoInsIds = db.Query<AbstTable>($"select * from AbstTable where Parent_Id={id} and AttrElem_Id is null and Name = '{hlt.TblName}'").ToList();
                            homoInsIds.ForEach(hi =>
                            {
                                insData.Clear();

                                //Fetch its attributes
                                try
                                {
                                    insData = db.Query<AbstTable>($"select * from AbstTable where Parent_Id={id} and Parent_Id<>AttrElem_Id and AttrElem_Id={hi.Abst_Id}").ToDictionary(key => key.Name ?? t.TblName, Val => Val.Value);
                                    insData.Add(hi.Name, hi.Value ?? "");//the value of the homo leaf
                                }
                                catch (Exception e)
                                {

                                    throw e;
                                }
                                MakeInsertStmt(hlt, hi.Abst_Id, insData);
                            }); 
                        }
                    }
                    else
                    {
                        insData = db.Query<AbstTable>("select Name, Value from AbstTable where Parent_Id=@0 and type<>@1", id, (int)EnumFldType.ParentElement).ToDictionary(key => key.Name ?? t.TblName, Val => Val.Value);
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
            StringBuilder insStmt = new StringBuilder($"INSERT INTO [{t.TblName}] (ImportLog_Id , [{t.TblName}_Id], ");
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
                insValues.Append($"{db.Single<AbstTable>(id).Parent_Id})");//Fk from original import table
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
            ParentElement,
            HomoElement
        }


        public class FieldDef
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
            public List<FieldDef> TblFields { get; set; }
            public int FieldCnt { get { return TblFields.Count; }  }
            public string CreateSQL { get; set; }
            public List<string> InsertSQL { get; set; }
            public List<string> AlterSQL { get; set; }

            public TableDef()
            {
                InsertSQL = new List<string>();
                AlterSQL = new List<string>();
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

        public class FieldDefEqualityComparer : IEqualityComparer<FieldDef>
        {
            public bool Equals(FieldDef x, FieldDef y)
            {
                var res = (GetOriginalFldName(y) == GetOriginalFldName(x));
                return res;
            }

            private static string GetOriginalFldName(FieldDef y)
            {
                string ystr = y.Name;
                return ystr.IndexOf('_') > 0 ? ystr.Substring(0, ystr.IndexOf('_')) : ystr;                
            }

            public int GetHashCode(FieldDef obj)
            {
                return GetOriginalFldName(obj).GetHashCode();
            }

        }

        public KeyValuePair<string,string> RemoveBetween(string s, char begin, char end)
        {
            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            var nsKey = regex.Replace(s, string.Empty);
            var nsStr = regex.Match(s).Value;

            if (nsStr.Length > 0)
            {
                if (!db.Exists<NameSpaceTracker>($"where Namespace='{nsStr}'"))
                {
                    int newNsId=int.Parse(db.Insert(new NameSpaceTracker { NameSpace = nsStr }).ToString());
                    //nsKey = $"ns{newNsId}|{nsKey}";
                }
                //else
                    //nsKey = $"ns{db.First<NameSpaceTracker>($"where Namespace='{nsStr}'").NS_Id}|{nsKey}";
            }

            return new KeyValuePair<string, string>(nsKey, nsStr);
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
