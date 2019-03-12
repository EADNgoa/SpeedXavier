using System;
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
    
    public partial class AgentView
    {
        public string AgentId { get; set; }
        public string RealName { get; set; }
        public string ContactName { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string PAN { get; set; }
        public string GST { get; set; }
        public string RCbook { get; set; }
        public string BkAccNo { get; set; }
        public string BkName { get; set; }
        public string BkIFSC { get; set; }
        public string BkAddress { get; set; }
    }
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
        public string tstr { get { return $"{Tdate:dd MMM yyyy HH:mm}"; } }
        public string fstr { get { return $"{Fdate:dd MMM yyyy HH:mm}"; } }
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
        public string tstr { get { return $"{Tdate:dd MMM yyyy HH:mm}"; } }        
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
        public bool IsCanceled { get; set; }

    }
    public partial class CustomerQuery
    {
        public string Instr { get { return $"{CheckIn:dd MMM yyyy HH:mm}"; } }
        public string Outstr { get { return $"{CheckOut:dd MMM yyyy HH:mm}"; } }
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
        public bool IsLead { get; set; }

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

        public string DT { get { return $"{TDate:dd MMM yy HH:mm}"; } }
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
        public string IgnoreReason { get; set; }

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

   
}