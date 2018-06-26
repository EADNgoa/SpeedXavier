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
    public class QuestionsController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Questions", Writable = false)]
        public ActionResult Index(int? page ,string Q )
        {
            if (Q?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<Question>(page, " * ","Questions Where FQuestion like '%" + Q + "%'"));
        }



        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Questions", Writable = true)]
        public ActionResult Manage(int? id)
        {
           // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");
            
            return View(base.BaseCreateEdit<Question>(id, "QuestionID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Questions", Writable = true)]

        public ActionResult Manage([Bind(Include = "QuestionID,FQuestion")] Question item)
        {
            return base.BaseSave<Question>(item, item.QuestionID > 0);
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
