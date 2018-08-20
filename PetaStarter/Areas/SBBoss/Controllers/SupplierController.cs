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
    public class SupplierController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Supplier", Writable = false)]
        public ActionResult Index(int? page ,string SP )
        {
            if (SP?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<Supplier>(page, " * ","Supplier Where SupplierName like '%" + SP + "%'"));
        }

        public ActionResult PaymentList(int? page ,int? SupplierID,DateTime? fd,DateTime? td)
        {
            page = 1;
            ViewBag.SupplierID = SupplierID;
            ViewBag.SupplierName = db.ExecuteScalar<string>("Select SupplierName from Supplier Where SupplierID=@0", SupplierID);

            var sql = new PetaPoco.Sql("Select (Select Sum(Amount) from RP_SR rs where rs.SRID = sd.SRID)as PaidAmt,Coalesce(sum(sd.Cost),0) as OA,s.SupplierName as UserName,sd.SRID,BookingNo " +
                "from SRdetails sd inner join ServiceRequest sr on sd.SRID=sr.SRID inner join Supplier s on s.SupplierID = sd.SupplierID  left join RP_SR rs on rs.SRID =sd.SRID  " +
                "left join RPdets rd on rd.RPDID =rs.RPDID where sd.SupplierID=@0", SupplierID,true);
            if (fd != null && td != null)
                sql.Append($" and cast(Cdate as Date) Between '{fd:yyyy-MM-dd}' and  '{td:yyyy-MM-dd}'");

            sql.Append(" Group By sd.SRID,sr.BookingNo, s.SupplierName");
            var rec = db.Query<SRBooking>(sql).ToList();


            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(rec.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult AutoCompleteSup(string term)
        {
            var filteredItems = db.Fetch<Supplier>($"Select * from Supplier Where SupplierName like '%{term}%'").Select(c => new { id = c.SupplierID, value = c.SupplierName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Supplier", Writable = true)]
        public ActionResult Manage(int? id)
        {
           // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");
            
            return View(base.BaseCreateEdit<Supplier>(id, "SupplierID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Supplier", Writable = true)]

        public ActionResult Manage([Bind(Include = "SupplierID,SupplierName")] Supplier item)
        {
            return base.BaseSave<Supplier>(item, item.SupplierID > 0);
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
