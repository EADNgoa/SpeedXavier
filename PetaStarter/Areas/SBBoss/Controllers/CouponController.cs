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
    public class CouponController : EAController
    {
        // GET: Clients
        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<DiscountCoupon>(page, " * ","DiscountCoupon Where CouponCode like '%" + AN + "%'"));
        }



        // GET: Clients/Create
        public ActionResult Manage(int? id)
        {
           // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");
            
            return View(base.BaseCreateEdit<DiscountCoupon>(id, "DiscountCouponID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "DiscountCouponID,CouponCode,ValidFrom,ValidTo,Amount,Perc")] DiscountCoupon item)
        {            
            return base.BaseSave<DiscountCoupon>(item, item.DiscountCouponID > 0);
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
