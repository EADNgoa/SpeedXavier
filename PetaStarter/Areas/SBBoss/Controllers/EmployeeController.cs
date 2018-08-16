using Microsoft.AspNet.Identity;
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
    public class EmployeeController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Employee", Writable = false)]
        public ActionResult Index(int? page ,string Emp,DateTime? fd,DateTime? td)
        {
            if (Emp?.Length > 0) page = 1;
            var services = db.Query<SRdetailDets>("Select sd.TDate Tdate,sr.SRID,sr.ServiceTypeID,Cost,SellPrice from ServiceRequest sd inner join SRdetails sr on sr.SRID=sd.SRID where EmpID=@0", User.Identity.GetUserId()).ToList();
            services.ForEach(s =>
            {
                var tax = db.ExecuteScalar<decimal?>("Select Percentage From Taxes Where ServiceTypeID=@0 and WEF<GetDate() order by WEF desc ", s.ServiceTypeID) ?? 0;
                s.Tax = s.SellPrice * tax / 100;
                var st = (ServiceTypeEnum)s.ServiceTypeID;
                var commision = db.Fetch<ServiceCommision>($"Select Perc,Amount  From ServiceCommision Where ServiceName='{st}' ");
                commision.ForEach(c => {
                    if (c.Perc != null)
                    {
                        s.Commision = c.Perc ?? 0;
                        var tot = s.SellPrice * s.Commision / 100;
                        s.Total = tot + s.SellPrice + s.Tax;
                    }
                    else if (c.Amount != null)
                    {
                        s.Commision = c.Amount ?? 0;
                        s.Total = s.SellPrice + s.Tax + s.Commision;
                    }

                });


            });
            services = services.GroupBy(s => new { s.SRID, s.Tdate }).Select(group => new SRdetailDets { Total = group.Sum(a => a.Commision), SellPrice = group.Sum(a => a.SellPrice), SRID = group.Key.SRID, Tdate = group.Key.Tdate }).OrderByDescending(s => s.SRID).Where(s => s.Total > 0).ToList();

            if (fd !=null && td != null)
            {
                services=services.Where(s=>s.Tdate >= fd && s.Tdate <=td ).ToList();
            }
            else
            {
                services = services.Take(10).ToList();
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(services.ToPagedList(pageNumber, pageSize));
          
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
