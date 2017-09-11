using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using HtmlAgilityPack;
using M10.lib.model;
using M10.lib;

namespace C10Mvc.Class
{
  public  class StockUtil : M10BaseClass
  {
    public StockRuntime getStockRealtimeYahooWeb(string stockcode)
    {
      if (stockcode == "t00") stockcode = "0000";
      //if (stockcode == "t00") stockcode = "0000";

      StockRuntime sr = new StockRuntime();
      sr.z = "";
      sr.y = "";
      sr.u = "";
      sr.w = "";
      sr.n = "";
      sr.c = "";
      sr.xx = "";


      string surl = "https://tw.stock.yahoo.com/q/q?s=" + stockcode;
      //surl = "https://tw.stock.yahoo.com/s/tse.php";
      HtmlWeb webClient = new HtmlWeb();
      //網頁特殊編碼
      webClient.OverrideEncoding = System.Text.Encoding.GetEncoding(950);

      // 載入網頁資料 
      HtmlAgilityPack.HtmlDocument doc = webClient.Load(surl);
      // 裝載查詢結果 
      HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table[2]/tr/td/table/tr[2]/td");
      
      int idx = 0;
      foreach (HtmlNode node in nodes)
      {
        //目前成交價
        if (idx == 2)
        {
          sr.z = node.InnerText;
        }

        //yahoo漲跌
        if (idx == 5)
        {
          if (node.InnerText.Length > 0)
          {
            sr.xx = node.InnerText.Substring(0, 1);
          }

        }

        //昨收
        if (idx == 7)
        {
          sr.y = node.InnerText;
        }

        idx++;
      }


      //使用api呼叫取得
      if (stockcode == "0000" || stockcode == "9999")
      {
        StockRuntime sr_0000 = getStockRealtimeYahooApi(stockcode);

        sr = sr_0000;
      }

      //取得資訊
      ssql = " select * from StockInfo where stockcode = '{0}' ";
      StockInfo si = dbDapper.QuerySingleOrDefault<StockInfo>(string.Format(ssql, stockcode));
      if (si != null)
      {
        //名稱
        sr.n = si.stockname;
        //代碼
        sr.c = si.stockcode;
      }


      

      return sr;
    }

    public StockRuntime getStockRealtimeYahooApi(string stockcode)
    {
      StockRuntime sr = new StockRuntime();
      sr.z = "";
      sr.y = "";
      sr.u = "";
      sr.w = "";
      sr.n = "";
      sr.c = "";
      sr.xx = "";

      try
      {
        string sUrl = "https://tw.quote.finance.yahoo.net/quote/q?type=tick&sym={0}";
        string sStockcodeUrl = "";
        sStockcodeUrl = stockcode;
        if (stockcode == "0000") sStockcodeUrl = "%23001";
        if (stockcode == "9999")
        {
          sStockcodeUrl = "WTX%26";
          sUrl = "https://tw.screener.finance.yahoo.net/future/q?type=tick&mkt=01&sym={0}";
        }

        using (WebClient wc = StockHelper.getNewWebClient())
        {
          sUrl = string.Format(sUrl, sStockcodeUrl);
          string text = wc.DownloadString(sUrl);

          text = text.Replace("null(", "");
          text = text.Replace(");", "");
          text = text.Replace(",\"tick\":[]}", "");
          text = text.Replace(text.Substring(0,text.IndexOf(",\"mem\":")+7), "");

          if (text.Contains(",\"143\":"))
          {
            text = text.Insert(text.IndexOf(",\"143\":") + 7, "\"").Insert(text.IndexOf(",\"143\":") + 14, "\"");
          }
          
          JObject jobj = JObject.Parse(text);

          //Price
          if (jobj["125"] !=null)
            sr.z = jobj["125"].ToString();
          ////昨收
          if (jobj["129"] != null)
            sr.y = jobj["129"].ToString();
          ////最高
          //if (jobj["mem"]["130"] != null)
          //  sr.u = jobj["mem"]["130"].ToString();
          ////最低
          //if (jobj["mem"]["131"] != null)
          //  sr.w = jobj["mem"]["131"].ToString();
          if (stockcode != "0000" && stockcode != "9999")
          {
            //LimitUp
            if (jobj["132"] != null)
              sr.u = jobj["132"].ToString();
            //LimitDw
            if (jobj["133"] != null)
              sr.w = jobj["133"].ToString();
          }        
        }

        //取得個股資訊
        ssql = " select * from StockInfo where stockcode = '{0}' ";
        StockInfo si = dbDapper.QuerySingleOrDefault<StockInfo>(string.Format(ssql, stockcode));
        if (si != null)
        {
          //個股名稱
          sr.n = si.stockname;
          //個股代碼
          sr.c = si.stockcode;
        }
      }
      catch (Exception ex)
      {
        //logger.Error(ex);
      }

      return sr;
    }

    private decimal getPriceLimitUpOrDown(string limitType, string  sPriceClose)
    {
      // reference: http://stock7.0123456789.tw/


      double price = 0;
      double.TryParse(sPriceClose, out price);

      //double price = dClose;
      //double limitUp = price * (PriceToday.Date >= new DateTime(2015, 6, 1) ? 1.10 : 1.07);
      //double limitDown = price * (PriceToday.Date >= new DateTime(2015, 6, 1) ? 0.90 : 0.93);
      double limitUp = price * 1.10 ;
      double limitDown = price * 0.90 ;
      double STOCKUP = 0, STOCKDW = 0;
      if (limitUp < 10 && limitDown < 10)
      {
        STOCKUP = ((Math.Floor((Math.Floor(limitUp * 100) * 100))) / 100) / 100;
        STOCKDW = ((Math.Floor((Math.Ceiling(limitDown * 100) * 100))) / 100) / 100;
      }
      else if (limitUp > 10 && limitDown < 10)
      {
        STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.05) * 0.05) * 100) * 100)) / 100) / 100;
        STOCKDW = ((Math.Floor((Math.Ceiling(limitDown * 100) * 100))) / 100) / 100;
      }
      else if (limitUp >= 10 && limitDown >= 10 && limitUp <= 50 && limitDown < 50)
      {
        STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.05) * 0.05) * 100) * 100)) / 100) / 100;
        STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 0.05) * 0.05) * 100) * 100)) / 100) / 100;
      }
      else if (limitUp >= 50 && limitDown >= 50 && limitUp < 100 && limitDown < 100)
      {
        STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.1) * 0.1) * 100) * 100)) / 100) / 100;
        STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 0.1) * 0.1) * 100) * 100)) / 100) / 100;
      }
      else if (limitUp >= 50 && limitDown < 50)
      {
        STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.1) * 0.1) * 100) * 100)) / 100) / 100;
        STOCKDW = (Math.Floor((Math.Ceiling(limitDown / 0.05) * 0.05) * 100)) / 100;
      }
      else if (limitUp >= 100 && limitDown >= 100 && limitUp < 1000 && limitDown < 1000)
      {
        STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.5) * 0.5) * 100) * 100)) / 100) / 100;
        STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 0.5) * 0.5) * 100) * 100)) / 100) / 100;
      }
      else if (limitUp >= 100 && limitDown < 100)
      {
        STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.5) * 0.5) * 100) * 100)) / 100) / 100;
        STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 0.1) * 0.1) * 100) * 100)) / 100) / 100;
      }
      else if (limitUp >= 1000 && limitDown <= 1000)
      {
        STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 5) * 5) * 100) * 100)) / 100) / 100;
        STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 5) * 5) * 100) * 100)) / 100) / 100;
      }
      else if (limitUp >= 1000 && limitDown >= 1000)
      {
        STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 5) * 5) * 100) * 100)) / 100) / 100;
        STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 5) * 5) * 100) * 100)) / 100) / 100;
      }
     
      STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.5) * 0.5) * 100) * 100)) / 100) / 100;
      STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 0.5) * 0.5) * 100) * 100)) / 100) / 100;

      if (limitType.ToUpper() == "UP")
      {
        return Convert.ToDecimal(STOCKUP);
      }
      else
      {
        return Convert.ToDecimal(STOCKDW);
      }
    }


  }
}