using PagedList;
using Speedbird.Controllers;
using System;
using System.Collections.Generic;
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
        public ActionResult Index(int? page, string AN, int? sid)
        {
            if (AN?.Length > 0) page = 1;
            ViewBag.sid = sid;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (sid == 1) { ViewBag.Title = "Packages"; }
            if (sid == 2) { ViewBag.Title = "Cruises"; }
            if (sid == 3) { ViewBag.Title = "Sight Seeing"; }

            var rec = db.Fetch<PackageDets>(" Select [PackageID], [ServiceTypeID], [PackageName], Substring(Description,1,100) as Description, [Duration], substring([Itinerary],1,100) as Itinerary, [Dificulty] from Package Where PackageName like '%" + AN + "%' and ServiceTypeID =@0", sid).ToList();

            return View(rec.ToPagedList(pageNumber, pageSize));


        }



        public ActionResult Manage(int? id, int? sid)
        {
            ViewBag.GuideLanguageID = new SelectList(db.Fetch<GuideLanguage>("Select GuideLanguageID,GuideLanguageName from GuideLanguage"), "GuideLanguageID", "GuideLanguageName");
            ViewBag.sid = sid;
            if (sid == 1) ViewBag.Title = "Manage Package";
            if (sid == 2) ViewBag.Title = "Manage Cruise";
            if (sid == 3) ViewBag.Title = "Manage SightSeeing";


            var pkg = base.BaseCreateEdit<Package>(id, "PackageID");
            ViewBag.GeoId = db.Query<GeoTree>("Select * from GeoTree where GeoTreeId in (Select GeoTreeId from Package_GeoTree where PackageId=@0)", pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.GeoName, Value = sl.GeoTreeID.ToString(), Selected = true });
            ViewBag.Acts = db.Query<Activity>("Select ActivityId, ActivityName from Activity where ActivityId in (Select ActivityId from Package_Activity where PackageId=@0)", pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.ActivityName, Value = sl.ActivityID.ToString(), Selected = true });
            ViewBag.Cats = db.Query<Category>("Select CategoryId, CategoryName from Category where CategoryId in (Select CategoryId from Package_Category where PackageId=@0)", pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.CategoryName, Value = sl.CategoryID.ToString(), Selected = true });
            ViewBag.Atts = db.Query<Attraaction>("Select AttractionId, AttractionName from Attraaction where AttractionId in (Select AttractionId from Package_Attraction where PackageId=@0)", pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.AttractionName, Value = sl.AttractionID.ToString(), Selected = true });
            ViewBag.Lang = db.Query<GuideLanguage>("Select GuideLanguageId, GuideLanguageName from GuideLanguage where GuideLanguageId in (Select GuideLanguageId from Package_Language where PackageId=@0)", pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.GuideLanguageName, Value = sl.GuideLanguageID.ToString(), Selected = true });
            return View(pkg);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage([Bind(Include = "PackageID,ServiceTypeID,PackageName,Description,Duration,Itinerary,Highlights,Dificulty,GroupSize,GuideLanguageID,StartTime,Inclusion,Exclusion")] Package item, System.Collections.Generic.List<int> GeoTreeID)
        {
            using (var transaction = db.GetTransaction())
            {
                if (ModelState.IsValid)
                {
                    var r = (item.PackageID > 0) ? db.Update(item) : db.Insert(item);


                    db.Execute("Delete from Package_GeoTree where packageId=@0", item.PackageID);
                    GeoTreeID.ForEach(g =>
                    {
                        db.Insert(new Package_GeoTree { GeoTreeID = g, PackageID = item.PackageID });
                    });

                    transaction.Complete();
                } else
                {
                    transaction.Dispose();
                    return RedirectToAction("Manage", new { id = item.PackageID, sid = item.ServiceTypeID });
                }
            }
            return RedirectToAction("Index", new { sid = item.ServiceTypeID });
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
            var item = new Package_Category { PackageID = pid, CategoryID = aid };
            db.Insert(item);
            return RedirectToAction("CatManage");
        }

        public ActionResult PVManage(int? id, int? EID)
        {
            ViewBag.Pack = db.FirstOrDefault<Package>($"Select * From Package Where PackageID='{id}'");

            ViewBag.Validity = db.Fetch<PackageValidity>($"Select * From PackageValidity where PackageID ='{id}'");
            return View(base.BaseCreateEdit<PackageValidity>(EID, "PVID"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PVManage([Bind(Include = "PVID,PackageId,ValidFrom,ValidTo,")] PackageValidity item)
        {
            base.BaseSave<PackageValidity>(item, item.PVId > 0);
            return RedirectToAction("PVManage");

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

        public ActionResult LangManage(int? id)
        {
            ViewBag.Pack = db.FirstOrDefault<Package>($"Select * From Package Where PackageID='{id}'");
            ViewBag.GuideLanguageID = new SelectList(db.Fetch<GuideLanguage>("Select GuideLanguageID,GuideLanguageName from GuideLanguage"), "GuideLanguageID", "GuideLanguageName");

            ViewBag.Lang = db.Fetch<LanguageDets>($"Select * From Package_Language pa inner join Package p on p.PackageID =pa.PackageID inner join GuideLanguage a on pa.GuideLanguageID=a.GuideLanguageID where pa.PackageID ='{id}'");
            return View(base.BaseCreateEdit<Package>(id, "PackageID"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LangManage(FormCollection fm)
        {
            int pid = int.Parse(fm["Id"]);
            int aid = int.Parse(fm["GuideLanguageID"]);
            var item = new Package_Language { GuideLanguageId = aid, PackageId = pid };
            db.Insert(item);
            return RedirectToAction("LangManage");
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
        public ActionResult PackPicture(int? id, int stid)
        {
            ViewBag.Pack = db.FirstOrDefault<Package>($"Select * From Package Where PackageID='{id}'");
            ViewBag.Pics = db.Fetch<Picture>($"Select * From Picture where ServiceID='{id}' and ServiceTypeID='{stid}'");
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
                PictureID = pics.PictureID,
                ServiceID = pics.ServiceID,
                ServiceTypeID = pics.ServiceTypeID
            };

            if (pics.UploadedFile != null)
            {
                string fn = pics.UploadedFile.FileName.Substring(pics.UploadedFile.FileName.LastIndexOf('\\') + 1);
                fn = String.Concat( ((ServiceTypeEnum)pics.ServiceTypeID).ToString() ,"_", pics.ServiceID.ToString(), "_", fn);
                
                string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                pics.UploadedFile.SaveAs(SavePath);

                res.PictureName = fn;
            }

            return base.BaseSave<Picture>(res, pics.PictureID > 0,"PackPicture", new { id=pics.ServiceID, stid=pics.ServiceTypeID});           

        }






        public ActionResult PackPrice(int? id, int? EID)
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
        public ActionResult Delete(int? PackID, int? ActID, int? pid, int? sid, int? AttractID, int? CatID, int? GuideID, int? stid)
        {
            if (ActID != null && PackID != null)
            {
                db.Execute($"Delete From Package_Activity Where PackageID={PackID} and ActivityID={ActID}");
                return RedirectToAction("ActManage", new { id = PackID });

            }
            if (GuideID != null && PackID != null)
            {
                db.Execute($"Delete From Package_Language Where PackageID={PackID} and GuideLanguageID={GuideID}");
                return RedirectToAction("LangManage", new { id = PackID });

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
                //First remove the old img (if exists)
                string oldImg = db.Single<string>("Select PictureName from Picture where PictureId=@0", pid);
                if (oldImg?.Length > 0) System.IO.File.Delete(System.IO.Path.Combine(Server.MapPath("~/Images"), oldImg));

                db.Execute($"Delete From Picture Where PictureID={pid}");
                return RedirectToAction("PackPicture", routeValues: new { id = sid, stid=stid.Value });

            }
            return RedirectToAction("Manage");
        }

        #region InLine form
        //Activities
        public JsonResult GetAct(string term)
        {
            var locs = db.Fetch<Activity>("Select ActivityId, ActivityName from Activity where ActivityName like '%" + term + "%'");
            return Json(new { results = locs.Select(a => new { id = a.ActivityID, text = a.ActivityName }) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void ActSave(int PackageId, IEnumerable<int> ActIds)
        {
            db.Delete<Package_Activity>("Where PackageId=@0", PackageId);
            foreach (var item in ActIds)
                db.Insert(new Package_Activity { PackageID = PackageId, ActivityID = item });
        }

        //Categories
        public JsonResult GetCat(string term)
        {
            var locs = db.Fetch<Category>("Select CategoryId, CategoryName from Category where CategoryName like '%" + term + "%'");
            return Json(new { results = locs.Select(a => new { id = a.CategoryID, text = a.CategoryName }) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void CatSave(int PackageId, IEnumerable<int> ActIds)
        {
            db.Delete<Package_Category>("Where PackageId=@0", PackageId);
            foreach (var item in ActIds)
                db.Insert(new Package_Category { PackageID = PackageId, CategoryID = item });
        }

        //Attractions
        public JsonResult GetAtt(string term)
        {            
            var locs = db.Fetch<Attraaction>("Select AttractionId, AttractionName from Attraaction where AttractionName like '%" + term + "%'");
            return Json(new { results = locs.Select(a => new { id = a.AttractionID, text = a.AttractionName }) }, JsonRequestBehavior.AllowGet);            
        }

        [HttpPost]
        public void AttSave(int PackageId, IEnumerable<int> ActIds)
        {
            db.Delete<Package_Attraction>("Where PackageId=@0", PackageId);
            foreach (var item in ActIds)
                db.Insert(new Package_Attraction { PackageID = PackageId, AttractionID= item });
        }

        //Language
        public JsonResult GetLan(string term)
        {
            var locs = db.Fetch<GuideLanguage>("Select GuideLanguageId, GuideLanguageName from GuideLanguage where GuideLanguageName like '%" + term + "%'");
            return Json(new { results = locs.Select(a => new { id = a.GuideLanguageID, text = a.GuideLanguageName }) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void LanSave(int PackageId, IEnumerable<int> ActIds)
        {
            db.Delete<Package_Language>("Where PackageId=@0", PackageId);
            foreach (var item in ActIds)
                db.Insert(new Package_Language { PackageId = PackageId, GuideLanguageId= item });
        }
        #endregion

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
