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
    public class CouponController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Coupon", Writable = false)]

        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<DiscountCoupon>(page, " * ","DiscountCoupon Where CouponCode like '%" + AN + "%'"));
        }



        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Coupon", Writable = true)]

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
        [EAAuthorize(FunctionName = "Coupon", Writable = true)]

        public ActionResult Manage([Bind(Include = "DiscountCouponID,CouponCode,ValidFrom,ValidTo,Amount,Perc")] DiscountCoupon item)
        {            
            return base.BaseSave<DiscountCoupon>(item, item.DiscountCouponID > 0);
        }

        // GET: Clients
        [EAAuthorize(FunctionName = "User Rights", Writable = false)]
        public ActionResult Agents(int? page, int? confirm, int? cancel)
        {
            if (confirm != null) db.Execute($"Update AgentDiscount Set IsApproved ='{true}' Where AgentDiscountID ={confirm}");
            if (cancel != null) db.Execute($"Update AgentDiscount Set IsApproved ='{false}' Where AgentDiscountID ={cancel}");
            var Agent = db.Fetch<AgentDiscDets>($"Select * from AgentDiscount ad inner join AspNetUsers anu on ad.UserID =anu.Id Order By AgentDiscountID Desc");
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(Agent.ToList().ToPagedList(pageNumber, pageSize));
        }
        [EAAuthorize(FunctionName = "User Rights", Writable = true)]
        public ActionResult AgentManage(int? id)
        {
            // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");
            var EnumValues = from ServiceTypeEnum c in Enum.GetValues(typeof(ServiceTypeEnum)) select new { ID = c, Name = c.ToString() };
            ViewBag.ServiceTypeID = new SelectList(EnumValues, "ID", "Name");
            return View(base.BaseCreateEdit<AgentDiscount>(id, "AgentDiscountID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "User Rights", Writable = true)]
        public ActionResult AgentManage([Bind(Include = "AgentDiscountID,UserID,ServiceTypeID,Amount,Percentage,IsApproved")] AgentDiscount item, ServiceTypeEnum? service)
        {
            item.ServiceTypeID = (int)service;
            base.BaseSave<AgentDiscount>(item, item.AgentDiscountID > 0);
            return RedirectToAction("Agents");
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
