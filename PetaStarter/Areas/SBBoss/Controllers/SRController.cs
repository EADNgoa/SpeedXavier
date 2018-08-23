using Microsoft.AspNet.Identity;
using Speedbird.Controllers;
using System;
using System.Collections.Generic;
//using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
//using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using static PetaStarter.Areas.SBBoss.Models.DataTablesModels;
using System.Collections;
using KellermanSoftware.CompareNetObjects;
using PetaPoco;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class SRController : EAController
    {
        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public ActionResult Index(int? page, string AN)
        {
            ViewBag.Title = "Service Requests";
            ViewBag.SRStatusID = Enum.GetValues(typeof(SRStatusEnum)).Cast<SRStatusEnum>().Where(v => v > SRStatusEnum.NoAction).Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            ViewBag.PayStatusID = Enum.GetValues(typeof(PayType)).Cast<PayType>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            return View();
        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public ActionResult SRQueue(int? page, string AN)
        {
            var LatestSR = db.Query<Queue>("Select ServiceTypeId, Count(1) as Count from ServiceRequest where SRStatusID = @0 group by ServiceTypeId having count(ServiceTypeId)>0", SRStatusEnum.New);

            return PartialView("_Queue", LatestSR);
        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public ActionResult SRQueueIndex(int? page, ServiceTypeEnum? st)
        {
            page = 1;
            ViewBag.ServiceRequestTypeId = st;
            return View("SRQueueIndex", base.BaseIndex<ServiceRequestDets>(page, "sr.SRID, TDate, event as request,FName, SName, Phone  ", $"ServiceRequest sr inner join SR_Cust sc on sr.SRID = sc.ServiceRequestID " +
                $"inner join Customer c on c.CustomerID = sc.CustomerID inner join SRlogs l on sr.SRID=l.SRID where ServiceTypeID={(int)st} and SRStatusID ={(int)SRStatusEnum.New}"));
        }



        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public ActionResult SRDiaryDets(DateTime? AN)
        {
            ViewBag.Title = "Diary";
            AN=AN ?? DateTime.Now.Date;
            
            return View("SRDiaryDets", base.BaseIndex<SRdetailDets>(1, " * ", $"ServiceRequest sr , SRdetails srd where sr.PayStatusID={(int)PayType.Full_Paid} and sr.srid=srd.SRID and (srd.FDate='{AN:yyyy-MM-dd}' or ('{AN:yyyy-MM-dd}' between srd.Fdate and srd.TDate))"));

        }

        [HttpPost]
        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public JsonResult GetSRList(DTParameters parameters)
        {
            var columnSearch = parameters.Columns.Select(s => s.Search.Value).Take(SRColumns.Count()).ToList();

            //XMLPath uses nested queries so to avoid that we construct these 4 filters ourselves
            string CustName = "";            
            string Cemail = "";
            string Cphone = "";
            string AgentName = "";

            if (columnSearch[2]?.Length > 0) { CustName = columnSearch[2]; columnSearch[2] = null; }            
            if (columnSearch[3]?.Length > 0) { Cphone = columnSearch[3]; columnSearch[3] = null; }
            if (columnSearch[4]?.Length > 0) { Cemail = columnSearch[4]; columnSearch[4] = null; }
            if (columnSearch[5]?.Length > 0) { AgentName = columnSearch[5]; columnSearch[5] = null; }



            var sql = new PetaPoco.Sql($"Select SRID, BookingNo,TDate, CONCAT(c.FName, ' ', c.SName) as CName, c.Phone, c.email, u.email as AgentName,SRStatusID, PayStatusID " +
                $"from ServiceRequest s left join AspNetUsers u on u.id=s.AgentId ");
            var fromsql = new PetaPoco.Sql(", Customer c ");                
            var wheresql = new PetaPoco.Sql(" where c.CustomerID=s.CustID and SRStatusId >@0", (int)SRStatusEnum.NoAction);

            if (CustName.Length > 0 || Cemail.Length > 0 || Cphone.Length > 0)
            {
                if (CustName.Length > 0)
                    wheresql.Append($" and (c.FName like '%{CustName}%' or c.SName like '%{CustName}%' )");                
                if (Cemail.Length > 0)
                    wheresql.Append($" and c.email like '%{Cemail}%' ");
                if (Cphone.Length > 0)
                    wheresql.Append($" and c.phone like '%{Cphone}%' ");
            }

            if (AgentName.Length>0)
            {
                wheresql.Append($" and u.email like '%{AgentName}%' ");
            }

            wheresql.Append($"{GetWhereWithOrClauseFromColumns(SRColumns, columnSearch)}");
            sql.Append(fromsql);
            sql.Append(wheresql);

            var sortStr = parameters.SortOrder;
            sortStr = sortStr.Replace("Status", "SRStatusID");            
            sql.Append(" order by " + sortStr);

            try
            {
                var cnt = db.Query<ServiceRequestDets>(sql).ToList();
                var res = cnt.Skip(parameters.Start).Take(parameters.Length).ToList();

                var dataTableResult = new DTResult<ServiceRequestDets>
                {
                    draw = parameters.Draw,
                    data = res,
                    recordsFiltered = cnt.Count,
                    recordsTotal = cnt.Count
                };
                return Json(dataTableResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public JsonResult GetSRDetList(DTParameters parameters, int? id)
        {
            var columnSearch = parameters.Columns.Select(s => s.Search.Value).Take(SRDetColumns.Count()).ToList();

            //XMLPath uses nested queries so to avoid that we construct these 4 filters ourselves


            var sql = new PetaPoco.Sql($"Select * from SRdetails sd left join Supplier s on s.SupplierID =sd.SupplierID where SRID=@0", id);
            var fromsql = new PetaPoco.Sql();

            var wheresql = new PetaPoco.Sql();


            wheresql.Append($"{GetWhereWithOrClauseFromColumns(SRDetColumns, columnSearch)}");
            //sql.Append(fromsql);
            sql.Append(wheresql);

            try
            {
                var res = db.Query<SRdetailDets>(sql).Skip(parameters.Start).Take(parameters.Length).ToList();


                var dataTableResult = new DTResult<SRdetailDets>
                {
                    draw = parameters.Draw,
                    data = res,
                    recordsFiltered = 10,
                    recordsTotal = res.Count()
                };
                return Json(dataTableResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        private string GetWhereWithOrClauseFromColumns(string[] columnDefs, List<string> searchValues)
        {
            try
            {
                var where = " ";
                var subQuery = "";
                for (int i = 0; i < searchValues.Count; i++)
                {
                    if (searchValues[i] != null)
                    {
                        if (columnDefs[i].IndexOf("Date") > -1)
                        {
                            //Date search
                            subQuery += columnDefs[i] + " = '%" + DateTime.Parse(searchValues[i]).ToString("yyyy-MM-dd") + "'" + " and ";
                        }
                        else
                        {
                            //free text
                            subQuery += columnDefs[i] + " like '%" + searchValues[i].Replace("'", "''") + "%'" + " and ";
                        }
                        if (i == searchValues.Count - 1)
                        {
                            subQuery = subQuery.Remove(subQuery.LastIndexOf("and"), 3);
                        }
                    }
                }
                if (subQuery.Trim() != "")
                {
                    where += " and (" + subQuery + ")";
                }

                if (where.LastIndexOf("and )") > 0)
                    where = where.Replace("and )", ")");
                return where;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private string[] SRColumns => new string[]
        {
            "BookingNo", 
            "TDate",
            "CName",            
            "Phone",
            "Email",            
            "AgentName",
            "SRStatusID",
            "PayStatusID"
        };
        private string[] SRDetColumns => new string[]
       {
            "SRID",
            "ServiceTypeName",
            "FromLoc",
            "ToLoc",
            "ContractNo",
            "CouponCode",
            "fstr",
            "SupplierName",
            "Cost",
            "SellPrice"
       };


      

        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public ActionResult GetSRInfo(int id, int? mode)
        {
            var rec = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest Where SRID=@0", id);
            rec.UserName = db.ExecuteScalar<string>("Select UserName From AspNetUsers Where Id=@0", rec.EmpID);
            rec.AgentName = db.ExecuteScalar<string>("Select UserName From AspNetUsers Where Id=@0", rec.AgentID);
            ViewBag.mode = mode;
            return PartialView("InfoHeader", rec);
        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult FetchDetails(int? id)
        {
            ViewBag.Title = id.Value>0 ?"Booking Folder": "New Enquiry";

            var SR = base.BaseCreateEdit<ServiceRequest>(id, "SRID");
            if (id > 0)
            {
                if (SR.SRStatusID < (int)SRStatusEnum.Unconfirmed)
                    SR.SRStatusID = (int)SRStatusEnum.Unconfirmed;

                ViewBag.AgentName = db.ExecuteScalar<string>("Select UserName From AspNetUsers where Id = @0 ", SR.AgentID);

                ViewBag.BookingTypeID = Enum.GetValues(typeof(BookingTypeEnum)).Cast<BookingTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            }


            ViewBag.SRStatusID = Enum.GetValues(typeof(SRStatusEnum)).Cast<SRStatusEnum>().Where(v => v > SRStatusEnum.NoAction).Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();            

            ViewBag.EnquirySource = Enum.GetValues(typeof(EnquirySourceEnum)).Cast<EnquirySourceEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            ViewBag.ServiceTypeID = Enum.GetValues(typeof(ServiceTypeEnum)).Cast<ServiceTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();

            ViewBag.Cust = db.FirstOrDefault<Customer>("Select FName,SName from Customer where CustomerID=@0", SR?.CustID ?? 0);

            return PartialView("Details", SR);
        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult ExistingCustRec(string fn, string sn, string ph, string em)
        {
            var sql = new PetaPoco.Sql("Select * from Customer where 1=1");
                        
            if (fn != null)            
                sql.Append($" and LOWER(FName) like '%{fn.ToLower()}%'");                
            
            if (sn != "")
                sql.Append($" and LOWER(SName) like '%{sn.ToLower()}%'");
            
            if (ph != "")
                sql.Append($" and Phone like '%{ph}%'");            
            
            if (em != "")
                sql.Append(" and LOWER(Email) like '%@0%'",em.ToLower());

           var recs = db.Query<CustomerDets>(sql);

            return PartialView("CustomerSearchPartial", recs);
        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult Manage(int? id, int mode = 1, int EID = 0) //Mode 1=Details,2=Notepad,3=Services, 4=Customers, 5=images,6=BookingSummary
        {

            ViewBag.mode = mode;


            ViewBag.EID = EID;
            ViewBag.SRID = id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult Manage([Bind(Include = "SRID,BookingNo,CustID,SRStatusID,EmpID,BookingTypeID,EnquirySource,AgentID,TDate,PayStatusID,ServiceTypeID")] ServiceRequest item, string Event, int? CID, string FName, string SName, string Email, string Phone)
        {
            using (var transaction = db.GetTransaction())
            {
                if (ModelState.IsValid)
                {                    
                    item.EmpID = User.Identity.GetUserId();
                    if (item.TDate==null)
                        item.TDate = DateTime.Now;

                    if (item.SRStatusID == null)
                        item.SRStatusID = (int)SRStatusEnum.New;

                    if (item.CustID == null)
                        item.CustID=CID;

                    //on 19 aug 18 Xavier said the Enquiry number should be different from BkNo. BkNo needs to be sequential
                    if (item.BookingNo == null && item.SRStatusID>(int)SRStatusEnum.NoAction)
                    {
                        var lastBkId = db.FirstOrDefault<int>("select coalesce(max(BookingNo),0) from ServiceRequest");
                        item.BookingNo = ++lastBkId;
                    }

                    if (item.SRID > 0)
                    {
                        db.Update(item);
                        string Objdiffs = FindDiffs<ServiceRequest>(item.SRID, item, "Service Request");
                        if (Objdiffs.Length>0)
                            LogAction(new SRlog { SRID = item.SRID, Event = Objdiffs });
                    }
                    else
                    {
                        db.Insert(item);
                        LogAction(new SRlog { SRID = item.SRID, Event = Event });
                    }

                    if (CID != null)
                    {
                        db.Insert(new SR_Cust { ServiceRequestID = item.SRID, CustomerID = (int)CID });
                    }
                    else if (FName != null && Phone != null && Email != null)
                    {
                        var cust = new Customer { FName = FName, SName = SName, Phone = Phone, Email = Email };
                        db.Insert(cust);
                        db.Insert(new SR_Cust { ServiceRequestID = item.SRID, CustomerID = cust.CustomerID });
                        item.CustID = cust.CustomerID;
                        db.Update(item);
                    }

                    if (Event?.Length == null)
                    {
                        Event = "User has Edited the Field";
                    }
                    
                    transaction.Complete();
                    return new JsonResult { Data = true };
                }
                else
                {
                    transaction.Dispose();
                    return new JsonResult { Data = false };

                }
            }

        }
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRdetails(int? id, int? EID)
        {
            var rec = base.BaseCreateEdit<SRdetail>(EID, "SRDID");
            
            ViewBag.SRID = id;
            ViewBag.Title = "Manage Services";
            ViewBag.SRs = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest sr inner join Customer c on c.CustomerID=sr.CustID Where SRID=@0", id);
            ViewBag.SRDets = db.Fetch<SRdetail>("Select * from SRdetails where SRID =@0", id);
            List<SelectListItem> ServiceTypeID = Enum.GetValues(typeof(ServiceTypeEnum)).Cast<ServiceTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            var defaultServType = db.Single<ServiceRequest>(id).ServiceTypeID;
            var selST= ServiceTypeID.First<SelectListItem>(a => int.Parse(a.Value) == defaultServType);
            selST.Selected = true;
            ViewBag.ServiceTypeID = ServiceTypeID;
            ViewBag.OptionTypeID = new SelectList(db.Fetch<OptionType>("Select OptionTypeID,OptionTypeName from OptionType where ServiceTypeID=@0", (int)ServiceTypeEnum.Accomodation), "OptionTypeID", "OptionTypeName");

            return PartialView(rec);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRdetails([Bind(Include = "SRDID,HasAc,HasCarrier,RateBasis,PayTo,PickUpPoint,DropPoint,SRID,ServiceTypeID,FromLoc,ToLoc,SuppInvNo,Fdate,Tdate,SupplierID,Cost,SellPrice,PNRno,TicketNo,Heritage,ChildNo,AdultNo,InfantNo,RoomType,CouponCode,City,Airline,DateOfIssue,FlightNo,ContractNo,GuideLanguageID,SSType,CarType,Model,ParentID,IsReturn,OptionTypeID,ItemID")] SRdetail item, string Event,string IsReturn)
        {
            using (var transaction = db.GetTransaction())
            {
                try
                {
                    DateTime? td =(DateTime?)item?.Tdate ;
                    if(item.SRDID > 0)
                    {

                        if (item.ServiceTypeID == (int)ServiceTypeEnum.Flight)
                        {
                            item.Tdate = null;
                        }

                        db.Update(item);
                        LogAction(new SRlog { SRID = item.SRID, SRDID=item.SRDID, Event = Event, Type=true});
                        LogAction(new SRlog { SRID = item.SRID, SRDID = item.SRDID, Event = FindDiffs<SRdetail>(item.SRDID,item,"Service") });
                        if(item.IsReturn == true)
                        {
                            db.Execute($"Update Srdetails Set Tdate ='{td.Value.ToString("dd-MM-yyyy")}',FromLoc='{item.ToLoc}',ToLoc ='{item.FromLoc}' where ParentID ={item.SRDID}");
                        }
                    }
                    else
                    {
                        if(item.ServiceTypeID == (int)ServiceTypeEnum.Flight)
                        {
                            item.Tdate = null;
                        }
                        if (item.ECommision == null)
                        {
                            item.ECommision = -1;
                        }
                        db.Insert(item);
                        if (IsReturn == "true")
                        {//We need to insert this duplicate record so that we can show this return flight in the Daily Diary on the return journey date
                            db.Insert(new SRdetail { FromLoc = item.ToLoc, ToLoc = item.FromLoc, Tdate = td, ParentID = item.SRDID, ServiceTypeID = item.ServiceTypeID });
                        }
                        LogAction(new SRlog { SRID = item.SRID, SRDID = item.SRDID, Event = Event, Type = true });
                        LogAction(new SRlog { SRID = item.SRID, SRDID = item.SRDID, Event = "Added " + item.ServiceTypeName });
                    }
                    
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    db.AbortTransaction();
                    throw ex;
                }
            }
            return RedirectToAction("Manage", new { id = item.SRID, mode = 3 });
        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRUpload(int? id)
        {
            ViewBag.Title = "Uploads";
            ViewBag.SRs = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest Where SRID=@0", id);
            ViewBag.Pics = db.Fetch<SRUpload>("Select * From SRUploads where SRID=@0", id);
            base.BaseCreateEdit<SRUpload>(id, "SRUID");

            SRuploadDets ci = new SRuploadDets() { };
            return PartialView(ci);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRUpload([Bind(Include = "SRUID,SRID,Path,UploadName,UploadedFile")] SRuploadDets item)
        {
            SRUpload res = new SRUpload
            {
                SRID = item.SRID,
                SRUID = item.SRUID,
                UploadName = item.UploadName
            };

            if (item.UploadedFile != null)
            {
                string fn = item.UploadedFile.FileName.Substring(item.UploadedFile.FileName.LastIndexOf('\\') + 1);
                fn = item.UploadName + "_" + fn;

                string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                item.UploadedFile.SaveAs(SavePath);

                res.Path = fn;
            }
            LogAction(new SRlog { SRID = item.SRID, Event = item.UploadName + " Uploaded" });
            return base.BaseSave<SRUpload>(res, item.SRUID > 0, "Manage", new { id = item.SRID, mode = 5 });

        }
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRCustomers(int? id, int? sid, int? EID, int? cid)
        {
            if (id != null && cid != null)
            {
                db.Execute($"Delete From SR_Cust where ServiceRequestID ={id} and CustomerID={cid}");
                return RedirectToAction("Manage", new { id, mode = 4 });
            }
            var rec = base.BaseCreateEdit<Customer>(EID, "CustomerID");
            ViewBag.SRID = id;
            ViewBag.Title = "Manage Customers";
            ViewBag.Custs = db.Fetch<Customer>($"Select * From Customer c inner join SR_Cust sc on sc.CustomerID = c.CustomerID inner join ServiceRequest sr on sr.SRID = sc.ServiceRequestID where sc.ServiceRequestID ='{id}'");
            return PartialView(rec);
        }

        [HttpPost]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRCustomers(string FName, string SName, string Email, string Phone, int? CID, int? SRID,string Type,string UploadName, HttpPostedFileBase UploadedFile)
        {
            if (CID != null)
            {
                db.Insert(new SR_Cust { ServiceRequestID = (int)SRID, CustomerID = (int)CID });
                LogAction(new SRlog { SRID = SRID.Value,  Event = "Existing customer added" });
            }
            else if (FName != null && Email != null && SName != null && Phone != null)
            {
                var cust = new Customer { FName = FName, SName = SName, Phone = Phone, Email = Email ,Type = Type };
                db.Insert(cust);
                db.Insert(new SR_Cust { ServiceRequestID = (int)SRID, CustomerID = cust.CustomerID });
                LogAction(new SRlog { SRID = SRID.Value, Event = $"Customer {FName} {SName} added" });
            }

            if (UploadedFile != null && UploadName != null)
            {
                string fn = UploadedFile.FileName.Substring(UploadedFile.FileName.LastIndexOf('\\') + 1);
                fn = UploadName + "_" + fn;

                string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                UploadedFile.SaveAs(SavePath);

                db.Insert(new SRUpload { UploadName = UploadName, Path = fn, SRID = SRID });
                LogAction(new SRlog { SRID = SRID.Value , Event = UploadName+  " Uploaded"});
            }
            return RedirectToAction("Manage", new { id = (int)SRID, mode = 4 });

        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult Reciepts(int? id, int? sid, int? EID)
        {
            ViewBag.SRID = id;
            ViewBag.Title = "Profit and loss Details";
            ViewBag.Debit = db.ExecuteScalar<decimal?>("Select sum(Cost) as Cost from SRdetails Where SRID =@0",id);
            ViewBag.Credit = db.ExecuteScalar<decimal?>("Select sum(Amount) as Amt from RP_SR Where SRID =@0", id)??0 +
                db.ExecuteScalar<decimal?>("Select sum(Amount) as Amt from DRP_SR Where SRID =@0", id)??0;
            ViewBag.PaxDetail = db.Fetch<PaxDets>("; Exec AmtPerPax @@SRID = @0", id).ToList();

            var services = db.Query<SRdetailDets>("Select SRID,SRDID,ServiceTypeID,Cost,SellPrice,ECommision from SRdetails where SRID=@0",id).ToList();
           services.ForEach(s=>
            {
                var tax = db.ExecuteScalar<decimal?>("Select Percentage From Taxes Where ServiceTypeID=@0 and WEF<GetDate() order by WEF desc ", s.ServiceTypeID)??0;
                s.Tax = s.SellPrice * tax / 100;                
                var c = db.FirstOrDefault<ServiceCommision>($"Select Perc,Amount  From ServiceCommision Where Serviceid={s.ServiceTypeID + 1} ");

                if (s.ECommision <0)
                {
                    if (c.Perc != null)
                    {
                        s.Commision = c.Perc ?? 0;
                        s.PercComm = "%";
                        var tot = s.SellPrice * s.Commision / 100;
                        s.Total = s.SellPrice - s.Tax - tot - s.Cost;
                    }
                    else if (c.Amount != null)
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
            ViewBag.Services = services;

            var  rp= db.Fetch<RPDetails>("select rp.CDate as [Date],rp.Note,rp.Type,rs.Amount,rp.IsPayment from RPDets rp left join RP_SR rs on rp.RPDID = rs.RPDID Where rs.SRID = @0",id);
            var drp = db.Fetch<RPDetails>("select rp.CDate as [Date],rp.Note,rp.Type,rs.Amount,rp.IsPayment from DRPDets rp left join DRP_SR rs on rp.DRPDID = rs.DRPDID Where rs.SRID = @0", id);
            ViewBag.Reciepts = rp.Concat(drp).Where(d => d.IsPayment == false);
            ViewBag.Payments = rp.Concat(drp).Where(r => r.IsPayment == true);


            return PartialView();
        }

      
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRLogs(int? id, int? sid, int? EID)
        {
            var rec = base.BaseCreateEdit<SRReciept>(EID, "RecieptID");
            ViewBag.SRID = id;
            ViewBag.Title = "Service Reauest Logs";

            ViewBag.SRs = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest Where SRID=@0", id);
            ViewBag.SRLogDets = db.Fetch<SRlogsDets>($"Select * From SRLogs sl inner join AspNetUsers anu on sl.UserID = anu.Id where SRID ='{id}' ORDER By SRLID Desc");
            return PartialView(rec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRLogs([Bind(Include = "SRLID,SRID,SRDID,Event")] SRlog item)
        {
            item.Type = true;
            LogAction(item);
            return RedirectToAction("Manage", new { id = item.SRID, mode = 2 });
        }

        /// <summary>
        /// Finds the changed properties and returns a formatted string OR says a new obj added
        /// </summary>
        /// <typeparam name="T">The type of object to create the DB log for</typeparam>
        /// <param name="Id">The id of the new(0) or edited object. It will be used to fetch the exiting db record</param>
        /// <param name="newObj">The new edited object</param>
        /// <param name="objName">The name of the object. Used only to state what got edited</param>
        /// <returns></returns>
        private string FindDiffs<T>(int Id, T newObj, string objName)
        {
            if (Id > 0)
            {
                var oldObj = db.Single<T>(Id);
                CompareLogic c = new CompareLogic(new ComparisonConfig { MaxDifferences = 99 });
                ComparisonResult comparisonResult = c.Compare(oldObj, newObj);
                string res = " ";
                foreach (var item in comparisonResult.Differences)
                    res += $"{item.PropertyName}: {item.Object1Value}->{item.Object2Value} +";
                if (res.Length > 1)
                    return objName + " Changed: " + res.Substring(0, res.Length - 1);
                else
                    return "";
            }
            else
                return objName+ " Added";
        }

        private void LogAction(SRlog item)
        {
            if (item.Event.Length > 0)
            {
                item.LogDateTime = DateTime.Now;
                item.UserID = User.Identity.GetUserId();

                db.Insert(item);
            }
        }
                
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult Delete(int? id, int? pid)
        {

            if (pid != null)
            {
                //First remove the old img (if exists)
                string oldImg = db.Single<string>("Select Path from SRUploads where SRUID=@0", pid);
                if (oldImg?.Length > 0) System.IO.File.Delete(System.IO.Path.Combine(Server.MapPath("~/Images"), oldImg));

                db.Execute($"Delete From SRUploads Where SRUID={pid}");
                return RedirectToAction("Manage", routeValues: new { id, mode = 4 });


            }
            return RedirectToAction("Manage");
        }
        public ActionResult AutoCompleteCust(string term)
        {
            var filteredItems = db.Fetch<Customer>($"Select * from Customer Where FName like '%{term}%'").Select(c => new { id = c.CustomerID, value = c.FName + " " + c.SName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AutoCompleteAgent(string term)
        {
            var filteredItems = db.Fetch<AspNetUser>($"Select * from AspNetUsers Where UserName like '%{term}%'").Select(c => new { id = c.Id, value = c.UserName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AutoCompleteSup(string term)
        {
            var filteredItems = db.Fetch<Supplier>($"Select * from Supplier Where SupplierName like '%{term}%'").Select(c => new { id = c.SupplierID, value = c.SupplierName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AutoCompleteDrv(string term)
        {
            var filteredItems = db.Fetch<Driver>($"Select * from Driver Where DriverName like '%{term}%'").Select(c => new { id = c.DriverID, value = c.DriverName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoCompletePack(string term)
        {
            var filteredItems = db.Fetch<Package>($"Select * from Package Where PackageName like '%{term}%' and ServiceTypeID={(int)ServiceTypeEnum.Packages}").Select(c => new { id = c.PackageID, value = c.PackageName});
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AutoCompleteSight(string term)
        {
            var filteredItems = db.Fetch<Package>($"Select * from Package Where PackageName like '%{term}%' and ServiceTypeID={(int)ServiceTypeEnum.SightSeeing}").Select(c => new { id = c.PackageID, value = c.PackageName});
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AutoCompleteCruise(string term)
        {
            var filteredItems = db.Fetch<Package>($"Select * from Package Where PackageName like '%{term}%' and ServiceTypeID={(int)ServiceTypeEnum.Cruise}").Select(c => new { id = c.PackageID, value = c.PackageName});
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutoCompleteAccom(string term)
        {
            var filteredItems = db.Fetch<Accomodation>($"Select * from Accomodation Where AccomName like '%{term}%'").Select(c => new { id = c.AccomodationID, value = c.AccomName});
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AutoCompleteCar(string term)
        {
            var filteredItems = db.Fetch<CarBike>($"Select * from CarBike Where CarBikeName like '%{term}%'").Select(c => new { id = c.CarBikeID, value = c.CarBikeName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FetchSTpartial(int? id, int ServiceTypeId, bool IsReadOnly)
        {
            ViewBag.GuideLanguageID = db.Fetch<GuideLanguage>("Select * from GuideLanguage").Select(v => new SelectListItem { Text = v.GuideLanguageName, Value = v.GuideLanguageID.ToString() }).ToList();
            ViewBag.CarType = Enum.GetValues(typeof(CarTypeEnum)).Cast<CarTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            ViewBag.SRDID = id;
            if (IsReadOnly)
                ViewBag.IsReadOnly = "disabled";
          

            switch ((ServiceTypeEnum)ServiceTypeId)
            {
                case ServiceTypeEnum.Accomodation:
                    List<SelectListItem> AccPayTo = new List<SelectListItem>()
                    {
                            new SelectListItem {
                                Text = "Pay to us", Value = "Pay to us"
                            },
                            new SelectListItem {
                                Text = "Pay to owner", Value = "Pay to owner"
                            }
                        };
                    ViewBag.PayTo = AccPayTo;

                    return PartialView($"_{((ServiceTypeEnum)ServiceTypeId).ToString()}", db.SingleOrDefault<SRdetail>(id));
                case ServiceTypeEnum.SightSeeing:
                    return PartialView($"_{((ServiceTypeEnum)ServiceTypeId).ToString()}", db.SingleOrDefault<SRdetail>(id));
                case ServiceTypeEnum.CarBike:
                    return PartialView($"_{((ServiceTypeEnum)ServiceTypeId).ToString()}", db.SingleOrDefault<SRdetail>(id));
                case ServiceTypeEnum.Cruise:
                    return PartialView($"_{((ServiceTypeEnum)ServiceTypeId).ToString()}", db.SingleOrDefault<SRdetail>(id));
                case ServiceTypeEnum.Packages:
                    return PartialView($"_{((ServiceTypeEnum)ServiceTypeId).ToString()}", db.SingleOrDefault<SRdetail>(id));
                case ServiceTypeEnum.Insurance:
                    return PartialView($"_{((ServiceTypeEnum)ServiceTypeId).ToString()}", db.SingleOrDefault<SRdetail>(id));
                case ServiceTypeEnum.Visa:
                    return PartialView($"_{((ServiceTypeEnum)ServiceTypeId).ToString()}", db.SingleOrDefault<SRdetail>(id));
                case ServiceTypeEnum.Flight:
                    var rec = db.SingleOrDefault<SRdetail>(id);
                    if (id > 0)
                    {
                        var prec = db.FirstOrDefault<SRdetail>("select Tdate,FromLoc,ToLoc from Srdetails where ParentID =@0", id);
                        if (prec != null)
                        {
                            rec.Tdate = prec.Tdate ?? null;
                        }
                    }
                    return PartialView($"_{((ServiceTypeEnum)ServiceTypeId).ToString()}", rec);
                case ServiceTypeEnum.TaxiHire:
                    var DrvID = db.FirstOrDefault<Driver>("Select DriverID from SRdetails where SRDID=@0", id);
                    if (DrvID?.DriverID > 0)
                    {
                        ViewBag.DrvNm = db.ExecuteScalar<string>("Select DriverName From Driver where DriverID=@0", DrvID.DriverID);
                    }
                    List<SelectListItem> RateBasis = new List<SelectListItem>()
                    {
                            new SelectListItem {
                                Text = "8hrs -80Kms", Value = "8hrs -80Kms"
                            },
                            new SelectListItem {
                                Text = "None", Value = "None"
                            }
                        };
                    ViewBag.RateBasis = RateBasis;
                    List<SelectListItem> TaxiPayTo = new List<SelectListItem>()
                    {
                            new SelectListItem {
                                Text = "Pay to us", Value = "Pay to us"
                            },
                            new SelectListItem {
                                Text = "Pay to driver", Value = "Pay to driver"
                            }
                        };
                    ViewBag.PayTo = TaxiPayTo;

                    return PartialView($"_{((ServiceTypeEnum)ServiceTypeId).ToString()}", db.SingleOrDefault<SRdetail>(id));
                default:
                    return PartialView("_NotFound");
            }
        }

        [HttpPost]
        public string FetchSTlabels(string CommonLabel, int ServiceTypeId)
        {

            Dictionary<string, string> Accomodation = new Dictionary<string, string>()
            {
                {"Fdate","From Date" },
                {"FromLoc","Address" }
            };

            Dictionary<string, string> SightSeeing = new Dictionary<string, string>()
            {
                {"Fdate","Pickup Date and Time" },
                {"FromLoc","Pickup Address" }
            };

            Dictionary<string, string> Packages = new Dictionary<string, string>()
            {
                {"Fdate","Start Date" },
                {"FromLoc","Start Location" }
            };

            Dictionary<string, string> CarBike = new Dictionary<string, string>()
            {
                {"Fdate","From Date" },
                {"FromLoc","Pickup Address" }
            };

            Dictionary<string, string> Cruise = new Dictionary<string, string>()
            {
                {"Fdate","From Date" },
                {"FromLoc","Port of Boarding" }
            };

            Dictionary<string, string> Flight = new Dictionary<string, string>()
            {
                {"Fdate","Departure Date and Time" },
                {"FromLoc","Departure Airport" }
            };

            Dictionary<string, string> Visa = new Dictionary<string, string>()
            {
                {"Fdate","Start Date" },
                {"FromLoc","Destination Country" }
            };

            Dictionary<string, string> Insurance = new Dictionary<string, string>()
            {
                {"Fdate","Valid From" },
                {"FromLoc","Destination Country" }
            };
            Dictionary<string, string> TaxiHire = new Dictionary<string, string>()
            {
                {"Fdate","From Date" },
                {"FromLoc","From Location" }
            };

            Dictionary<string, Dictionary<string, string>> dynLabels = new Dictionary<string, Dictionary<string, string>>()
            {
                { (ServiceTypeEnum.Accomodation).ToString(), Accomodation},
                { (ServiceTypeEnum.SightSeeing).ToString(), SightSeeing},
                { (ServiceTypeEnum.Packages).ToString(), Packages},
                { (ServiceTypeEnum.CarBike).ToString(), CarBike},
                { (ServiceTypeEnum.Cruise).ToString(), Cruise},
                { (ServiceTypeEnum.Flight).ToString(), Flight},
                { (ServiceTypeEnum.Visa).ToString(), Visa},
                { (ServiceTypeEnum.Insurance).ToString(), Insurance},
                { (ServiceTypeEnum.TaxiHire).ToString(), TaxiHire}
            };


            return dynLabels[((ServiceTypeEnum)ServiceTypeId).ToString()][CommonLabel];

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool IgnoreReason(int SRID,string IgnoreReason)
        {
            try
            {
                int res = db.Execute("Update ServiceRequest set IgnoreReason=@0 ,SRStatusId=@1, EmpId=@2  where SRID=@3", IgnoreReason, (int)SRStatusEnum.NoAction,User.Identity.GetUserId() , SRID);                
                
                return true;

            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool AssignDriver(int? SRDID, int? DID)
        {
            try
            {
                int res = db.Execute("Update SRdetails set DriverID=@0 where SRDID=@1", DID, SRDID);
                return true;

            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool ReviewDriver(IEnumerable<int> Star,IEnumerable<int> QuestionID,int SRDID)
        {
            try
            {
                //First delete old ratings for thsi record if they exist
                db.Execute("Delete from Feedback where SRDID=@0", SRDID);

                var SQ = Star.Zip(QuestionID, (s, q) => new { star = s, questID = q });
                foreach (var item in SQ)
                {
                    db.Insert(new FeedBack { SRDID = SRDID, QuestionID = item.questID, StarRating = item.star });
                }                  
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool RemindAt(int SRID, DateTime remindAtDt)
        {
            try
            {
                int res = db.Execute("Update ServiceRequest set RemindAt=@0,SRStatusId=@1, EmpId=@2  where SRID=@3", remindAtDt, (int)SRStatusEnum.NoAction, User.Identity.GetUserId(), SRID);
                
                return true;

            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        public PartialViewResult _AssignDriver(int? SRID,int? SRDID)
        {
            ViewBag.SRDID = SRDID;
            ViewBag.SRID = SRID;

            return PartialView();
        }

        public PartialViewResult _AssignCust(int? SRID, int? SRDID)
        {
            ViewBag.Cust = db.Query<CustomerDets>("Select c.CustomerID, sc.ServiceRequestID as BookingID, c.FName,c.SName,c.Email,c.Phone, c.[Type] from Customer c inner join SR_Cust sc on c.CustomerID = sc.CustomerID 	" +
                "Where sc.ServiceRequestID=@0 and c.CustomerID not in (Select CustomerID from SRD_Cust where srdid=@1)", SRID,SRDID).ToList();
            ViewBag.AssignedCust = db.Query<CustomerDets>("Select c.CustomerID, c.FName,c.SName,c.Email,c.Phone, c.[Type]  from Customer c inner join SRD_Cust sc on c.CustomerID = sc.CustomerID Where sc.SRDID=@0 ", SRDID).ToList();

            ViewBag.SRDID = SRDID;
            ViewBag.SRID = SRID;

            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool AssignCust(IEnumerable<int?> CID, int? SRDID)
        {
            try
            {
                foreach (var item in CID)
                {
                    db.Insert(new SRD_Cust { SRDID =(int) SRDID,CustomerID=(int)item });
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult FetchDrv(string DriverName,int? GeoTreeID ,int? SRDID,string scr,string bks)
      {
            var sql = new PetaPoco.Sql("Select d.DriverID,d.DriverName,d.Phone,d.Address,d.CarModel,g.GeoName,coalesce(sum(f.StarRating),0)as Score,(select count(DriverID) as DriverID " +
                "from SRdetails where DriverID =sd.DriverID) as DBCount from Driver d left  join SRdetails sd on sd.DriverID = d.DriverID left join FeedBack f on f.SRDID = sd.SRDID  " +
                "inner join GeoTree g on g.GeoTreeID= d.LocationId ");

            if (DriverName != null)
                sql.Append($" Where LOWER(DriverName) like '%{DriverName.ToLower()}%'");

            if (GeoTreeID != null)
                sql.Append($" and LocationId={GeoTreeID}");

            sql.Append(" Group By d.DriverName,d.DriverID,d.Address,d.CarModel,d.Phone,d.LocationId,g.GeoName,sd.DriverID");
            if (scr != null)
            {
                sql.Append($" Order By Score Desc");
            }
            else if(bks != null)
            {
                sql.Append("Order By DBCount Asc");
            }
            var recs = db.Query<DriverDets>(sql);

         
            
            return PartialView("_DriverSearch", recs);
        }
        public PartialViewResult _DrvReview(int? SRID, int? SRDID)
        {
            ViewBag.Quests = db.Query<Question>("Select * from Questions ");
            ViewBag.SRDID = SRDID;
            ViewBag.SRID = SRID;

            return PartialView();
        }
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public JsonResult GetLocations(string term)
       {
            var locs = db.Fetch<GeoTree>("Select CONCAT(g.GeoName,': ', dbo.GetGeoAncestorsStr(g.GeoTreeID)) as GeoName,g.GeoTreeID from GeoTree g where GeoName like '%" + term + "%'");
            return Json(new { results = locs.Select(a => new { id = a.GeoTreeID, text = a.GeoName.TrimEnd(',', ' ') }) }, JsonRequestBehavior.AllowGet);
        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult CustSearch(int? id)
        {
            ViewBag.SRDID = id;
            ViewBag.Check = "True";
            var rec = db.Query<CustomerDets>("Select * from Customer c inner join SRD_Cust sc on c.CustomerID = sc.CustomerID inner join SRdetails sd on sd.SRDID = sc.SRDID Where sc.SRDID=@0 ", id).ToList();

            return PartialView("CustomerSearchPartial",rec);
        }

        public JsonResult GetOptionsOfST(int serviceTypeId)
        {
            var locs = db.Fetch<OptionType>("Select * from OptionType where ServiceTypeId=@0",serviceTypeId);
            return Json(new { results = locs.Select(a => new { id = a.OptionTypeID, text = a.OptionTypeName }) }, JsonRequestBehavior.AllowGet);
        }


        //Load Edit Commsion Modal
        public PartialViewResult _EditComm(int? SRID)
        {
            ViewBag.SRID = SRID;
           return PartialView();
        }

        public bool EditComm(int? SRDID, decimal? ECommision)
        {
            try
            {
                int res = db.Execute("Update SRdetails set ECommision=@0 where SRDID=@1",ECommision,SRDID);
                return true;

            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
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
