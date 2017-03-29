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
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                        "~/Scripts/spin.min.js",
                        "~/Content/Proyecto/jquery/dist/jquery.min.js",
                        "~/Content/Proyecto/bootstrap/dist/js/bootstrap.min.js",
                        "~/Content/Proyecto/fastclick/lib/fastclick.js",
                        "~/Content/Proyecto/switchery/dist/switchery.min.js",
                        "~/Content/Proyecto/bootstrap-wysiwyg/js/bootstrap-wysiwyg.min.js",
                        "~/Content/Proyecto/jquery.hotkeys/jquery.hotkeys.js",
                        "~/Content/Proyecto/select2/dist/js/select2.full.min.js",
                        "~/Content/Proyecto/parsleyjs/dist/parsley.min.js",
                        "~/Content/Proyecto/nprogress/nprogress.js",
                        "~/Content/Proyecto/Chart.js/dist/Chart.min.js",
                        "~/Content/Proyecto/gauge.js/dist/gauge.min.js",
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
                        "~/Content/Proyecto/jqvmap/examples/js/jquery.vmap.sampledata.js",
                        "~/Content/Proyecto/moment/min/moment.min.js",
                        "~/Content/Proyecto/bootstrap-daterangepicker/daterangepicker.js",
                        "~/Content/Proyecto/build/js/custom.min.js",
                        "~/Content/Proyecto/EasyAutocomplete-1.3.5/easy-autocomplete.min.js",
                        "~/Content/Proyecto/toastmessage/jquery.toastmessage.js"));

            bundles.Add(new ScriptBundle("~/bundles/tables").Include(
                        "~/Content/Proyecto/datatables.net/js/jquery.dataTables.min.js",
                        "~/Content/Proyecto/datatables.net-bs/js/dataTables.bootstrap.min.js",
                        "~/Content/Proyecto/datatables.net-buttons/js/dataTables.buttons.min.js",
                        "~/Content/Proyecto/datatables.net-buttons-bs/js/buttons.bootstrap.min.js",
                        "~/Content/Proyecto/datatables.net-buttons/js/buttons.flash.min.js",
                        "~/Content/Proyecto/datatables.net-buttons/js/buttons.html5.min.js",
                        "~/Content/Proyecto/datatables.net-buttons/js/buttons.print.min.js",
                        "~/Content/Proyecto/datatables.net-fixedheader/js/dataTables.fixedHeader.min.js",
                        "~/Content/Proyecto/datatables.net-keytable/js/dataTables.keyTable.min.js",
                        "~/Content/Proyecto/datatables.net-responsive/js/dataTables.responsive.min.js",
                        "~/Content/Proyecto/datatables.net-responsive-bs/js/responsive.bootstrap.js",
                        "~/Content/Proyecto/datatables.net-scroller/js/dataTables.scroller.min.js",
                        "~/Content/Proyecto/jszip/dist/jszip.min.js",
                        "~/Content/Proyecto/pdfmake/build/pdfmake.min.js",
                        "~/Content/Proyecto/pdfmake/build/vfs_fonts.js"));


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
