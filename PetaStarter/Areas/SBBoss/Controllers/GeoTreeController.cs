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
        [EAAuthorize(FunctionName = "Geo Locations", Writable = false)]

        public ActionResult Index(int? GeoId)
        {
            if (GeoId.HasValue && GeoId>0)
            {
                var thisGeo = db.Single<GeoTree>("Select * from GeoTree where GeoTreeId=@0", GeoId);
                ViewBag.GeoName = thisGeo.GeoName;
                ViewBag.GeoGrandParentId = thisGeo.GeoParentID??0;
                ViewBag.GeoParentId = GeoId;
                ViewBag.GeoBrdCrmb = db.Query<GeoTree>("Select * from GetGeoAncestors(@0)", GeoId);
            } else
            {
                ViewBag.GeoParentId = 0;
                ViewBag.GeoName = "Worldwide";
            }
            
            return View("Index", base.BaseIndex<GeoTree>(1, " * ", (GeoId.HasValue && GeoId > 0) ? "GeoTree Where GeoParentId = " + GeoId : "GeoTree Where GeoParentId IS NULL"));
        }



        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Geo Locations", Writable = true)]

        public ActionResult Manage(int? id)
        {   
            return View(base.BaseCreateEdit<GeoTree>(id, "GeoTreeId"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Geo Locations", Writable = true)]

        public ActionResult Manage([Bind(Include = "GeoTreeID,GeoName,GeoParentID,ImagePath")] GeoTree item, System.Web.HttpPostedFileBase UploadedFile)
        {
            item.ImagePath= SaveImage(new PetaPoco.Sql("Select ImagePath from GeoTree where GeoTreeId=@0", item.GeoTreeID),"GeoTree",item.GeoTreeID, UploadedFile);

            //Now save the new file
            return base.BaseSave<GeoTree>(item, true, "Index", new { GeoId = item.GeoParentID });
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Geo Locations", Writable = true)]

        public ActionResult AddGeo([Bind(Include = "GeoParentId,GeoName")] GeoTree item)
        {
            if (item.GeoParentID == 0) item.GeoParentID = null;
            return base.BaseSave<GeoTree>(item, false,"Index",new { GeoId=item.GeoParentID});
        }
        [EAAuthorize(FunctionName = "Geo Locations", Writable = true)]
        public JsonResult GetLocations(string term)
        {
            var locs = db.Fetch<GeoTree>("Select CONCAT(g.GeoName,': ', dbo.GetGeoAncestorsStr(g.GeoTreeID)) as GeoName,g.GeoTreeID from GeoTree g where GeoName like '%" + term + "%'");
            return Json(new { results = locs.Select(a => new { id = a.GeoTreeID, text = a.GeoName.TrimEnd(',',' ') }) }, JsonRequestBehavior.AllowGet);
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
