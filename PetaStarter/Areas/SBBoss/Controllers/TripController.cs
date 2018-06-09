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
    public class TripController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Car Trip", Writable = false)]
        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<OwnCarTripDets>(page, " * ","OwnCarTrip o inner join Driver d on d.DriverID = o.DriverID inner join CarBike c on c.CarBikeID = o.CarBikeID Where CarBikeName like '%" + AN + "%'"));
        }



        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Car Trip", Writable = true)]
        public ActionResult Manage(int? id)
        {
           ViewBag.DriverID = new SelectList(db.Fetch<Driver>("Select DriverID,DriverName from Driver"), "DriverID", "DriverName");
            ViewBag.CarBikeID = new SelectList(db.Fetch<CarBike>("Select CarBikeID,CarBikeName from CarBike"), "CarBikeID", "CarBikeName");

            return View(base.BaseCreateEdit<OwnCarTrip>(id, "OwnCarTripID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Car Trip", Writable = true)]
        public ActionResult Manage([Bind(Include = "OwnCarTripID,CarBikeID,DriverID,TripStart,StartKms,EndKms")] OwnCarTrip item)
        {
            return base.BaseSave<OwnCarTrip>(item, item.OwnCarTripID > 0);
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
