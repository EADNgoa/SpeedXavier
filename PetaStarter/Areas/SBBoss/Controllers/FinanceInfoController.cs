using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetaPoco;
using Speedbird.Controllers;
using Speedbird.Models;
using Microsoft.AspNet.Identity;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class FinanceInfoController : EAController
    {
        // GET: SBBoss/FinanceInfo
        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        public ActionResult Index(PayToEnum mode)
        {
            ViewBag.Mode = (int)mode;
            ViewBag.Type = Enum.GetValues(typeof(AmtType)).Cast<AmtType>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            return View();
        }

        // GET: SBBoss/FinanceInfo
        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        [HttpPost]
        public ActionResult Index(PaymentViewModel paymentView, int? mode)
        {
            using (var transaction = db.GetTransaction())
            {

                try
                {
                    var payment = new Payment
                    {
                        AgentId = paymentView.AgentId,
                        Amount = paymentView.Amount,
                        SupplierID = paymentView.SupplierID,
                        DriverID = paymentView.DriverID,
                        BankID = paymentView.BankID,
                        ChequeNo = paymentView.ChequeNo,
                        Note = paymentView.Note,
                        TDate = DateTime.Now,
                        TransactionNo = paymentView.TransactionNo,
                        Type = paymentView.Type,
                        CustomerID = paymentView.CustomerID
                    };
                    db.Insert(payment);

                    string note = null;
                    string driver = null;
                    string supplier = null;
                    string agent = null;
                    string walkin = null;
         
                    if (payment.DriverID != null)
                    {
                        driver = db.SingleOrDefault<Driver>(paymentView.DriverID).DriverName;
                    }
                    if (payment.SupplierID != null)
                    {
                        supplier = db.SingleOrDefault<Supplier>(paymentView.SupplierID).SupplierName;
                    }
                    if(payment.AgentId != null)
                    {
                        agent = db.ExecuteScalar<string>("select ContactName from Agent where AgentId = @0", paymentView.AgentId);
                    }
                    if (payment.CustomerID != null)
                    {
                        walkin = db.ExecuteScalar<string>("Select Concat(FName,'',SName) as Name from Customer where CustomerID = @0",paymentView.CustomerID);
                    }
                    paymentView.Note = " User Note: " + paymentView.Note;
                    if (mode == (int)PayToEnum.Driver)
                    {
                        note = $"Paid to {driver} Rs. {paymentView.Amount} towards settlement of bookings " + paymentView.Note;
                    }
                    if (mode == (int)PayToEnum.Supplier)
                    {
                        note = $"Paid to {supplier} Rs. {paymentView.Amount} towards settlement of bookings " + paymentView.Note;
                    }
                    if (mode == (int)PayToEnum.Agent)
                    {
                        note = $"Recieved from {agent} Rs. {paymentView.Amount} towards settlement of bookings " + paymentView.Note;
                    }
                    if (mode == (int)PayToEnum.Walkin)
                    {
                        note = $"Paid to {walkin} Rs. {paymentView.Amount} towards settlement of bookings " + paymentView.Note;
                    }

                    decimal TotalSell, creditAmt = 0;
                    TotalSell = creditAmt = 0;

                    //getting credit amount of that agent
                    if(mode == (int)PayToEnum.Agent)
                    {
                        creditAmt = db.ExecuteScalar<decimal>($"Select coalesce(CreditAmt,0) from Agent where AgentID =  '{payment.AgentId}'");

                        TotalSell = db.ExecuteScalar<decimal>("select coalesce(Sum(srd.SellPrice),0) as TotalCost from SRdetails srd " +
                            "inner join ServiceRequest srq on srq.SRID = srd.SRID " +
                            $"inner join Agent ag on ag.AgentId = srq.AgentID where ag.AgentId = '{payment.AgentId}' and PaymentID is null");
                    }

                    if (paymentView.SelectedSRDID_Left != null)
                    {
                        foreach (var lsrdid in paymentView.SelectedSRDID_Left)
                        {
                            if(mode == (int)PayToEnum.Supplier)
                            {
                                db.Execute($"Update SRdetails set SupplierPaymentId = {payment.PaymentID} where SRDID = {lsrdid}");
                                var Srid = db.SingleOrDefault<int>("Select SRID from SRdetails where SRDID = @0", lsrdid);
                                db.Insert(new SRlog { SRID = Srid, LogDateTime = DateTime.Now, UserID = User.Identity.GetUserId(), SRDID = lsrdid, Event = note, Type = true });
                            }

                            if (mode == (int)PayToEnum.Agent)
                            {
                                db.Execute($"Update SRdetails set AgentPaymentId = {payment.PaymentID} where SRDID = {lsrdid}");
                                var Srid = db.SingleOrDefault<int>("Select SRID from SRdetails where SRDID = @0", lsrdid);
                                bool UnconfirmedRecordsExists = db.Fetch<int?>($"Select SRDID from SRdetails where SRID = {Srid} and AgentPaymentId is null and SRDID <> {lsrdid}").Any();
                                if (!UnconfirmedRecordsExists)
                                {
                                    db.Execute($"Update ServiceRequest set SRStatusID = {(int)SRStatusEnum.Confirmed},PayStatusID = {(int)PayType.Full_Paid} where SRID = {Srid}");                          
                                }
                                db.Insert(new SRlog { SRID = Srid, LogDateTime = DateTime.Now, UserID = User.Identity.GetUserId(), SRDID = lsrdid, Event = note, Type = true });
                            }

                            if(mode == (int)PayToEnum.Driver)
                            {
                                string PayTo = db.SingleOrDefault<string>($"Select Coalesce(PayTo,null) from SRdetails where SRDID = {lsrdid}");
                                if(PayTo == "Payed to us")
                                {
                                    db.Execute($"Update SRdetails set AgentPaymentId = {payment.PaymentID} where SRDID = {lsrdid}");
                                    var Srid = db.SingleOrDefault<int>("Select SRID from SRdetails where SRDID = @0", lsrdid);
                                    bool UnconfirmedRecordsExists = db.Fetch<int?>($"Select SRDID from SRdetails where SRID = {Srid} and AgentPaymentId is null and SRDID <> {lsrdid}").Any();
                                    if (!UnconfirmedRecordsExists)
                                    {
                                        db.Execute($"Update ServiceRequest set SRStatusID = {(int)SRStatusEnum.Confirmed},PayStatusID = {(int)PayType.Full_Paid} where SRID = {Srid}");
                                    }
                                    db.Insert(new SRlog { SRID = Srid, LogDateTime = DateTime.Now, UserID = User.Identity.GetUserId(), SRDID = lsrdid, Event = note, Type = true });
                                }

                                if (PayTo == "Pay to driver")
                                {
                                    db.Execute($"Update SRdetails set SupplierPaymentId = {payment.PaymentID} where SRDID = {lsrdid}");
                                    var Srid = db.SingleOrDefault<int>("Select SRID from SRdetails where SRDID = @0", lsrdid);
                                    db.Insert(new SRlog { SRID = Srid, LogDateTime = DateTime.Now, UserID = User.Identity.GetUserId(), SRDID = lsrdid, Event = note, Type = true });
                                }
                                
                            }

                            if (mode == (int)PayToEnum.Walkin)
                            {
                                db.Execute($"Update SRdetails set AgentPaymentId = {payment.PaymentID} where SRDID = {lsrdid}");
                                var Srid = db.SingleOrDefault<int>("Select SRID from SRdetails where SRDID = @0", lsrdid);
                                bool UnconfirmedRecordsExists = db.Fetch<int?>($"Select SRDID from SRdetails where SRID = {Srid} and AgentPaymentId is null and SRDID <> {lsrdid}").Any();
                                if (!UnconfirmedRecordsExists)
                                {
                                    db.Execute($"Update ServiceRequest set SRStatusID = {(int)SRStatusEnum.Confirmed},PayStatusID = {(int)PayType.Full_Paid} where SRID = {Srid}");
                                }
                                var LogVal = db.Insert(new SRlog { SRID = Srid, LogDateTime = DateTime.Now, UserID = User.Identity.GetUserId(), SRDID = lsrdid, Event = note, Type = true });
                               
                            }


                        }
                    }

                    if (paymentView.SelectedSRDID_Right != null)
                    {
                        foreach (var rsrdid in paymentView.SelectedSRDID_Right)
                        {


                            if (mode == (int)PayToEnum.Supplier)
                            {
                                db.Execute($"Update SRdetails set SupplierPaymentId = {payment.PaymentID},CancelledSupplierPaymentId = {payment.PaymentID} where SRDID = {rsrdid}");
                                var Srid = db.SingleOrDefault<int>("Select SRID from SRdetails where SRDID = @0", rsrdid);
                                db.Insert(new SRlog { SRID = Srid, LogDateTime = DateTime.Now, UserID = User.Identity.GetUserId(), SRDID = rsrdid, Event = note, Type = true });
                            }

                            if (mode == (int)PayToEnum.Agent)
                            {
                                db.Execute($"Update SRdetails set AgentPaymentId = {payment.PaymentID},CancelledAgentPaymentId = {payment.PaymentID} where SRDID = {rsrdid}");
                                var Srid = db.SingleOrDefault<int>("Select SRID from SRdetails where SRDID = @0", rsrdid);
                                bool UnconfirmedRecordsExists = db.Fetch<int?>($"Select SRDID from SRdetails where SRID = {Srid} and AgentPaymentId is null and SRDID <> {rsrdid}").Any();
                                if (!UnconfirmedRecordsExists)
                                {
                                    db.Execute($"Update ServiceRequest set SRStatusID = {(int)SRStatusEnum.Confirmed},PayStatusID = {(int)PayType.Full_Paid} where SRID = {Srid}");
                                }
                                db.Insert(new SRlog { SRID = Srid, LogDateTime = DateTime.Now, UserID = User.Identity.GetUserId(), SRDID = rsrdid, Event = note, Type = true });
                            }

                            if (mode == (int)PayToEnum.Driver)
                            {
                                string PayTo = db.SingleOrDefault<string>($"Select Coalesce(PayTo,null) from SRdetails where SRDID = {rsrdid}");
                                if (PayTo == "Payed to us")
                                {
                                    db.Execute($"Update SRdetails set AgentPaymentId = {payment.PaymentID},CancelledAgentPaymentId = {payment.PaymentID} where SRDID = {rsrdid}");
                                    var Srid = db.SingleOrDefault<int>("Select SRID from SRdetails where SRDID = @0", rsrdid);
                                    bool UnconfirmedRecordsExists = db.Fetch<int?>($"Select SRDID from SRdetails where SRID = {Srid} and AgentPaymentId is null and SRDID <> {rsrdid}").Any();
                                    if (!UnconfirmedRecordsExists)
                                    {
                                        db.Execute($"Update ServiceRequest set SRStatusID = {(int)SRStatusEnum.Confirmed},PayStatusID = {(int)PayType.Full_Paid} where SRID = {Srid}");
                                    }
                                    db.Insert(new SRlog { SRID = Srid, LogDateTime = DateTime.Now, UserID = User.Identity.GetUserId(), SRDID = rsrdid, Event = note, Type = true });
                                }

                                if (PayTo == "Pay to driver")
                                {
                                    db.Execute($"Update SRdetails set SupplierPaymentId = {payment.PaymentID},CancelledSupplierPaymentId = {payment.PaymentID} where SRDID = {rsrdid}");
                                    var Srid = db.SingleOrDefault<int>("Select SRID from SRdetails where SRDID = @0", rsrdid);
                                    db.Insert(new SRlog { SRID = Srid, LogDateTime = DateTime.Now, UserID = User.Identity.GetUserId(), SRDID = rsrdid, Event = note, Type = true });
                                }

                            }

                            if (mode == (int)PayToEnum.Walkin)
                            {
                                db.Execute($"Update SRdetails set CancelledAgentPaymentId = {payment.PaymentID} where SRDID = {rsrdid}");
                                var Srid = db.SingleOrDefault<int>("Select SRID from SRdetails where SRDID = @0", rsrdid);
                                db.Insert(new SRlog { SRID = Srid, LogDateTime = DateTime.Now, UserID = User.Identity.GetUserId(), SRDID = rsrdid, Event = note, Type = true });
                            }
                        }
                    }

                    transaction.Complete();
                    return RedirectToAction("Index", new { mode });
                }
                catch (Exception ex)
                {
                    db.AbortTransaction();
                    throw ex;
                }

            }
        }

        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        public PartialViewResult _FinanceInfo(int? mode, DateTime? FromDate, DateTime? ToDate, int? DriverID, bool? All, bool? Paid, bool? Pending, int? SupplierID, string AgentId, int? CustomerID)
        {
            PaymentViewModel paymentdetailvws = new PaymentViewModel();
            Sql leftsq = new Sql();
            Sql rightsq = new Sql();

            if (mode == (int)PayToEnum.Driver)
            {
                leftsq.Append("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
                         "srd.SRDID,srq.SRID, srd.Cost, srd.AgentPaymentId,srd.SupplierPaymentId,srd.CancelledAgentPaymentId,srd.CancelledSupplierPaymentId,srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo " +
                         "from SRdetails srd, ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc where sc.SRDID = srd.SRDID " +
                         "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and IsLead = 1 " +
                        $"and DriverID = {DriverID} and srd.PayTo = 'Payed to us' and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}'");

                rightsq.Append("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
                       "srd.SRDID,srq.SRID, srd.Cost, srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo,srd.AgentPaymentId,srd.SupplierPaymentId " +
                       "from SRdetails srd, ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc where sc.SRDID = srd.SRDID " +
                       "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and IsLead = 1 " +
                       $"and DriverID = {DriverID} and srd.PayTo = 'Pay to driver' and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}'");
            }

            if (mode == (int)PayToEnum.Supplier)
            {
                leftsq.Append("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
                        "srd.SRDID,srq.SRID, srd.Cost, srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo,Coalesce(IsCancelled,0) as IsCancelled,srd.SupplierPaymentId " +
                        "from SRdetails srd, ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc where sc.SRDID = srd.SRDID " +
                        "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and IsLead = 1 " +
                       $"and IsCancelled is null and SupplierID = {SupplierID} and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}'");

                rightsq.Append("Select CONCAT(c.fName, ' ',c.sName) as PaxName, srd.SRDID,srq.SRID, srd.Cost, srq.BookingNo,srd.CancelledSupplierPaymentId, srd.Fdate, SellPrice, srd.PayTo,srd.SupplierPaymentId, " +
                    "rf.SRDID,rf.ProdCanxCost, rf.SBCanxCost,Coalesce(IsCancelled,0)as IsCancelled, rf.RefundId from SRdetails srd, ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc, Refunds rf " +
                    "where sc.SRDID = srd.SRDID and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID " +
                    "and rf.SRDID = srd.SRDID and IsLead = 1 " +
                    $"and IsCancelled = 1 and srd.CancelledSupplierPaymentId is null and SupplierID = {SupplierID} and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}'");
            }

            if (mode == (int)PayToEnum.Agent)
            {
                leftsq.Append("Select CONCAT(c.fName, ' ',c.sName) as PaxName, srd.SRDID,srq.SRID, srd.Cost,srd.AgentPaymentId, " +
                    "srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo,Coalesce(IsCancelled,0) as IsCancelled from SRdetails srd, " +
                    "ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc where sc.SRDID = srd.SRDID " +
                    "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and IsLead = 1 " +
                    $"and IsCancelled is null and AgentId = '{AgentId}' and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}'");

                rightsq.Append("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
                   "srd.SRDID,srq.SRID, srd.Cost, srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo,srd.AgentPaymentId,Coalesce(IsCancelled,0) as IsCancelled,srd.CancelledAgentPaymentId,rf.ProdCanxCost,rf.SRDID, rf.SBCanxCost, rf.RefundId " +
                   "from SRdetails srd, ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc, Refunds rf where sc.SRDID = srd.SRDID " +
                   "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and rf.SRDID = srd.SRDID and IsLead = 1 " +
                   $"and IsCancelled = 1 and srd.CancelledAgentPaymentId is null and AgentId = '{AgentId}' and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}'");
            }

            if (mode == (int)PayToEnum.Walkin)
            {
                leftsq.Append("Select CONCAT(c.fName, ' ',c.sName) as PaxName, srd.SRDID,srq.SRID, srd.Cost,srd.AgentPaymentId, " +
                    "srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo,Coalesce(IsCancelled,0) as IsCancelled from SRdetails srd, " +
                    "ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc where sc.SRDID = srd.SRDID " +
                    "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and IsLead = 1 " +
                    $"and IsCancelled is null and c.CustomerID = {CustomerID} and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}'");

                rightsq.Append("Select CONCAT(c.fName, ' ',c.sName) as PaxName, " +
                   "srd.SRDID,srq.SRID, srd.Cost, srq.BookingNo, srd.Fdate, SellPrice, srd.PayTo,Coalesce(IsCancelled,0) as IsCancelled,srd.AgentPaymentId,srd.CancelledAgentPaymentId,rf.ProdCanxCost,rf.SRDID, rf.SBCanxCost, rf.RefundId " +
                   "from SRdetails srd, ServiceRequest srq, SR_Cust src, Customer c, SRD_Cust sc, Refunds rf where sc.SRDID = srd.SRDID " +
                   "and c.CustomerID = sc.CustomerID and srq.SRID = srd.SRID and src.ServiceRequestID = srq.SRID and rf.SRDID = srd.SRDID and IsLead = 1 " +
                   $"and IsCancelled = 1 and c.CustomerID = {CustomerID} and srd.Fdate between '{String.Format("{0:yyyy-MM-dd}", FromDate)}' and '{String.Format("{0:yyyy-MM-dd}", ToDate)}'");
            }


            if (Paid == true && mode == (int)PayToEnum.Driver)
            {
                leftsq.Append($" and srd.AgentPaymentId is not null");
                rightsq.Append($" and srd.SupplierPaymentId is not null");
            }

            if (Pending == true && mode == (int)PayToEnum.Driver)
            {
                leftsq.Append($" and srd.AgentPaymentId is null");
                rightsq.Append($" and srd.SupplierPaymentId is null");
            }

            if (Paid == true && mode == (int)PayToEnum.Supplier)
            {
                leftsq.Append($" and srd.SupplierPaymentId is not null");
                rightsq.Append($" and srd.CancelledSupplierPaymentId is not null");
            }


            if (Pending == true && mode == (int)PayToEnum.Supplier)
            {
                leftsq.Append($" and srd.SupplierPaymentId is null");
                rightsq.Append($" and srd.CancelledSupplierPaymentId is null");
            }

            if (Paid == true && mode == (int)PayToEnum.Agent)
            {
                leftsq.Append($" and srd.AgentPaymentId is not null");
                rightsq.Append($" and srd.CancelledAgentPaymentId is not null");
            }

            if (Pending == true && mode == (int)PayToEnum.Agent)
            {
                leftsq.Append($" and srd.AgentPaymentId is null");
                rightsq.Append($" and srd.CancelledAgentPaymentId is null");
            }

            if (Paid == true && mode == (int)PayToEnum.Walkin)
            {
                leftsq.Append($" and srd.AgentPaymentId is not null");
                rightsq.Append($" and srd.CancelledAgentPaymentId is not null");
            }

            if (Pending == true && mode == (int)PayToEnum.Walkin)
            {
                leftsq.Append($" and srd.AgentPaymentId is null");
                rightsq.Append($" and srd.CancelledAgentPaymentId is null");
            }


            paymentdetailvws.LeftBkPayVm = db.Query<BKPayVM>(leftsq).ToList();
            paymentdetailvws.RightBkPayVm = db.Query<BKPayVM>(rightsq).ToList();
            ViewBag.DriverID = DriverID;
            ViewBag.SupplierID = SupplierID;
            ViewBag.AgentId = AgentId;
            ViewBag.CustomerId = CustomerID;
            ViewBag.Mode = mode;
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
            var filteredItems = db.Fetch<AspNetUser>($"Select au.Id,au.RealName from AspNetUsers au " +
                $"inner join Agent ag on ag.AgentId = au.Id Where au.RealName like '%{term}%'").Select(c => new { id = c.Id, value = c.RealName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        //[EAAuthorize(FunctionName = "FinanceInfo", Writable = false)]
        public ActionResult AutoCompleteDriver(string term)
        {
            var filteredItems = db.Fetch<Driver>($"Select DriverID,DriverName from Driver Where DriverName like '%{term}%'").Select(c => new { id = c.DriverID, value = c.DriverName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExistingCustRec(string fn, string sn, string ph, string em)
        {
            var sql = new PetaPoco.Sql("Select * from Customer where 1=1");

            if (fn != null && fn.Length > 0)
                sql.Append($" and LOWER(FName) like '%{fn.ToLower()}%'");

            if (sn != null && fn.Length > 0)
                sql.Append($" and LOWER(SName) like '%{sn.ToLower()}%'");

            if (ph != null && fn.Length > 0)
                sql.Append($" and Phone like '%{ph}%'");

            if (em != null && fn.Length > 0)
                sql.Append($" and LOWER(Email) like '%{em.ToLower()}%'");

            var recs = db.Query<CustomerDets>(sql);

            return PartialView("CustomerSearchPartial", recs);
        }

    }

}
