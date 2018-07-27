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


namespace Speedbird.Areas.SBBoss.Controllers
{
    [Authorize]
    public class AdminController : EAController
    {
        // GET: Clients
        
        public ActionResult Index()
        {
            var userid = User.Identity.GetUserId();
            ViewBag.OpenSR = db.Query<ServiceRequestDets>("Select Top 5 FName,SName,ServiceTypeID,Tdate,EnquirySource from ServiceRequest sr inner join Customer c on c.CustomerID =sr.CustID where sr.SRStatusID = @0", (int)SRStatusEnum.New).ToList();
            var services = db.Query<SRdetailDets>("Select sd.TDate Tdate,sr.SRID,sr.ServiceTypeID,Cost,SellPrice from ServiceRequest sd inner join SRdetails sr on sr.SRID=sd.SRID where EmpID=@0", userid).ToList();
            services.ForEach(s =>
            {
                var tax = db.ExecuteScalar<decimal?>("Select Percentage From Taxes Where ServiceTypeID=@0 and WEF<GetDate() order by WEF desc ", s.ServiceTypeID) ?? 0;
                s.Tax = s.SellPrice * tax / 100;
                var st = (ServiceTypeEnum)s.ServiceTypeID;
                var commision = db.Fetch<ServiceCommision>($"Select Perc,Amount  From ServiceCommision Where ServiceName='{st}' ");
                commision.ForEach(c => {
                    if (c.Perc != null)
                    {
                        s.Commision = c.Perc ?? 0;
                        var tot = s.SellPrice * s.Commision / 100;
                        s.Total = tot + s.SellPrice + s.Tax;
                    }
                    else if (c.Amount != null)
                    {
                        s.Commision = c.Amount ?? 0;
                        s.Total = s.SellPrice + s.Tax + s.Commision;
                    }

                });


            });
          
            IEnumerable<SRdetailDets> sRdetailDet=  services.GroupBy(s=>new { s.SRID ,s.Tdate}).Select(group=>new SRdetailDets { Total=group.Sum(a=>a.Commision), SellPrice = group.Sum(a => a.SellPrice),SRID=group.Key.SRID ,Tdate=group.Key.Tdate}).OrderByDescending(s=>s.SRID).Where(s=>s.Total>0).Take(5) ;
            ViewBag.Services = sRdetailDet;

            ViewBag.UserLogs = db.Query<UserLogRecDets>("Select Top 5 * from UserLogRec Where UserID=@0 Order By UserLogID Desc",userid).ToList();
            ViewBag.LvApp = db.Query<LeaveApplicationDets>("Select Top 5 LeaveTypeName,LeaveStartDate,NoOfDays,ApplicationDate From LeaveApplications la inner join LeaveType lt on lt.LeaveTypeID=la.LeaveTypeID where UserID=@0 and StatusID=@1 and Year(LeaveStartDate)=@2",userid,LeaveApplicationStatusEnum.Approved,DateTime.Now.Year).ToList();
            return View("Index");
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
