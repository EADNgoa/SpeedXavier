using Microsoft.AspNet.Identity;
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
    public class ReviewController : EAController
    {
        // GET: Clients
        public ActionResult Index(int? page ,DateTime? AN,int? hide,int? show)
        {
            if (hide != null) db.Execute($"Update Review set IsVisible='{false}' where ReviewID={hide}");
            if (show != null) db.Execute($"Update Review set IsVisible='{true}' where ReviewID={show}");
            if (AN != null) page = 1;
            return View("Index", base.BaseIndex<Speedbird.Review>(page, " * ","Review where ReviewDate like '%" + AN + "%'"));
        }
        public ActionResult Reply(int? id,int? EID)
        {
            // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");
            ViewBag.RevID = id;
            ViewBag.Chat = db.Fetch<ReviewRepDets>($"Select * from ReviewReplies Where ReviewID = {id} Order By ReviewReplyID Desc");
            return View(base.BaseCreateEdit<ReviewReply>(EID, "ReviewReplyID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reply([Bind(Include = "ReviewReplyID,ReviewID,UserID,ReviewDate,Reply,IsVisible")] ReviewReply item)
        {
            item.UserID = User.Identity.GetUserId();
            item.ReviewDate = DateTime.Now;
            item.IsVisible = true;
            base.BaseSave<ReviewReply>(item, item.ReviewReplyID > 0);
            return RedirectToAction("Reply",new {id=item.ReviewID });
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
