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
using static PetaStarter.Areas.SBBoss.Models.DataTablesModels;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class AccomodationController : EAController
    {
        public ActionResult Index(int? page, string AN)
        {

           

         ViewBag.Title = "Accomodation"; 
        

            return View();
        }

        [HttpPost]
        public JsonResult GetAccomList(DTParameters parameters)
        {
            var columnSearch = parameters.Columns.Select(s => s.Search.Value).Take(AccomColumns.Count()).ToList();

            //XMLPath uses nested queries so to avoid that we construct these 4 filters ourselves
            string geos = "";
            string fac = "";

            if (columnSearch[2]?.Length > 0) { geos = columnSearch[2]; columnSearch[2] = null; }

            if (columnSearch[4]?.Length > 0) { fac = columnSearch[4]; columnSearch[4] = null; }

            var sql = new PetaPoco.Sql($"Select distinct a.*,Substring(Description,1,100) as Description from Accomodation a");
            var fromsql = new PetaPoco.Sql();
            var wheresql = new PetaPoco.Sql("where a.AccomodationID >0 ");

            if (geos.Length > 0)
            {
                fromsql.Append(", Geotree g");
                wheresql.Append($"and  g.GeoTreeID=a.GeoTreeID and geoname like '%{geos}%'");
            }
            if (fac.Length > 0)
            {
                fromsql.Append(", Facility f,Facility_Accomodation fa");
                wheresql.Append($" and f.FacilityID=fa.FacilityID and a.AccomodationID = fa.AccomodationID and FacilityName like '%{fac}%'");
            }

            wheresql.Append($"{GetWhereWithOrClauseFromColumns(AccomColumns, columnSearch)}");
            sql.Append(fromsql);
            sql.Append(wheresql);

            try
            {
                var res = db.Query<AccomodationDets>(sql).Skip(parameters.Start).Take(parameters.Length).ToList();

                res.ForEach(r =>
                {
                    r.GeoName = String.Join(", ", db.Query<string>("Select GeoName from GeoTree where GeoTreeID=@0", r.GeoTreeID));
                    r.FacilityName= String.Join(", ", db.Query<string>("Select FacilityName from Facility f, Facility_Accomodation fa where f.FacilityID=fa.FacilityID and fa.AccomodationID=@0", r.AccomodationID));
                });


                var dataTableResult = new DTResult<AccomodationDets>
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

                if (where.LastIndexOf("and )") > 0)
                    where = where.Replace("and )", ")");
                return where;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string[] AccomColumns => new string[]
        {
            "AccomodationID",
            "AccomName",
            "GeoName",
            "Description",
            "FacilityName",
            "Lat",
            "longt",
            "SelfOwned"



        };





        public ActionResult Manage(int? id, int? sid, int mode = 1, int EID = 0) //Mode 1=Details,2=Prices,3=images,4=validity
        {
            ViewBag.sid = sid;
            ViewBag.mode = mode;
            ViewBag.EID = EID;
            
            ViewBag.AccomName = db.ExecuteScalar<string>("Select AccomName from Accomodation where AccomodationID=@0", id);
            ViewBag.AccomodationID = id;
            return View();
        }

        public ActionResult FetchDetails(int? id)
        {
            var accom = base.BaseCreateEdit<Accomodation>(id, "AccomodationID")??new Accomodation();
            ViewBag.GeoId = db.Query<GeoTree>("Select * from GeoTree where GeoTreeId = (Select GeoTreeId from Accomodation where AccomodationId=@0)", accom?.AccomodationID ?? 0).Select(sl => new SelectListItem { Text = sl.GeoName, Value = sl.GeoTreeID.ToString(), Selected = true });
            ViewBag.FaciityID = db.Query<Facility>("Select FacilityID, FacilityName from Facility where FacilityID in (Select FacilityID from Facility_Accomodation where FacilityID=@0)", accom?.AccomodationID ?? 0).Select(sl => new SelectListItem { Text = sl.FacilityName, Value = sl.FacilityID.ToString(), Selected = true });
            ViewBag.Sups = db.Query<Supplier>("Select * from Supplier where SupplierId in (Select SupplierId from Package_Supplier where PackageId=@0)", accom?.AccomodationID ?? 0).Select(sl => new SelectListItem { Text = sl.SupplierName, Value = sl.SupplierID.ToString(), Selected = true });
            ViewBag.SupConts = db.Query<Package_Supplier>("Select * from Package_Supplier where PackageId=@0", accom?.AccomodationID ?? 0).Select(sl => sl.ContractNo);
            //init lat lang
            if (id.Value ==0)
            {
                accom.lat = "15.499539572611416";
                accom.longt = "73.82869418361702";
            }

            return PartialView("Details", accom);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool Manage([Bind(Include = "AccomodationID,AccomName,Description,GeoTreeID,Lat,Longt,CouponCode,SupplierNotepad,SelfOwned")] Accomodation item)
        {
            using (var transaction = db.GetTransaction())
            {
                if (ModelState.IsValid)
                {
                    var r = (item.AccomodationID > 0) ? db.Update(item) : db.Insert(item);
                    transaction.Complete();
                    return true;
                }
                else
                {
                    transaction.Dispose();
                    return false;

                }
            }

        }

        public ActionResult Price(int? id, int? EID)
        {
            ViewBag.PackageId = id;
            ViewBag.sid = ServiceTypeEnum.Accomodation;
            ViewBag.Pack = db.FirstOrDefault<Accomodation>($"Select * From Accomodation Where AccomodationID='{id}'");
            ViewBag.Price = db.Fetch<PriceDets>($"Select * from Prices p inner join OptionType ot on ot.OptionTypeID = p.OptionTypeID where ServiceID= {id} and ServiceTypeID ='{(int)ServiceTypeEnum.Accomodation}'");
            ViewBag.OptionTypeID = new SelectList(db.Fetch<OptionType>("Select OptionTypeID,OptionTypeName from OptionType where ServiceTypeID=@0", (int)ServiceTypeEnum.Accomodation), "OptionTypeID", "OptionTypeName");

            return PartialView(base.BaseCreateEdit<Price>(EID, "PriceID"));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Price([Bind(Include = "PriceID,ServiceID,OptionTypeID,WEF,_Price")] Price item, int sid)
        {
            base.BaseSave<Price>(item, item.PriceID > 0);
            return RedirectToAction("Manage", new { id = item.ServiceID, sid, mode = 2 });
        }


        public ActionResult Picture(int? id)
        {
            ViewBag.ServiceTypeID = ServiceTypeEnum.Accomodation;

            ViewBag.Pack = db.FirstOrDefault<Accomodation>($"Select * From Accomodation Where AccomodationID={id}");
            ViewBag.Pics = db.Fetch<Picture>($"Select * From Picture where ServiceID='{id}' and ServiceTypeID='{(int)ServiceTypeEnum.Accomodation}'");
            base.BaseCreateEdit<Picture>(id, "PictureID");

            PictureDets ci = new PictureDets() { };
            return PartialView(ci);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Picture([Bind(Include = "PictureID,ServiceTypeID,PictureName,ServiceID,UploadedFile")] PictureDets pics)
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

            return base.BaseSave<Picture>(res, pics.PictureID > 0, "Manage", new { id = pics.ServiceID, sid = pics.ServiceTypeID, mode = 3 });

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
        



        public ActionResult Delete(int? aid,int? fid,int?pid ,int? sid)
        {
            if (aid != null && fid != null)
            {
                db.Execute($"Delete From Facility_Accomodation Where FacilityID={fid} and AccomodationID={aid}");
                return RedirectToAction("FacManage", new { id = aid });

            }
            if (pid!= null) 
            {
                //First remove the old img (if exists)
                string oldImg = db.Single<string>("Select PictureName from Picture where PictureId=@0", pid);
                if (oldImg?.Length > 0) System.IO.File.Delete(System.IO.Path.Combine(Server.MapPath("~/Images"), oldImg));

                db.Execute($"Delete From Picture Where PictureID={pid}");
                return RedirectToAction("Manage", routeValues: new { id = sid, sid = ServiceTypeEnum.Accomodation, mode = 3 });


            }
            return RedirectToAction("Manage");
        }
        public JsonResult GetFac(string term)
        {
            var locs = db.Fetch<Facility>("Select FacilityID, FacilityName from Facility where FacilityName like '%" + term + "%'");
            return Json(new { results = locs.Select(a => new { id = a.FacilityID, text = a.FacilityName }) }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public void FacSave(int AccomodationID, IEnumerable<int> FacIDs)
        {
            db.Delete<Facility_Accomodation>("Where AccomodationID=@0", AccomodationID);
            foreach (var item in FacIDs)
                db.Insert(new Facility_Accomodation { AccomodationID = AccomodationID, FacilityID = item });
        }

  

        public ActionResult PriceInclusions(int? id, int? EID)
        {            
            ViewBag.Price = db.Single<PriceDets>($"Select * from Prices p inner join OptionType ot on ot.OptionTypeID = p.OptionTypeID where PriceID= @0 ",id);
            ViewBag.PriceInclusions = db.Fetch<PriceInclusion>($"Select * from PriceInclusions where PriceID= @0 ", id);
            ViewBag.MealPlanId = Enum.GetValues(typeof(MealPlanEnum)).Cast<MealPlanEnum>()
                .Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            return View(base.BaseCreateEdit<PriceInclusion>(EID, "PriceInclusionId"));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PriceInclusions([Bind(Include = "PriceInclusionId, PriceID,Amount,Description,MealPlanId")] PriceInclusion item)
        {
            return base.BaseSave<PriceInclusion>(item, item.PriceInclusionId > 0, "PriceInclusions", new { id = item.PriceId});
        }
        public ActionResult PVManage(int? id, int? sid, int? EID)
        {
            ViewBag.Pack = db.FirstOrDefault<Accomodation>($"Select * From Accomodation Where AccomodationID={id}");
            ViewBag.sid = ServiceTypeEnum.Accomodation;
            ViewBag.Validity = db.Fetch<PackageValidity>($"Select * From PackageValidity where ServiceID =@0 and ServiceTypeId=@1", id,(int)ServiceTypeEnum.Accomodation);
            return PartialView(base.BaseCreateEdit<PackageValidity>(EID, "PVID"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void PVManage([Bind(Include = "PVID,ValidFrom,ValidTo,ServiceID,ServiceTypeID")] PackageValidity item)
        {
            base.BaseSave<PackageValidity>(item, item.PVId > 0);
        }
        //Supplier
        public JsonResult GetSup(string term)
        {
            var locs = db.Fetch<Supplier>("Select * from Supplier where SupplierName like '%" + term + "%'");
            return Json(new { results = locs.Select(a => new { id = a.SupplierID, text = a.SupplierName }) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void SupSave(int PackageId, IEnumerable<int> ActIds, string Conts)
        {

            //Clean and split Contract nos
            Conts = Conts.Replace("\n", "");
            var SplitConts = Conts.Split(',');
            var GoodConts = SplitConts.Where(c => c.Trim().Length > 1).Select(c => c.Trim()).ToArray();

            //handle deletions
            //var oldRecs = db.Query<Package_Supplier>("Where PackageId=@0", PackageId);
            //var bestRecs = oldRecs.Where(r => !ActIds.Contains(r.SupplierID));

            db.Delete<Package_Supplier>("Where PackageId=@0", PackageId);

            int i = 0;
            foreach (var item in ActIds)
            {
                db.Insert(new Package_Supplier { PackageID = PackageId, SupplierID = item, ServiceTypeID = (int)ServiceTypeEnum.Accomodation, ContractNo = GoodConts[i] });
                i++;
            }
        }


        public string KillSup(int AccomodationID, int deadSup)
        {
            db.Delete<Package_Supplier>("Where packageId=@0 and SupplierId=@1 and ServiceTypeId=@2", AccomodationID, deadSup, (int)ServiceTypeEnum.Accomodation);
            return String.Join(",", db.Query<Package_Supplier>("Where packageId=@0 and ServiceTypeId=@1", AccomodationID, (int)ServiceTypeEnum.Accomodation).Select(s => s.ContractNo));
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
