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
  }
}
