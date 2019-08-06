using Speedbird.Areas.SBBoss.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

        //[HttpPost]
        //public ActionResult Indexs(FormCollection formCollection)
        //{

        //    string strpwd, strpwdEncoded;
        //    byte[] b;

        //    UriBuilder Url = new UriBuilder("https://paynetzuat.atomtech.in/paynetz/rfts");
        //    string merchantid = "197";
        //    string pwd = "Test@123";
        //    string atomtxnid = "100004489745";
        //    string refundamt = "150.00";
        //    string txndate = DateTime.Today.ToString("yyyy-MM-dd");
        //    string merefundref = "test123";

        //    b = Encoding.UTF8.GetBytes(pwd);
        //    strpwd = Convert.ToBase64String(b);
        //    strpwdEncoded = HttpUtility.UrlEncode(strpwd);

        //    Url.Query = $"merchantid={merchantid}&pwd={strpwdEncoded}&atomtxnid={atomtxnid}" +
        //        $"&refundamt={refundamt}&txndate={txndate}" +
        //        $"&merefundref={merefundref}";
        //    return Redirect(Url.ToString());
        //}


        //[HttpPost]
        //public async System.Threading.Tasks.Task<ActionResult> IndexsAsync(string url)
        //{

        //string merchantid = "197";
        //string pwd = "Test@123";
        //string atomtxnid = "100004489745";
        //string refundamt = "200.00";
        //string txndate = DateTime.Today.ToString("yyyy-MM-dd");
        //string merefundref = "test123";
        //string strpwd, strpwdEncoded;
        //byte[] b;

        //b = Encoding.UTF8.GetBytes(pwd);
        //strpwd = Convert.ToBase64String(b);
        //strpwdEncoded = HttpUtility.UrlEncode(strpwd);
        //}
        //IEnumerable<KeyValuePair<string, string>> queries = new List<KeyValuePair<string, string>>();
        //{
        //    new KeyValuePair<string, string>("merchantid", "197");
        //    new KeyValuePair<string, string>("pwd", "Test@123");
        //    new KeyValuePair<string, string>("atomtxnid", "100004489745");
        //    new KeyValuePair<string, string>("refundamt", "200.00");
        //    new KeyValuePair<string, string>("txndate", "2019-08-02");
        //    new KeyValuePair<string, string>("merefundref", "test123");
        //};

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> PostRequestAsync(string url)
        {
            url = "https://paynetzuat.atomtech.in/paynetz/rfts";

            string merchantid = "197";
            string pwd = "Test@123";
            string atomtxnid = "100004489745";
            string refundamt = "200.00";
            string txndate = DateTime.Today.ToString("yyyy-MM-dd");
            string merefundref = "test123";
            string strpwd, strpwdEncoded;
            byte[] b;

            b = Encoding.UTF8.GetBytes(pwd);
            strpwd = Convert.ToBase64String(b);
            strpwdEncoded = HttpUtility.UrlEncode(strpwd);

            List<string> queries = new List<string>();
            {
                queries.Add(merchantid);
                queries.Add(strpwdEncoded);
                queries.Add(atomtxnid);
                queries.Add(refundamt);
                queries.Add(txndate);
                queries.Add(merefundref);
            }

            HttpContent q = new FormUrlEncodedContent(queries);
            HttpContent q = new StringContent(queries.ToString());
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.PostAsync(url, q))
                {
                    using (HttpContent content = response.Content)
                    {
                        string mycontent = await content.ReadAsStringAsync();
                        HttpContentHeaders headers = content.Headers;
                        return Redirect(headers.ToString());
                    }
                }
            }

            using (var client = new HttpClient())
            {
                HttpContent q = new StringContent(queries.ToString());
                string Url = "https://paynetzuat.atomtech.in/paynetz/rfts";
                var response = client.PostAsync(Url, q).Result;
                if (response.IsSuccessStatusCode)
                {
                    Console.Write("Success");
                }
                else
                {
                    Console.Write("Error");
                }
                return Redirect(response.RequestMessage.RequestUri.OriginalString);
            }

        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> PostRequest(string url)
        {
            url = "https://paynetzuat.atomtech.in/paynetz/rfts";

            string merchantid = "197";
            string pwd = "Test@123";
            string atomtxnid = "100004489745";
            string refundamt = "200.00";
            string txndate = DateTime.Today.ToString("yyyy-MM-dd");
            string merefundref = "test123";
            string strpwd, strpwdEncoded;
            byte[] b;

            b = Encoding.UTF8.GetBytes(pwd);
            strpwd = Convert.ToBase64String(b);
            strpwdEncoded = HttpUtility.UrlEncode(strpwd);

            List<string> queries = new List<string>();
            {
                queries.Add(merchantid);
                queries.Add(strpwdEncoded);
                queries.Add(atomtxnid);
                queries.Add(refundamt);
                queries.Add(txndate);
                queries.Add(merefundref);
            }

            using (var client = new HttpClient())
            {
                HttpContent q = new StringContent(queries.ToString());
                string Url = "https://paynetzuat.atomtech.in/paynetz/rfts";
                var response = client.PostAsync(Url, q).Result;
                if (response.IsSuccessStatusCode)
                {
                    Console.Write("Success");
                }
                else
                {
                    Console.Write("Error");
                }
                return Redirect(response.RequestMessage.RequestUri.OriginalString);
            }

        }

    }
}