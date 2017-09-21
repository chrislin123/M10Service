using System.Web;
using System.Web.Optimization;

namespace DhoeMvc
{
  public class BundleConfig
  {
    // 如需「搭配」的詳細資訊，請瀏覽 http://go.microsoft.com/fwlink/?LinkId=301862
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                  "~/Scripts/jquery-{version}.js",
                  "~/Scripts/common.js"));

      bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                  "~/Scripts/jquery.validate*"));

      // 使用開發版本的 Modernizr 進行開發並學習。然後，當您
      // 準備好實際執行時，請使用 http://modernizr.com 上的建置工具，只選擇您需要的測試。
      bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                  "~/Scripts/modernizr-*"));

      bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js"));


      //easyui js
      bundles.Add(new ScriptBundle("~/bundles/easyui").Include(
                "~/Scripts/jquery.easyui-1.4.5.js",
                "~/Scripts/locale/easyui-lang-zh_TW.js"));
      //easyui css
      bundles.Add(new StyleBundle("~/Content/easyui").Include(
                "~/Content/themes/default/easyui.css",
                "~/Content/themes/color.css",
                "~/Content/themes/icon.css"));



      bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"));
    }
  }
}
