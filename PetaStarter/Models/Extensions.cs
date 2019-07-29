using PetaPoco;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace Speedbird
{
    public class Extensions
    {
        
    }

    public static class MyExtensions
    {

        //Extension method for Date
        public static string EAFormat(this DateTime dt, bool withTime = false, bool forSQL = false)
        {
            string strfmt = "";
            strfmt = (forSQL) ? "yyyy/MM/dd" : "dd/MMM/yyyy";
            strfmt += (withTime) ? " hh:mm:ss tt" : "";

            return dt.ToString(strfmt);
        }

        public static string EAFormat(this DateTime? dt, bool withTime = false, bool forSQL = false)
        {
            return dt.HasValue ? ((DateTime)dt).EAFormat(withTime, forSQL) : "";
        }



        //Extension method for decimal
        public static string EAFormat(this decimal dcml, string currency = "")
        {
            return $"{currency} {dcml:#,##0.##}";
        }


        /// <summary>
        /// Return a substring for use in the @Html methods
        /// </summary>
        /// <param name="value">String to trim</param>
        /// <param name="len">Size of desired new string</param>
        /// <returns></returns>
        public static string EAFormat(this string value, int len = 0)
        {
            return (value.Length > len ? value.Substring(0, len) + "..." : value).Trim();
        }

        /// <summary>
        /// Converts a Camel case string to one with Spaces. Used to make Sql field names user friendly
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string CamelToSpaceString(string id)
        {
            return System.Text.RegularExpressions.Regex.Replace(id, "(\\B[A-Z])", " $1");
        }


        public static string getModelStateErrors(ModelStateDictionary mS)
        {
            /// <summary>
            /// Called only when BaseSave cannot be used
            /// </summary>
            /// <param name="mS">Th ModelState to check for errors in</param>
            /// <returns></returns>
            if (!mS.IsValid)
            {
                var modelErrors = new List<string>();
                foreach (var modelState in mS.Values)
                {
                    foreach (var modelError in modelState.Errors)
                    {
                        modelErrors.Add(modelError.ErrorMessage);
                    }
                }
                return String.Join(", ", modelErrors);
            }
            return "No errors found in Model State";
        }

        //called from BaseSave. Internally calls getModelStateErrors
        public static void logModelStateErrors(ModelStateDictionary mS,Repository db, ControllerContext ctxt)
        {
            string actionName = ctxt.RouteData.Values["action"].ToString();
            string controllerName = ctxt.RouteData.Values["controller"].ToString();
            db.Insert(new InsectLog { CritterTime = DateTime.Now, Controller = controllerName, Action = actionName, Message = getModelStateErrors(mS) });
        }

        /// <summary>
        /// Finds if the User has permission for the given action
        /// </summary>
        /// <param name="db">Petapoco repository insance</param>
        /// <param name="FunctionName">Function to be checked for</param>
        /// <param name="Writable">Is write access provided</param>
        /// <param name="CurrUser">The user GUID as string who is to be permitted</param>
        /// <returns></returns>
        public static bool IsPermitted(Repository db, string FunctionName, bool Writable, string CurrUser)
        {
            int Perms;

            Sql sq = new Sql("Select count(1) from AspNetUsers u, UserGroups ug, UserFunctions uf, FunctionGroups fg " +
                    " where u.id = ug.UserID and uf.FunctionID = fg.FunctionID and fg.GroupID = ug.GroupID and u.Id = @0  " +
                    "and uf.FunctionName = @1", CurrUser, FunctionName);

            if (Writable)//We check for writable permissions only if the calling method writes to the DB
                sq.Append(" and fg.Writable = @0", Writable);
            Perms = db.ExecuteScalar<int>(sq);


            return (Perms > 0) ? true : false;
        }

        /// <summary>
        /// Populate the ViewBag with a selectlist for a Year Dropdown
        /// </summary>
        /// <param name="FromYr">No. of years before SelYr</param>
        /// <param name="ToYr">No. of years after SelYr</param>
        /// <param name="SelYr">Default selected year in list (YYYY)</param>
        //public static List<SelectListItem> MakeYrRq(int FromYr, int ToYr, int SelYr)
        //{
        //    List<SelectListItem> slyr = new List<SelectListItem>();
        //    FromYr = SelYr - FromYr;
        //    ToYr += SelYr;
        //    for (int i = FromYr; i <= ToYr; i++)
        //    {
        //        slyr.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString(), Selected = (i == SelYr) });
        //    }
        //    return slyr;
        //}

        /// <summary>
        /// Crop the blank parts out of an image
        /// </summary>
        /// <param name="bmpOriginal"></param>
        /// <returns></returns>
        public static Bitmap CropUnwantedBackground(Bitmap bmpOriginal)
        {
            // Getting The Background Color by checking Corners of Original Image

            Color baseColor = bmpOriginal.GetPixel(0, 0);
            int SameCorners = 0;
            bool hasBaseColor = false;

            Point[] Corners = new Point[]{new Point(0,0),
                new Point(0,bmpOriginal.Height-1),
                new Point(bmpOriginal.Width-1,0),
                new Point(bmpOriginal.Width-1,bmpOriginal.Height-1)};

            for (int i = 0; i < 4; i++)
            {
                SameCorners = 0;
                baseColor = bmpOriginal.GetPixel(Corners[i].X, Corners[i].Y);
                for (int j = 0; j < 4; j++)
                {
                    Color CornerColor = bmpOriginal.GetPixel(Corners[j].X, Corners[j].Y);
                    if ((CornerColor.R <= baseColor.R * 1.1 && CornerColor.R >= baseColor.R * 0.9) &&
                        (CornerColor.G <= baseColor.G * 1.1 && CornerColor.G >= baseColor.G * 0.9) &&
                        (CornerColor.B <= baseColor.B * 1.1 && CornerColor.B >= baseColor.B * 0.9))
                    {
                        SameCorners++;
                    }
                }
                if (SameCorners > 2)
                {
                    hasBaseColor = true;
                    break;
                }
            }

            //--------------------------------------------------------------------
            // Finding the Bounds of Crop Area bu using Unsafe Code and Image Proccesing
            // keep in mind in BitmapData Pixel Format is BGR NOT RGB !!!!!!

            if (hasBaseColor)
            {

                int width = bmpOriginal.Width, height = bmpOriginal.Height;
                bool UpperLeftPointFounded = false;
                Point[] bounds = new Point[2];
                Color c;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        c = bmpOriginal.GetPixel(x, y);
                        bool sameAsBackColor = ((c.R <= baseColor.R * 1.1 && c.R >= baseColor.R * 0.9) &&
                                                (c.G <= baseColor.G * 1.1 && c.G >= baseColor.G * 0.9) &&
                                                (c.B <= baseColor.B * 1.1 && c.B >= baseColor.B * 0.9));
                        if (!sameAsBackColor)
                        {
                            if (!UpperLeftPointFounded)
                            {
                                bounds[0] = new Point(x, y);
                                bounds[1] = new Point(x, y);
                                UpperLeftPointFounded = true;
                            }
                            else
                            {
                                if (x > bounds[1].X)
                                    bounds[1].X = x;
                                else if (x < bounds[0].X)
                                    bounds[0].X = x;
                                if (y >= bounds[1].Y)
                                    bounds[1].Y = y;
                            }
                        }
                    }
                }
                Bitmap result = new Bitmap(bounds[1].X - bounds[0].X + 1, bounds[1].Y - bounds[0].Y + 1);
                Graphics g = Graphics.FromImage(result);
                g.DrawImage(bmpOriginal, new Rectangle(0, 0, result.Width, result.Height), new Rectangle(bounds[0].X, bounds[0].Y, bounds[1].X - bounds[0].X + 1, bounds[1].Y - bounds[0].Y + 1), GraphicsUnit.Pixel);
                return result;
            }
            else
            {
                return bmpOriginal;
            }
        }


        public static IEnumerable<System.Web.Mvc.SelectListItem> MonthList()
        {
            return System.Globalization.DateTimeFormatInfo
                       .InvariantInfo
                       .MonthNames
                       .Select((monthName, index) => new System.Web.Mvc.SelectListItem
                       {
                           Value = (index + 1).ToString(),
                           Text = monthName
                       });
        }

        public static string MonthFromInt(int mon)
        {
            return System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(mon).ToString();
        }

        public static string CamelToSentence(string InStr)
        {
            return System.Text.RegularExpressions.Regex.Replace(InStr, "(\\B[A-Z])", " $1");
        }

        public static string gethtm(string r, int co,int ro, ref int[,] cls, ref int[,] rws)
        {
            if (r != null)
            {
                if (r.Substring(0, 1) == "O")
                {
                    rws[co, 0]++;
                    cls[ro, 0]++;
                }
                else
                {
                    rws[co, 1]++;
                    cls[ro, 1]++;
                }

            }
            return r ?? "";
        }
        
    }

    public class ChangeNumbersToWords
    {
        public String changeToWords(String numb)
        {
            String val = "", wholeNo = numb, points = "", andStr = "";
            String endStr = ("");
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        andStr = ("point ");// just to separate whole numbers from points/Rupees

                    }
                }
                val = String.Format("{0} {1}{2} {3}", translateWholeNumber(wholeNo).Trim(), andStr, tens(points.Substring(0,2)), endStr);
            }
            catch
            {
                ;
            }
            return val;
        }

        private String translateWholeNumber(String number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX
                bool isDone = false;//test if already translated
                double dblAmt = (Convert.ToDouble(number));
                //if ((dblAmt > 0) && number.StartsWith("0"))

                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric
                    beginsZero = number.StartsWith("0");
                    int numDigits = number.Length;
                    int pos = 0;//store digit grouping
                    String place = "";//digit grouping name:hundres,thousand,etc...
                    switch (numDigits)
                    {
                        case 1://ones' range
                            word = ones(number);
                            isDone = true;
                            break;
                        case 2://tens' range
                            word = tens(number);
                            isDone = true;
                            break;
                        case 3://hundreds' range
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4://thousands' range
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 7://millions' range
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range
                            pos = (numDigits % 10) + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)
                        word = translateWholeNumber(number.Substring(0, pos)) + place + translateWholeNumber(number.Substring(pos));
                        //check for trailing zeros
                        if (beginsZero) word = " and " + word.Trim();
                    }
                    //ignore digit grouping names
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch
            {
                ;
            }
            return word.Trim();
        }

        private String tens(String digit)
        {
            int digt = Convert.ToInt32(digit);
            String name = null;
            switch (digt)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Fourty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (digt > 0)
                    {
                        name = tens(digit.Substring(0, 1) + "0") + " " + ones(digit.Substring(1));
                    }
                    break;
            }
            return name;
        }

        private String ones(String digit)
        {
            int digt = Convert.ToInt32(digit);
            String name = "";
            switch (digt)
            {
                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }


    }

    public static class IdentityExtensions
    {
        public static string GetRealName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("RealName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
    }

    //Read at https://blogs.msdn.microsoft.com/stuartleeks/2012/04/23/asp-net-mvc-jquery-ui-autocomplete/
    //@Html.LabelFor(m=>m.SomeValue) 
    //@Html.AutocompleteFor(m=>m.SomeValue, “Autocomplete”, “Home”)
    //public static class AutocompleteHelpers
    //{
    //    public static MvcHtmlString AutocompleteFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string actionName, string controllerName)
    //    {
    //        string autocompleteUrl = UrlHelper.GenerateUrl(null, actionName, controllerName,
    //        null,
    //        html.RouteCollection,
    //        html.ViewContext.RequestContext,
    //        includeImplicitMvcValues: true);
    //        return html.TextBoxFor(expression, new { data_autocomplete_url = autocompleteUrl });
    //    }
    //}
}