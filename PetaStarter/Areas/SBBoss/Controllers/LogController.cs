using Microsoft.AspNet.Identity;
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
    public class LogController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "LogDetails", Writable = false)]
        public ActionResult Index(int? page ,DateTime? dt )
        {
            page = 1;
            return View("Index", base.BaseIndex<UserLogRec>(page, " * ", $"UserLogRec Where UserID='{User.Identity.GetUserId()}' and Cast(LogIn as Date) like '%" + dt + "%'"));
        }
        [EAAuthorize(FunctionName = "BossLogDetails", Writable = false)]
        public ActionResult BossIndex(int? page, DateTime? dt)
        {
            page = 1;
            return View("BossIndex", base.BaseIndex<UserLogRecDets>(page, " * ", $"UserLogRec u inner join AspNetUsers anu on u.UserID = anu.Id  Where Cast(LogIn as Date)='{DateTime.Now.Date}' and Cast(LogIn as Date) like '%" + dt + "%'"));

        }


        // GET: Clients/Create
        public ActionResult Manage(int id)
        {
            // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");

            return View(base.BaseCreateEdit<UserLogRec>(id, "UserLogID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "UserLogID,UserID,LogIn,LogOut")] UserLogRec item)
        {
            return base.BaseSave<UserLogRec>(item, item.UserLogID > 0);
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
