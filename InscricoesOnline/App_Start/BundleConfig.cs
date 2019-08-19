using System.Web;
using System.Web.Optimization;

namespace InscricoesOnline
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.3.1.min.js",
                        "~/Scripts/jquery-sortable.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/owl.carousel.min.js",
                      "~/Scripts/jquery.flexslider.js",
                      "~/Scripts/formValidation.min.js",
                      "~/Scripts/formValidation.bootstrap.min.js",
                      "~/Scripts/wow.min.js",
                      "~/Scripts/site.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/responsive.css",
                      "~/Content/Admin/font-awesome.min.css",
                      "~/Content/flexslider.css",
                      "~/Content/animate.css",
                      "~/Content/owl.carousel.min.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap_admin").Include(
                      "~/Scripts/bootstrap_admin.min.js",
                      "~/Scripts/jquery-admin.min.js",
                      "~/Scripts/validacoes.js",
                      "~/Scripts/custom.min.js",
                      //"~/Scripts/fastclick.js",
                      "~/Scripts/ckfinder/ckfinder.js",
                      "~/Scripts/bootstrap-colorpicker.js",
                      "~/Scripts/jquery.radiosforbuttons.js"
                      //"~/Scripts/select2.min.js"
                      //"~/Scripts/formValidation.min.js"
                      //"~/Scripts/formValidation.bootstrap.min.js"
                      //"~/Scripts/nprogress.js"
                      ));

            bundles.Add(new StyleBundle("~/ContentAdmin/css").Include(
                      "~/Content/Admin/bootstrap-progressbar-3.3.4.min.css",
                      "~/Content/Admin/bootstrap.min.css",
                      "~/Content/Admin/custom.min.css",
                      "~/Content/Admin/datetimepicker.css",
                      "~/Content/Admin/daterangepicker.css",
                      "~/Content/Admin/font-awesome.min.css",
                      "~/Content/Admin/green.css",
                      "~/Content/Admin/dropdownlist.css",
                      "~/Content/Admin/jqvmap.min.css",
                      "~/Content/Admin/bootstrap-colorpicker.css",
                      "~/Content/select2.min.css",
                      "~/Content/html5imageupload.css",
                      "~/Content/Admin/formValidation.min.css",
                      "~/Content/Admin/nprogress.css"));
        }
    }
}
