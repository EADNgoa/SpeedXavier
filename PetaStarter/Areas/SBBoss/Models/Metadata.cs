using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speedbird
{
  public class SRdetailsMetadata
    {
        [StringLength(50,MinimumLength =3)]
        public string Airline;
        [StringLength(50)]
       
        public string FlightNo;
        [StringLength(50,MinimumLength = 6)]
        public string PNRno;
    }




}