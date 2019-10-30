using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Speedbird.Models
{
    public class PaymentViewModel
    {
        public int PaymentID { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? TDate { get; set; }
        public decimal? Amount { get; set; }
        public string TransactionNo { get; set; }
        public string Note { get; set; }
        public int? Type { get; set; }
        public int? DriverID { get; set; }
        public int? BankID { get; set; }
        public string AgentId { get; set; }
        public int? SupplierID { get; set; }
        public int SRID { get; set; }
        public int? CustomerID { get; set; }
        public List<BKPayVM> LeftBkPayVm { get; set; }
        public List<BKPayVM> RightBkPayVm { get; set; }
        public List<int> SelectedSRDID_Left { get; set; }
        public List<int> SelectedSRDID_Right { get; set; }
    }
}