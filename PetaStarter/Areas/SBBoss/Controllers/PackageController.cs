using PagedList;
using Speedbird.Controllers;
using System;
using System.Collections.Generic;
//using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.IO;
using System.Net;
//using System.Web;
using System.Web.Mvc;
using static PetaStarter.Areas.SBBoss.Models.DataTablesModels;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class PackageController : EAController
    {
        public ActionResult Index(int? page, string AN, int? sid)
        {
        
            ViewBag.sid = sid;
        
            if (sid == 1) { ViewBag.Title = "Packages"; }
            if (sid == 2) { ViewBag.Title = "Cruises"; }
            if (sid == 3) { ViewBag.Title = "Sight Seeing"; }
            
            return View();
        }

        [HttpPost]
        public JsonResult GetPkgList(DTParameters parameters, int sid)
        {
            var columnSearch = parameters.Columns.Select(s => s.Search.Value).Take(PackageColumns.Count()).ToList();

            //XMLPath uses nested queries so to avoid that we construct these 4 filters ourselves
            string supname = "";
            string geos = "";
            string cats = "";
            string conts ="";
            int daysl = 0;

            if (columnSearch[2]?.Length > 0) {geos = columnSearch[2]; columnSearch[2] = null; }
            if (columnSearch[4]?.Length > 0) {cats = columnSearch[4]; columnSearch[4] =null; }
            if (columnSearch[6]?.Length > 0) { conts = columnSearch[6]; columnSearch[6] = null; }
            if (columnSearch[5]?.Length > 0) { supname = columnSearch[5]; columnSearch[5] = null; }
            if (columnSearch[8]?.Length > 0) { int.TryParse(columnSearch[8], out daysl); columnSearch[8] = null; }
            string sortOrder = parameters.SortOrder.Replace("EndDateStr", "ValidTo");

            var sql = new PetaPoco.Sql($"Select distinct p.*,ValidTo, (select max(v.ValidTo) from PackageValidity v where v.ServiceID=p.PackageID) as EndDate " +
                $"from Package p Left Join PackageValidity v on v.ServiceID=p.PackageID  and v.PVId=(select max(PVId) from PackageValidity where PackageId=p.PackageID)" );
            var fromsql= new PetaPoco.Sql();
            var wheresql = new PetaPoco.Sql($" where p.ServiceTypeId={sid} ");

            if (geos.Length > 0)
            {
                fromsql.Append(", Geotree g, package_geotree pg");
                wheresql.Append($" and p.packageId=pg.packageId and pg.geotreeid=g.geotreeid and geoname like '%{geos}%'");
            }
            if (cats.Length > 0)
            {
                fromsql.Append(", Category c, package_category pc");
                wheresql.Append($" and p.packageId=pc.packageId and pc.categoryid=c.categoryid and categoryname like '%{cats}%'");
            }
            if (supname.Length > 0 || conts.Length>0)
            {
                fromsql.Append(", supplier s, package_supplier ps");
                wheresql.Append($" and p.packageId=ps.packageId and ps.Supplierid=s.Supplierid ");

                if (supname.Length > 0) wheresql.Append($" and Suppliername like '%{supname}%'");
                if (conts.Length > 0) wheresql.Append($" and ContractNo like '%{conts}%'");
            }
            if (daysl>0)
            {
                wheresql.Append($" and datediff(day,GETDATE(), v.ValidTo) <{daysl} ");
            }

            
            wheresql.Append($"{GetWhereWithOrClauseFromColumns(PackageColumns, columnSearch)}");
            sql.Append(fromsql);
            sql.Append(wheresql);
            sql.Append($"order by {sortOrder}"); 

            try
            {
                var res = db.Query<PackageDets>(sql).Skip(parameters.Start).Take(parameters.Length).ToList();

                res.ForEach(r =>
                   {
                       r.GeoName = String.Join(", ", db.Query<string>("Select GeoName from GeoTree g, package_Geotree pg where g.GeoTreeId=pg.GeoTreeid and pg.packageId=@0", r.PackageID));
                       r.CategoryName = String.Join(", ", db.Query<string>("Select CategoryName from Category g, package_Category pg where g.CategoryId=pg.Categoryid and pg.packageId=@0", r.PackageID));
                       r.SupplierNames = String.Join(", ", db.Query<string>("Select SupplierName from Supplier g, package_Supplier pg where g.SupplierId=pg.Supplierid and pg.packageId=@0", r.PackageID));
                       r.SupplierContractNos = String.Join(", ", db.Query<string>("Select ContractNo from package_supplier where packageId=@0", r.PackageID));                       
                   });


            var dataTableResult = new DTResult<PackageDets>
            {
                draw = parameters.Draw,
                data = res,
                recordsFiltered = 10,
                recordsTotal = res.Count()
            };
            return Json(dataTableResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string GetWhereWithOrClauseFromColumns(string[] columnDefs, List<string> searchValues)
        {
            try
            {
                var where = " ";
                var subQuery = "";
                for (int i = 0; i < searchValues.Count; i++)
                {
                    if (searchValues[i] != null)
                    {
                        if (columnDefs[i].IndexOf("Date") > -1)
                        {
                            //Date search
                            subQuery += columnDefs[i] + " = '%" + DateTime.Parse(searchValues[i]).ToString("yyyy-MM-dd") + "'" + " and ";
                        }
                        else
                        {
                            //free text
                            subQuery += columnDefs[i] + " like '%" + searchValues[i].Replace("'", "''") + "%'" + " and ";
                        }
                        if (i == searchValues.Count - 1)
                        {
                            subQuery = subQuery.Remove(subQuery.LastIndexOf("and"), 2).Insert(subQuery.LastIndexOf("and"), "");
                        }
                    }
                }
                if (subQuery.Trim() != "")
                {
                    where += " and (" + subQuery + ")";
                }

                if(where.LastIndexOf("and )")>0)
                    where = where.Replace("and )",")");
                return where;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string[] PackageColumns => new string[]
        {
            "PackageID",
            "PackageName",
            "GeoName",
            "Duration",
            "CategoryName",
            "SupplierNames",
            "SupplierContractNos",
            "EndDateStr",
            "Daysleft"
        };

        public ActionResult Manage(int? id, int? sid, int mode = 1, int EID = 0) //Mode 1=Details,2=Prices,3=images,4=validity
        {
            ViewBag.sid = sid;
            ViewBag.mode = mode;
            ViewBag.EID = EID;
            if (sid == 1) ViewBag.Title = "Manage Package";
            if (sid == 2) ViewBag.Title = "Manage Cruise";
            if (sid == 3) ViewBag.Title = "Manage SightSeeing";

            ViewBag.PackageName = db.ExecuteScalar<string>("Select PackageName from Package where PackageId=@0", id);
            ViewBag.PackageId = id;
            return View();
        }

        public ActionResult FetchDetails(int? id, int sid)
        {
            ViewBag.sid = sid;
            if (id.HasValue)
            {
                var pkg = base.BaseCreateEdit<Package>(id, "PackageID");
                ViewBag.GeoId = db.Query<GeoTree>("Select * from GeoTree where GeoTreeId in (Select GeoTreeId from Package_GeoTree where PackageId=@0)", pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.GeoName, Value = sl.GeoTreeID.ToString(), Selected = true });
                ViewBag.Acts = db.Query<Activity>("Select ActivityId, ActivityName from Activity where ActivityId in (Select ActivityId from Package_Activity where PackageId=@0)", pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.ActivityName, Value = sl.ActivityID.ToString(), Selected = true });
                ViewBag.Cats = db.Query<Category>("Select CategoryId, CategoryName from Category where CategoryId in (Select CategoryId from Package_Category where PackageId=@0)", pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.CategoryName, Value = sl.CategoryID.ToString(), Selected = true });
                ViewBag.Atts = db.Query<Attraaction>("Select AttractionId, AttractionName from Attraaction where AttractionId in (Select AttractionId from Package_Attraction where PackageId=@0)", pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.AttractionName, Value = sl.AttractionID.ToString(), Selected = true });
                ViewBag.Lang = db.Query<GuideLanguage>("Select GuideLanguageId, GuideLanguageName from GuideLanguage where GuideLanguageId in (Select GuideLanguageId from Package_Language where PackageId=@0)", pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.GuideLanguageName, Value = sl.GuideLanguageID.ToString(), Selected = true });
                ViewBag.Atrs = db.Query<Attribute>("Select AttributeId, AttributeText from Attribute where AttributeId in (Select AttributeId from Package_Attribute where PackageId=@0)", pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.AttributeText, Value = sl.AttributeId.ToString(), Selected = true });
                ViewBag.Sups = db.Query<Supplier>("Select * from Supplier where SupplierId in (Select SupplierId from Package_Supplier where PackageId=@0)", pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.SupplierName, Value = sl.SupplierID.ToString(), Selected = true });
                ViewBag.SupConts = db.Query<Package_Supplier>("Select * from Package_Supplier where PackageId=@0", pkg?.PackageID ?? 0).Select(sl => sl.ContractNo);
                ViewBag.Icns = db.Query<Icon>("Select IconPath from Icons where ServiceTypeId=@0 and ServiceId=@1 ",sid, pkg?.PackageID ?? 0).Select(sl => new SelectListItem { Text = sl.IconPath, Value = sl.IconPath, Selected = true });
                return PartialView("Details", pkg);
            }
            return PartialView("Details", new Package());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult  Manage([Bind(Include = "PackageID,ServiceTypeID,PackageName,Description,Duration,Itinerary,Highlights,Dificulty,GroupSize,GuideLanguageID,StartTime,Inclusion,Exclusion,CouponCode")] Package item, System.Collections.Generic.List<int> GeoTreeID )
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
                    return new JsonResult { Data = true };
                } else
                {
                    transaction.Dispose();
                    return new JsonResult { Data = false };

                }
            }
            
        }

        public ActionResult PackPrice(int? id,int? sid, int? EID)
        {
            ViewBag.PackageId = id;
            ViewBag.sid = sid;
            ViewBag.Pack = db.FirstOrDefault<Package>($"Select * From Package Where PackageID='{id}'");
            ViewBag.Price = db.Fetch<PriceDets>($"Select * from Prices p inner join OptionType ot on ot.OptionTypeID = p.OptionTypeID where ServiceID= {id} and ServiceTypeID ='{(int)ServiceTypeEnum.Packages}'");
            ViewBag.OptionTypeID = new SelectList(db.Fetch<OptionType>("Select OptionTypeID,OptionTypeName from OptionType where ServiceTypeID=@0", (int)ServiceTypeEnum.Packages), "OptionTypeID", "OptionTypeName");

            return PartialView(base.BaseCreateEdit<Price>(EID, "PriceID"));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PackPrice([Bind(Include = "PriceID,ServiceID,OptionTypeID,WEF,_Price")] Price item,int sid)
        {
            base.BaseSave<Price>(item, item.PriceID > 0);
            return RedirectToAction("Manage",new { id=item.ServiceID,  sid, mode=2});
        }


        public ActionResult PackPicture(int? id, int sid)
        {
            ViewBag.PackageId = id;
            
            ViewBag.Pack = db.FirstOrDefault<Package>($"Select * From Package Where PackageID='{id}'");
            ViewBag.Pics = db.Fetch<Picture>($"Select * From Picture where ServiceID='{id}' and ServiceTypeID='{sid}'");
            base.BaseCreateEdit<Picture>(id, "PictureID");

            PictureDets ci = new PictureDets() { };
            return PartialView(ci);
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
                fn = String.Concat(((ServiceTypeEnum)pics.ServiceTypeID).ToString(), "_", pics.ServiceID.ToString(), "_", fn);

                string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                pics.UploadedFile.SaveAs(SavePath);

                res.PictureName = fn;
            }

            return base.BaseSave<Picture>(res, pics.PictureID > 0, "Manage", new { id = pics.ServiceID, sid = pics.ServiceTypeID, mode=3 });

        }
        
        //Now only for Picture Delete
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
                return RedirectToAction("Manage", routeValues: new { id = sid, sid = stid.Value, mode = 3 });

            }
            return RedirectToAction("Manage");
        }

        public ActionResult PVManage(int? id, int? sid, int? EID)
        {
            ViewBag.Pack = db.FirstOrDefault<Package>($"Select * From Package Where PackageID='{id}'");
            ViewBag.sid = ServiceTypeEnum.Packages;
            ViewBag.Validity = db.Fetch<PackageValidity>($"Select * From PackageValidity where ServiceID ='{id}'");
            return PartialView(base.BaseCreateEdit<PackageValidity>(EID, "PVID"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void PVManage([Bind(Include = "PVID,ValidFrom,ValidTo,ServiceID,ServiceTypeID")] PackageValidity item)
        {
            base.BaseSave<PackageValidity>(item, item.PVId > 0);
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
                db.Insert(new Package_Attraction { PackageID = PackageId, AttractionID = item });
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
                db.Insert(new Package_Language { PackageId = PackageId, GuideLanguageId = item });
        }

        //Attributes
        public JsonResult GetAtr(string term)
        {
            var locs = db.Fetch<Attribute>("Select AttributeId, AttributeText from Attribute where AttributeText like '%" + term + "%'");
            return Json(new { results = locs.Select(a => new { id = a.AttributeId, text = a.AttributeText }) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void AtrSave(int PackageId, IEnumerable<int> ActIds)
        {
            db.Delete<Package_Attribute>("Where PackageId=@0", PackageId);
            foreach (var item in ActIds)
                db.Insert(new Package_Attribute { PackageID = PackageId, AttributeID = item, ServiceTypeId = (int) ServiceTypeEnum.Packages  });
        }

        //Icons
        public JsonResult GetIcn()
        {   
                var i = Directory.EnumerateFiles(Server.MapPath("~/Icons"), "*.png").Select(f => f.Substring(f.LastIndexOf("\\")+1));
                return Json(new { results = i.Select(a => new { id = a, text = a }) }, JsonRequestBehavior.AllowGet);         
        }

        [HttpPost]
        public void IcnSave(int PackageId, IEnumerable<string> IcnNames, int ServiceTypeId)
        {
            db.Delete<Icon>("Where ServiceId=@0 and ServiceTypeId=@1", PackageId, ServiceTypeId);
            try
            {
                foreach (var item in IcnNames)
                    db.Insert(poco: new Icon { ServiceId = PackageId, IconPath = item, ServiceTypeId = ServiceTypeId });
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        //Supplier
        public JsonResult GetSup(string term)
        {
            var locs = db.Fetch<Supplier>("Select * from Supplier where SupplierName like '%" + term + "%'");
            return Json(new { results = locs.Select(a => new { id = a.SupplierID, text = a.SupplierName}) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void SupSave(int PackageId, IEnumerable<int> ActIds, string Conts)
        {

            //Clean and split Contract nos
            Conts= Conts.Replace("\n", "");
            var SplitConts = Conts.Split(',');
            var GoodConts = SplitConts.Where(c => c.Trim().Length > 1).Select(c=>c.Trim()).ToArray();

            //handle deletions
            //var oldRecs = db.Query<Package_Supplier>("Where PackageId=@0", PackageId);
            //var bestRecs = oldRecs.Where(r => !ActIds.Contains(r.SupplierID));

            db.Delete<Package_Supplier>("Where PackageId=@0", PackageId);

            int i = 0;
            foreach (var item in ActIds)
            {
                db.Insert(new Package_Supplier { PackageID = PackageId, SupplierID = item,ServiceTypeID=(int)ServiceTypeEnum.Packages, ContractNo = GoodConts[i] });
                i++;
            }
        }
        #endregion

        public string KillSup(int PackageId, int deadSup)
        {
            db.Delete<Package_Supplier>("Where packageId=@0 and SupplierId=@1",PackageId,deadSup);
            return String.Join(",", db.Query<Package_Supplier>("Where packageId=@0", PackageId).Select(s=>s.ContractNo));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Below lies unused code.

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
        
    }
}
