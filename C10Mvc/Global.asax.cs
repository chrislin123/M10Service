using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Quartz;
using C10Mvc.Controllers;
using NLog;
namespace C10Mvc
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

      //Quartz Log與Nlog綁定
      Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter { Level = Common.Logging.LogLevel.Info };

      NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
      _logger.Log(NLog.LogLevel.Info, "C10Mvc Application_Start()");

      //建立以Ram為儲存體的排程器
      ISchedulerFactory schedulerFactory = new Quartz.Impl.StdSchedulerFactory();
      IScheduler _Scheduler = schedulerFactory.GetScheduler();


      //// 建立工作
      //IJobDetail job = JobBuilder.Create<SendMailTask>()
      //                    .WithIdentity("SendMailJob")
      //                    .Build();
      ////WithCronSchedule：https://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/crontriggers.html
      //// 建立觸發器
      //ITrigger trigger = TriggerBuilder.Create()
      //                        .WithCronSchedule("0/10 * * * * ?")  // 每一分鐘觸發一次。
      //                        .WithIdentity("SendMailTrigger")
      //                        .Build();


      // 建立工作
      IJobDetail job = JobBuilder.Create<StockTransTask>()
                          .WithIdentity("StockTransJob")
                          .Build();
      //WithCronSchedule：https://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/crontriggers.html
      // 建立觸發器
      ITrigger trigger = TriggerBuilder.Create()
                              .WithCronSchedule("0 10 3 * * ?")  // 每一分鐘觸發一次。
                              .WithIdentity("StockTransTrigger")
                              .Build();

      // 把工作加入排程
      _Scheduler.ScheduleJob(job, trigger);

      // 啟動排程器
      _Scheduler.Start();


      //===========================================
      //JobBuilder.Create<SendMailTask>().
      //IJobDetail job = JobBuilder.Create().WithIdentity("test").Build();

      //ITrigger tt = TriggerBuilder.Create()
      //  .WithCronSchedule("")
      //  .WithIdentity("")
      //  .Build();

      //ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
      //    .WithSimpleSchedule(a => a.WithIntervalInSeconds(1).WithRepeatCount(10))
      //    .Build();

      //_Scheduler.ScheduleJob(job, trigger);

      //_Scheduler.Start();

      //=====================================
      ////1.0創建調度工廠
      //ISchedulerFactory factory = new StdSchedulerFactory();
      ////2.0通過工廠獲取調度器實例
      //IScheduler scheduler = factory.GetScheduler();
      ////3.0通過JobBuilder構建Job
      //IJobDetail job = JobBuilder.Create<JobGetNowTime>().Build();
      ////4.0通過TriggerBuilder構建Trigger
      //ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
      //    .WithSimpleSchedule(a => a.WithIntervalInSeconds(1).WithRepeatCount(10))
      //    .Build();
      ////5.0組裝各個組件<Job,Trigger>
      //scheduler.ScheduleJob(job, trigger);
      ////6.0啟動
      //scheduler.Start();
      //Thread.Sleep(10000);
      ////7.0銷毀內置的Job和Trigger
      //scheduler.Shutdown(true);
      //Console.ReadKey();






    }
  }
}
