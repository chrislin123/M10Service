using C10Mvc.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using M10.lib;
using System.Data;
using System.Dynamic;
using M10.lib.model;
using Newtonsoft.Json;

namespace C10Mvc.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult TumdList()
        {

            //string ssql = @" select top 20 *
            //    from stocktumd 
            //    order by stockdate desc
            //    ";
            //for mysql 
            string ssql = @" select  *
                from stocktumd 
                order by stockdate desc
                ";

            List<dynamic> data = dbDapper.Query(ssql);

            foreach (var item in data)
            {
                //處理狀態改中文顯示
                //if (item.status == "I") item.status = M10Const.AlertStatus.I;
                //if (item.status == "C") item.status = M10Const.AlertStatus.C;
                //if (item.status == "O") item.status = M10Const.AlertStatus.O;
                //if (item.status == "D") item.status = M10Const.AlertStatus.D;

                ////108 年
                ////if (item.status == "A1") item.status = M10Const.AlertStatus.A1;
                ////if (item.status == "A2") item.status = M10Const.AlertStatus.A2;
                ////if (item.status == "A3") item.status = M10Const.AlertStatus.A3;
                ////if (item.status == "AD") item.status = M10Const.AlertStatus.AD;

                ////109 年
                //if (item.status == "A2") item.status = M10Const.AlertStatus.A1;
                //if (item.status == "A3") item.status = M10Const.AlertStatus.A3;
                //if (item.status == "AD") item.status = M10Const.AlertStatus.AD;

                ////處理ELRTI取至小數第二位
                //decimal dELRTI = 0;
                //if (decimal.TryParse(Convert.ToString(item.ELRTI), out dELRTI))
                //{
                //    item.ELRTI = Math.Round(dELRTI, 2).ToString();
                //}

                //decimal dRT = 0;
                //if (decimal.TryParse(Convert.ToString(item.RT), out dRT))
                //{
                //    item.RT = Math.Round(dRT, 2).ToString();
                //}

            }


            ViewBag.count = data.Count;
            ViewData["tumd"] = data;


            return View();
        }


        [HttpPost]
        public JsonResult TumdSave(StockTumd JsonInput)
        {

            //foreach (StockTumd item in JsonInput)
            //{
            //    //ssql = @" select * from  BasRainallStation where ID = '{0}' ";
            //    //ssql = string.Format(ssql, item.ID);
            //    //BasRainallStation BRS = dbDapper.QuerySingleOrDefault<BasRainallStation>(ssql);
            //    //BRS.Active_YN = item.Active_YN;

            //    //dbDapper.Update<BasRainallStation>(BRS);
            //}

            ssql = @" select * from  StockTumd where stockdate  = '{0}' ";
            ssql = string.Format(ssql, JsonInput.stockdate);

            StockTumd BRS = dbDapper.QuerySingleOrDefault<StockTumd>(ssql);
            if (BRS == null)
            {
                BRS = new StockTumd();
                BRS.stockdate = JsonInput.stockdate;
                BRS.TseTU = JsonInput.TseTU;
                BRS.TseTM = JsonInput.TseTM;
                BRS.TseTD = JsonInput.TseTD;
                BRS.TseTW = JsonInput.TseTW;
                BRS.OtcTU = JsonInput.OtcTU;
                BRS.OtcTM = JsonInput.OtcTM;
                BRS.OtcTD = JsonInput.OtcTD;
                BRS.OtcTW = JsonInput.OtcTW;
                BRS.TxfTU = JsonInput.TxfTU;
                BRS.TxfTM = JsonInput.TxfTM;
                BRS.TxfTD = JsonInput.TxfTD;
                BRS.TxfTW = JsonInput.TxfTW;

                dbDapper.Insert<StockTumd>(BRS);
            }
            else
            {
                //BRS.stockdate = JsonInput.stockdate;
                BRS.TseTU = JsonInput.TseTU;
                BRS.TseTM = JsonInput.TseTM;
                BRS.TseTD = JsonInput.TseTD;
                BRS.TseTW = JsonInput.TseTW;
                BRS.OtcTU = JsonInput.OtcTU;
                BRS.OtcTM = JsonInput.OtcTM;
                BRS.OtcTD = JsonInput.OtcTD;
                BRS.OtcTW = JsonInput.OtcTW;
                BRS.TxfTU = JsonInput.TxfTU;
                BRS.TxfTM = JsonInput.TxfTM;
                BRS.TxfTD = JsonInput.TxfTD;
                BRS.TxfTW = JsonInput.TxfTW;

                dbDapper.Update<StockTumd>(BRS);
            }

            var FailResult = new { Success = "False", Message = "Error" };
            var SuccessResult = new { Success = "True", Message = "儲存完成" };

            return Json(SuccessResult, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult getTumdQuery(string Params)
        {
            MVCControlClass.ApiResult aResult = new MVCControlClass.ApiResult();
            dynamic dParams = JsonConvert.DeserializeObject<dynamic>(Params);

            string station = dParams["station"];

            //string ssql = @" select top 20 *
            //    from stocktumd 
            //    order by stockdate desc
            //    ";
            //for mysql 
            string ssql = @" select *
                from stocktumd 
                order by stockdate desc
                ";
            List<StockTumd> data = dbDapper.Query<StockTumd>(ssql);
            
       //     // 20200429 只顯示小時數據
       //     ssql = @"
       //         select a.*,b.datavalue from Result10MinData a 
			    //left join BasM11Setting b on a.siteid = b.dataitem and b.datatype = 'SiteCName'
       //         where StationID = '{0}' 
       //         and SensorID in ( '{1}') 
       //         and DatetimeString between '{2}' and '{3}'                 
       //     ";
       //     if (stimerange == "hr")
       //     {
       //         ssql = ssql + " and DatetimeString like '%:00:00' ";
       //     }
       //     ssql = string.Format(ssql, station, sInSensor, M11Utils.M11DatetimeToString(dtstart), M11Utils.M11DatetimeToString(dtend));
       //     ssql = ssql + " order by StationID,SensorID,DatetimeString desc ";
       //     List<Result10MinData> Stations = dbDapper.Query<Result10MinData>(ssql);

            aResult.ApiResultStauts = "Y";
            aResult.Data = data;

            return this.Json(aResult, JsonRequestBehavior.AllowGet);
        }


    }
}
