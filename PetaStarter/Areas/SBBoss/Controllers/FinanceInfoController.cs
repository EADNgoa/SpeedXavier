using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Speedbird.Controllers;
using Speedbird.Models;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class FinanceInfoController : EAController
    {
        // GET: SBBoss/FinanceInfo
        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        public ActionResult Index(int mode)
        {
            ViewBag.Mode = mode;
            ViewBag.Type = Enum.GetValues(typeof(AmtType)).Cast<AmtType>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            return View();
        }

        // GET: SBBoss/FinanceInfo
        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        [HttpPost]
        public ActionResult Index(PaymentViewModel paymentView)
        {
            using (var transaction = db.GetTransaction())
            {
            }
                return RedirectToAction("Index");
        }

        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        public PartialViewResult _FinanceInfo(DateTime? FromDate, DateTime? ToDate, int? DriverID, bool? All, bool? Paid, bool? Pending, int? SupplierID, string AgentId)
        {
            PaymentViewModel paymentdetailvws = new PaymentViewModel();

            if (All == true)
            {
                paymentdetailvws.LeftBkPayVm = db.Query<BKPayVM>("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
                    "srd.SRDID, srd.Cost, srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo " +
                    "from SRdetails srd, ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc where sc.SRDID = srd.SRDID " +
                    "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and IsLead = 1 " +
                    "and srd.PayTo = 'Pay to us' " +
                   $"srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}' ");

                paymentdetailvws.RightBkPayVm = db.Query<BKPayVM>("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
                   "srd.SRDID, srd.Cost, srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo " +
                   "from SRdetails srd, ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc where sc.SRDID = srd.SRDID " +
                   "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and IsLead = 1 " +
                   "and srd.PayTo = 'Pay to driver' " +
                   $"and DriverID = {DriverID} and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}' ");
            }
            if (Paid == true)
            {
                paymentdetailvws.LeftBkPayVm = db.Query<BKPayVM>("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
                    "srd.SRDID, srd.Cost, srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo " +
                    "from SRdetails srd, ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc where sc.SRDID = srd.SRDID " +
                    "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and IsLead = 1 " +
                    "and srd.PayTo = 'Pay to us' " +
                    "and PaymentID is not null " +
                   $"and DriverID = {DriverID} and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}' ");

                paymentdetailvws.RightBkPayVm = db.Query<BKPayVM>("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
                    "srd.SRDID, srd.Cost, srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo " +
                    "from SRdetails srd, ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc where sc.SRDID = srd.SRDID " +
                    "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and IsLead = 1 " +
                    "and srd.PayTo = 'Pay to driver' " +
                    "and PaymentID is not null " +
                    $"and DriverID = {DriverID} and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}' ");
            }
            if (Pending == true)
            {
                paymentdetailvws.LeftBkPayVm = db.Query<BKPayVM>("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
                    "srd.SRDID, srd.Cost, srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo " +
                    "from SRdetails srd, ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc where sc.SRDID = srd.SRDID " +
                    "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and IsLead = 1 " +
                    "and srd.PayTo = 'Pay to us' " +
                    "and PaymentID is null " +
                   $"and DriverID = {DriverID} and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}' ");

                paymentdetailvws.RightBkPayVm = db.Query<BKPayVM>("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
                   "srd.SRDID, srd.Cost, srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo " +
                   "from SRdetails srd, ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc where sc.SRDID = srd.SRDID " +
                   "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and IsLead = 1 " +
                   "and srd.PayTo = 'Pay to driver' " +
                   "and PaymentID is null " +
                   $"and DriverID = {DriverID} and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}' ");
            }

            return PartialView("_FinanceInfo", paymentdetailvws);
        }

        public ActionResult FetchRcptpartial(int? id, int Type)
        {
            ViewBag.RPDID = id;
            ViewBag.BankName = db.Query<Bank>("Select * from Banks").Select(sl => new SelectListItem { Text = sl.BankName, Value = sl.BankID.ToString(), Selected = true });
            return PartialView($"_{((AmtType)Type).ToString()}", db.SingleOrDefault<RPdet>(id));
        }

        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        public ActionResult AutoCompleteSupplier(string term)
        {
            var filteredItems = db.Fetch<Supplier>($"Select SupplierID,SupplierName from Supplier Where SupplierName like '%{term}%'").Select(c => new { id = c.SupplierID, value = c.SupplierName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        public ActionResult AutoCompleteAgent(string term)
        {
            var filteredItems = db.Fetch<Agent>($"Select ag.AgentId,ag.ContactName from Agent ag inner join AspNetUsers au on au.Id = ag.AgentId Where ContactName like '%{term}%'").Select(c => new { id = c.AgentId, value = c.ContactName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        public ActionResult AutoCompleteDriver(string term)
        {
            var filteredItems = db.Fetch<Driver>($"Select DriverID,DriverName from Driver Where DriverName like '%{term}%'").Select(c => new { id = c.DriverID, value = c.DriverName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoCompleteDrivers(string term)
        {
            var filteredItems = db.Fetch<Driver>($"Select DriverID,DriverName from Driver Where DriverName like '%{term}%'").Select(c => new { id = c.DriverID, value = c.DriverName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
    }



}
