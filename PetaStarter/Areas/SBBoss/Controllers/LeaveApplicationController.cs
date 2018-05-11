using Microsoft.AspNet.Identity;
using Speedbird.Controllers;
using System;
using System.Collections.Generic;
//using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
//using System.Web;
using System.Web.Mvc;
using static PetaStarter.Areas.SBBoss.Models.DataTablesModels;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class LeaveApplicationController : EAController
    {
        [EAAuthorize(FunctionName = "Leave Application", Writable = false)]
        public ActionResult Index(int? page, DateTime? AN)
        {
            page = 1;
            return View("Index", base.BaseIndex<LeaveApplicationDets>(page, " * ", $"LeaveApplications la inner join LeaveType lt on lt.LeaveTypeID=la.LeaveTypeID inner join BookingStatus bs on la.StatusID =bs.BookingStatusID Where ApplicationDate like '%" + AN + "%'"));
        }

        [EAAuthorize(FunctionName = "Leave Application", Writable = true)]
        public ActionResult Manage(int? id)
        {
            ViewBag.LeaveTypeID = new SelectList(db.Fetch<LeaveType>("Select LeaveTypeID,LeaveTypeName from LeaveType"), "LeaveTypeID", "LeaveTypeName");
            return View(base.BaseCreateEdit<LeaveApplication>(id, "LeaveApplicationID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Leave Application", Writable = true)]
        public ActionResult Manage([Bind(Include = "LeaveApplicationID,ApplicationDate,UserID,LeaveTypeID,LeaveStartDate,NoOfDays,StatusID,StatusBy,StatusDate")] LeaveApplication item)
        {
            item.UserID = User.Identity.GetUserId();
            int DaysAlowed = db.ExecuteScalar<int>("Select LeaveDays from LeaveEntitlement where LeaveTypeID=@0 and LeaveYear=@1", item.LeaveTypeID,DateTime.Now.Year);
            int? LeaveBal = db.FirstOrDefault<int>("Select LeaveDays From LeaveBalance Where UserID=@0 and LeaveTypeID=@1 and LeaveYear =@2 ",item.UserID,item.LeaveTypeID,DateTime.Now.Year);
            var balance =  item.NoOfDays + LeaveBal ??0;
            if(balance<=DaysAlowed)
            {
                item.ApplicationDate = DateTime.Now;
                item.StatusID = 1;
                return base.BaseSave<LeaveApplication>(item, item.LeaveApplicationID > 0);
            }
            ViewBag.LeaveTypeID = new SelectList(db.Fetch<LeaveType>("Select LeaveTypeID,LeaveTypeName from LeaveType"), "LeaveTypeID", "LeaveTypeName",item.LeaveTypeID);
            return View();
        }
        [EAAuthorize(FunctionName = "LeaveApprove", Writable = true)]
        public ActionResult ConfirmOrCancel(int? page, DateTime? AN,int? Approve, int? Reject)
        {
            page = 1;
            var userID= User.Identity.GetUserId();
            LeaveApplication ExistingRec = new LeaveApplication();
            if (Approve != null || Reject != null)
            {
                ExistingRec = db.FirstOrDefault<LeaveApplication>("Select * from LeaveApplications where LeaveApplicationID=@0", Approve ?? Reject);
                ExistingRec.StatusBy = userID;
                ExistingRec.StatusDate = DateTime.Now;
            }
            if (Approve != null)
            {            
                ExistingRec.StatusID = 2;
                var Bal = db.FirstOrDefault<LeaveBalance>("Select * from LeaveBalance Where LeaveTypeID=@0 and LeaveYear=@1 and UserID=@2",ExistingRec.LeaveTypeID,ExistingRec.ApplicationDate.Value.Year,ExistingRec.UserID);
                if (Bal == null)
                {
                    db.Insert(new LeaveBalance {UserID=ExistingRec.UserID,LeaveTypeID=(int)ExistingRec.LeaveTypeID,LeaveYear=DateTime.Now.Year,LeaveDays=ExistingRec.NoOfDays });
                }
                else
                {
                    Bal.LeaveDays += ExistingRec.NoOfDays;
                    db.Update(Bal);
                }
            }
            if (Reject != null)
            {
                ExistingRec.StatusID = 3;

            }
            db.Update(ExistingRec);
            return View("ConfirmOrCancel", base.BaseIndex<LeaveApplicationDets>(page, " * ", $"LeaveApplications la inner join LeaveType lt on lt.LeaveTypeID=la.LeaveTypeID inner join BookingStatus bs on la.StatusID =bs.BookingStatusID Where ApplicationDate like '%" + AN + "%'"));
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
