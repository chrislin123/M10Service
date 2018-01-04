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

    public static string StockInfoTse = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=2";
    public static string StockInfoOtc = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=4";
    public static string StockInfoOtc1 = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=5";


    public static class AlertStatus
    {
      public const string I = "發布黃色";
      public const string C = "黃升紅";
      public const string O = "紅降黃";
      public const string D = "解除黃色";
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
      public const string StockAfterTse = "StockAfterTse";
      public const string StockAfterOtc = "StockAfterOtc";
      public const string StockThreeTse = "StockThreeTse";
      public const string StockThreeOtc = "StockThreeOtc";
      public const string StockInfoTse = "StockInfoTse";
      public const string StockInfoOtc = "StockInfoOtc";
      public const string StockInfoOtc1 = "StockInfoOtc1";
      public const string StockBrokerBSTse = "StockBrokerBSTse";
      public const string StockBrokerBSOtc = "StockBrokerBSOtc";
    }

    public enum StockRuntimeStatus:int
    {
      /// <summary>
      /// YahooApi
      /// </summary>
      YahooApi=10,
      /// <summary>
      /// 歷史資料
      /// </summary>
      Histroy=99,
      
    }



    public enum DateStringType
    {
      /// <summary>
      /// 日期格式化：民國年1060904
      /// </summary>
      ChineseT1 ,
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

}
