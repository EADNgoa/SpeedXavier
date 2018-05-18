using Microsoft.AspNet.Identity;
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
using static PetaStarter.Areas.SBBoss.Models.DataTablesModels;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class SRController : EAController
    {
        public ActionResult Index(int? page, string AN)
        {         
         ViewBag.Title = "Service Requests"; 
         return View();
        }

        [HttpPost]
        public JsonResult GetSRList(DTParameters parameters)
        {
            var columnSearch = parameters.Columns.Select(s => s.Search.Value).Take(SRColumns.Count()).ToList();

            //XMLPath uses nested queries so to avoid that we construct these 4 filters ourselves
            string CustName = "";
            string AgentName = "";

            if (columnSearch[2]?.Length > 0) { CustName = columnSearch[2]; columnSearch[2] = null; }
            if (columnSearch[2]?.Length > 0) { AgentName = columnSearch[2]; columnSearch[2] = null; }



            var sql = new PetaPoco.Sql($"Select * from ServiceRequest s inner join Customer c on c.CustomerID=s.CustID");
            var fromsql = new PetaPoco.Sql();
            var wheresql = new PetaPoco.Sql("where s.SRID > 0 ");

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
                    r.FName = db.FirstOrDefault<string>("Select FName from Customer where CustomerID=@0",r.CustomerID);
                    r.SName = db.FirstOrDefault<string>("Select SName from Customer where CustomerID=@0", r.CustomerID);

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
            "FName",
            "SName",
            "Email",
            "Phone",
            "SRStatusID",
            "EnquirySouce",
        };




        public ActionResult Manage(int? id, int mode = 1, int EID = 0) //Mode 1=Details,2=Prices,3=images,4=validity
        {
        
            ViewBag.mode = mode;
            ViewBag.EID = EID;
            ViewBag.SRID = id;

            return View();
        }

        public ActionResult FetchDetails(int? id)
        {
            var SR = base.BaseCreateEdit<ServiceRequest>(id, "SRID");
            ViewBag.SRStatusID = Enum.GetValues(typeof(SRStatusEnum)).Cast<SRStatusEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();

            ViewBag.Cust = db.FirstOrDefault<Customer>("Select FName,SName from Customer where CustomerID=@0", SR?.CustID ?? 0);

            return PartialView("Details", SR);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public bool Manage([Bind(Include = "SRID,CustID,SRStatusID,EmpID,EnquirySource,AgentID")] ServiceRequest item)
        {
            using (var transaction = db.GetTransaction())
            {
                if (ModelState.IsValid)
                {
                    item.EmpID = User.Identity.GetUserId();
                    var r = (item.SRID > 0) ? db.Update(item) : db.Insert(item);
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

        public ActionResult SRdetails(int? id, int? EID)
        {
            ViewBag.SRID = id;
            ViewBag.SRs = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest sr inner join Customer c on c.CustomerID=sr.CustID Where SRID=@0",id);
            ViewBag.SRDets = db.Fetch<SRdetail>("Select * from SRdetails where SRID =@0",id);
            ViewBag.ServiceTypeID = Enum.GetValues(typeof(ServiceTypeEnum)).Cast<ServiceTypeEnum>().Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();

            return PartialView(base.BaseCreateEdit<SRdetail>(EID, "SRDID"));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SRdetails([Bind(Include = "SRDID,SRID,ServiceTypeID,FromLoc,ToLoc,Fdate,Tdate,SupplierID,Cost,SellPrice,PNRno,TicketNo")] SRdetail item)
        {
            using (var transaction = db.GetTransaction())
            {

                try
                {                   
                    base.BaseSave<SRdetail>(item, item.SRDID > 0);
                    db.Insert(new SRlog { SRDID = item.SRDID,LogDateTime=DateTime.Now,UserID=User.Identity.GetUserId(),Type=true,Event=null});
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    db.AbortTransaction();
                    throw ex;
                }
            }
            return RedirectToAction("Manage", new { id = item.SRID, mode = 2 });
        }


        public ActionResult SRUpload(int? id)
        {

            ViewBag.SRs = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest sr inner join Customer c on c.CustomerID=sr.CustID Where SRID=@0", id);
            ViewBag.Pics = db.Fetch<SRUpload>("Select * From SRUploads where SRID=@0",id);
            base.BaseCreateEdit<SRUpload>(id, "SRUID");

            SRuploadDets ci = new SRuploadDets() { };
            return PartialView(ci);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SRUpload([Bind(Include = "SRUID,SRID,Path,UploadName,UploadedFile")] SRuploadDets item)
        {
            SRUpload res = new SRUpload
            {
              SRID=item.SRID,
              SRUID=item.SRUID,
             UploadName=item.UploadName           
            };

            if (item.UploadedFile != null)
            {
                string fn = item.UploadedFile.FileName.Substring(item.UploadedFile.FileName.LastIndexOf('\\') + 1);
                fn = item.UploadName + "_" + fn;

                string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                item.UploadedFile.SaveAs(SavePath);

                res.Path = fn;
            }

            return base.BaseSave<SRUpload>(res, item.SRUID > 0, "Manage", new { id = item.SRID, mode = 3 });

        }

        public ActionResult Reciepts(int? id, int? sid, int? EID)
        {
            var rec = base.BaseCreateEdit<SRReciept>(EID, "RecieptID");
            ViewBag.SRID = id;
            ViewBag.BankID = db.Query<Bank>("Select * from Banks", rec?.BankID ?? 0).Select(sl => new SelectListItem { Text = sl.BankName, Value = sl.BankID.ToString(), Selected = true });
            ViewBag.SRs = db.FirstOrDefault<ServiceRequestDets>("Select * From ServiceRequest sr inner join Customer c on c.CustomerID=sr.CustID Where SRID=@0", id);
            ViewBag.Reciepts = db.Fetch<SRReciept>($"Select * From SRReciepts where SRID ='{id}'");
            return PartialView(rec);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reciepts([Bind(Include = "RecieptID,SRID,RecieptDate,Amount,PayMode")] SRReciept item)
        {
            base.BaseSave<SRReciept>(item, item.RecieptID > 0);
            return RedirectToAction("Manage", new { id = item.SRID, mode =4 });

        }

        public ActionResult SRLogs(int? page)
        {
            page = 1;
            return View("SRlogs", base.BaseIndex<SRlogsDets>(page, " * ", "SRlogs Where SRLID >0"));
        }

        public ActionResult Delete(int? id, int? pid)
        {
          
            if (pid != null)
            {
                //First remove the old img (if exists)
                string oldImg = db.Single<string>("Select Path from SRUploads where SRUID=@0", pid);
                if (oldImg?.Length > 0) System.IO.File.Delete(System.IO.Path.Combine(Server.MapPath("~/Images"), oldImg));

                db.Execute($"Delete From SRUploads Where SRUID={pid}");
                return RedirectToAction("Manage", routeValues: new { id, mode = 3 });


            }
            return RedirectToAction("Manage");
        }
        public ActionResult AutoCompleteCust(string term)
        {
            var filteredItems = db.Fetch<Customer>($"Select * from Customer Where FName like '%{term}%'").Select(c => new { id = c.CustomerID, value = c.FName+" "+c.SName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AutoCompleteSup(string term)
        {
            var filteredItems = db.Fetch<Supplier>($"Select * from Supplier Where SupplierName like '%{term}%'").Select(c => new { id = c.SupplierID, value = c.SupplierName});
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
