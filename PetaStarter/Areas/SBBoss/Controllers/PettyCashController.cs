using Microsoft.AspNet.Identity;
using PagedList;
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
    public class PettyCashController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Petty Cash", Writable = false)]
        public ActionResult Index(int? page, DateTime? dt)
        {
            page = 1;
            return View("Index", base.BaseIndex<PettyCash>(page, " * ", "PettyCash Where Tdate Like '%" + dt + "%' Order By CashInHandRegID Desc"));
        }



        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Petty Cash", Writable = false)]
        public ActionResult Manage(int? id)
        {
           // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");
            
            return View(base.BaseCreateEdit<PettyCash>(id, "CashInHandRegID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Petty Cash", Writable = false)]
        public ActionResult Manage([Bind(Include = "CashInHandRegID,Tdate,NameAndDesg,CashToDeclareStart,CashToDeclareEnd,DetailsOfCashExp,Remarks")] PettyCash item)
        {
            string UN = db.ExecuteScalar<string>("Select UserName From AspNetUsers where Id=@0",User.Identity.GetUserId());
            item.NameAndDesg = UN;
            item.Tdate = DateTime.Now;
            return base.BaseSave<PettyCash>(item, item.CashInHandRegID > 0);
        }
        [EAAuthorize(FunctionName = "Petty Cash", Writable = true)]
        public ActionResult Details(int? id, int? EID)
        {
            var rec = base.BaseCreateEdit<PCdetail>(EID, "PCDID");
            if(rec?.PettyCashID > 0)
            {
                ViewBag.SupplierName = db.ExecuteScalar<string>("Select SupplierName from Supplier where SupplierID = @0",rec.SupplierID);
            }
            ViewBag.PCID = id;
            ViewBag.Title = "Petty Cash Details";

            ViewBag.PC = db.FirstOrDefault<PettyCash>("Select * From PettyCash Where CashInHandRegID=@0", id);
            ViewBag.PCD = db.Fetch<PCdetailDets>($"Select * From PCdetails pd inner join Supplier s on s.SupplierID = pd.SupplierID where PettyCashID ='{id}' ORDER By PCDID Desc");
            return View(rec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Petty Cash", Writable = true)]
        public ActionResult Details([Bind(Include = "PCDID,PettyCashID,Category,Details,Cost,SupplierID,InvoiceNo,BillImage,SRID")] PCdetail item, System.Web.HttpPostedFileBase UploadedFile)
        {
            item.BillImage = SaveImage(new PetaPoco.Sql("Select BillImage from PCdetails where PCDID=@0", item.PCDID), "PCdetails", item.PCDID, UploadedFile);
            base.BaseSave<PCdetail>(item, item.PCDID > 0);
            return RedirectToAction("Details", new { id = item.PettyCashID});
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
