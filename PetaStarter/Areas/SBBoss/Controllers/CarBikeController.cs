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
    public class CarBikeController : EAController
    {
        // GET: Clients
        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<CarBikeDets>(page, " * ","CarBike c inner join GeoTree g on c.GeoTreeID = g.GeoTreeID Where CarBikeName like '%" + AN + "%'"));
        }



        // GET: Clients/Create
        public ActionResult Manage(int? id)
        {
           ViewBag.GeoTreeID = new SelectList(db.Fetch<GeoTree>("Select GeoTreeID,GeoName from GeoTree"), "GeoTreeID", "GeoName");          
           return View(base.BaseCreateEdit<CarBike>(id, "CarBikeID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "CarBikeID,CarBikeName,GeoTreeID,Description,NoPax,NoSmallBags,NoLargeBags,HasAc,HasCarrier,InclHelmet")] CarBike item)
        {            
            return base.BaseSave<CarBike>(item, item.CarBikeID > 0);
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
