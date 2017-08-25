using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using C10Mvc.Class;
using M10.lib.model;
using HtmlAgilityPack;

namespace C10Mvc.Controllers
{
  [RoutePrefix("StockApi")]
  public class StockController : BaseApiController
  {

    [HttpGet]
    [Route("getStockType")]
    public StockInfo getStockType()
    {
      if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return null;

      NameValueCollection Params = M10.lib.Utils.ParseQueryString(ActionContext.Request.RequestUri.Query);


      string ssql = " select * from stockinfo where 1=1 and status = 'Y' ";
      if (Params["StockCode"] != null)
      {
        ssql += string.Format(" and stockcode = '{0}' ", Params["StockCode"]);
      }


      StockInfo StockInfoItem = dbDapper.QuerySingleOrDefault<StockInfo>(ssql);

      return StockInfoItem;
    }


    [HttpGet]
    [Route("getStockThreeTrade")]
    public dynamic getStockThreeTrade()
    {
      //if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return null;

      //NameValueCollection Params = M10.lib.Utils.ParseQueryString(ActionContext.Request.RequestUri.Query);

      string sMaxDate = "";
      string ssql = " select max(date) from Stockthreetrade ";

      var maxdate = dbDapper.ExecuteScale(ssql);
      if (maxdate != null)
      {
        sMaxDate = maxdate.ToString();
      }

      ssql = @"select top 30 * from Stockthreetrade a
                left join stockinfo b on a.stockcode = b.stockcode
                where threeinv > 0 
                and date = '{0}'  
                order by threeinv desc
                ";

      List<dynamic> ThreeTradeList = dbDapper.Query<dynamic>(string.Format(ssql, sMaxDate));

      List<dynamic> ListB = ThreeTradeList
                  .Select(t => new
                  {
                    stockcode = t.stockcode
                    ,
                    threeinv = t.threeinv
                                    ,
                    stockname = t.stockname
                  })
                  .ToList<dynamic>();

      ssql = @"select top 30 * from Stockthreetrade a
                left join stockinfo b on a.stockcode = b.stockcode
                where threeinv < 0
                and date = '{0}'  
                order by threeinv 
                ";

      ThreeTradeList = dbDapper.Query<dynamic>(string.Format(ssql, sMaxDate));

      List<dynamic> ListS = ThreeTradeList
                  .Select(t => new
                  {
                    stockcode = t.stockcode
                    ,
                    threeinv = t.threeinv
                                    ,
                    stockname = t.stockname
                  })
                  .ToList<dynamic>();

      dynamic dd = new { date = sMaxDate, s = ListS, b = ListB };

      return dd;
    }

    [HttpGet]
    [Route("getStockThreeTrade3d")]
    public dynamic getStockThreeTrade3d()
    {
      ////if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return null;

      ////NameValueCollection Params = M10.lib.Utils.ParseQueryString(ActionContext.Request.RequestUri.Query);

      string sMaxDate = "";
      string ssql = " select max(date) from Stockthreetrade ";

      var maxdate = dbDapper.ExecuteScale(ssql);
      if (maxdate != null)
      {
        sMaxDate = maxdate.ToString();
      }

      ssql = @"
            select top 30 * from (
              select stockcode,count(*) as tt , sum(threeinv) as threeinv  from (
                select * from Stockthreetrade
                where date in (
                  select top 3 * from (
                    select distinct date from Stockthreetrade 
                  ) as a order by a.date desc
                ) and threeinv > 0
              ) b group by stockcode
            ) as c 
            left join stockinfo d on c.stockcode = d.stockcode
            where c.tt =3 order by threeinv desc
            ";

      List<dynamic> ThreeTradeList = dbDapper.Query<dynamic>(ssql);

      List<dynamic> ListB3d = ThreeTradeList
                  .Select(t => new
                  {
                    stockcode = t.stockcode
                    ,
                    threeinv = t.threeinv
                                    ,
                    stockname = t.stockname
                  })
                  .ToList<dynamic>();

      ssql = @"
            select top 30 * from (
              select stockcode,count(*) as tt , sum(threeinv) as threeinv  from (
                select * from Stockthreetrade
                where date in (
                  select top 3 * from (
                    select distinct date from Stockthreetrade 
                  ) as a order by a.date desc
                ) and threeinv < 0
              ) b group by stockcode
            ) as c 
            left join stockinfo d on c.stockcode = d.stockcode
            where c.tt =3 order by threeinv asc
                ";

      ThreeTradeList = dbDapper.Query<dynamic>(string.Format(ssql, sMaxDate));

      List<dynamic> ListS3d = ThreeTradeList
                  .Select(t => new
                  {
                    stockcode = t.stockcode
                    ,
                    threeinv = t.threeinv
                                    ,
                    stockname = t.stockname
                  })
                  .ToList<dynamic>();

      dynamic dd = new { date = sMaxDate, s = ListS3d, b = ListB3d };

      return dd;
    }



    [HttpGet]
    [Route("getStockRealtime")]
    public dynamic getStockRealtime(string stockcode)
    {
      
      string surl = "https://tw.stock.yahoo.com/q/q?s=" + stockcode;

      HtmlWeb webClient = new HtmlWeb();
      //網頁特殊編碼
      webClient.OverrideEncoding = System.Text.Encoding.GetEncoding(950);
      //webClient.OverrideEncoding = Encoding;

      // 載入網頁資料 
      HtmlAgilityPack.HtmlDocument doc = webClient.Load(surl);
      //*[@id="yui_3_5_1_13_1503571196918_6"]/table[2]/tbody/tr/td/table
      //*[@id="yui_3_5_1_13_1503571196918_6"]/table[2]/tbody/tr/td/table/tbody/tr[2]
      // 裝載查詢結果 
      //HtmlNode nnn = doc.DocumentNode.SelectSingleNode("//table[2]/tr/td/table/tr[2]");
      HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table[2]/tr/td/table/tr[2]/td");

      //HtmlNodeCollection tdnodes = nnn.SelectNodes("td");

      //HtmlNodeCollection nodes1 = doc.DocumentNode.SelectNodes("//table[2]/tbody/tr/td/table/tbody/tr[2]");

      StockRuntime sr = new StockRuntime();
      sr.z = "";
      sr.y = "";
      sr.u = "";
      sr.w = "";      
      sr.n = "";
      sr.c = "";
      sr.xx = "";

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
          if (node.InnerText.Length >0)
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



      return sr;
    }
  }
}
