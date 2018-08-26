﻿using Microsoft.AspNet.Identity.Owin;
using PagedList;
using Speedbird.Controllers;
using Speedbird.Models;
using System;
//using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
//using System.Web;
using System.Web.Mvc;


namespace Speedbird.Areas.SBBoss.Controllers
{
    public class AgentController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Agents", Writable = false)]
        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<AspNetUser>(page, " * ",$"AspNetUsers Where UserType={(int)UserTypeEnum.Agent} and realName like '%" + AN + "%'"));
        }

        public ActionResult PaymentList(int? page, string Id, DateTime? fd, DateTime? td)
        {
            page = 1;
            ViewBag.Id = Id;
            ViewBag.AgentName = db.ExecuteScalar<string>("Select UserName from AspNetUsers Where Id = @0", Id);

            var sql = new PetaPoco.Sql("Select distinct rd.RPDID,sr.SRID, sr.BookingNo, anu.UserName,sum(rs.Amount) as PaidAmt ,(select Coalesce(sum(SellPrice),0) From SRdetails Where SRID =sr.SRID) as OA from ServiceRequest sr inner join AspNetUsers anu on anu.Id = sr.AgentID left join  RP_SR rs on rs.SRID = sr.SRID left join RPdets rd on rd.RPDID = rs.RPDID Where sr.AgentID Is Not Null  and AgentID=@0", Id);
            if (fd != null && td != null)
                sql.Append($" and cast(Cdate as Date) Between '{fd:yyyy-MM-dd}' and  '{td:yyyy-MM-dd}'");

            sql.Append(" Group By sr.SRID,sr.BookingNo,anu.UserName,rd.RPDID");
            var rec = db.Query<SRBooking>(sql).ToList();
            rec= rec.Where(a => a.OA > 0).ToList();
 
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(rec.ToPagedList(pageNumber, pageSize));

        }
        public ActionResult AutoCompleteAgent(string term)
        {
            var filteredItems = db.Fetch<AspNetUser>($"Select * from AspNetUsers Where UserName like '%{term}%'").Select(c => new { id = c.Id, value = c.UserName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Aspnet Identity management bare bones
        /// </summary>
        private ApplicationUserManager _userManager;
        public AgentController()
        {         
        }
        public AgentController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;            
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
               

        [EAAuthorize(FunctionName = "Agents", Writable = true)]
        public ActionResult Manage(string id)
        {            
            return View(db.SingleOrDefault<AspNetUser>($"Select top 1 * from AspNetUsers where Id = '{id}'") );
        }
                
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Agents", Writable = true)]
        public async Task<ActionResult> Manage([Bind(Include = "Id,RealName,email")] AspNetUser model)
        {
            if (model.Id?.Length > 0)
            {
                db.Execute($"Update AspNetUsers Set RealName='{model.RealName}' Where Id='{model.Id}'");
            } else
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                var result = await UserManager.CreateAsync(user, "Agent1Pwd!");

                if (result.Succeeded)
                {
                    db.Execute($"Update AspNetUsers Set UserType='{(int)UserTypeEnum.Agent}' , RealName='{model.RealName}' Where Id='{user.Id}'");
                    var item = new AgentDiscount { UserID = user.Id };
                    db.Insert(item);

                }
            }
                        
                    return RedirectToAction("Index", "Agent");
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
