using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using M10.lib.model;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace M10.lib
{
  public class StockHelper : M10BaseClass
  {
    




    public static WebClient getNewWebClient()
    {
      var wc = new WebClient();
      //wc.Headers.Add("User-Agent", HttpHelper.GetRandomAgent());
      wc.Encoding = Encoding.UTF8;
      wc.Proxy = null;
      return wc;
    }

    public bool GetStockAfterTse(DateTime GetDatetime)
    {
      //bool bResult = false;
      string sLineTrans = "";
      try
      {
        string sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1);
        string sUrl = string.Format(M10Const.StockAfterTseUrl, sDate);


        using (WebClient wc = getNewWebClient())
        {
          wc.Encoding = Encoding.GetEncoding(950);
          string text = wc.DownloadString(sUrl);


          List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

          foreach (string LoopItem in StringList)
          {
            string Line = LoopItem;
            Line = Line.Replace(" ", "");
            Line = Line.Replace("\",\"", "|");
            Line = Line.Replace("\"", "");
            Line = Line.Replace(",", "");
            Line = Line.Replace("=", "");
            sLineTrans = Line;
            string[] aCol = Line.Split('|');

            if (aCol.Length == 16)
            {
              //檢核資料
              Decimal iCheck = -1;

              if (Decimal.TryParse(aCol[8], out iCheck) == false)
              {
                continue;
              }

              ssql = " select * from stockafter  where stockdate = '{0}' and stockcode = '{1}'  ";
              Stockafter sa = dbDapper.QuerySingleOrDefault<Stockafter>(string.Format(ssql, sDate, aCol[0]));

              decimal dpricelastbuy = 0;
              decimal.TryParse(aCol[11], out dpricelastbuy);

              decimal dpricelastsell = 0;
              decimal.TryParse(aCol[13], out dpricelastsell);

              if (aCol[9] == "") aCol[9] = "X";

              //計算昨收
              Decimal dPriceYesterday = CalcPriceYesterday(aCol[8], aCol[9], aCol[10]);

              if (sa == null)
              {
                sa = new Stockafter();
                sa.stockdate = sDate;
                sa.stocktype = M10Const.StockType.tse;
                sa.stockcode = aCol[0];
                sa.pricelast = Convert.ToDecimal(aCol[8]);
                sa.updown = aCol[9];
                sa.pricediff = aCol[10];
                sa.priceopen = Convert.ToDecimal(aCol[5]);
                sa.pricetop = Convert.ToDecimal(aCol[6]);
                sa.pricelow = Convert.ToDecimal(aCol[7]);
                sa.priceavg = 0;
                sa.dealnum = Convert.ToInt64(aCol[2]);
                sa.dealmoney = Convert.ToInt64(aCol[4]);
                sa.dealamount = Convert.ToInt64(aCol[3]);
                sa.pricelastbuy = dpricelastbuy;
                sa.pricelastsell = dpricelastsell;
                sa.publicnum = 0;
                sa.pricenextday = Convert.ToDecimal(aCol[8]);
                sa.pricenextlimittop = 0;
                sa.pricenextlimitlow = 0;
                sa.priceyesterday = dPriceYesterday;
                sa.updatetime = Utils.getDatatimeString();
                
                dbDapper.Insert(sa);
              }
              else
              {
                sa.stocktype = M10Const.StockType.tse;
                sa.pricelast = Convert.ToDecimal(aCol[8]);
                sa.updown = aCol[9];
                sa.pricediff = aCol[10];
                sa.priceopen = Convert.ToDecimal(aCol[5]);
                sa.pricetop = Convert.ToDecimal(aCol[6]);
                sa.pricelow = Convert.ToDecimal(aCol[7]);
                sa.priceavg = 0;
                sa.dealnum = Convert.ToInt64(aCol[2]);
                sa.dealmoney = Convert.ToInt64(aCol[4]);
                sa.dealamount = Convert.ToInt64(aCol[3]);
                sa.pricelastbuy = dpricelastbuy;
                sa.pricelastsell = dpricelastsell;
                sa.publicnum = 0;
                sa.pricenextday = Convert.ToDecimal(aCol[8]);
                sa.pricenextlimittop = 0;
                sa.pricenextlimitlow = 0;
                sa.updatetime = Utils.getDatatimeString();
                sa.priceyesterday = dPriceYesterday;
                dbDapper.Update(sa);
              }
            }

          }
        }

        StockLog sl = new StockLog();
        sl.logdate = Utils.getDateString(DateTime.Now, M10Const.DateStringType.ADT1);
        sl.logdatetime = Utils.getDatatimeString();
        sl.logstatus = M10Const.StockLogStatus.s200;
        sl.memo = "";
        sl.logtype = M10Const.StockLogType.StockAfterTse;
        dbDapper.Insert(sl);
      }
      catch (Exception ex)
      {
        logger.Error(ex, "stock after:" + sLineTrans);
        //System.Threading.Thread.Sleep(10000);
        return false;
      }

      return true;
    }
    
    public bool GetStockAfterOtc(DateTime GetDatetime)
    {
      try
      {
        string sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ChineseT2);
        string sUrl = string.Format(M10Const.StockAfterOtcUrl, sDate);
        sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1);


        using (WebClient wc = getNewWebClient())
        {
          wc.Encoding = Encoding.GetEncoding(950);
          string text = wc.DownloadString(sUrl);


          List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

          foreach (string LoopItem in StringList)
          {
            string Line = LoopItem;
            Line = Line.Replace(" ", "");
            Line = Line.Replace("\",\"", "|");
            Line = Line.Replace("\"", "");
            Line = Line.Replace(",", "");
            Line = Line.Replace("=", "");

            string[] aCol = Line.Split('|');

            if (aCol.Length == 17)
            {
              //檢核資料
              Decimal iCheck = -1;

              if (Decimal.TryParse(aCol[2], out iCheck) == false)
              {
                continue;
              }

              //資訊整理
              string sDiff = aCol[3];
              string sUpdown = "X";
              string sPricediff = "0.00";
              if (sDiff.Length > 0)
              {
                string sCheck = sDiff.Substring(0, 1);
                if (sCheck == "+")
                {
                  sUpdown = "+";
                  sPricediff = sDiff.Replace("+", "");
                }
                if (sCheck == "-")
                {
                  sUpdown = "-";
                  sPricediff = sDiff.Replace("-", "");
                }
                if (sCheck == "0")
                {
                  sUpdown = "X";
                }

                if (sCheck != "+" && sCheck != "-" && sCheck != "0")
                {
                  sUpdown = "X";
                }
              }

              //計算昨收
              Decimal dPriceYesterday = CalcPriceYesterday(aCol[2], sUpdown, sPricediff);


              ssql = " select * from stockafter  where stockdate = '{0}' and stockcode = '{1}'  ";
              Stockafter sa = dbDapper.QuerySingleOrDefault<Stockafter>(string.Format(ssql, sDate, aCol[0]));

              if (aCol[0] == "3226")
              {
                string aaaa = string.Empty;
              }
              if (sa == null)
              {
                sa = new Stockafter();
                sa.stockdate = sDate;
                sa.stocktype = M10Const.StockType.otc;
                sa.stockcode = aCol[0];
                sa.pricelast = Convert.ToDecimal(aCol[2]);
                sa.updown = sUpdown;
                sa.pricediff = sPricediff;
                sa.priceopen = Convert.ToDecimal(aCol[4]);
                sa.pricetop = Convert.ToDecimal(aCol[5]);
                sa.pricelow = Convert.ToDecimal(aCol[6]);
                sa.priceavg = Convert.ToDecimal(aCol[7]);
                sa.dealnum = Convert.ToInt64(aCol[8]);
                sa.dealmoney = Convert.ToInt64(aCol[9]);
                sa.dealamount = Convert.ToInt64(aCol[10]);
                sa.pricelastbuy = Convert.ToDecimal(aCol[11]);
                sa.pricelastsell = Convert.ToDecimal(aCol[12]);
                sa.publicnum = Convert.ToInt64(aCol[13]);
                sa.pricenextday = Convert.ToDecimal(aCol[14]);
                sa.pricenextlimittop = Convert.ToDecimal(aCol[15]);
                sa.pricenextlimitlow = Convert.ToDecimal(aCol[16]);
                sa.updatetime = Utils.getDatatimeString();
                sa.priceyesterday = dPriceYesterday;
                dbDapper.Insert(sa);
              }
              else
              {
                sa.stocktype = M10Const.StockType.otc;
                sa.pricelast = Convert.ToDecimal(aCol[2]);
                sa.updown = sUpdown;
                sa.pricediff = sPricediff;
                sa.priceopen = Convert.ToDecimal(aCol[4]);
                sa.pricetop = Convert.ToDecimal(aCol[5]);
                sa.pricelow = Convert.ToDecimal(aCol[6]);
                sa.priceavg = Convert.ToDecimal(aCol[7]);
                sa.dealnum = Convert.ToInt64(aCol[8]);
                sa.dealmoney = Convert.ToInt64(aCol[9]);
                sa.dealamount = Convert.ToInt64(aCol[10]);
                sa.pricelastbuy = Convert.ToDecimal(aCol[11]);
                sa.pricelastsell = Convert.ToDecimal(aCol[12]);
                sa.publicnum = Convert.ToInt64(aCol[13]);
                sa.pricenextday = Convert.ToDecimal(aCol[14]);
                sa.pricenextlimittop = Convert.ToDecimal(aCol[15]);
                sa.pricenextlimitlow = Convert.ToDecimal(aCol[16]);
                sa.updatetime = Utils.getDatatimeString();
                sa.priceyesterday = dPriceYesterday;
                dbDapper.Update(sa);
              }


            }


          }

        }

        StockLog sl = new StockLog();
        sl.logdate = Utils.getDateString(DateTime.Now, M10Const.DateStringType.ADT1);
        sl.logdatetime = Utils.getDatatimeString();
        sl.logstatus = M10Const.StockLogStatus.s200;
        sl.memo = "";
        sl.logtype = M10Const.StockLogType.StockAfterOtc;
        dbDapper.Insert(sl);


        
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        System.Threading.Thread.Sleep(10000);
        return false;
      }

      return true;
    }
    
    public bool GetStockThreeTradeTse(DateTime GetDatetime)
    {
      try
      {
        string sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1);
        string sUrl = string.Format(M10Const.StockThreeTradeTse, sDate);

        using (WebClient wc = getNewWebClient())
        {
          wc.Encoding = Encoding.GetEncoding(950);
          string text = wc.DownloadString(sUrl);


          List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

          foreach (string LoopItem in StringList)
          {
            string Line = LoopItem;
            Line = Line.Replace(" ", "");
            Line = Line.Replace("\",\"", "|");
            Line = Line.Replace("\"", "");
            Line = Line.Replace(",", "");
            Line = Line.Replace("=", "");

            string[] aCol = Line.Split('|');

            if (aCol.Length == 16)
            {
              //檢核資料
              int iCheck = -1;

              if (int.TryParse(aCol[15], out iCheck) == false)
              {
                continue;
              }

              ssql = " select * from Stockthreetrade where date = '{0}' and stockcode = '{1}' ";
              Stockthreetrade st = dbDapper.QuerySingleOrDefault<Stockthreetrade>(string.Format(ssql, sDate, aCol[0]));

              if (st == null)
              {
                st = new Stockthreetrade();
                st.stockcode = aCol[0];
                st.date = sDate;
                st.type = M10Const.StockType.tse;
                st.foreigninv = Convert.ToInt32(aCol[4]);
                st.trustinv = Convert.ToInt32(aCol[7]);
                st.selfempinv = Convert.ToInt32(aCol[14]);
                st.threeinv = Convert.ToInt32(aCol[15]);
                st.updatetime = Utils.getDatatimeString();
                dbDapper.Insert(st);
              }
              else
              {
                st.stockcode = aCol[0];
                st.date = sDate;
                st.type = M10Const.StockType.tse;
                st.foreigninv = Convert.ToInt32(aCol[4]);
                st.trustinv = Convert.ToInt32(aCol[7]);
                st.selfempinv = Convert.ToInt32(aCol[14]);
                st.threeinv = Convert.ToInt32(aCol[15]);
                st.updatetime = Utils.getDatatimeString();
                dbDapper.Update(st);
              }
            }

            if (aCol.Length == 12)
            {
              //檢核資料
              int iCheck = -1;

              if (int.TryParse(aCol[11], out iCheck) == false)
              {
                continue;
              }

              ssql = " select * from Stockthreetrade where date = '{0}' and stockcode = '{1}' ";
              Stockthreetrade st = dbDapper.QuerySingleOrDefault<Stockthreetrade>(string.Format(ssql, sDate, aCol[0]));

              if (st == null)
              {
                st = new Stockthreetrade();
                st.stockcode = aCol[0];
                st.date = sDate;
                st.type = M10Const.StockType.tse;
                st.foreigninv = Convert.ToInt32(aCol[4]);
                st.trustinv = Convert.ToInt32(aCol[7]);
                st.selfempinv = Convert.ToInt32(aCol[10]);
                st.threeinv = Convert.ToInt32(aCol[11]);
                st.updatetime = Utils.getDatatimeString();
                dbDapper.Insert(st);
              }
              else
              {
                st.type = M10Const.StockType.tse;
                st.foreigninv = Convert.ToInt32(aCol[4]);
                st.trustinv = Convert.ToInt32(aCol[7]);
                st.selfempinv = Convert.ToInt32(aCol[10]);
                st.threeinv = Convert.ToInt32(aCol[11]);
                st.updatetime = Utils.getDatatimeString();
                dbDapper.Update(st);
              }


            }
          }
        }


        StockLog sl = new StockLog();
        sl.logdate = Utils.getDateString(DateTime.Now, M10Const.DateStringType.ADT1);
        sl.logdatetime = Utils.getDatatimeString();
        sl.logstatus = M10Const.StockLogStatus.s200;
        sl.memo = "";
        sl.logtype = M10Const.StockLogType.StockThreeTse;
        dbDapper.Insert(sl);

      }
      catch (Exception ex)
      {
        logger.Error(ex);
        System.Threading.Thread.Sleep(10000);
        return false;
      }

      return true;
    }

    public bool GetStockThreeTradeOtc(DateTime GetDatetime)
    {
      try
      {
        string sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ChineseT2);
        string sUrl = string.Format(M10Const.StockThreeTradeOtc, sDate);
        //改為寫入資料庫格式
        sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1);

        using (WebClient wc = getNewWebClient())
        {
          wc.Encoding = Encoding.GetEncoding(950);
          string text = wc.DownloadString(sUrl);


          List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

          foreach (string LoopItem in StringList)
          {
            string Line = LoopItem;
            Line = Line.Replace(" ", "");
            Line = Line.Replace("\",\"", "|");
            Line = Line.Replace("\"", "");
            Line = Line.Replace(",", "");
            Line = Line.Replace("=", "");

            string[] aCol = Line.Split('|');

            if (aCol.Length == 16)
            {
              //檢核資料
              int iCheck = -1;

              if (int.TryParse(aCol[15], out iCheck) == false)
              {
                continue;
              }

              ssql = " select * from Stockthreetrade where date = '{0}' and stockcode = '{1}' ";
              Stockthreetrade st = dbDapper.QuerySingleOrDefault<Stockthreetrade>(string.Format(ssql, sDate, aCol[0]));

              if (st == null)
              {
                st = new Stockthreetrade();
                st.stockcode = aCol[0];
                st.date = sDate;
                st.type = M10Const.StockType.otc;
                st.foreigninv = Convert.ToInt32(aCol[4]);
                st.trustinv = Convert.ToInt32(aCol[7]);
                st.selfempinv = Convert.ToInt32(aCol[14]);
                st.threeinv = Convert.ToInt32(aCol[15]);
                st.updatetime = Utils.getDatatimeString();
                dbDapper.Insert(st);
              }
              else
              {
                st.type = M10Const.StockType.otc;
                st.foreigninv = Convert.ToInt32(aCol[4]);
                st.trustinv = Convert.ToInt32(aCol[7]);
                st.selfempinv = Convert.ToInt32(aCol[14]);
                st.threeinv = Convert.ToInt32(aCol[15]);
                st.updatetime = Utils.getDatatimeString();
                dbDapper.Update(st);
              }
            }
          }
        }

        StockLog sl = new StockLog();
        sl.logdate = Utils.getDateString(DateTime.Now, M10Const.DateStringType.ADT1);
        sl.logdatetime = Utils.getDatatimeString();
        sl.logstatus = M10Const.StockLogStatus.s200;
        sl.memo = "";
        sl.logtype = M10Const.StockLogType.StockThreeOtc;
        dbDapper.Insert(sl);
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        System.Threading.Thread.Sleep(10000);
        return false;
      }

      return true;
    }

    public bool GetStockInfo()
    {
      try
      {
        List<string> TypeList = new List<string>();
        TypeList.Add(M10Const.StockType.tse);
        TypeList.Add(M10Const.StockType.otc);
        
        foreach (string sType in TypeList)
        {
          string surl = "";

          if (sType == M10Const.StockType.tse) surl = M10Const.StockInfoTse;
          if (sType == M10Const.StockType.otc) surl = M10Const.StockInfoOtc;

          HtmlWeb webClient = new HtmlWeb();
          //網頁特殊編碼
          webClient.OverrideEncoding = Encoding.GetEncoding(950);

          // 載入網頁資料 
          HtmlDocument doc = webClient.Load(surl);

          // 裝載查詢結果 
          HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table[2]/tr");

          if (nodes.Count > 0)
          {
            ssql = " update stockinfo set updstatus = 'N' where type = '{0}'  ";
            dbDapper.Execute(string.Format(ssql, sType));
          }
          
          foreach (HtmlNode node in nodes)
          {
            string sCode = "";
            string sName = "";
            
            HtmlNodeCollection tdnodes = node.SelectNodes("td");

            if (tdnodes.Count > 0)
            {
              HtmlNode tdnode = tdnodes[0];
              string[] StockInfoSplit = tdnode.InnerText.Split('　');

              if (StockInfoSplit.Length != 2) continue;

              sCode = StockInfoSplit[0];
              sName = StockInfoSplit[1];

              //判斷代碼存在則更新，不存在新增
              ssql = " select * from stockinfo where stockcode = '{0}' ";
              StockInfo StockInfoItem = dbDapper.QuerySingleOrDefault<StockInfo>(string.Format(ssql, sCode));

              if (StockInfoItem == null) //不存在新增
              { 
                StockInfoItem = new StockInfo();
                StockInfoItem.stockcode = sCode;
                StockInfoItem.stockname = sName;
                StockInfoItem.type = sType;
                StockInfoItem.updatetime = Utils.getDatatimeString();
                StockInfoItem.updstatus = "Y";
                StockInfoItem.status = "Y";

                dbDapper.Insert(StockInfoItem);
              }
              else
              {
                StockInfoItem.type = sType;
                StockInfoItem.updatetime = Utils.getDatatimeString();
                StockInfoItem.updstatus = "Y";
                StockInfoItem.status = "Y";

                dbDapper.Update(StockInfoItem);
              }
            }
            
          }

          ssql = "update stockinfo set status = 'N' where updstatus = 'N' ";
          dbDapper.Execute(ssql);


          StockLog sl = new StockLog();
          sl.logdate = Utils.getDateString(DateTime.Now, M10Const.DateStringType.ADT1);
          sl.logdatetime = Utils.getDatatimeString();
          sl.logstatus = M10Const.StockLogStatus.s200;
          sl.memo = "";
          if (sType == M10Const.StockType.tse) {
            sl.logtype = M10Const.StockLogType.StockInfoTse;
          }
          if (sType == M10Const.StockType.otc) {
            sl.logtype = M10Const.StockLogType.StockInfoOtc;
          }
          dbDapper.Insert(sl);

        }
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        return false;
      }

      return true;
    }

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
          text = text.Replace(text.Substring(0, text.IndexOf(",\"mem\":") + 7), "");

          if (text.Contains(",\"143\":"))
          {
            text = text.Insert(text.IndexOf(",\"143\":") + 7, "\"").Insert(text.IndexOf(",\"143\":") + 14, "\"");
          }

          JObject jobj = JObject.Parse(text);

          //Price
          if (jobj["125"] != null)
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

        sr.status = M10Const.StockRuntimeStatus.YahooApi;

      }
      catch (Exception ex)
      {
        logger.Error(ex);
      }

      return sr;
    }

    private decimal getPriceLimitUpOrDown(string limitType, string sPriceClose)
    {
      // reference: http://stock7.0123456789.tw/


      double price = 0;
      double.TryParse(sPriceClose, out price);

      //double price = dClose;
      //double limitUp = price * (PriceToday.Date >= new DateTime(2015, 6, 1) ? 1.10 : 1.07);
      //double limitDown = price * (PriceToday.Date >= new DateTime(2015, 6, 1) ? 0.90 : 0.93);
      double limitUp = price * 1.10;
      double limitDown = price * 0.90;
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


    public decimal CalcPriceYesterday(string price,string updown,string pricediff) {
      Decimal dPrice = 0;
      Decimal dPricediff = 0;
      Decimal dResult = 0;

      try
      {
        //異常回傳0
        if (Decimal.TryParse(price, out dPrice) == false || Decimal.TryParse(pricediff, out dPricediff) == false)
        {
          return 0;
        }
        
        //判斷平
        if (dPricediff == 0 || updown == "" || updown == "X")
        {
          return dPrice;
        }

        if (updown == "+")
        {
          dResult = dPrice - dPricediff;
        }

        if (updown == "-")
        {
          dResult = dPrice + dPricediff;
        }

      }
      catch (Exception)
      {
        dResult = 0;

      }
      



      return dResult;
    }

    public static Stream GenerateStreamFromString(string s)
    {
      MemoryStream stream = new MemoryStream();
      StreamWriter writer = new StreamWriter(stream);
      writer.Write(s);
      writer.Flush();
      stream.Position = 0;
      return stream;
    }
    

  }
}
