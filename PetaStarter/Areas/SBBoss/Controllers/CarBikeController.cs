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
    public class CarBikeController : EAController
    {
        // GET: Clients
        public ActionResult Index(int? page, string AN)
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<CarBikeDets>(page, " * ", "CarBike c inner join GeoTree g on c.GeoTreeID = g.GeoTreeID Where CarBikeName like '%" + AN + "%'"));
        }



        // GET: Clients/Create
        public ActionResult Manage(int? id)
        {
            var carBike = base.BaseCreateEdit<CarBike>(id, "CarBikeID");
            ViewBag.GeoId = db.Query<GeoTree>("Select * from GeoTree where GeoTreeId=@0", carBike?.GeoTreeId??0).Select(sl => new SelectListItem { Text = sl.GeoName, Value = sl.GeoTreeID.ToString() });
            return View(carBike);
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "CarBikeID,CarBikeName,GeoTreeID,Description,NoPax,NoSmallBags,NoLargeBags,HasAc,HasCarrier,InclHelmet")] CarBike item)
        {
            return base.BaseSave<CarBike>(item, item.CarBikeID > 0);
        }
        public ActionResult CarPicture(int? id)
        {

            ViewBag.CarBike = db.FirstOrDefault<CarBike>($"Select * From CarBike Where CarBikeID='{id}'");
            ViewBag.Pics = db.Fetch<Picture>($"Select * From Picture where ServiceID='{id}' and ServiceTypeID='{(int)ServiceTypeEnum.CarBike}'");
            base.BaseCreateEdit<Picture>(id, "PictureID");

            PictureDets ci = new PictureDets() { };
            return View(ci);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CarPicture([Bind(Include = "PictureID,ServiceTypeID,PictureName,ServiceID,UploadedFile")] PictureDets pics)
        {

            Picture res = new Picture
            {
                PictureID = pics.PictureID,
                ServiceID = pics.ServiceID,
                ServiceTypeID = pics.ServiceTypeID

            };

            if (pics.UploadedFile != null)
            {
                string fn = pics.UploadedFile.FileName.Substring(pics.UploadedFile.FileName.LastIndexOf('\\') + 1);
                fn = String.Concat("Car_", pics.ServiceID.ToString(), "_", fn); 
                string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                pics.UploadedFile.SaveAs(SavePath);

                res.PictureName = fn;
            }

            base.BaseSave<Picture>(res, pics.PictureID > 0);

            return RedirectToAction("CarPicture");
        }

        public ActionResult Price(int? id, int? EID)
        {
            ViewBag.CarBike = db.FirstOrDefault<CarBike>($"Select * From CarBike Where CarBikeID='{id}'");
            ViewBag.Price = db.Fetch<PriceDets>($"Select * from Prices p inner join OptionType ot on ot.OptionTypeID = p.OptionTypeID where ServiceID= '{id}' and ot.ServiceTypeID ='{(int)ServiceTypeEnum.CarBike}'");
            ViewBag.OptionTypeID = new SelectList(db.Fetch<OptionType>("Select OptionTypeID,OptionTypeName from OptionType where ServiceTypeID=@0", (int)ServiceTypeEnum.CarBike), "OptionTypeID", "OptionTypeName");

            return View(base.BaseCreateEdit<Price>(EID, "PriceID"));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Price([Bind(Include = "PriceID,ServiceID,OptionTypeID,WEF,_Price")] Price item)
        {
            return base.BaseSave<Price>(item, item.PriceID > 0, "Price", new { id = item.ServiceID });
        }



        public ActionResult Delete(int? CarID, int? pid)
        {
       
            if (pid != null)
            {
                //First remove the old img (if exists)
                string oldImg = db.Single<string>("Select PictureName from Picture where PictureId=@0", pid);
                if (oldImg?.Length > 0) System.IO.File.Delete(System.IO.Path.Combine(Server.MapPath("~/Images"), oldImg));

                db.Execute($"Delete From Picture Where PictureID={pid}");
                return RedirectToAction("CarPicture", new { id = CarID });
            }
            return RedirectToAction("Manage");
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
