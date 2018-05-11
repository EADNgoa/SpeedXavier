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
    public class LeaveEntitlementController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Leave Entitlement", Writable = false)]
        public ActionResult Index(int? page, string AN)
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<LeaveEntitlementDets>(page, " * ", "LeaveEntitlement le inner join LeaveType lt on lt.LeaveTypeID =le.LeaveTypeID Where LeaveYear like '%" + AN + "%'"));
        }




        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Leave Entitlement", Writable = true)]
        public ActionResult Manage(int? id)
        {
            ViewBag.LeaveTypeID = new SelectList(db.Fetch<LeaveType>("Select LeaveTypeID,LeaveTypeName from LeaveType"), "LeaveTypeID", "LeaveTypeName");
            return View(base.BaseCreateEdit<LeaveEntitlement>(id, "LeaveEntitlementID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Leave Entitlement", Writable = true)]
        public ActionResult Manage([Bind(Include = "LeaveEntitlementID,LeaveYear,LeaveTypeID,LeaveDays")] LeaveEntitlement item)
        {
            return base.BaseSave<LeaveEntitlement>(item, item.LeaveEntitlementID > 0);
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
