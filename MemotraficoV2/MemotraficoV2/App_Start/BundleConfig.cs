using System.Web.Optimization;

namespace IdentitySample
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                        "~/Content/Proyecto/jquery/dist/jquery.min.js",
                        "~/Content/Proyecto/bootstrap/dist/js/bootstrap.min.js",
                        "~/Content/Proyecto/vendors/fastclick/lib/fastclick.js",
                        "~/Content/Proyecto/vendors/nprogress/nprogress.js",
                        "~/Content/Proyecto/vendors/Chart.js/dist/Chart.min.js",
                        "~/Content/Proyecto//gauge.js/dist/gauge.min.js",
                        "~/Content/Proyecto/bootstrap-progressbar/bootstrap-progressbar.min.js",
                        "~/Content/Proyecto/iCheck/icheck.min.js",
                        "~/Content/Proyecto/skycons/skycons.js",
                        "~/Content/Proyecto/Flot/jquery.flot.js",
                        "~/Content/Proyecto/Flot/jquery.flot.pie.js",
                        "~/Content/Proyecto/Flot/jquery.flot.time.js",
                        "~/Content/Proyecto/Flot/jquery.flot.stack.js",
                        "~/Content/Proyecto/Flot/jquery.flot.resize.js",
                        "~/Content/Proyecto/flot.orderbars/js/jquery.flot.orderBars.js",
                        "~/Content/Proyecto/flot-spline/js/jquery.flot.spline.min.js",
                        "~/Content/Proyecto/flot.curvedlines/curvedLines.js",
                        "~/Content/Proyecto/DateJS/build/date.js",
                        "~/Content/Proyecto/jqvmap/dist/jquery.vmap.js",
                        "~/Content/Proyecto/jqvmap/dist/maps/jquery.vmap.world.js",
                        "~/Content/Proyecto/examples/js/jquery.vmap.sampledata.js",
                        "~/Content/Proyecto/moment/min/moment.min.js",
                        "~/Content/Proyecto/vendors/bootstrap-daterangepicker/daterangepicker.js",
                        "~/Content/Proyecto/build/js/custom.min.js"));


            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/loader-style.css"));

            bundles.Add(new StyleBundle("~/view").Include(
                      "~/Content/Loader-style2.css"));

        }
    }
}
