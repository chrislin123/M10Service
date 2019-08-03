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
            //WithCronSchedule：https://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/crontriggers.html


            //==更新個股基本資料
            // 建立工作
            IJobDetail jobStockInfo = JobBuilder.Create<StockInfoTask>()
                                .WithIdentity("jobStockInfo")
                                .Build();
            // 建立觸發器
            ITrigger triggerStockInfo = TriggerBuilder.Create()
                                    .WithCronSchedule("0 10 3 * * ?")  // 每天0310
                                    .WithIdentity("triggerStockInfo")
                                    .Build();

            //==取得三大法人資料
            // 建立工作
            IJobDetail jobStockThreeTrade = JobBuilder.Create<StockThreeTradeTask>()
                                .WithIdentity("jobStockThreeTrade")
                                .Build();
            // 建立觸發器
            ITrigger triggerStockThreeTrade = TriggerBuilder.Create()
                                    .WithCronSchedule("0 5/30 16-17 * * ?")  // 每天1605-1705，每三十分鐘一次
                                                                             //.WithCronSchedule("0/3 * * * * ?")  // 每三秒觸發一次。
                                    .WithIdentity("triggerStockThreeTrade")
                                    .Build();

            //==盤後，巨量換手
            // 建立工作
            IJobDetail jobStockAfter = JobBuilder.Create<StockAfterTask>()
                                .WithIdentity("jobStockAfter")
                                .Build();
            // 建立觸發器
            ITrigger triggerStockAfter = TriggerBuilder.Create()
                                    .WithCronSchedule("0 11/30 15-17 * * ?")  // 每天1511-1711，每三十分鐘一次
                                                                              //.WithCronSchedule("0/3 * * * * ?")  // 每三秒觸發一次。
                                    .WithIdentity("triggerStockAfter")
                                    .Build();


            // 建立工作
            IJobDetail jobStockBrokerBS = JobBuilder.Create<StockBrokerBSTask>()
                                .WithIdentity("jobStockBrokerBS")
                                .Build();
            // 建立觸發器
            ITrigger triggerStockBrokerBS = TriggerBuilder.Create()
                                    .WithCronSchedule("0 5 16,18 * * ?")  // 每天16:05 18:05執行
                                                                          //.WithCronSchedule("0/3 * * * * ?")  // 每三秒觸發一次。
                                    .WithIdentity("triggerStockBrokerBS")
                                    .Build();

            //==當沖資料
            // 建立工作
            IJobDetail jobStockAfterRush = JobBuilder.Create<StockAfterRushTask>()
                                .WithIdentity("jobStockAfterRush")
                                .Build();
            // 建立觸發器
            ITrigger triggerStockAfterRush = TriggerBuilder.Create()
                                    .WithCronSchedule("0 0/30 16-23 * * ?")  // 每天16-23時，每30分鐘 執行                                                                    
                                    .WithIdentity("triggerStockAfterRush")
                                    .Build();




            // 把工作加入排程
            _Scheduler.ScheduleJob(jobStockInfo, triggerStockInfo);
            _Scheduler.ScheduleJob(jobStockThreeTrade, triggerStockThreeTrade);
            _Scheduler.ScheduleJob(jobStockAfter, triggerStockAfter);
            _Scheduler.ScheduleJob(jobStockBrokerBS, triggerStockBrokerBS);
            _Scheduler.ScheduleJob(jobStockAfterRush, triggerStockAfterRush);

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


        protected void Application_End()
        {
            NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
            _logger.Log(NLog.LogLevel.Info, "C10Mvc Application_End()");

            System.Threading.Thread.Sleep(3000);

            string sUrl = "http://localhost/C10Mvc/StockApi/getStockType?StockCode=1475";
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(sUrl);
            System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();

            string desc = res.StatusDescription;

            _logger.Log(NLog.LogLevel.Info, "C10Mvc Application_End(), Application Restart....." + desc);

        }
    }
}
