using Speedbird.Areas.SBBoss.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost]
        public ActionResult RefundReport()
        {
            try
            {

                string strPwd, strPwdEncoded;
                byte[] b;


                UriBuilder Url = new UriBuilder("https://paynetzuat.atomtech.in/paynetz/rfts");
                string merchantid = "197";
                string pwd = "Test@123";
                string atomtxnid = "100001416131";
                string refundamt = "10.00";
                string txndate = "2014-01-29";
                string merefundref = "test123";


                b = Encoding.UTF8.GetBytes(pwd);
                strPwd = Convert.ToBase64String(b);
                strPwdEncoded = HttpUtility.UrlEncode(strPwd);

                Url.Query = $"merchantid={merchantid}&pwd={strPwd}&atomtxnid={atomtxnid}" +
                $"&refundamt={refundamt}&txndate={txndate}" +
                $"&merefundref={merefundref}";

                return Redirect(Url.ToString());
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult AtomRefundRecieve(string MERCHANTID, string AMOUNT, string TXNID, string STATUSCODE, string STATUSMESSAGE, string ATOMREFUNDID)
        {
            try
            {
                var atomrcv = new AtomRefundRecieve()
                {
                    merchantid = MERCHANTID,
                    amt = AMOUNT,
                    txnid = TXNID,
                    statuscode = STATUSCODE,
                    statusmsg = STATUSMESSAGE,
                    atomrefundid = ATOMREFUNDID
                };

                return View("AtomResponseReciever", atomrcv);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Settlementapi()
        {
            try
            {
                UriBuilder Url = new UriBuilder("https://pgreports.atomtech.in/SettlementReport/generateReport");
                return Redirect(Url.ToString());
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}