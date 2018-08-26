﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
//using System.Web.Mvc; for dynamic roles
//using Microsoft.AspNet.Identity;for dynamic roles
//using Speedbird.Models;for dynamic roles
//using Microsoft.AspNet.Identity.EntityFramework;for dynamic roles

namespace Speedbird
{
    public class PCdetailDets
    {
        public int PCDID { get; set; }
        public int SupplierID { get; set; }

        public string PettyCashID { get; set; }
        public string Category { get; set; }
        public string Details { get; set; }
        public decimal Cost { get; set; }
        public string SupplierName { get; set; }
        public string InvoiceNo { get; set; }
        public string BillImage { get; set; }
        public int SRID { get; set; }

    }
    public class RPDetails
    {
        public int DRPDID { get; set; }

        public int RPDID { get; set; }
        public int Type { get; set; }
        public bool IsPayment { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public decimal Amount { get; set; }
        public decimal UnUsedAmt { get; set; }


    }
    public class SRBooking
    {
        public int SRID { get; set; }
        public int BookingNo { get; set; }
        public int SRDID { get; set; }

        public string cName { get; set; }
        public string UserName { get; set; }
        public decimal PaidAmt { get; set; }
        public decimal OrigAmt { get; set; }
        public int SupplierID { get; set; }
        public DateTime Cdate { get; set; }
        public decimal OA { get; set; }
        public decimal Remainder { get; set; }


    }
    public class PaxDets
    {
        public int SRID { get; set; }
        public int SRDID { get; set; }
        public int CustomerID { get; set; }

        public int Qty { get; set; }
        public string UserName { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Cost { get; set; }
        public decimal PSell { get; set; }
        public decimal PCost { get; set; }


    }
    public class DriverDets
    {
        public int DriverID { get; set; }
        public string Phone { get; set; }
        public string GeoName { get; set; }
        public string DriverName { get; set; }
        public string CarModel { get; set; }
        public int Score { get; set; }
        public int DBCount { get; set; }



    }
    public class BankAccountDets
    {
        public int BankAccountID { get; set; }
        public DateTime TDate { get; set; }
        public decimal? AmountIn { get; set; }
        public decimal? AmountOut { get; set; }
        public int SRID { get; set; }
        public int BookingNo { get; set; }
        public string TransNo { get; set; }
        public string UserID { get; set; }
        public string SupplierName { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
    }


    public class OwnCarTripDets
    {
        public int OwnCarTripID { get; set; }
        public int CarBikeID { get; set; }
        public int DriverID { get; set; }
        public DateTime TripStart { get; set; }
        public int StartKms { get; set; }
        public int EndKms { get; set; }
        public string CarBikeName { get; set; }
        public string DriverName { get; set; }


    }
    public class UserLogRecDets
    {
        public int UserLogID { get; set; }
        public string UserID { get; set; }
        public DateTime LogIn { get; set; }
        public DateTime LogOut { get; set; }
        public string UserName { get; set; }
    }

    [MetadataType(typeof(SRdetailsMetadata))]
    public partial class SRdetail
    {
        public string tstr { get { return $"{Tdate:dd MMM yyyy hh:mm}"; } }
        public string fstr { get { return $"{Fdate:dd MMM yyyy hh:mm}"; } }
        public string ServiceTypeName { get { return ((ServiceTypeEnum)ServiceTypeID).ToString(); } }


    }
    public partial class SRdetailDets
    {
        public int SRDID { get; set; }
        public int SRID { get; set; }
        public int BookingNo { get; set; }
        public int ServiceTypeID { get; set; }
        public string ServiceTypeName { get { return ((ServiceTypeEnum)ServiceTypeID).ToString(); } }
        public int CarType { get; set; }
        public string CouponCode { get; set; }
        public string Airline { get; set; }
        public string Model { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public DateTime Fdate { get; set; }
        public string fstr { get { return $"{Fdate:dd-MMM-yyyy}"; } }
        public DateTime Tdate { get; set; }
        public string tstr { get { return $"{Tdate:dd MMM yyyy hh:mm}"; } }        
        public int SupplierID { get; set; }
        public decimal Cost { get; set; }
        public decimal SellPrice { get; set; }
        public decimal ECommision { get; set; }

        public string PNRno { get; set; }
        public string TicketNo { get; set; }
        public int ChildNo { get; set; }
        public int AdultNo { get; set; }
        public int InfantNo { get; set; }
        public string RoomType { get; set; }
        public string City { get; set; }
        public string Heritage { get; set; }
        public bool HasAC { get; set; }
        public bool HasCarrier { get; set; }
        public int GuideLanguageID { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string ContractNo { get; set; }
        public string GuideLanguageName { get; set; }
        public string SupplierName { get; set; }
        public decimal Tax { get; set; }
        public decimal Commision { get; set; }
        public string PercComm { get; set; }

        public decimal Total { get; set; }




    }
    public partial class CustomerQuery
    {
        public string Instr { get { return $"{CheckIn:dd MMM yyyy hh:mm}"; } }
        public string Outstr { get { return $"{CheckOut:dd MMM yyyy hh:mm}"; } }
        public string Tostr { get { return $"{Tdate:dd MMM yyyy}"; } }
        public string CustName { get { return FName + ' ' + SName;  } }        
        public string ServiceTypeName { get { return ((ServiceTypeEnum)ServiceTypeID).ToString();  } }


    }
    public  class LeaveEntitlementDets
    {
        public int LeaveEntitlementID { get; set; }
        public int LeaveYear { get; set; }
        public int LeaveDays { get; set; }
        public string LeaveTypeName { get; set; }
    }

    public class LeaveBalanceRpt
    {
        public string LeaveTypeName { get; set; }
        public decimal TotalLeave { get; set; }
        public decimal Availed { get; set; }
        public decimal Remaining { get; set; }
    }
    public  class LeaveApplicationDets
    {
        public int LeaveApplicationID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string UserID { get; set; }
        public string LeaveTypeName { get; set; }
        public DateTime LeaveStartDate { get; set; }
        public decimal NoOfDays { get; set; }
        public int StatusID { get; set; }
    }

    [MetadataType(typeof(DiscountCouponMetadata))]
    public partial class DiscountCoupon
    {
    }

    [MetadataType(typeof(PriceMetadata))]
    public partial class Price
    {
    }

    public class FacilityDets
    {
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public string AccomodationID { get; set; }
    }
    public class PictureDets
    {
       public int PictureID { get; set; }
       public int ServiceID { get; set; }
       public int ServiceTypeID { get; set; }
       public string PictureName { get; set; }
       public HttpPostedFileBase UploadedFile { get; set; }
    }


    public class PriceDets
    {
        public int PriceID { get; set; }
        public int OptionTypeID { get; set; }
        public string OptionTypeName { get; set; }
        public int ServiceTypeID { get; set; }
        public int ServiceID { get; set; }
        public DateTime WEF { get; set; }
        public decimal Price { get; set;}
        public decimal WeekendPrice { get; set; }
    }
    public class PackageDets
    {
        public int PackageID { get; set; }
        public int ServiceTypeID { get; set; }
        public int CategoryID { get; set; }

        public string PackageName { get; set; }
        public string Description { get; set; }
        public string CouponCode { get; set; }

        public String GeoName { get; set; }
        public IEnumerable<String> GeoNames { get; set; }

        public int Duration { get; set; }
        public string Itinerary { get; set; }
        public int Dificulty { get; set; }
        public int GroupSize { get; set; }
        public IEnumerable<GuideLanguage> GuideLanguages { get; set; }
        public string StartTime { get; set; }
        public string Inclusion { get; set; }
        public string Exclusion { get; set; }
        public string Highlights { get; set; }
        public string MeetAndInfo { get; set; }

        public int ServiceID { get; set; }
        public string PictureName { get; set; }
        public string AttractionName { get; set; }
        public string ActivityName { get; set; }
        public string CategoryName { get; set; }
        public decimal price { get; set; }

        public IEnumerable<PictureDets> Pic { get; set; }
        public IEnumerable<GuideLanguage> Glang { get; set; }
        public IEnumerable<string> Gtime { get; set; }

        public IEnumerable<CategoryDets>Cat { get; set; }
        public IEnumerable<PriceDets>Pric { get; set; }
        public IEnumerable<ActivityDets> Act { get; set; }
        public string SupplierNames { get; set; }
        public string SupplierContractNos { get; set; }

        public DateTime EndDate { get; set; }
        public string EndDateStr { get { return $"{EndDate:dd MMM yyyy}"; } }
        public int Daysleft { get { return (EndDate - DateTime.Now).Days; } }
    }

    [MetadataType(typeof(PackageMetadata))]
    public partial class Package
    {
    }
 
    [MetadataType(typeof(AccomodationMetadata))]
    public partial class Accomodation
    {
    }

    [MetadataType(typeof(CarBikeMetadata))]
    public partial class CarBike
    {
    }


    [MetadataType(typeof(AssetBillMetadatta))]
    public partial class OwnAssetBill
    {
    }

    [MetadataType(typeof(BankAccountMetadata))]
    public partial class BankAccount
    {
        public int BookingNo { get; set; }
    }
    public class ActivityDets
    {
        public int PackageID { get; set; }
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }


    }
    public class LanguageDets
    {
        public int PackageID { get; set; }
        public int GuideLanguageID { get; set; }
        public string GuideLanguageName { get; set; }


    }
    public class AttractionDets
    {
        public int PackageID { get; set; }
        public int AttractionID { get; set; }
        public string AttractionName { get; set; }


    }

    public class CategoryDets
    {
        public int PackageID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }


    }
    public class CarBikeDets
    {
        public int CarBikeID { get; set; }
        public int GeoTreeID { get; set; }
        public string GeoName { get; set; }
        public string GeoTreeName { get; set; }
                public string CarBikeName { get; set; }
        public decimal price { get; set; }

        public string CouponCode { get; set; }
        public string Description { get; set; }
        public int NoPax { get; set; }
        public int NoSmallBags { get; set; }
        public int NoLargeBags { get; set; }
        public bool HasAc { get; set; }
        public bool IsBike { get; set; }
        public bool SelfOwned { get; set; }

        public bool HasCarrier { get; set; }
        public bool InclHelmet { get; set; }
        public int ServiceID { get; set; }
        public int ServiceTypeID { get; set; }

        public string PictureName { get; set; }
        public IEnumerable<PriceDets> Pric { get; set; }
        public IEnumerable<PictureDets> pic { get; set; }


    }
    public class VisaDets
    {
        public int VisaID { get; set; }
        public string VisaCountry { get; set; }
        public string FlagPicture { get; set; }
        public string EmbassyAddress { get; set; }
        public string Details { get; set; }

        public HttpPostedFileBase UploadedFile1 { get; set; }
        public HttpPostedFileBase UploadedFile2 { get; set; }

    }
    public class ReviewRepDets
    {
        public int ReviewReplyID { get; set; }
        public int ReviewID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Reply { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool IsVisible { get; set; }


    }
    public class BookingDets
    {
        public int BookingID { get; set; }
        public int BookingDetailID { get; set; }
        public int ServiceID { get; set; }
        public int ServiceTypeID { get; set; }
        public int OptionTypeID { get; set; }
        public string OptionTypeName { get; set; }
        public int Qty { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NoOfGuests { get; set; }
        public decimal Price { get; set; }
        public string BlockedReason { get; set; }
      

    }
    public class CustomerDets
    {
        public int CustomerID { get; set; }
        public int BookingID { get; set; }
        public string FName { get; set; }
        public string SName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string IdPicture { get; set; }
        public string Type { get; set; }

    }

    public class AgentDiscDets
    {
        public int AgentDiscountID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public int ServiceTypeID { get; set; }
        public decimal Amount { get; set; }
        public decimal Percantage { get; set; }
        public bool IsApproved { get; set; }
    }

    public class BookingRec
    {
        public int BookingStatusID { get; set; }
        public string BookingStatusName { get; set; }
        public int BookingID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public DateTime BookDate { get; set; }
        public int StatusID { get; set; }
        public IEnumerable<BookingDets> bookdets { get; set; }
        public  IEnumerable<CustomerDets> Customer { get; set; }
    }
    public class CategoryRec
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }      
        public IEnumerable<PackageDets> pack { get; set; }
    }
    public class ActivityRec
    {
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public IEnumerable<PackageDets> pack { get; set; }
    }
    public class AttractRec
    {
        public int AttractionID { get; set; }
        public string AttractionName { get; set; }
        public IEnumerable<PackageDets> pack { get; set; }
    }

    public class AccomodationDets
    {
        public int AccomodationID { get; set; }
        public string AccomName { get; set; }
        public string Description { get; set; }
        public int GeoTreeID { get; set; }
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }

        public string GeoName { get; set; }
        public string Lat { get; set; }
        public string longt { get; set; }
        public int ServiceID { get; set; }
        public decimal price { get; set; }
        public bool SelfOwned { get; set; }

        public string PictureName { get; set; }
        public IEnumerable<PriceDets> Pric { get; set; }
        public IEnumerable<PictureDets> pic { get; set; }

    }

    public class AccomPackCarBike
    {
        public int ServiceID { get; set; }
        public int ServiceTypeID { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string ServiceGeoName { get; set; }
        public string ServicePic { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<Attribute> Attributes { get; set; }
        public IEnumerable<string> Icons { get; set; }

    }

    public class CartDets
    {
        public int CartID { get; set; }
        public int ServiceID { get; set; }
        public string Id { get; set; }
        public int ServiceTypeID { get; set; }
        public int OptionTypeID { get; set; }

        public int Qty { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NoOfGuest { get; set; }
        public decimal OrigPrice { get; set; }
        public string Pic { get; set; }
        public string ServiceName { get; set; }
        public string CouponCode { get; set; }
        public IEnumerable<CustomerDets> cust { get; set; }
        public IEnumerable<LanguageDets> lang { get; set; }


    }
    public class ReviewDets
    {
        public int ReviewID { get; set; }
        public int ServiceID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public int ServiceTypeID { get; set; }
        public DateTime ReviewDate { get; set; }
        public string Review { get; set; }
        public bool IsVisible { get; set; }
        public IEnumerable<ReviewRepDets> Replies { get; set; }
    }


    public class ServiceRequestDets
    {
        public int SRID { get; set; }
        public int BookingNo { get; set; }
        public int CustomerID { get; set; }
        public int SRStatusID { get; set; }
        public int PayStatusID { get; set; }
        public int ServiceTypeID { get; set; }

        public string Status { get { return ((SRStatusEnum)SRStatusID).ToString(); } }
        public DateTime TDate { get; set; }

        public string DT { get { return $"{TDate:dd MMM yyyy HH:MM}"; } }
        public decimal? TotSA { get; set; }
        public decimal? AccAmt { get; set; }

        public string EmpID { get; set; }
        public string AgentID { get; set; }
        public string FName { get; set; }
        public string SName { get; set; }
        public string CName { get; set; }
        
        public string Phone { get; set; }
        public string Email { get; set; }
        public string AgentName { get; set; }
        public string UserName { get; set; }
        public string Src { get { return ((EnquirySourceEnum)EnquirySource).ToString(); } }

        public int EnquirySource { get; set; }
        public string Request { get; set; }

    }
    public class SRuploadDets
    {
        public int SRUID { get; set; }
        public int SRID { get; set; }
        public int Path { get; set; }
        public string UploadName { get; set; }
        public HttpPostedFileBase UploadedFile { get; set; }
    }
    public class SRlogsDets
    {
        public int SRLID { get; set; }
        public int SRDID { get; set; }
        public DateTime LogDateTime { get; set; }
        public string UserName { get; set; }
        public bool Type { get; set; }
        public string Event { get; set; }

    }

    public class Queue
    {
        public int ServiceTypeId { get; set; }
        public int Count { get; set; }
    }

    public enum ServiceTypeEnum
    {
        Accomodation,
        Packages,
        Cruise,
        SightSeeing,
        CarBike,
        Insurance,
        Flight,
        Visa,
        TaxiHire
    }

    public enum MealPlanEnum
    {
        None,
        Breakfast,
        Lunch,
        Tea,
        Dinner
    }

    public enum UserTypeEnum
    {
        Admin,
        Agent,
        Guest
    }
    public enum BookingTypeEnum
    {
        Agent,
        Coporate,
        Online,
        Pax

    }

    public enum EnquirySourceEnum
    {
        Web,
        Walk_In,
        Word_Of_Mouth,
        Print_Media,
        Agent
    }
    public enum SRStatusEnum
    {
        New,
        Reminder,
        NoAction,
        Unconfirmed,    
        Confirmed,
        Completed,
        Cancelled
      


    }
    public enum PayModeEnum
    {
        Cash,
        CC,
        Internet
    }
  
    public enum SSTypeEnum
    {
        Private,
        General
    }
    public enum CarTypeEnum
    {
        Small,
        Medium,
        Large
    }
    public enum LeaveApplicationStatusEnum
    {
        Pending,
        Approved,
        Rejected
    }
    public enum PayType
    {
        Not_Paid,
        Part_Paid,
        Full_Paid,
        Cancelled

    }
    public enum AmtType
    {
        Cash,
        Cheque,
        Internet_Banking,
        Card
    }

    public class EAAuthorizeAttribute : AuthorizeAttribute
    {
        public string FunctionName { get; set; }
        public bool Writable { get; set; }
        private Repository rep;


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            rep = new Repository();
            return MyExtensions.IsPermitted(rep, FunctionName, Writable, httpContext.User.Identity.GetUserId());
        }
    }

    //[MetadataType(typeof(ConfigMetadata))]
    //public partial class Config
    //{
    //}

    //[MetadataType(typeof(EmpTypeMetadata))]
    //public partial class EmpTypes
    //{
    //}

    //[MetadataType(typeof(EDocTypeMetadata))]
    //public partial class EDocTypes
    //{
    //}

    //[MetadataType(typeof(AllowanceTypeMetadata))]
    //public partial class AllowanceTypes
    //{
    //}

    //[MetadataType(typeof(EmployeesMetadata))]
    //public partial class Employees
    //{
    //}

    //[MetadataType(typeof(EmpDocsMetadata))]
    //public partial class EmpDocs
    //{
    //}

    //[MetadataType(typeof(EmploymentHistoryMetadata))]
    //public partial class EmploymentHistory
    //{
    //}
    //[MetadataType(typeof(LoansMetadata))]
    //public partial class Loans
    //{
    //}
    //[MetadataType(typeof(LoanSkipMetadata))]
    //public partial class LoanSkip
    //{
    //}

    //[MetadataType(typeof(AllowanceMetadata))]
    //public partial class Allowance
    //{
    //}

    //[MetadataType(typeof(AdvanceMetadata))]
    //public partial class Advance
    //{
    //}

    //[MetadataType(typeof(BonusMetadata))]
    //public partial class Bonus
    //{        
    //}

    //[MetadataType(typeof(WagesMetadata))]
    //public partial class Wages
    //{
    //}

    //[MetadataType(typeof(PayrollMetadata))]
    //public partial class Payroll
    //{
    //}

    //public class DailyAllowEvent
    //{
    //    public string title { get; set; }
    //    public string start { get; set; }
    //    public bool allDay { get; set; }
    //}


    ///// <summary>
    ///// Used to Create a new Employee with an auto created Join date
    ///// </summary>
    //public class NewEmp
    //{
    //    public Employees emp { get; set; }

    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
    //    [Display(Name = "Join Date")]        
    //    [Required]
    //    public DateTime JoinDate { get; set; }

    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
    //    [Display(Name = "Drivers Lic. Expiry Date")]
    //    [RequiredIf(CompareToInt =1)] //EmpTypeID 1 has to be driver
    //    public Nullable<System.DateTime> DrivLicExp { get; set; }

    //}

    ///// <summary>
    ///// Used instead of the DailyAllowance class in order to allow null dates
    ///// </summary>
    //public class DA
    //{
    //    public int EmpID { get; set; }
    //    public string EmpName { get; set; }
    //    public Nullable<System.DateTime> SaveTime { get; set; }
    //    public Nullable<System.DateTime> AllowDate { get; set; }
    //}

    //    /// <summary>
    //    /// As the name suggests> Done only after payroll freeze
    //    /// </summary>
    //    public class AdjustPay
    //{
    //    public int EmpID { get; set; }
    //    public string EmpName { get; set; }
    //    public int GenMonth { get; set; }
    //    public int GenYear { get; set; }
    //    public decimal AdjAmt { get; set; }
    //    public string AdjRemark { get; set; }
    //}


    ///// <summary>
    ///// Export sal of reg emps to excel
    ///// </summary>
    //public class RegExport
    //{
    //    public string AccountNo { get; set; }
    //    public char c { get; set; } = 'c';
    //    public decimal Amount { get; set; }
    //    public string Narration { get; set; }
    //}

    ///// <summary>
    ///// Export sal of reg emps to excel
    ///// </summary>
    //public class NonRegExport
    //{
    //    public string ForMonth { get; set; }
    //    public string Name { get; set; }
    //    public string AccountNo { get; set; }
    //    public string IFSC { get; set; }
    //    public int Ten { get; set; } = 10;
    //    public decimal Amount { get; set; }
    //    public string ExportDate { get; set; }
    //}

    ///// /// <summary>
    ///// Employee Card
    ///// </summary>
    //public class EmployeeCard
    //{
    //    public Employees emp { get; set; }
    //    public IEnumerable<EmploymentHistory> emphist { get; set; }
    //    public IEnumerable<Wages> wages { get; set; }
    //    public IEnumerable<EmpDocs> empdocs { get; set; }
    //    public EmpLoanHistory elh { get; set; }
    //}

    ///// <summary>
    ///// Employee Loan History Report
    ///// </summary>
    //public class EmpLoanHistory
    //{
    //    public IEnumerable<Loans> Ln { get; set; }
    //    public IEnumerable<GetLoanHistory_Result> Lhist { get; set; }

    //}

    ///// <summary>
    ///// Used to upload Employee Documents with image
    ///// </summary>
    //public class EmpDocsImg
    //{        
    //    public int EDID { get; set; }
    //    public int EmpID { get; set; }
    //    public int EDocTypeID { get; set; }
    //    public HttpPostedFileBase UploadedFile { get; set; }

    //    [DataType(DataType.Date)]
    //    [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
    //    [Display(Name = "Expiry Date")]
    //    [CheckDateRange(AddMonths =1 , ForceMore =true) ]
    //    public Nullable<System.DateTime> ExpiryDate { get; set; }
    //}

    ///// <summary>
    ///// For marking Leave
    ///// </summary>
    //public class LeaveDays
    //{
    //    public int EmpID { get; set; }
    //    public DateTime Dayt { get; set; }

    //    public bool IsLeave { get; set; }
    //}

    ///// <summary>
    ///// Date limiter
    ///// </summary>
    //public class CheckDateRangeAttribute : ValidationAttribute
    //{
    //    public int AddMonths { get; set; }
    //    public int AddDays { get; set; }
    //    public bool ForceMore { get; set; }

    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        if (value == null)
    //        {
    //            return ValidationResult.Success;
    //        }

    //        string comparer;

    //        DateTime CompareDate = DateTime.UtcNow.Date;
    //        CompareDate= CompareDate.AddMonths(AddMonths);
    //        CompareDate = CompareDate.AddDays(AddDays);

    //        DateTime dt = (DateTime)value;

    //        if (ForceMore)
    //        {
    //            if (dt >= CompareDate)
    //                return ValidationResult.Success;
    //            comparer = " greater than or equal to ";
    //        }else
    //        {
    //            if (dt <= CompareDate)
    //                return ValidationResult.Success;
    //            comparer = " less than or equal to ";
    //        }

    //        return new ValidationResult(ErrorMessage ?? "Make sure your date is " + comparer + CompareDate.ToString("dd-MMM-yyyy"));
    //    }

    //}

    //public class RequiredIfAttribute : ValidationAttribute
    //{
    //    public int CompareToInt { get; set; }


    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        NewEmp ne = (NewEmp) validationContext.ObjectInstance;

    //        if (ne.emp.EmpTypeID == CompareToInt)
    //        {
    //            if (value != null && (value.GetType() == typeof(DateTime)) )
    //                return ValidationResult.Success;
    //        }
    //        else
    //            return ValidationResult.Success;

    //        return new ValidationResult("Drivers have to have a licence with a future expiry date");
    //    }

    //}
    ///// <summary>
    ///// Custom authorization attribute for setting per-method accessibility 
    ///// </summary>
    ////[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    ////public class SetPermissionsAttribute : AuthorizeAttribute
    ////{
    ////    /// <summary>
    ////    /// The name of each action that must be permissible for this method, separated by a comma.
    ////    /// </summary>
    ////    public string Permissions { get; set; }

    ////    protected override bool AuthorizeCore(HttpContextBase httpContext)
    ////    {
    ////        NTHRPayEntities1 db = new NTHRPayEntities1();            
    ////        UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
    ////        ApplicationDbContext dbu = new ApplicationDbContext();

    ////        bool isUserAuthorized = base.AuthorizeCore(httpContext);

    ////        string[] permissions = Permissions.Split(',').ToArray();

    ////        IEnumerable<string> perms = permissions.Intersect(db.Permissions.Select(p => p.ActionName));
    ////        List<IdentityRole> roles = new List<IdentityRole>();

    ////        if (perms.Count() > 0)
    ////        {
    ////            foreach (var item in perms)
    ////            {
    ////                var currentUserId = httpContext.User.Identity.GetUserId();
    ////                var relatedPermisssionRole = dbu.Roles.Find(db.Permissions.Single(p => p.ActionName == item).RoleId).Name;
    ////                if (userManager.IsInRole(currentUserId, relatedPermisssionRole))
    ////                {
    ////                    return true;
    ////                }
    ////            }
    ////        }
    ////        return false;
    ////    }
    ////}
}