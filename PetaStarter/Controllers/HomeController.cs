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
                ViewBag.loc = "Interational";
            }
            ViewBag.Accomodatation = db.FirstOrDefault<AccomodationDets>("Select Top 1 * From Accomodation a inner join Picture p on a.AccomodationID = p.ServiceID");
            ViewBag.Package = db.FirstOrDefault<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID where a.ServiceTypeID={(int)ServiceTypeEnum.Packages}");
            ViewBag.SightSeeing = db.FirstOrDefault<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID where a.ServiceTypeID={(int)ServiceTypeEnum.SightSeeing}");
            ViewBag.Cruise = db.FirstOrDefault<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID where a.ServiceTypeID={(int)ServiceTypeEnum.Cruise}");
            ViewBag.CarBike = db.FirstOrDefault<CarBikeDets>($"Select Top 1 * From CarBike a inner join Picture p on a.CarBikeID = p.ServiceID ");
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