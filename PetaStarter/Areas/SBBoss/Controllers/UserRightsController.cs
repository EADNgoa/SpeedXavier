using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class UserRightsController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "User Rights", Writable = false)]
        public ActionResult AddUserGroups(int? page)
        {
         

            ViewBag.UserName = db.Fetch<Group>("Select Id,UserName from AspNetUsers");
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
            List<Speedbird.Areas.SBBoss.Models.ExistingUserViewModel> recs = db.Fetch<Speedbird.Areas.SBBoss.Models.ExistingUserViewModel>("Select * from AspNetUsers anu inner join UserGroups ug on anu.Id = ug.UserID inner Join Groups g on g.GroupID = ug.GroupID Where ug.GroupID = @0", GID ?? 0);
            ViewBag.func = db.Fetch< Speedbird.Areas.SBBoss.Models.ExistingUserViewModel>("Select * from UserFunctions uf inner join FunctionGroups fg on uf.FunctionID =fg.FunctionID inner join Groups g on g.GroupID = fg.GroupID where fg.GroupID=@0;",GID ?? 0);
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
            List<Speedbird.Areas.SBBoss.Models.ExistingUserViewModel> recs = db.Fetch<Speedbird.Areas.SBBoss.Models.ExistingUserViewModel>("Select * from AspNetUsers anu inner join UserGroups ug on anu.Id = ug.UserID inner Join Groups g on g.GroupID = ug.GroupID Where ug.GroupID = @0", (int)GID);
            ViewBag.func = db.Fetch<Speedbird.Areas.SBBoss.Models.ExistingUserViewModel>("Select * from UserFunctions uf inner join FunctionGroups fg on uf.FunctionID =fg.FunctionID inner join Groups g on g.GroupID = fg.GroupID where fg.GroupID=@0", (int)GID);
            return PartialView("ExistingUsersPartial", recs);
        }

        [EAAuthorize(FunctionName = "User Rights", Writable = false)]
        public ActionResult AddFuncGroups(int? page)
        {
            ViewBag.GroupID = db.Fetch<Group>("Select GroupID,GroupName from Groups");
            ViewBag.Func = db.Fetch<Speedbird.Areas.SBBoss.Models.ExistingFuncViewModel>("Select FunctionID,FunctionName,Module from UserFunctions ORDER BY Module,FunctionName ASC");
            return View();
                    
        }
        [EAAuthorize(FunctionName = "User Rights", Writable = true)]
        public ActionResult ExistingFuncRec(int? GID, FormCollection fm,string sub)
        {
            try
            {
                if (fm["G"] != null && fm["F"] != null)
                {
                    int fid = int.Parse(fm["F"]);
                    int gid = int.Parse(fm["G"]);
                    var item = new FunctionGroup() { FunctionID = fid, GroupID = (int)gid};
                    if(sub == "Read")
                    {
                        item.Writable = false;
                    }
                    else if (sub == "Write")
                    {
                        item.Writable = true;
                    }
                    db.Insert(item);
                    GID = gid;
                }

                if (fm["GroupID"] != null) GID = int.Parse(fm["GroupID"]);
            }
            catch(Exception e)
            {
                Console.WriteLine("Please Select The Group: '{0}'", e);

            }
            List<Speedbird.Areas.SBBoss.Models.ExistingFuncViewModel> recs = db.Fetch<Speedbird.Areas.SBBoss.Models.ExistingFuncViewModel>("Select * from UserFunctions uf inner join FunctionGroups fg on uf.FunctionID = fg.FunctionID inner join Groups g on g.GroupID = fg.GroupID where fg.GroupID = @0", GID ?? 0);
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
            List<Speedbird.Areas.SBBoss.Models.ExistingFuncViewModel> recs = db.Fetch<Speedbird.Areas.SBBoss.Models.ExistingFuncViewModel>("Select * from UserFunctions uf inner join FunctionGroups fg on uf.FunctionID = fg.FunctionID inner join Groups g on g.GroupID = fg.GroupID where fg.GroupID = @0", (int)GID);
            return PartialView("ExistingFuncPartial", recs);
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
