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
    public class GeoTreeController : EAController
    {
        // GET: Clients
        public ActionResult Index(int? GeoParentId)
        {
            if (GeoParentId.HasValue && GeoParentId>0)
            {
                var thisGeo = db.Single<GeoTree>("Select * from GeoTree where GeoTreeId=@0", GeoParentId);
                ViewBag.GeoName = thisGeo.GeoName;
                ViewBag.GeoGrandParentId = thisGeo.GeoParentID??0;
                ViewBag.GeoParentId = GeoParentId;
                ViewBag.GeoBrdCrmb = db.Query<GeoTree>("Select * from GetGeoAncestors(@0)", GeoParentId);
            } else
            {
                ViewBag.GeoParentId = 0;
                ViewBag.GeoName = "Worldwide";
            }
            return View("Index", base.BaseIndex<GeoTree>(1, " * ", (GeoParentId.HasValue && GeoParentId > 0) ? "GeoTree Where GeoParentId = " + GeoParentId : "GeoTree Where GeoParentId IS NULL"));
        }



        // GET: Clients/Create
        public ActionResult Manage(int? id)
        {   
            return View(base.BaseCreateEdit<GeoTree>(id, "GeoTreeId"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "GeoTreeID,GeoName,GeoParentID")] GeoTree item)
        {            
            return base.BaseSave<GeoTree>(item, true, "Index", new { GeoparentId = item.GeoParentID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGeo([Bind(Include = "GeoParentId,GeoName")] GeoTree item)
        {
            if (item.GeoParentID == 0) item.GeoParentID = null;
            return base.BaseSave<GeoTree>(item, false,"Index",new { GeoparentId=item.GeoParentID});
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
