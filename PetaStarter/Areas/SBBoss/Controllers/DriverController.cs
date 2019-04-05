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
    public class DriverController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Driver", Writable = false)]

        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<Driver>(page, " * ","Driver Where DriverName like '%" + AN + "%' order by DriverName"));
        }

        public ActionResult PaymentList(int? page, int? DriverID, DateTime? fd, DateTime? td)
        {
            page = 1;
            ViewBag.DriverID = DriverID;
            ViewBag.DriverName = db.ExecuteScalar<string>("Select DriverName from Driver Where DriverID=@0", DriverID);

            var sql = new PetaPoco.Sql("Select (Select Sum(Amount) from DRP_SR rs where rs.SRID = sd.SRID)as PaidAmt,Coalesce(sum(sd.Cost),0) as OA,d.DriverName as UserName,sd.SRID , BookingNo from SRdetails sd inner join ServiceRequest sr on sd.SRID=sr.SRID inner join Driver d on d.DriverID = sd.DriverID  left join DRP_SR rs on rs.SRID =sd.SRID  left join RPdets rd on rd.RPDID =rs.DRPDID where sd.DriverID=@0", DriverID);
            if (fd != null && td != null)
                sql.Append($" and cast(Cdate as Date) Between '{fd:yyyy-MM-dd}' and  '{td:yyyy-MM-dd}'");

            sql.Append(" Group By sd.SRID,sr.BookingNo,d.DriverName");
            var rec = db.Query<SRBooking>(sql).ToList();


            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(rec.ToPagedList(pageNumber, pageSize));
        }


        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Driver", Writable = true)]
        public ActionResult Manage(int? id)
        {
            ViewBag.GeoId = db.Query<GeoTree>("Select * from GeoTree where GeoTreeId = (Select LocationId from driver where DriverID=@0)", id ?? 0).Select(sl => new SelectListItem { Text = sl.GeoName, Value = sl.GeoTreeID.ToString(), Selected = true });

            return View(base.BaseCreateEdit<Driver>(id, "DriverID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Driver", Writable = true)]
        public ActionResult Manage([Bind(Include = "DriverID,DriverName,Phone,Address,EmerContactName,EmerContactNo, CarModel, LocationId")] Driver item)
        {
            return base.BaseSave<Driver>(item, item.DriverID > 0);
        }


        [EAAuthorize(FunctionName = "Driver", Writable = true)]
        public ActionResult CarManage(int? id, int? CarId)
        {
            if (CarId.HasValue)
                id = db.SingleOrDefault<DriversCar>(CarId).DriverId;

            ViewBag.DriverName = db.SingleOrDefault<Driver>(id).DriverName;
            ViewBag.DriverId = id;
            ViewBag.ExistingCars = db.Query<DriversCar>("where DriverId=@0", id);

            return View(base.BaseCreateEdit<DriversCar>(CarId, "CarID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Driver", Writable = true)]
        public ActionResult CarManage([Bind(Include = "CarId, DriverId,CarBrand,Model,DateOfPurchase,PlateNo,RCBookNo,InsuranceEndDate,InsuranceCompany")] DriversCar item)
        {
            return base.BaseSave<DriversCar>(item, item.CarId > 0,"CarManage",new { Id= item.DriverId });
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
