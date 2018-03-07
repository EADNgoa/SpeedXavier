using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetaPoco;
//using System.Web.Mvc; for dynamic roles
//using Microsoft.AspNet.Identity;for dynamic roles
//using Cavala.Models;for dynamic roles
//using Microsoft.AspNet.Identity.EntityFramework;for dynamic roles

namespace Cavala.Models
{
 

    public class ExistingFuncViewModel
    {
        public int FunctionID { get; set; }
        public string FunctionName { get; set; }
        public string Module { get; set; }
        public bool Writable { get; set; }

        public int GroupID { get; set; }
        public string GroupName { get; set; }

    }


}