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
    public class OptionTypeController : EAController
    {
        // GET: Clients
        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<OptionType>(page, " * ","OptionType Where OptionTypeName like '%" + AN + "%'"));
        }



        // GET: Clients/Create
        public ActionResult Manage(int? id)
        {
            // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");
            var EnumValues = from ServiceTypeEnum c in Enum.GetValues(typeof(ServiceTypeEnum)) select new { ID = c, Name = c.ToString() };
            ViewBag.ServiceTypeID = new SelectList(EnumValues, "ID", "Name");
            return View(base.BaseCreateEdit<OptionType>(id, "OptionTypeID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "OptionTypeID,OptionTypeName,ServiceTypeID")] OptionType item,ServiceTypeEnum? ServiceType)
        {
            item.ServiceTypeID = (int)ServiceType;
            return base.BaseSave<OptionType>(item, item.OptionTypeID > 0);
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
