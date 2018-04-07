using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speedbird.Controllers
{
    public class CartController : EAController
    {
        public ActionResult Cart()
        {
         
            return View();
        }
        public ActionResult ThankYou()
        {

            return View();
        }
        public ActionResult SearchCoupon(string dc,int? tot)
        {
            try
            {
                var IfApplic = db.FirstOrDefault<Accomodation>("Select CouponCode from Accomodation where CouponCode=@0", dc);
                if (IfApplic != null)
                {
                    var CheckVal = db.FirstOrDefault<DiscountCoupon>("Select * from DiscountCoupon where CouponCode=@0 and ValidFrom <= @1 and ValidTo >=@2", dc, DateTime.Now, DateTime.Now);
                    return PartialView(CheckVal);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
            return PartialView();

        }

    }
}