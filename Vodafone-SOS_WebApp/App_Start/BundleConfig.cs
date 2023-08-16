using System.Web;
using System.Web.Optimization;

namespace Vodafone_SOS_WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/StyleVF.css",
                     "~/Content/styles/jqx*"));

            bundles.Add(new StyleBundle("~/Content/VodafoneThemes/css").Include(
                "~/Content/VodafoneThemes/css/Black-theme.css",
                "~/Content/VodafoneThemes/css/bootstrap-theme.css",
                "~/Content/VodafoneThemes/css/bootstrap-theme.css.map",
                "~/Content/VodafoneThemes/css/bootstrap-theme.min.css",
                "~/Content/VodafoneThemes/css/bootstrap-theme.min.css.map",
                "~/Content/VodafoneThemes/css/bootstrap.css",
                "~/Content/VodafoneThemes/css/bootstrap.css.map",
                "~/Content/VodafoneThemes/css/bootstrap.min.css",
                "~/Content/VodafoneThemes/css/bootstrap.min.css.map",
                "~/Content/VodafoneThemes/css/custom.css",
               "~/Content/VodafoneThemes/css/default.css",
                "~/Content/VodafoneThemes/css/font-awesome.min.css"));

              bundles.Add(new ScriptBundle("~/bundles/jqwidgets").Include(
                "~/Scripts/jqxcore.js",
                "~/Scripts/jqxbuttons.js",
                "~/Scripts/jqxscrollbar.js",
                "~/Scripts/jqxpanel.js",
                "~/Scripts/jqxtree.js",
                
                "~/Scripts/jqxswitchbutton.js",
                "~/Scripts/jqxexpander.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqxgridbundle").Include(
                "~/Scripts/jqxcore.js",
                "~/Scripts/jqxdata.js",
                "~/Scripts/jqxbuttons.js",
                "~/Scripts/jqxscrollbar.js",
                "~/Scripts/jqxmenu.js",
                "~/Scripts/jqxlistbox.js",
                "~/Scripts/jqxdropdownlist.js",
                "~/Scripts/jqxeditor.js",//added for LEmailTemplate
                "~/Scripts/jqxdropdownbutton.js",
                "~/Scripts/jqxcolorpicker.js",
                "~/Scripts/jqxtooltip.js",
                "~/Scripts/jqxwindow.js",
                "~/Scripts/jqxgrid.js",
                "~/Scripts/jqxgrid.selection.js",
                "~/Scripts/jqxgrid.columnsresize.js",
                "~/Scripts/jqxgrid.filter.js",
                "~/Scripts/jqxgrid.sort.js",
                "~/Scripts/jqxgrid.pager.js",
                "~/Scripts/jqxgrid.grouping.js",
                "~/Scripts/jqxgrid.columnsresize.js",
                "~/Scripts/jqxgrid.columnsreorder.js",
                "~/Scripts/jqxgrid.edit.js",
                "~/Scripts/jqxtabs.js",
                "~/Scripts/jqxcheckbox.js",
                "~/Scripts/jqxcalendar.js",
                "~/Scripts/jqxdatetimeinput.js",
                "~/Scripts/globalization/globalize.js"
                ));
        }
    }
}
