using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M10.lib
{
    public static class M10Const
    {
        public static string StockAfterTseUrl = "http://www.tse.com.tw/exchangeReport/MI_INDEX?response=csv&date={0}&type=ALLBUT0999";
        
        public static string StockAfterOtcUrl = "http://www.tpex.org.tw/web/stock/aftertrading/daily_close_quotes/stk_quote_download.php?l=zh-tw&d={0}&s=0,asc,0";

        public static string StockThreeTradeTse = "http://www.tse.com.tw/fund/T86?response=csv&date={0}&selectType=ALLBUT0999";
        public static string StockThreeTradeOtc = "http://www.tpex.org.tw/web/stock/3insti/daily_trade/3itrade_hedge_download.php?l=zh-tw&se=EW&t=D&d={0}&s=0,asc";

        //個股基本資料網址
        public static string StockInfoTse = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=2";
        public static string StockInfoOtc = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=4";
        public static string StockInfoOtc1 = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=5";

        //***盤後資料，每日當沖資訊
        //這是櫃買中心公告網址
        //要晚上才會更新今天的 http://www.tpex.org.tw/web/stock/trading/intraday_stat/intraday_trading_stat.php?l=zh-tw
        //這是交易所的(上市)
        //要自己選擇日期，類別
        //今天的一樣晚上才會更新
        //http://www.twse.com.tw/zh/page/trading/exchange/TWTB4U.html
        public static string StockAfterRushTse = "http://www.twse.com.tw/exchangeReport/TWTB4U?response=csv&date={0}&selectType=All";
        public static string StockAfterRushOtc = "http://www.tpex.org.tw/web/stock/trading/intraday_stat/intraday_trading_stat_result.php?l=zh-tw&d={0}&s=0,asc,0&o=csv";



        public static class AlertStatus
        {
            public const string I = "發布黃色";
            public const string C = "黃升紅";
            public const string O = "紅降黃";
            public const string D = "解除黃色";

            public const string A1 = "黃色";
            public const string A2 = "澄色";
            public const string A3 = "紅色";
            public const string AD = "解除";
        }

        /// <summary>
        /// //UnitType 雨量站單位 => A1:氣象署自動站,A2:氣象署氣象站,A3:水保署,A4:水利署
        /// </summary>
        public static class RainStationUnit
        {
            public const string A1 = "氣象署自動站";
            public const string A2 = "氣象署氣象站";
            public const string A3 = "水保署";
            public const string A4 = "水利署";
        }

        public static class DataStaticLogType
        {
            public const string RainAreaSplit = "RainAreaSplit";
            //雨場分割轉EXCEL
            public const string RainAreaToExcel = "RainAreaToExcel";
          
        }

        public static class StockType
        {
            public const string otc = "otc";
            public const string tse = "tse";
            public const string otc1 = "otc1";
        }


        public static class StockLogStatus
        {
            public const string s200 = "success";
            public const string s400 = "failure";
        }

        public static class StockLogType
        {
            /// <summary>
            /// 盤後資料轉檔(TSE)
            /// </summary>
            public const string StockAfterTse = "StockAfterTse";
            /// <summary>
            /// 盤後資料轉檔(OTC)
            /// </summary>
            public const string StockAfterOtc = "StockAfterOtc";
            public const string StockThreeTse = "StockThreeTse";
            public const string StockThreeOtc = "StockThreeOtc";
            public const string StockInfoTse = "StockInfoTse";
            public const string StockInfoOtc = "StockInfoOtc";
            public const string StockInfoOtc1 = "StockInfoOtc1";
            public const string StockBrokerBSTse = "StockBrokerBSTse";
            public const string StockBrokerBSOtc = "StockBrokerBSOtc";    //}
            public const string StockAfterRushTseLog = "StockAfterRushTseLog";
            public const string StockAfterRushOtcLog = "StockAfterRushOtcLog";
        }
        public enum StockRuntimeStatus : int
        {
            /// <summary>
            /// YahooApi
            /// </summary>
            YahooApi = 10,
            /// <summary>
            /// 歷史資料
            /// </summary>
            Histroy = 99,

        }



        public enum DateStringType
        {
            /// <summary>
            /// 日期格式化：民國年1060904
            /// </summary>
            ChineseT1,
            /// <summary>
            /// 日期格式化：民國年106/09/04
            /// </summary>
            ChineseT2,


            /// <summary>
            /// 日期格式化：西元年20170904
            /// </summary>
            ADT1,
            /// <summary>
            /// 日期格式化：西元年2017/09/04
            /// </summary>
            ADT2,

        }

        public enum DatetimeStringType
        {
            /// <summary>
            /// 日期格式化：民國年1060904
            /// </summary>
            //ChineseT1,
            /// <summary>
            /// 日期格式化：民國年106/09/04
            /// </summary>
            //ChineseT2,


            /// <summary>
            /// 日期格式化：yyyyMMddTHHmmss(20150805173000)
            /// </summary>
            ADDT1,
            /// <summary>
            /// 日期格式化：yyyy-MM-ddTHH:mm:ss(2015-08-05T17:30:00)
            /// </summary>
            ADDT2,

        }

        //public static class StockTransRecType
        //{
        //  public const string StockAfterRush = "StockAfterRush";

        //}


        public enum StockTransRecType
        {
            StockAfterRush,
            Histroy,
        }


    }

    public static class DhoeConst
    {
        public static string StockAfterTseUrl = "http://www.tse.com.tw/exchangeReport/MI_INDEX?response=csv&date={0}&type=ALLBUT0999";


        public static class StudentType
        {
            public const string CurrSt = "CurrSt";
            //public const string CurrPhd = "CurrPhd";
            //public const string CurrMas = "CurrMas";
            public const string HisPhd = "HisPhd";
            public const string HisMas = "HisMas";

        }


        public enum StockRuntimeStatus : int
        {
            /// <summary>
            /// YahooApi
            /// </summary>
            YahooApi = 10,
            /// <summary>
            /// 歷史資料
            /// </summary>
            Histroy = 99,

        }


    }

    public static class GlobalConst
    {
        public static string Demo = "123";



        public static class AlertStatus
        {
            public const string I = "發布黃色";
            public const string C = "黃升紅";
            public const string O = "紅降黃";
            public const string D = "解除黃色";

            public const string A1 = "黃色";
            public const string A2 = "澄色";
            public const string A3 = "紅色";
            public const string AD = "解除";
        }

        public enum StockTransRecType
        {
            StockAfterRush,
            Histroy,
        }


    }

    public static class MVCControlClass
    {
        public static string Demo = "123";



        public class ApiResult
        {

            public string ApiResultStauts { get; set; }

            public string Message { get; set; }

            public dynamic Data { get; set; }

        }


        public static class ApiResultStautsType
        {
            public static string Success = "123";
            public static string Error = "123";
        }


    }


}
