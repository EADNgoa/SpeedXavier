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
            return View("Index", base.BaseIndex<Driver>(page, " * ","Driver Where DriverName like '%" + AN + "%'"));
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
