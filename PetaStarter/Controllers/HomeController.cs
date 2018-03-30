using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speedbird.Controllers
{
    public class HomeController : EAController
    {
        public ActionResult Index()
        {
            var cat = db.Fetch<CategoryRec>("Select Top 3 * from Category Order By NewID()");
            cat.ForEach(c =>
            {
                c.pack = db.Fetch<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID inner join Package_Category pc on a.PackageID = pc.PackageID inner join Category c on c.CategoryID = pc.CategoryID where pc.CategoryID={c.CategoryID} Order By NewID()").ToList();
            });
            return View(cat);
        }

        ///Activities(partial view of index)
        public ActionResult IndexActPartial()
        {
            var Act = db.Fetch<ActivityRec>("Select Top 3 * from Activity Order By NewID()");
            Act.ForEach(c =>
            {
                c.pack = db.Fetch<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID inner join Package_Activity pa on a.PackageID = pa.PackageID inner join Activity ac on ac.ActivityID = pa.ActivityID where pa.ActivityID={c.ActivityID} Order By NewID()").ToList();
            });
            return PartialView(Act);
        }
        ///Attraction(Partial view of index)
        public ActionResult IndexAttractPartial()
        {
            var Act = db.Fetch<AttractRec>("Select Top 3 * from Attraaction Order By NewID()");
            Act.ForEach(c =>
            {
                c.pack = db.Fetch<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID inner join Package_Attraction pa on a.PackageID = pa.PackageID inner join Attraaction ac on ac.AttractionID = pa.AttractionID where pa.AttractionID={c.AttractionID} Order By NewID()").ToList();
            });
            return PartialView(Act);
        }

        public ActionResult LoadService(int? Goa,int? India,int? World)
        {
            if (Goa != null)
            {
                ViewBag.loc = "Goa";
            }
            else if (India != null)
            {
                ViewBag.loc = "India";
            }
            else if (World != null)
            {
                ViewBag.loc = "International";
            }
          
            ViewBag.Accomodation = db.FirstOrDefault<AccomodationDets>("Select Top 1 * From Accomodation a inner join Picture p on a.AccomodationID = p.ServiceID Order By NewID()");
            ViewBag.Package = db.FirstOrDefault<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID where a.ServiceTypeID={(int)ServiceTypeEnum.Packages} Order By NewID()");
            ViewBag.SightSeeing = db.FirstOrDefault<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID where a.ServiceTypeID={(int)ServiceTypeEnum.SightSeeing} Order By NewID()");
            ViewBag.Cruise = db.FirstOrDefault<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID where a.ServiceTypeID={(int)ServiceTypeEnum.Cruise} Order By NewID()");
            ViewBag.CarBike = db.FirstOrDefault<CarBikeDets>($"Select Top 1 * From CarBike a inner join Picture p on a.CarBikeID = p.ServiceID ");
            var cat = db.Fetch<CategoryRec>("Select * from Category");
            cat.ForEach(c=>
            {
                c.pack = db.Fetch<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID inner join Package_Category pc on a.PackageID = pc.PackageID inner join Category c on c.CategoryID = pc.CategoryID where pc.CategoryID={c.CategoryID} Order By NewID()").ToList(); 
            });
            return View(cat);
        }

        public ActionResult LoadPackages(ServiceTypeEnum? st,string Loc,int? CatID)
        {
            ViewBag.st = st;
            ViewBag.loc = Loc;
            ViewBag.GuideLanguageID = new SelectList(db.Fetch<GuideLanguage>("Select GuideLanguageID,GuideLanguageName from GuideLanguage"), "GuideLanguageID", "GuideLanguageName");

            List<SelectListItem> items = new List<SelectListItem>();
            for(int i = 1; i <= 10; i++)
            {
                items.Add(new SelectListItem { Text = ""+i, Value= ""+i });
            }
            List<SelectListItem> tf = new List<SelectListItem>();
           
                tf.Add(new SelectListItem { Text = "YES" , Value = "1"  });
                tf.Add(new SelectListItem { Text = "NO", Value = "0" });




            ViewBag.TF = tf;
            ViewBag.nums = items;
            var geoloc = db.FirstOrDefault<GeoTree>("Select * from GeoTree where GeoName = @0", Loc);

            if (st == ServiceTypeEnum.Packages || st == ServiceTypeEnum.SightSeeing)
            {
                ViewBag.Cats = db.Fetch<Category>("Select * From Category");
                ViewBag.Acts = db.Fetch<Activity>("Select * From Activity");
                ViewBag.Attracts = db.Fetch<Attraaction>("Select a.* From Attraaction a inner join Package_Attraction pa on pa.AttractionID = a.AttractionID inner join Package p on p.PackageID =pa.PackageID inner join Package_GeoTree pg on pg.PackageID= p.PackageID inner join GeoTree g on g.GeoTreeID=pg.GeoTreeID where g.GeoTreeID in(Select GeotreeId from GetChildGeos(@0))",geoloc.GeoTreeID);



            }
            if (st == ServiceTypeEnum.Accomodation)
            {
                ViewBag.fac = db.Fetch<Facility>("Select * From Facility");             
            }
            return View();
        }
        public ActionResult PackagesPartialView(ServiceTypeEnum? st, string Loc ,IEnumerable<int> CatID, IEnumerable<int> ActID,int? Gsize,int? diff,int? dur,int? GuideLanguageID, IEnumerable<int> AttractID,IEnumerable<int> FacilityID,decimal? maxPrice,decimal? minPrice,int? NoPax,int? large,int? small,int? hasAc,int? HasCarrier)
        {
            var geoloc = db.FirstOrDefault<GeoTree>("Select * from GeoTree where GeoName = @0", Loc);
            List<AccomPackCarBike> apc = new List<AccomPackCarBike>();

            ViewBag.st = st;
            if (st == ServiceTypeEnum.Accomodation)
            {
                ViewBag.ServiceTitle = "Accomodations";
                PetaPoco.Sql MainSql = new PetaPoco.Sql("Select Distinct a.AccomodationID,a.AccomName,a.Description,a.GeoTreeID,a.lat,a.longt");
                PetaPoco.Sql FromSql = new PetaPoco.Sql("from Accomodation a,GeoTree g");
                PetaPoco.Sql WhereSql = new PetaPoco.Sql("where a.GeoTreeID=g.GeoTreeID  and g.GeoTreeID in (Select GeoTreeID from GetChildGeos(@1))", (int)st, geoloc.GeoTreeID);
                if (maxPrice != null && minPrice != null)
                {
                    FromSql.Append(",Prices pr,OptionType ot");
                    WhereSql.Append("and a.AccomodationID = pr.ServiceID and ot.OptionTypeID = pr.OptionTypeID and ot.ServiceTypeID=@2 and Price Between @0 and @1 ", minPrice, maxPrice,ServiceTypeEnum.Accomodation);
                }
                if(FacilityID != null)
                {
                    FromSql.Append(",Facility f,Facility_Accomodation fa");
                    WhereSql.Append("and a.AccomodationID = fa.AccomodationID and f.FacilityID = fa.FacilityID and fa.FacilityID in (@0)",FacilityID.ToArray());
                }
             
                MainSql.Append(FromSql);
                MainSql.Append(WhereSql);
                var accom = db.Query<AccomodationDets>(MainSql).ToList();
                accom.ForEach(a=>
                {
                    a.pic = db.Fetch<PictureDets>("Select Top 1 * From Picture Where ServiceID=@0 and ServiceTypeID=@1 Order By NewID()", a.AccomodationID,(int)Speedbird.ServiceTypeEnum.Accomodation).ToList();
                    a.GeoName = db.First<string>("Select GeoName From GeoTree Where GeoTreeID= @0",a.GeoTreeID);
                    apc.Add(new AccomPackCarBike { ServiceDescription = a.Description, ServiceGeoName = a.GeoName, ServiceID =a.AccomodationID, ServiceName = a.AccomName, ServicePic = a.pic.FirstOrDefault().PictureName });

                });

            }
            if (st == ServiceTypeEnum.CarBike)
            {
                ViewBag.ServiceTitle = "Car And Bikes Rental";
                var CarBike = db.Fetch<CarBikeDets>("Select * From CarBike c where GeoTreeID in (Select GeoTreeID from GetChildGeos(@0))", geoloc.GeoTreeID);
                if (NoPax != null)
                {
                    CarBike = CarBike.Where(c=>c.NoPax <= NoPax).ToList();
                }
                if (large != null)
                {
                    CarBike = CarBike.Where(c => c.NoLargeBags <= large).ToList();
                }
                if (small != null)
                {
                    CarBike = CarBike.Where(c => c.NoSmallBags <= small).ToList();
                }
                if (hasAc == 0)
                {
                    CarBike = CarBike.Where(c => c.HasAc == false).ToList();
                }
                else if(hasAc == 1)
                {
                    CarBike = CarBike.Where(c => c.HasAc == true).ToList();

                }
                if (HasCarrier == 0)
                {
                    CarBike = CarBike.Where(c => c.HasCarrier == false).ToList();
                }
                else if (HasCarrier == 1)
                {
                    CarBike = CarBike.Where(c => c.HasCarrier == true).ToList();

                }
                CarBike.ForEach(c =>
                {
                    c.GeoName = db.First<string>("Select GeoName From GeoTree Where GeoTreeID= @0", c.GeoTreeID);

                    c.pic= db.Fetch<PictureDets>("Select Top 1 * From Picture Where ServiceID=@0 and ServiceTypeID=@1 Order By NewID()", c.CarBikeID, (int)ServiceTypeEnum.CarBike).ToList();
                    apc.Add(new AccomPackCarBike { ServiceDescription = c.Description, ServiceGeoName = c.GeoName, ServiceID = c.CarBikeID, ServiceName = c.CarBikeName, ServicePic = c.pic.FirstOrDefault().PictureName });

                });

            }
            if (st == ServiceTypeEnum.Packages || st == ServiceTypeEnum.SightSeeing || st == ServiceTypeEnum.Cruise)
            {
                ViewBag.ServiceTitle = "Our Best Tours And Excursions";


                PetaPoco.Sql MainSql = new PetaPoco.Sql("Select Distinct p.PackageID,p.ServiceTypeID,p.PackageName,p.Description,p.Duration,p.Itinerary," +
                                                        "p.Dificulty,p.GroupSize,p.StartTime,p.Inclusion,p.Exclusion,p.HighLights");
                PetaPoco.Sql FromSql = new PetaPoco.Sql("from Package p,GeoTree g,Package_GeoTree pg");
                PetaPoco.Sql WhereSql = new PetaPoco.Sql("where p.PackageID=pg.PackageID and g.GeoTreeID=pg.GeoTreeID and  p.ServiceTypeID=@0 and g.GeoTreeID in (Select GeoTreeID from GetChildGeos(@1))", (int)st, geoloc.GeoTreeID);

                if (CatID != null)
                {
                    FromSql.Append(",Package_Category pc,Category c");
                    WhereSql.Append("and p.PackageID = pc.PackageID and c.CategoryID = pc.CategoryID and pc.CategoryID in (@0)",  CatID.ToArray());
                }

                if (ActID != null)
                {
                    FromSql.Append(",Package_Activity pa, Activity a");
                    WhereSql.Append("and p.PackageID = pa.PackageID and a.ActivityID= pa.ActivityID and pa.ActivityID in (@0)",  ActID.ToArray());
                }
                if (AttractID != null)
                {
                    FromSql.Append(",Package_Attraction pat, Attraaction at");
                    WhereSql.Append("and p.PackageID = pat.PackageID and at.AttractionID= pat.AttractionID and pat.AttractionID in (@0)", AttractID.ToArray());
                }
                if (Gsize!=null)
                {
                    WhereSql.Append("and p.GroupSize <= @0",Gsize);
                }
                if (diff != null)
                {
                    WhereSql.Append("and p.Dificulty <= @0", diff);
                }
                if (dur != null)
                {
                    WhereSql.Append("and p.Duration <= @0", dur);
                }
                if (GuideLanguageID != null)
                {
                    FromSql.Append(",Package_Language pl, GuideLanguage gl");
                    WhereSql.Append("and p.PackageID= pl.PackageId and pl.GuideLanguageId = gl.GuideLanguageID and pl.GuideLanguageId =@0 ", GuideLanguageID);
                }
                if(maxPrice!=null && minPrice != null)
                {
                    FromSql.Append(",Prices pri");
                    WhereSql.Append("and p.PackageID = pri.ServiceID and Price Between @0 and @1 ", minPrice,maxPrice);


                }
                MainSql.Append(FromSql);
                MainSql.Append(WhereSql);
                var pack = db.Query<PackageDets>(MainSql).ToList();
                pack.ForEach(p =>
                {
                    p.Pic = db.Fetch<PictureDets>("Select Top 1 * From Picture Where ServiceID=@0 and ServiceTypeID=@1 Order By NewID()", p.PackageID, p.ServiceTypeID).ToList();
                    p.GeoName = db.First<string>("Select GeoName From GeoTree g,Package_GeoTree pg  where pg.PackageID=@0 and g.GeoTreeID = pg.GeoTreeID",p.PackageID );
                    apc.Add(new AccomPackCarBike {ServiceDescription=p.Description,ServiceGeoName=p.GeoName,ServiceID=p.PackageID,ServiceName=p.PackageName,ServicePic=p.Pic.FirstOrDefault().PictureName });

                });


            }
            return PartialView(apc);


        }

        public ActionResult InfoPage(int? ServiceID, ServiceTypeEnum? st)
        {
            ViewBag.ST = st;
            ViewBag.ServiceID = ServiceID;
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i = 1; i <= 10; i++)
            {
                items.Add(new SelectListItem { Text = "" + i, Value = "" + i });
            }
            ViewBag.nums = items;
            return View();
        }
        public ActionResult InfoAccomPartial(int? ServiceID, ServiceTypeEnum? st)
        {
            var accom = db.Query<AccomodationDets>("select * From Accomodation  Where AccomodationID =@0", ServiceID).ToList().FirstOrDefault();
            accom.pic = db.Fetch<PictureDets>("Select * From Picture Where ServiceID=@0 and ServiceTypeID=@1 Order By NewID()", accom.AccomodationID, (int)ServiceTypeEnum.Accomodation).ToList();
            accom.GeoName = db.First<string>("Select GeoName From GeoTree  where GeoTreeID=@0", accom.GeoTreeID);
            int facID = db.ExecuteScalar<int>("Select FacilityID From Facility_Accomodation where AccomodationID=@0 ",accom.AccomodationID);
            ViewBag.SimilarAccom = db.Query<AccomodationDets>("Select * From Accomodation a inner join Facility_Accomodation fa on a.AccomodationID= fa.AccomodationID inner join Facility f on f.FacilityID = fa.FacilityID where fa.FacilityID=@0 ", facID);

            return PartialView(accom);
        }
        public ActionResult AccomFacPartial(int? ServiceID)
        {
            int facID = db.ExecuteScalar<int>("Select FacilityID From Facility_Accomodation where AccomodationID=@0 ", ServiceID);
            var Similar = db.Query<AccomodationDets>("Select * From Accomodation a inner join Facility_Accomodation fa on a.AccomodationID= fa.AccomodationID inner join Facility f on f.FacilityID = fa.FacilityID where fa.FacilityID=@0 ", facID).ToList();
            Similar.ForEach(a=>
            {
                a.pic = db.Fetch<PictureDets>("Select Top 1 * From Picture Where ServiceID=@0 and ServiceTypeID=@1 Order By NewID()", a.AccomodationID, (int)Speedbird.ServiceTypeEnum.Accomodation).ToList();
                a.GeoName = db.First<string>("Select GeoName From GeoTree Where GeoTreeID= @0", a.GeoTreeID);
            });
            return PartialView(Similar);
        }
        public ActionResult InfoCarBikePartial(int? ServiceID, ServiceTypeEnum? st)
        {
            var CarBike = db.FirstOrDefault<CarBikeDets>("select * From CarBike  Where CarBikeID =@0", ServiceID);
            CarBike.pic = db.Fetch<PictureDets>("Select * From Picture Where ServiceID=@0 and ServiceTypeID=@1 Order By NewID()", CarBike.CarBikeID, (int)ServiceTypeEnum.CarBike).ToList();
            CarBike.GeoName = db.First<string>("Select GeoName From GeoTree  where GeoTreeID=@0", CarBike.GeoTreeID);

            return PartialView(CarBike);
        }

        public ActionResult InfoPackagePartial(int? ServiceID, ServiceTypeEnum? st)
        {
            var Packages = db.FirstOrDefault<PackageDets>("select * From Package  Where PackageID =@0 and ServiceTypeID=@1", ServiceID, st);
            Packages.Pic = db.Fetch<PictureDets>("Select * From Picture Where ServiceID=@0 and ServiceTypeID=@1 Order By NewID()", Packages.PackageID, st).ToList();
            Packages.GeoName = db.First<string>("Select GeoName From GeoTree g,Package_GeoTree pg  where pg.PackageID=@0 and g.GeoTreeID = pg.GeoTreeID", Packages.PackageID);
            Packages.Glang = db.Fetch<GuideLanguage>("Select * from GuideLanguage g  inner join Package_Language pl on pl.GuideLanguageID= g.GuideLanguageID inner join Package p on p.PackageID= pl.PackageID where pl.PackageID=@0",Packages.PackageID);
            return PartialView(Packages);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}