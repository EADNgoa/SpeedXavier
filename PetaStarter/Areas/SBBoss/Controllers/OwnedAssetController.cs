
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
    public class OwnedAssetController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "OwnedAsset", Writable = false)]
        public ActionResult Index(int? page ,DateTime? fd,DateTime? td,ServiceTypeEnum? ServiceTypeID )
        {
            if (fd!=null) page = 1;
            ViewBag.ServiceTypeID = Enum.GetValues(typeof(ServiceTypeEnum)).Cast<ServiceTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).Where(a=>a.Value== ((int)0).ToString() || a.Value== ((int)4).ToString()).ToList();

            var carbike = db.Query<SRdetailDets>("Select cb.CarBikeName as SupplierName,sr.SRID,sr.TDate as Tdate,sd.SRDID,sd.ServiceTypeID,Cost,SellPrice,ECommision from SRdetails sd inner join CarBike cb on cb.CarBikeID = sd.ItemID and ServiceTypeID=@0 inner join ServiceRequest sr on sr.SRID = sd.SRID where cb.SelfOwned =1", (int)ServiceTypeEnum.CarBike).ToList();
            var accom = db.Query<SRdetailDets>("Select ac.AccomName as SupplierName,sr.SRID,sr.TDate as Tdate,sd.SRDID,sd.ServiceTypeID,Cost,SellPrice,ECommision from SRdetails sd inner join Accomodation ac on ac.AccomodationID = sd.ItemID and ServiceTypeID=@0 inner join ServiceRequest sr on sr.SRID = sd.SRID where ac.SelfOwned =1", (int)ServiceTypeEnum.Accomodation).ToList();
        
            var services = carbike.Concat(accom).ToList();
            if (ServiceTypeID != null)
            {
                services = services.Where(s => s.ServiceTypeID == (int)ServiceTypeID).ToList();
            }
            if (fd != null && td!=null)
            {
                services = services.Where(a=>a.Tdate.Date>=fd &&a.Tdate<=td).ToList();
            }

            services.ForEach(s =>
            {
                var tax = db.ExecuteScalar<decimal?>("Select Percentage From Taxes Where ServiceTypeID=@0 and WEF<GetDate() order by WEF desc ", s.ServiceTypeID) ?? 0;
                s.Tax = s.SellPrice * tax / 100;
                var c = db.FirstOrDefault<ServiceCommision>($"Select Perc,Amount  From ServiceCommision Where Serviceid={s.ServiceTypeID + 1} ");

                if (s.ECommision < 0)
                {
                    if(c.Perc != null)
                    {
                        s.Commision = c.Perc ?? 0;
                        s.PercComm = "%";
                        var tot = s.SellPrice * s.Commision / 100;
                        s.Total = s.SellPrice - s.Tax - tot - s.Cost;
                    }
                    else if(c.Amount != null)
                    {
                        s.Commision = c.Amount ?? 0;
                        s.Total = s.SellPrice - s.Tax - s.Commision - s.Cost;
                    }
                }
                else
                {
                    s.Commision = s.ECommision;
                    s.Total = s.SellPrice - s.Tax - s.Commision - s.Cost;
                  
                }

            });
            services = services.GroupBy(s =>new { s.ServiceTypeID,s.SupplierName ,s.Tdate}).Select(y => new Speedbird.SRdetailDets
            {
                ServiceTypeID = y.Key.ServiceTypeID,
                SellPrice = y.Sum(x => x.SellPrice),
                Cost = y.Sum(x => x.Cost),
                Commision = y.Sum(x => x.Commision),
                Tax = y.Sum(x => x.Tax),
                SRID=y.First().SRID,
                Total = y.Sum(x => x.Total),
                SupplierName=y.Key.SupplierName,
                Tdate=y.Key.Tdate







            }).ToList();



            return View(services);
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
