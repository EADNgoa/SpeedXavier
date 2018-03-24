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
            if (st == ServiceTypeEnum.Packages || st == ServiceTypeEnum.SightSeeing)
            {
                ViewBag.Cats = db.Fetch<Category>("Select * From Category");

                ViewBag.Acts = db.Fetch<Activity>("Select * From Activity");
            }
            return View();
        }
        public ActionResult PackagesPartialView(ServiceTypeEnum? st, string Loc ,IEnumerable<int> CatID, IEnumerable<int> ActID)
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

                List<PackageDets> pack = new List<PackageDets>();
                if (CatID != null)
                {
                   pack = db.Query<PackageDets>("Select a.PackageID,a.ServiceTypeID,a.PackageName,a.Description,a.Duration,a.Itinerary,a.Dificulty,a.GroupSize,a.StartTime,a.Inclusion,a.Exclusion,a.HighLights,pg.GeoTreeID,g.GeoName,pc.CategoryID From Package a" +
                    $"  inner join Package_Geotree pg on a.PackageID=pg.PackageID inner join GeoTree g on g.GeoTreeID = pg.GeoTreeID inner join Package_Category pc on pc.PackageID =a.PackageID inner join Category c on c.CategoryID=pc.CategoryID where a.ServiceTypeID=@0 and pc.CategoryID in (@2) and g.GeoTreeID in (Select GeoTreeID from GetChildGeos(@1))", (int)st, geoloc.GeoTreeID, CatID).ToList();                    

                }
                else if (ActID!=null)
                {
                    pack = db.Query<PackageDets>("Select a.PackageID,a.ServiceTypeID,a.PackageName,a.Description,a.Duration,a.Itinerary,a.Dificulty,a.GroupSize,a.StartTime,a.Inclusion,a.Exclusion,a.HighLights,pg.GeoTreeID,g.GeoName,pc.ActivityID From Package a" +
           $"  inner join Package_Geotree pg on a.PackageID=pg.PackageID inner join GeoTree g on g.GeoTreeID = pg.GeoTreeID inner join Package_Activity pc on pc.PackageID =a.PackageID inner join Activity c on c.ActivityID=pc.ActivityID where a.ServiceTypeID=@0 and pc.ActivityID in (@2) and g.GeoTreeID in (Select GeoTreeID from GetChildGeos(@1))", (int)st, geoloc.GeoTreeID, ActID).ToList();
                }
                else if ( ActID!=null && CatID!=null)
                {
                    pack = db.Query<PackageDets>("Select a.PackageID,a.ServiceTypeID,a.PackageName,a.Description,a.Duration,a.Itinerary,a.Dificulty,a.GroupSize,a.StartTime,a.Inclusion,a.Exclusion,a.HighLights,pg.GeoTreeID,g.GeoName,pc.CategoryID,pc.ActivityID From Package a" +
                              $"  inner join Package_Geotree pg on a.PackageID=pg.PackageID inner join GeoTree g on g.GeoTreeID = pg.GeoTreeID inner join Package_Activity pa on pa.PackageID =a.PackageID inner join Activity a on a.ActivityID=pa.ActivityID inner join Package_Category pc on pc.PackageID = a.PackageID inner join Category c on c.CategoryID = pc.CategoryID where a.ServiceTypeID=@0 and pc.CategoryID in (@2) and pa.ActivityID = (@3) and g.GeoTreeID in (Select GeoTreeID from GetChildGeos(@1))", (int)st, geoloc.GeoTreeID,CatID ,ActID).ToList();
                }
                else if(CatID==null)
                {
                   pack = db.Query<PackageDets>("Select a.PackageID,a.ServiceTypeID,a.PackageName,a.Description,a.Duration,a.Itinerary,a.Dificulty,a.GroupSize,a.StartTime,a.Inclusion,a.Exclusion,a.HighLights,pg.GeoTreeID,g.GeoName From Package a" +
                            $"  inner join Package_Geotree pg on a.PackageID=pg.PackageID inner join GeoTree g on g.GeoTreeID = pg.GeoTreeID where a.ServiceTypeID=@0 and g.GeoTreeID in (Select GeoTreeID from GetChildGeos(@1))", (int)st, geoloc.GeoTreeID).ToList();                    

                }
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