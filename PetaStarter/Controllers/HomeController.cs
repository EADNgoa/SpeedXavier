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
            return View();
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
          
            ViewBag.Accomodation = db.FirstOrDefault<AccomodationDets>("Select Top 1 * From Accomodation a inner join Picture p on a.AccomodationID = p.ServiceID");
            ViewBag.Package = db.FirstOrDefault<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID where a.ServiceTypeID={(int)ServiceTypeEnum.Packages}");
            ViewBag.SightSeeing = db.FirstOrDefault<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID where a.ServiceTypeID={(int)ServiceTypeEnum.SightSeeing}");
            ViewBag.Cruise = db.FirstOrDefault<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID where a.ServiceTypeID={(int)ServiceTypeEnum.Cruise}");
            ViewBag.CarBike = db.FirstOrDefault<CarBikeDets>($"Select Top 1 * From CarBike a inner join Picture p on a.CarBikeID = p.ServiceID ");
            var cat = db.Fetch<CategoryRec>("Select * from Category");
            cat.ForEach(c=>
            {
                c.pack = db.Fetch<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID inner join Package_Category pc on a.PackageID = pc.PackageID inner join Category c on c.CategoryID = pc.CategoryID where pc.CategoryID={c.CategoryID}").ToList(); 
            });
            return View(cat);
        }

        public ActionResult LoadPackages(ServiceTypeEnum? st,string Loc,int? CatID)
        {
            var geoloc = db.FirstOrDefault<GeoTree>("Select * from GeoTree where GeoName = @0",Loc);

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
            if (st==ServiceTypeEnum.Packages || st == ServiceTypeEnum.SightSeeing || st == ServiceTypeEnum.Cruise)
            {
                ViewBag.ServiceTitle = "Our Best Tours And Excursions";
                if (st == ServiceTypeEnum.Packages || st == ServiceTypeEnum.SightSeeing)
                {
                    ViewBag.Cats = db.Fetch<Category>("Select * From Category");

                    ViewBag.Acts = db.Fetch<Activity>("Select * From Activity");
                }
                ViewBag.st = st;
                ViewBag.Package = db.Fetch<PackageDets>($"Select * From Package a  inner join Package_Geotree pg on a.PackageID=pg.PackageID inner join GeoTree g on g.GeoTreeID = pg.GeoTreeID inner join Package_Category pc on a.PackageID=pc.PackageID inner join Category c on c.CategoryID= pc.CategoryID inner join Picture p on a.PackageID = p.ServiceID inner join Prices rs on a.PackageID=rs.ServiceID where a.ServiceTypeID=@0 and pc.CategoryID Like '%" +CatID+ "%'  and g.GeoTreeID in (Select GeoTreeID from GetChildGeos(@1))", (int)st,geoloc.GeoTreeID);
             
            }
            return View();
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