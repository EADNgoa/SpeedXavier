﻿using Speedbird.Controllers;
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
    public class GuideLanguageController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Guide Language", Writable = false)]

        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<GuideLanguage>(page, " * ","GuideLanguage Where GuideLanguageName like '%" + AN + "%'"));
        }



        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Guide Language", Writable = true)]

        public ActionResult Manage(int? id)
        {
           // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");
            
            return View(base.BaseCreateEdit<GuideLanguage>(id, "GuideLanguageID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Guide Language", Writable = true)]
        public ActionResult Manage([Bind(Include = "GuideLanguageID,GuideLanguageName")] GuideLanguage item)
        {            
            return base.BaseSave<GuideLanguage>(item, item.GuideLanguageID > 0);
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
