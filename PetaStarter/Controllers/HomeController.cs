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



            ViewBag.nums = items;
            var geoloc = db.FirstOrDefault<GeoTree>("Select * from GeoTree where GeoName = @0", Loc);

            if (st == ServiceTypeEnum.Packages || st == ServiceTypeEnum.SightSeeing)
            {
                ViewBag.Cats = db.Fetch<Category>("Select * From Category");
                ViewBag.Acts = db.Fetch<Activity>("Select * From Activity");
                ViewBag.Attracts = db.Fetch<Attraaction>("Select a.* From Attraaction a inner join Package_Attraction pa on pa.AttractionID = a.AttractionID inner join Package p on p.PackageID =pa.PackageID inner join Package_GeoTree pg on pg.PackageID= p.PackageID inner join GeoTree g on g.GeoTreeID=pg.GeoTreeID where g.GeoTreeID in(Select GeotreeId from GetChildGeos(@0))",geoloc.GeoTreeID);



            }
            return View();
        }
        public ActionResult PackagesPartialView(ServiceTypeEnum? st, string Loc ,IEnumerable<int> CatID, IEnumerable<int> ActID,int? Gsize,int? diff,int? dur,int? GuideLanguageID, IEnumerable<int> AttractID)
        {
            var geoloc = db.FirstOrDefault<GeoTree>("Select * from GeoTree where GeoName = @0", Loc);

            if (st == ServiceTypeEnum.Accomodation)
            {
                ViewBag.ServiceTitle = "Accomodations";
                ViewBag.Accomodation = db.Fetch<AccomodationDets>($"Select * From Accomodation a inner join Picture p on a.AccomodationID = p.ServiceID  inner join Prices rs on a.AccomodationID=rs.ServiceID inner join OptionType ot on rs.OptionTypeID =ot.OptionTypeID where ot.SerViceTypeID ={(int)ServiceTypeEnum.Accomodation} and GeoTreeID in (Select GeoTreeID from GetChildGeos(@0))", geoloc.GeoTreeID);
            }
            if (st == ServiceTypeEnum.CarBike)
            {
                ViewBag.ServiceTitle = "Car And Bikes Rental";
                ViewBag.CarBike = db.Fetch<CarBikeDets>($"Select * From CarBike a inner join Picture p on a.CarBikeID = p.ServiceID  inner join Prices rs on a.CarBikeID=rs.ServiceID and GeoTreeID in (Select GeoTreeID from GetChildGeos(@0))", geoloc.GeoTreeID);
            }
            if (st == ServiceTypeEnum.Packages || st == ServiceTypeEnum.SightSeeing || st == ServiceTypeEnum.Cruise)
            {
                ViewBag.ServiceTitle = "Our Best Tours And Excursions";

                ViewBag.st = st;

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
                    WhereSql.Append("and p.PackageID= pl.PackageId and pl.GuideLanguageId = gl.GuideLanguageID and pl.GuideLanguageId ", GuideLanguageID);
                }




                MainSql.Append(FromSql);
                MainSql.Append(WhereSql);
                var pack = db.Query<PackageDets>(MainSql).ToList();

                pack.ForEach(p =>
                {
                    p.Act = db.Fetch<ActivityDets>("Select * From Activity a inner join Package_Activity pa on a.ActivityID = pa.ActivityID inner join Package p on p.PackageID=pa.PackageID Where p.PackageID=@0 Order By NewID()", p.PackageID).ToList();
                    p.Cat = db.Fetch<CategoryDets>("Select * From Category a inner join Package_Category pa on a.CategoryID = pa.CategoryID inner join Package p on p.PackageID=pa.PackageID Where p.PackageID=@0 Order By NewID()", p.PackageID).ToList();
                    p.Pic = db.Fetch<PictureDets>("Select Top 1 * From Picture Where ServiceID=@0 and ServiceTypeID=@1 Order By NewID()", p.PackageID, p.ServiceTypeID).ToList();
                });

                return PartialView(pack);

            }
            return PartialView();


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