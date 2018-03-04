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
    public class BookingController : EAController
    {
        // GET: Clients
        public ActionResult Index(int? page ,int? AN ,int? confirm,int? cancel)
        {
            if (AN !=null) page = 1;
            if(confirm!=null) db.Execute($"Update Booking Set StatusID ={2} where BookingID ={confirm}");
            if (cancel != null) db.Execute($"Update Booking Set StatusID ={3} where BookingID ={cancel}");
            return View("Index", base.BaseIndex<BookingDets>(page, " * ","Booking b inner join BookingStatus bs on b.StatusID = bs.BookingStatusID inner join BookingDetail bd on bd.BookingID = b.BookingID inner join BookedCustomer bc on bc.BookingID = b.BookingID inner join Customer c on bc.CustomerID = c.CustomerID Where b.BookingID like '%" + AN + "%'"));
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
