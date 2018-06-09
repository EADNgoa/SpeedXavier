using Speedbird.Controllers;
using System;
//using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
//using System.Web;
using System.Web.Mvc;


namespace Speedbird.Areas.SBBoss.Controllers
{
    public class BanksController : EAController
    {
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
        public ActionResult Manage([Bind(Include = "BankID,BankName")] Bank item)
        {
            return base.BaseSave<Bank>(item, item.BankID > 0);
        }

        [EAAuthorize(FunctionName = "Bank Details", Writable = true)]
        public ActionResult BankAccount(int? id,  int? EID)
        {
            var rec = base.BaseCreateEdit<BankAccount>(EID, "BankAccountID");
            ViewBag.BankID = id;
            if (EID != null)
            {
                ViewBag.UserName = db.ExecuteScalar<string>("Select UserName From AspNetUsers Where Id=@0",rec.UserID);
                ViewBag.SupplierName = db.ExecuteScalar<string>("Select SupplierName From Supplier Where SupplierID=@0", rec.SupplierID);

            }
            ViewBag.Banks = db.FirstOrDefault<Bank>("Select * From Banks Where BankID=@0", id);
            ViewBag.BA = db.Fetch<BankAccountDets>($"Select * From BankAccount ba inner join AspNetUsers anu on anu.Id=ba.UserID inner join Supplier s on s.SupplierID = ba.SupplierID inner join Banks bs on bs.BankID = ba.BankID where ba.BankID ='{id}'");
            return View(rec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Bank Details", Writable = true)]
        public ActionResult BankAccount([Bind(Include = "BankAccountID,TDate,AmountIn,AmountOut,SRID,TransNo,UserID,SupplierID,Comment,BankID")] BankAccount item)
        {
            item.TDate = DateTime.Now;
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
