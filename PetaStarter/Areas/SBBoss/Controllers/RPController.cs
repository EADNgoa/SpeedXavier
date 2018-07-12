using Speedbird.Controllers;
using System;
//using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
//using System.Web;
using System.Web.Mvc;


namespace Speedbird.Areas.SBBoss.Controllers
{
    public class RPController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Reciept", Writable = false)]
        public ActionResult Reciept(string CustName,string AgentName,bool? IsSearch,decimal? OA,int? id,decimal? ManAmt,int? SRID,decimal? TotalAmt)
       {
            ViewBag.Type = Enum.GetValues(typeof(AmtType)).Cast<AmtType>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            decimal usedAmt = db.ExecuteScalar<decimal?>("Select Coalesce(sum(Amount),0) From RP_SR Where RPDID =@0", id) ?? 0;
            decimal ActualOA = db.ExecuteScalar<decimal?>("Select Coalesce(sum(SellPrice),0) From SRdetails Where SRID =@0", SRID) ?? 0;
            
            if (SRID != null)
            {
                usedAmt+= ManAmt ?? OA ?? 0;
                if (OA != null || ManAmt != null)
                {
                    if (OA <= TotalAmt || ManAmt <= TotalAmt)
                    {
                        if (ManAmt == null)
                        {
                            ManAmt = 0;
                        }
                        if ((ManAmt < OA && usedAmt <=TotalAmt ) ||( OA <= TotalAmt && ManAmt <=TotalAmt && usedAmt <=TotalAmt && ManAmt <= OA  ))
                        {
                            var PExist = db.ExecuteScalar<decimal?>("Select Coalesce(Amount,0) From RP_SR Where RPDID =@0 and  SRID = @1", id, SRID) ?? 0;
                            if (PExist != 0)
                            {
                                
                              
                                if (ManAmt <= OA && OA <= TotalAmt &&  PExist < (OA+ActualOA))
                                    db.Execute("Delete from RP_SR Where SRID=@0 and RPDID =@1", SRID, id);
                            }

                            if (ManAmt <= OA || (PExist < (OA + ActualOA) && OA <= TotalAmt))
                            {
                                if (ManAmt == 0)
                                    ManAmt = null;
                                ManAmt += PExist;
                                OA += PExist;
                                db.Insert(new RP_SR { SRID = (int)SRID, RPDID = (int)id, Amount = ManAmt ?? OA });
                                if (usedAmt == TotalAmt)
                                {
                                     db.Execute("Update RPDets set AmtUsed=@0 where RPDID=@1", true, id);

                                }
                                int pid = 0;
                                if (OA != null || OA ==ManAmt)
                                {
                                    pid = (int)PayType.Full_Paid;
                                }
                                if (ManAmt != null && (ManAmt<=(OA+PExist)) && OA != ManAmt)
                                {
                                    pid = (int)PayType.Part_Paid;
                                }

                                
                                db.Execute("Update ServiceRequest set PayStatusID =@0 Where SRID = @1 ",pid, SRID);
                            }

                        }
                    }
                }
            }
            
            var NPbkngs = new PetaPoco.Sql($"Select distinct sr.SRID, CONCAT(c.FName,' ' ,c.SName) as cName,anu.UserName," +
                "(select Coalesce(sum(SellPrice),0) From SRdetails Where SRID =sr.SRID) as OA ," +
                "(Select  Coalesce(Sum(Amount),0) from RP_SR rs Where SRID=sr.SRID) as PaidAmt " +
                "from ServiceRequest sr left join AspNetUsers anu on anu.Id = sr.AgentID left join Customer c on c.CustomerID =sr.CustID  " +
                $"Where sr.PayStatusID in (1,2)");
                if (CustName?.Length > 0)
                {
                    NPbkngs.Append($" and LOWER(CONCAT(c.FName,' ' ,c.SName)) like '%{CustName.ToLower()}%'");
                }
                if (AgentName?.Length >0)
                {
                string cha = "@";
                NPbkngs.Append($" and LOWER(anu.UserName) like '%{cha}%'",AgentName);
                }
          var bkngs = db.Query<SRBooking>(NPbkngs).Where(a=>a.OA > 0).ToList();
          ViewBag.UnUsedP = db.Fetch<RPDetails>("Select rp.RPDID,rp.Amount,rp.Type,(Select Coalesce(Sum(Amount),0) from RP_SR Where RPDID = rp.RPDID) as UnUsedAmt from RPdets rp  where AmtUsed is Null and IsPayment = @0",false);           
          decimal getT = db.ExecuteScalar<decimal?>("Select Amount from RPDets Where RPDID=@0",id) ?? 0;
            ViewBag.TotAmt = getT - usedAmt;
          ViewBag.Bookings = bkngs;
         
            if (IsSearch==true )
            {
                ViewBag.Reciepts = "true";
                return PartialView("_SearchBooking");
            }
            
            return View(base.BaseCreateEdit<RPdet>(id, "RPDID"));

        }
        public ActionResult Payment(int? SupplierID, bool? IsSearch, decimal? OA, int? id, decimal? ManAmt, int? SRID, decimal? TotalAmt)
        {
            ViewBag.Type = Enum.GetValues(typeof(AmtType)).Cast<AmtType>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            decimal usedAmt = db.ExecuteScalar<decimal?>("Select Coalesce(sum(Amount),0) From RP_SR Where RPDID =@0", id) ?? 0;
            decimal ActualOA = db.ExecuteScalar<decimal?>("Select Coalesce(sum(Cost),0) From SRdetails Where SRID =@0", SRID) ?? 0;

            if (SRID != null)
            {
                usedAmt += ManAmt ?? OA ?? 0;
                if (OA != null || ManAmt != null)
                {
                    if (OA <= TotalAmt || ManAmt <= TotalAmt)
                    {
                        if (ManAmt == null)
                        {
                            ManAmt = 0;
                        }
                        if ((ManAmt < OA && usedAmt <= TotalAmt) || (OA <= TotalAmt && ManAmt <= TotalAmt && usedAmt <= TotalAmt && ManAmt <= OA))
                        {
                            var PExist = db.ExecuteScalar<decimal?>("Select Coalesce(Amount,0) From RP_SR Where RPDID =@0 and  SRID = @1", id, SRID) ?? 0;
                            if (PExist != 0)
                            {


                                if (ManAmt <= OA && OA <= TotalAmt && PExist < (OA + ActualOA))
                                    db.Execute("Delete from RP_SR Where SRID=@0 and RPDID =@1", SRID, id);
                            }

                            if (ManAmt <= OA || (PExist < (OA + ActualOA) && OA <= TotalAmt))
                            {
                                if (ManAmt == 0)
                                    ManAmt = null;
                                ManAmt += PExist;
                                OA += PExist;
                                db.Insert(new RP_SR { SRID = (int)SRID, RPDID = (int)id, Amount = ManAmt ?? OA });
                                if (usedAmt == TotalAmt)
                                {
                                    db.Execute("Update RPDets set AmtUsed=@0 where RPDID=@1", true, id);

                                }
                             /*   int pid = 0;
                                if (OA != null || OA == ManAmt)
                                {
                                    pid = (int)PayType.Full_Paid;
                                }
                                if (ManAmt != null && (ManAmt <= (OA + PExist)) && OA != ManAmt)
                                {
                                    pid = (int)PayType.Part_Paid;
                                }


                                db.Execute("Update ServiceRequest set PayStatusID =@0 Where SRID = @1 ", pid, SRID);*/
                            }

                        }
                    }
                }
            }

            var NPbkngs = new PetaPoco.Sql("Select sd.SRID,sd.SupplierID,s.SupplierName as UserName,Sum(sd.Cost) as OA From SRdetails sd inner join Supplier s on s.SupplierID=sd.SupplierID inner join ServiceRequest sr on sr.SRID =sd.SRID Where sd.SupplierID is not null and sr.PayStatusID =0");
            if (SupplierID != null)
            {
                NPbkngs.Append($" and sd.SupplierID = {SupplierID}");
            }
            NPbkngs.Append(" Group By sd.SupplierID,sd.SRID,s.SupplierName");
            var bkngs = db.Query<SRBooking>(NPbkngs).Where(a => a.OA > 0).ToList();
            bkngs.ForEach(b=> {
                b.PaidAmt = db.ExecuteScalar<decimal?>("Select Sum(Amount) as PaidAmt From RP_SR where SRID = @0",b.SRID) ?? 0;

            });
            ViewBag.UnUsedP = db.Fetch<RPDetails>("Select rp.RPDID,rp.Amount,rp.Type,(Select Coalesce(Sum(Amount),0) from RP_SR Where RPDID = rp.RPDID) as UnUsedAmt from RPdets rp  where AmtUsed is Null and IsPayment =@0",true);
            decimal getT = db.ExecuteScalar<decimal?>("Select Amount from RPDets Where RPDID=@0", id) ?? 0;
            ViewBag.TotAmt = getT - usedAmt;
            ViewBag.Bookings = bkngs;

            if (IsSearch == true)
            {
                ViewBag.Payments = "true";
                return PartialView("_SearchBooking");
            }

            return View(base.BaseCreateEdit<RPdet>(id, "RPDID"));

        }
        [EAAuthorize(FunctionName = "Reciept", Writable = true)]
        public ActionResult SaveReciept([Bind(Include = "RPDID,BankName,ChequeNo,Date,Amount,TransactionNo,Note,Type,IsPayment")] RPdet item)
        {
            base.BaseSave<RPdet>(item, item.RPDID > 0);
            string p = "";
            if (item.IsPayment == false)
            {
                p = "Reciept";
            }
            else if(item.IsPayment==true)
            {
                p = "Payment";
            }
            return RedirectToAction(p, new { id = item.RPDID });
        }

        public ActionResult FetchRcptpartial(int? id, int Type)
        {
            ViewBag.RPDID = id;

            ViewBag.BankName = db.Query<Bank>("Select * from Banks").Select(sl => new SelectListItem { Text = sl.BankName, Value = sl.BankID.ToString(), Selected = true });

            switch ((AmtType)Type)
            {
                case AmtType.Cash:
                    return PartialView($"_{((AmtType)Type).ToString()}", db.SingleOrDefault<RPdet>(id));
                case AmtType.Cheque:
                    return PartialView($"_{((AmtType)Type).ToString()}", db.SingleOrDefault<RPdet>(id));
                case AmtType.Internet_Banking:

                    return PartialView($"_{((AmtType)Type).ToString()}", db.SingleOrDefault<RPdet>(id));

                default:
                    return PartialView("_NotFound");
            }
        }
        public ActionResult AutoCompleteSup(string term)
        {
            var filteredItems = db.Fetch<Supplier>($"Select * from Supplier Where SupplierName like '%{term}%'").Select(c => new { id = c.SupplierID, value = c.SupplierName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
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
