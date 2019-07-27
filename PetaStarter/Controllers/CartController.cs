using Microsoft.AspNet.Identity;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Speedbird.Controllers
{
    public class CartController : EAController
    {
        public ActionResult Cart()
        {
            var CartItems = db.Query<CartDets>("Select * from Cart c Where Id= @0", (string)User.Identity.GetUserId()).ToList();
            CartItems.ForEach(c =>
            {
                c.Pic = db.ExecuteScalar<string>("Select PictureName From Picture Where ServiceID =@0 and ServiceTypeID =@1 ", c.ServiceID, c.ServiceTypeID);

                if (c.ServiceTypeID == (int)ServiceTypeEnum.Accomodation)
                {
                    c.ServiceName = db.ExecuteScalar<string>("Select AccomName From Accomodation where AccomodationID=@0", c.ServiceID);

                }
                if (c.ServiceTypeID == (int)ServiceTypeEnum.Packages || c.ServiceTypeID == (int)ServiceTypeEnum.Cruise || c.ServiceTypeID == (int)ServiceTypeEnum.SightSeeing)
                {
                    c.lang = db.Query<LanguageDets>("Select * from GuideLanguage gl inner join Package_Language pl on pl.GuideLanguageID =gl.GuideLanguageID inner join Package p on p.PackageID= pl.PackageID where pl.PackageID=@0", c.ServiceID);
                    c.ServiceName = db.ExecuteScalar<string>("Select PackageName From Package where PackageID=@0", c.ServiceID);
                }
                if (c.ServiceTypeID == (int)ServiceTypeEnum.CarBike)
                {
                    c.ServiceName = db.ExecuteScalar<string>("Select CarBikeName From CarBike where CarBikeID=@0", c.ServiceID);
                }

                //ViewBag.DiscountedTotal = db.ExecuteScalar<decimal>("Select Sum(DiscountedPrice) as DiscountedPrice from Cart Where Id=@0", (string)User.Identity.GetUserId());

            });
            

            return View(CartItems);
        }



        [HttpPost]
        public ActionResult AddToCart(int? ServiceID, int? ServiceTypeID, System.Web.HttpPostedFileBase UploadedFile, DateTime? CheckIn, DateTime? CheckOut, int? nums, int? Qty, string fname, string sname, string email, string phno, int? CustomerID, string chckbtn, string Query, string Gtime, int? Glang)
        {
            if (chckbtn == "cart")
            {

                Cart item = new Cart();
                if (ServiceTypeID == (int)ServiceTypeEnum.Accomodation)
                {
                    var getItem = db.FirstOrDefault<AccomodationDets>("Select * From Accomodation where AccomodationID=@0", ServiceID);
                    var dets = db.FirstOrDefault<CartDets>("Select Top 1 Price as OrigPrice,p.OptionTypeID from Prices p inner join OptionType ot on p.OptionTypeID = ot.OptionTypeID Where ServiceID=@0 and ot.ServiceTypeID=@1 and WEF<GetDate() order by WEF desc", ServiceID, ServiceTypeID);
                    getItem.price = dets.OrigPrice;

                    item = new Cart { Id = User.Identity.GetUserId(), ServiceID = ServiceID, Qty = Qty, CheckIn = CheckIn, CheckOut = CheckOut, NoOfGuest = nums, OrigPrice = getItem.price, ServiceTypeID = ServiceTypeID, OptionTypeID = dets.OptionTypeID };
                    db.Insert(item);
                }
                if (ServiceTypeID == (int)ServiceTypeEnum.Packages || ServiceTypeID == (int)ServiceTypeEnum.Cruise || ServiceTypeID == (int)ServiceTypeEnum.SightSeeing)
                {
                    var getItem = db.FirstOrDefault<PackageDets>("Select * From Package where PackageID=@0", ServiceID);
                    var dets = db.FirstOrDefault<CartDets>("Select Top 1 Price as OrigPrice,p.OptionTypeID from Prices p inner join OptionType ot on p.OptionTypeID = ot.OptionTypeID Where ServiceID=@0 and ot.ServiceTypeID=@1 and WEF<GetDate() order by WEF desc", ServiceID, ServiceTypeID);
                    getItem.price = dets.OrigPrice;

                    item = new Cart { Id = User.Identity.GetUserId(), ServiceID = ServiceID, Qty = Qty, CheckIn = CheckIn, CheckOut = CheckOut, NoOfGuest = nums, OrigPrice = getItem.price, ServiceTypeID = ServiceTypeID, OptionTypeID = dets.OptionTypeID };
                    db.Insert(item);
                }
                if (ServiceTypeID == (int)ServiceTypeEnum.CarBike)
                {
                    var getItem = db.FirstOrDefault<CarBikeDets>("Select * From CarBike where CarbikeID=@0", ServiceID);
                    var dets = db.FirstOrDefault<CartDets>("Select Top 1 Price as OrigPrice,p.OptionTypeID from Prices p inner join OptionType ot on p.OptionTypeID = ot.OptionTypeID Where ServiceID=@0 and ot.ServiceTypeID=@1 and WEF<GetDate() order by WEF desc", ServiceID, ServiceTypeID);
                    getItem.price = dets.OrigPrice;

                    item = new Cart { Id = User.Identity.GetUserId(), ServiceID = ServiceID, Qty = Qty, CheckIn = CheckIn, CheckOut = CheckOut, NoOfGuest = nums, OrigPrice = getItem.price, ServiceTypeID = ServiceTypeID, OptionTypeID = dets.OptionTypeID };
                    db.Insert(item);
                }

                if (CustomerID == null)
                {
                    var cust = new Customer { FName = fname, SName = sname, Email = email, Phone = phno };

                    if (UploadedFile != null)
                    {
                        string fn = UploadedFile.FileName.Substring(UploadedFile.FileName.LastIndexOf('\\') + 1);
                        fn = String.Concat(((ServiceTypeEnum)ServiceTypeID).ToString(), "_", ServiceID.ToString(), "_", fn);

                        string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                        UploadedFile.SaveAs(SavePath);

                        cust.IdPicture = fn;
                    }
                    db.Insert(cust);
                    db.Insert(new BookedCustomer { CartID = item.CartID, CustomerID = cust.CustomerID });
                }
                else
                {
                    db.Execute($"Update Customer Set FName='{fname}',SName='{sname}',Phone={phno} where CustomerID={CustomerID}");
                }
                return RedirectToAction("Cart");
            }
            if (chckbtn == "Query")
            {

                if (Query != null)
                {

                    if (ServiceTypeID == (int)ServiceTypeEnum.Accomodation)
                    {
                        var ServiceName = db.FirstOrDefault<AccomodationDets>("Select * From Accomodation where AccomodationID=@0", ServiceID).AccomName;
                    }
                    if (ServiceTypeID == (int)ServiceTypeEnum.Packages || ServiceTypeID == (int)ServiceTypeEnum.Cruise || ServiceTypeID == (int)ServiceTypeEnum.SightSeeing)
                    {
                        var ServiceName = db.FirstOrDefault<PackageDets>("Select * From Package where PackageID=@0", ServiceID).PackageName;
                    }
                    if (ServiceTypeID == (int)ServiceTypeEnum.CarBike)
                    {
                        var ServiceName = db.FirstOrDefault<CarBikeDets>("Select * From CarBike where CarbikeID=@0", ServiceID).CarBikeName;
                    }

                    // var cust = new ServiceRequest { FName = fname, SName = sname, Email = email, Phone = phno,_Query=Query,ServiceID=ServiceID,ServiceTypeID=ServiceTypeID ,CheckIn=CheckIn,CheckOut=CheckOut,NoPax=nums,Qty=Qty,Tdate=DateTime.Now, Glang=Glang, Gtime=Gtime,ServiceName=ServiceName};
                    var sr = new ServiceRequest { BookingTypeID = (int)BookingTypeEnum.Online, EnquirySource = (int)EnquirySourceEnum.Web, ServiceTypeID = ServiceTypeID, PayStatusID = (int)PayType.Not_Paid, SRStatusID = (int)SRStatusEnum.Unconfirmed };

                    var cust = new Customer { FName = fname, SName = sname, Email = email, Phone = phno };
                    if (UploadedFile != null)
                    {
                        string fn = UploadedFile.FileName.Substring(UploadedFile.FileName.LastIndexOf('\\') + 1);
                        fn = String.Concat(((ServiceTypeEnum)ServiceTypeID).ToString(), "_", ServiceID.ToString(), "_", fn);
                        string SavePath = System.IO.Path.Combine(Server.MapPath("~/Images"), fn);
                        UploadedFile.SaveAs(SavePath);
                        cust.IdPicture = fn;
                    }
                    db.Insert(cust);
                    sr.CustID = cust.CustomerID;
                    db.Insert(sr);
                    db.Insert(new SRdetail
                    {
                        SRID = sr.SRID,
                        ItemID = ServiceID,
                        ServiceTypeID = ServiceTypeID,

                        Qty = Qty,
                        Fdate = CheckIn,
                        Tdate = CheckOut,
                        AdultNo = nums,
                    });
                    db.Insert(new SR_Cust { ServiceRequestID = sr.SRID, CustomerID = cust.CustomerID });
                }
            }
            return RedirectToAction("Index", "Home");
        }

        private string SaveImage(Sql sql, string v, int customerID, HttpPostedFileBase uploadedFile)
        {
            throw new NotImplementedException();
        }

        //[HttpPost]
        //public ActionResult BooKCartItems()
        //{
        //    using (var transaction = db.GetTransaction())
        //    {
        //        var CartItems = db.Query<CartDets>("Select * from Cart c Where Id= @0", (string)User.Identity.GetUserId()).ToList();

        //        try
        //        {
        //            var item = new ServiceRequest { TDate = DateTime.Now, UserID = User.Identity.GetUserId(), SRStatusID = (int)SRStatusEnum.Confirmed,PayStatusID=(int)PayType.Full_Paid,BookingTypeID=(int)BookingTypeEnum.Online,EnquirySource=(int)EnquirySourceEnum.Web };
        //            db.Insert(item);
        //            CartItems.ForEach(c =>
        //            {

        //                var Bcust = db.FirstOrDefault<BookedCustomer>("select * from BookedCustomer where CartID=@0", c.CartID);
        //                if (Bcust!=null && Bcust.CustomerID!= null)
        //                {
        //                    item.CustID = Bcust.CustomerID;
        //                    db.Update(item);
        //                   var del= db.Execute($"Delete From BookedCustomer Where BCID={Bcust.BCID}");

        //                    db.Insert(new SR_Cust {ServiceRequestID=item.SRID,CustomerID=(int)Bcust.CustomerID });

        //                }
        //                var bd = new SRdetail
        //                {
        //                   /* BookingID = item.BookingID,
        //                    ServiceID = c.ServiceID,
        //                    ServiceTypeID = c.ServiceTypeID,
        //                    OptionTypeID = c.OptionTypeID,
        //                    Qty = c.Qty,
        //                    CheckIn = c.CheckIn,
        //                    CheckOut = c.CheckOut,
        //                    NoOfGuests = c.NoOfGuest,
        //                    Price = c.OrigPrice*/

        //                    SRID=item.SRID,
        //                    ItemID=c.ServiceID,
        //                    ServiceTypeID=c.ServiceTypeID,
        //                    OptionTypeID=c.OptionTypeID,
        //                    Qty=c.Qty,
        //                    Fdate=c.CheckIn,
        //                    Tdate=c.CheckOut,
        //                    AdultNo=c.NoOfGuest,
        //                    SellPrice=c.OrigPrice

        //                };
        //                db.Insert(bd);
        //             var delc= db.Execute("Delete From Cart Where CartID=@0", c.CartID);
        //            });
        //            transaction.Complete();

        //        }
        //        catch (Exception ex)
        //        {
        //            db.AbortTransaction();
        //            throw ex;
        //        }
        //        return RedirectToAction("ThankYou");
        //    }
        //}

        [HttpPost]
        public ActionResult PaymentIntegrate()
        {

            decimal amt = 0;
            var Amount = db.ExecuteScalar<decimal>("Select Sum(OrigPrice) as OrigPrice from Cart Where Id=@0", (string)User.Identity.GetUserId());
            var DiscountAmount = db.ExecuteScalar<decimal>("Select Sum(DiscountedPrice) as OrigPrice from Cart Where Id=@0", (string)User.Identity.GetUserId());
            
            var cart = db.Query<Cart>("Select CouponCode from Cart Where Id=@0", (string)User.Identity.GetUserId());

            foreach (var crt in cart)
            {
                if (crt.CouponCode != null)
                {
                    amt = DiscountAmount;
                }
                else
                {
                    amt = Amount;
                }
            }
           

            string ProductID = "NSE";
            string TransactionServiceCharge = "0";
            var config = db.Query<Config>("Select * from Config");
            foreach (var product in config)
            {
                if (product.ProductId == "NSE")
                {
                    ProductID = "NSE";
                }
                else
                {
                    ProductID = "CSE";
                }

                TransactionServiceCharge = "0";
            }


            string strClientCode, strClientCodeEncoded;
            byte[] b;
            string strResponse = "";

            UriBuilder Url = new UriBuilder("https://paynetzuat.atomtech.in/paynetz/epi/fts");
            string MerchantLogin = "197";
            string MerchantPass = "Test@123";
            string TransactionType = "NBFundtransfer";
            string TransactionID = "123";
            string TransactionAmount = amt.ToString();
            string TransactionCurrency = "INR";
            DateTime TransactionDateTime = DateTime.UtcNow;
            //string BankID = "2001";
            string CustomerAccountNo = "100000036600";
            string ClientCode = "ESB07874"; //newly added

            string ru = "http://localhost:53040/Cart/AtomResponseReciever";
            try
            {
                b = Encoding.UTF8.GetBytes(ClientCode);
                strClientCode = Convert.ToBase64String(b);
                strClientCodeEncoded = HttpUtility.UrlEncode(strClientCode);
                //string reqHashKey = requestkey;
                string reqHashKey = "KEY123657234";
                //string respHashKey = "KEYRESP123657234";
                string signature = "";
                byte[] bytes = Encoding.UTF8.GetBytes(reqHashKey);
                string strsignature = MerchantLogin + MerchantPass + TransactionType + ProductID + TransactionID + TransactionAmount + TransactionCurrency;
                byte[] bt = new System.Security.Cryptography.HMACSHA512(bytes).ComputeHash(Encoding.UTF8.GetBytes(strsignature));
                //byte[] b = new HMACSHA512(bytes).ComputeHash(Encoding.UTF8.GetBytes(prodid));
                signature = byteToHexString(bt).ToLower();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                Url.Query = $"login={MerchantLogin}&pass={MerchantPass}&ttype={TransactionType}" +
                    $"&prodid={ProductID}&amt={TransactionAmount}" +
                    $"&txncurr={TransactionCurrency}&txnscamt={TransactionServiceCharge}" +
                    $"&clientcode={ClientCode}&txnid={TransactionID}&date={TransactionDateTime}" +
                    $"&custacc={CustomerAccountNo}&udf1=bobsmith&udf9=ABCD&udf3=9999999999" +
                    $"&ru={ru}&signature={signature}";

                return Redirect(Url.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static string byteToHexString(byte[] byData)
        {
            StringBuilder sb = new StringBuilder((byData.Length * 2));
            for (int i = 0; (i < byData.Length); i++)
            {
                int v = (byData[i] & 255);
                if ((v < 16))
                {
                    sb.Append('0');
                }

                sb.Append(v.ToString("X"));

            }

            return sb.ToString();
        }

        public ActionResult AtomResponseReciever(string mmp_txn, string mer_txn, string amt, string prod, string date, string bank_txn, string f_code, string bank_name, string clientcode, string signature, string discriminator)
        {
            try
            {

                string strsignature = null;
                string respHashKey = "KEYRESP123657234";
                string ressignature = "";

                var atom = new AtomResponse()
                {
                    postingmmp_txn = mmp_txn,
                    postingmer_txn = mer_txn,
                    postinamount = amt,
                    postingdate = date,
                    postingprod = prod,
                    postingbank_txn = bank_txn,
                    postingf_code = f_code,
                    postingbank_name = bank_name,
                    postingclientcode = clientcode,
                    signature = signature,
                    postingdiscriminator = discriminator
                };

                strsignature = atom.postingmmp_txn + atom.postingmer_txn + atom.postingf_code + atom.postingprod + atom.postingdiscriminator + atom.postinamount + atom.postingbank_txn;

                byte[] bytes = Encoding.UTF8.GetBytes(respHashKey);
                byte[] b = new System.Security.Cryptography.HMACSHA512(bytes).ComputeHash(Encoding.UTF8.GetBytes(strsignature));

                ressignature = byteToHexString(b).ToLower();

                return View("AtomResponseReciever", atom);

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public ActionResult ThankYou()
        //{

        //    return View();
        //}

        //public ActionResult SearchCoupon()
        //{
        //    return RedirectToAction("Cart");
        //}

        [HttpPost]
        public void UpdateCoupon(string dc)
        {
            decimal disc = 0;
            decimal OrigTot = 0;
            decimal amt = 0;

            var CartItems = db.Query<Cart>("Select * from Cart c Where Id= @0", (string)User.Identity.GetUserId()).ToList();
            foreach (var c in CartItems)
            {
                var CouponExist = db.SingleOrDefault<DiscountCoupon>("Select * from DiscountCoupon where CouponCode=@0 and ValidFrom <= @1 and ValidTo >=@1", dc, DateTime.Now);

                var DiscPerc = CouponExist.Perc;
                amt = c.OrigPrice * DiscPerc / 100 ?? 0;
                disc = (decimal)c.OrigPrice - amt;
                OrigTot += disc;
                db.Execute("Update cr Set cr.CouponCode = @0 from Cart as cr " +
                   "INNER join AspNetUsers as au on cr.Id = au.Id WHERE au.Id = @1", dc, (string)User.Identity.GetUserId());

                if (c.ServiceTypeID == (int)ServiceTypeEnum.Accomodation)
                {                 
                    var DiscountCoupon = db.FirstOrDefault<Accomodation>("Select CouponCode from Accomodation where CouponCode=@0", dc);
                    if (DiscountCoupon != null)
                    {
                        db.Execute("Update cr Set cr.DiscountedPrice = @0 from Cart as cr where cr.OrigPrice =@1", disc, c.OrigPrice);

                    }
                }
                if (c.ServiceTypeID == (int)ServiceTypeEnum.Packages || c.ServiceTypeID == (int)ServiceTypeEnum.SightSeeing || c.ServiceTypeID == (int)ServiceTypeEnum.Cruise)
                {
                    var DiscountCoupon = db.FirstOrDefault<Package>("Select CouponCode from Package where CouponCode=@0", dc);
                    if (DiscountCoupon != null)
                    {
                        db.Execute("Update cr Set cr.DiscountedPrice = @0 from Cart as cr where cr.CouponCode =@1", OrigTot, dc);
                    }
                }
                if (c.ServiceTypeID == (int)ServiceTypeEnum.CarBike)
                {
                    var DiscountCoupon = db.FirstOrDefault<CarBike>("Select CouponCode from CarBike where CouponCode=@0", dc);
                    if (DiscountCoupon != null)
                    {
                        if (DiscountCoupon != null)
                        {
                            db.Execute("Update cr Set cr.DiscountedPrice = @0 from Cart as cr where cr.CouponCode =@1", OrigTot, dc);
                        }

                    }
                }
                
            }

            //return RedirectToAction("Cart");
        }



        public ActionResult Delete(int? id)
        {
            db.Execute("Delete from Cart Where CartID=@0", id);
            return RedirectToAction("Cart");
        }

    }
}