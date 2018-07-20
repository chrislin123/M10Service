using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace M10Api
{
  public class WebApiApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);

      //資料回覆為Json格式
      GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
    }

    void ErrorMail_Mailing(object sender, Elmah.ErrorMailEventArgs e)
    {
      string machineName = Request.ServerVariables["HTTP_HOST"];
      string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      // 取得 Elamh ErrorMail 的主旨
      // "$MachineName$ at $ErrorTime$ : {0}"
      string elmahSubject = e.Mail.Subject;
      //替換 ErrorMail 的主旨內容
      string emailSubject = elmahSubject
              .Replace("$MachineName$", machineName)
              .Replace("$ErrorTime$", currentDateTime)
      ;
      e.Mail.Subject = emailSubject;
    }
  }
}
