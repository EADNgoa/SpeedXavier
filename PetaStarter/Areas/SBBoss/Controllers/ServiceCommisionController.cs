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
    public class ServiceCOmmisionController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Commision", Writable = false)]
        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<ServiceCommision>(page, " * ","ServiceCommision"));
        }



        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Commision", Writable = true)]

        public ActionResult Manage(int? id)
        {
                   
            return View(db.SingleOrDefault<ServiceCommision>($"where ServiceID = @0", id));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Commision", Writable = true)]

        public ActionResult Manage([Bind(Include = "ServiceID,ServiceName,Amount,Perc")] ServiceCommision item)
        {

            return base.BaseSave<ServiceCommision>(item,true);
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
