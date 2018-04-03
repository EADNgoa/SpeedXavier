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

      
    }
}