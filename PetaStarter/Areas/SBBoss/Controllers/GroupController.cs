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
    public class GroupController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Group", Writable = false)]
        public ActionResult Index(int? page ,string gn )
        {
            if (gn?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<Group>(page, " * ","Groups Where GroupName like '%" + gn + "%'"));
        }



        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Group", Writable = true)]
        public ActionResult Manage(int? id)
        {
           // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");
            
            return View(base.BaseCreateEdit<Group>(id, "GroupID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Activity", Writable = true)]

        public ActionResult Manage([Bind(Include = "GroupID,GroupName")] Group item)
        {
            return base.BaseSave<Group>(item, item.GroupID > 0);
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
