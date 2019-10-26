using Microsoft.AspNet.Identity.Owin;
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
            return View("Index", base.BaseIndex<AgentView>(page, " a.*,u.id, u.RealName ",$"Agent a, AspNetUsers u Where a.AgentId=u.Id " +
                $"and u.UserType={(int)UserTypeEnum.Agent} and realName like '%" + AN + "%' order by u.RealName"));
        }

        //public ActionResult PaymentList(int? page, string Id, DateTime? fd, DateTime? td)
        //{
        //    page = 1;
        //    ViewBag.Id = Id;
        //    ViewBag.AgentName = db.ExecuteScalar<string>("Select UserName from AspNetUsers Where Id = @0", Id);

        //    var sql = new PetaPoco.Sql("Select distinct rd.RPDID,sr.SRID, sr.BookingNo, anu.UserName,sum(rs.Amount) as PaidAmt ,(select Coalesce(sum(SellPrice),0) " +
        //        "From SRdetails Where SRID =sr.SRID) as OA from ServiceRequest sr inner join AspNetUsers anu on anu.Id = sr.AgentID left join  RP_SR rs on rs.SRID = sr.SRID " +
        //        "left join RPdets rd on rd.RPDID = rs.RPDID Where sr.AgentID Is Not Null  and AgentID=@0", Id);
        //    if (fd != null && td != null)
        //        sql.Append($" and cast(Cdate as Date) Between '{fd:yyyy-MM-dd}' and  '{td:yyyy-MM-dd}'");

        //    sql.Append(" Group By sr.SRID,sr.BookingNo,anu.UserName,rd.RPDID");
        //    var rec = db.Query<SRBooking>(sql).ToList();
        //    rec= rec.Where(a => a.OA > 0).ToList();

        //    int pageSize = 10;
        //    int pageNumber = (page ?? 1);
        //    return View(rec.ToPagedList(pageNumber, pageSize));

        //}

        public ActionResult PaymentList(int? page, string Id, DateTime? fd, DateTime? td)
        {
            page = 1;
            ViewBag.Id = Id;
            ViewBag.AgentName = db.ExecuteScalar<string>("Select UserName from AspNetUsers Where Id = @0", Id);

            var rec = db.Query<Paymentvw>("select py.Tdate,py.Amount,py.Note,py.Type, " +
                "b.BankID,b.BankName,py.ChequeNo,py.TransactionNo from Payments py " +
                "left join Banks b on b.BankID = py.BankID " +
                $"where AgentId = '{Id}'");

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
            var agt = db.SingleOrDefault<AgentView>($"Select u.RealName as RealName,a.* from Agent a, AspNetUsers u where a.AgentId=u.Id and u.Id = '{id}'");
            return View(agt);
        }
                
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Agents", Writable = true)]
        public async Task<ActionResult> Manage([Bind(Include = "AgentId,RealName,ContactName,AgencyName,PhoneNo,Email,Address,State,PAN,GST,RCbook,BkAccNo,BkName,BkIFSC,BkAddress,CreditAmt")] AgentView model)
        {
            using (var transaction = db.GetTransaction())
            {

                try
                {
                    if (model.AgentId?.Length > 0)
                    {
                        db.Execute($"Update AspNetUsers Set RealName='{model.RealName}' Where Id='{model.AgentId}'");
                        db.Update(new Agent
                        {
                            Address = model.Address,
                            State = model.State,
                            AgentId = model.AgentId,
                            BkAccNo = model.BkAccNo,
                            BkAddress = model.BkAddress,
                            BkIFSC = model.BkIFSC,
                            BkName = model.BkName,
                            ContactName = model.ContactName,
                            Email = model.Email,
                            GST = model.GST,
                            PAN = model.PAN,
                            PhoneNo = model.PhoneNo,
                            RCbook = model.RCbook,
                            CreditAmt = model.CreditAmt
                        });
                    }
                    else
                    {
                        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                        var result = await UserManager.CreateAsync(user, "Agent1Pwd!");

                        if (result.Succeeded)
                        {
                            db.Execute($"Update AspNetUsers Set UserType='{(int)UserTypeEnum.Agent}' , RealName='{model.RealName}' Where Id='{user.Id}'");
                            db.Insert(new Agent
                            {
                                Address = model.Address,
                                State = model.State,
                                AgentId = user.Id,
                                BkAccNo = model.BkAccNo,
                                BkAddress = model.BkAddress,
                                BkIFSC = model.BkIFSC,
                                BkName = model.BkName,
                                ContactName = model.ContactName,
                                Email = model.Email,
                                GST = model.GST,
                                PAN = model.PAN,
                                PhoneNo = model.PhoneNo,
                                RCbook = model.RCbook,
                                CreditAmt = model.CreditAmt
                            });
                            var item = new AgentDiscount { UserID = user.Id };
                            db.Insert(item);
                        }
                    }
                    transaction.Complete();
                }
                catch (Exception e)
                {
                    transaction.Dispose();
                    throw e;
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
