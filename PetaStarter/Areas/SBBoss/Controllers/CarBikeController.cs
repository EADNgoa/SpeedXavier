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
    public class CarBikeController : EAController
    {
        public ActionResult Index(int? page, string AN)
        {

           

         ViewBag.Title = "Accomodation"; 
        

            return View();
        }

        [HttpPost]
        public JsonResult GetCarBikeList(DTParameters parameters)
        {
            var columnSearch = parameters.Columns.Select(s => s.Search.Value).Take(CarBikeColumns.Count()).ToList();

            //XMLPath uses nested queries so to avoid that we construct these 4 filters ourselves
            string geos = "";

            if (columnSearch[2]?.Length > 0) { geos = columnSearch[2]; columnSearch[2] = null; }


            var sql = new PetaPoco.Sql($"Select distinct c.*,Substring(Description,1,100) as Description from CarBike c");
            var fromsql = new PetaPoco.Sql();
            var wheresql = new PetaPoco.Sql("where c.CarBikeID >0 ");

            if (geos.Length > 0)
            {
                fromsql.Append(", Geotree g");
                wheresql.Append($"and  g.GeoTreeID=a.GeoTreeID and geoname like '%{geos}%'");
            }
          
            wheresql.Append($"{GetWhereWithOrClauseFromColumns(CarBikeColumns, columnSearch)}");
            sql.Append(fromsql);
            sql.Append(wheresql);

            try
            {
                var res = db.Query<CarBikeDets>(sql).Skip(parameters.Start).Take(parameters.Length).ToList();

                res.ForEach(r =>
                {
                    r.GeoName = String.Join(", ", db.Query<string>("Select GeoName from GeoTree where GeoTreeID=@0", r.GeoTreeID));
                });


                var dataTableResult = new DTResult<CarBikeDets>
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

        private string[] CarBikeColumns => new string[]
        {
            "CarBikeID",
            "CarBikeName",
            "GeoName",
            "Description",
            "NoPax",
            "NoSmallBags",
            "NoLargeBags",
            "HasAc",
            "HasCarrier",
            "InclHelmet",

        };




        public ActionResult Manage(int? id, int? sid, int mode = 1, int EID = 0) //Mode 1=Details,2=Prices,3=images,4=validity
        {
            ViewBag.sid = sid;
            ViewBag.mode = mode;
            ViewBag.EID = EID;
            ViewBag.CarBikeName = db.ExecuteScalar<string>("Select CarBikeName from CarBike where CarBikeID=@0", id);
            ViewBag.CarBikeID = id;
            return View();
        }

        public ActionResult FetchDetails(int? id)
        {
            var accom = base.BaseCreateEdit<CarBike>(id, "CarBikeID");
            ViewBag.GeoId = db.Query<GeoTree>("Select * from GeoTree where GeoTreeId in (Select GeoTreeId from Package_GeoTree where PackageId=@0)", accom?.CarBikeID ?? 0).Select(sl => new SelectListItem { Text = sl.GeoName, Value = sl.GeoTreeID.ToString(), Selected = true });
            return PartialView("Details", accom);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool Manage([Bind(Include = "CarBikeID,CouponCode,CarBikeName,GeoTreeID,Description,NoPax,NoSmallBags,NoLargeBags,HasAc,HasCarrier,InclHelmet")] CarBike item)
        {
            using (var transaction = db.GetTransaction())
            {
                if (ModelState.IsValid)
                {
                    var r = (item.CarBikeID > 0) ? db.Update(item) : db.Insert(item);
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
            ViewBag.CarBikeID = id;
            ViewBag.sid = ServiceTypeEnum.CarBike;
            ViewBag.Pack = db.FirstOrDefault<CarBike>($"Select * From CarBike Where CarBikeID={id}");
            ViewBag.Price = db.Fetch<PriceDets>($"Select * from Prices p inner join OptionType ot on ot.OptionTypeID = p.OptionTypeID where ServiceID= {id} and ServiceTypeID ='{(int)ServiceTypeEnum.CarBike}'");
            ViewBag.OptionTypeID = new SelectList(db.Fetch<OptionType>("Select OptionTypeID,OptionTypeName from OptionType where ServiceTypeID=@0", (int)ServiceTypeEnum.CarBike), "OptionTypeID", "OptionTypeName");

            return PartialView(base.BaseCreateEdit<Price>(EID, "PriceID"));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Price([Bind(Include = "PriceID,ServiceID,OptionTypeID,WEF,_Price")] Price item, int sid)
        {
            base.BaseSave<Price>(item, item.PriceID > 0);
            return RedirectToAction("Manage", new { id = item.ServiceID, sid, mode = 2 });
        }


        public ActionResult CarPicture(int? id)
        {
            ViewBag.ServiceTypeID = ServiceTypeEnum.Accomodation;

            ViewBag.Pack = db.FirstOrDefault<CarBike>($"Select * From CarBike Where CarBikeID={id}");
            ViewBag.Pics = db.Fetch<Picture>($"Select * From Picture where ServiceID='{id}' and ServiceTypeID='{(int)ServiceTypeEnum.Accomodation}'");
            base.BaseCreateEdit<Picture>(id, "PictureID");

            PictureDets ci = new PictureDets() { };
            return PartialView(ci);
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
                fn = String.Concat(((ServiceTypeEnum)pics.ServiceTypeID).ToString(), "_", pics.ServiceID.ToString(), "_", fn);

                string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                pics.UploadedFile.SaveAs(SavePath);

                res.PictureName = fn;
            }

            return base.BaseSave<Picture>(res, pics.PictureID > 0, "Manage", new { id = pics.ServiceID, sid = pics.ServiceTypeID, mode = 3 });

        }




        public ActionResult Delete(int? stid, int? pid, int? sid)
        {
          
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






        /*old code
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
        public ActionResult Manage([Bind(Include = "CarBikeID,CouponCode,CarBikeName,GeoTreeID,Description,NoPax,NoSmallBags,NoLargeBags,HasAc,HasCarrier,InclHelmet")] CarBike item)
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
        */
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
