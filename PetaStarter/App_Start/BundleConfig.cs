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
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",                      
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                      "~/Scripts/jquery-ui-{version}.js"));
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/all.css"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/EAcss").Include(
                      "~/Content/EA.css"));
            bundles.Add(new StyleBundle("~/SBBoss/Content/EAcss").Include(
                      "~/Areas/SBBoss/Content/EAadmin.css"));


            bundles.Add(new ScriptBundle("~/bundles/jquerydtpicker").Include(
                    "~/Scripts/jquery-ui-timepicker-addon.js"));
            bundles.Add(new StyleBundle("~/Content/dtpic").Include(
                    "~/Content/jquery-ui-timepicker-addon.css"));

            bundles.Add(new ScriptBundle("~/fa5").Include(
                      "~/fa5/svg-with-js/js/fa-solid.js",
                      "~/fa5/svg-with-js/js/fontawesome.js"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
