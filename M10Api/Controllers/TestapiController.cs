using M10Api.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace M10Api.Controllers
{
  [RoutePrefix("rti")]
  public class TestapiController : BaseApiController
  {
    //public Dictionary<string, string> Get()
    //{
    //  var result = new Dictionary<string, string>()
    //        {
    //            {"001", "Banana3333"},
    //            {"002", "Apple222"},
    //            {"003", "Orange"}
    //        };
    //  return result;
    //}

    //[HttpGet]
    //public Dictionary<string, string> HandMadeJson()
    //{

    //  var result = new Dictionary<string, string>()
    //        {
    //            {"001", "Banana1111"},
    //            {"002", "Apple2222"},
    //            {"003", "Orange"}
    //        };
    //  return result;
    //}

    //[HttpGet]
    //public dynamic HandMadeJsonok()
    //{


    //  var list = db.Query(@" 

    //      select a.STID,a.LAT,a.LON,b.COUNTY from RunTimeRainData a
    //      left join StationData b on a.STID = b.STID
    //    "
    //    );

    //  return list;

    //}


    [HttpGet]
    [Route("getStationData")]
    public List<dynamic> getStationData()
    {
      var list = db.Query(@" select a.*,b.lat,b.lon from LRTIAlert a 
        left join runtimeraindata b on a.stid = b.stid 
        "
        );
      //where a.COUNTY = '臺中市'
      return list;
    }


    [HttpGet]
    [Route("getdata")]
    public List<dynamic> HandRtidata()
    {





      //var list = db.Query(" select top 10 * from RtiData ");


      //var list = db.Query(" select * from rtidata where station = @station  and ver = @ver " 
      //  , new { station = "C1X030", ver = "20160416" });

      //var list = db.Query(" select * from stationdata "
      //  );

      var list = db.Query(@" select * from stationdata "
        );


      //var filer = from data in list
      //            where data.var == "20160416"
      //            select data;


      //20160416

      return list;
      //return filer.ToList<dynamic>();





      //List<string> sList = new List<string>();
      //string connstr = "";
      //using (var cn = new SqlConnection(connstr))
      //{
      //  //1) 不需要定義POCO物件，直接SELECT結果轉成.NET物件集合!(酷)
      //  //   注意: 結果為IEnumerable<dynamic>，會喪失強型別優勢
      //  //2) 可宣告及傳入具名參數


      //  //var list = cn.Query<RtiData>()
      //  var list = cn.Query<RtiData>(@" 

      //    select top 10 * from RtiData

      //  ");

      //  //foreach (var item in list)
      //  //{
      //  //  sList.Add(item.STID);
      //  //}

      //  return list as List<RtiData>;
      //  //var list = cn.Query(
      //  //  "SELECT * FROM Products WHERE CategoryID=@catg", new { catg = 2 });
      //  //foreach (var item in list)
      //  //{
      //  //  Console.WriteLine("{0}.{1}({2})",
      //  //      item.ProductID, item.ProductName, item.QuantityPerUnit);
      //  //}
      //}


    }

    [HttpGet, HttpPost]
    [Route("getdatap")]
    //public List<dynamic> HandRtidata([FromBody]string STID, string COUNTY = "")
    //public List<dynamic> HandRtidata(HttpRequestMessage request)
    public List<dynamic> HandRtidata([FromBody]JObject json)
    {


      var request = JsonConvert.DeserializeObject(json.ToString());


      //var test = HttpContext.Current.Request.Form["STID"];

      

      string STID = json.Property("STID").Value.ToString();      
      string country = json.Property("country").Value.ToString();

      //var list = db.Query(" select top 10 * from RtiData ");


      //var list = db.Query(" select * from stationdata where STID = @STID  and COUNTY = @COUNTY "
      //  , new { STID = STID, COUNTY = COUNTY });

      var list = db.Query(" select * from stationdata  ");

      //var list = db.Query(" select * from stationdata where STID = @STID  "
      //  , new { STID = STID });

      //var list = db.Query(" select * from stationdata "
      //  );

      //var list = db.Query(@" select * from stationdata "
      //  );


      //var filer = from data in list
      //            where data.var == "20160416"
      //            select data;


      //20160416

      return list;
      //return filer.ToList<dynamic>();








    }

    [HttpGet, HttpPost]
    [Route("Demo")]
    public string Demo3([FromBody]int id)
    {
      return String.Format("ID: {0}, Company: {1}", id, id);
    }

  }


  public class RtiData
  {
    public int no;
    public string ver;
    public string station;
    public double? raindelay;
    public double? maxraintime;
    public double? Rtd;
    public double? Rti;
    public double? Rti3;
    public string date;
  }
}
