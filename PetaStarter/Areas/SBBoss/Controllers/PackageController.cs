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
    public class PackageController : EAController
    {
        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<PackageDets>(page, " * ","Package p inner join GuideLanguage gl on p.GuideLanguageID =gl.GuideLanguageID Where PackageName like '%" + AN + "%'"));
        }



        public ActionResult Manage(int? id)
        {
           ViewBag.GuideLanguageID = new SelectList(db.Fetch<GuideLanguage>("Select GuideLanguageID,GuideLanguageName from GuideLanguage"), "GuideLanguageID", "GuideLanguageName");
            
            return View(base.BaseCreateEdit<Package>(id, "PackageID"));
        }

  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "PackageID,ServiceTypeID,PackageName,Description,Duration,Itinerary,Dificulty,GroupSize,GuideLanguageID,StartTime,Inclusion,Exclusion")] Package item)
        {            
            return base.BaseSave<Package>(item, item.PackageID > 0);
        }

        public ActionResult CatManage(int? id)
        {
            ViewBag.Pack = db.FirstOrDefault<Package>($"Select * From Package Where PackageID='{id}'");
            ViewBag.CategoryID = new SelectList(db.Fetch<Category>("Select CategoryID,CategoryName from Category"), "CategoryID", "CategoryName");

            ViewBag.Category = db.Fetch<CategoryDets>($"Select * From Package_Category pa inner join Package p on p.PackageID =pa.PackageID inner join Category a on pa.CategoryID=a.CategoryID where pa.PackageID ='{id}'");
            return View(base.BaseCreateEdit<Package>(id, "PackageID"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CatManage(FormCollection fm)
        {
            int pid = int.Parse(fm["Id"]);
            int aid = int.Parse(fm["CategoryID"]);
            var item = new Package_Category { PackageID=pid,CategoryID=aid };
            db.Insert(item);
            return RedirectToAction("CatManage");
        }


        public ActionResult AttractManage(int? id)
        {
            ViewBag.Pack = db.FirstOrDefault<Package>($"Select * From Package Where PackageID='{id}'");
            ViewBag.AttractionID = new SelectList(db.Fetch<Attraaction>("Select AttractionID,AttractionName from Attraaction"), "AttractionID", "AttractionName");

            ViewBag.Attraction = db.Fetch<AttractionDets>($"Select * From Package_Attraction pa inner join Package p on p.PackageID =pa.PackageID inner join Attraaction a on pa.AttractionID=a.AttractionID where pa.PackageID ='{id}'");
            return View(base.BaseCreateEdit<Package>(id, "PackageID"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AttractManage(FormCollection fm)
        {
            int pid = int.Parse(fm["Id"]);
            int aid = int.Parse(fm["AttractionID"]);
            var item = new Package_Attraction { PackageID = pid, AttractionID = aid };
            db.Insert(item);
            return RedirectToAction("AttractManage");
        }

        public ActionResult ActManage(int? id)
        {
            ViewBag.Pack = db.FirstOrDefault<Package>($"Select * From Package Where PackageID='{id}'");
            ViewBag.ActivityID = new SelectList(db.Fetch<Activity>("Select ActivityID,ActivityName from Activity"), "ActivityID", "ActivityName");

            ViewBag.Activity = db.Fetch<ActivityDets>($"Select * From Package_Activity pa inner join Package p on p.PackageID =pa.PackageID inner join Activity a on pa.ActivityID=a.ActivityID where pa.PackageID ='{id}'");
            return View(base.BaseCreateEdit<Package>(id, "PackageID"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActManage(FormCollection fm)
        {
            int pid = int.Parse(fm["Id"]);
            int aid = int.Parse(fm["ActivityID"]);
            var item = new Package_Activity { PackageID = pid, ActivityID = aid };
            db.Insert(item);
            return RedirectToAction("ActManage");
        }
        public ActionResult PackPicture(int? id)
        {

            ViewBag.Pack = db.FirstOrDefault<Package>($"Select * From Package Where PackageID='{id}'");
            ViewBag.Pics = db.Fetch<Picture>($"Select * From Picture where ServiceID='{id}' and ServiceTypeID='{(int)ServiceTypeEnum.Packages}'");
            base.BaseCreateEdit<Picture>(id, "PictureID");
       
                PictureDets ci = new PictureDets() { };
            return View(ci);     

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackPicture([Bind(Include = "PictureID,ServiceTypeID,PictureName,ServiceID,UploadedFile")] PictureDets pics)
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
               
                    return RedirectToAction("PackPicture");
             
            
        }




  

        public ActionResult PackPrice(int? id,int? EID)
        {
            ViewBag.Pack = db.FirstOrDefault<Package>($"Select * From Package Where PackageID='{id}'");
            ViewBag.Price = db.Fetch<PriceDets>($"Select * from Prices p inner join OptionType ot on ot.OptionTypeID = p.OptionTypeID where ServiceID= {id} and ServiceTypeID ='{(int)ServiceTypeEnum.Packages}'");
            ViewBag.OptionTypeID = new SelectList(db.Fetch<OptionType>("Select OptionTypeID,OptionTypeName from OptionType where ServiceTypeID=@0", (int)ServiceTypeEnum.Packages), "OptionTypeID", "OptionTypeName");

            return View(base.BaseCreateEdit<Price>(EID, "PriceID"));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackPrice([Bind(Include = "PriceID,ServiceID,OptionTypeID,WEF,_Price")] Price item)
        {
             base.BaseSave<Price>(item, item.PriceID > 0);
            return RedirectToAction("PackPrice");

        }
        public ActionResult Delete(int? PackID, int? ActID, int? pid, int? sid, int? AttractID, int? CatID)
        {
            if (ActID != null && PackID != null)
            {
                db.Execute($"Delete From Package_Activity Where PackageID={PackID} and ActivityID={ActID}");
                return RedirectToAction("ActManage", new { id = PackID });

            }
            if (AttractID != null && PackID != null)
            {
                db.Execute($"Delete From Package_Attraction Where PackageID={PackID} and AttractionID={AttractID}");
                return RedirectToAction("AttractManage", new { id = PackID });

            }
            if (CatID != null && PackID != null)
            {
                db.Execute($"Delete From Package_Category Where PackageID={PackID} and CategoryID={CatID}");
                return RedirectToAction("CatManage", new { id = PackID });

            }
            if (pid != null)
            {
                db.Execute($"Delete From Picture Where PictureID={pid}");
                return RedirectToAction("PackPicture", new { id = PackID });

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
