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
    public class AccomodationController : EAController
    {
        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<Accomodation>(page, " * ","Accomodation Where AccomName like '%" + AN + "%'"));
        }



        public ActionResult Manage(int? id)
        {
            var newAcc = base.BaseCreateEdit<Accomodation>(id, "AccomodationID");
            if (!id.HasValue) //Create mode
            {
                newAcc = new Accomodation();
                newAcc.lat = "15.498626410761135";
                newAcc.longt = "73.82881432771683";
            }
            ViewBag.GeoId = db.Query<GeoTree>("Select * from GeoTree where GeoTreeId=@0",newAcc.GeoTreeID??0).Select(sl => new SelectListItem { Text = sl.GeoName, Value = sl.GeoTreeID.ToString() });
            return View(newAcc);
        }

  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "AccomodationID,AccomName,Description,GeoTreeID,lat,longt")] Accomodation item)
        {            
            return base.BaseSave<Accomodation>(item, item.AccomodationID > 0);
        }

        public ActionResult FacManage(int? id)
        {
            ViewBag.Accom = db.FirstOrDefault<Accomodation>($"Select * From Accomodation Where AccomodationID='{id}'");
            ViewBag.FacilityID = new SelectList(db.Fetch<Facility>("Select FacilityID,FacilityName from Facility"), "FacilityID", "FacilityName");

            ViewBag.Facility = db.Fetch<FacilityDets>($"Select * From Facility_Accomodation fa inner join Facility f on f.FacilityID= fa.FacilityID inner join Accomodation a on a.AccomodationID = fa.AccomodationID where fa.AccomodationID ='{id}'");
            return View(base.BaseCreateEdit<Accomodation>(id, "AccomodationID"));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FacManage(FormCollection fm)
        {
            int fid = int.Parse(fm["FacilityID"]);
            int aid = int.Parse(fm["Id"]);
            var item = new Facility_Accomodation { AccomodationID=aid,FacilityID=fid };
            db.Insert(item);
            return RedirectToAction("FacManage");
        }
        public ActionResult Picture(int? id)
        {

            ViewBag.Accom = db.FirstOrDefault<Accomodation>($"Select * From Accomodation Where AccomodationID='{id}'");
            ViewBag.Pics = db.Fetch<Picture>($"Select * From Picture where ServiceID='{id}' and ServiceTypeID='{(int)ServiceTypeEnum.Accomodation}'");
            base.BaseCreateEdit<Picture>(id, "PictureID");
       
                PictureDets ci = new PictureDets() { };
                return View(ci);
            

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Picture([Bind(Include = "PictureID,ServiceTypeID,PictureName,ServiceID,UploadedFile")] PictureDets pics)
        {
          
                    Picture res = new Picture
                    {
                        PictureID=pics.PictureID,
                        ServiceID=pics.ServiceID,
                        ServiceTypeID=pics.ServiceTypeID
                        
                    };

                    if (pics.UploadedFile != null)
                    {
                        string fn = pics.UploadedFile.FileName.Substring(pics.UploadedFile.FileName.LastIndexOf('\\') + 1);
                        fn = pics.ServiceTypeID.ToString() + "_" + fn;
                        string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                        pics.UploadedFile.SaveAs(SavePath);

                        //System.Drawing.Bitmap upimg = new System.Drawing.Bitmap(siteTransaction.UploadedFile.InputStream);
                        //System.Drawing.Bitmap svimg = MyExtensions.CropUnwantedBackground(upimg);
                        //svimg.Save(System.IO.Path.Combine(Server.MapPath("~/Images"), fn));

                        res.PictureName = fn;
                    }
                  
                    base.BaseSave<Picture>(res, pics.PictureID > 0);
               
                    return RedirectToAction("Picture");
             
            
        }




        public ActionResult Delete(int? aid,int? fid,int?pid ,int? sid)
        {
            if (aid != null && fid != null)
            {
                db.Execute($"Delete From Facility_Accomodation Where FacilityID={fid} and AccomodationID={aid}");
                return RedirectToAction("FacManage", new { id = aid });

            }
            if (pid!= null) 
            {
                db.Execute($"Delete From Picture Where PictureID={pid}");
                return RedirectToAction("Picture", new { id = sid });

            }
            return RedirectToAction("Manage");
        }

        public ActionResult Price(int? id,int? EID)
        {
            ViewBag.Accom = db.FirstOrDefault<Accomodation>($"Select * From Accomodation Where AccomodationID='{id}'");
            ViewBag.Price = db.Fetch<PriceDets>($"Select * from Prices p inner join OptionType ot on ot.OptionTypeID = p.OptionTypeID where ServiceID= '{id}' and ot.ServiceTypeID ='{(int)ServiceTypeEnum.Accomodation}'");
            ViewBag.OptionTypeID = new SelectList(db.Fetch<OptionType>("Select OptionTypeID,OptionTypeName from OptionType where ServiceTypeID=@0",(int)ServiceTypeEnum.Accomodation), "OptionTypeID", "OptionTypeName");

            return View(base.BaseCreateEdit<Price>(EID, "PriceID"));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Price([Bind(Include = "PriceID,ServiceID,OptionTypeID,WEF,_Price")] Price item)
        {
             base.BaseSave<Price>(item, item.PriceID > 0);
            return RedirectToAction("Price");

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
