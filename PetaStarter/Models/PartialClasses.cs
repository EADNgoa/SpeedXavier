using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Xml.Serialization;
//using System.Web.Mvc; for dynamic roles
//using Microsoft.AspNet.Identity;for dynamic roles
//using Speedbird.Models;for dynamic roles
//using Microsoft.AspNet.Identity.EntityFramework;for dynamic roles

namespace Speedbird
{
    public class EASelectListData
    {
        public string id { get; set; }
        public string value { get; set; }
    }

    public class ServiceCustomervw
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class Paymentdetailvw
    {
        public string PaxName { get; set; }
        public int DriverID { get; set; }
        public string DriverName { get; set; }
        public int SRDID { get; set; }
        public string BookingNo { get; set; }
        public DateTime Fdate { get; set; }
        public decimal SellPrice { get; set; }
        public decimal Cost { get; set; }
        public string PayTo { get; set; }
    }


    [Serializable()]
    [XmlRoot("REFUND")]
    public class REFUND
    {
        [System.Xml.Serialization.XmlElement("MERCHANTID")]
        public string MERCHANTID { get; set; }

        [System.Xml.Serialization.XmlElement("TXNID")]
        public string TXNID { get; set; }

        [System.Xml.Serialization.XmlElement("AMOUNT")]
        public string AMOUNT { get; set; }

        [System.Xml.Serialization.XmlElement("STATUSCODE")]
        public string STATUSCODE { get; set; }

        [System.Xml.Serialization.XmlElement("STATUSMESSAGE")]
        public string STATUSMESSAGE { get; set; }

        [System.Xml.Serialization.XmlElement("ATOMREFUNDID")]
        public string ATOMREFUNDID { get; set; }
    }

    public class Refundvw
    {
        public string merchantid { get; set; }
        public string txnid { get; set; }
        public string amount { get; set; }
        public string statuscode { get; set; }
        public string statusmsg { get; set; }
        public string refundid { get; set; }
    }


    //Classes for SRDetails

    public abstract class SupplierInfo
    {
        public int SRID { get; set; }
        public int ServiceTypeID { get; set; }
        public int SRDID { get; set; }
        public string IsCanceled { get; set; }
        public string PaxName { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string SuppInvNo { get; set; }
        public DateTime? SuppInvDt { get; set; }
        public int SuppInvAmt { get; set; }
        public string CouponCode { get; set; }
        public string SuppConfNo { get; set; }
        public string ContractNo { get; set; }
        public abstract decimal TotalCost { get; }
        public abstract decimal COM { get; }
        public decimal Tax { get; set; }
        public decimal ECommision { get; set; }
        public decimal SellPrice { get; set; }
    }

    public class Packagevw : SupplierInfo
    {
        public int NOOfPax { get; set; }
        public DateTime Tdate { get; set; }
        public DateTime Fdate { get; set; }
        public DateTime DOB { get; set; }
        public string PackageType { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public string NameofTour { get; set; }
        public bool IsReturn { get; set; }
        public decimal NoOfDays { get; set; }
        public string HotelCat { get; set; }
        public int CarType { get; set; }
        public decimal Cost { get; set; }
        public int DinnerCost { get; set; }
        public decimal NetCost { get; set; }
        public decimal AddCost { get; set; }
        public string AddDtl { get; set; }
        public bool Cancelled { get; set; }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public int RefundId { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }

    public class Visavw : SupplierInfo
    {
        public string PassportNo { get; set; }
        public DateTime Tdate { get; set; }
        public DateTime Fdate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? DOB { get; set; }
        public string Nationality { get; set; }
        public decimal Cost { get; set; }
        public string PayTo { get; set; }
        public int Duration { get; set; }
        public int OptionTypeID { get; set; }
        public string OptionTypeName { get; set; }
        public string VisaCountry { get; set; }
        public decimal ExtraServiceCost { get; set; }
        public bool Cancelled { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public override decimal COM { get { return SellPrice - TotalCost; } }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public int RefundId { get; set; }
    }

    
    public class Insurancevw : SupplierInfo
    {
        public DateTime? DOB { get; set; }
        public string Destination { get; set; }
        public string PolicyType { get; set; }
        public string PolicyName { get; set; }
        public int NoOfDays { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public decimal Cost { get; set; }
        public bool IsCancelled { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public override decimal COM { get { return SellPrice - TotalCost; } }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public int RefundId { get; set; }
    }

    public class Cruisevw : SupplierInfo
    {
        public string DropPoint { get; set; }
        public string PickUpPoint { get; set; }
        public string PickupLocation { get; set; }
        public int NoOfCars { get; set; }
        public decimal Passengers { get; set; }
        public string CabinType { get; set; }
        public string Cabins { get; set; }
        public DateTime Departuredate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string FromPort { get; set; }
        public string ViaPoint { get; set; }
        public string ToPort { get; set; }
        public int Nights { get; set; }
        public string MealPlan { get; set; }
        public decimal Cost { get; set; }
        public int DinnerCost { get; set; }
        public string CruiseName { get; set; }
        public bool Cancelled { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public override decimal COM { get { return SellPrice - TotalCost; } }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public int RefundId { get; set; }
    }


    public class Busvw : SupplierInfo
    {
        public DateTime? DOT { get; set; }
        public string PickUpPoint { get; set; }
        public string BusName { get; set; }
        public int Age { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public DateTime? Arrival { get; set; }
        public DateTime? Departure { get; set; }
        public int BusNo { get; set; }
        public int OptionTypeID { get; set; }
        public int CarType { get; set; }
        public int TicketNo { get; set; }
        public decimal Cost { get; set; }
        public int DinnerCost { get; set; }
        public decimal AddCost { get; set; }
        public decimal FullCost { get; set; }
        public string AddDetl { get; set; }
        public bool Cancelled { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public override decimal COM { get { return SellPrice - TotalCost; } }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public int RefundId { get; set; }
    }

    public class Railvw : SupplierInfo
    {
        public DateTime? DOT { get; set; }
        public string PickUpPoint { get; set; }
        public string TrainName { get; set; }
        public int Age { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public DateTime? Arrival { get; set; }
        public DateTime? Departure { get; set; }
        public int TrainNo { get; set; }
        public string Class { get; set; }
        public int OptionTypeID { get; set; }
        public int CarType { get; set; }
        public int TicketNo { get; set; }
        public decimal Cost { get; set; }
        public int DinnerCost { get; set; }
        public decimal AddCost { get; set; }
        public string AddDetl { get; set; }
        public bool Cancelled { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public override decimal COM { get { return SellPrice - TotalCost; } }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public int RefundId { get; set; }
    }



    public class CarBikevw : SupplierInfo
    {
        public string DropPoint { get; set; }
        public string PickUpPoint { get; set; }
        public string PickupLocation { get; set; }
        public int NoOfCars { get; set; }
        public decimal Qty { get; set; }
        public string Model { get; set; }
        public DateTime? Fdate { get; set; }
        public DateTime? Tdate { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public int CarType { get; set; }
        public decimal Cost { get; set; }
        public int BFCost { get; set; }
        public decimal ExtraServiceCost { get; set; }
        public string PayTo { get; set; }
        public bool IsCancelled { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public override decimal COM { get { return SellPrice - TotalCost; } }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public int RefundId { get; set; }
    }

    public class TransferServiceView : SupplierInfo
    {
        public int cartype { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime? serviceDate { get; set; }
        public decimal Cost { get; set; }
        public string DriverName { get; set; }
        public string Car { get; set; }
        public string DropPoint { get; set; }
        public DateTime? Fdate { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public bool HasAc { get; set; }
        public bool HasCarrier { get; set; }
        public string RateBasis { get; set; }
        public string Model { get; set; }
        public string PayTo { get; set; }
        public string PickUpPoint { get; set; }
        public int NoOfVehicles { get; set; }
        public decimal VehicleCost { get; set; }
        public bool IsCancelled { get; set; }
        public override decimal TotalCost { get { return Cost + VehicleCost; } }
        public override decimal COM { get { return SellPrice - TotalCost; } }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public int RefundId { get; set; }
    }

    public class AccomodationServiceView : SupplierInfo
    {        
        public string AccomName { get; set; }
        public int AdultNo { get; set; }
        public int ExtraBedCost { get; set; }
        public int ChildNo { get; set; }
        public decimal Cost { get; set; }
        public DateTime checkin { get; set; }
        public string FromLoc { get; set; }
        public bool HasAc { get; set; }
        public string ExtraService { get; set; }
        public int InfantNo { get; set; }
        public string RoomCategory { get; set; }
        public string payto { get; set; }
        public string RoomType { get; set; }
        public int CarType { get; set; }
        public int NoOfRooms { get; set; }        
        public DateTime checkout { get; set; }
        public int NoExtraBeds { get; set; }
        public int BFCost { get; set; }
        public int LunchCost { get; set; }
        public int DinnerCost { get; set; }
        public int NoExtraService { get; set; }
        public int ExtraServiceCost { get; set; }
        public int EBCostPNight { get; set; }
        public bool IsCancelled { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
        public override decimal TotalCost { get { return Cost ; } }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public int RefundId { get; set; }
    }

        public class SightseeingServiceView : SupplierInfo
    {
        public string DriverName { get; set; }
        public string Car { get; set; }
        public string SightseeingName { get; set; }
        public int AdultNo { get; set; }
        public int ChildNo { get; set; }
        public int OptionTypeID { get; set; }
        public string OptionTypeName { get; set; }
        public string PickUpPoint { get; set; }
        public string PickupLocation { get; set; }
        public string Private_Sic { get; set; }
        public decimal CostPerCar { get; set; }
        public int NoOfCars { get; set; }
        public int AdultCost { get; set; }
        public int ChildCost { get; set; }
        public DateTime TourDate { get; set; }
        public DateTime PickupTime { get; set; }
        public int CarType { get; set; }
        public bool MealIncluded { get; set; }
        public int GuideLanguageName { get; set; }
        public bool IsCancelled { get; set; }
        public override decimal TotalCost { get { return (CostPerCar * NoOfCars) + (AdultCost * AdultNo) + (ChildCost * ChildNo); } }
        public override decimal COM { get { return SellPrice - TotalCost; } }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public int RefundId { get; set; }
    }

    public class PassportView : SupplierInfo
    {
        public DateTime? DOB { get; set; }
        public string PassPortNo { get; set; }
        public string Nationality { get; set; }
        public decimal Cost { get; set; }
        public string Heritage { get; set; }
        public bool IsCancelled { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public override decimal COM { get { return SellPrice - TotalCost; } }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public int RefundId { get; set; }
    }

    public class FlightServiceView : SupplierInfo
    {        
        public int AdultNo {get; set; }
        public int ChildNo {get; set; } 
        public int InfantNo {get; set; } 
        public bool IsInternational {get; set; } 
        public string FromLoc {get; set; } 
        public string ToLoc {get; set; } 
        public int ClassID {get; set; } 
        public string Class { get { return (ClassID == 1) ? "Economy" : "Business"; } } 
        public string AirlineCode {get; set; } 
        public string FlightNo {get; set; } 
        public DateTime? DepartureOn {get; set; } 
        public DateTime? ArrivalOn {get; set; } 
        public string TicketNo {get; set; } 
        public string GDSConfNo {get; set; } 
        public string AirlinePNR {get; set; } 
        public decimal Cost {get; set; } 
        public string Airline {get; set; } 
        public string Extra {get; set; } 
        public string ExtraDetails {get; set; }
        public bool IsCancelled { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public override decimal COM { get { return SellPrice - TotalCost; } }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public int RefundId { get; set; }
    }

    //class daily services

    public class PackageDaily : SupplierInfo
    {
        public int NOOfPax { get; set; }
        public DateTime Tdate { get; set; }
        public DateTime Fdate { get; set; }
        public DateTime DOB { get; set; }
        public string PackageType { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public string NameofTour { get; set; }
        public bool IsReturn { get; set; }
        public decimal NoOfDays { get; set; }
        public string HotelCat { get; set; }
        public int CarType { get; set; }
        public decimal Cost { get; set; }
        public decimal NetCost { get; set; }
        public decimal AddCost { get; set; }
        public string AddDtl { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public string AgentID { get; set; }
        public string ContactName { get; set; }
        public int? BookingNo { get; set; }
        public int? EnquirySource { get; set; }
        public int? PaxNo { get; set; }
        public DateTime? TransDate { get; set; }
        public string Event { get; set; }
        public string PhoneNo { get; set; }
        public string Contact { get; set; }
        public string Id { get; set; }
        public string Agency { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }

    public class VisaDaily : SupplierInfo
    {
        public string PassportNo { get; set; }
        public DateTime Tdate { get; set; }
        public DateTime Fdate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? DOB { get; set; }
        public string Nationality { get; set; }
        public decimal Cost { get; set; }
        public string PayTo { get; set; }
        public int Duration { get; set; }
        public int OptionTypeID { get; set; }
        public string OptionTypeName { get; set; }
        public string VisaCountry { get; set; }
        public decimal ExtraServiceCost { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public string AgentID { get; set; }
        public string ContactName { get; set; }
        public int? BookingNo { get; set; }
        public int? EnquirySource { get; set; }
        public int? PaxNo { get; set; }
        public DateTime? TransDate { get; set; }
        public string Event { get; set; }
        public string PhoneNo { get; set; }
        public string Contact { get; set; }
        public string Id { get; set; }
        public string Agency { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }



    public class InsuranceDaily : SupplierInfo
    {
        public DateTime? DOB { get; set; }
        public string Destination { get; set; }
        public string PolicyType { get; set; }
        public string PolicyName { get; set; }
        public int NoOfDays { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public decimal Cost { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public string AgentID { get; set; }
        public string ContactName { get; set; }
        public int? BookingNo { get; set; }
        public int? EnquirySource { get; set; }
        public int? PaxNo { get; set; }
        public DateTime? TDate { get; set; }
        public string Event { get; set; }
        public string PhoneNo { get; set; }
        public string Contact { get; set; }
        public string Id { get; set; }
        public string Agency { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }

    public class CruiseDaily : SupplierInfo
    {
        public string DropPoint { get; set; }
        public string PickUpPoint { get; set; }
        public string PickupLocation { get; set; }
        public int NoOfCars { get; set; }
        public decimal Passengers { get; set; }
        public string CabinType { get; set; }
        public string Cabins { get; set; }
        public DateTime Departuredate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string FromPort { get; set; }
        public string ViaPoint { get; set; }
        public string ToPort { get; set; }
        public int Nights { get; set; }
        public string MealPlan { get; set; }
        public decimal Cost { get; set; }
        public string CruiseName { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public string AgentID { get; set; }
        public string ContactName { get; set; }
        public int? BookingNo { get; set; }
        public int? EnquirySource { get; set; }
        public int? PaxNo { get; set; }
        public DateTime? TDate { get; set; }
        public string Event { get; set; }
        public string PhoneNo { get; set; }
        public string Contact { get; set; }
        public string Id { get; set; }
        public string Agency { get; set; }
        public int DinnerCost { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }


    public class BusDaily : SupplierInfo
    {
        public DateTime? DOT { get; set; }
        public string PickUpPoint { get; set; }
        public string BusName { get; set; }
        public int Age { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public DateTime? Arrival { get; set; }
        public DateTime? Departure { get; set; }
        public int BusNo { get; set; }
        public int OptionTypeID { get; set; }
        public int CarType { get; set; }
        public int TicketNo { get; set; }
        public int TicketCost { get; set; }
        public decimal Cost { get; set; }
        public decimal AddCost { get; set; }
        public string AddDetl { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public string AgentID { get; set; }
        public string ContactName { get; set; }
        public int? BookingNo { get; set; }
        public int? EnquirySource { get; set; }
        public int? PaxNO { get; set; }
        public DateTime? TDate { get; set; }
        public string Event { get; set; }
        public string PhoneNo { get; set; }
        public string Contact { get; set; }
        public string Id { get; set; }
        public string Agency { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }

    public class RailDaily : SupplierInfo
    {
        public DateTime? DOT { get; set; }
        public string PickUpPoint { get; set; }
        public string TrainName { get; set; }
        public int Age { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public DateTime? Arrival { get; set; }
        public DateTime? Departure { get; set; }
        public int TicketCost { get; set; }
        public int TrainNo { get; set; }
        public string Class { get; set; }
        public int OptionTypeID { get; set; }
        public int CarType { get; set; }
        public int TicketNo { get; set; }
        public decimal Cost { get; set; }
        public decimal AddCost { get; set; }
        public string AdditionalDetails { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public string AgentID { get; set; }
        public string ContactName { get; set; }
        public int? BookingNo { get; set; }
        public int? EnquirySource { get; set; }
        public int? PaxNo { get; set; }
        public DateTime? TDate { get; set; }
        public string Event { get; set; }
        public string PhoneNo { get; set; }
        public string Contact { get; set; }
        public string Id { get; set; }
        public string Agency { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }

    public class CarDaily : SupplierInfo
    {
        public string DropPoint { get; set; }
        public string PickUpPoint { get; set; }
        public string PickupLocation { get; set; }
        public int NoOfCars { get; set; }
        public decimal Qty { get; set; }
        public string Model { get; set; }
        public DateTime? Fdate { get; set; }
        public DateTime? Tdate { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public int CarType { get; set; }
        public decimal Cost { get; set; }
        public int NoExtraBeds { get; set; }
        public string PayTo { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public string AgentID { get; set; }
        public string ContactName { get; set; }
        public int? BookingNo { get; set; }
        public int? EnquirySource { get; set; }
        public int? PaxNo { get; set; }
        public DateTime? TransDate { get; set; }
        public string Event { get; set; }
        public string PhoneNo { get; set; }
        public string Contact { get; set; }
        public string Id { get; set; }
        public string Agency { get; set; }
        public int BFCost { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }

    public class TransferServiceDaily : SupplierInfo
    {
        public int cartype { get; set; }
        public DateTime? serviceDate { get; set; }
        public decimal Cost { get; set; }
        public string DriverName { get; set; }
        public string Car { get; set; }
        public string DropPoint { get; set; }
        public DateTime? Fdate { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public bool HasAc { get; set; }
        public bool HasCarrier { get; set; }
        public string RateBasis { get; set; }
        public string Model { get; set; }
        public string PayTo { get; set; }
        public string PickUpPoint { get; set; }
        public int NoOfVehicles { get; set; }
        public decimal VehicleCost { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public string AgentID { get; set; }
        public string ContactName { get; set; }
        public int? BookingNo { get; set; }
        public int? EnquirySource { get; set; }
        public int? PaxNo { get; set; }
        public DateTime? TDate { get; set; }
        public string Event { get; set; }
        public string PhoneNo { get; set; }
        public string Contact { get; set; }
        public string Id { get; set; }
        public string Agency { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }

    public class AccomodationServiceDaily : SupplierInfo
    {
        public string AccomName { get; set; }
        public int AdultNo { get; set; }
        public int ExtraBedCost { get; set; }
        public int ChildNo { get; set; }
        public decimal Cost { get; set; }
        public DateTime checkin { get; set; }
        public string FromLoc { get; set; }
        public bool HasAc { get; set; }
        public string ExtraService { get; set; }
        public int InfantNo { get; set; }
        public string RoomCategory { get; set; }
        public string payto { get; set; }
        public string RoomType { get; set; }
        public int CarType { get; set; }
        public int NoOfRooms { get; set; }
        public DateTime checkout { get; set; }
        public int NoExtraBeds { get; set; }
        public int BFCost { get; set; }
        public int LunchCost { get; set; }
        public int DinnerCost { get; set; }
        public int NoExtraService { get; set; }
        public int ExtraServiceCost { get; set; }
        public int EBCostPNight { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public string AgentID { get; set; }
        public string ContactName { get; set; }
        public int? BookingNo { get; set; }
        public int? EnquirySource { get; set; }
        public DateTime? TDate { get; set; }
        public string Event { get; set; }
        public string PhoneNo { get; set; }
        public string Contact { get; set; }
        public string Id { get; set; }
        public string Agency { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }

    public class SightseeingServiceViewDaily : SupplierInfo
    {
        public string DriverName { get; set; }
        public string Car { get; set; }
        public string SightseeingName { get; set; }
        public int AdultNo { get; set; }
        public int ChildNo { get; set; }
        public int OptionTypeID { get; set; }
        public string OptionTypeName { get; set; }
        public string PickUpPoint { get; set; }
        public string PickupLocation { get; set; }
        public string Private_Sic { get; set; }
        public decimal CostPerCar { get; set; }
        public decimal Cost { get; set; }
        public int NoOfCars { get; set; }
        public int AdultCost { get; set; }
        public int ChildCost { get; set; }
        public DateTime TourDate { get; set; }
        public int CarType { get; set; }
        public bool MealIncluded { get; set; }
        public int GuideLanguageName { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public string AgentID { get; set; }
        public string ContactName { get; set; }
        public int? BookingNo { get; set; }
        public DateTime? TDate { get; set; }
        public int? EnquirySource { get; set; }
        public string Event { get; set; }
        public string PhoneNo { get; set; }
        public string Contact { get; set; }
        public string Id { get; set; }
        public string Agency { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }

    public class PassportDaily : SupplierInfo
    {
        public DateTime? Fdate { get; set; }
        public string PassPortNo { get; set; }
        public string Nationality { get; set; }
        public decimal Cost { get; set; }
        public string Heritage { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public string AgentID { get; set; }
        public string ContactName { get; set; }
        public int? BookingNo { get; set; }
        public DateTime? TDate { get; set; }
        public int? EnquirySource { get; set; }
        public int? PaxNo { get; set; }
        public string Event { get; set; }
        public string PhoneNo { get; set; }
        public string Contact { get; set; }
        public string Id { get; set; }
        public string Agency { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }

    public class FlightServiceDaily : SupplierInfo
    {
        public int AdultNo { get; set; }
        public int ChildNo { get; set; }
        public int InfantNo { get; set; }
        public bool IsInternational { get; set; }
        public string FromLoc { get; set; }
        public string ToLoc { get; set; }
        public int ClassID { get; set; }
        public string Class { get { return (ClassID == 1) ? "Economy" : "Business"; } }
        public string AirlineCode { get; set; }
        public string FlightNo { get; set; }
        public DateTime? DepartureOn { get; set; }
        public DateTime? ArrivalOn { get; set; }
        public string TicketNo { get; set; }
        public string GDSConfNo { get; set; }
        public string AirlinePNR { get; set; }
        public decimal Cost { get; set; }
        public string Airline { get; set; }
        public string Extra { get; set; }
        public string ExtraDetails { get; set; }
        public override decimal TotalCost { get { return Cost; } }
        public string AgentID { get; set; }
        public string ContactName { get; set; }
        public int? BookingNo { get; set; }
        public DateTime? TDate { get; set; }
        public int? EnquirySource { get; set; }
        public string Event { get; set; }
        public string PhoneNo { get; set; }
        public string Contact { get; set; }
        public string Id { get; set; }
        public string Agency { get; set; }
        public override decimal COM { get { return SellPrice - TotalCost; } }
    }


    //EOF Classes for SRDetails

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
        public decimal CreditAmt { get; set; }
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

    public class DriversEndingInsurance
    {
        public int DriverId { get; set; }
        public string DriverName { get; set; }
        public string CarBrand { get; set; }
        public string Model { get; set; }
        public string PlateNo { get; set; }
        public DateTime InsuranceEndDate { get; set; }
        public string InsuranceCompany { get; set; }

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

        public decimal DiscountedPrice { get; set; }
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
        public int SRDID { get; set; }
        public int Path { get; set; }
        public string UploadName { get; set; }
        public HttpPostedFileBase UploadedFile { get; set; }
    }

    public class Refunds
    {
        public int SRID { get; set; }
        public int SRDID { get; set; }
        public decimal ProdCanxCost { get; set; }
        public decimal SBCanxCost { get; set; }
        public string Note { get; set; }
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
        Transfer=0,
        Accomodation=1,
        SightSeeing=2,
        Flight=3,
        Insurance=4,
        Packages=5,
        Visa=6,
        CarBike=7,
        Cruise=8,
        Passport = 9,
        Bus = 10,
        Rail = 11
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
        Corporate_Client,
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
        Cancelled,
        Deleted
      


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
        HatchBack,
        Sedan,
        SUV,
        MPV
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
        Cancelled,
        Deleted

    }
    public enum AmtType
    {
        Cash,
        Cheque,
        Internet_Banking,
        Card
    }

    public enum PayToEnum
    {
        Driver,
        Supplier,
        Agent
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