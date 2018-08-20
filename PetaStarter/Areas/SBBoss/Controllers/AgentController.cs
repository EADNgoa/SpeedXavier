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
    public class AgentController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Agents", Writable = false)]
        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<AspNetUser>(page, " * ",$"AspNetUsers Where UserType={(int)UserTypeEnum.Agent} and UserName like '%" + AN + "%'"));
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
