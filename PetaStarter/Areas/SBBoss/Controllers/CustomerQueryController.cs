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
    public class CustomerQueryController : EAController
    {
        [EAAuthorize(FunctionName = "Customer Query", Writable = false)]

        public ActionResult Index(int? page, string AN)
        {

           

         ViewBag.Title = "Customer Queries"; 
        

            return View();
        }

        [HttpPost]
        [EAAuthorize(FunctionName = "Customer Query", Writable = false)]

        public JsonResult GetCustomerQueries(DTParameters parameters)
        {
            var columnSearch = parameters.Columns.Select(s => s.Search.Value).Take(CustomerColumns.Count()).ToList();

            //XMLPath uses nested queries so to avoid that we construct these 4 filters ourselves



            var sql = new PetaPoco.Sql($"Select distinct c.CustomerQueryID,ServiceId, ServiceName, Email, Phone, Substring(Query,1,100) + '...' as Query, FName, SName, CheckIn, Tdate, ServiceTypeId, NoPax, Qty from CustomerQuery c");
            var fromsql = new PetaPoco.Sql();
            var wheresql = new PetaPoco.Sql("where c.CustomerQueryID >0");

         
          
            wheresql.Append($"{GetWhereWithOrClauseFromColumns(CustomerColumns, columnSearch)} Order By CustomerQueryID Desc ");
            sql.Append(fromsql);
            sql.Append(wheresql);

            try
            {
                var res = db.Query<CustomerQuery>(sql).Skip(parameters.Start).Take(parameters.Length).ToList();


                var dataTableResult = new DTResult<CustomerQuery>
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
        [EAAuthorize(FunctionName = "Customer Query", Writable = false)]

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

        private string[] CustomerColumns => new string[]
        {
            "CustomerQueryID",
            "Tostr",
            "CustName",            
            "ServiceName",
            "ServiceTypeName",
            "Email",
            "Phone",
            "Query",       
            "Instr",            
            "NoPax",
            "Qty",
        };




        [EAAuthorize(FunctionName = "Customer Query", Writable = true)]

        public ActionResult Reply(int? id, int? ServiceID, ServiceTypeEnum? st, int? EID)
        {
            // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");
            ViewBag.cqid = id;
            if (st != null)
            {
                ViewBag.sid = (int)st;
             }
            ViewBag.ServiceID = ServiceID;
            ViewBag.Dets = db.FirstOrDefault<CustomerQuery>("Select * from CustomerQuery Where CustomerQueryID=@0",id);
            ViewBag.Chat = db.Fetch<CustQueryReply>($"Select * from CustQueryReply Where CustomerQueryID = {id} Order By CustomerQueryID Desc");
            return View(base.BaseCreateEdit<CustQueryReply>(EID, "CustQueryReplyID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Customer Query", Writable = true)]

        public ActionResult Reply([Bind(Include = "CustQueryReplyID,CustomerQueryID,ReplyDate,Reply")] CustQueryReply item,ServiceTypeEnum? str,int? ServiceIDr)
        {
            item.ReplyDate = DateTime.Now;
            base.BaseSave<CustQueryReply>(item, item.CustQueryReplyID > 0);
            return RedirectToAction("Reply", new { id = item.CustomerQueryID,st= str,ServiceID=ServiceIDr});
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
