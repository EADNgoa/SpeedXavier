using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Speedbird.Models
{
    public class BKPayVM
    {
        public string PaxName { get; set; }
        public int? DriverID { get; set; }
        public string DriverName { get; set; }
        public int? SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string AgentId { get; set; }
        public string ContactName { get; set; }
        public int SRDID { get; set; }
        public string BookingNo { get; set; }
        public DateTime Fdate { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Cost { get; set; }
        public string PayTo { get; set; }
        public int PaymentId { get; set; }
        public bool IsSelected { get; set; }
    }
}