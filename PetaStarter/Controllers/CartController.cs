using Microsoft.AspNet.Identity;
using PetaPoco;
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
            var CartItems = db.Query<CartDets>("Select * from Cart c Where Id= @0",(string)User.Identity.GetUserId()).ToList();
            CartItems.ForEach(c => {
                c.Pic = db.ExecuteScalar<string>("Select PictureName From Picture Where ServiceID =@0 and ServiceTypeID =@1 ", c.ServiceID, c.ServiceTypeID);
                
                if (c.ServiceTypeID == (int)ServiceTypeEnum.Accomodation)
                {
                    c.ServiceName = db.ExecuteScalar<string>("Select AccomName From Accomodation where AccomodationID=@0",c.ServiceID);
                   
                }
                if (c.ServiceTypeID == (int)ServiceTypeEnum.Packages|| c.ServiceTypeID == (int)ServiceTypeEnum.Cruise|| c.ServiceTypeID == (int)ServiceTypeEnum.SightSeeing)
                {
                    c.lang = db.Query<LanguageDets>("Select * from GuideLanguage gl inner join Package_Language pl on pl.GuideLanguageID =gl.GuideLanguageID inner join Package p on p.PackageID= pl.PackageID where pl.PackageID=@0",c.ServiceID);
                    c.ServiceName = db.ExecuteScalar<string>("Select PackageName From Package where PackageID=@0", c.ServiceID);
                }
                if (c.ServiceTypeID == (int)ServiceTypeEnum.CarBike)
                {
                    c.ServiceName = db.ExecuteScalar<string>("Select CarBikeName From CarBike where CarBikeID=@0", c.ServiceID);
                }
            });
            return View(CartItems);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(int? ServiceID,int? ServiceTypeID,System.Web.HttpPostedFileBase UploadedFile,DateTime? CheckIn, DateTime? CheckOut,int? nums,int? Qty,string fname,string sname,string email,string phno,int? CustomerID)
        {
            Cart item = new Cart();
            if (ServiceTypeID == (int)ServiceTypeEnum.Accomodation)
            {
                var getItem = db.FirstOrDefault<AccomodationDets>("Select * From Accomodation where AccomodationID=@0", ServiceID);
                getItem.price = db.FirstOrDefault<decimal>("Select Top 1 Price from Prices p inner join OptionType ot on p.OptionTypeID = ot.OptionTypeID Where ServiceID=@0 and ot.ServiceTypeID=@1 and WEF<GetDate() order by WEF desc", ServiceID, ServiceTypeID);
                item = new Cart { Id = User.Identity.GetUserId(), ServiceID = ServiceID, Qty = Qty, CheckIn = CheckIn, CheckOut = CheckOut, NoOfGuest = nums, OrigPrice = getItem.price, ServiceTypeID = ServiceTypeID };
                db.Insert(item);
            }
            if (ServiceTypeID == (int)ServiceTypeEnum.Packages)
            {
                var getItem = db.FirstOrDefault<PackageDets>("Select * From Package where PackageID=@0", ServiceID);
                getItem.price = db.FirstOrDefault<decimal>("Select Top 1 Price from Prices p inner join OptionType ot on p.OptionTypeID = ot.OptionTypeID Where ServiceID=@0 and ot.ServiceTypeID=@1 and WEF<GetDate() order by WEF desc", ServiceID, ServiceTypeID);
                 item = new Cart { Id = User.Identity.GetUserId(), ServiceID = ServiceID, Qty = Qty, CheckIn = CheckIn, CheckOut = CheckOut, NoOfGuest = nums, OrigPrice = getItem.price, ServiceTypeID = ServiceTypeID };
                db.Insert(item);
            }
            if (ServiceTypeID == (int)ServiceTypeEnum.CarBike)
            {
                var getItem = db.FirstOrDefault<CarBikeDets>("Select * From CarBike where CarbikeID=@0", ServiceID);
                getItem.price = db.FirstOrDefault<decimal>("Select Top 1 Price from Prices p inner join OptionType ot on p.OptionTypeID = ot.OptionTypeID Where ServiceID=@0 and ot.ServiceTypeID=@1 and WEF<GetDate() order by WEF desc", ServiceID, ServiceTypeID);
                 item = new Cart { Id = User.Identity.GetUserId(), ServiceID = ServiceID, Qty = Qty, CheckIn = CheckIn, CheckOut = CheckOut, NoOfGuest = nums, OrigPrice = getItem.price, ServiceTypeID = ServiceTypeID };
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

        private string SaveImage(Sql sql, string v, int customerID, HttpPostedFileBase uploadedFile)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult BooKCartItems()
        {
            using (var transaction = db.GetTransaction())
            {
                var CartItems = db.Query<CartDets>("Select * from Cart c Where Id= @0", (string)User.Identity.GetUserId()).ToList();

                try
                {
                    var item = new Booking { BookDate = DateTime.Now, UserID = User.Identity.GetUserId(), StatusID = 1 };
                    db.Insert(item);
                    CartItems.ForEach(c =>
                    {

                        var Bcust = db.FirstOrDefault<BookedCustomer>("select * from BookedCustomer where CartID=@0", c.CartID);
                        if (Bcust!=null && Bcust.CustomerID!= null)
                        {
                           var del= db.Execute($"Delete From BookedCustomer Where BCID={Bcust.BCID}");
                            db.Insert(new BookedCustomer {BookingID=item.BookingID,CustomerID=Bcust.CustomerID });
                          
                        }
                        var bd = new BookingDetail
                        {
                            BookingID = item.BookingID,
                            ServiceID = c.ServiceID,
                            ServiceTypeID = c.ServiceTypeID,
                            OptionTypeID = c.OptionTypeID,
                            Qty = c.Qty,
                            CheckIn = c.CheckIn,
                            CheckOut = c.CheckOut,
                            NoOfGuests = c.NoOfGuest,
                            Price = c.OrigPrice
                        };
                        db.Insert(bd);
                     var delc= db.Execute("Delete From Cart Where CartID=@0", c.CartID);
                    });
                    transaction.Complete();

                }
                catch (Exception ex)
                {
                    db.AbortTransaction();
                    throw ex;
                }
                return RedirectToAction("ThankYou");

            }


        }
        public ActionResult ThankYou()
        {

            return View();
        }
        public ActionResult SearchCoupon(string dc,decimal? tot)
        {
            decimal disc = 0;
            decimal OrigTot = 0;
            decimal amt = 0;
            try
            {
                DiscountCoupon CheckVal = new DiscountCoupon();
                var CartItems = db.Query<CartDets>("Select * from Cart c Where Id= @0", (string)User.Identity.GetUserId()).ToList();
                CartItems.ForEach(c=>{
                    if(c.ServiceTypeID == (int)ServiceTypeEnum.Accomodation)
                    {
                          var IfApplic = db.FirstOrDefault<Accomodation>("Select CouponCode from Accomodation where CouponCode=@0", dc);
                           if (IfApplic != null)
                           {
                            CheckVal = db.First<DiscountCoupon>("Select * from DiscountCoupon where CouponCode=@0 and ValidFrom <= @1 and ValidTo >=@2", dc, DateTime.Now, DateTime.Now);
                                if (CheckVal != null)
                                {
                                    var DiscPerc = CheckVal.Perc;
                                     amt = c.OrigPrice * DiscPerc/ 100 ?? 0;
                                     disc = c.OrigPrice - amt;
                              
                                if (c.CouponCode==null)
                                {
                                    db.Execute($"update Cart Set OrigPrice={disc},CouponCode='{dc}' Where CartID={c.CartID}");
                                }
                                OrigTot += disc;
                            }
                        
                               
                            
                        }
                    }
                    if (c.ServiceTypeID == (int)ServiceTypeEnum.Packages|| c.ServiceTypeID == (int)ServiceTypeEnum.SightSeeing|| c.ServiceTypeID == (int)ServiceTypeEnum.Cruise)
                    {
                        var IfApplic = db.FirstOrDefault<Package>("Select CouponCode from Package where CouponCode=@0", dc);
                        if (IfApplic != null)
                        {
                            CheckVal = db.First<DiscountCoupon>("Select * from DiscountCoupon dc where CouponCode=@0 and ValidFrom<=@1 and ValidTo >=@2", dc, DateTime.Now, DateTime.Now);
                            if (CheckVal != null)
                            {
                                var DiscPerc = CheckVal.Perc;
                                amt = c.OrigPrice * DiscPerc / 100 ?? 0;
                                disc = c.OrigPrice - amt;
                                if (c.CouponCode == null)
                                {
                                    db.Execute($"update Cart Set OrigPrice={disc},CouponCode='{dc}' Where CartID={c.CartID}");
                                }
                                OrigTot += disc;
                            }
                        }
                    }
                    if (c.ServiceTypeID == (int)ServiceTypeEnum.CarBike)
                    {
                        var IfApplic = db.FirstOrDefault<CarBike>("Select CouponCode from CarBike where CouponCode=@0", dc);
                        if (IfApplic != null)
                        {
                            CheckVal = db.First<DiscountCoupon>("Select * from DiscountCoupon where CouponCode=@0 and ValidFrom <= @1 and ValidTo >=@2", dc, DateTime.Now, DateTime.Now);
                            if (CheckVal != null)
                            {
                                var DiscPerc = CheckVal.Perc;
                                amt = c.OrigPrice * DiscPerc / 100 ?? 0;
                                 disc = c.OrigPrice - amt;
                                if (c.CouponCode == null)
                                {
                                    db.Execute($"update Cart Set OrigPrice={disc},CouponCode='{dc}' Where CartID={c.CartID}");
                                }
                                OrigTot += disc;
                            }
                           
                        }
                    }
                });
                ViewBag.Amt = OrigTot;

                return PartialView();
            }
            catch (Exception e)
            {
                ViewBag.Amt = db.ExecuteScalar<decimal>("Select Sum(OrigPrice) as OrigPrice from Cart Where Id=@0", (string)User.Identity.GetUserId());
                Console.WriteLine("{0} Exception caught.", e);
            }
            if (OrigTot != 0)
            {
                ViewBag.Amt = OrigTot;
            }
            return PartialView();

        }
        public ActionResult Delete(int? id)
        {
            db.Execute("Delete from Cart Where CartID=@0",id);
            return RedirectToAction("Cart");
        }

    }
}