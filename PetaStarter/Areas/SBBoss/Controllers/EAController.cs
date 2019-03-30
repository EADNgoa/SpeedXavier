using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Speedbird.Areas.SBBoss.Controllers
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
                if (id.HasValue && id>0) //is edit
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
                } else
                {
                    MyExtensions.logModelStateErrors(ModelState,db,ControllerContext);
                    return View(ObjToSave);
                }
                
            }

            protected ActionResult BaseSave<T>(T ObjToSave, bool isExisting, object routeValues)
            {
                return BaseSave<T>(ObjToSave, isExisting, "Index", routeValues);
            }

            protected ActionResult BaseSave<T>(T ObjToSave, bool isExisting, string RAction, object routeValues)
            {
                if (ModelState.IsValid)
                {
                    var r = (isExisting) ? db.Update(ObjToSave) : db.Insert(ObjToSave);
                    return RedirectToAction(RAction, routeValues);
                }
                else
                {
                    MyExtensions.logModelStateErrors(ModelState, db, ControllerContext);
                    return View(ObjToSave);
                }
            }
        /// <summary>
        /// Deletes the old image (if any) and replaces with new, else just saves the new image
        /// </summary>
        /// <param name="oldImageName">sql to fetch a string of the existing image path</param>
        /// <param name="imgType">Used in prefixing img name. E.g. GeoTree, Acc, Pkg...</param>
        /// <param name="itemId">Parent records primary key</param>
        /// <param name="UploadedFile">HttpPostedFileBase</param>
        /// <returns></returns>
        protected string SaveImage(PetaPoco.Sql oldImageName, string imgType, int itemId, System.Web.HttpPostedFileBase UploadedFile)
        {
            string fn = "";
            string oldImg = (itemId>0)?db.Single<string>(oldImageName):"";

            if (UploadedFile != null)
            {
                //First remove the old img (if exists)
                if (oldImg?.Length > 0) System.IO.File.Delete(System.IO.Path.Combine(Server.MapPath("~/Images"), oldImg));

                //Save the new file
                fn = UploadedFile.FileName.Substring(UploadedFile.FileName.LastIndexOf('\\') + 1);
                fn = String.Concat(imgType, "_", itemId.ToString(), "_", fn);
                string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                UploadedFile.SaveAs(SavePath);
            }
            return (fn.Length>0)?fn:oldImg;
        }

        internal JsonResult GetAutoCompleteData(string idField, string nameField, string table, string whereClause)
        {
            var filteredItems = db.Fetch<EASelectListData>($"Select {idField} as id, {nameField} as value from {table} {whereClause}");
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
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
                var errorTime = DateTime.Now;

                string MessageStack = ex.Message;
                while (ex.InnerException != null)
                {
                    MessageStack += " > " + ex.InnerException.Message;
                    ex = ex.InnerException;
                }

               db.Insert(new InsectLog { CritterTime = errorTime, Controller = controller, Action = action, Message = MessageStack, Stack = ex.StackTrace });
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
