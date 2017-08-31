using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using M10Api.Class;
using System.Web;
using M10.lib;

namespace M10Api.Controllers
{
  [RoutePrefix("MapApi")]
  public class MapApiController : BaseApiController
  {
    [HttpGet]
    [Route("getAlertLrti")]
    public List<dynamic> getStationData()
    {
      var list = dbDapper.Query(@" 
          select a.*,b.lat,b.lon from LRTIAlert a 
          left join runtimeraindata b on a.stid = b.stid 
          where a.status != 'D'        
        "
        );

      //格式化資料
      foreach (var item in list)
      {
        //處理狀態改中文顯示
        if (item.status == "I") item.status = "新增";
        if (item.status == "C") item.status = "持續";
        if (item.status == "D") item.status = "刪除";


        //處理ELRTI無條件捨去
        decimal dELRTI = 0;
        if (decimal.TryParse(Convert.ToString(item.ELRTI), out dELRTI))
        {
          item.ELRTI = Math.Floor(dELRTI).ToString();
        }

      }


      return list;
    }

    [HttpGet]
    [Route("getCoordinate")]
    public List<dynamic> getCoordinate()
    {
      if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return new List<dynamic>();

      var Params = lib.M10apiLib.ParseQueryString(ActionContext.Request.RequestUri.Query);

      string sType = Params["type"];

      string ssql = @" 
        select a.villageid as relano ,c.lat,c.lng from LRTIAlert a
        inner join areacode b on a.villageid = b.villageid
        inner join mapvillage c on a.villageid = c.villageid and c.type = 'stvillage'
        where 1=1 {0}
        order by a.villageid
        ";
      if (sType == "R")
      {
        ssql = string.Format(ssql, " and a.status = 'C' ");
      }

      if (sType == "Y")
      {
        ssql = string.Format(ssql, " and a.status in ('I','O') ");
      }

      var list = dbDapper.Query(ssql);


      return list;
    }

    /// <summary>
    /// 1060804 修改架構備份(已不使用)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("getCoordinateBackup")]
    public List<dynamic> getCoordinateBackup()
    {
      if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return new List<dynamic>();

      var Params = lib.M10apiLib.ParseQueryString(ActionContext.Request.RequestUri.Query);

      string sType = Params["type"];

      string ssql = "";
      //ssql = @" 
      //  select c.relano ,c.lat,c.lng from LRTIAlert a
      //  inner join StationVillageLRTI b on a.STID =b.STID and a.country =b.Country and a.village =b.village 
      //  inner join Coordinate c on b.no = c.relano and c.type = 'stvillage'
      //  where 1=1 {0}
      //  order by b.stid,b.country,b.town,b.village,c.pointseq   
      //  ";
      if (sType == "R")
      {
        ssql = string.Format(ssql, " and a.status = 'C' ");
      }

      if (sType == "Y")
      {
        ssql = string.Format(ssql, " and a.status in ('I','O') ");
      }

      var list = dbDapper.Query(ssql);


      return list;
    }

    [HttpGet]
    [Route("getCoordinateCountry")]
    public List<dynamic> getCoordinateCountry()
    {
      string ssql = @" 
        select relano,lat,lng from Coordinate where type = 'country' order by relano, pointseq
        ";

      var list = dbDapper.Query(ssql);
      var gList = list.ToLookup(p => p.relano, p => new point { lat = p.lat, lng = p.lng });


      List<polygon> resultList = new List<polygon>();
      foreach (var item in gList)
      {
        var poly = new polygon();
        poly.relano = item.Key;
        poly.points = item.AsQueryable().Select(p => p).ToList<point>();

        resultList.Add(poly);
      }

      //1060621 linq lamda 效能不佳，取消
      //List<polygon> myList = new List<polygon>();

      //var s = list.Select(p => p.relano).Distinct();
      //var relano = s.ToList<dynamic>();

      //foreach (var item in relano)
      //{
      //  polygon oPolygon = new polygon();
      //  oPolygon.relano = item;

      //  var pp = list.Where(a => a.relano == item).Select(a => new point { lat = a.lat, lng = a.lng });

      //  oPolygon.points = pp.ToList<point>();
      //  myList.Add(oPolygon);
      //}

      //return myList11.ToList<dynamic>();


      return resultList.ToList<dynamic>();
    }


    [HttpGet]
    [Route("getMapDatas")]
    public List<dynamic> getMapDatas()
    {

      if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return new List<dynamic>();

      var Params = lib.M10apiLib.ParseQueryString(ActionContext.Request.RequestUri.Query);

      string sType = Params["type"];

      string ssql = @" 
          select *,b.villageid as relano from LRTIAlert a
          inner join areacode b on a.villageid = b.villageid
          where 1 = 1 
          ";
      if (sType == "R")
      {
        ssql += " and a.status = 'C' ";
      }

      if (sType == "Y")
      {
        ssql += " and a.status in ('I','O') ";
      }

      var list = dbDapper.Query(ssql);


      //格式化資料
      foreach (var item in list)
      {
        //處理狀態改中文顯示
        if (item.status == "I") item.status = M10Const.AlertStatus.I;
        if (item.status == "C") item.status = M10Const.AlertStatus.C;
        if (item.status == "O") item.status = M10Const.AlertStatus.O;
        if (item.status == "D") item.status = M10Const.AlertStatus.D;


        //處理ELRTI無條件捨去
        decimal dELRTI = 0;
        if (decimal.TryParse(Convert.ToString(item.ELRTI), out dELRTI))
        {
          item.ELRTI = Math.Round(dELRTI, 2).ToString();
        }

        decimal dRT = 0;
        if (decimal.TryParse(Convert.ToString(item.RT), out dRT))
        {
          item.RT = Math.Round(dRT, 2).ToString();
        }

      }

      return list;
    }


    /// <summary>
    /// 1060804 修改架構備份(已不使用)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("getMapDatasBackup")]
    public List<dynamic> getMapDatasBackup()
    {

      if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return new List<dynamic>();

      var Params = lib.M10apiLib.ParseQueryString(ActionContext.Request.RequestUri.Query);

      string sType = Params["type"];

      string ssql = "";
      //ssql = @" 
      //    select *,b.no as relano from LRTIAlert a
      //    inner join StationVillageLRTI b on a.STID =b.STID and a.country =b.Country and a.village =b.village           
      //    where 1 = 1 
      //    ";
      if (sType == "R")
      {
        ssql += " and a.status = 'C' ";
      }

      if (sType == "Y")
      {
        ssql += " and a.status in ('I','O') ";
      }

      var list = dbDapper.Query(ssql);


      //格式化資料
      foreach (var item in list)
      {
        //處理狀態改中文顯示
        if (item.status == "I") item.status = M10Const.AlertStatus.I;
        if (item.status == "C") item.status = M10Const.AlertStatus.C;
        if (item.status == "O") item.status = M10Const.AlertStatus.O;
        if (item.status == "D") item.status = M10Const.AlertStatus.D;


        //處理ELRTI無條件捨去
        decimal dELRTI = 0;
        if (decimal.TryParse(Convert.ToString(item.ELRTI), out dELRTI))
        {
          item.ELRTI = Math.Round(dELRTI, 2).ToString();
        }

        decimal dRT = 0;
        if (decimal.TryParse(Convert.ToString(item.RT), out dRT))
        {
          item.RT = Math.Round(dRT, 2).ToString();
        }

      }

      return list;
    }
  }


  public class polygon {
    public int relano { get; set; }

    public List<point> points { get; set; }
  }

  public class point {
    public string lat { get; set; }
    
    public string lng { get; set; }

  }
}
