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
    }

    public static class DataStringType1
    {
      public const string ChineseT1 = "ChineseT1";
      public const string ChineseT2 = "ChineseT2";
    }


    public enum DataStringType
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
    
  }
}
