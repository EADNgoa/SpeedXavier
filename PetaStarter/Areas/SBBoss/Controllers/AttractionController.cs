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
    public class AttractionController : EAController
    {
        // GET: Clients
        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<Attraaction>(page, " * ","Attraaction Where AttractionName like '%" + AN + "%'"));
        }



        // GET: Clients/Create
        public ActionResult Manage(int? id)
        {              
            return View(base.BaseCreateEdit<Attraaction>(id, "AttractionID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "AttractionID,AttractionName,Description")] Attraaction item, System.Web.HttpPostedFileBase UploadedFile)
        {
            item.ImagePath = SaveImage(new PetaPoco.Sql("Select ImagePath from Attraaction where AttractionID=@0", item.AttractionID), "Attraction", item.AttractionID, UploadedFile);
            return base.BaseSave<Attraaction>(item, item.AttractionID > 0);
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
