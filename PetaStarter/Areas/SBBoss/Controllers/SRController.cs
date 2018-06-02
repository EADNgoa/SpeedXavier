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

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class SRController : EAController
    {
        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public ActionResult Index(int? page, string AN)
        {
            ViewBag.Title = "Service Requests";
            return View();
        }

        [HttpPost]
        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public JsonResult GetSRList(DTParameters parameters)
        {
            var columnSearch = parameters.Columns.Select(s => s.Search.Value).Take(SRColumns.Count()).ToList();

            //XMLPath uses nested queries so to avoid that we construct these 4 filters ourselves
            string CustName = "";
            string AgentName = "";

            if (columnSearch[2]?.Length > 0) { CustName = columnSearch[2]; columnSearch[2] = null; }
            if (columnSearch[2]?.Length > 0) { AgentName = columnSearch[2]; columnSearch[2] = null; }



            var sql = new PetaPoco.Sql($"Select * from ServiceRequest s");
            var fromsql = new PetaPoco.Sql();
            var wheresql = new PetaPoco.Sql();

            if (CustName.Length > 0)
            {
                fromsql.Append(", Customer c");
                wheresql.Append($"and  c.CustomerID=s.CustomerID and CustomerName like '%{CustName}%'");
            }

            wheresql.Append($"{GetWhereWithOrClauseFromColumns(SRColumns, columnSearch)}");
            sql.Append(fromsql);
            sql.Append(wheresql);

            try
            {
                var res = db.Query<ServiceRequestDets>(sql).Skip(parameters.Start).Take(parameters.Length).ToList();

                res.ForEach(r =>
                {
                    r.FName = db.ExecuteScalar<string>("Select FName from Customer c inner join SR_Cust sc on c.CustomerID=sc.CustomerID inner join ServiceRequest sr on sr.SRID = sc.ServiceRequestID Where sc.ServiceRequestID = @0", r.SRID);
                    r.SName = db.ExecuteScalar<string>("Select SName from Customer c inner join SR_Cust sc on c.CustomerID=sc.CustomerID inner join ServiceRequest sr on sr.SRID = sc.ServiceRequestID Where sc.ServiceRequestID = @0", r.SRID);
                    r.Phone = db.ExecuteScalar<string>("Select Phone from Customer c inner join SR_Cust sc on c.CustomerID=sc.CustomerID inner join ServiceRequest sr on sr.SRID = sc.ServiceRequestID Where sc.ServiceRequestID = @0", r.SRID);
                    r.Email = db.ExecuteScalar<string>("Select Email from Customer c inner join SR_Cust sc on c.CustomerID=sc.CustomerID inner join ServiceRequest sr on sr.SRID = sc.ServiceRequestID Where sc.ServiceRequestID = @0", r.SRID);

                    r.AgentName = db.FirstOrDefault<string>("Select UserName from AspNetUsers where Id=@0", r.AgentID);

                });


                var dataTableResult = new DTResult<ServiceRequestDets>
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
        [HttpPost]
        [EAAuthorize(FunctionName = "Service Requests", Writable = false)]
        public JsonResult GetSRDetList(DTParameters parameters, int? id)
        {
            var columnSearch = parameters.Columns.Select(s => s.Search.Value).Take(SRDetColumns.Count()).ToList();

            //XMLPath uses nested queries so to avoid that we construct these 4 filters ourselves



            var sql = new PetaPoco.Sql($"Select * from SRdetails where SRID=@0", id);
            var fromsql = new PetaPoco.Sql();
            var wheresql = new PetaPoco.Sql();


            wheresql.Append($"{GetWhereWithOrClauseFromColumns(SRDetColumns, columnSearch)}");
            sql.Append(fromsql);
            sql.Append(wheresql);

            try
            {
                var res = db.Query<SRdetail>(sql).Skip(parameters.Start).Take(parameters.Length).ToList();


                var dataTableResult = new DTResult<SRdetail>
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
                            subQuery = subQuery.Remove(subQuery.LastIndexOf("and"), 2).Insert(subQuery.LastIndexOf("and"), "");
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
            "SRID",
            "DT",
            "FName",
            "SName",
            "Email",
            "Phone",
            "Status",
            "AgentName",
            "Src",
        };
        private string[] SRDetColumns => new string[]
       {
            "SRID",
            "ServiceTypeName",
            "FromLoc",
            "ToLoc",
            "tstr",
            "fstr",
            "Supplier",
            "Cost",
            "SellPrice"
       };


        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult Manage(int? id, int mode = 1, int EID = 0) //Mode 1=Details,2=Prices,3=images,4=validity
        {

            ViewBag.mode = mode;
          
          
            ViewBag.EID = EID;
            ViewBag.SRID = id;

            return View();
        }
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
            ViewBag.Title = "Service Request";

            var SR = base.BaseCreateEdit<ServiceRequest>(id, "SRID");
            if (id > 0)
            {
                ViewBag.AgentName = db.ExecuteScalar<string>("Select UserName From AspNetUsers where Id = @0 ", SR.AgentID);
                ViewBag.BookingTypeID = Enum.GetValues(typeof(BookingTypeEnum)).Cast<BookingTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            }
            ViewBag.SRStatusID = Enum.GetValues(typeof(SRStatusEnum)).Cast<SRStatusEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            ViewBag.EnquirySource = Enum.GetValues(typeof(EnquirySourceEnum)).Cast<EnquirySourceEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
            ViewBag.ServiceTypeID = Enum.GetValues(typeof(ServiceTypeEnum)).Cast<ServiceTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();

            ViewBag.Cust = db.FirstOrDefault<Customer>("Select FName,SName from Customer where CustomerID=@0", SR?.CustID ?? 0);

            return PartialView("Details", SR);
        }
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult ExistingCustRec(string fn,string sn,string ph,string em)
        {

            var recs = db.Query<CustomerDets>("Select * from Customer").ToList(); 
            if (fn != null)
            {
                recs = recs.Where(c => Regex.IsMatch(c.FName,fn,RegexOptions.IgnoreCase)).ToList();
            }
            if (sn != "")
            {
                recs = recs.Where(c => Regex.IsMatch(c.SName, sn,RegexOptions.IgnoreCase)).ToList();
            }
            if (ph != "")
            {
                recs = recs.Where(c => Regex.IsMatch(c.Phone, ph, RegexOptions.IgnoreCase)).ToList();
            }
            if (em != "")
            {
                recs = recs.Where(c => Regex.IsMatch(c.Email, em,RegexOptions.IgnoreCase)).ToList();
            }

            return PartialView("CustomerSearchPartial", recs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public bool Manage([Bind(Include = "SRID,CustID,SRStatusID,EmpID,BookingTypeID,EnquirySource,AgentID,TDate")] ServiceRequest item, string Event, int? CID, string FName, string SName, string Email, string Phone)
        {
            using (var transaction = db.GetTransaction())
            {
                if (ModelState.IsValid)
                {
                   item.EmpID = User.Identity.GetUserId();
                    item.TDate = DateTime.Now;
                    var r = (item.SRID > 0) ? db.Update(item) : db.Insert(item);
                    if (CID != null)
                    {
                        db.Insert(new SR_Cust { ServiceRequestID = item.SRID, CustomerID = (int)CID });
                    }
                    else if (FName != null && Phone != null && Email != null)
                    {
                        var cust = new Customer { FName = FName, SName = SName, Phone = Phone, Email = Email };
                        db.Insert(cust);
                        db.Insert(new SR_Cust { ServiceRequestID = item.SRID, CustomerID = cust.CustomerID });

                    }
                    if (item.SRID > 0)
                    {
                        Event = "User has Edited the Field";
                    }
                    db.Insert(new SRlog { SRID = item.SRID, LogDateTime = DateTime.Now, UserID = User.Identity.GetUserId(), Type = true, Event = Event });
                    transaction.Complete();
                    return true;
                }
                else
                {
                    transaction.Dispose();
                    return false;

                }
            }

        }
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRdetails(int? id, int? EID)
        {
            ViewBag.SRID = id;
            ViewBag.Title = "Manage Services";
            ViewBag.SRs = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest sr inner join Customer c on c.CustomerID=sr.CustID Where SRID=@0", id);
            ViewBag.SRDets = db.Fetch<SRdetail>("Select * from SRdetails where SRID =@0", id);
            ViewBag.ServiceTypeID = Enum.GetValues(typeof(ServiceTypeEnum)).Cast<ServiceTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();

            return PartialView(base.BaseCreateEdit<SRdetail>(EID, "SRDID"));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRdetails([Bind(Include = "SRDID,SRID,ServiceTypeID,FromLoc,ToLoc,Fdate,Tdate,SupplierID,Cost,SellPrice,PNRno,TicketNo")] SRdetail item)
        {
            using (var transaction = db.GetTransaction())
            {

                try
                {
                    string Action = "";
                    if (item.SRDID > 0)
                    {
                         Action = "This record has been Edited";
                    }
                    else
                    {
                        Action = $"Added {(ServiceTypeEnum)item.ServiceTypeID} Service";
                    }
                    base.BaseSave<SRdetail>(item, item.SRDID > 0);
                    var r= db.Insert(new SRlog { SRDID = item.SRDID, LogDateTime = DateTime.Now, UserID = User.Identity.GetUserId(), Type = true, Event = Action,SRID=item.SRID });
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

            return base.BaseSave<SRUpload>(res, item.SRUID > 0, "Manage", new { id = item.SRID, mode = 5 });

        }
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRCustomers(int? id, int? sid, int? EID)
        {
            var rec = base.BaseCreateEdit<Customer>(EID, "CustomerID");
            ViewBag.SRID = id;
            ViewBag.Title = "Manage Customers";
            ViewBag.Custs = db.Fetch<Customer>($"Select * From Customer c inner join SR_Cust sc on sc.CustomerID = c.CustomerID inner join ServiceRequest sr on sr.SRID = sc.ServiceRequestID where sc.ServiceRequestID ='{id}'");
            return PartialView(rec);
        }

        [HttpPost]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRCustomers(string FName, string SName, string Email, string Phone, int? CID, int? SRID, string UploadName, HttpPostedFileBase UploadedFile)
        {
            if (CID != null)
            {
                db.Insert(new SR_Cust { ServiceRequestID = (int)SRID, CustomerID = (int)CID });
            }
            else if (FName != null && Email != null && SName != null && Phone != null)
            {

                var cust = new Customer { FName = FName, SName = SName, Phone = Phone, Email = Email };
                db.Insert(cust);
                db.Insert(new SR_Cust { ServiceRequestID = (int)SRID, CustomerID = cust.CustomerID });
            }

            if (UploadedFile != null && UploadName != null)
            {
                string fn = UploadedFile.FileName.Substring(UploadedFile.FileName.LastIndexOf('\\') + 1);
                fn = UploadName + "_" + fn;

                string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                UploadedFile.SaveAs(SavePath);

                db.Insert(new SRUpload { UploadName = UploadName, Path = fn, SRID = SRID });
            }
            return RedirectToAction("Manage", new { id = (int)SRID, mode = 4 });

        }
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult Reciepts(int? id, int? sid, int? EID)
        {
            var rec = base.BaseCreateEdit<SRReciept>(EID, "RecieptID");
            ViewBag.SRID = id;
            ViewBag.Title = "Reciepts";
            ViewBag.PayMode = Enum.GetValues(typeof(PayModeEnum)).Cast<PayModeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();

            ViewBag.BankID = db.Query<Bank>("Select * from Banks", rec?.BankID ?? 0).Select(sl => new SelectListItem { Text = sl.BankName, Value = sl.BankID.ToString(), Selected = true });
            ViewBag.SRs = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest Where SRID=@0", id);
            ViewBag.Reciepts = db.Fetch<SRReciept>($"Select * From SRReciepts where SRID ='{id}'");
            return PartialView(rec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult Reciepts([Bind(Include = "RecieptID,SRID,RecieptDate,Amount,PayMode")] SRReciept item)
        {
            base.BaseSave<SRReciept>(item, item.RecieptID > 0);
            return RedirectToAction("Manage", new { id = item.SRID, mode = 6 });

        }

        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRLogs(int? id, int? sid, int? EID)
        {
            var rec = base.BaseCreateEdit<SRReciept>(EID, "RecieptID");
            ViewBag.SRID = id;
            ViewBag.Title = "Service Reauest Logs";

            ViewBag.SRs = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest Where SRID=@0", id);
            ViewBag.SRLogDets = db.Fetch<SRlogsDets>($"Select * From SRLogs sl inner join AspNetUsers anu on sl.UserID = anu.Id where SRID ='{id}'");
            return PartialView(rec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Service Requests", Writable = true)]
        public ActionResult SRLogs([Bind(Include = "SRLID,SRID,SRDID,LogDateTime,UserID,Type,Event,SRID")] SRlog item)
        {
            item.LogDateTime = DateTime.Now;
            item.UserID = User.Identity.GetUserId();
            base.BaseSave<SRlog>(item, item.SRLID > 0);
            return RedirectToAction("Manage", new { id = item.SRID, mode = 2 });

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
            var filteredItems = db.Fetch<Customer>($"Select * from Customer Where FName like '%{term}%'").Select(c => new { id = c.CustomerID, value = c.FName+" "+c.SName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AutoCompleteAgent(string term)
        {
            var filteredItems = db.Fetch<AspNetUser>($"Select * from AspNetUsers Where UserName like '%{term}%'").Select(c => new { id = c.Id, value = c.UserName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AutoCompleteSup(string term)
        {
            var filteredItems = db.Fetch<Supplier>($"Select * from Supplier Where SupplierName like '%{term}%'").Select(c => new { id = c.SupplierID, value = c.SupplierName});
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FetchSTpartial(int? id, int ServiceTypeId, bool IsReadOnly)
        {
            if (IsReadOnly)
                ViewBag.IsReadOnly = "disabled";
            
            switch ((ServiceTypeEnum)ServiceTypeId)
            {
                case ServiceTypeEnum.Accomodation:                    
                    return PartialView($"_{((ServiceTypeEnum)ServiceTypeId).ToString()}", db.SingleOrDefault<Accomodation>(id));                    
                case ServiceTypeEnum.Packages:                    
                    return PartialView($"_{((ServiceTypeEnum)ServiceTypeId).ToString()}", db.SingleOrDefault<Package>(id));
                //case ServiceTypeEnum.Cruise:
                //    break;
                //case ServiceTypeEnum.SightSeeing:
                //    break;
                //case ServiceTypeEnum.CarBike:
                //    break;
                //case ServiceTypeEnum.Insurance:
                //    break;
                //case ServiceTypeEnum.Visa:
                //    break;
                default:
                    return PartialView("_NotFound");
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
