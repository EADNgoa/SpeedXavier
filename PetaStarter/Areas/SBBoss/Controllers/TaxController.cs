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
    public class TaxController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Tax", Writable = false)]

        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<Tax>(page, " * ","Taxes Where TaxName like '%" + AN + "%'"));
        }



        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Tax", Writable = true)]

        public ActionResult Manage(int? id)
        {   
            ViewBag.ServiceTypeId = Enum.GetValues(typeof(ServiceTypeEnum)).Cast<ServiceTypeEnum>()
                .Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            return View(base.BaseCreateEdit<Tax>(id, "TaxID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Tax", Writable = true)]

        public ActionResult Manage([Bind(Include = "TaxID,TaxName, ServiceTypeId,WEF,Percentage")] Tax item)
        {
            return base.BaseSave<Tax>(item, item.TaxId > 0);
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
