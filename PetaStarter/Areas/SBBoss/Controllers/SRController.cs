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
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Serialization;

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
            var LatestSR = db.Query<Queue>("Select ServiceTypeId, Count(1) as Count from ServiceRequest where SRStatusID in (@0,@1) and (RemindAt is null OR RemindAt<GetDate()) group by ServiceTypeId having count(ServiceTypeId)>0", SRStatusEnum.New, SRStatusEnum.Reminder);

            return PartialView("_Queue", LatestSR);
        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public ActionResult SRQueueIndex(int? page, ServiceTypeEnum? st)
        {
            page = 1;
            ViewBag.ServiceRequestTypeId = st;
            return View("SRQueueIndex", base.BaseIndex<ServiceRequestDets>(page, "sr.SRID, TDate, event as request,FName, SName, Phone, IgnoreReason  ", $"ServiceRequest sr inner join SR_Cust sc on sr.SRID = sc.ServiceRequestID " +
                $"inner join Customer c on c.CustomerID = sc.CustomerID inner join SRlogs l on sr.SRID=l.SRID where ServiceTypeID={(int)st} and SRStatusID in({(int)SRStatusEnum.New} , {(int)SRStatusEnum.Reminder})"));
        }



        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public ActionResult SRDiaryDets(DateTime? AN)
        {
            ViewBag.Title = "Diary";
            AN=AN ?? DateTime.Now.Date;
            
            return View("SRDiaryDets", base.BaseIndex<SRdetailDets>(1, " srd.serviceTypeId, srd.srdid ", $"ServiceRequest sr , SRdetails srd where sr.SRstatusID in ({(int)SRStatusEnum.Confirmed},{(int)SRStatusEnum.Completed}) and sr.srid=srd.SRID " +
                $"and (convert(date,srd.FDate)='{AN:yyyy-MM-dd}' or ('{AN:yyyy-MM-dd}' between srd.Fdate and srd.TDate))"));

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
            string UserName = "";

            if (columnSearch[2]?.Length > 0) { CustName = columnSearch[2]; columnSearch[2] = null; }            
            if (columnSearch[3]?.Length > 0) { Cphone = columnSearch[3]; columnSearch[3] = null; }
            if (columnSearch[4]?.Length > 0) { Cemail = columnSearch[4]; columnSearch[4] = null; }
            if (columnSearch[5]?.Length > 0) { AgentName = columnSearch[5]; columnSearch[5] = null; }
            if (columnSearch[6]?.Length > 0) { UserName = columnSearch[6]; columnSearch[6] = null; }



            var sql = new PetaPoco.Sql($"Select SRID, BookingNo,TDate, CONCAT(c.FName, ' ', c.SName) as CName, c.Phone, c.email, u.realName as AgentName, substring(e.RealName,1,10) as UserName, SRStatusID, PayStatusID " +
                $"from ServiceRequest s left join AspNetUsers u on u.id=s.AgentId left join AspNetUsers e on s.EmpId=e.Id ");
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
            if (UserName.Length > 0)
            {
                wheresql.Append($" and e.RealName like '%{UserName}%' ");
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
            "UserName",
            "SRStatusID",
            "PayStatusID"
        };
        private string[] SRDetColumns => new string[]
       {
            "SRID",
            "ServiceTypeName",
            "FromLoc",
            "ToLoc",            
            "fstr",
            "SupplierName",
            "Cost",
            "SellPrice",
            "IsCanceled"
       };


      

        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public ActionResult GetSRInfo(int id, int? mode)
        {
            var rec = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest Where SRID=@0", id);
            rec.UserName = db.ExecuteScalar<string>("Select substring(RealName,1,3) From AspNetUsers Where Id=@0", rec.EmpID);
            rec.AgentName = db.ExecuteScalar<string>("Select RealName From AspNetUsers Where Id=@0", rec.AgentID);
            ViewBag.mode = mode;
            return PartialView("InfoHeader", rec);
        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult FetchDetails(int? id, bool DirectBF=false)
        {
            ViewBag.Title = id.Value>0 ?"Booking Folder": "New Enquiry or Booking";

            var SR = base.BaseCreateEdit<ServiceRequest>(id, "SRID");
            if (id > 0)
            {
                if (SR.SRStatusID < (int)SRStatusEnum.Unconfirmed)
                    SR.SRStatusID = (int)SRStatusEnum.Unconfirmed;

                ViewBag.AgentName = db.ExecuteScalar<string>("Select RealName From AspNetUsers where Id = @0 ", SR.AgentID);
                ViewBag.AgentDetails = db.FirstOrDefault<Agent>($"Select * from Agent where AgentId='{SR.AgentID}'");
                ViewBag.LeadCust = db.FirstOrDefault<CustomerDets>("Select c.*, sc.IsLead From Customer c inner join SR_Cust sc on sc.CustomerID = c.CustomerID " +
                    "inner join ServiceRequest sr on sr.SRID = sc.ServiceRequestID where sc.ServiceRequestID =@0", id);
                ViewBag.BookingTypeID = Enum.GetValues(typeof(BookingTypeEnum)).Cast<BookingTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            }


            if (id.HasValue && id>0)         //on 6 sept we changed this for the auto status feature.
                ViewBag.SRStatusID = db.Single<ServiceRequest>(id).SRStatusID;
                //ViewBag.SRStatusID = Enum.GetValues(typeof(SRStatusEnum)).Cast<SRStatusEnum>().Where(v => v > SRStatusEnum.NoAction).Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();            


            
            ViewBag.EnquirySource = Enum.GetValues(typeof(EnquirySourceEnum)).Cast<EnquirySourceEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            ViewBag.ServiceTypeID = Enum.GetValues(typeof(ServiceTypeEnum)).Cast<ServiceTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();

            ViewBag.Cust = db.FirstOrDefault<Customer>("Select FName,SName from Customer where CustomerID=@0", SR?.CustID ?? 0);
                        
            return DirectBF? PartialView("DetailsDirectBooking", SR): PartialView("Details", SR);
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
        public ActionResult Manage(string Response,int? id, int EID = 0, bool DirectBF=false) 
        {
           
            ViewBag.EID = EID;
            ViewBag.SRID = id;
            ViewBag.DirectBF = DirectBF;
            ViewBag.Response = Response;
            ViewBag.Title = (id.HasValue && id.Value > 0 || DirectBF) ? "Booking Folder" : "Enquiry";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult Manage([Bind(Include = "SRID,BookingNo,CustID,SRStatusID,EmpID,BookingTypeID,EnquirySource,AgentID,TDate,PayStatusID,ServiceTypeID,AgentBooker")] ServiceRequest item, string Event, int? CID, string FName, string SName, string Email, string Phone, string routeto,bool isLead=false)
        {
            using (var transaction = db.GetTransaction())
            {
                if (ModelState.IsValid)
                {                    
                    item.EmpID = User.Identity.GetUserId();
                    if (item.TDate==null)
                        item.TDate = DateTime.Now;

                    if (item.SRStatusID == null) //New 
                    {
                        item.SRStatusID = routeto=="BFSave" ? (int)SRStatusEnum.Unconfirmed : (int)SRStatusEnum.New; //make Enq or Direct BF                        
                    }

                    if (routeto == "BFDelete")
                    {
                        item.SRStatusID = (int)SRStatusEnum.Deleted;
                        item.PayStatusID = (int)PayType.Deleted;
                        LogAction(new SRlog { SRID = item.SRID, Event = "Booking deleted" });
                    }


                    if (item.CustID == null)
                        item.CustID=CID;

                    item.PayStatusID = item.PayStatusID ?? (int) PayType.Not_Paid;

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
                        db.Insert(new SR_Cust { ServiceRequestID = item.SRID, CustomerID = (int)CID, IsLead=isLead });
                    }
                    else if (FName != null && Phone != null && Email != null)
                    {
                        var cust = new Customer { FName = FName, SName = SName, Phone = Phone, Email = Email, Type= "Adult" }; //Assume that the Lead is an Adult
                        db.Insert(cust);
                        db.Insert(new SR_Cust { ServiceRequestID = item.SRID, CustomerID = cust.CustomerID, IsLead=isLead });
                        item.CustID = cust.CustomerID;
                        db.Update(item);
                    }

                    if (Event?.Length == null)
                    {
                        Event = "User has Edited a Field";
                    }
                    
                    transaction.Complete();
                    return new JsonResult { Data = new { success = true, routeto =routeto, srid= item.SRID}  };
                }
                else
                {
                    transaction.Dispose();
                    throw new Exception(MyExtensions.getModelStateErrors(ModelState));
                }
            }

        }
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRdetails(int? id, int? EID)//Services tab
        {
            var rec = base.BaseCreateEdit<SRdetail>(EID, "SRDID");
            
            ViewBag.SRID = id;
            ViewBag.Title = "Manage Services";
            ViewBag.SRs = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest sr inner join Customer c on c.CustomerID=sr.CustID Where SRID=@0", id);
            ViewBag.SRDets = db.Query<SRdetail>("Select srdid,ServiceTypeId from SRdetails where SRID =@0 order by Fdate", id);
            List<SelectListItem> ServiceTypeID = Enum.GetValues(typeof(ServiceTypeEnum)).Cast<ServiceTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            var defaultServType = db.Single<ServiceRequest>(id).ServiceTypeID;
            var selST= ServiceTypeID.First(a => int.Parse(a.Value) == defaultServType);
            selST.Selected = true;
            ViewBag.ServiceTypeID = ServiceTypeID;
            if (rec!=null)
                ViewBag.OptionTypeID = new SelectList(db.Fetch<OptionType>("Select OptionTypeID,OptionTypeName from OptionType where ServiceTypeID=@0", (int)rec.ServiceTypeID), "OptionTypeID", "OptionTypeName",rec.OptionTypeID);
            else
                ViewBag.OptionTypeID = new SelectList(db.Fetch<OptionType>("Select OptionTypeID,OptionTypeName from OptionType where ServiceTypeID=@0", defaultServType), "OptionTypeID", "OptionTypeName");

            return PartialView(rec);
        }

        //Gets the details of the services saved in this booking folder.
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public PartialViewResult FetchSRdetails(ServiceTypeEnum sType, int srdid)
        {
            //First Fetch the uploads
            ViewBag.ufs = db.Query<SRUpload>("Where srdid=@0", srdid);

            //Next get the data of the SR
            switch (sType)
            {
                case ServiceTypeEnum.Accomodation:
                    return PartialView($"ReadPVs/_{(sType).ToString()}", db.SingleOrDefault<AccomodationServiceView>($"select (select top 1 CONCAT(c.fName, ' ',c.sName) from Customer c, SRD_Cust sc where c.CustomerID=sc.CustomerID and sc.SRDID={srdid}) as PaxName, " +
                        $"sd.AdultNo,CarType as ExtraBedCost, ChildNo,ContractNo,cost,sd.CouponCode,ECommision,Fdate as checkin,FromLoc,HasAc,Heritage as ExtraService,InfantNo, " +
                        $"IsCanceled, Model as AccomName, ot.OptionTypeName as RoomCategory,payto,Pickuppoint as RoomType,qty as NoOfRooms, Sellprice,sd.ServiceTypeID,SRDID,SRID, " +
                        $"SuppInvNo,SupplierName,sd.SupplierID,Tdate as checkout, SuppInvDt,SuppConfNo,NoExtraBeds,BFCost,LunchCost,DinnerCost,NoExtraService,ExtraServiceCost,SuppInvAmt, " +
                        $"ECommision,Tax,EBCostPNight from SRdetails sd left join Supplier s on s.supplierid=sd.supplierid left join OptionType ot on ot.OptionTypeId=sd.OptionTypeId WHERE SRDID = {srdid} "));
                    break;
                case ServiceTypeEnum.Packages:
                    return PartialView($"ReadPVs/_{(sType).ToString()}", db.SingleOrDefault<Packagevw>($"SELECT (select top 1 CONCAT(c.fName, ' ',c.sName) from Customer c, SRD_Cust sc " +
                        $"where c.CustomerID = sc.CustomerID and sc.SRDID = {srdid}) as PaxName, sd.ChildNo as NOOfPax, " +
                        $"sd.Model as PackageType, sd.Fdate, sd.Tdate, " +
                        $"sd.AdultNo as NoOfDays, ot.OptionTypeID, CarType,sd.IsReturn, sd.FromLoc, " +
                        $"sd.ToLoc, sd.CouponCode as NameofTour, ot.OptionTypeID, sd.DropPoint as HotelCat, " +
                        $"sd.Cost, sd.Heritage as AddDtl, sd.LunchCost as AddCost, sd.BFCost as NetCost, Sellprice, " +
                        $"SRDID, SRID, SuppInvNo, ContractNo,SupplierName, sd.SupplierID, SuppInvDt, SuppConfNo, SuppInvAmt " +
                        $"FROM SRdetails sd left join Supplier s on s.supplierid = sd.supplierid " +
                        $"left join OptionType ot on ot.OptionTypeID = sd.OptionTypeID WHERE SRDID = {srdid} "));
                    break;
                case ServiceTypeEnum.Cruise:
                    return PartialView($"ReadPVs/_{(sType).ToString()}", db.SingleOrDefault<Cruisevw>($"select (select top 1 CONCAT(c.fName, ' ',c.sName) from Customer c, " +
                        $"SRD_Cust sc where c.CustomerID = sc.CustomerID and sc.SRDID = {srdid}) as PaxName, " +
                        $"sd.Qty as Passengers, sd.NoExtraBeds as Cabins, sd.Model as CabinType, sd.Fdate as Departuredate, sd.Tdate as ReturnDate, " +
                        $"sd.PickUpPoint as FromPort, sd.FromLoc as ViaPoint, sd.DropPoint as ToPort, CouponCode as MealPlan, ot.OptionTypeID,sd.ServiceTypeID,Sellprice, SRDID, SRID," +
                        $"SuppInvNo, SupplierName, ContractNo, sd.CouponCode, sd.SupplierID, sd.CouponCode, SuppInvDt, SuppConfNo, SuppInvAmt, " +
                        $"sd.Cost, sd.ContractNo as CruiseName from SRdetails sd " +
                        $"left JOIN Supplier su ON su.SupplierID = sd.SupplierID " +
                        $"left join OptionType ot on ot.OptionTypeId = sd.OptionTypeId WHERE SRDID = {srdid}"));
                    break;
                case ServiceTypeEnum.SightSeeing:
                    ViewBag.Inc = db.FirstOrDefault<SRlog>("where Type=1 and srdid=@0", srdid)?.Event ??"";
                    var qry = $"select (select top 1 CONCAT(c.fName, ' ',c.sName) from Customer c, SRD_Cust sc where c.CustomerID=sc.CustomerID and sc.SRDID={srdid}) as PaxName, " +
                        $" AdultNo,ChildNo,sd.Model as SightseeingName, sd.OptionTypeID, ot.OptionTypeName,PickUpPoint,FromLoc as PickupLocation, Heritage as Private_Sic, cost as CostPerCar, " +
                        $"Qty as NoOfCars, BFCost as AdultCost, LunchCost as ChildCost,Fdate as TourDate,d.DriverID,d.DriverName, CONCAT (dc.CarBrand, ' ', dc.Model, ' ', dc.PlateNo) as Car, CarType,HasAc as MealIncluded, srid, srdid, IsCanceled, sd.ServiceTypeID, SuppInvNo, " +
                        $"SupplierName,sd.SupplierID, SuppInvDt,SuppConfNo,SuppInvAmt,sd.CouponCode, SellPrice,contractNo,ECommision,Tax from SRdetails sd left join OptionType ot on ot.OptionTypeID=sd.OptionTypeID " +
                        $"LEFT JOIN Driver d ON sd.DriverID = d.DriverID left JOIN Supplier su ON su.SupplierID = sd.SupplierID LEFT JOIN DriversCars dc ON dc.CarId = NoExtraBeds WHERE SRDID = {srdid} ";
                    var res = db.SingleOrDefault<SightseeingServiceView>(qry);
                    return PartialView($"ReadPVs/_{(sType).ToString()}", res);
                    break;
                case ServiceTypeEnum.CarBike:
                    return PartialView($"ReadPVs/_{(sType).ToString()}", db.SingleOrDefault<CarBikevw>($"select (select top 1 CONCAT(c.fName, ' ',c.sName) from Customer c, " +
                        $"SRD_Cust sc where c.CustomerID = sc.CustomerID and sc.SRDID = {srdid}) as PaxName, " +
                        $"CONCAT(dc.CarBrand, ' ', dc.Model, ' ', dc.PlateNo) AS Car, sd.PickUpPoint, sd.DropPoint, " +
                        $"sd.Fdate, sd.Tdate, sd.FromLoc, sd.ToLoc, sd.Cost, sd.CarType, sd.NoExtraBeds, sd.Qty, sd.PayTo, " +
                        $"Sellprice, sd.ServiceTypeID, ContractNo,SRDID, SRID, SuppInvNo, SupplierName, " +
                        $"sd.CouponCode, sd.SupplierID, sd.CouponCode, sd.SuppInvDt, " +
                        $"sd.SuppConfNo, sd.SuppInvAmt from SRdetails sd left JOIN Supplier su ON su.SupplierID = sd.SupplierID " +
                        $"left join OptionType ot on ot.OptionTypeId = sd.OptionTypeId " +
                        $"LEFT JOIN DriversCars dc ON dc.CarId = NoExtraBeds WHERE SRDID = {srdid}"));
                    break;
                case ServiceTypeEnum.Insurance:
                    return PartialView($"ReadPVs/_{(sType).ToString()}", db.SingleOrDefault<Insurancevw>($"SELECT (select top 1 CONCAT(c.fName, ' ',c.sName) from Customer c, SRD_Cust sc " +
                        $"where c.CustomerID = sc.CustomerID and sc.SRDID = {srdid}) as PaxName, " +
                        $"sd.DateOfIssue as DOB, sd.PickUpPoint as Destination, sd.Model as PolicyName, sd.ToLoc as PolicyType, sd.NoExtraBeds as NoofDays, " +
                        $"sd.Fdate as ValidFrom, sd.Tdate as ValidTo, sd.Cost, Sellprice, sd.ServiceTypeID, SRDID, SRID, " +
                        $"sd.ServiceTypeID, SRDID, SRID, CouponCode,ContractNo,SuppInvNo, SupplierName, sd.SupplierID, SuppInvDt, SuppConfNo, SuppInvAmt FROM SRdetails sd " +
                        $"left join Supplier s on s.supplierid = sd.supplierid WHERE SRDID = {srdid}"));
                    break;
                case ServiceTypeEnum.Flight:
                    return PartialView($"ReadPVs/_{(sType).ToString()}", db.SingleOrDefault<FlightServiceView>($"SELECT (select top 1 CONCAT(c.fName, ' ',c.sName) from Customer c, SRD_Cust sc where c.CustomerID=sc.CustomerID and sc.SRDID={srdid}) as PaxName, " +
                        "sd.AdultNo, sd.ChildNo,sd.InfantNo, IsInternational, FromLoc ,ToLoc,CarType as ClassID, Model as AirlineCode, Heritage as FlightNo,Fdate as DepartureOn, Tdate as ArrivalOn, " +
                        "PickupPoint as TicketNo,GDSConfNo,PayTo as AirlinePNR, Cost,ContractNo,sd.CouponCode,ECommision,IsCanceled, ot.OptionTypeName as Extra, " +
                        "DropPoint as ExtraDetails, Sellprice,sd.ServiceTypeID,SRDID,SRID, SuppInvNo,SupplierName,sd.SupplierID, SuppInvDt,SuppConfNo,SuppInvAmt, ECommision,Tax " +
                        $"from SRdetails sd left join Supplier s on s.supplierid=sd.supplierid left join OptionType ot on ot.OptionTypeId=sd.OptionTypeId WHERE SRDID = {srdid} "));
                    break;
                case ServiceTypeEnum.Visa:
                    return PartialView($"ReadPVs/_{(sType).ToString()}", db.SingleOrDefault<Visavw>($"SELECT (select top 1 CONCAT(c.fName, ' ',c.sName) from Customer c, SRD_Cust sc " +
                        $"where c.CustomerID = sc.CustomerID and sc.SRDID = {srdid}) as PaxName, " +
                        $"sd.AdultNo as PassportNo, sd.DateOfIssue as DOB, sd.ExpiryDate, " +
                        $"sd.FromLoc as Nationality,sd.Tdate,sd.Heritage as VisaCountry, sd.Fdate, sd.Cost, sd.ChildNo as Duration, " +
                        $"sd.ServiceTypeID,Sellprice, sd.ExtraServiceCost,SRDID, SRID, SuppInvNo, CouponCode,ContractNo,SupplierName, sd.SupplierID, SuppInvDt, SuppConfNo, SuppInvAmt FROM SRdetails sd " +
                        $"left join Supplier s on s.supplierid = sd.supplierid " +
                        $" WHERE SRDID = {srdid}"));
                    break;
                case ServiceTypeEnum.Transfer:
                    return PartialView($"ReadPVs/_{(sType).ToString()}", db.SingleOrDefault<TransferServiceView>($"SELECT (select top 1 CONCAT(c.fName, ' ',c.sName) from Customer c, SRD_Cust sc where c.CustomerID=sc.CustomerID and sc.SRDID={srdid}) as PaxName, " +
                        "sd.srid, sd.cartype, Tdate AS serviceDate, sd.ContractNo, sd.Cost, sd.CouponCode, d.DriverName, CONCAT (dc.CarBrand, ' ', dc.Model, ' ', dc.PlateNo) AS Car, " +
                        "sd.DropPoint, sd.ECommision, sd.Fdate, sd.FromLoc,sd.ToLoc, sd.HasAc, sd.HasCarrier, sd.Heritage AS RateBasis, sd.IsCanceled, sd.Model, sd.PayTo, sd.PickUpPoint, " +
                        "sd.Qty AS NoOfVehicles, Sellprice, sd.ServiceTypeID, sd.SRDID, sd.SuppInvNo, sd.SuppConfNo, sd.BFCost as VehicleCost, sd.SuppInvDt, sd.SuppInvAmt,ECommision,Tax FROM SRdetails sd " +
                        $"LEFT JOIN Driver d ON sd.DriverID = d.DriverID LEFT JOIN DriversCars dc ON dc.CarId = NoExtraBeds WHERE SRDID = {srdid} "));
                    break;
                case ServiceTypeEnum.Passport:
                    return PartialView($"ReadPVs/_{(sType).ToString()}", db.SingleOrDefault<PassportView>($"SELECT (select top 1 CONCAT(c.fName, ' ',c.sName) from Customer c, SRD_Cust sc where c.CustomerID=sc.CustomerID and sc.SRDID={srdid}) as PaxName, " +
                        "sd.Fdate as DOB, sd.srid, sd.FromLoc as Nationality, sd.ContractNo as PassPortNo, sd.Heritage, sd.Cost,sd.ServiceTypeID, Sellprice, " +
                        "SRDID, SRID, SuppInvNo, SupplierName, sd.SupplierID,CouponCode, ContractNo,SuppInvDt, SuppConfNo, SuppInvAmt FROM SRdetails sd " +
                        "left join Supplier s on s.supplierid = sd.supplierid " +
                        $"WHERE SRDID = {srdid} "));
                    break;
                case ServiceTypeEnum.Bus:
                    return PartialView($"ReadPVs/_{(sType).ToString()}", db.SingleOrDefault<Busvw>($"SELECT (select top 1 CONCAT(c.fName, ' ',c.sName) from Customer c, SRD_Cust sc " +
                        $"where c.CustomerID = sc.CustomerID and sc.SRDID = {srdid}) as PaxName, sd.ChildNo as Age," +
                        $"sd.DateOfIssue as DOT, sd.FromLoc, sd.ToLoc, sd.Model as BusName,ot.OptionTypeID, sd.InfantNo as BusNo, sd.AdultNo as TicketNo, " +
                        $"sd.Fdate as Arrival, sd.Tdate as Departure, sd.Cost, sd.LunchCost as AddCost,sd.CouponCode as AddDetl, sd.BFCost as FullCost, Sellprice,CarType," +
                        $"sd.ServiceTypeID, SRDID, SRID, SuppInvNo, ContractNo,SupplierName, CouponCode,sd.SupplierID, SuppInvDt, SuppConfNo, SuppInvAmt " +
                        $"FROM SRdetails sd left join Supplier s on s.supplierid = sd.supplierid " +
                        $"left join OptionType ot on ot.OptionTypeId = sd.OptionTypeId WHERE SRDID = {srdid}"));
                    break;
                case ServiceTypeEnum.Rail:
                    return PartialView($"ReadPVs/_{(sType).ToString()}", db.SingleOrDefault<Railvw>($"SELECT (select top 1 CONCAT(c.fName, ' ',c.sName) from Customer c, SRD_Cust sc " +
                        $"where c.CustomerID = sc.CustomerID and sc.SRDID = {srdid}) as PaxName, sd.ChildNo as Age, " +
                        $"sd.DateOfIssue as DOT, sd.FromLoc, sd.ToLoc, sd.Model as TrainName,sd.InfantNo as TrainNo,CarType, " +
                        $"sd.AdultNo as TicketNo, sd.Heritage as Class, " +
                        $"sd.Fdate as Arrival, sd.Tdate as Departure, sd.Cost as TicketCost, sd.LunchCost as AddCost," +
                        $"sd.BFCost as TotalCost, Sellprice, SRDID,CouponCode, ContractNo, " +
                        $"sd.ServiceTypeID, SRDID, SRID, SuppInvNo, SupplierName, sd.SupplierID, SuppInvDt, SuppConfNo, SuppInvAmt " +
                        $"FROM SRdetails sd left join Supplier s on s.supplierid = sd.supplierid " +
                        $"left join OptionType ot on ot.OptionTypeId = sd.OptionTypeId WHERE SRDID = {srdid}"));
                    break;
                default:
                    return null;
                    break;
            }
                    return null;
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRdetails([Bind(Include = "SRDID,SRID, ServiceTypeID,CarType,ItemID,CouponCode,Model,FromLoc,ToLoc,Fdate,Tdate,SupplierID,Cost,SellPrice,ChildNo," +
            "AdultNo,InfantNo,Heritage,HasAc,HasCarrier,GuideLanguageID,DateOfIssue,ContractNo,PayTo,PickUpPoint,DropPoint,DriverID,SuppInvNo,Qty,ParentID,IsReturn," +
            "IsInternational,OptionTypeID,ECommision,IsCanceled,SuppInvDt,SuppConfNo,NoExtraBeds,BFCost,LunchCost,DinnerCost,NoExtraService,ExtraServiceCost," +
            "SuppInvAmt,GDSConfNo,ExpiryDate,EBCostPNight")] SRdetail item, string Event, int CustomerId)
        {
            using (var transaction = db.GetTransaction())
            {
                
                try
                {
                    //Calc the tax and Employee commission
                    var tax = db.ExecuteScalar<decimal?>("Select Percentage From Taxes Where ServiceTypeID=@0 and WEF<GetDate() order by WEF desc ", item.ServiceTypeID) ?? 0;
                    item.Tax = item.SellPrice * tax / 100;
                    var c = db.FirstOrDefault<ServiceCommision>($"Select Perc,Amount  From ServiceCommision Where Serviceid={item.ServiceTypeID} ");
                    if (c != null)
                    {
                        item.ECommision += c.Amount ?? 0;
                        item.ECommision = (item.SellPrice * (c.Perc ?? 0)) / 100;
                    }


                    //For Accomodation calc the total cost
                    if (item.ServiceTypeID==(int)ServiceTypeEnum.Accomodation)
                    {
                        var NoNights = int.Parse((item.Tdate - item.Fdate).Value.TotalDays.ToString());
                        item.Cost = ((item.EBCostPNight * item.Qty) * NoNights) + (item.NoExtraBeds??0 * item.CarType??0) + ((item.BFCost??0 + item.LunchCost ??0+ item.DinnerCost??0) * NoNights);
                    }

                    DateTime? td =(DateTime?)item?.Tdate ;
                    if(item.SRDID > 0)//Update existing record when in edit mode
                    {
                        LogAction(new SRlog { SRID = item.SRID, SRDID=item.SRDID, Event = Event, Type=true});
                        LogAction(new SRlog { SRID = item.SRID, SRDID = item.SRDID, Event = FindDiffs<SRdetail>(item.SRDID,item,item.SRDID + ": " + item.ServiceTypeName) });
                        db.Update(item);

                        db.Execute($"Update SRD_Cust Set CustomerId ='{CustomerId}' where srdid ={item.SRDID}");

                        if (td.HasValue)
                        {
                            db.Execute($"Update Srdetails Set Tdate ='{td.EAFormat(true,true)}',FromLoc='{item.ToLoc}',ToLoc ='{item.FromLoc}' where ParentID ={item.SRDID}");
                        }
                    }
                    else //Insert new SRdetail
                    {
                        string logNote = FindDiffs<SRdetail>(0, item, "");                        
                        db.Insert(item);
                        if (td.HasValue && item.ServiceTypeID==(int)ServiceTypeEnum.Flight)
                        {//We need to insert this duplicate record so that we can show this return flight in the Daily Diary on the return journey date
                            db.Insert(new SRdetail { FromLoc = item.ToLoc, ToLoc = item.FromLoc, Tdate = td, ParentID = item.SRDID, ServiceTypeID = item.ServiceTypeID });
                        }

                        db.Insert(new SRD_Cust { CustomerID = CustomerId, SRDID = item.SRDID });
                        logNote = item.SRDID + ": " + item.ServiceTypeName + " added:- " + logNote;
                        LogAction(new SRlog { SRID = item.SRID, SRDID = item.SRDID, Event = Event, Type = true });
                        LogAction(new SRlog { SRID = item.SRID, SRDID = item.SRDID, Event = logNote });
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
        public ActionResult SRUpload([Bind(Include = "SRUID,SRID,SRDID,Path,UploadName,UploadedFile")] SRuploadDets item,FormCollection fm )
        {
            SRUpload res = new SRUpload
            {
                SRID = item.SRID,
                SRDID = item.SRDID,
                SRUID = item.SRUID,
                UploadName = item.UploadName
            };

            if (item.UploadedFile?.ContentLength > 0)
            {
                string fn = item.UploadedFile.FileName.Substring(item.UploadedFile.FileName.LastIndexOf('\\') + 1);
                fn = item.UploadName + "_" + fn;

                string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                item.UploadedFile.SaveAs(SavePath);

                res.Path = fn;
            }
            LogAction(new SRlog { SRID = item.SRID, SRDID=item.SRDID, Event = item.UploadName + " Uploaded" });
            return base.BaseSave<SRUpload>(res, item.SRUID > 0, "Manage", new { id = item.SRID, mode = 5 });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public async System.Threading.Tasks.Task<ActionResult> SRCanxServiceAsync([Bind(Include = "SRID,SRDID,ProdCanxCost,SBCanxCost,Note")] Refunds item)
        {
            using (var transaction = db.GetTransaction())
            {
                //If the user cancels a service give him a refund by first minusing the Product and SB canx costs.
                var SRDetails = db.Single<SRdetail>(item.SRDID);
                decimal refundAmt = (SRDetails.SellPrice ?? 0) - item.ProdCanxCost - item.SBCanxCost;

                if (SRDetails.ServiceTypeID == (int)ServiceTypeEnum.Transfer || SRDetails.ServiceTypeID == (int)ServiceTypeEnum.SightSeeing)
                {
                    var drpdets = new DRPdet() { Date = DateTime.Today, Amount = -refundAmt, Note = item.Note, AmtUsed = true, IsPayment = true, Cdate = DateTime.Today };
                    var res= db.Insert(drpdets);

                    var drp_sr = new DRP_SR() { Amount = -refundAmt, SRID = item.SRID, SRDID = item.SRDID, DRPDID=(int)res };
                    db.Insert(drp_sr);
                }
                else
                {
                    var rpdets = new RPdet() { Date = DateTime.Today, Amount = -refundAmt, Note = item.Note, AmtUsed = true, IsPayment = true, Cdate = DateTime.Today };
                    var res= db.Insert(rpdets);

                    var rp_sr = new RP_SR() { Amount = -refundAmt, SRID = item.SRID, SRDID=item.SRDID, RPDID=(int)res };
                    db.Insert(rp_sr);
                }

                //If the supplier has been paid then adjust his payment amount too.
                var supPayRecs = db.Query<DRP_SR>("where SRID=@0", item.SRID);
                var supPay = supPayRecs.Sum(s => s.Amount) ?? 0; //Amount already paid to supplier for this booking

                //Set the cost of this Service to ProdCanxCost
                SRDetails.Cost = item.ProdCanxCost;
                db.Update(SRDetails);

                //Amt that the supplier needs to refund: Min(already paid to sup, Service cost) - Prod Canx.
                //if supOwedAmt +ve: Supplier has to redund SB, else SB to pay supplier.
                var supOwedAmt = Math.Min(supPay, (SRDetails.Cost??0)) - item.ProdCanxCost;
                //if (supOwedAmt>0)
                //{
                //    var tmpsupOwedAmt = supOwedAmt;
                //    foreach (var rec in supPayRecs)
                //    {
                //        var valToSubtract = Math.Min((rec.Amount ?? 0), supOwedAmt);

                //        valToSubtract= (valToSubtract>0)?valToSubtract:

                //        tmpsupOwedAmt -= valToSubtract;
                //        rec.Amount -= valToSubtract;
                //        db.Update(rec);
                //    }
                //} 

                SRDetails.IsCanceled = true;
                LogAction(new SRlog { SRID = item.SRID, SRDID = item.SRDID, Event = $"Refund of {refundAmt} given for {SRDetails.ServiceTypeName}: {item.SRDID} . Supplier refund amount is {supOwedAmt}" });

                //fetching customer info with current servicerequestid
                var customer = db.SingleOrDefault<ServiceCustomervw>("select CONCAT(cu.FName, ' ', cu.SName) as FullName, cu.Email, cu.Phone, src.IsLead from SR_Cust src " +
                        "left join Customer cu on cu.CustomerID = src.CustomerID " +
                        "left join ServiceRequest srq on srq.SRID = src.ServiceRequestID " +
                        "where src.ServiceRequestID = @0 and IsLead = 1", item.SRID);

                var config = db.FirstOrDefault<Config>("select merchantid,pwd from config");
                var atomtxn = db.SingleOrDefault<AtomPaymentLog>("select rmmp_txn,TDate from AtomPaymentLogs where srid = @0", item.SRID);

                string strpwd, strpwdencoded;
                byte[] b;

                b = Encoding.UTF8.GetBytes(config.Pwd);
                strpwd = Convert.ToBase64String(b);
                strpwdencoded = HttpUtility.UrlEncode(strpwd);

                string merchantid = config.MerchantId;
                string atomtxnid = atomtxn.RMmp_txn;
                string refundamt = refundAmt.ToString();
                string txndate = atomtxn.TDate.Value.ToString("yyyy-MM-dd");
                string merrefundref = "M" + item.SRID;


                var client = new HttpClient();
               
                    var values = new List<KeyValuePair<string, string>>();

                    values.Add(new KeyValuePair<string, string>("merchantid", merchantid));
                    values.Add(new KeyValuePair<string, string>("pwd", strpwdencoded));
                    values.Add(new KeyValuePair<string, string>("atomtxnid", atomtxnid));
                    values.Add(new KeyValuePair<string, string>("refundamt", refundamt));
                    values.Add(new KeyValuePair<string, string>("txndate", txndate));
                    values.Add(new KeyValuePair<string, string>("merefundref", merrefundref));

                    var content = new FormUrlEncodedContent(values);

                    var response = await client.PostAsync("https://paynetzuat.atomtech.in/paynetz/rfts", content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    string responses = responseString.Replace("\n", String.Empty);


                    XmlSerializer xmlSer = new XmlSerializer(typeof(REFUND));

                    StringReader stringReader = new StringReader(responseString);
                    REFUND reader = (REFUND)xmlSer.Deserialize(stringReader);

                    //var refundvw = new Refundvw();
                    //{
                    //    refundvw.merchantid = reader.MERCHANTID;
                    //    refundvw.txnid = reader.TXNID;
                    //    refundvw.amount = reader.AMOUNT;
                    //    refundvw.statuscode = reader.STATUSCODE;
                    //    refundvw.statusmsg = reader.STATUSMESSAGE;
                    //    refundvw.refundid = reader.ATOMREFUNDID;
                    //}

                
                //inserting Logs in Refund table
                db.Insert(new AtomRefundLog { AtomRefundId = reader.ATOMREFUNDID, MerchantReferanceId = merrefundref, AtomTxnId = reader.TXNID, RefundAmt = reader.AMOUNT ,StatusCode = reader.STATUSCODE, StatusMessege = reader.STATUSMESSAGE, Tdate = DateTime.Now, SRID = item.SRID, SRDID = item.SRDID,UserId = User.Identity.GetUserId(), CustomerInfo = customer.FullName });
                ViewBag.Response = reader.STATUSMESSAGE + " with TransactionId = " + reader.TXNID + ",Amount " + reader.AMOUNT + "and RefundID = " + reader.ATOMREFUNDID + " of Customer " + customer.FullName;
                transaction.Complete();
                return RedirectToAction("Manage", new { id = item.SRID, mode = 5, ViewBag.Response });
            }

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
            ViewBag.Custs = db.Fetch<CustomerDets>($"Select c.*, sc.IsLead From Customer c inner join SR_Cust sc on sc.CustomerID = c.CustomerID inner join ServiceRequest sr on sr.SRID = sc.ServiceRequestID where sc.ServiceRequestID ='{id}'");
            return PartialView(rec);
        }

        [HttpPost]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRCustomers(string FName, string SName, string Email, string Phone, int? CID, int? SRID,string Type,string UploadName)
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

            //if (UploadedFile != null && UploadName != null)
            //{
            //    string fn = UploadedFile.FileName.Substring(UploadedFile.FileName.LastIndexOf('\\') + 1);
            //    fn = UploadName + "_" + fn;

            //    string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
            //    UploadedFile.SaveAs(SavePath);

            //    db.Insert(new SRUpload { UploadName = UploadName, Path = fn, SRID = SRID });
            //    LogAction(new SRlog { SRID = SRID.Value , Event = UploadName+  " Uploaded"});
            //}
            return RedirectToAction("Manage", new { id = (int)SRID, mode = 4 });

        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult Reciepts(int? id, int? sid, int? EID)
        {
            ViewBag.SRID = id;
            ViewBag.Title = "Profit and loss Details";

            //Get Forecast
            ViewBag.FDebit = db.ExecuteScalar<decimal>("Select Coalesce(sum(Cost),0) from SRdetails Where isCanceled=0 and SRID =@0", id);
            ViewBag.FCredit = db.ExecuteScalar<decimal>("Select Coalesce(sum(SellPrice),0) from SRdetails Where isCanceled=0 and SRID =@0", id);

            //Get actuals (from suppliers and drivers)
            var ADebit = db.ExecuteScalar<decimal?>("Select Coalesce(sum(c.Amount),0) from RP_SR c inner join RPdets m on  m.RPDID=c.RPDID Where c.SRID =@0 and m.IsPayment=1", id) +
                db.ExecuteScalar<decimal>("Select Coalesce(sum(c.Amount),0) from DRP_SR c inner join DRPdets m on  m.DRPDID=c.DRPDID Where c.SRID =@0 and m.IsPayment=1 ", id);            

            var ACredit = db.ExecuteScalar<decimal?>("Select Coalesce(sum(c.Amount),0) from RP_SR c inner join RPdets m on  m.RPDID=c.RPDID Where c.SRID =@0 and m.IsPayment=0", id) +
                db.ExecuteScalar<decimal>("Select Coalesce(sum(c.Amount),0) from DRP_SR c inner join DRPdets m on  m.DRPDID=c.DRPDID Where c.SRID =@0 and m.IsPayment=0 ", id);

            var SRdetail = db.FirstOrDefault<SRdetail>(" where servicetypeId=0 and srid=@0",id); //if pay to driver then set that the supplier is paid off.
            
            List<RPDetails> payDriver = new List<RPDetails>();
            if (SRdetail?.PayTo.Contains("driver")??false)
            {
                ADebit = ADebit + SRdetail.Cost;
                ACredit = ACredit + SRdetail.Cost;
                payDriver= new List<RPDetails> { new RPDetails { Date=SRdetail.Fdate?? DateTime.Today, Type = 0, Amount = SRdetail.Cost ?? 0, IsPayment = true } };
            }
            ViewBag.ADebit = ADebit;
            ViewBag.ACredit = ACredit;


            ViewBag.PaxDetail = db.Fetch<PaxDets>("; Exec AmtPerPax @@SRID = @0", id).ToList();

            var services = db.Query<SRdetailDets>("Select SRID,SRDID,ServiceTypeID,Tax,Cost,SellPrice,ECommision , SellPrice - Tax - ECommision - Cost as Total from SRdetails where SRID=@0", id).ToList();
           
            ViewBag.Services = services;

            var  rp= db.Fetch<RPDetails>("select rp.CDate as [Date],rp.Note,rp.Type,rs.Amount,rp.IsPayment from RPDets rp left join RP_SR rs on rp.RPDID = rs.RPDID Where rs.SRID = @0",id);
            var drp = db.Fetch<RPDetails>("select rp.CDate as [Date],rp.Note,rp.Type,rs.Amount,rp.IsPayment from DRPDets rp left join DRP_SR rs on rp.DRPDID = rs.DRPDID Where rs.SRID = @0", id);
            ViewBag.Reciepts = rp.Concat(drp).Where(d => d.IsPayment == false);
            ViewBag.Payments = rp.Concat(drp).Concat(payDriver).Where(r => r.IsPayment == true);


            return PartialView();
        }


        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRLogs(int? id, int? sid, int? EID)
        {
            var rec = base.BaseCreateEdit<SRReciept>(EID, "RecieptID");
            ViewBag.SRID = id;
            ViewBag.Title = "Service Reauest Logs";

            ViewBag.SRs = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest Where SRID=@0", id);
            ViewBag.SRLogDets = db.Fetch<SRlogsDets>($"Select sl.srlid,sl.srdid,sl.logDateTime,substring(anu.Realname,1,10) as UserName,sl.Type,Event From SRLogs sl inner join AspNetUsers anu on sl.UserID = anu.Id where SRID ='{id}' ORDER By SRLID Desc");
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
            var nameList = new List<SRTranslation>();
            if (newObj is SRdetail)
            {
                nameList = db.Query<SRTranslation>("where ServiceTypeId=@0", newObj.GetType().GetProperty("ServiceTypeID").GetValue(newObj)).ToList();
            }
            if (Id > 0)
            {
                var oldObj = db.Single<T>(Id);
                CompareLogic c = new CompareLogic(new ComparisonConfig { MaxDifferences = 99 });
                ComparisonResult comparisonResult = c.Compare(oldObj, newObj);
                string res = " ";
                foreach (var item in comparisonResult.Differences)
                {
                    string propName = LookupFriendlyName(nameList, item);
                    if (propName.Length>0)
                        res += $"{propName} Changed from: {item.Object1Value} to {item.Object2Value}, ";
                }
                if (res.Length > 1)
                    return objName + " :- " + res.Substring(0, res.Length - 1);
                else
                    return "";
            }
            else
            {                
                CompareLogic c = new CompareLogic(new ComparisonConfig { MaxDifferences = 99 });
                ComparisonResult comparisonResult = c.Compare(Activator.CreateInstance(typeof(T)), newObj);
                string res = " ";
                foreach (var item in comparisonResult.Differences)
                {
                    string propName = LookupFriendlyName(nameList, item);
                    if (propName.Length > 0)
                        res += $"{propName}: {item.Object2Value}, ";
                }
                if (res.Length > 1)
                    return objName + " :- " + res.Substring(0, res.Length - 1);
                else
                    return "";
            }
        }

        private static string LookupFriendlyName(List<SRTranslation> nameList, Difference item)
        {
            string[] blockList = new string[] { "SRID", "ServiceTypeID", "tstr", "fstr" }; //we dont want to log these
            if (blockList.Contains(item.PropertyName))
                return "";

            string propName = nameList.FirstOrDefault<SRTranslation>(x => x.ColumnName == item.PropertyName)?.FriendlyName ?? MyExtensions.CamelToSpaceString(item.PropertyName);
            if (propName.Length < 1) propName = MyExtensions.CamelToSpaceString(item.PropertyName);
            return propName;
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
            var filteredItems = db.Fetch<AspNetUser>($"Select * from AspNetUsers Where UserType={(int)UserTypeEnum.Agent} and RealName like '%{term}%'").Select(c => new { id = c.Id, value = c.RealName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AutoCompleteSup(string term)
        {
            var filteredItems = db.Fetch<Supplier>($"Select * from Supplier Where SupplierName like '%{term}%'").Select(c => new { id = c.SupplierID, value = c.SupplierName });
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
        public JsonResult AutoCompleteCar(string term)
        {
            return GetAutoCompleteData("CarBikeID", "CarBikeName", "CarBike", $"Where CarBikeName like '%{term}%'");
        }
        public JsonResult AutoCompleteDriver(string term)
        {
            return GetAutoCompleteData("DriverID", "DriverName", "Driver", $"Where DriverName like '%{term}%'");            
        }
        public JsonResult AutoCompleteCars(int DriverId)
        {
            return GetAutoCompleteData("CarID", " CONCAT (CarBrand, ' ', Model, ' ', PlateNo) ", "DriversCars", $"Where DriverId ={DriverId}");
        }

        public ActionResult FetchSTpartial(int? id, int ServiceTypeId,  int SRID, bool IsReadOnly = false)
        {           
            ViewBag.SRDID = id;
            var reca = db.SingleOrDefault<SRdetail>(id);
            if (reca != null)
            {
                ViewBag.SelectedCustomerId = db.SingleOrDefault<SRD_Cust>(id)?.CustomerID.ToString();
                ViewBag.Supplier = db.SingleOrDefault<Supplier>(reca.SupplierID)?.SupplierName ?? "";
            }

            ViewBag.CustomerList = db.Fetch<Customer>($"Select c.CustomerId, Concat(Fname, ' ',Sname) as FName  from Customer c, sr_cust r where c.CustomerId=r.CustomerId and r.servicerequestid = @0",SRID).Select(u => new SelectListItem
            {
                Value = u.CustomerID.ToString(),
                Text = u.FName
            });

            if (IsReadOnly)
                ViewBag.IsReadOnly = "disabled";
          

            

            switch ((ServiceTypeEnum)ServiceTypeId)
            {
                case ServiceTypeEnum.Accomodation:
                    List<SelectListItem> AccPayTo = new List<SelectListItem>()
                    {
                            new SelectListItem { Text = "Pay to us", Value = "Pay to us" },
                            new SelectListItem { Text = "Pay to owner", Value = "Pay to owner" }
                        };
                    ViewBag.PayTo = AccPayTo;
                    
                    ViewBag.Heritage = new List<SelectListItem> {
                               new SelectListItem { Text = "Cake", Value = "Cake" },
                               new SelectListItem { Text = "Wine", Value = "Wine" },
                               new SelectListItem { Text = "Bouquet", Value = "Bouquet" },
                               new SelectListItem { Text = "Honeymoon Pkg", Value = "Honeymoon Pkg" },
                            };
                    ViewBag.OptionTypeId = base.GetSelectListData("OptionTypeId", "OptionTypeName", "OptionType",$"where ServiceTypeId={(int)ServiceTypeEnum.Accomodation}");
                    return PartialView($"WritePVs/_{((ServiceTypeEnum)ServiceTypeId).ToString()}", reca);
                case ServiceTypeEnum.SightSeeing:
            
            
                    //    ViewBag.Sightseeing = db.SingleOrDefault<Package>(reca.ItemID).PackageName;
                    ViewBag.Heritage = new List<SelectListItem> {
                                          new SelectListItem { Text = "Private", Value = "Private" },
                                          new SelectListItem { Text = "Sic", Value = "Sic" }
                                        };
                    ViewBag.CarType = Enum.GetValues(typeof(CarTypeEnum)).Cast<CarTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
                    ViewBag.OptionTypeId = base.GetSelectListData("OptionTypeId", "OptionTypeName", "OptionType", $"where ServiceTypeId={(int) ServiceTypeEnum.SightSeeing}");
                    //ViewBag.GuideLanguageID = db.Fetch<GuideLanguage>("Select * from GuideLanguage").Select(v => new SelectListItem { Text = v.GuideLanguageName, Value = v.GuideLanguageID.ToString() }).ToList();
                    return PartialView($"WritePVs/_{((ServiceTypeEnum)ServiceTypeId).ToString()}",reca);
                case ServiceTypeEnum.CarBike:

                    ViewBag.OptionTypeId = base.GetSelectListData("OptionTypeId", "OptionTypeName", "OptionType", $"where ServiceTypeId={(int)ServiceTypeEnum.CarBike}");
                    ViewBag.CarType = Enum.GetValues(typeof(CarTypeEnum)).Cast<CarTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
                    List<SelectListItem> TaxiPayToo = new List<SelectListItem>()
                    {
                            new SelectListItem {
                                Text = "Pay to us", Value = "Pay to us"
                            },
                            new SelectListItem {
                                Text = "Pay to driver", Value = "Pay to driver"
                            }
                        };
                    ViewBag.PayTo = TaxiPayToo;
                    return PartialView($"WritePVs/_{((ServiceTypeEnum)ServiceTypeId).ToString()}",reca);
                case ServiceTypeEnum.Cruise:
                    return PartialView($"WritePVs/_{((ServiceTypeEnum)ServiceTypeId).ToString()}",reca);
                case ServiceTypeEnum.Packages:
                    ViewBag.IsReturn = new List<SelectListItem> {
                               new SelectListItem { Text = "Yes", Value = "true" },
                               new SelectListItem { Text = "No", Value = "false" },
                            };
                    ViewBag.CarType = Enum.GetValues(typeof(MealPlanEnum)).Cast<MealPlanEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
                    ViewBag.OptionTypeId = base.GetSelectListData("OptionTypeId", "OptionTypeName", "OptionType", $"where ServiceTypeId={(int)ServiceTypeEnum.Packages}");
                    return PartialView($"WritePVs/_{((ServiceTypeEnum)ServiceTypeId).ToString()}",reca);
                case ServiceTypeEnum.Insurance:
                    return PartialView($"WritePVs/_{((ServiceTypeEnum)ServiceTypeId).ToString()}",reca);
                case ServiceTypeEnum.Visa:
                    List<SelectListItem> dropDownList = new List<SelectListItem>();
                    var country = db.Fetch<Visa>("select VisaID,VisaCountry from Visa");
                    foreach (var cntr in country)
                    {
                        dropDownList.Add(new SelectListItem() { Text = cntr.VisaCountry, Value = cntr.VisaCountry });
                    }

                    ViewBag.Heritage = dropDownList;
                    //ViewBag.Heritage = new SelectList(db.Fetch<Visa>("Select VisaID,VisaCountry from Visa"), "VisaID", "VisaCountry");
                  
                    return PartialView($"WritePVs/_{((ServiceTypeEnum)ServiceTypeId).ToString()}",reca);
                case ServiceTypeEnum.Passport:
                    ViewBag.OptionTypeId = base.GetSelectListData("OptionTypeId", "OptionTypeName", "OptionType", $"where ServiceTypeId={(int)ServiceTypeEnum.Passport}");
                    return PartialView($"WritePVs/_{((ServiceTypeEnum)ServiceTypeId).ToString()}", reca);
                case ServiceTypeEnum.Flight:
                    ViewBag.OptionTypeId = base.GetSelectListData("OptionTypeId", "OptionTypeName", "OptionType", $"where ServiceTypeId={(int) ServiceTypeEnum.Flight}");
                    ViewBag.IsInternational = new List<SelectListItem> {
                               new SelectListItem { Text = "International", Value = "true" },
                               new SelectListItem { Text = "Domestic", Value = "false" }
                            };
                    ViewBag.CarType = new List<SelectListItem> {
                               new SelectListItem { Text = "Economy", Value = "1" },
                               new SelectListItem { Text = "Business", Value = "2" }
                            };
                    if (id > 0)
                        reca.Tdate = db.FirstOrDefault<SRdetail>("where ParentID =@0", id)?.Tdate;     //return flight date                   
                    return PartialView($"WritePVs/_{((ServiceTypeEnum)ServiceTypeId).ToString()}", reca);
                case ServiceTypeEnum.Transfer:
                    ViewBag.CarType = Enum.GetValues(typeof(CarTypeEnum)).Cast<CarTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
                    
                    if (reca !=null)
                        ViewBag.Driver = db.SingleOrDefault<Driver>(reca.DriverID)?.DriverName ?? "";
                        
                    List<SelectListItem> RateBasis = new List<SelectListItem>()
                    {
                            new SelectListItem {
                                Text = "8hrs -80Kms", Value = "8hrs -80Kms"
                            },
                            new SelectListItem {
                                Text = "None", Value = "None"
                            }
                        };
                    ViewBag.Heritage = RateBasis;
                    List<SelectListItem> TaxiPayTo = new List<SelectListItem>()
                    {
                            new SelectListItem {
                                Text = "Payed to us", Value = "Payed to us"
                            },
                            new SelectListItem {
                                Text = "Pay to driver", Value = "Pay to driver"
                            }
                        };
                    ViewBag.PayTo = TaxiPayTo;
                    
                    return PartialView($"WritePVs/_{((ServiceTypeEnum)ServiceTypeId).ToString()}", reca);
                case ServiceTypeEnum.Bus:
                    ViewBag.CarType = Enum.GetValues(typeof(BookingTypeEnum)).Cast<BookingTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
                    ViewBag.OptionTypeId = base.GetSelectListData("OptionTypeId", "OptionTypeName", "OptionType", $"where ServiceTypeId={(int)ServiceTypeEnum.Bus}");
                    return PartialView($"WritePVs/_{((ServiceTypeEnum)ServiceTypeId).ToString()}", reca);
                case ServiceTypeEnum.Rail:
                    ViewBag.CarType = Enum.GetValues(typeof(BookingTypeEnum)).Cast<BookingTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();             
                    return PartialView($"WritePVs/_{((ServiceTypeEnum)ServiceTypeId).ToString()}", reca);
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
            Dictionary<string, string> Transfer = new Dictionary<string, string>()
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
                { (ServiceTypeEnum.Transfer).ToString(), Transfer}
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
        public bool RemindAt(int SRID, DateTime remindAtDt, string IgnoreReason)
        {
            try
            {
                int res = db.Execute("Update ServiceRequest set RemindAt=@0,SRStatusId=@1, EmpId=@2,IgnoreReason=@3 where SRID=@4", remindAtDt, (int)SRStatusEnum.Reminder, User.Identity.GetUserId(), IgnoreReason, SRID);
                
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
        [EAAuthorize(FunctionName = "EditComm", Writable = true)]
        public PartialViewResult _EditComm(int? SRID)
        {
            ViewBag.SRID = SRID;
           return PartialView();
        }

        [EAAuthorize(FunctionName = "EditComm", Writable = true)]
        public bool EditComm(int? SRDID, decimal? ECommision)
        {
            try
            {
                int res = db.Execute("Update SRdetails set ECommision=@0 where SRDID=@1",ECommision,SRDID);
                
                LogAction(new SRlog { SRID = db.Single<SRdetail>(SRDID).SRID, SRDID=SRDID, Event = "Commission Edited to "+ ECommision });
                return true;

            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        public void CancelService(int SRDID)
        {
            var killrec = base.BaseCreateEdit<SRdetail>(SRDID,"SRDID");
            killrec.IsCanceled = !killrec.IsCanceled; //Toggle the cancellation of service

            var serv = db.Single<SRdetail>(SRDID);
            var servType = serv.ServiceTypeName;
            LogAction(new SRlog { SRID = serv.SRID, SRDID=SRDID, Event = serv.ServiceTypeName + " was canceled" });

            base.BaseSave<SRdetail>(killrec, true);
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
