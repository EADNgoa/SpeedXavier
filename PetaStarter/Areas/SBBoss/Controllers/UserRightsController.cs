
using PagedList;
using Speedbird.Areas.SBBoss.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class UserRightsController : EAController
    {
  

        [EAAuthorize(FunctionName = "User Rights", Writable = false)]
        public ActionResult AddUserGroups()
        {            
            ViewBag.Grp = db.Fetch<Group>("Select * from Groups");
            return View();
        }
        [EAAuthorize(FunctionName = "User Rights", Writable = true)]
        public ActionResult ExistingUserRec(int? GID, FormCollection fm)
        {
            try
            {
                if (fm["Id"] != null && fm["Group"] != null)
                {

                    string uid = fm["Id"];
                    var gid = int.Parse(fm["Group"]);
                   
                    var item = new UserGroup { GroupID = (int)gid, UserID = uid };
                    db.Insert(item);
                }
             
            }
            catch(Exception e)
            {
                Console.WriteLine("Please Select The Group: '{0}'", e);
                

            }
            List<ExistingUserViewModel> recs = db.Fetch<ExistingUserViewModel>("Select * from AspNetUsers anu inner join UserGroups ug on anu.Id = ug.UserID inner Join Groups g on g.GroupID = ug.GroupID Where ug.GroupID = @0", GID ?? 0);
            ViewBag.func = db.Fetch<ExistingUserViewModel>("Select * from UserFunctions uf inner join FunctionGroups fg on uf.FunctionID =fg.FunctionID inner join Groups g on g.GroupID = fg.GroupID where fg.GroupID=@0;",GID ?? 0);
            return PartialView("ExistingUsersPartial", recs);
        }
        public ActionResult DelUserRec(int? GID, FormCollection fm)
        {          
             if ( fm["GroupID"] != null && fm["Id"] != null)
            {
              string uid = fm["Id"];
              var gid = int.Parse(fm["GroupID"]);
              var DeleteRec = db.Execute("Delete from UserGroups Where UserID=@0 and GroupID=@1 ", uid, gid);
                GID = gid;
            }
            List<ExistingUserViewModel> recs = db.Fetch<ExistingUserViewModel>("Select * from AspNetUsers anu inner join UserGroups ug on anu.Id = ug.UserID inner Join Groups g on g.GroupID = ug.GroupID Where ug.GroupID = @0", (int)GID);
            ViewBag.func = db.Fetch<ExistingUserViewModel>("Select * from UserFunctions uf inner join FunctionGroups fg on uf.FunctionID =fg.FunctionID inner join Groups g on g.GroupID = fg.GroupID where fg.GroupID=@0", (int)GID);
            return PartialView("ExistingUsersPartial", recs);
        }

        [EAAuthorize(FunctionName = "User Rights", Writable = false)]
        public ActionResult AddFuncGroups(string err)
        {            
            //ViewBag.Func = db.Fetch<ExistingFuncViewModel>("Select FunctionID,FunctionName,Module from UserFunctions ORDER BY Module,FunctionName ASC");
            ViewBag.errMsg = err;
            return View();
                    
        }

        [EAAuthorize(FunctionName = "User Rights", Writable = false)]
        public ActionResult GetAllOtherFns(int GID)
        {         
            var res = db.Fetch<ExistingFuncViewModel>("Select FunctionID,FunctionName,Module from UserFunctions where FunctionID not in (select FunctionID from FunctionGroups where GroupID = @0) ORDER BY Module,FunctionName ASC",GID);
            
            return PartialView("AvailableFnsPartial", res);

        }
        [EAAuthorize(FunctionName = "User Rights", Writable = true)]
        public ActionResult ExistingFuncRec(int GID, int FID,string sub)
        {
            try
            {
                if (GID >0 && FID >0)
                {                    
                    var item = new FunctionGroup() { FunctionID = FID, GroupID = GID};
                    if(sub == "Read")
                    {
                        item.Writable = false;
                    }
                    if (sub == "Write")
                    {
                        item.Writable = true;
                    }
                    db.Insert(item);                    
                }
            }
            catch(Exception e)
            {                
                return RedirectToAction("AddFuncGroups", new { err= $"Please Select The Group: '{e}'" } );
            }

            return new EmptyResult();
        }

        [EAAuthorize(FunctionName = "User Rights", Writable = true)]
        public ActionResult GetExistingFns(int GID)
        {
            List<ExistingFuncViewModel> recs = db.Fetch<ExistingFuncViewModel>("Select * from UserFunctions uf inner join FunctionGroups fg on uf.FunctionID = fg.FunctionID inner join Groups g on g.GroupID = fg.GroupID where fg.GroupID = @0", GID);
            return PartialView("ExistingFuncPartial", recs);
        }

        [EAAuthorize(FunctionName = "User Rights", Writable = true)]
        public ActionResult DelFuncRec(int? GID, FormCollection fm)
        {
            if (fm["GroupID"] != null && fm["FunctionID"] != null)
            {
                string fid = fm["FunctionID"];
                var gid = int.Parse(fm["GroupID"]);
                var DeleteRec = db.Execute("Delete from FunctionGroups Where FunctionID=@0 and GroupID=@1 ", fid, gid);
                GID = gid;
            }

            return GetExistingFns(GID.Value);
        }

        public ActionResult AutoCompleteGroups(string term)
        {
            var filteredItems = db.Fetch<Group>($"Select * from Groups Where GroupName like '%{term}%'").Select(c => new { id = c.GroupID, value = c.GroupName });
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AutoCompleteUsers(string term)
        {
            var filteredItems = db.Fetch<AspNetUser>($"Select * from AspNetUsers Where UserName like '%{term}%'").Select(c => new { id = c.Id, value = c.UserName });
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
