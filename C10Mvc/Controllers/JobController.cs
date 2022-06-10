using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using System.Text;
using Quartz;
using M10.lib;
using NLog;
using M10.lib.model;
using HtmlAgilityPack;
using System.IO;

namespace C10Mvc.Controllers
{
    public class JobController : ApiController
    {
    }

    public class BaseJob
    {
        private NLog.Logger _logger;
        protected string ssql = string.Empty;
        private string _ConnectionString;
        private string _ProviderName;
        private ConnectionStringSettings _ConnectionStringSettings;
        private DALDapper _dbDapper;
        private StockHelper _stockhelper;

        public StockHelper Stockhelper
        {
            get
            {
                if (_stockhelper == null)
                {
                    _stockhelper = new StockHelper();
                }
                return _stockhelper;
            }

        }


        protected Logger logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = NLog.LogManager.GetCurrentClassLogger();
                }

                return _logger;
            }
        }

        protected DALDapper dbDapper
        {
            get
            {
                if (_dbDapper == null)
                {
                    //_dbDapper = new DALDapper(ConnectionString);
                    _dbDapper = new DALDapper(ConnectionStringSettings);
                }

                return _dbDapper;
            }
        }

        protected string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_ConnectionString))
                {
                    _ConnectionString = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DBDefault"]].ConnectionString;
                }

                return _ConnectionString;
            }
        }

        public string ProviderName
        {
            get
            {
                if (string.IsNullOrEmpty(_ProviderName))
                {
                    _ProviderName = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DBDefault"]].ProviderName;
                }

                return _ProviderName;
            }
        }

        public ConnectionStringSettings ConnectionStringSettings
        {
            get
            {
                if (_ConnectionStringSettings == null)
                {
                    _ConnectionStringSettings = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DBDefault"]];
                }

                return _ConnectionStringSettings;
            }
        }
    }

    public class SendMailTask : BaseJob, IJob
    {
        private void Log(string msg)
        {
            System.IO.File.AppendAllText(@"C:\Temp\log.txt", msg + Environment.NewLine);
        }

        public void DoSendMail()
        {
            Log("Entering DoSendMail() at " + DateTime.Now.ToString());

            int.Parse("tt1");
            // 發送 email。這裡只固定輸出一筆文字訊息至 log 檔案，方便觀察測試。
            // 每發送一封 email 就檢查一次 IntervalTask.Current.SuttingDown 以配合外部的終止事件。
            string msg = String.Format("DoSendMail() at {0:yyyy/MM/dd HH:mm:ss}", DateTime.Now);
            Log(msg);
            //Thread.Sleep(2000);

        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                DoSendMail();
            }
            catch (Exception ex)
            {
                logger.Log(NLog.LogLevel.Error, ex.Message);
            }
        }
    }

    //一次只執行一個體
    [DisallowConcurrentExecutionAttribute]
    public class StockInfoTask : BaseJob, IJob
    {
        public void DoStockTrans()
        {
            //Log("START DoStockTrans() at " + DateTime.Now.ToString());
            logger.Info("START DoStockTrans()");
            try
            {
                //更新個股基本資料
                Stockhelper.GetStockInfo();
            }
            catch (Exception)
            {


            }


            logger.Info("END DoStockTrans()");
            //MessageBox.Show("Finish");



            // 發送 email。這裡只固定輸出一筆文字訊息至 log 檔案，方便觀察測試。
            // 每發送一封 email 就檢查一次 IntervalTask.Current.SuttingDown 以配合外部的終止事件。
            //Log("Finish DoStockTrans() at " + DateTime.Now.ToString());

            //Thread.Sleep(2000);

        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                DoStockTrans();
            }
            catch (Exception ex)
            {
                logger.Log(NLog.LogLevel.Error, ex.Message);

            }
        }
    }


    /// <summary>
    /// 三大法人個股買賣盤後資料
    /// </summary>
    //一次只執行一個體
    [DisallowConcurrentExecutionAttribute]
    public class StockThreeTradeTask : BaseJob, IJob
    {
        public void DoStockThreeTrade()
        {

            logger.Info("START DoStockThreeTrade()");

            DateTime dt = DateTime.Now;

            for (DateTime dtTemp = dt; dtTemp >= dt.AddDays(-3); dtTemp = dtTemp.AddDays(-1))
            {
                logger.Info(string.Format("{0}=={1}", "DoStockThreeTrade()", Utils.getDatatimeString(dtTemp)));

                #region tse-threeTrade
                //三大法人個股買賣盤後資料(TSE)
                Stockhelper.GetStockThreeTradeTse(dtTemp);
                #endregion

                #region otc-threeTrade
                //三大法人個股買賣盤後資料(OTC)
                Stockhelper.GetStockThreeTradeOtc(dtTemp);
                #endregion

                System.Threading.Thread.Sleep(30000);
            }

            logger.Info("END DoStockThreeTrade()");
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                DoStockThreeTrade();
            }
            catch (Exception ex)
            {
                logger.Log(NLog.LogLevel.Error, ex.Message);
            }
        }
    }


    /// <summary>
    /// 盤後資料 + 巨量換手
    /// </summary>
    //一次只執行一個體
    [DisallowConcurrentExecutionAttribute]
    public class StockAfterTask : BaseJob, IJob
    {
        /// <summary>
        /// 盤後資料
        /// </summary>
        public void DoStockAfter()
        {

            logger.Info("START DoStockAfter()");
            DateTime dt = DateTime.Now;

            for (DateTime dtTemp = dt; dtTemp >= dt.AddDays(-3); dtTemp = dtTemp.AddDays(-1))
            {
                logger.Info(string.Format("{0}=={1}", "DoStockAfter()", Utils.getDatatimeString(dtTemp)));
                #region tse-StockAfter
                Stockhelper.GetStockAfterTse(dtTemp);
                #endregion

                System.Threading.Thread.Sleep(30000);

                #region otc-StockAfter
                Stockhelper.GetStockAfterOtc(dtTemp);
                #endregion

                System.Threading.Thread.Sleep(30000);
            }


            logger.Info("END DoStockAfter()");
        }

        /// <summary>
        /// 巨量換手
        /// </summary>
        public void DoStockHugeTurnover()
        {

            logger.Info("START DoStockHugeTurnover()");


            //預設今天資料
            string sRunDate = DateTime.Now.ToString("yyyyMMdd");

            //轉最新資料
            ssql = " select Max(stockdate) from stockafter where stockcode = '2330' ";
            object oMax = dbDapper.ExecuteScale(ssql);
            if (oMax != null)
            {
                sRunDate = Convert.ToString(Convert.ToInt32(oMax.ToString()));
            }

            List<StockInfo> siList = new List<StockInfo>();
            ssql = " select * from stockinfo where status = 'Y' and LEN(stockcode) = 4 order by stockcode ";
            siList = dbDapper.Query<StockInfo>(ssql);

            foreach (StockInfo item in siList)
            {
                Stockhelper.GetHugeTurnover(item.stockcode, sRunDate);
            }

            logger.Info("END DoStockHugeTurnover()");
        }


        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //盤後資料
                DoStockAfter();

                //搜尋巨量換手
                DoStockHugeTurnover();
            }
            catch (Exception ex)
            {
                logger.Log(NLog.LogLevel.Error, ex.Message);
            }
        }
    }

    /// <summary>
    /// 券商買賣證券日報表查詢系統
    /// </summary>
    //一次只執行一個體
    [DisallowConcurrentExecutionAttribute]
    public class StockBrokerBSTask : BaseJob, IJob
    {
        public void DoStockBrokerBS()
        {

            logger.Info("START DoStockBrokerBS()");
            //券商買賣證券日報表查詢系統
            Stockhelper.GetStockBrokerBS();

            logger.Info("END DoStockBrokerBS()");
        }


        public void Execute(IJobExecutionContext context)
        {
            try
            {
                DoStockBrokerBS();
            }
            catch (Exception ex)
            {
                logger.Log(NLog.LogLevel.Error, ex.Message);
            }
        }
    }

    /// <summary>
    /// 盤後當沖資料
    /// </summary>
    //一次只執行一個體
    [DisallowConcurrentExecutionAttribute]
    public class StockAfterRushTask : BaseJob, IJob
    {
        public void DoStockAfterRush()
        {

            logger.Info("START DoStockAfterRush()");
            DateTime dt = DateTime.Now;

            Stockhelper.GetStockAfterRushTse(dt);

            Stockhelper.GetStockAfterRushOtc(dt);

            logger.Info("END DoStockAfterRush()");
        }



        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //盤後當沖資料
                DoStockAfterRush();
            }
            catch (Exception ex)
            {
                logger.Log(NLog.LogLevel.Error, ex.Message);
            }
        }
    }



}
