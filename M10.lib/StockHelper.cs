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
        TypeList.Add(M10Const.StockType.otc1);
        
        foreach (string sType in TypeList)
        {
          GetStockInfoSub(sType);
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        return false;
      }

      return true;
    }

    public void GetStockInfoSub(string sType) {
      string surl = "";

      if (sType == M10Const.StockType.tse) surl = M10Const.StockInfoTse;
      if (sType == M10Const.StockType.otc) surl = M10Const.StockInfoOtc;
      if (sType == M10Const.StockType.otc1) surl = M10Const.StockInfoOtc1;

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

      //興櫃轉上櫃
      if (sType == M10Const.StockType.otc1)
      {
        ssql = "update stockinfo set type = 'otc' where type = 'otc1' ";
        dbDapper.Execute(ssql);
      }

      StockLog sl = new StockLog();
      sl.logdate = Utils.getDateString(DateTime.Now, M10Const.DateStringType.ADT1);
      sl.logdatetime = Utils.getDatatimeString();
      sl.logstatus = M10Const.StockLogStatus.s200;
      sl.memo = "";
      if (sType == M10Const.StockType.tse)
      {
        sl.logtype = M10Const.StockLogType.StockInfoTse;
      }
      if (sType == M10Const.StockType.otc)
      {
        sl.logtype = M10Const.StockLogType.StockInfoOtc;
      }
      if (sType == M10Const.StockType.otc1)
      {
        sl.logtype = M10Const.StockLogType.StockInfoOtc1;
      }
      dbDapper.Insert(sl);


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
      sr.a = "";
      sr.TradeDay = "";

      string sJson = "";
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
            //1061219 修改為取代
            //text = text.Insert(text.IndexOf(",\"143\":") + 7, "\"").Insert(text.IndexOf(",\"143\":") + 14, "\"");

            int iStart = text.IndexOf(",\"143\":");
            int iEnd = text.IndexOf(",", iStart + 1);
            string sRep = text.Substring(iStart, iEnd - iStart);

            text = text.Replace(sRep,"");
          }


          sJson = text;
          JObject jobj = JObject.Parse(text);

          //Price
          if (jobj["125"] != null)
            sr.z = jobj["125"].ToString();
          ////昨收
          if (jobj["129"] != null)
            sr.y = jobj["129"].ToString();
          //成交量
          if (jobj["404"] != null)
            sr.a = jobj["404"].ToString();

          //日期
          if (jobj["TradeDay"] != null)
            sr.TradeDay = jobj["TradeDay"].ToString();
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

        //大盤不顯示量
        if (stockcode == "0000") sr.a = "";

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
        logger.Error(ex, sJson);
      }

      return sr;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="limitType">UP , DOWN</param>
    /// <param name="sPriceClose"></param>
    /// <returns></returns>
    public decimal getPriceLimitUpOrDown(string limitType, string sPriceClose)
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

      //STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.5) * 0.5) * 100) * 100)) / 100) / 100;
      //STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 0.5) * 0.5) * 100) * 100)) / 100) / 100;

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



    protected void AppendParameter(StringBuilder sb, string name, string value)
    {
      string encodedValue = System.Web.HttpUtility.UrlEncode(value);
      sb.AppendFormat("{0}={1}&", name, encodedValue);
    }


    public bool GetStockBrokerBSTSE()
    {
      //string sStockCode = "2330";
      string sUrl = "http://bsr.twse.com.tw/bshtm/bsMenu.aspx";
      StringBuilder sbPostData = new StringBuilder();
      //AppendParameter(sbPostData, "stk_code", sStockCode);
      //AppendParameter(sbPostData, "topage", "1");

      byte[] byteArray = Encoding.UTF8.GetBytes(sbPostData.ToString());

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
      request.Proxy = null;
      request.Method = "POST";
      request.ContentType = "application/x-www-form-urlencoded";

      // 寫入 Post Body Message 資料流
      using (Stream requestStream = request.GetRequestStream())
      {
        requestStream.Write(byteArray, 0, byteArray.Length);
      }

      using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
      {



        HtmlDocument HtmlDoc = new HtmlDocument();
        HtmlDoc.Load(response.GetResponseStream(), Encoding.UTF8);
        //HtmlNodeCollection nodes1 = HtmlDoc.DocumentNode.SelectNodes("//form[@name='brokerBS2']");

        //if (nodes1 != null)
        //{
        //  foreach (HtmlNode node in nodes1)
        //  {
        //    //取得共有幾頁
        //    HtmlNodeCollection tdnodes = node.SelectNodes("a");

        //    foreach (HtmlNode item in tdnodes)
        //    {
        //      //PageList.Add(item.InnerText);
        //    }
        //  }
        //}
      }
      return true;
    }

    /// <summary>
    /// 券商買賣證券日報表查詢系統
    /// </summary>
    /// <returns></returns>
    public bool GetStockBrokerBS()
    {
      try
      {
        //網頁瀏覽停滯時間
        int iSec = 2;

        DateTime dtTread = DateTime.MinValue;

        //取得上櫃所有的股票代號
        ssql = " select * from stockinfo where type = 'otc' and status = 'Y' and len(stockcode) = 4 ";
        List<StockInfo> StockInfoList = dbDapper.Query<StockInfo>(ssql);

        foreach (StockInfo si in StockInfoList)
        {
          string sStockCode = si.stockcode;

          //未取交易日期，則取得目前提供資料的交易日期
          if (dtTread == DateTime.MinValue)
          {
            dtTread = getStockBrokerBSNow(sStockCode);
          }

          //判斷是否已經轉檔成功
          ssql = " select * from stocklog where logtype = '{0}' and logdate = '{1}' and  memo = '{2}'  ";
          StockLog slchk = dbDapper.QuerySingleOrDefault<StockLog>(string.Format(
            ssql, M10Const.StockLogType.StockBrokerBSOtc
            , Utils.getDateString(dtTread, M10Const.DateStringType.ADT1), sStockCode));
          if (slchk != null) continue;

          System.Threading.Thread.Sleep(iSec * 1000);

          List<string> PageList = new List<string>();
          string sUrl = "http://www.tpex.org.tw/web/stock/aftertrading/broker_trading/brokerBS.php?l=zh-tw";

          StringBuilder sbPostData = new StringBuilder();
          AppendParameter(sbPostData, "stk_code", sStockCode);
          AppendParameter(sbPostData, "topage", "1");

          byte[] byteArray = Encoding.UTF8.GetBytes(sbPostData.ToString());

          HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
          request.Proxy = null;
          request.Method = "POST";
          request.ContentType = "application/x-www-form-urlencoded";

          // 寫入 Post Body Message 資料流
          using (Stream requestStream = request.GetRequestStream())
          {
            requestStream.Write(byteArray, 0, byteArray.Length);
          }

          using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
          {
            HtmlDocument HtmlDoc = new HtmlDocument();
            HtmlDoc.Load(response.GetResponseStream(), Encoding.UTF8);
            HtmlNodeCollection nodes1 = HtmlDoc.DocumentNode.SelectNodes("//form[@name='brokerBS2']");

            if (nodes1 != null)
            {
              foreach (HtmlNode node in nodes1)
              {
                //取得共有幾頁
                HtmlNodeCollection tdnodes = node.SelectNodes("a");

                foreach (HtmlNode item in tdnodes)
                {
                  PageList.Add(item.InnerText);
                }
              }
            }            
          }

          //每個分頁進行資料取得
          foreach (string sPage in PageList)
          {
            //每一個分頁，暫停十秒鐘
            System.Threading.Thread.Sleep(iSec * 1000);

            sbPostData.Clear();
            AppendParameter(sbPostData, "stk_code", sStockCode);
            AppendParameter(sbPostData, "topage", sPage);

            byteArray = Encoding.UTF8.GetBytes(sbPostData.ToString());

            request = (HttpWebRequest)WebRequest.Create(sUrl);
            request.Proxy = null;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // 寫入 Post Body Message 資料流
            using (Stream requestStream = request.GetRequestStream())
            {
              requestStream.Write(byteArray, 0, byteArray.Length);
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
              HtmlDocument HtmlDoc = new HtmlDocument();
              HtmlDoc.Load(response.GetResponseStream(), Encoding.UTF8);
              HtmlNodeCollection MainTableNodes = HtmlDoc.DocumentNode.SelectNodes("//table[@id='data_01']");

              if (MainTableNodes == null) continue;

              //取得共有幾頁
              HtmlNodeCollection SubTableNodes = MainTableNodes[0].SelectNodes("//table");

              foreach (HtmlNode SubTableNode in SubTableNodes)
              {
                if (SubTableNode.Id == "data_01") continue;

                
                HtmlNodeCollection TrNodes = SubTableNode.SelectNodes("tr");

                foreach (HtmlNode TrNode in TrNodes)
                {
                  HtmlNodeCollection TdNodes = TrNode.SelectNodes("td");

                  //取得日期
                  if (TdNodes.Count == 4)
                  {
                    string sTreadDate = TdNodes[1].InnerText;
                    sTreadDate = sTreadDate.Replace("年", "").Replace("月", "").Replace("日", "");
                    dtTread = new DateTime(int.Parse(sTreadDate.Substring(0, 3)) + 1911, int.Parse(sTreadDate.Substring(3, 2)), int.Parse(sTreadDate.Substring(5, 2)));
                  }

                  if (TdNodes.Count != 5) continue;

                  if (TdNodes[1].InnerText == "券商") continue;

                  string sVoucher = TdNodes[1].InnerText;
                  string sVoucherCode = "";
                  string sVoucherName = "";
                  string[] aVoucher = sVoucher.Split(new string[] { "&nbsp;" }, StringSplitOptions.None);
                  if (aVoucher.Length == 2)
                  {
                    sVoucherCode = aVoucher[0];
                    sVoucherName = aVoucher[1];

                    //判斷Voucher資料是否存在，如不存在則新增一筆
                    ssql = " select * from stockvoucher where vouchercode = '{0}' ";
                    StockVoucher oStockVoucher = dbDapper.QuerySingleOrDefault<StockVoucher>(string.Format(ssql, sVoucherCode));

                    if (oStockVoucher == null)
                    {
                      //新增
                      oStockVoucher = new StockVoucher();
                      oStockVoucher.vouchercode = sVoucherCode;
                      oStockVoucher.vouchername = sVoucherName;
                      oStockVoucher.updatetime = Utils.getDatatimeString();
                      dbDapper.Insert(oStockVoucher);
                    }
                    else
                    {
                      //名稱不一樣進行更新
                      if (oStockVoucher.vouchername != sVoucherName)
                      {
                        oStockVoucher.vouchername = sVoucherName;
                        dbDapper.Update(oStockVoucher);
                      }
                    }
                  }

                  if (aVoucher.Length == 1)
                  {
                    sVoucherCode = aVoucher[0];
                  }

                  //判斷資料是否存在，
                  ssql = " select * from StockBrokerBS where stockcode = '{0}' and stockdate = '{1}' and bsno = '{2}' ";
                  StockBrokerBS oStockBrokerBS = dbDapper.QuerySingleOrDefault<StockBrokerBS>(string.Format(ssql
                    , sStockCode, Utils.getDateString(dtTread, M10Const.DateStringType.ADT1)
                    , TdNodes[0].InnerText));

                  if (oStockBrokerBS == null)
                  {
                    oStockBrokerBS = new StockBrokerBS();
                    oStockBrokerBS.stockcode = sStockCode;
                    oStockBrokerBS.stockdate = Utils.getDateString(dtTread, M10Const.DateStringType.ADT1);
                    oStockBrokerBS.vouchercode = sVoucherCode;
                    oStockBrokerBS.bsno = TdNodes[0].InnerText;
                    oStockBrokerBS.bsprice = Convert.ToDecimal(TdNodes[2].InnerText);
                    oStockBrokerBS.bvol = int.Parse(TdNodes[3].InnerText.Replace(",", ""));
                    oStockBrokerBS.svol = int.Parse(TdNodes[4].InnerText.Replace(",", ""));
                    oStockBrokerBS.updatetime = Utils.getDatatimeString();
                    dbDapper.Insert(oStockBrokerBS);
                  }
                  else
                  {
                    oStockBrokerBS.vouchercode = sVoucherCode;
                    oStockBrokerBS.bsprice = Convert.ToDecimal(TdNodes[2].InnerText);
                    oStockBrokerBS.bvol = int.Parse(TdNodes[3].InnerText.Replace(",", ""));
                    oStockBrokerBS.svol = int.Parse(TdNodes[4].InnerText.Replace(",", ""));
                    oStockBrokerBS.updatetime = Utils.getDatatimeString();
                    dbDapper.Update(oStockBrokerBS);
                  }
                }
              }

            }
          }

          //寫入log紀錄
          StockLog sl = new StockLog();
          sl.logdate = Utils.getDateString(dtTread, M10Const.DateStringType.ADT1);
          sl.logdatetime = Utils.getDatatimeString();
          sl.logstatus = M10Const.StockLogStatus.s200;
          sl.memo = sStockCode;
          sl.logtype = M10Const.StockLogType.StockBrokerBSOtc;
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

    protected DateTime getStockBrokerBSNow(string sStockCode)
    {
      DateTime dt = DateTime.MinValue;
      string sUrl = "http://www.tpex.org.tw/web/stock/aftertrading/broker_trading/brokerBS.php?l=zh-tw";

      StringBuilder sbPostData = new StringBuilder();
      AppendParameter(sbPostData, "stk_code", sStockCode);
      AppendParameter(sbPostData, "topage", "1");

      byte[] byteArray = Encoding.UTF8.GetBytes(sbPostData.ToString());

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
      request.Proxy = null;
      request.Method = "POST";
      request.ContentType = "application/x-www-form-urlencoded";

      // 寫入 Post Body Message 資料流
      using (Stream requestStream = request.GetRequestStream())
      {
        requestStream.Write(byteArray, 0, byteArray.Length);
      }

      using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
      {
        HtmlDocument HtmlDoc = new HtmlDocument();
        HtmlDoc.Load(response.GetResponseStream(), Encoding.UTF8);
        HtmlNodeCollection MainTableNodes = HtmlDoc.DocumentNode.SelectNodes("//table[@id='data_01']");
     
        //取得共有幾頁
        HtmlNodeCollection SubTableNodes = MainTableNodes[0].SelectNodes("//table");

        foreach (HtmlNode SubTableNode in SubTableNodes)
        {
          if (SubTableNode.Id == "data_01") continue;


          HtmlNodeCollection TrNodes = SubTableNode.SelectNodes("tr");

          foreach (HtmlNode TrNode in TrNodes)
          {
            HtmlNodeCollection TdNodes = TrNode.SelectNodes("td");

            //取得日期
            if (TdNodes.Count == 4)
            {
              string sTreadDate = TdNodes[1].InnerText;
              sTreadDate = sTreadDate.Replace("年", "").Replace("月", "").Replace("日", "");
              dt = new DateTime(int.Parse(sTreadDate.Substring(0, 3)) + 1911, int.Parse(sTreadDate.Substring(3, 2)), int.Parse(sTreadDate.Substring(5, 2)));
            }
          }
        }
      }

      return dt;
    }

    public List<dynamic> getStockDataLvstg(string stockcode, string date)
    {
      List<dynamic> StockafterList = new List<dynamic>();

      ssql = @" select stockcode, stockdate,pricelast as price from stockafter where stockcode = '{0}' and stockdate >= '{1}' order by stockdate asc ";
      StockafterList = dbDapper.Query(string.Format(ssql, stockcode, date));   

      return StockafterList;
    }


    /// <summary>
    /// 計算每日巨量換手
    /// </summary>
    public void GetHugeTurnover()
    {
      try
      {

        //預設今天資料
        string sRunDate = DateTime.Now.ToString("yyyyMMdd");

        //轉最新資料
        ssql = " select Max(stockdate) from stockafter where stockcode = '2330' ";
        object oMax = dbDapper.ExecuteScale(ssql);
        if (oMax != null)
        {
          sRunDate = Convert.ToString(Convert.ToInt32(oMax.ToString()));
        }


        sRunDate = "20180326";
        //刪除目前存在資料
        //dbDapper.Execute("delete stockget");

        List<StockInfo> siList = new List<StockInfo>();
        ssql = " select * from stockinfo where status = 'Y' and LEN(stockcode) = 4 order by stockcode ";
        siList = dbDapper.Query<StockInfo>(ssql);

        int iListTotal = siList.Count();
        int idex = 0;

        foreach (StockInfo item in siList)
        {
          idex++;
          string stockcode = item.stockcode;

          //stockcode = "3162";

          //判斷盤後資料是否存在
          ssql = " select * from StockAfter where stockcode  = '{0}' ";
          ssql = string.Format(ssql, stockcode);
          int iTotal = dbDapper.QueryTotalCount(ssql);
          if (iTotal == 0)
          {
            continue;
          }



          ssql = @" select top 3 * from stockafter 
               where stockdate <= '{0}' and stockcode = '{1}' order by stockdate desc
               ";
          ssql = string.Format(ssql, sRunDate, stockcode);
          List<Stockafter> saList = dbDapper.Query<Stockafter>(ssql);

          //if (saList.Count != 3)
          //{
          //  dt = dt.AddDays(-1);
          //  continue;
          //}

          if (saList.Count == 3)
          {
            //to
            Stockafter saToday = saList[0];
            //yes
            Stockafter saYes = saList[1];
            //per
            Stockafter saPer = saList[2];

            //判斷前一天+9%
            Boolean bCheckUp9 = checkUp9(saYes, saPer);
            //if (bCheckUp9 == false)
            //{
            //  dt = dt.AddDays(-1);
            //  continue;
            //}
            //判斷當天為180天最大量
            Boolean bCheck180Max = check180Max(saToday);

            //今天量是昨天的兩倍
            Boolean bCheckTodayOver2 = checkTodayOver2(saToday, saYes);


            if (bCheckUp9 == true && bCheck180Max == true && bCheckTodayOver2 == true)
            {
              //確認資料是否已存在，如果存在則判斷下一天
              ssql = " select * from StockGet where stockcode = '{0}' and stockdate = '{1}' and getdate  = '{2}' ";
              ssql = string.Format(ssql, stockcode, saToday.stockdate, sRunDate);
              StockGet sgcheck = dbDapper.QuerySingleOrDefault<StockGet>(ssql);
              if (sgcheck == null)
              {
                StockGet sg = new StockGet();
                sg.getdate = sRunDate;
                sg.stockcode = saToday.stockcode;
                sg.stockdate = saToday.stockdate;

                dbDapper.Insert(sg);
              }
              
            }


          }



          //DateTime dt = DateTime.Now;
          //DateTime dtTarget = dt.AddDays(-180);


          //while (true)
          //{
          //  if (dt.ToString("yyyyMMdd") == dtTarget.ToString("yyyyMMdd")) break;

          //  //if (stockcode == "1413")
          //  //{
          //  //  ssql = "";
          //  //}

          //  //if (dt.ToString("yyyyMMdd") == "20170818" && stockcode == "3162")
          //  //{
          //  //  ssql = "";
          //  //}

          //  //StatusLabel.Text = string.Format("[{2}/{3}]{0}({1})"
          //  //  , stockcode, dt.ToString("yyyyMMdd"), idex.ToString(), iListTotal.ToString());
          //  //Application.DoEvents();

            

          //  dt = dt.AddDays(-1);
          //  continue;

          //}
        }


        //StatusLabel.Text = "轉檔完畢";
        //Application.DoEvents();


      }
      catch (Exception ex)
      {
        logger.Error(ex, "巨量換手轉檔異常通報");
      }

    }  

    /// <summary>
    /// 判斷前一天上漲>9%
    /// </summary>
    /// <param name="saYes"></param>
    /// <param name="saPer"></param>
    /// <returns></returns>
    public Boolean checkUp9(Stockafter saYes, Stockafter saPer)
    {
      Boolean bResult = false;


      if (saYes.updown == "+")
      {
        double aa = 0.09;
        decimal dd = 0;
        dd = (Convert.ToDecimal(saYes.pricelast) - Convert.ToDecimal(saPer.pricelast)) / Convert.ToDecimal(saPer.pricelast);
        if (dd > Convert.ToDecimal(aa))
        {
          bResult = true;
        }
      }



      return bResult;
    }

    /// <summary>
    /// 當天是180天 最大量
    /// </summary>
    /// <param name="saYes"></param>
    /// <param name="saPer"></param>
    /// <returns></returns>
    public Boolean check180Max(Stockafter saToday)
    {
      Boolean bResult = false;


      ssql = @" select max(dealnum) from (
                      select top 180 * from stockafter where stockcode = '{1}' 
                      and stockdate <= '{0}'  order by stockdate desc
                      ) as dd 
                     ";
      ssql = string.Format(ssql, saToday.stockdate, saToday.stockcode);
      object sa = dbDapper.ExecuteScale(ssql);
      if (sa != null)
      {
        long lDealNum = Convert.ToInt64(sa.ToString());

        if (saToday.dealnum == lDealNum)
        {
          bResult = true;
        }
      }


      return bResult;
    }


    /// <summary>
    /// 今天的量是昨天的兩倍
    /// </summary>
    /// <param name="saToday"></param>
    /// <param name="saYes"></param>
    /// <returns></returns>
    public Boolean checkTodayOver2(Stockafter saToday, Stockafter saYes)
    {
      Boolean bResult = false;

      if (saToday.dealnum > saYes.dealnum * 2)
      {
        bResult = true;
      }

      return bResult;
    }




  }
}
