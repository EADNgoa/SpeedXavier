using Speedbird.Areas.SBBoss.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Speedbird.Areas.SBBoss.Controllers
{
    public class RefundController : EAController
    {
        // GET: SBBoss/Refund
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult AtomRefundRecieve()
        //{
        //    return View();
        //}

        [HttpPost]
        public async Task<ActionResult> RefundAsync()
        {

            using (var client = new HttpClient())
            {
                var values = new List<KeyValuePair<string, string>>();

                values.Add(new KeyValuePair<string, string>("merchantid", "197"));
                values.Add(new KeyValuePair<string, string>("pwd", "VGVzdEAxMjM="));
                values.Add(new KeyValuePair<string, string>("atomtxnid", "100004487215"));
                values.Add(new KeyValuePair<string, string>("refundamt", "100.00"));
                values.Add(new KeyValuePair<string, string>("txndate", DateTime.Today.ToString("yyyy-MM-dd")));
                values.Add(new KeyValuePair<string, string>("merefundref", "25631"));
                
                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("https://paynetzuat.atomtech.in/paynetz/rfts", content);

                var responseString = await response.Content.ReadAsStringAsync();
            }
            //using (var client = new HttpClient())
            //{

            //    client.BaseAddress = new Uri("https://paynetzuat.atomtech.in/paynetz/rfts");
            //    //HTTP POST
            //    var responseTask = client.PostAsJsonAsync(client.BaseAddress, refundvw);

            //    responseTask.Wait();
            //    var result = responseTask.Result;
            //    ViewBag.Messege = null;
            //    if (result.IsSuccessStatusCode == true)
            //    {
            //        Console.Write("Success");
            //    }
            return View();

        }


            //return View();
            //ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            //return RedirectToAction("ThankYou", "Cart");
    }

        //[HttpPost]
        //public async System.Threading.Tasks.Task<bool> RefundAsync(string url)
        //{

        //    url = "https://paynetzuat.atomtech.in/paynetz/rfts";
        //    string merchantid = "197";
        //string pwd = "Test@123";
        //string atomtxnid = "100004493434";
        //string refundamt = "50.00";
        //string txndate = DateTime.Today.ToString("yyyy-MM-dd");
        //string merefundref = "test123";
        //string strpwd, strpwdEncoded;
        //byte[] b;

        //b = Encoding.UTF8.GetBytes(pwd);
        //    strpwd = Convert.ToBase64String(b);
        //    strpwdEncoded = HttpUtility.UrlEncode(strpwd);

        //    //var refundvw = new Refundvw()
        //    //{
        //    //    merchantid = merchantid,
        //    //    pwd = strpwdEncoded,
        //    //    atomtxnid = atomtxnid,
        //    //    refundamt = refundamt,
        //    //    txndate = txndate,
        //    //    merefundref = merefundref,
        //    //};

        //    IEnumerable<KeyValuePair<string, string>> queries = new List<KeyValuePair<string, string>>();
        //    {
        //        new KeyValuePair<string, string>("merchantid", merchantid);
        //        new KeyValuePair<string, string>("pwd", strpwdEncoded);
        //        new KeyValuePair<string, string>("atomtxnid", atomtxnid);
        //        new KeyValuePair<string, string>("refundamt", refundamt);
        //        new KeyValuePair<string, string>("txndate", txndate);
        //        new KeyValuePair<string, string>("merefundref", merefundref);
        //    };

        //    HttpContent q = new FormUrlEncodedContent(queries);

        //    using (HttpClient client = new HttpClient())
        //    {
        //        using (HttpResponseMessage msg = await client.PostAsJsonAsync(url, q))
        //        {
        //            using (HttpContent content = msg.Content)
        //            {
        //                string mycontent = await content.ReadAsStringAsync();
        //                if (msg.IsSuccessStatusCode == true)
        //                {
        //                    Redirect(url);
        //                    Console.Write("Success");
        //                }
        //            }
        //        }
        //    }

        //    return true;

        //}




        //public class Refundvw
        //{
        //    public string merchantid { get; set; }
        //    public string pwd { get; set; }
        //    public string atomtxnid { get; set; }
        //    public string txndate { get; set; }
        //    public string refundamt { get; set; }
        //    public string merefundref { get; set; }
        //}
    

}   
