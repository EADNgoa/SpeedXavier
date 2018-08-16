using Speedbird.Controllers;
using System;
//using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
//using System.Web;
using System.Web.Mvc;


namespace Speedbird.Areas.SBBoss.Controllers
{
    public class AssetBillController : EAController
    {
        // GET: Clients
        [EAAuthorize(FunctionName = "Asset Bill", Writable = false)]
        public ActionResult Index(int? page ,string AN )
        {
            if (AN?.Length > 0) page = 1;
            return View("Index", base.BaseIndex<OwnAssetBill>(page, " * ","OwnAssetBill Where BillDate like '%" + AN + "%'"));
        }



        // GET: Clients/Create
        [EAAuthorize(FunctionName = "Asset Bill", Writable = true)]
        public ActionResult Manage(int? id)
        {
            // ViewBag.UnitID = new SelectList(db.Fetch<Unit>("Select UnitID,UnitName from Units"), "UnitID", "UnitName");

            ViewBag.ServiceTypeID = Enum.GetValues(typeof(ServiceTypeEnum)).Cast<ServiceTypeEnum>().Where(a => a.ToString() == ServiceTypeEnum.Accomodation.ToString() || a.ToString() == ServiceTypeEnum.CarBike.ToString()).Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() }).ToList();
       

            return View(base.BaseCreateEdit<OwnAssetBill>(id, "OwnAssetBillID"));
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EAAuthorize(FunctionName = "Asset Bill", Writable = true)]
        public ActionResult Manage([Bind(Include = "OwnAssetBillID,ServiceTypeID,ServiceID,BillDate,BillNo,Amount,BillImage")] OwnAssetBill item, System.Web.HttpPostedFileBase UploadedFile)
        {
            item.BillImage = SaveImage(new PetaPoco.Sql("Select BillImage from OwnAssetBill where OwnAssetBillID=@0", item.OwnAssetBillID), "OwnAssetBill", item.OwnAssetBillID, UploadedFile);
            return base.BaseSave<OwnAssetBill>(item, item.OwnAssetBillID > 0);
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
