﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using M10.lib;

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

        public string HOUR6 { get; set; }

        public string nowwarm { get; set; }

        public string statustime { get; set; }

        public string villageid { get; set; }

        public string ReleaseRTime { get; set; }

        public string Remark { get; set; }
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

        public string HOUR6 { get; set; }

        public string nowwarm { get; set; }

        public string statustime { get; set; }

        public string statuscheck { get; set; }

        public string villageid { get; set; }

        public string ReleaseRTime { get; set; }

        public string Remark { get; set; }
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

        public string coefficient { get; set; }

        public string version { get; set; }

        public string startdate { get; set; }

        public string enddate { get; set; }

        public double? rti10 { get; set; }

        public double? rti30 { get; set; }

        public double? rti50 { get; set; }

        public double? rti70 { get; set; }

        public double? rti90 { get; set; }

        public double? rt10 { get; set; }

        public double? rt30 { get; set; }

        public double? rt50 { get; set; }

        public double? rt70 { get; set; }

        public double? rt90 { get; set; }

        public double? r310 { get; set; }

        public double? r330 { get; set; }

        public double? r350 { get; set; }

        public double? r370 { get; set; }

        public double? r390 { get; set; }

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

        public string coefficient { get; set; }

        public string version { get; set; }

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
        //[Key]
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

    [Table("MapVillage")]
    public class MapVillage
    {
        //設定key
        [Key]
        public int No { get; set; }

        public string Type { get; set; }

        public string VillageID { get; set; }

        public int Pointseq { get; set; }

        public string Lat { get; set; }

        public string Lng { get; set; }

        public string STID { get; set; }

    }

    [Table("AreaCode")]
    public class AreaCode
    {
        //設定key
        [Key]
        public int No { get; set; }

        public string CountryID { get; set; }

        public string CountryName { get; set; }

        public string TownID { get; set; }

        public string TownName { get; set; }

        public string VillageID { get; set; }

        public string VillageName { get; set; }

    }

    [Table("LrtiBasic")]
    public class LrtiBasic
    {
        //設定key
        [Key]
        public int no { get; set; }

        public string type { get; set; }

        public string stid { get; set; }

        public string elrti { get; set; }

        public string villageid { get; set; }

    }
    [Table("DataStaticLog")]
    public class DataStaticLog
    {
        //設定key
        [Key]
        public long no { get; set; }

        public string type { get; set; }

        public string key1 { get; set; }

        public string key2 { get; set; }

        public string key3 { get; set; }

        public string status { get; set; }

        public DateTime logtime { get; set; }

    }


    [Table("WeaRainData")]
    public class WeaRainData
    {
        //設定key
        [Key]
        public long no { get; set; }

        public string STID { get; set; }

        public string time { get; set; }

        public decimal PP01 { get; set; }

        public decimal PP01old { get; set; }


    }

    [Table("WeaRainDataHis")]
    public class WeaRainDataHis
    {
        //設定key
        [Key]
        public long no { get; set; }

        public string Stid { get; set; }

        public string Time { get; set; }

        public decimal Hour { get; set; }

        public decimal Hour3 { get; set; }

        public decimal Hour6 { get; set; }

        public decimal Hour12 { get; set; }

        public decimal Hour24 { get; set; }

        public decimal DayRainfall { get; set; }

        public decimal RT { get; set; }

        public string TransYN { get; set; }
    }

    [Table("SoilWaterRainDataHis")]
    public class SoilWaterRainDataHis
    {
        //設定key
        [Key]
        public long no { get; set; }

        public string Stid { get; set; }

        public string Time { get; set; }

        public decimal Min10 { get; set; }

        public decimal Hour { get; set; }

        public decimal Hour6 { get; set; }

        public decimal Hour24 { get; set; }

        public decimal DayRainfall { get; set; }

        public decimal Hour3 { get; set; }

        public decimal Hour12 { get; set; }

        public decimal RT { get; set; }


    }

    [Table("WeaRainStatistics")]
    public class WeaRainStatistics
    {
        //設定key
        [Key]
        public long no { get; set; }

        public string stid { get; set; }

        public string year { get; set; }

        public decimal m01 { get; set; }

        public decimal m02 { get; set; }

        public decimal m03 { get; set; }

        public decimal m04 { get; set; }

        public decimal m05 { get; set; }

        public decimal m06 { get; set; }

        public decimal m07 { get; set; }

        public decimal m08 { get; set; }

        public decimal m09 { get; set; }

        public decimal m10 { get; set; }

        public decimal m11 { get; set; }

        public decimal m12 { get; set; }

        public decimal mavg { get; set; }

        public decimal yearsum { get; set; }

        public decimal? max1 { get; set; }

        public string max1date { get; set; }

        public decimal? max2 { get; set; }

        public string max2date { get; set; }

        public decimal? max3 { get; set; }

        public string max3date { get; set; }

        public int? raindatecount { get; set; }

    }


    [Table("WeaRainArea")]
    public class WeaRainArea
    {
        //設定key
        [Key]
        public long no { get; set; }

        public string stid { get; set; }

        public string TimeStart { get; set; }

        public string TimeEnd { get; set; }

        public int RainHour { get; set; }

        public decimal TotalRain { get; set; }

        public decimal MaxRain { get; set; }

        public string MaxRainTime { get; set; }

        public decimal Max3Sum { get; set; }

        public decimal Max6Sum { get; set; }

        public decimal Max12Sum { get; set; }

        public decimal Max24Sum { get; set; }

        public decimal Max48Sum { get; set; }

        public decimal Pre7DayRain6 { get; set; }

        public decimal Pre7DayRain7 { get; set; }

        public decimal Pre7DayRain8 { get; set; }

        public decimal CumRain { get; set; }

        public decimal RT6 { get; set; }

        public decimal RT7 { get; set; }

        public decimal RT8 { get; set; }

        public int RainAreaSeq { get; set; }

        public string RainHourList { get; set; }

        public string UnitType { get; set; }
        
    }

    [Table("WraRainDataNew")]
    public class WraRainDataNew
    {
        public string STID { get; set; }

        public string rTime { get; set; }

        public string Min10 { get; set; }

        public string OneHour { get; set; }

        public string SixHour { get; set; }

        public string Hour24 { get; set; }

        public string DayRainfall { get; set; }

        public string Effective { get; set; }

        public string RT { get; set; }

        public string ETR3 { get; set; }

    }

    [Table("WraRainDataHis")]
    public class WraRainDataHis
    {
        //設定key
        [Key]
        public long no { get; set; }

        public string Stid { get; set; }

        public string Time { get; set; }

        public decimal Min10 { get; set; }

        public decimal Hour { get; set; }

        public decimal Hour3 { get; set; }

        public decimal Hour6 { get; set; }

        public decimal Hour12 { get; set; }

        public decimal Hour24 { get; set; }

        public decimal DayRainfall { get; set; }

        public decimal RT { get; set; }

    }


    [Table("BasStationData")]
    public class BasStationData
    {
        //設定key
        [Key]
        public int no { get; set; }

        public string stid { get; set; }

        public string stname { get; set; }

        public string county { get; set; }

        public string lat { get; set; }

        public string lng { get; set; }

        public string enable { get; set; }

        public string unittype { get; set; }

    }




    [Table("StockInfo")]
    public class StockInfo
    {
        //設定key
        [Key]
        public int no { get; set; }

        public string stockcode { get; set; }

        public string stockname { get; set; }

        public string type { get; set; }

        public string updatetime { get; set; }

        public string status { get; set; }

        public string updstatus { get; set; }

        public StockInfo()
        {
            this.updatetime = Utils.getDatatimeString();
            this.updstatus = "Y";
            this.status = "Y";
        }

    }

    [Table("Stockthreetrade")]
    public class Stockthreetrade
    {
        //設定key
        [Key]
        public int no { get; set; }

        public string date { get; set; }

        public string stockcode { get; set; }

        public string type { get; set; }

        public int? foreigninv { get; set; }

        public int? trustinv { get; set; }

        public int? selfempinv { get; set; }

        public int? threeinv { get; set; }

        public string updatetime { get; set; }

    }


    public class StockRuntime
    {


        /// <summary>
        /// 目前成交價
        /// </summary>
        public string z { get; set; }

        /// <summary>
        /// 昨日成交價
        /// </summary>
        public string y { get; set; }

        /// <summary>
        /// 漲停價
        /// </summary>
        public string u { get; set; }

        /// <summary>
        /// 跌停價
        /// </summary>
        public string w { get; set; }

        /// <summary>
        /// yahoo漲跌
        /// </summary>
        public string xx { get; set; }

        /// <summary>
        /// 代號名稱
        /// </summary>
        public string n { get; set; }

        /// <summary>
        /// 代碼
        /// </summary>
        public string c { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string TradeDay { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        public string a { get; set; }

        /// <summary>
        /// 開盤價
        /// </summary>
        public string open { get; set; }

        /// <summary>
        /// 昨高
        /// </summary>
        public string YTop { get; set; }

        /// <summary>
        /// 昨低
        /// </summary>
        public string YLow { get; set; }

        /// <summary>
        /// 資料狀態
        /// </summary>
        public M10Const.StockRuntimeStatus status { get; set; }
    }

    public class StockYahooAPI
    {

        private List<StockYahooAPITick> _tick = null;

        /// <summary>
        /// 代號
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 交易日
        /// </summary>
        public string TradeDay { get; set; }

        /// <summary>
        /// 開盤價
        /// </summary>
        public string open { get; set; }

        /// <summary>
        /// 開盤價
        /// </summary>
        public string close { get; set; }

        /// <summary>
        /// 最高價
        /// </summary>
        public string high { get; set; }

        /// <summary>
        /// 最低價
        /// </summary>
        public string low { get; set; }


        /// <summary>
        /// 成交量
        /// </summary>
        public string vol { get; set; }

        /// <summary>
        /// 當日Tick
        /// </summary>
        public List<StockYahooAPITick> Tick
        {
            get
            {
                if (_tick == null)
                {
                    _tick = new List<StockYahooAPITick>();
                }

                return _tick;
            }
            set { _tick = value; }

        }



        ///// <summary>
        ///// 目前成交價
        ///// </summary>
        //public string z { get; set; }

        ///// <summary>
        ///// 昨日成交價
        ///// </summary>
        //public string y { get; set; }

        ///// <summary>
        ///// 漲停價
        ///// </summary>
        //public string u { get; set; }

        ///// <summary>
        ///// 跌停價
        ///// </summary>
        //public string w { get; set; }

        ///// <summary>
        ///// yahoo漲跌
        ///// </summary>
        //public string xx { get; set; }







        ///// <summary>
        ///// 昨高
        ///// </summary>
        //public string YTop { get; set; }

        ///// <summary>
        ///// 昨低
        ///// </summary>
        //public string YLow { get; set; }

        ///// <summary>
        ///// 資料狀態
        ///// </summary>
        //public M10Const.StockRuntimeStatus status { get; set; }
    }

    public class StockYahooAPITick
    {
        /// <summary>
        /// 目前成交價
        /// </summary>
        public string tick { get; set; }

        /// <summary>
        /// 目前成交價
        /// </summary>
        public string price { get; set; }

        /// <summary>
        /// 目前成交價
        /// </summary>
        public string vol { get; set; }




    }


    [Table("Stockafter")]
    public class Stockafter
    {
        //設定key
        [Key]
        //[ExplicitKey]
        public int no { get; set; }

        public string stockcode { get; set; }

        public string stockdate { get; set; }

        public string stocktype { get; set; }

        public decimal? pricelast { get; set; }

        public string updown { get; set; }

        public string pricediff { get; set; }

        public decimal? priceopen { get; set; }

        public decimal? pricetop { get; set; }

        public decimal? pricelow { get; set; }

        public decimal? priceavg { get; set; }

        public long? dealnum { get; set; }

        public long? dealmoney { get; set; }

        public long? dealamount { get; set; }

        public decimal? pricelastbuy { get; set; }

        public decimal? pricelastsell { get; set; }

        public long? publicnum { get; set; }

        public decimal? pricenextday { get; set; }

        public decimal? pricenextlimittop { get; set; }

        public decimal? pricenextlimitlow { get; set; }

        public string updatetime { get; set; }

        public decimal? priceyesterday { get; set; }

        public string updYN { get; set; }

    }

    [Table("StockGet")]
    public class StockGet
    {
        //設定key
        [Key]
        public int no { get; set; }

        public string getdate { get; set; }

        public string stockcode { get; set; }

        public string stockdate { get; set; }

    }

    [Table("StockLog")]
    public class StockLog
    {
        //設定key
        [Key]
        public int no { get; set; }

        public string logtype { get; set; }

        public string logdate { get; set; }

        public string logstatus { get; set; }

        public string memo { get; set; }

        public string logdatetime { get; set; }

    }

    [Table("StockSet")]
    public class StockSet
    {
        //設定key
        [Key]
        public int no { get; set; }

        public string settype { get; set; }

        public int setseq { get; set; }

        public string setvalue { get; set; }

    }

    [Table("StockBrokerBS")]
    public class StockBrokerBS
    {
        //設定key
        [Key]
        public int no { get; set; }

        public string stockcode { get; set; }

        public string stockdate { get; set; }

        public string stocktype { get; set; }

        public string bsno { get; set; }

        public string vouchercode { get; set; }

        public decimal? bsprice { get; set; }

        public long? bvol { get; set; }

        public long? svol { get; set; }

        public string updatetime { get; set; }

    }

    [Table("StockVoucher")]
    public class StockVoucher
    {
        //設定key
        [Key]
        public int no { get; set; }

        public string vouchercode { get; set; }

        public string vouchername { get; set; }

        public string datatype { get; set; }

        public string updatetime { get; set; }

    }

    [Table("StockAfterRush")]
    public class StockAfterRush
    {
        //設定key
        [Key]
        public int no { get; set; }

        public string stockdate { get; set; }

        public string stockcode { get; set; }

        public string type { get; set; }

        public string stoprush { get; set; }

        public long? rushdealnum { get; set; }

        public long? rushmoneybuy { get; set; }

        public long? rushmoneysell { get; set; }

        public string createdatetime { get; set; }

        public string updatetime { get; set; }


    }

    [Table("StockTransRec")]
    public class StockTransRec
    {
        //設定key
        [Key]
        public int no { get; set; }

        public string stockdate { get; set; }

        public string stockcode { get; set; }

        public string type { get; set; }

        public string status { get; set; }

        public string finish { get; set; }

        public string finishtime { get; set; }

        public string updatetime { get; set; }

    }

    [Table("StockTumd")]
    public class StockTumd
    {
        private string _tseTU;
        private string _tseTM;
        private string _tseTD;
        private string _tseTW;
        private string _otcTU;
        private string _otcTM;
        private string _otcTD;
        private string _otcTW;
        private string _txfTU;
        private string _txfTM;
        private string _txfTD;
        private string _txfTW;

        //設定key
        [Key]
        public int no { get; set; }

        public string stockdate { get; set; }

        public string TseTU
        {
            get
            {
                if (_tseTU == null)
                {
                    _tseTU = "0";
                }

                return _tseTU;
            }
            set { _tseTU = value; }
        }

        public string TseTM
        {
            get
            {
                if (_tseTM == null)
                {
                    _tseTM = "0";
                }

                return _tseTM;
            }
            set { _tseTM = value; }
        }

        public string TseTD
        {
            get
            {
                if (_tseTD == null)
                {
                    _tseTD = "0";
                }

                return _tseTD;
            }
            set { _tseTD = value; }
        }

        public string TseTW
        {
            get
            {
                if (_tseTW == null)
                {
                    _tseTW = "0";
                }

                return _tseTW;
            }
            set { _tseTW = value; }
        }

        public string OtcTU
        {
            get
            {
                if (_otcTU == null)
                {
                    _otcTU = "0";
                }

                return _otcTU;
            }
            set { _otcTU = value; }
        }

        public string OtcTM
        {
            get
            {
                if (_otcTM == null)
                {
                    _otcTM = "0";
                }

                return _otcTM;
            }
            set { _otcTM = value; }
        }

        public string OtcTD
        {
            get
            {
                if (_otcTD == null)
                {
                    _otcTD = "0";
                }

                return _otcTD;
            }
            set { _otcTD = value; }
        }

        public string OtcTW
        {
            get
            {
                if (_otcTW == null)
                {
                    _otcTW = "0";
                }

                return _otcTW;
            }
            set { _otcTW = value; }
        }

        public string TxfTU
        {
            get
            {
                if (_txfTU == null)
                {
                    _txfTU = "0";
                }

                return _txfTU;
            }
            set { _txfTU = value; }
        }

        public string TxfTM
        {
            get
            {
                if (_txfTM == null)
                {
                    _txfTM = "0";
                }

                return _txfTM;
            }
            set { _txfTM = value; }
        }

        public string TxfTD
        {
            get
            {
                if (_txfTD == null)
                {
                    _txfTD = "0";
                }

                return _txfTD;
            }
            set { _txfTD = value; }
        }


        public string TxfTW
        {
            get
            {
                if (_txfTW == null)
                {
                    _txfTW = "0";
                }

                return _txfTW;
            }
            set { _txfTW = value; }
        }

    }

    [Table("StockTumdRec")]
    public class StockTumdRec
    {
        //設定key
        [Key]
        public int no { get; set; }

        public string stockcode { get; set; }

        public string stockdate { get; set; }

        public decimal? Tu { get; set; }

        public decimal? Tm { get; set; }

        public decimal? Td { get; set; }

        public decimal? DefWeek { get; set; }

        public decimal? DefMonth { get; set; }

        public decimal? DayVolTurnoverRate { get; set; }

        public decimal? RecentVolAvgHr { get; set; }

        public decimal? RecentTurnoverRate { get; set; }

        public decimal? Ema5 { get; set; }

        public decimal? Ema10 { get; set; }

        public decimal? Ema20 { get; set; }

        public decimal? Ema60 { get; set; }

        public decimal? Ema120 { get; set; }

        public decimal? Ema240 { get; set; }

        public string updatetime { get; set; }

        public string updYN { get; set; }

    }








    [Table("Price1")]
    public class Price1
    {
        //設定key,[Key]自動生成;[ExplicitKey]非自動生成
        [ExplicitKey]
        public string UID { get; set; }

        public DateTime? Date { get; set; }

        public string StockID { get; set; }

        public decimal? Open { get; set; }

        public decimal? Close { get; set; }

        public decimal? High { get; set; }

        public decimal? Low { get; set; }

        public decimal? Change { get; set; }

        public long? Transaction { get; set; }

        public long? Capacity { get; set; }

        public long? Turnover { get; set; }

        public DateTime? CreateDt { get; set; }

        public string CreateUser { get; set; }

    }


    [Table("Price1")]
    public class RtiData2
    {
        //設定key
        [Key]
        public long no { get; set; }

        public string ver { get; set; }

        public string station { get; set; }

        public double? raindelay { get; set; }

        public double? maxraintime { get; set; }

        public double? Rtd { get; set; }

        public double? Rti { get; set; }

        public double? Rti3 { get; set; }

        public double? Rtd6 { get; set; }

        public double? Rtd7 { get; set; }

        public double? Rtd8 { get; set; }

        public double? Rti6 { get; set; }

        public double? Rti7 { get; set; }

        public double? Rti8 { get; set; }

        public double? Rti36 { get; set; }

        public double? Rti37 { get; set; }

        public double? Rti38 { get; set; }

        public string date { get; set; }

    }








}
