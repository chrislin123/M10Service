using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using M10Api.Class;

namespace M10Api.Controllers
{
  [RoutePrefix("MapApi")]
  public class MapApiController : BaseApiController
  {
    [HttpGet]
    [Route("getAlertLrti")]
    public List<dynamic> getStationData()
    {
      var list = db.Query(@" 
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
      var list = db.Query(@" 
        select c.relano ,c.lat,c.lng from LRTIAlert a
        inner join StationVillageLRTI b on a.STID =b.STID and a.country =b.Country and a.village =b.village 
        inner join Coordinate c on b.no = c.relano and c.type = 'stvillage'
        where a.status != 'D' 
        order by b.stid,b.country,b.town,b.village,c.pointseq   
        "
        );

      //格式化資料
      foreach (var item in list)
      {
        //處理狀態改中文顯示
        //if (item.status == "I") item.status = "新增";
        //if (item.status == "C") item.status = "持續";
        //if (item.status == "D") item.status = "刪除";


        //處理ELRTI無條件捨去
        //decimal dELRTI = 0;
        //if (decimal.TryParse(Convert.ToString(item.ELRTI), out dELRTI))
        //{
        //  item.ELRTI = Math.Floor(dELRTI).ToString();
        //}

      }

      return list;
    }

    [HttpGet]
    [Route("getMapDatas")]
    public List<dynamic> getMapDatas()
    {
      var list = db.Query(@" 
          select *,b.no as relano from LRTIAlert a
          inner join StationVillageLRTI b on a.STID =b.STID and a.country =b.Country and a.village =b.village           
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



    //Here is an example of a KML file containing a Network Link that loads this Python script:
    //#!/usr/bin/python

    //    import random

    //latitude = random.randrange(-90, 90)
    //longitude = random.randrange(-180, 180)
    //kml = (
    //   '<?xml version="1.0" encoding="UTF-8"?>\n'
    //   '<kml xmlns="http://www.opengis.net/kml/2.2">\n'
    //   '<Placemark>\n'
    //   '<name>Random Placemark</name>\n'
    //   '<Point>\n'
    //   '<coordinates>%d,%d</coordinates>\n'
    //   '</Point>\n'
    //   '</Placemark>\n'
    //   '</kml>'
    //   ) %(longitude, latitude)
    //print 'Content-Type: application/vnd.google-earth.kml+xml\n'
    //print kml


    //    <?xml version = "1.0" encoding="UTF-8"?>
    //<kml xmlns = "http://www.opengis.net/kml/2.2" >
    //  < Folder >
    //    < name > Network Links</name>
    //    <visibility>0</visibility>
    //    <open>0</open>
    //    <description>Network link example 1</description>
    //    <NetworkLink>
    //      <name>Random Placemark</name>
    //      <visibility>0</visibility>
    //      <open>0</open>
    //      <description>A simple server-side script that generates a new random
    //        placemark on each call</description>
    //      <refreshVisibility>0</refreshVisibility>
    //      <flyToView>0</flyToView>
    //      <Link>
    //        <href>http://yourserver.com/cgi-bin/randomPlacemark.py</href>
    //      </Link>
    //    </NetworkLink>
    //  </Folder>
    //</kml>


  }
}
