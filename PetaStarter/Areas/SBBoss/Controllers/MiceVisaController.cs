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
    public class MiceVisaController : EAController
    {
        // GET: Clients
        public ActionResult Mice(int? page ,string AN,int? Id )
        {
            if (Id != null) db.Execute($"Update MiceDetails Set IsRead='{true}' Where MiceID='{Id}'");
            if (AN?.Length > 0) page = 1;
            return View("Mice", base.BaseIndex<MiceDetail>(page, " * ","MiceDetails Where GuestName like '%" + AN + "%'"));
        }
        public ActionResult Visa(int? page, string AN)
        {
            if (AN?.Length > 0) page = 1;
            return View("Visa", base.BaseIndex<Visa>(page, " * ", "Visa Where VisaCountry like '%" + AN + "%'"));
        }

        public ActionResult Manage(int? id)
        {

            var rec = base.BaseCreateEdit<Visa>(id, "VisaID");
            if (id != null)
            {
                VisaDets ci = new VisaDets()
                {
                  Details=rec.Details,
                  EmbassyAddress=rec.EmbassyAddress,
                  FlagPicture=rec.FlagPicture,
                  VisaCountry=rec.VisaCountry,
                  VisaID=rec.VisaID
                };
                return View(ci);
            }
            else
            {
                VisaDets ci = new VisaDets() { };
                return View(ci);
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "VisaID,VisaCountry,FlagPicture,EmbassyAddress,Details,UploadedFile1,UploadedFile2")] VisaDets rec)
        {

            Visa d = new Visa
            {
                EmbassyAddress = rec.EmbassyAddress,
                VisaCountry = rec.VisaCountry,
                VisaID = rec.VisaID

            };

            if (rec.UploadedFile1 != null && rec.UploadedFile2 != null)
            {
                string fn = rec.UploadedFile1.FileName.Substring(rec.UploadedFile1.FileName.LastIndexOf('\\') + 1);
                string fn2 = rec.UploadedFile2.FileName.Substring(rec.UploadedFile2.FileName.LastIndexOf('\\') + 1);

                fn = rec.FlagPicture + "_" + fn;
                fn2 = rec.VisaCountry + "_" + fn2;

                string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                rec.UploadedFile1.SaveAs(SavePath);

                SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn2);
                rec.UploadedFile2.SaveAs(SavePath);

                d.FlagPicture = fn;
                d.Details = fn2;
            }
            else
            {
                d.FlagPicture = rec.FlagPicture;
                d.Details = rec.Details;

            }
            base.BaseSave<Visa>(d, rec.VisaID > 0);


            return RedirectToAction("Visa");


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
