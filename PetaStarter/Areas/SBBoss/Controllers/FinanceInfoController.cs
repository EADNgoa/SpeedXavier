using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Speedbird.Controllers;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class FinanceInfoController : EAController
    {
        // GET: SBBoss/FinanceInfo
        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        public ActionResult Index()
        {
            return View();
        }

        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        public PartialViewResult _FinanceInfo(DateTime? FromDate, DateTime? ToDate, int? DriverID)
        {

                ViewBag.PayToDriver = db.Query<Paymentdetailvw>("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
                "d.DriverID, d.DriverName, srq.SRID, srd.SRDID, srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo " +
                "from SRdetails srd, ServiceRequest srq, Driver d, SR_Cust src, Customer c, SRD_Cust sc " +
                "where d.DriverID = srd.DriverID " +
                "and sc.SRDID = srd.SRDID " +
                "and c.CustomerID = sc.CustomerID " +
                "and srq.SRID = srd.SRID " +
                "and src.ServiceRequestID = srq.SRID " +
                "and IsLead = 1 " +
                "and srd.PayTo = 'Pay to driver' " +
               $"and d.DriverID = {DriverID} and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}' ");

            ViewBag.PayToUs = db.Query<Paymentdetailvw>("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
               "d.DriverID, d.DriverName, srq.SRID, srd.SRDID, srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo " +
               "from SRdetails srd, ServiceRequest srq, Driver d, SR_Cust src, Customer c, SRD_Cust sc " +
               "where d.DriverID = srd.DriverID " +
               "and sc.SRDID = srd.SRDID " +
               "and c.CustomerID = sc.CustomerID " +
               "and srq.SRID = srd.SRID " +
               "and src.ServiceRequestID = srq.SRID " +
               "and IsLead = 1 " +
               "and srd.PayTo = 'Pay to us' " +
               $"and d.DriverID = {DriverID} and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}' ");

            return PartialView("_FinanceInfo");
        }

        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        public ActionResult AutoCompleteDriver(string term)
        {
            var filteredItems = db.Fetch<Driver>($"Select DriverID,DriverName from Driver Where DriverName like '%{term}%'").Select(c => new { id = c.DriverID, value = c.DriverName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
    }
}