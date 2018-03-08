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
    public class BookingController : EAController
    {
        // GET: Clients
        public ActionResult Index(int? page ,int? AN ,int? confirm,int? cancel)
        {
            if (AN !=null) page = 1;
            if(confirm!=null) db.Execute($"Update Booking Set StatusID ={2} where BookingID ={confirm}");
            if (cancel != null) db.Execute($"Update Booking Set StatusID ={3} where BookingID ={cancel}");
            var bookings = db.Fetch<BookingRec>("Select * from Booking b inner join BookingStatus bs on b.StatusID = bs.BookingStatusID  inner join AspNetUsers anu on b.UserID=anu.Id");
            bookings.ForEach(b =>
            {
                b.bookdets = db.Query<BookingDets>($"Select * from BookingDetail bd inner join OptionType ot on bd.OptionTypeID = ot.OptionTypeID where BookingID={b.BookingID}");
                b.Customer = db.Query<CustomerDets>($"Select * from Customer c inner join BookedCustomer bc on c.CustomerID = bc.CustomerID inner join Booking b on bc.BookingID = b.BookingID Where bc.BookingID = {b.BookingID}");
            });
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(bookings.ToList().ToPagedList(pageNumber, pageSize));

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
