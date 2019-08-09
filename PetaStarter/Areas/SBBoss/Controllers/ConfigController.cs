using Speedbird.Areas.SBBoss.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class ConfigController : EAController
    {
        // GET: SBBoss/Config
        [EAAuthorize(FunctionName = "Config", Writable = false)]
        public ActionResult Index()
        {
            var config = db.Query<Config>("select * from Config");
            return View("Index",config);
        }

        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Config", Writable = true)]
        public ActionResult Manage(int? id)
        {
            return View(base.BaseCreateEdit<Config>(id, "ConfigId"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Config", Writable = true)]
        public ActionResult Manage([Bind(Include = "ConfigId,ProductId,TransServiceCharge, MerchantId,Pwd")] Config item)
        {
            return base.BaseSave<Config>(item, item.ConfigId > 0);
        }
    }
}