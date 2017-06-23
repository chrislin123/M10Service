using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace M10.lib.model
{
  class model
  {
  }

  [Table("FileTransLog")]
  public class FileTransLog
  {
    //設定key
    [Key]
    public int FileTransNo { get; set; }

    
    public string FileTransName { get; set; }

    public DateTime? FileTransTime { get; set; }

  }

  [Table("LRTIAlertHis")]
  public class LRTIAlertHis
  {
    //設定key
    [Key]
    //自動相加key的設定
    //[ExplicitKey]
    public int no { get; set; }

    public string STID { get; set; }

    public string country { get; set; }

    public string town { get; set; }

    public string village { get; set; }

    public string status { get; set; }

    public string RecTime { get; set; }

    public string HOUR3 { get; set; }

    public string RT { get; set; }

    public string LRTI { get; set; }

    public string ELRTI { get; set; }

    public string HOUR2 { get; set; }

    public string HOUR1 { get; set; }

    public string nowwarm { get; set; }

    public string statustime { get; set; }


  }

  [Table("LRTIAlertMail")]
  public class LRTIAlertMail
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string type { get; set; }

    public string value { get; set; }

  }

  [Table("LRTIAlert")]
  public class LRTIAlert
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string STID { get; set; }

    public string country { get; set; }

    public string town { get; set; }

    public string village { get; set; }

    public string status { get; set; }

    public string HOUR3 { get; set; }

    public string RT { get; set; }

    public string LRTI { get; set; }

    public string ELRTI { get; set; }

    public string HOUR2 { get; set; }

    public string HOUR1 { get; set; }

    public string nowwarm { get; set; }

    public string statustime { get; set; }

    public string statuscheck { get; set; }

  }

  [Table("RainStation")]
  public class RainStation
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string STID { get; set; }

    public string STNAME { get; set; }

    public string STTIME { get; set; }

    public string LAT { get; set; }

    public string LON { get; set; }

    public string ELEV { get; set; }

    public string RAIN { get; set; }

    public string MIN10 { get; set; }

    public string HOUR3 { get; set; }

    public string HOUR6 { get; set; }

    public string HOUR12 { get; set; }

    public string HOUR24 { get; set; }

    public string NOW { get; set; }

    public string COUNTY { get; set; }

    public string TOWN { get; set; }

    public string ATTRIBUTE { get; set; }

    public string diff { get; set; }

    public string STATUS { get; set; }

    public string TMX { get; set; }

    public string TMY { get; set; }

    public string RTime { get; set; }

    public string SWCBID { get; set; }

    public string DebrisRefStation { get; set; }

    public string EffectiveRainfall { get; set; }

    public string RT { get; set; }

    public string Cumulation { get; set; }

    public string Day1 { get; set; }

    public string Day2 { get; set; }

    public string Day3 { get; set; }

    public string Hour2 { get; set; }

    public string WGS84_lon { get; set; }

    public string WGS84_lat { get; set; }

    public string LRTI { get; set; }

    public string WLRTI { get; set; }

  }

  [Table("Rti3Detail")]
  public class Rti3Detail
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string station { get; set; }

    public string delaytime { get; set; }

    public int? totalcount { get; set; }

    public string startdate { get; set; }

    public string enddate { get; set; }

    public double? rti10 { get; set; }

    public double? rti30 { get; set; }

    public double? rti50 { get; set; }

    public double? rti70 { get; set; }

    public double? rti90 { get; set; }

  }

  [Table("RtiData")]
  public class RtiData
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string ver { get; set; }

    public string station { get; set; }

    public double? raindelay { get; set; }

    public double? maxraintime { get; set; }

    public double? Rtd { get; set; }

    public double? Rti { get; set; }

    public double? Rti3 { get; set; }

    public string date { get; set; }

  }

  [Table("RtiDataTrans")]
  public class RtiDataTrans
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string ver { get; set; }

    public string station { get; set; }

    public string raindelaytype { get; set; }

    public string totalcount { get; set; }

    public string startdate { get; set; }

    public string enddate { get; set; }

    public string rti10 { get; set; }

    public string rti30 { get; set; }

    public string rti50 { get; set; }

    public string rti70 { get; set; }

    public string rti90 { get; set; }

  }

  [Table("RtiDetail")]
  public class RtiDetail
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string station { get; set; }

    public string delaytime { get; set; }

    public int? totalcount { get; set; }

    public string startdate { get; set; }

    public string enddate { get; set; }

    public double? rti10 { get; set; }

    public double? rti30 { get; set; }

    public double? rti50 { get; set; }

    public double? rti70 { get; set; }

    public double? rti90 { get; set; }

  }
  [Table("RunTimeRainData")]
  public class RunTimeRainData
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string STID { get; set; }

    public string STNAME { get; set; }

    public string STTIME { get; set; }

    public string LAT { get; set; }

    public string LON { get; set; }

    public string ELEV { get; set; }

    public string RAIN { get; set; }

    public string MIN10 { get; set; }

    public string HOUR3 { get; set; }

    public string HOUR6 { get; set; }

    public string HOUR12 { get; set; }

    public string HOUR24 { get; set; }

    public string NOW { get; set; }

    public string COUNTY { get; set; }

    public string TOWN { get; set; }

    public string ATTRIBUTE { get; set; }

    public string diff { get; set; }

    public string STATUS { get; set; }

    public string TMX { get; set; }

    public string TMY { get; set; }

    public string RTime { get; set; }

    public string SWCBID { get; set; }

    public string DebrisRefStation { get; set; }

    public string EffectiveRainfall { get; set; }

    public string RT { get; set; }

    public string Cumulation { get; set; }

    public string Day1 { get; set; }

    public string Day2 { get; set; }

    public string Day3 { get; set; }

    public string HOUR2 { get; set; }

    public string WGS84_lon { get; set; }

    public string WGS84_lat { get; set; }

    public string LRTI { get; set; }

    public string WLRTI { get; set; }

  }
  [Table("StationData")]
  public class StationData
  {
    //設定key
    [Key]
    public string STID { get; set; }

    public string STNAME { get; set; }

    public string COUNTY { get; set; }

    public string TOWN { get; set; }

  }
  [Table("StationErrLRTI")]
  public class StationErrLRTI
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string STID { get; set; }

    public string ELRTI { get; set; }

  }
  [Table("StationVillageLRTI")]
  public class StationVillageLRTI
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string STID { get; set; }

    public string Country { get; set; }

    public string Town { get; set; }

    public string Village { get; set; }

  }
  [Table("Updatelog")]
  public class Updatelog
  {
    //設定key
    [Key]
    public int no { get; set; }

    public DateTime updatetime { get; set; }

  }

  [Table("Coordinate")]
  public class Coordinate
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string type { get; set; }

    public int? relano { get; set; }

    public int? pointseq { get; set; }

    public string lat { get; set; }

    public string lng { get; set; }

  }

  [Table("HighRail")]
  public class HighRail
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string type { get; set; }

    public string country { get; set; }

    public string week { get; set; }

    public string car { get; set; }

    public string time { get; set; }

  }

}
