using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace DeMonte.Controllers
{
    [HandleError]
    public class EAController : Controller
    {
        // GET: EA
            protected Repository db;
            public EAController()
            {
                this.db = new Repository();
            }

            /// <summary>
            /// Used to access db for Index page
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="page">Set to 1 if filter present else just pass through</param>
            /// <param name="FieldList">Select clause fields</param>
            /// <param name="TableWithWhere">from, join, where and group</param>
            /// <returns></returns>
            protected IPagedList<T> BaseIndex<T>(int? page, string FieldList, string TableWithWhere)
            {
                var res = db.Query<T>($"Select {FieldList} from {TableWithWhere}");
                //var res= FieldList?.Length>0 ? db.Query<T>($"Select {FieldList} from {TableWithWhere}") : db.Query<T>($"Select * from {TableWithWhere}");

                //int pageSize = db.Fetch<int>("Select top 1 RowsPerPage from Config").FirstOrDefault();
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                return res.ToPagedList(pageNumber, pageSize);
            }

            //Overload to support simple views that have no joins
            protected IPagedList<T> BaseIndex<T>(int? page, string TableWithWhere)
            {
                return BaseIndex<T>(page, "*", TableWithWhere);
            }

            protected T BaseCreateEdit<T>(int? id, string IDname)
            {
                T a;
                if (id.HasValue) //is edit
                {
                    a = db.SingleOrDefault<T>($"where {IDname} = @0", id);
                    return a;
                }
                return default(T);
            }

            protected ActionResult BaseSave<T>(T ObjToSave, bool isExisting)
            {
                if (ModelState.IsValid)
                {
                    var r = (isExisting) ? db.Update(ObjToSave) : db.Insert(ObjToSave);
                    return RedirectToAction("Index");
                }

                return View(ObjToSave);
            }


            // GET: EA
            protected override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                //if (DateTime.Now.Date > DateTime.Parse("15 Aug 2017"))
                //{                
                //    filterContext.Result = new RedirectResult("~/Home/pli");
                //    return;
                //}

            }

            protected override void OnException(ExceptionContext filterContext)
            {
                if (!filterContext.ExceptionHandled)
                {
                    string controller = filterContext.RouteData.Values["controller"].ToString();
                    string action = filterContext.RouteData.Values["action"].ToString();
                    Exception ex = filterContext.Exception;

                    filterContext.Result = new ViewResult
                    {
                        ViewName = "Error",
                        ViewData = new ViewDataDictionary(new HandleErrorInfo(ex, controller, action))
                    };
                    filterContext.ExceptionHandled = true;
                }
            }
        }
    }
