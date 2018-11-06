using System.Web;
using System.Web.Optimization;

namespace Speedbird
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js"));

           
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",                      
                      "~/Scripts/respond.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                      "~/Scripts/jquery-ui-1.12.1.min.js"));
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/all.css"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/EAcss").Include(
                      "~/Content/EA.css"));
            bundles.Add(new StyleBundle("~/SBBoss/Content/EAcss").Include(
                      "~/Areas/SBBoss/Content/EAadmin.css"));
            //bundles.Add(new StyleBundle("~/Content/fa5").Include(
            //       "~/Content/css/FA5all.css"));



            bundles.Add(new ScriptBundle("~/bundles/jquerydtpicker").Include(
                    "~/Scripts/jquery-ui-timepicker-addon.min.js",
                    "~/Scripts/select2.min.js",
                    "~/Scripts/jquery-te-1.4.0.min.js",
                    "~/Scripts/jquery.barrating.min.js"));
            bundles.Add(new StyleBundle("~/Content/dtpic").Include(
                    "~/Content/jquery-ui-timepicker-addon.min.css",
                    "~/Content/css/select2.min.css",
                    "~/Content/select2-bootstrap.css",
                    "~/Content/jquery-te-1.4.0.css"));

         
            BundleTable.EnableOptimizations = false;
        }
    }
}
