using Microsoft.AspNet.Identity;
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
    public class PettyCashController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Petty Cash", Writable = false)]
        public ActionResult Index(int? page, DateTime? dt)
        {
            page = 1;
            return View("Index", base.BaseIndex<PettyCash>(page, " * ", "PettyCash Where Tdate Like '%" + dt + "%'"));
        }



        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Petty Cash", Writable = false)]
        public ActionResult Manage(int? id)
        {
           // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");
            
            return View(base.BaseCreateEdit<PettyCash>(id, "CashInHandRegID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Petty Cash", Writable = false)]
        public ActionResult Manage([Bind(Include = "CashInHandRegID,Tdate,NameAndDesg,CashToDeclareStart,CashToDeclareEnd,DetailsOfCashExp,Remarks")] PettyCash item)
        {
            string UN = db.ExecuteScalar<string>("Select UserName From AspNetUsers where Id=@0",User.Identity.GetUserId());
            item.NameAndDesg = UN;
            item.Tdate = DateTime.Now;
            return base.BaseSave<PettyCash>(item, item.CashInHandRegID > 0);
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
