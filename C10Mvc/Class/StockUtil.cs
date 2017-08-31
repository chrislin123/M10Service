using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using M10.lib.model;
using M10.lib;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace C10Mvc.Class
{
  public  class StockUtil : BaseClass
  {


    public StockRuntime getStockRealtimeYahooWeb(string stockcode)
    {
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


      //使用api呼叫取得
      if (stockcode == "0000" || stockcode == "t00")
      {
        StockRuntime sr_0000 = getStockRealtimeYahooApi(stockcode);

        sr = sr_0000;
        sr.n = "加權指數";
        sr.c = "0000";
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

//https://tw.quote.finance.yahoo.net/quote/q?type=tick&sym=%23001
//https://tw.quote.finance.yahoo.net/quote/q?type=tick&sym=3019
//https://tw.quote.finance.yahoo.net/quote/q?type=tick&sym=3162

//(OK)
//成
//"125":115.5,

//----
//(OK)
//pricediff
//"184":-1.5,
                                                        
//----
//(OK)
//昨收

//"129":27.6,
//27.6

//(OK)
//最高
//"130":120.5,
//"130":29.65,


//(OK)
//最低
//"131":462.5,


      try
      {
        if (stockcode == "0000") stockcode = "%23001";
        if (stockcode == "t00") stockcode = "%23001";

        string sUrl = "https://tw.quote.finance.yahoo.net/quote/q?type=tick&sym={0}";
        string sDate = DateTime.Now.ToString("yyyyMMdd");

    
        sUrl = string.Format(sUrl, stockcode);
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sUrl);
        req.Proxy = null;
        //改為寫入資料庫格式

        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

        //判斷http回應狀態(HttpStatusCode.OK=200)
        if (resp.StatusCode != HttpStatusCode.OK)
        {
          return sr;
        }

        //using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.GetEncoding(950)))
        using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.UTF8))
        {
          string Line;
          while ((Line = SR.ReadLine()) != null)
          {
            Line = Line.Replace("null(", "");
            Line = Line.Replace(");", "");

            //JObject jobj = JsonConvert.DeserializeObject(Line) as JObject;
            if (stockcode == "%23001")
            {
              Line = Line.Insert(Line.IndexOf(",\"143\":") + 7, "\"").Insert(Line.IndexOf(",\"143\":") + 14, "\"");
            }
            

            JObject jobj = JObject.Parse(Line);
            //jobj["mem"]["125"];

            //Price
            sr.z = jobj["mem"]["125"].ToString();

        
            //昨收
            sr.y = jobj["mem"]["129"].ToString();
            //最高
            sr.u = jobj["mem"]["130"].ToString();
            //最低
            sr.w = jobj["mem"]["131"].ToString();

            //  if(StockInfo.z == StockInfo.u) mark="▲";
            //  if(StockInfo.z == StockInfo.w) mark="▼";
            //  if(ud>0) mark="△";
            //  if(ud<0) mark="▽";
            //var mark = "±";
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
        //throw ex;
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