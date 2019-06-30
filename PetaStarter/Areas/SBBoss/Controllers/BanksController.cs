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
    public class BanksController : EAController
    {

        public ActionResult Fat()
        {
            XDocument doc = XDocument.Load("http://localhost:53040/Books.xml");

            //truncate the temp table
            db.Execute("truncate table AbstTable");
            //Log the root element
            var root = doc.Elements().First();
            int newId = LogNode(root, 0, 0, "Root");

            //The child of doc is root, so get the kids of root and log them.
            var rootsKids = root.Elements();
            rootsKids.ToList().ForEach(e => { EAXmlDig(e, 1, newId); });


            //Make the tables based on the XML imported
            var tableDefs = MakeSQLTables();

            //Fill the tables with data from the imported XML
            FillSQLtables(tableDefs);
            return View();
        }

        private void FillSQLtables(List<TableDef> tableDefs)
        {
            tableDefs.ForEach(t => {

                t.InsertSQL = new List<string>();                

                //get the Ids of each row to be inserted
                List<int> insIds = db.Query<int>($"SELECT distinct ParentId from AbstTable where recursionLevel={t.RecursionLevel + 1} and ParentTableName='{t.TblName}'").ToList();

                foreach (var id in insIds)
                {
                    var insData = new Dictionary<string, string>();
                    insData = db.Query<AbstTable>("select Name, Value from AbstTable where parentId=@0", id).ToDictionary(key => key.Name??t.TblName, Val => Val.Value);
                    
                    StringBuilder insStmt = new StringBuilder($"INSERT INTO {t.TblName}_{t.RecursionLevel} ({t.TblName}Id");
                    StringBuilder insValues = new StringBuilder($" VALUES ({id}");
                    t.TblFields.ForEach(f => {
                        insStmt.Append($"{f.Name}_{f.Type}, ");
                        insValues.Append($"'{((insData.ContainsKey(f.Name)) ? insData[f.Name]: "")}', ");
                    });

                    insStmt.Append($"[{t.ParentTblName}_Id])");
                    //insStmt.Replace(",", ")", insStmt.ToString().LastIndexOf(","), 1); //remove last ","                    
                    insValues.Append($"{id})");

                    t.InsertSQL.Add(insStmt.Append(insValues.ToString()).ToString());
                }
            });

            tableDefs.OrderBy(o => o.RecursionLevel).ToList().ForEach(t => {
                t.InsertSQL.ForEach(sq =>
                {
                    db.Execute(sq);
                });
            });
        }

        private List<TableDef> MakeSQLTables()
        {
            //Fetch the list of potential tables to create
            var tableDefs = db.Query<TableDef>("select [Name] as TblName,recursionLevel from AbstTable where [type]=0 and " +
                "AbstId in (Select distinct ParentId from AbstTable where type<>1) " + //1 is attributes 
                "group by[Name], recursionLevel").ToList();

            //Fetch the list of fields for each table and then get the table parent
            tableDefs.ForEach(t =>
            {
                t.TblFields = db.Fetch<FieldLst>("select Name, Type, max(len([Value])) as MaxFieldLen from " +
                $"(select ISNULL(Name, '{t.TblName}') as Name, Type, Value from AbstTable where ParentTableName = '{t.TblName}' and recursionLevel = {t.RecursionLevel+ 1} " +//n + 1 for child elements
                "union " +
                $"select Name, Type, Value from AbstTable where AttrElemId in (select AbstId from AbstTable where Name = '{t.TblName}' and recursionLevel = {t.RecursionLevel})) as tmpTbl " + //n for attributes
                "group by Name,Type " +
                "having max(len([Value])) > 0");

                t.ParentTblName = db.ExecuteScalar<string>($"select top 1 CONCAT(ParentTableName,'_',{t.RecursionLevel - 1}) as ParentTableName from AbstTable where Name='{t.TblName}' and recursionLevel={t.RecursionLevel}");
            });

            //Some Parents may have only other parents as their children. They have no leaf or attribute nodes. Hence dont make tables from them.
            //Generate the Create Table statements
            tableDefs.Where(t => t.FieldCnt > 0).ToList().ForEach(t =>
            {
                StringBuilder createSQL = new StringBuilder($"CREATE TABLE {t.TblName}_{t.RecursionLevel} (");  //Postfix table name with recursion level so that kid tables with the same name wont clash

                createSQL.Append($"[{t.TblName}Id] INT NOT NULL PRIMARY KEY, "); //make PK

                //Fill table fields
                t.TblFields.ForEach(f => {
                    createSQL.Append($"[{f.Name}_{f.Type}] VARCHAR({f.MaxFieldLen}) NULL, "); //Postfix fields with numbers according to EnumFldType
                });

                //make FK to Parent for all below the kids of root. 
                if (t.RecursionLevel>1)
                { 
                    string pureParentName = t.ParentTblName.Substring(0, t.ParentTblName.LastIndexOf('_'));
                    createSQL.Append($"[{t.ParentTblName}_Id] INT NULL, " +
                        $" CONSTRAINT [FK_{t.TblName}_{t.ParentTblName}] FOREIGN KEY ([{t.ParentTblName}_Id]) REFERENCES [{t.ParentTblName}]([{pureParentName}Id])"); 
                }
                createSQL.Append(" )"); //Closing bracket of create stmt
                t.CreateSQL = createSQL.ToString();
            });

            //With our statements ready, lets now make our tables
            tableDefs.OrderBy(o=>o.RecursionLevel).ToList().ForEach(t => {
                db.Execute(t.CreateSQL);
            });

            return tableDefs;
        }

        public int LogNode(XElement elem, int level, int parentId, string elemType)
        {
            string elemName = elem.Name.ToString().Replace("\r\n\t", "");
            //LogParent the parents data
            int newId = (int)db.Insert(new AbstTable
            {
                Name = elemName,
                recursionLevel = level,
                Type = GetTypeInt(elemType),
                ParentTableName = elem.Parent?.Name.LocalName??"",
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
                        Name = x.Name.LocalName,
                        Value = x.Value,
                        recursionLevel = level,
                        Type = GetTypeInt("Attribute"),
                        ParentTableName = elem.Parent.Name.LocalName,
                        ParentId = parentId,
                        AttrElemId = newId
                    });
                });
            }

            return newId;
        }

        public void EAXmlDig(XElement input, int level, int parentId)
        {
            int newId = LogNode(input, level++, parentId, "Element");
            
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
                        recursionLevel = level,
                        Type = GetTypeInt("Text"),
                        ParentTableName = input.Name.LocalName,
                        ParentId = newId,
                        Value = input.LastNode.ToString().Replace("\r\n\t", "")
                    });
                }
            }
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
            Root
        }


        class FieldLst
        {
            public string Name { get; set; }
            public int Type { get; set; }
            public int MaxFieldLen { get; set; }
        }

        class TableDef
        {
            public string TblName { get; set; }
            public int RecursionLevel { get; set; }
            public string ParentTblName { get; set; }
            public int ParntTblId { get; set; }
            public List<FieldLst> TblFields { get; set; }
            public int FieldCnt { get { return TblFields.Count; }  }
            public string CreateSQL { get; set; }
            public List<string> InsertSQL { get; set; }
        }

        // GET: Clients
        [EAAuthorize(FunctionName = "Bank Details", Writable = false)]
        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<Bank>(page, " * ","Banks Where BankName like '%" + AN + "%'"));
        }



        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Bank Details", Writable = true)]
        public ActionResult Manage(int? id)
        {
           // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");
            
            return View(base.BaseCreateEdit<Bank>(id, "BankID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Bank Details", Writable = true)]
        public ActionResult Manage([Bind(Include = "BankID,BankName,AccNo, Address")] Bank item)
        {
            return base.BaseSave<Bank>(item, item.BankID > 0);
        }

        [EAAuthorize(FunctionName = "Bank Details", Writable = true)]
        public ActionResult BankAccount(int? id,  int? EID)
        {
            var rec = base.BaseCreateEdit<BankAccount>(EID, "BankAccountID")??new BankAccount { BankID = id, TDate= DateTime.Today.Date }; //default to todays date

            ViewBag.BankID = id;
            ViewBag.BankRec = BaseCreateEdit<Bank>(id, "BankID");
            if (EID != null)
            {
                ViewBag.UserName = db.ExecuteScalar<string>("Select UserName From AspNetUsers Where Id=@0",rec.UserID);
                ViewBag.SupplierName = db.ExecuteScalar<string>("Select SupplierName From Supplier Where SupplierID=@0", rec.SupplierID);

            }
            
            ViewBag.BA = db.Fetch<BankAccountDets>($"Select ba.BankAccountID,ba.TDate,AmountIn,AmountOut,ba.SRID, sr.BookingNo,TransNo,ba.UserID, s.SupplierName,anu.UserName,Comment From BankAccount ba " +
                $" inner join ServiceRequest sr on sr.SRID=ba.SRID left join AspNetUsers anu on anu.Id=ba.UserID left join Supplier s on s.SupplierID = ba.SupplierID where ba.BankID ='{id}'");
            return View(rec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Bank Details", Writable = true)]
        public ActionResult BankAccount([Bind(Include = "BankAccountID,TDate,AmountIn,AmountOut,SRID,TransNo,UserID,SupplierID,Comment,BankID")] BankAccount item)
        {   
            base.BaseSave<BankAccount>(item, item.BankAccountID > 0);
            return RedirectToAction("BankAccount", new { id = item.BankID});

        }
        public ActionResult AutoCompleteAgent(string term)
        {
            var filteredItems = db.Fetch<AspNetUser>($"Select * from AspNetUsers Where UserName like '%{term}%'").Select(c => new { id = c.Id, value = c.UserName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AutoCompleteSup(string term)
        {
            var filteredItems = db.Fetch<Supplier>($"Select * from Supplier Where SupplierName like '%{term}%'").Select(c => new { id = c.SupplierID, value = c.SupplierName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
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
