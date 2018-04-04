using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speedbird.Controllers
{
    public class MiceController : EAController
    {
        public ActionResult Index(int? id)
        {

            return View(base.BaseCreateEdit<MiceDetail>(id, "MiceID"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "MiceID,GuestName,TDate,Phone,Email,AgentName,Detail")] MiceDetail item)
        {
            item.TDate = DateTime.Now;
            return base.BaseSave<MiceDetail>(item, item.MiceID > 0);
        }
    }
}