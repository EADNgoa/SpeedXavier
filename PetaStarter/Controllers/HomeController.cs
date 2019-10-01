using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speedbird.Controllers
{
    public class HomeController : EAController
    {
        public ActionResult Index()
        {
            var cat = db.Fetch<CategoryRec>("Select Top 3 * from Category Order By NewID()");
            cat.ForEach(c =>
            {
                c.pack = db.Fetch<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID inner join Package_Category pc on a.PackageID = pc.PackageID inner join Category c on c.CategoryID = pc.CategoryID where pc.CategoryID={c.CategoryID} Order By NewID()").ToList();
            });
            return View(cat);
        }

        ///Activities(partial view of index)
        public ActionResult IndexActPartial()
        {
            var Act = db.Fetch<ActivityRec>("Select Top 3 * from Activity Order By NewID()");
            Act.ForEach(c =>
            {
                c.pack = db.Fetch<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID inner join Package_Activity pa on a.PackageID = pa.PackageID inner join Activity ac on ac.ActivityID = pa.ActivityID where pa.ActivityID={c.ActivityID} Order By NewID()").ToList();
            });
            return PartialView(Act);
        }
        ///Attraction(Partial view of index)
        public ActionResult IndexAttractPartial()
        {
            var Act = db.Fetch<AttractRec>("Select Top 3 * from Attraaction Order By NewID()");
            Act.ForEach(c =>
            {
                c.pack = db.Fetch<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID inner join Package_Attraction pa on a.PackageID = pa.PackageID inner join Attraaction ac on ac.AttractionID = pa.AttractionID where pa.AttractionID={c.AttractionID} Order By NewID()").ToList();
            });
            return PartialView(Act);
        }

        public ActionResult LoadService(int? Goa,int? India,int? World)
        {
            if (Goa != null)
            {
                ViewBag.loc = "Goa";
            }
            else if (India != null)
            {
                ViewBag.loc = "India";
            }
            else if (World != null)
            {
                ViewBag.loc = "International";
            }
          
            ViewBag.Accomodation = db.FirstOrDefault<AccomodationDets>("Select Top 1 * From Accomodation a inner join Picture p on a.AccomodationID = p.ServiceID Order By NewID()");
            ViewBag.Package = db.FirstOrDefault<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID where a.ServiceTypeID={(int)ServiceTypeEnum.Packages} Order By NewID()");
            ViewBag.SightSeeing = db.FirstOrDefault<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID where a.ServiceTypeID={(int)ServiceTypeEnum.SightSeeing} Order By NewID()");
            ViewBag.Cruise = db.FirstOrDefault<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID where a.ServiceTypeID={(int)ServiceTypeEnum.Cruise} Order By NewID()");
            ViewBag.CarBike = db.FirstOrDefault<CarBikeDets>($"Select Top 1 * From CarBike a inner join Picture p on a.CarBikeID = p.ServiceID ");
            var cat = db.Fetch<CategoryRec>("Select * from Category");
            cat.ForEach(c=>
            {
                c.pack = db.Fetch<PackageDets>($"Select Top 1 * From Package a inner join Picture p on a.PackageID = p.ServiceID inner join Package_Category pc on a.PackageID = pc.PackageID inner join Category c on c.CategoryID = pc.CategoryID where pc.CategoryID={c.CategoryID} Order By NewID()").ToList(); 
            });
            return View(cat);
        }

        public ActionResult LoadPackages(ServiceTypeEnum? st,string Loc,int? CatID,int LocId=0)
        {
            ViewBag.st = st;
            
            ViewBag.GuideLanguageID = new SelectList(db.Fetch<GuideLanguage>("Select GuideLanguageID,GuideLanguageName from GuideLanguage"), "GuideLanguageID", "GuideLanguageName");

            List<SelectListItem> items = new List<SelectListItem>();
            for(int i = 1; i <= 10; i++)
            {
                items.Add(new SelectListItem { Text = ""+i, Value= ""+i });
            }
            List<SelectListItem> tf = new List<SelectListItem>
            {
                new SelectListItem { Text = "YES", Value = "1" },
                new SelectListItem { Text = "NO", Value = "0" }
            };




            ViewBag.TF = tf;
            ViewBag.nums = items;

            if (LocId==0)
                ViewBag.LocId = db.FirstOrDefault<GeoTree>("Select * from GeoTree where GeoName = @0", Loc).GeoTreeID;
            else 
                ViewBag.LocId = LocId;

            if (st == ServiceTypeEnum.Packages || st == ServiceTypeEnum.SightSeeing || st == ServiceTypeEnum.Cruise)
            {
                ViewBag.Cats = db.Fetch<Category>("Select * From Category");
                ViewBag.Acts = db.Fetch<Activity>("Select * From Activity");
                ViewBag.Attracts = db.Fetch<Attraaction>("Select Distinct a.* From Attraaction a inner join Package_Attraction pa on pa.AttractionID = a.AttractionID inner join Package p on p.PackageID =pa.PackageID inner join Package_GeoTree pg on pg.PackageID= p.PackageID where pg.GeoTreeID in(Select GeotreeId from GetChildGeos(@0))",ViewBag.LocId);

            }
            if (st == ServiceTypeEnum.Accomodation)
            {
                ViewBag.fac = db.Fetch<Facility>("Select * From Facility");             
            }
            return View();
        }


        public ActionResult PackagesPartialView(ServiceTypeEnum? st, IEnumerable<int> CatID, IEnumerable<int> ActID,int? Gsize,int? diff,int? dur,int? GuideLanguageID, IEnumerable<int> AttractID,IEnumerable<int> FacilityID,decimal? maxPrice,decimal? minPrice,int? NoPax,int? large,int? small,int? hasAc,int? HasCarrier,IEnumerable<int> DestIds,int? IsBike)
        {            
            
            string Dests = "";
            if (DestIds != null)          
                Dests = string.Join(",", DestIds.ToArray());

            PetaPoco.Sql MainSql = new PetaPoco.Sql();
            PetaPoco.Sql FromSql = new PetaPoco.Sql();
            PetaPoco.Sql WhereSql = new PetaPoco.Sql();

            ViewBag.st = st;
            if (st == ServiceTypeEnum.Accomodation)
            {
                ViewBag.ServiceTitle = "Accomodations";
                MainSql = new PetaPoco.Sql(" Select a.AccomodationID as ServiceId,@0 as ServiceTypeId, " +
                    "a.AccomName as ServiceName, substring(a.Description, 0, 100) + '...' as ServiceDescription," +
                    "g.geoName as ServiceGeoName, min(pr.price) as price ", (int) ServiceTypeEnum.Accomodation);
                FromSql = new PetaPoco.Sql("from Accomodation a " +
                    $"left join GeoTree g on a.geoTreeId = g.GeoTreeId and a.GeoTreeID in (SELECT GeoTreeID FROM STRING_SPLIT('{Dests}', ',') CROSS APPLY dbo.GetChildGeos(value))" +
                    "left join Prices pr on pr.PriceID = (select top 1 PriceID from Prices where a.AccomodationID = Prices.ServiceID  and Prices.WEF < GetDate() order by Prices.WEF desc) " +
                    "left join OptionType ot on ot.OptionTypeID = pr.OptionTypeID " +
                    "left join Facility_Accomodation fa on a.AccomodationID = fa.AccomodationID " +
                    "left join Facility f on f.FacilityID = fa.FacilityID ");
                WhereSql = new PetaPoco.Sql($"where ot.ServiceTypeID=@0 ", ServiceTypeEnum.Accomodation);
                if (maxPrice != null && minPrice != null)
                {                
                    WhereSql.Append(" and Price Between @0 and @1" , minPrice, maxPrice);
                }
                if(FacilityID != null)
                {
                    WhereSql.Append(" and fa.FacilityID in (@0)",FacilityID.ToArray());
                }
             
                MainSql.Append(FromSql);
                MainSql.Append(WhereSql);
                MainSql.Append(" Group by a.AccomodationID, a.AccomName, [Description], g.geoName  ");
                var accom = db.Query<AccomodationDets>(MainSql);
                accom = accom.OrderBy(a => a.AccomName);
                
                
            }
            if (st == ServiceTypeEnum.CarBike)
            {
                ViewBag.ServiceTitle = "Car And Bikes Rental";
                MainSql = new PetaPoco.Sql("Select [CarBikeID] as ServiceId, [CarBikeName] as ServiceName,@0 as ServiceTypeId, g.geoName as ServiceGeoName, " +
                    "substring(description, 0, 100) + '...' as ServiceDescription, min(pr.price) as price ", (int) ServiceTypeEnum.CarBike);
                FromSql = new PetaPoco.Sql("From CarBike c left join GeoTree g on c.geoTreeId=c.GeoTreeId " +
                    "left join Prices pr on pr.PriceID = (select top 1 PriceID from Prices where c.CarBikeID = Prices.ServiceID  and Prices.WEF < GetDate() order by Prices.WEF desc) " +
                    "left join OptionType ot on ot.OptionTypeID = pr.OptionTypeID ");
                WhereSql = new PetaPoco.Sql($"where ot.ServiceTypeID=@0 and c.GeoTreeID in (SELECT GeoTreeID FROM STRING_SPLIT('{Dests}', ',') CROSS APPLY dbo.GetChildGeos(value)) ",  ServiceTypeEnum.CarBike);
                if (maxPrice != null && minPrice != null)
                {                    
                    WhereSql.Append(" and Price Between @0 and @1 ", minPrice, maxPrice);
                }
                if (NoPax != null)
                {
                    WhereSql.Append(" and c.NoPax<=@0", NoPax);                    
                }
                if (large != null)
                {
                    WhereSql.Append(" and c.NoLargeBags<=@0", large);                    
                }
                if (small != null)
                {
                    WhereSql.Append(" and c.NoSmallBags<=@0", small);                    
                }
                if (hasAc!= null)
                {
                    WhereSql.Append(" and c.HasAc=@0", hasAc);                    
                }
                
                if (HasCarrier!=null)
                {
                    WhereSql.Append(" and c.Hascarrier=@0", HasCarrier);                    
                }
                if (IsBike != null)
                {
                    WhereSql.Append(" and c.IsBike=@0", IsBike);
                }
                MainSql.Append(FromSql);
                MainSql.Append(WhereSql);
                MainSql.Append(" Group by c.CarBikeID, c.CarBikeName, [Description], g.geoName  ");

            }
            if (st == ServiceTypeEnum.Packages || st == ServiceTypeEnum.SightSeeing || st == ServiceTypeEnum.Cruise)
            {
                ViewBag.ServiceTitle = "Our Best Tours And Excursions";


                MainSql = new PetaPoco.Sql("Select Count(rv.ReviewID) as TotalReview,Avg(rv.Value) as AvgReview," +
                    " pv.PackageID as ServiceId,pv.ServiceTypeID ,pv.PackageName as ServiceName, " +
                    "substring(pv.Description, 0, 100) + '...' as ServiceDescription, min(coalesce(pr.price, 0)) as price ");
                FromSql = new PetaPoco.Sql("from Package pv left join Review rv on rv.ServiceID = pv.PackageID " +
                    "left join Prices pr on pr.PriceID = (select top 1 PriceID from Prices " +
                    "where pv.PackageID = Prices.ServiceID and Prices.WEF < GetDate() order by Prices.WEF desc) " +
                    "left join OptionType ot on ot.OptionTypeID = pr.OptionTypeID " +
                    "left join Package_Category pc on pc.PackageID = pv.PackageID " +
                    "left join Category c on c.CategoryID = pc.CategoryID " +
                    "left join Package_Activity pa on pa.PackageID = pv.PackageID " +
                    "left join Activity a on a.ActivityID = pa.ActivityID " +
                    "left join Package_Attraction pat on pat.PackageID = pv.PackageID " +
                    "left join Attraaction at on at.AttractionID = pat.AttractionID " +
                    "left join Package_Language pl on pl.PackageId = pv.PackageID " +
                    "left join GuideLanguage gl on gl.GuideLanguageID = pl.GuideLanguageId " +
                    "left join Package_GeoTree pg on pg.PackageID = pv.PackageID");
                WhereSql = new PetaPoco.Sql(" where pv.ServiceTypeID=@0", (int)st);
                //WhereSql.Append(" and p.PackageID = pr.ServiceID and ot.OptionTypeID = pr.OptionTypeID  and pr.PriceID = (select top 1 PriceID from Prices where p.PackageID = Prices.ServiceID and ot.OptionTypeID = Prices.OptionTypeID  and Prices.WEF<GetDate() order by Prices.WEF desc) ");
                if (maxPrice!=null && minPrice != null)
                {
                    WhereSql.Append(" and Price Between @0 and @1 ", minPrice,maxPrice);
                }
                if (CatID != null)
                {
                    WhereSql.Append(" and pc.CategoryID in (@0)",  CatID.ToArray());
                }

                if (ActID != null)
                {
                    WhereSql.Append(" and pa.ActivityID in (@0)",  ActID.ToArray());
                }
                if (AttractID != null)
                {
                    WhereSql.Append(" and pat.AttractionID in (@0)", AttractID.ToArray());
                }
                if (Gsize!=null)
                {
                    WhereSql.Append(" and pv.GroupSize <= @0",Gsize);
                }
                if (diff != null)
                {
                    WhereSql.Append(" and pv.Dificulty <= @0", diff);
                }
                if (dur != null)
                {
                    WhereSql.Append(" and pv.Duration <= @0", dur);
                }
                if (GuideLanguageID != null)
                {
                    WhereSql.Append(" and pl.GuideLanguageId =@0 ", GuideLanguageID);
                }

                //Filter for destination
                WhereSql.Append($" and pg.GeoTreeId in (SELECT GeoTreeID FROM STRING_SPLIT('{Dests}', ',') CROSS APPLY dbo.GetChildGeos(value))");


                MainSql.Append(FromSql);
                MainSql.Append(WhereSql);
                MainSql.Append(" Group by pv.PackageID, pv.ServiceTypeID,pv.PackageName, pv.[Description] ");
            }

            MainSql.Append(" order by min(pr.price)");
            var apc = db.Fetch<AccomPackCarBike>(MainSql);

            foreach (var i in apc)
            {
                i.ServicePic = db.FirstOrDefault<PictureDets>("Select Top 1 * From Picture Where ServiceID=@0 and ServiceTypeID=@1", i.ServiceID, i.ServiceTypeID)?.PictureName ?? "";
                if (string.IsNullOrWhiteSpace(i.ServiceGeoName)) i.ServiceGeoName = db.First<string>("Select GeoName From GeoTree g,Package_GeoTree pg  where pg.PackageID=@0 and g.GeoTreeID = pg.GeoTreeID", i.ServiceID);
                i.Attributes = db.Fetch<Attribute>("Select * from Attribute a, Package_attribute pa where a.attributeID=pa.attributeID and pa.packageID=@0 and pa.serviceTypeId=@1", i.ServiceID, i.ServiceTypeID);
                i.Icons = db.Fetch<string>("select IconPath from Icons where ServiceId=@0 and ServiceTypeId=@1", i.ServiceID, i.ServiceTypeID);
            }
            
            return PartialView(apc);


        }

        public ActionResult InfoPage(int? ServiceID, ServiceTypeEnum? st, string Comment,int? jqStar)
        {
            if (Comment != null) db.Insert(new Review {_Review=Comment,UserID=User.Identity.GetUserId(),ServiceID=ServiceID,ServiceTypeID=(int)st,ReviewDate=DateTime.Now,IsVisible=true,Value=jqStar });
            var CartItems = db.Query<CartDets>("Select * from Cart c Where Id= @0", (string)User.Identity.GetUserId()).ToList();

            bool exec = true;
            CartItems.ForEach(c=>
            {
                if (exec == true)
                {
                    var cid = db.ExecuteScalar<int>("Select CustomerID From BookedCustomer where CartID=@0", c.CartID);
                    if (cid != 0)
                    {
                        exec = false;
                        ViewBag.CustRec = db.First<Customer>("Select * from Customer where CustomerID=@0", cid) ?? null;
                    }
                }
            });
            string chk = db.ExecuteScalar<string>("Select b.UserID From ServiceRequest b inner join SRdetails bd on bd.SRID =b.SRID Where b.UserID=@0 and bd.ItemId=@1 and bd.ServiceTypeID=@2", User.Identity.GetUserId(),ServiceID,st);
            if(chk != null)
            {
                ViewBag.checkBooking = chk;
            }
            var reviews = db.Query<ReviewDets>("Select * from Review r inner join AspNetUsers anu on anu.Id=r.UserID where ServiceID=@0 and ServiceTypeID=@1 and IsVisible = @2",ServiceID,(int)st,true).ToList();
            reviews.ForEach(r=>
            {
                r.Replies = db.Query<ReviewRepDets>("Select * from ReviewReplies rr where ReviewID= @0 and IsVisible =@1", r.ReviewID,true).ToList();
            });
            ViewBag.ST = st;
            ViewBag.ServiceID = ServiceID;
            List<SelectListItem> items = new List<SelectListItem>();
            for (int i=1;i<=10;i++)
            {
                items.Add(new SelectListItem { Text = ""+i, Value = "" + i });
            }
            ViewBag.nums = items;

            if(((int)ViewBag.ST) == (int)ServiceTypeEnum.Packages || ((int)ViewBag.ST) == (int)ServiceTypeEnum.Cruise || ((int)ViewBag.ST) == (int)ServiceTypeEnum.SightSeeing)
            {
                var Packages = db.FirstOrDefault<string>("select StartTime From Package  Where PackageID =@0 and ServiceTypeID=@1", ServiceID, st);
                ViewBag.Glang = db.Fetch<GuideLanguage>("Select g.* from GuideLanguage g  inner join Package_Language pl on pl.GuideLanguageID= g.GuideLanguageID where pl.PackageID=@0", ServiceID.Value).Select(a => new SelectListItem { Text=a.GuideLanguageName, Value=a.GuideLanguageID.ToString()});
                ViewBag.Gtime = Packages.Split(',').Select(a => new SelectListItem { Text=a,Value=a } );
                //ViewBag.SLoc = db.First<GeoTree>("Select * from GeoTree g, Package_Geotree pg where g.GeotreeId=pg.geotreeId and pg.packageId=@0", ServiceID).GeoName;
            }

            ViewBag.HeadPic = db.FirstOrDefault<Picture>("Select top 1 * from Picture where ServiceId=@0 and ServiceTypeId=@1", ServiceID, st)?? new Picture { PictureName="Goa" };
                return View(reviews);
        }
        public ActionResult InfoAccomPartial(int? ServiceID, ServiceTypeEnum? st)
        {
            var accom = db.Query<AccomodationDets>("select * From Accomodation  Where AccomodationID =@0", ServiceID).ToList().FirstOrDefault();
            accom.pic = db.Fetch<PictureDets>("Select * From Picture Where ServiceID=@0 and ServiceTypeID=@1 Order By NewID()", accom.AccomodationID, (int)ServiceTypeEnum.Accomodation).ToList();
            accom.GeoName = db.First<string>("Select GeoName From GeoTree  where GeoTreeID=@0", accom.GeoTreeID);
            ViewBag.Icns = db.Query<string>("Select Iconpath from Icons where ServiceTypeId=@0 and ServiceId=@1 ", st, ServiceID);

            try
            {
                ViewBag.facs = db.Fetch<Facility>("Select f.* From facility f, Facility_Accomodation a where f.facilityID= a.facilityID and  a.AccomodationID=@0 ", ServiceID);
            }
            catch (Exception e)
            {

                throw;
            }
            
            return PartialView(accom);
        }
        
        public ActionResult InfoCarBikePartial(int? ServiceID, ServiceTypeEnum? st)
        {
            var CarBike = db.FirstOrDefault<CarBikeDets>("select * From CarBike  Where CarBikeID =@0", ServiceID);
            CarBike.pic = db.Fetch<PictureDets>("Select * From Picture Where ServiceID=@0 and ServiceTypeID=@1 Order By NewID()", CarBike.CarBikeID, (int)ServiceTypeEnum.CarBike).ToList();
            CarBike.GeoName = db.First<string>("Select GeoName From GeoTree  where GeoTreeID=@0", CarBike.GeoTreeID);
            ViewBag.Icns = db.Query<string>("Select Iconpath from Icons where ServiceTypeId=@0 and ServiceId=@1 ", st, ServiceID);

            return PartialView(CarBike);
        }

        public ActionResult InfoPackagePartial(int? ServiceID, ServiceTypeEnum? st)
        {
            var Packages = db.FirstOrDefault<PackageDets>("select * From Package  Where PackageID =@0 and ServiceTypeID=@1", ServiceID, st);
            Packages.Pic = db.Fetch<PictureDets>("Select * From Picture Where ServiceID=@0 and ServiceTypeID=@1 Order By NewID()", Packages.PackageID, st).ToList();
            Packages.GeoNames = db.Fetch<string>("Select GeoName From GeoTree g,Package_GeoTree pg  where pg.PackageID=@0 and g.GeoTreeID = pg.GeoTreeID", Packages.PackageID);
            Packages.GuideLanguages = db.Fetch<GuideLanguage>("Select g.* from GuideLanguage g inner join Package_Language pl on pl.GuideLanguageID= g.GuideLanguageID where pl.PackageID=@0", ServiceID.Value);
            ViewBag.Icns = db.Query<string>("Select Iconpath from Icons where ServiceTypeId=@0 and ServiceId=@1 ", st, ServiceID);
            
            return PartialView(Packages);
        }
        public ActionResult InfoPackCatPartial(int? ServiceID)
        {
            int GeoTreeID = db.ExecuteScalar<int>("Select GeoTreeID From Package_GeoTree where PackageID=@0 ", ServiceID);

            int CatID = db.ExecuteScalar<int>("Select CategoryID From Package_Category where PackageID=@0 ", ServiceID);
            var Similar = db.Query<PackageDets>("Select * From Package p inner join Package_Category pc on p.PackageID= pc.PackageID inner join Category c on c.CategoryID = pc.CategoryID inner join Package_GeoTreeID pg on p.PackageID = pg.PackageID inner join GeoTree g on g.GeoTreeID = pg.GeoTreeID where pc.CategoryID=@0 and pg.GeoTreeID in(Select GeotreeId from GetChildGeos(@0)  ", CatID, GeoTreeID).ToList();
            Similar.ForEach(a =>
            {
                a.Pic = db.Fetch<PictureDets>("Select Top 1 * From Picture Where ServiceID=@0 and ServiceTypeID=@1 Order By NewID()", a.PackageID, (int)Speedbird.ServiceTypeEnum.Packages).ToList();
                a.GeoName = db.First<string>("Select GeoName From GeoTree g,Package_GeoTree pg  where pg.PackageID=@0 and g.GeoTreeID = pg.GeoTreeID", a.PackageID);
            });
            return PartialView(Similar);
        }
        public ActionResult InfoPackActPartial(int? ServiceID)
        {
            int GeoTreeID = db.ExecuteScalar<int>("Select GeoTreeID From Package_GeoTree where PackageID=@0 ", ServiceID);
            int ActID = db.ExecuteScalar<int>("Select ActivityID From Package_Activity where PackageID=@0 ", ServiceID);
            var Similar = db.Query<PackageDets>("Select * From Package p inner join Package_Activity pa on p.PackageID= pa.PackageID inner join Activity a on a.ActivityID = pa.ActivityID inner join Package_GeoTree pg on pg.PackageID = p.PackageID inner join GeoTree g on g.GeoTreeID = pg.GeoTreeID where pa.ActivityID   =@0 and pg.GeoTreeID in(Select GeotreeId from GetChildGeos(@0) ", ActID, GeoTreeID).ToList();
            Similar.ForEach(a =>
            {
                a.GeoName = db.First<string>("Select GeoName From GeoTree g,Package_GeoTree pg  where pg.PackageID=@0 and g.GeoTreeID = pg.GeoTreeID", a.PackageID);
                a.ActivityName = db.First<string>("Select ActivityName From Activity a,Package_Activity pa where pa.PackageID =@0 and a.ActivityID = pa.ActivityID");
                a.PictureName = db.First<string>("Select Top 1 * From Picture Where ServiceID =@0 and ServiceTypeID =@1 Order By NewID()", a.PackageID, (int)Speedbird.ServiceTypeEnum.Packages);

            });
            return PartialView(Similar);
        }
        public ActionResult InfoPackAttractPartial(int? ServiceID)
        {
            int GeoTreeID =db.ExecuteScalar<int>("Select GeoTreeID From Package_GeoTree where PackageID=@0 ", ServiceID);

            int ActID = db.ExecuteScalar<int>("Select AttractionID From Package_Attraction where PackageID=@0 ", ServiceID);
            var Similar = db.Query<PackageDets>("Select Top 5 * From Package p inner join Package_Attraction pa on p.PackageID= pa.PackageID inner join Attraaction a on a.AttractionID = pa.AttractionID inner join Package_GeoTree pg on pg.PackageID = p.PackageID inner join GeoTree g on g.GeoTreeID = pg.GeoTreeID where pa.AttractionID=@0 and pg.GeoTreeID in(Select GeotreeId from GetChildGeos(@1)) and ServiceTypeID=@2 and p.PackageID !=(@3) Order By NewID()", ActID, GeoTreeID,(int)ServiceTypeEnum.Packages,ServiceID).ToList();
            Similar.ForEach(a =>
            {
                a.GeoName = db.First<string>("Select GeoName From GeoTree g,Package_GeoTree pg  where pg.PackageID=@0 and g.GeoTreeID = pg.GeoTreeID", a.PackageID);
                a.AttractionName = db.First<string>("Select AttractionName From Attraaction a,Package_Attraction pa where pa.PackageID =@0 and a.AttractionID = pa.AttractionID",a.PackageID);
                a.PictureName = db.ExecuteScalar<string>("Select PictureName From Picture Where ServiceID =@0 and ServiceTypeID =@1 ", a.PackageID, (int)Speedbird.ServiceTypeEnum.Packages);

            });
            return PartialView("PackageAttractPartial",Similar);
        }
        public ActionResult GetPrice(int? ServiceID,int? st)
        {
            ViewBag.Price = db.First<decimal>("Select Top 1 Price from Prices p inner join OptionType ot on p.OptionTypeID = ot.OptionTypeID Where ServiceID=@0 and ot.ServiceTypeID=@1 and WEF<GetDate() order by WEF desc", ServiceID, st);

            return PartialView();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}