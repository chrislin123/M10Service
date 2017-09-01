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
      //get from yahoo web
      StockRuntime sr = oStockUtil.getStockRealtimeYahooWeb(stockcode);
     
      //get from yahoo api
      //StockRuntime sr = oStockUtil.getStockRealtimeYahooApi(stockcode);


      return sr;
    }
  }
}
