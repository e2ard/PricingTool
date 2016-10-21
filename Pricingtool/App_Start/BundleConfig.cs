using System.Web;
using System.Web.Optimization;

namespace getLayout
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                         "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/respond.min.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/bootstrap-datetimepicker.min.js",
                      "~/Scripts/bootstrap.min.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/Site.css"/*,
                       "~/Content/themes/ui-darkness/jquery.ui.all.css"*/));
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                "~/Content/bootstrap.min.css"));
        }
    }
}
