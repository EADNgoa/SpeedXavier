
// This file was automatically generated by the PetaPoco T4 Template
// Do not make changes directly to this file - edit the template instead
// 
// The following connection settings were used to generate this file
// 
//     Connection String Name: `DefaultConnection`
//     Provider:               `System.Data.SqlClient`
//     Connection String:      `Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=SpeedBirdDB;Integrated Security=True`
//     Schema:                 ``
//     Include Views:          `False`

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace Speedbird
{
	public partial class Repository : Database
	{
		public Repository() 
			: base("DefaultConnection")
		{
			CommonConstruct();
		}

		public Repository(string connectionStringName) 
			: base(connectionStringName)
		{
			CommonConstruct();
		}
		
		partial void CommonConstruct();
		
		public interface IFactory
		{
			Repository GetInstance();
		}
		
		public static IFactory Factory { get; set; }
        public static Repository GetInstance()
        {
			if (_instance!=null)
				return _instance;
				
			if (Factory!=null)
				return Factory.GetInstance();
			else
				return new Repository();
        }

		[ThreadStatic] static Repository _instance;
		
		public override void OnBeginTransaction()
		{
			if (_instance==null)
				_instance=this;
		}
		
		public override void OnEndTransaction()
		{
			if (_instance==this)
				_instance=null;
		}
        
	}
	

    
	[TableName("dbo.__MigrationHistory")]
	[PrimaryKey("MigrationId", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class __MigrationHistory  
    {
		[Column] public string MigrationId { get; set; }
		[Column] public string ContextKey { get; set; }
		[Column] public byte[] Model { get; set; }
		[Column] public string ProductVersion { get; set; }
	}
    
	[TableName("dbo.__RefactorLog")]
	[PrimaryKey("OperationKey", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class __RefactorLog  
    {
		[Column] public Guid OperationKey { get; set; }
	}
    
	[TableName("dbo.Accomodation")]
	[PrimaryKey("AccomodationID")]
	[ExplicitColumns]
    public partial class Accomodation  
    {
		[Column] public int AccomodationID { get; set; }
		[Column] public string AccomName { get; set; }
		[Column] public string Description { get; set; }
		[Column] public int? GeoTreeID { get; set; }
		[Column] public string lat { get; set; }
		[Column] public string longt { get; set; }
		[Column] public string CouponCode { get; set; }
	}
    
	[TableName("dbo.Activity")]
	[PrimaryKey("ActivityID")]
	[ExplicitColumns]
    public partial class Activity  
    {
		[Column] public int ActivityID { get; set; }
		[Column] public string ActivityName { get; set; }
		[Column] public string ImagePath { get; set; }
	}
    
	[TableName("dbo.AgentDiscount")]
	[PrimaryKey("AgentDiscountID")]
	[ExplicitColumns]
    public partial class AgentDiscount  
    {
		[Column] public int AgentDiscountID { get; set; }
		[Column] public string UserID { get; set; }
		[Column] public int? ServiceTypeID { get; set; }
		[Column] public decimal? Amount { get; set; }
		[Column] public decimal? Percentage { get; set; }
		[Column] public bool? IsApproved { get; set; }
	}
    
	[TableName("dbo.AspNetRoles")]
	[PrimaryKey("Id", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class AspNetRole  
    {
		[Column] public string Id { get; set; }
		[Column] public string Name { get; set; }
	}
    
	[TableName("dbo.AspNetUserClaims")]
	[PrimaryKey("Id")]
	[ExplicitColumns]
    public partial class AspNetUserClaim  
    {
		[Column] public int Id { get; set; }
		[Column] public string UserId { get; set; }
		[Column] public string ClaimType { get; set; }
		[Column] public string ClaimValue { get; set; }
	}
    
	[TableName("dbo.AspNetUserLogins")]
	[PrimaryKey("LoginProvider", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class AspNetUserLogin  
    {
		[Column] public string LoginProvider { get; set; }
		[Column] public string ProviderKey { get; set; }
		[Column] public string UserId { get; set; }
	}
    
	[TableName("dbo.AspNetUserRoles")]
	[PrimaryKey("UserId", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class AspNetUserRole  
    {
		[Column] public string UserId { get; set; }
		[Column] public string RoleId { get; set; }
	}
    
	[TableName("dbo.AspNetUsers")]
	[PrimaryKey("Id", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class AspNetUser  
    {
		[Column] public string Id { get; set; }
		[Column] public string Email { get; set; }
		[Column] public bool EmailConfirmed { get; set; }
		[Column] public string PasswordHash { get; set; }
		[Column] public string SecurityStamp { get; set; }
		[Column] public string PhoneNumber { get; set; }
		[Column] public bool PhoneNumberConfirmed { get; set; }
		[Column] public bool TwoFactorEnabled { get; set; }
		[Column] public DateTime? LockoutEndDateUtc { get; set; }
		[Column] public bool LockoutEnabled { get; set; }
		[Column] public int AccessFailedCount { get; set; }
		[Column] public string UserName { get; set; }
		[Column] public DateTime DateCreated { get; set; }
		[Column] public bool Disabled { get; set; }
		[Column] public DateTime? LastLogin { get; set; }
		[Column] public int? UserType { get; set; }
	}
    
	[TableName("dbo.Attraaction")]
	[PrimaryKey("AttractionID")]
	[ExplicitColumns]
    public partial class Attraaction  
    {
		[Column] public int AttractionID { get; set; }
		[Column] public string AttractionName { get; set; }
		[Column] public string ImagePath { get; set; }
		[Column] public string Description { get; set; }
	}
    
	[TableName("dbo.Attribute")]
	[PrimaryKey("AttributeId")]
	[ExplicitColumns]
    public partial class Attribute  
    {
		[Column] public int AttributeId { get; set; }
		[Column] public string AttributeText { get; set; }
		[Column] public string fatext { get; set; }
		[Column] public string Color { get; set; }
	}
    
	[TableName("dbo.BookedCustomer")]
	[PrimaryKey("BCID")]
	[ExplicitColumns]
    public partial class BookedCustomer  
    {
		[Column] public int BCID { get; set; }
		[Column] public int? CartID { get; set; }
		[Column] public int? BookingID { get; set; }
		[Column] public int? CustomerID { get; set; }
	}
    
	[TableName("dbo.Booking")]
	[PrimaryKey("BookingID")]
	[ExplicitColumns]
    public partial class Booking  
    {
		[Column] public int BookingID { get; set; }
		[Column] public string UserID { get; set; }
		[Column] public DateTime? BookDate { get; set; }
		[Column] public int? StatusID { get; set; }
	}
    
	[TableName("dbo.BookingDetail")]
	[PrimaryKey("BookingDetailID")]
	[ExplicitColumns]
    public partial class BookingDetail  
    {
		[Column] public int BookingDetailID { get; set; }
		[Column] public int? BookingID { get; set; }
		[Column] public int? ServiceID { get; set; }
		[Column] public int? ServiceTypeID { get; set; }
		[Column] public int? OptionTypeID { get; set; }
		[Column] public int? Qty { get; set; }
		[Column] public DateTime? CheckIn { get; set; }
		[Column] public DateTime? CheckOut { get; set; }
		[Column] public int? NoOfGuests { get; set; }
		[Column] public decimal? Price { get; set; }
		[Column] public string BlockedReason { get; set; }
	}
    
	[TableName("dbo.BookingStatus")]
	[PrimaryKey("BookingStatusID")]
	[ExplicitColumns]
    public partial class BookingStatus  
    {
		[Column] public int BookingStatusID { get; set; }
		[Column] public string BookingStatusName { get; set; }
	}
    
	[TableName("dbo.CarBike")]
	[PrimaryKey("CarBikeID")]
	[ExplicitColumns]
    public partial class CarBike  
    {
		[Column] public int CarBikeID { get; set; }
		[Column] public string CarBikeName { get; set; }
		[Column] public int GeoTreeId { get; set; }
		[Column] public string Description { get; set; }
		[Column] public int? NoPax { get; set; }
		[Column] public int? NoSmallBags { get; set; }
		[Column] public int? NoLargeBags { get; set; }
		[Column] public bool? HasAc { get; set; }
		[Column] public bool? HasCarrier { get; set; }
		[Column] public bool? InclHelmet { get; set; }
		[Column] public string CouponCode { get; set; }
	}
    
	[TableName("dbo.Cart")]
	[PrimaryKey("CartID")]
	[ExplicitColumns]
    public partial class Cart  
    {
		[Column] public int CartID { get; set; }
		[Column] public string Id { get; set; }
		[Column] public int? ServiceID { get; set; }
		[Column] public int? ServiceTypeID { get; set; }
		[Column] public int? OptionTypeID { get; set; }
		[Column] public int? Qty { get; set; }
		[Column] public DateTime? CheckIn { get; set; }
		[Column] public DateTime? CheckOut { get; set; }
		[Column] public int? NoOfGuest { get; set; }
		[Column] public decimal? OrigPrice { get; set; }
		[Column] public string CouponCode { get; set; }
	}
    
	[TableName("dbo.Category")]
	[PrimaryKey("CategoryID")]
	[ExplicitColumns]
    public partial class Category  
    {
		[Column] public int CategoryID { get; set; }
		[Column] public string CategoryName { get; set; }
		[Column] public string ImagePath { get; set; }
	}
    
	[TableName("dbo.Cruise")]
	[PrimaryKey("CruiseID")]
	[ExplicitColumns]
    public partial class Cruise  
    {
		[Column] public int CruiseID { get; set; }
		[Column] public string CruiseName { get; set; }
		[Column] public string Description { get; set; }
		[Column] public int? Duration { get; set; }
		[Column] public string Itinerary { get; set; }
		[Column] public int? StarRating { get; set; }
	}
    
	[TableName("dbo.Cruise_GeoTree")]
	[PrimaryKey("CruiseID", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class Cruise_GeoTree  
    {
		[Column] public int CruiseID { get; set; }
		[Column] public int GeoTreeID { get; set; }
	}
    
	[TableName("dbo.Customer")]
	[PrimaryKey("CustomerID")]
	[ExplicitColumns]
    public partial class Customer  
    {
		[Column] public int CustomerID { get; set; }
		[Column] public string FName { get; set; }
		[Column] public string SName { get; set; }
		[Column] public string Email { get; set; }
		[Column] public string Phone { get; set; }
		[Column] public string IdPicture { get; set; }
	}
    
	[TableName("dbo.CustomerQuery")]
	[PrimaryKey("CustomerQueryID")]
	[ExplicitColumns]
    public partial class CustomerQuery  
    {
		[Column] public int CustomerQueryID { get; set; }
		[Column] public string FName { get; set; }
		[Column] public string SName { get; set; }
		[Column] public string Email { get; set; }
		[Column] public string Phone { get; set; }
		[Column] public string IdPicture { get; set; }
		[Column("Query")] public string _Query { get; set; }
		[Column] public int? ServiceID { get; set; }
		[Column] public int? ServiceTypeID { get; set; }
		[Column] public DateTime? CheckIn { get; set; }
		[Column] public DateTime? CheckOut { get; set; }
		[Column] public int? NoPax { get; set; }
		[Column] public int? Qty { get; set; }
		[Column] public DateTime? Tdate { get; set; }
	}
    
	[TableName("dbo.CustQueryReply")]
	[PrimaryKey("CustQueryReplyID")]
	[ExplicitColumns]
    public partial class CustQueryReply  
    {
		[Column] public int CustQueryReplyID { get; set; }
		[Column] public int? CustomerQueryID { get; set; }
		[Column] public DateTime? ReplyDate { get; set; }
		[Column] public string Reply { get; set; }
	}
    
	[TableName("dbo.DiscountCoupon")]
	[PrimaryKey("DiscountCouponID")]
	[ExplicitColumns]
    public partial class DiscountCoupon  
    {
		[Column] public int DiscountCouponID { get; set; }
		[Column] public string CouponCode { get; set; }
		[Column] public DateTime? ValidFrom { get; set; }
		[Column] public DateTime? ValidTo { get; set; }
		[Column] public decimal? Amount { get; set; }
		[Column] public decimal? Perc { get; set; }
	}
    
	[TableName("dbo.Facility")]
	[PrimaryKey("FacilityID")]
	[ExplicitColumns]
    public partial class Facility  
    {
		[Column] public int FacilityID { get; set; }
		[Column] public string FacilityName { get; set; }
	}
    
	[TableName("dbo.Facility_Accomodation")]
	[PrimaryKey("FacilityID", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class Facility_Accomodation  
    {
		[Column] public int FacilityID { get; set; }
		[Column] public int AccomodationID { get; set; }
	}
    
	[TableName("dbo.FunctionGroups")]
	[PrimaryKey("FunctionID", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class FunctionGroup  
    {
		[Column] public int FunctionID { get; set; }
		[Column] public int GroupID { get; set; }
		[Column] public bool Writable { get; set; }
	}
    
	[TableName("dbo.GeoTree")]
	[PrimaryKey("GeoTreeID")]
	[ExplicitColumns]
    public partial class GeoTree  
    {
		[Column] public int GeoTreeID { get; set; }
		[Column] public string GeoName { get; set; }
		[Column] public int? GeoParentID { get; set; }
		[Column] public int? GeoLevel { get; set; }
		[Column] public string ImagePath { get; set; }
	}
    
	[TableName("dbo.Groups")]
	[PrimaryKey("GroupID")]
	[ExplicitColumns]
    public partial class Group  
    {
		[Column] public int GroupID { get; set; }
		[Column] public string GroupName { get; set; }
	}
    
	[TableName("dbo.GuideLanguage")]
	[PrimaryKey("GuideLanguageID")]
	[ExplicitColumns]
    public partial class GuideLanguage  
    {
		[Column] public int GuideLanguageID { get; set; }
		[Column] public string GuideLanguageName { get; set; }
	}
    
	[TableName("dbo.Icons")]
	[PrimaryKey("IconId")]
	[ExplicitColumns]
    public partial class Icon  
    {
		[Column] public int IconId { get; set; }
		[Column] public int ServiceId { get; set; }
		[Column] public int ServiceTypeId { get; set; }
		[Column] public string IconPath { get; set; }
	}
    
	[TableName("dbo.LateBreak")]
	[PrimaryKey("LateBreakID", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class LateBreak  
    {
		[Column] public int LateBreakID { get; set; }
	}
    
	[TableName("dbo.MiceDetails")]
	[PrimaryKey("MiceID")]
	[ExplicitColumns]
    public partial class MiceDetail  
    {
		[Column] public int MiceID { get; set; }
		[Column] public string GuestName { get; set; }
		[Column] public DateTime? TDate { get; set; }
		[Column] public string Phone { get; set; }
		[Column] public string Email { get; set; }
		[Column] public string AgentName { get; set; }
		[Column] public string Detail { get; set; }
		[Column] public bool? IsRead { get; set; }
	}
    
	[TableName("dbo.OptionType")]
	[PrimaryKey("OptionTypeID")]
	[ExplicitColumns]
    public partial class OptionType  
    {
		[Column] public int OptionTypeID { get; set; }
		[Column] public string OptionTypeName { get; set; }
		[Column] public int? ServiceTypeID { get; set; }
	}
    
	[TableName("dbo.Package")]
	[PrimaryKey("PackageID")]
	[ExplicitColumns]
    public partial class Package  
    {
		[Column] public int PackageID { get; set; }
		[Column] public int? ServiceTypeID { get; set; }
		[Column] public string PackageName { get; set; }
		[Column] public string Description { get; set; }
		[Column] public int? Duration { get; set; }
		[Column] public string Itinerary { get; set; }
		[Column] public int? Dificulty { get; set; }
		[Column] public int? GroupSize { get; set; }
		[Column] public int? GuideLanguageID { get; set; }
		[Column] public string StartTime { get; set; }
		[Column] public string Inclusion { get; set; }
		[Column] public string Exclusion { get; set; }
		[Column] public string Highlights { get; set; }
		[Column] public string CouponCode { get; set; }
		[Column] public string SupplierNotepad { get; set; }
	}
    
	[TableName("dbo.Package_Activity")]
	[PrimaryKey("PackageID", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class Package_Activity  
    {
		[Column] public int PackageID { get; set; }
		[Column] public int ActivityID { get; set; }
	}
    
	[TableName("dbo.Package_Attraction")]
	[PrimaryKey("PackageID", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class Package_Attraction  
    {
		[Column] public int PackageID { get; set; }
		[Column] public int AttractionID { get; set; }
	}
    
	[TableName("dbo.Package_Attribute")]
	[PrimaryKey("PackageID", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class Package_Attribute  
    {
		[Column] public int PackageID { get; set; }
		[Column] public int AttributeID { get; set; }
		[Column] public int ServiceTypeId { get; set; }
	}
    
	[TableName("dbo.Package_Category")]
	[PrimaryKey("PackageID", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class Package_Category  
    {
		[Column] public int PackageID { get; set; }
		[Column] public int CategoryID { get; set; }
	}
    
	[TableName("dbo.Package_GeoTree")]
	[PrimaryKey("PackageID", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class Package_GeoTree  
    {
		[Column] public int PackageID { get; set; }
		[Column] public int GeoTreeID { get; set; }
	}
    
	[TableName("dbo.Package_Language")]
	[PrimaryKey("PackageId", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class Package_Language  
    {
		[Column] public int PackageId { get; set; }
		[Column] public int GuideLanguageId { get; set; }
	}
    
	[TableName("dbo.Package_Supplier")]
	[PrimaryKey("PackageID", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class Package_Supplier  
    {
		[Column] public int PackageID { get; set; }
		[Column] public int SupplierID { get; set; }
		[Column] public string ContractNo { get; set; }
	}
    
	[TableName("dbo.PackageValidity")]
	[PrimaryKey("PVId")]
	[ExplicitColumns]
    public partial class PackageValidity  
    {
		[Column] public int PVId { get; set; }
		[Column] public int PackageId { get; set; }
		[Column] public DateTime ValidFrom { get; set; }
		[Column] public DateTime ValidTo { get; set; }
	}
    
	[TableName("dbo.Picture")]
	[PrimaryKey("PictureID")]
	[ExplicitColumns]
    public partial class Picture  
    {
		[Column] public int PictureID { get; set; }
		[Column] public int? ServiceTypeID { get; set; }
		[Column] public string PictureName { get; set; }
		[Column] public int? ServiceID { get; set; }
	}
    
	[TableName("dbo.PriceInclusions")]
	[PrimaryKey("PriceInclusionId")]
	[ExplicitColumns]
    public partial class PriceInclusion  
    {
		[Column] public int PriceInclusionId { get; set; }
		[Column] public int PriceId { get; set; }
		[Column] public decimal Amount { get; set; }
		[Column] public string Description { get; set; }
		[Column] public int? MealPlanId { get; set; }
	}
    
	[TableName("dbo.Prices")]
	[PrimaryKey("PriceID")]
	[ExplicitColumns]
    public partial class Price  
    {
		[Column] public int PriceID { get; set; }
		[Column] public int? ServiceID { get; set; }
		[Column] public int? OptionTypeID { get; set; }
		[Column] public DateTime? WEF { get; set; }
		[Column("Price")] public decimal? _Price { get; set; }
		[Column] public decimal? WeekendPrice { get; set; }
	}
    
	[TableName("dbo.Receipt")]
	[PrimaryKey("ReceiptID")]
	[ExplicitColumns]
    public partial class Receipt  
    {
		[Column] public int ReceiptID { get; set; }
		[Column] public DateTime? TDate { get; set; }
		[Column] public string Name { get; set; }
		[Column] public decimal? Amount { get; set; }
		[Column] public string ChequeNo { get; set; }
		[Column] public DateTime? ChqDate { get; set; }
		[Column] public string DrawnOn { get; set; }
		[Column] public string RoomNo { get; set; }
		[Column] public int? BillNo { get; set; }
	}
    
	[TableName("dbo.Review")]
	[PrimaryKey("ReviewID")]
	[ExplicitColumns]
    public partial class Review  
    {
		[Column] public int ReviewID { get; set; }
		[Column] public string UserID { get; set; }
		[Column] public int? ServiceID { get; set; }
		[Column] public int? ServiceTypeID { get; set; }
		[Column] public DateTime? ReviewDate { get; set; }
		[Column("Review")] public string _Review { get; set; }
		[Column] public bool? IsVisible { get; set; }
	}
    
	[TableName("dbo.ReviewReplies")]
	[PrimaryKey("ReviewReplyID")]
	[ExplicitColumns]
    public partial class ReviewReply  
    {
		[Column] public int ReviewReplyID { get; set; }
		[Column] public int? ReviewID { get; set; }
		[Column] public string UserID { get; set; }
		[Column] public DateTime? ReviewDate { get; set; }
		[Column] public string Reply { get; set; }
		[Column] public bool? IsVisible { get; set; }
	}
    
	[TableName("dbo.ServiceDiscount")]
	[PrimaryKey("SDID")]
	[ExplicitColumns]
    public partial class ServiceDiscount  
    {
		[Column] public int SDID { get; set; }
		[Column] public int? ServiceID { get; set; }
		[Column] public int? ServiceTypeID { get; set; }
		[Column] public string CouponCode { get; set; }
	}
    
	[TableName("dbo.Supplier")]
	[PrimaryKey("SupplierID")]
	[ExplicitColumns]
    public partial class Supplier  
    {
		[Column] public int SupplierID { get; set; }
		[Column] public string SupplierName { get; set; }
	}
    
	[TableName("dbo.Taxes")]
	[PrimaryKey("TaxId")]
	[ExplicitColumns]
    public partial class Tax  
    {
		[Column] public int TaxId { get; set; }
		[Column] public int ServiceTypeId { get; set; }
		[Column] public DateTime? WEF { get; set; }
		[Column] public decimal Percentage { get; set; }
		[Column] public string TaxName { get; set; }
	}
    
	[TableName("dbo.UserFunctions")]
	[PrimaryKey("FunctionID")]
	[ExplicitColumns]
    public partial class UserFunction  
    {
		[Column] public int FunctionID { get; set; }
		[Column] public string FunctionName { get; set; }
		[Column] public string Module { get; set; }
	}
    
	[TableName("dbo.UserGroups")]
	[PrimaryKey("UserID", AutoIncrement=false)]
	[ExplicitColumns]
    public partial class UserGroup  
    {
		[Column] public string UserID { get; set; }
		[Column] public int GroupID { get; set; }
	}
    
	[TableName("dbo.Visa")]
	[PrimaryKey("VisaID")]
	[ExplicitColumns]
    public partial class Visa  
    {
		[Column] public int VisaID { get; set; }
		[Column] public string VisaCountry { get; set; }
		[Column] public string FlagPicture { get; set; }
		[Column] public string EmbassyAddress { get; set; }
		[Column] public string Details { get; set; }
	}
    
	[TableName("dbo.Voucher")]
	[PrimaryKey("VoucherID")]
	[ExplicitColumns]
    public partial class Voucher  
    {
		[Column] public int VoucherID { get; set; }
		[Column] public DateTime? TDate { get; set; }
		[Column] public string PayTo { get; set; }
		[Column] public decimal? Amount { get; set; }
		[Column] public string OnAccountOf { get; set; }
		[Column] public string ChequeNo { get; set; }
		[Column] public string DrawnOn { get; set; }
	}
}
