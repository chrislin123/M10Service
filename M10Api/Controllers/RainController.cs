﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using M10Api.Class;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using M10.lib.model;
using System.Data;
using System.Dynamic;
using M10.lib;
using Microsoft.Ajax.Utilities;
using Elmah.ContentSyndication;


namespace M10Api.Controllers
{
    public class RainController : BaseController
    {
        // GET: Rain
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QueryRain(string cityId)
        {


            //string cityid1 = Request.QueryString["cityId"];
            ViewBag.SelectCountry = "全部";

            ssql = @"   select CONVERT(float, MIN10) as MIN10
                        ,CONVERT(float, RAIN) as RAIN
                        ,CONVERT(float, HOUR3) as HOUR3
                        ,CONVERT(float, HOUR6) as HOUR6
                        ,CONVERT(float, HOUR12) as HOUR12
                        ,CONVERT(float, HOUR24) as HOUR24
                        ,CONVERT(float, NOW) as NOW
                        ,ROUND(CONVERT(float, RT),2) as RT
                        ,CONVERT(float, LRTI) as LRTI
                        ,ROUND(CONVERT(float, b.ELRTI),2) as ELRTI
                        ,* from RunTimeRainData a
                        left join StationErrLRTI b on a.STID = b.STID 
                    ";

            if (string.IsNullOrEmpty(cityId) == false)
            {
                ViewBag.SelectCountry = cityId;

                if (cityId != "全部")
                {
                    ssql += "where COUNTY = @cityId ";
                }
            }

            List<dynamic> data = dbDapper.Query(ssql, new { cityId = cityId });
            foreach (var LoopItem in data)
            {
                LoopItem.LStatus = "";

                if (LoopItem.STATUS == "-99")
                {
                    LoopItem.MIN10 = "0";
                    LoopItem.RAIN = "0";
                    LoopItem.HOUR3 = "0";
                    LoopItem.HOUR6 = "0";
                    LoopItem.HOUR12 = "0";
                    LoopItem.HOUR24 = "0";
                    LoopItem.NOW = "0";
                    LoopItem.RT = "0";
                    LoopItem.LRTI = "0";

                    LoopItem.STATUS = "異常";
                    //設定顏色
                    LoopItem.LStatus = "LightGray";
                }

                double dLRTI = 0;
                double dELRTI = 0;
                double.TryParse(Convert.ToString(LoopItem.LRTI), out dLRTI);
                double.TryParse(Convert.ToString(LoopItem.ELRTI), out dELRTI);


                if (dLRTI > dELRTI && LoopItem.ELRTI != null)
                {
                    LoopItem.LStatus = "Red";
                }
            }

            //取得更新最新時間
            ViewBag.forecastdate = "";
            ssql = " select MAX(RTime) as RTime from RunTimeRainData ";
            object oforecastdate = dbDapper.ExecuteScale(ssql);
            if (oforecastdate != null)
            {
                ViewBag.forecastdate = dbDapper.ExecuteScale(ssql).ToString();
            }


            //資料筆數
            ViewBag.count = data.Count;
            ViewData["RunTimeRainData"] = data;





            return View();
        }

        public JsonResult GetCountyDDL(string cityId)
        {
            List<SelectListItem> items = new List<SelectListItem>();


            var Countrys = dbDapper.Query(" select distinct COUNTY from StationData order by COUNTY ");

            foreach (var item in Countrys)
            {
                items.Add(new SelectListItem()
                {
                    Text = item.COUNTY,
                    Value = item.COUNTY
                });
            }

            if (Countrys.Count != 0)
            {
                items.Insert(0, new SelectListItem() { Text = "全部", Value = "全部" });
            }


            return this.Json(items, JsonRequestBehavior.AllowGet);
        }


        public JsonResult QueryRainStatisticsGetStidDDL()
        {
            List<SelectListItem> items = new List<SelectListItem>();


            var Countrys = dbDapper.Query(" select stid,stname,enable from BasStationData order by enable desc,stid asc ");

            foreach (var item in Countrys)
            {
                items.Add(new SelectListItem()
                {
                    Text = string.Format("{2}{0}({1})", item.stid, item.stname, item.enable == "Y" ? "" : "[停]"),
                    Value = item.stid
                });
            }

            //if (Countrys.Count != 0)
            //{
            //    items.Insert(0, new SelectListItem() { Text = "全部", Value = "全部" });
            //}


            return this.Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStationDDL(string STID)
        {
            List<SelectListItem> items = new List<SelectListItem>();


            ssql = @" select * from StationData ";
            if (string.IsNullOrEmpty(STID) == false) ssql += " where STID like @STID ";
            ssql += " order by STID ";

            var Stations = dbDapper.Query(ssql, new { STID = "%" + STID + "%" });

            foreach (var item in Stations)
            {
                items.Add(new SelectListItem()
                {
                    Text = string.Format("{0}[{1}-{2}]", Convert.ToString(item.STID).ToUpper(), item.COUNTY, item.STNAME),
                    Value = item.STID
                });
            }

            return this.Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWeaRainStationDDL(string STID)
        {
            List<SelectListItem> items = new List<SelectListItem>();


            ssql = @" select a.STID,b.COUNTY,b.STNAME from WeaRainStation a
                    left join BasStationData b on a.stid = b.stid
                     ";
            if (string.IsNullOrEmpty(STID) == false) ssql += " where a.STID like @STID ";
            ssql += " order by a.STID ";

            var Stations = dbDapper.Query(ssql, new { STID = "%" + STID + "%" });

            foreach (var item in Stations)
            {
                items.Add(new SelectListItem()
                {
                    Text = string.Format("{0}[{1}-{2}]", Convert.ToString(item.STID).ToUpper(), item.COUNTY, item.STNAME),
                    Value = item.STID
                });
            }

            return this.Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWeaRainStationDDLByCountry(string Country)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            ssql = @" select a.STID,b.COUNTY,b.STNAME from WeaRainStation a
                    inner join BasStationData b on a.stid = b.stid
                     ";
            if (string.IsNullOrEmpty(Country) == false && Country != "全部") ssql += " where b.COUNTY = @COUNTY ";
            ssql += " order by a.STID ";

            var Stations = dbDapper.Query(ssql, new { COUNTY = Country });

            foreach (var item in Stations)
            {
                items.Add(new SelectListItem()
                {
                    Text = string.Format("{0}[{1}-{2}]", Convert.ToString(item.STID).ToUpper(), item.COUNTY, item.STNAME),
                    Value = item.STID
                });
            }

            return this.Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSoilWaterRainStationDDL(string STID)
        {
            List<SelectListItem> items = new List<SelectListItem>();


            ssql = @" select a.STID,b.COUNTY,b.STNAME from SoilWaterRainStation a
                    left join BasStationData b on a.stid = b.stid
                     ";
            if (string.IsNullOrEmpty(STID) == false) ssql += " where a.STID like @STID ";
            ssql += " order by a.STID ";

            var Stations = dbDapper.Query(ssql, new { STID = "%" + STID + "%" });

            foreach (var item in Stations)
            {
                items.Add(new SelectListItem()
                {
                    Text = string.Format("{0}[{1}-{2}]", Convert.ToString(item.STID).ToUpper(), item.COUNTY, item.STNAME),
                    Value = item.STID
                });
            }

            return this.Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWraRainStationDDL(string STID)
        {
            List<SelectListItem> items = new List<SelectListItem>();


            ssql = @" select a.STID,b.COUNTY,b.STNAME from WraRainStation a
                    left join BasStationData b on a.stid = b.stid
                     ";
            if (string.IsNullOrEmpty(STID) == false) ssql += " where a.STID like @STID ";
            ssql += " order by a.STID ";

            var Stations = dbDapper.Query(ssql, new { STID = "%" + STID + "%" });

            foreach (var item in Stations)
            {
                items.Add(new SelectListItem()
                {
                    Text = string.Format("{0}[{1}-{2}]", Convert.ToString(item.STID).ToUpper(), item.COUNTY, item.STNAME),
                    Value = item.STID
                });
            }

            return this.Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSoilWaterRainStationDDLByCountry(string Country)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            ssql = @" select a.STID,b.COUNTY,b.STNAME from SoilWaterRainStation a
                    inner join BasStationData b on a.stid = b.stid
                     ";
            if (string.IsNullOrEmpty(Country) == false && Country != "全部") ssql += " where b.COUNTY = @COUNTY ";
            ssql += " order by a.STID ";

            var Stations = dbDapper.Query(ssql, new { COUNTY = Country });

            foreach (var item in Stations)
            {
                items.Add(new SelectListItem()
                {
                    Text = string.Format("{0}[{1}-{2}]", Convert.ToString(item.STID).ToUpper(), item.COUNTY, item.STNAME),
                    Value = item.STID
                });
            }

            return this.Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWraRainStationDDLByCountry(string Country)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            ssql = @" select a.STID,b.COUNTY,b.STNAME from WraRainStation a
                    inner join BasStationData b on a.stid = b.stid
                     ";
            if (string.IsNullOrEmpty(Country) == false && Country != "全部") ssql += " where b.COUNTY = @COUNTY ";
            ssql += " order by a.STID ";

            var Stations = dbDapper.Query(ssql, new { COUNTY = Country });

            foreach (var item in Stations)
            {
                items.Add(new SelectListItem()
                {
                    Text = string.Format("{0}[{1}-{2}]", Convert.ToString(item.STID).ToUpper(), item.COUNTY, item.STNAME),
                    Value = item.STID
                });
            }

            return this.Json(items, JsonRequestBehavior.AllowGet);
        }


        public ActionResult postAlertSet()
        {
            dynamic result = new ExpandoObject();


            ssql = " select * from LRTIAlertMail where type = 'isal' ";
            LRTIAlertMail inst = dbDapper.QuerySingleOrDefault<LRTIAlertMail>(ssql);

            if (inst.value == "Y")
            {
                inst.value = "N";
            }
            else
            {
                inst.value = "Y";
            }

            //更新成功才回傳
            if (dbDapper.Update(inst))
            {
                result.result = "OK";
                result.AlertSet = inst.value;
            }



            return Content(JsonConvert.SerializeObject(result), "application/json");
            //return Json(JsonConvert.SerializeObject(result), "application/json", JsonRequestBehavior.AllowGet);

            //return this.Json(result, JsonRequestBehavior.AllowGet);
            //return JsonConvert.SerializeObject(result);
        }

        public ActionResult getAlertSet()
        {
            dynamic result = new ExpandoObject();


            ssql = " select * from LRTIAlertMail where type = 'isal' ";
            LRTIAlertMail inst = dbDapper.QuerySingleOrDefault<LRTIAlertMail>(ssql);

            result.AlertSet = inst.value;

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        public ActionResult checkPass(string pass)
        {
            dynamic result = new ExpandoObject();
            result.pass = "N";

            ssql = " select * from LRTIAlertMail where type = 'usps' and value = @value ";
            int iCount = dbDapper.QueryTotalCount(ssql, new { value = pass.ToUpper() });

            if (iCount > 0)
            {
                result.pass = "Y";
            }

            return Content(JsonConvert.SerializeObject(result), "application/json");
        }


        public ActionResult ExportRain()
        {
            return View();
        }

        public ActionResult QueryRTI(string delaytime, string coefficient)
        {
            ViewBag.delaytime = "0";
            ViewBag.coefficient = "7";
            if (string.IsNullOrEmpty(delaytime) == false) ViewBag.delaytime = delaytime;
            if (string.IsNullOrEmpty(coefficient) == false) ViewBag.coefficient = coefficient;

            ssql = @" select RtiDetail.*,BasStationData.stname from RtiDetail 
                        left join BasStationData on RtiDetail.station = BasStationData.stid
                        where 1=1 
                        and delaytime = @delaytime 
                        and coefficient = @coefficient 
                        and version = 'new' 
                        order by station ";

            List<dynamic> data = dbDapper.Query(ssql, new { delaytime = ViewBag.delaytime, coefficient = ViewBag.coefficient });

            //資料筆數
            ViewBag.count = data.Count;
            ViewData["RtiDetail"] = data;

            //
            return View();
        }

        public ActionResult QueryRTI3(string delaytime, string coefficient)
        {
            ViewBag.delaytime = "0";
            ViewBag.coefficient = "7";
            if (string.IsNullOrEmpty(delaytime) == false) ViewBag.delaytime = delaytime;
            if (string.IsNullOrEmpty(coefficient) == false) ViewBag.coefficient = coefficient;

            ssql = @" select Rti3Detail.*,BasStationData.stname from Rti3Detail 
                        left join BasStationData on Rti3Detail.station = BasStationData.stid
                        where 1=1 
                        and delaytime = @delaytime 
                        and coefficient = @coefficient 
                        and version = 'new' 
                        order by station ";
            List<dynamic> data = dbDapper.Query(ssql, new { delaytime = ViewBag.delaytime, coefficient = ViewBag.coefficient });

            //資料筆數
            ViewBag.count = data.Count;
            ViewData["Rti3Detail"] = data;

            return View();
        }

        public ActionResult ExpRainCountry()
        {
            return View();
        }



        public ActionResult DownRainCountry(string sd, string ed, string country)
        {
            DateTime dtStart;
            DateTime dtEnd;

            sd = sd.Replace(' ', '-');
            ed = ed.Replace(' ', '-');

            if (string.IsNullOrEmpty(sd) || string.IsNullOrEmpty(ed))
            {
                dtStart = DateTime.Now;
                dtEnd = DateTime.Now;
            }
            else
            {
                string[] aSdt = sd.Split('-');
                string[] aEdt = ed.Split('-');

                dtStart = new DateTime(Convert.ToInt32(aSdt[0]), Convert.ToInt32(aSdt[1]), Convert.ToInt32(aSdt[2]), Convert.ToInt32(aSdt[3]), 0, 1);
                dtEnd = new DateTime(Convert.ToInt32(aEdt[0]), Convert.ToInt32(aEdt[1]), Convert.ToInt32(aEdt[2]), Convert.ToInt32(aEdt[3]), 59, 59);
            }

            string sStartDate = dtStart.ToString("yyyy-MM-ddTHH:mm:ss");
            string sEndDate = dtEnd.ToString("yyyy-MM-ddTHH:mm:ss");

            string ssql = @" select * from RainStation where 1=1
                      and RTime between @sd and @ed
                      and datepart(mi,RTime) = 0 and datepart(ss,RTime) = 0
                        ";
            if (country != "全部") ssql += " and COUNTY = @COUNTY ";
            ssql += " order by RTime ";

            List<RainStation> DataList = new List<RainStation>();
            DataList = dbDapper.Query<RainStation>(ssql, new { COUNTY = country, sd = sStartDate, ed = sEndDate });

            //產生檔案路徑
            string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
            //建立資料夾
            Directory.CreateDirectory(sTempPath);
            string sSaveFilePath = Path.Combine(sTempPath, "RainDataByCountry_" + Guid.NewGuid().ToString() + ".csv");

            DataTable dt = Utils.ConvertToDataTable<RainStation>(DataList);


            DataExport de = new DataExport();
            Boolean bSuccess = de.ExportBigDataToCsv(sSaveFilePath, dt);

            if (bSuccess)
            {
                string filename = string.Format("RainDataByCountry_{0}_{1}.csv", sd, ed).Replace("-", "");

                //ASP.NET 回應大型檔案的注意事項
                //http://blog.miniasp.com/post/2008/03/11/Caution-about-ASPNET-Response-a-Large-File.aspx


                //***** 下載檔案過大，使用特殊方法 *****
                HttpContext context = System.Web.HttpContext.Current;
                context.Response.TransmitFile(sSaveFilePath);
                context.Response.ContentType = "text/csv";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.End();
            }

            return null;
        }

        public ActionResult DownRainStation(string sd, string ed, string Station)
        {
            DateTime dtStart;
            DateTime dtEnd;

            sd = sd.Replace(' ', '-');
            ed = ed.Replace(' ', '-');

            if (string.IsNullOrEmpty(sd) || string.IsNullOrEmpty(ed))
            {
                dtStart = DateTime.Now;
                dtEnd = DateTime.Now;
            }
            else
            {
                string[] aSdt = sd.Split('-');
                string[] aEdt = ed.Split('-');

                dtStart = new DateTime(Convert.ToInt32(aSdt[0]), Convert.ToInt32(aSdt[1]), Convert.ToInt32(aSdt[2]), Convert.ToInt32(aSdt[3]), 0, 1);
                dtEnd = new DateTime(Convert.ToInt32(aEdt[0]), Convert.ToInt32(aEdt[1]), Convert.ToInt32(aEdt[2]), Convert.ToInt32(aEdt[3]), 59, 59);
            }

            string sStartDate = dtStart.ToString("yyyy-MM-ddTHH:mm:ss");
            string sEndDate = dtEnd.ToString("yyyy-MM-ddTHH:mm:ss");

            string ssql = @" select * from RainStation where 1=1
                      and STID = @STID
                      and RTime between @sd and @ed
                      and datepart(mi,RTime) = 0 and datepart(ss,RTime) = 0
                       order by RTime   ";

            List<RainStation> DataList = new List<RainStation>();
            DataList = dbDapper.Query<RainStation>(ssql, new { STID = Station, sd = sStartDate, ed = sEndDate });

            //產生檔案路徑
            string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
            //建立資料夾
            Directory.CreateDirectory(sTempPath);
            string sSaveFilePath = Path.Combine(sTempPath, "RainDataByStation_" + Guid.NewGuid().ToString() + ".csv");

            DataTable dt = Utils.ConvertToDataTable<RainStation>(DataList);


            DataExport de = new DataExport();
            Boolean bSuccess = de.ExportBigDataToCsv(sSaveFilePath, dt);

            if (bSuccess)
            {
                string filename = string.Format("RainDataByStation_{0}_{1}.csv", sd, ed).Replace("-", "");

                //ASP.NET 回應大型檔案的注意事項
                //http://blog.miniasp.com/post/2008/03/11/Caution-about-ASPNET-Response-a-Large-File.aspx


                //***** 下載檔案過大，使用特殊方法 *****
                HttpContext context = System.Web.HttpContext.Current;
                context.Response.TransmitFile(sSaveFilePath);
                context.Response.ContentType = "text/csv";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.End();
            }

            return null;
        }

        public ActionResult DownWeaRainHis(string sd, string ed, string Station)
        {
            DateTime dtStart;
            DateTime dtEnd;

            sd = sd.Replace(' ', '-');
            ed = ed.Replace(' ', '-');

            if (string.IsNullOrEmpty(sd) || string.IsNullOrEmpty(ed))
            {
                dtStart = DateTime.Now;
                dtEnd = DateTime.Now;
            }
            else
            {
                string[] aSdt = sd.Split('-');
                string[] aEdt = ed.Split('-');

                dtStart = new DateTime(Convert.ToInt32(aSdt[0]), Convert.ToInt32(aSdt[1]), Convert.ToInt32(aSdt[2]), Convert.ToInt32(aSdt[3]), 0, 1);
                dtEnd = new DateTime(Convert.ToInt32(aEdt[0]), Convert.ToInt32(aEdt[1]), Convert.ToInt32(aEdt[2]), Convert.ToInt32(aEdt[3]), 59, 59);
            }

            string sStartDate = dtStart.ToString("yyyyMMddHH");
            string sEndDate = dtEnd.ToString("yyyyMMddHH");

            string ssql = @" select * from WeaRainDataHis where 1=1
                              and STID = @STID
                              and Time between @sd and @ed                      
                              order by Time   ";

            List<WeaRainDataHis> DataList = new List<WeaRainDataHis>();
            DataList = dbDapper.Query<WeaRainDataHis>(ssql, new { STID = Station, sd = sStartDate, ed = sEndDate });

            //產生檔案路徑
            string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
            //建立資料夾
            Directory.CreateDirectory(sTempPath);
            string sSaveFilePath = Path.Combine(sTempPath, "WeaRainDataHisByStation_" + Guid.NewGuid().ToString() + ".xlsx");

            DataTable dt = Utils.ConvertToDataTable<WeaRainDataHis>(DataList);

            List<string> head = new List<string>();
            head.Add("站號");
            head.Add("時間");
            head.Add("Hour");
            head.Add("Hour3");
            head.Add("Hour6");
            head.Add("Hour12");
            head.Add("Hour24");
            head.Add("DayRainFall");
            head.Add("RT");


            List<string[]> datas = new List<string[]>();
            foreach (WeaRainDataHis item in DataList)
            {
                List<string> cols = new List<string>();
                cols.Add(item.Stid);

                //日期格式化
                DateTime MainTime = DateTime.ParseExact(item.Time, "yyyyMMddHH", null);

                cols.Add(MainTime.ToString("yyyy/MM/dd HH:00"));
                cols.Add(item.Hour.ToString());
                cols.Add(item.Hour3.ToString());
                cols.Add(item.Hour6.ToString());
                cols.Add(item.Hour12.ToString());
                cols.Add(item.Hour24.ToString());
                cols.Add(item.DayRainfall.ToString());
                cols.Add(item.RT.ToString());
                datas.Add(cols.ToArray());
            }

            DataExport de = new DataExport();
            Boolean bSuccess = de.ExportListToExcel(sSaveFilePath, head, datas);

            if (bSuccess)
            {
                string filename = string.Format("WeaRainDataHis_{0}_{1}_{2}.xlsx", Station, sd, ed).Replace("-", "");

                //ASP.NET 回應大型檔案的注意事項
                //http://blog.miniasp.com/post/2008/03/11/Caution-about-ASPNET-Response-a-Large-File.aspx


                //***** 下載檔案過大，使用特殊方法 *****
                HttpContext context = System.Web.HttpContext.Current;
                context.Response.TransmitFile(sSaveFilePath);
                context.Response.ContentType = "application/vnd.ms-excel";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.End();
            }

            return null;
        }

        public ActionResult DownSoilWaterRainHis(string sd, string ed, string Station)
        {
            DateTime dtStart;
            DateTime dtEnd;

            sd = sd.Replace(' ', '-');
            ed = ed.Replace(' ', '-');

            if (string.IsNullOrEmpty(sd) || string.IsNullOrEmpty(ed))
            {
                dtStart = DateTime.Now;
                dtEnd = DateTime.Now;
            }
            else
            {
                string[] aSdt = sd.Split('-');
                string[] aEdt = ed.Split('-');

                dtStart = new DateTime(Convert.ToInt32(aSdt[0]), Convert.ToInt32(aSdt[1]), Convert.ToInt32(aSdt[2]), Convert.ToInt32(aSdt[3]), 0, 1);
                dtEnd = new DateTime(Convert.ToInt32(aEdt[0]), Convert.ToInt32(aEdt[1]), Convert.ToInt32(aEdt[2]), Convert.ToInt32(aEdt[3]), 59, 59);
            }
            
            string sStartDate = dtStart.ToString("yyyyMMddHH");
            string sEndDate = dtEnd.ToString("yyyyMMddHH");

            string ssql = @" select * from SoilWaterRainDataHis where 1=1
                              and STID = @STID
                              and Time between @sd and @ed                      
                              order by Time   ";

            List<SoilWaterRainDataHis> DataList = new List<SoilWaterRainDataHis>();
            DataList = dbDapper.Query<SoilWaterRainDataHis>(ssql, new { STID = Station, sd = sStartDate, ed = sEndDate });

            //產生檔案路徑
            string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
            //建立資料夾
            Directory.CreateDirectory(sTempPath);
            string sSaveFilePath = Path.Combine(sTempPath, "SoilWaterRainDataHisByStation_" + Guid.NewGuid().ToString() + ".xlsx");

            DataTable dt = Utils.ConvertToDataTable<SoilWaterRainDataHis>(DataList);


            List<string> head = new List<string>();
            head.Add("站號");
            head.Add("時間");
            head.Add("Hour");
            head.Add("Hour3");
            head.Add("Hour6");
            head.Add("Hour12");
            head.Add("Hour24");
            head.Add("DayRainFall");
            head.Add("RT");
            
            List<string[]> datas = new List<string[]>();
            foreach (SoilWaterRainDataHis item in DataList)
            {   
                //日期格式化
                DateTime MainTime = DateTime.ParseExact(item.Time, "yyyyMMddHH", null);

                List<string> cols = new List<string>();
                cols.Add(item.Stid);
                cols.Add(MainTime.ToString("yyyy/MM/dd HH:00"));                
                cols.Add(item.Hour.ToString());
                cols.Add(item.Hour3.ToString());
                cols.Add(item.Hour6.ToString());
                cols.Add(item.Hour12.ToString());
                cols.Add(item.Hour24.ToString());
                cols.Add(item.DayRainfall.ToString());
                cols.Add(item.RT.ToString());

                datas.Add(cols.ToArray());
            }

            DataExport de = new DataExport();
            Boolean bSuccess = de.ExportListToExcel(sSaveFilePath, head, datas);

            if (bSuccess)
            {
                string filename = string.Format("SoilWaterRainDataHis_{0}_{1}_{2}.xlsx", Station, sd, ed).Replace("-", "");

                //ASP.NET 回應大型檔案的注意事項
                //http://blog.miniasp.com/post/2008/03/11/Caution-about-ASPNET-Response-a-Large-File.aspx


                //***** 下載檔案過大，使用特殊方法 *****
                HttpContext context = System.Web.HttpContext.Current;
                context.Response.TransmitFile(sSaveFilePath);
                context.Response.ContentType = "application/vnd.ms-excel";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.End();
            }

            return null;
        }

        public ActionResult DownWraRainHis(string sd, string ed, string Station)
        {
            DateTime dtStart;
            DateTime dtEnd;

            sd = sd.Replace(' ', '-');
            ed = ed.Replace(' ', '-');

            if (string.IsNullOrEmpty(sd) || string.IsNullOrEmpty(ed))
            {
                dtStart = DateTime.Now;
                dtEnd = DateTime.Now;
            }
            else
            {
                string[] aSdt = sd.Split('-');
                string[] aEdt = ed.Split('-');

                dtStart = new DateTime(Convert.ToInt32(aSdt[0]), Convert.ToInt32(aSdt[1]), Convert.ToInt32(aSdt[2]), Convert.ToInt32(aSdt[3]), 0, 1);
                dtEnd = new DateTime(Convert.ToInt32(aEdt[0]), Convert.ToInt32(aEdt[1]), Convert.ToInt32(aEdt[2]), Convert.ToInt32(aEdt[3]), 59, 59);
            }

            string sStartDate = dtStart.ToString("yyyyMMddHH");
            string sEndDate = dtEnd.ToString("yyyyMMddHH");

            string ssql = @" select * from WraRainDataHis where 1=1
                              and STID = @STID
                              and Time between @sd and @ed                      
                              order by Time   ";

            List<SoilWaterRainDataHis> DataList = new List<SoilWaterRainDataHis>();
            DataList = dbDapper.Query<SoilWaterRainDataHis>(ssql, new { STID = Station, sd = sStartDate, ed = sEndDate });

            //產生檔案路徑
            string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
            //建立資料夾
            Directory.CreateDirectory(sTempPath);
            string sSaveFilePath = Path.Combine(sTempPath, "WraRainDataHisByStation_" + Guid.NewGuid().ToString() + ".xlsx");

            DataTable dt = Utils.ConvertToDataTable<SoilWaterRainDataHis>(DataList);


            List<string> head = new List<string>();
            head.Add("站號");
            head.Add("時間");
            head.Add("Hour");
            head.Add("Hour3");
            head.Add("Hour6");
            head.Add("Hour12");
            head.Add("Hour24");
            head.Add("DayRainFall");
            head.Add("RT");

            List<string[]> datas = new List<string[]>();
            foreach (SoilWaterRainDataHis item in DataList)
            {
                //日期格式化
                DateTime MainTime = DateTime.ParseExact(item.Time, "yyyyMMddHH", null);

                List<string> cols = new List<string>();
                cols.Add(item.Stid);
                cols.Add(MainTime.ToString("yyyy/MM/dd HH:00"));
                cols.Add(item.Hour.ToString());
                cols.Add(item.Hour3.ToString());
                cols.Add(item.Hour6.ToString());
                cols.Add(item.Hour12.ToString());
                cols.Add(item.Hour24.ToString());
                cols.Add(item.DayRainfall.ToString());
                cols.Add(item.RT.ToString());

                datas.Add(cols.ToArray());
            }

            DataExport de = new DataExport();
            Boolean bSuccess = de.ExportListToExcel(sSaveFilePath, head, datas);

            if (bSuccess)
            {
                string filename = string.Format("WraRainDataHis_{0}_{1}_{2}.xlsx", Station, sd, ed).Replace("-", "");

                //ASP.NET 回應大型檔案的注意事項
                //http://blog.miniasp.com/post/2008/03/11/Caution-about-ASPNET-Response-a-Large-File.aspx


                //***** 下載檔案過大，使用特殊方法 *****
                HttpContext context = System.Web.HttpContext.Current;
                context.Response.TransmitFile(sSaveFilePath);
                context.Response.ContentType = "application/vnd.ms-excel";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.End();
            }

            return null;
        }

        public ActionResult DownSumRain(string sd, string ed)
        {
            DateTime dtStart;
            DateTime dtEnd;

            sd = sd.Replace(' ', '-');
            ed = ed.Replace(' ', '-');

            if (string.IsNullOrEmpty(sd) || string.IsNullOrEmpty(ed))
            {
                dtStart = DateTime.Now;
                dtEnd = DateTime.Now;
            }
            else
            {
                string[] aSdt = sd.Split('-');
                string[] aEdt = ed.Split('-');

                dtStart = new DateTime(Convert.ToInt32(aSdt[0]), Convert.ToInt32(aSdt[1]), Convert.ToInt32(aSdt[2]), Convert.ToInt32(aSdt[3]), 0, 1);
                dtEnd = new DateTime(Convert.ToInt32(aEdt[0]), Convert.ToInt32(aEdt[1]), Convert.ToInt32(aEdt[2]), Convert.ToInt32(aEdt[3]), 59, 59);
            }

            string sStartDate = dtStart.ToString("yyyy-MM-ddTHH:mm:ss");
            string sEndDate = dtEnd.ToString("yyyy-MM-ddTHH:mm:ss");

            string ssql = @" select STID,STNAME,COUNTY,LAT,LON,sum(CONVERT(float, RAIN)) as RAIN from RainStation where 1=1
                       and RTime between @sd and @ed
                       and datepart(mi,RTime) = 0 and datepart(ss,RTime) = 0
                       group by STID,STNAME,COUNTY,LAT,LON   ";

            //List<RainStation> DataList = new List<RainStation>();
            var DataList = dbDapper.Query(ssql, new { sd = sStartDate, ed = sEndDate });

            //產生檔案路徑
            string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
            //建立資料夾
            Directory.CreateDirectory(sTempPath);
            string sSaveFilePath = Path.Combine(sTempPath, "SumRainData_" + Guid.NewGuid().ToString() + ".csv");

            DataTable dt = new DataTable();
            dt.Columns.Add("STID");
            dt.Columns.Add("STNAME");
            dt.Columns.Add("COUNTY");
            dt.Columns.Add("LAT");
            dt.Columns.Add("LON");
            dt.Columns.Add("RAIN");

            foreach (var item in DataList)
            {
                DataRow NewRow = dt.NewRow();
                NewRow["STID"] = item.STID;
                NewRow["STNAME"] = item.STNAME;
                NewRow["COUNTY"] = item.COUNTY;
                NewRow["LAT"] = item.LAT;
                NewRow["LON"] = item.LON;
                NewRow["RAIN"] = item.RAIN;

                dt.Rows.Add(NewRow);
            }

            DataExport de = new DataExport();
            Boolean bSuccess = de.ExportBigDataToCsv(sSaveFilePath, dt);

            if (bSuccess)
            {
                string filename = string.Format("SumRainData_{0}_{1}.csv", sd, ed).Replace("-", "");

                //ASP.NET 回應大型檔案的注意事項
                //http://blog.miniasp.com/post/2008/03/11/Caution-about-ASPNET-Response-a-Large-File.aspx


                //***** 下載檔案過大，使用特殊方法 *****
                HttpContext context = System.Web.HttpContext.Current;
                context.Response.TransmitFile(sSaveFilePath);
                context.Response.ContentType = "text/csv";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.End();
            }

            return null;
        }

        public ActionResult ExpRainStation()
        {
            return View();
        }

        public ActionResult ExpRainArea()
        {
            return View();
        }

        public ActionResult ExpRainAreaHis()
        {
            return View();
        }

        public ActionResult ExpWeaRainHis()
        {
            return View();
        }

        public ActionResult ExpSoilWaterRainHis()
        {
            return View();
        }

        public ActionResult ExpWraRainHis()
        {
            return View();
        }

        public ActionResult ExpStationCoord()
        {
            return View();
        }

        public bool xDownload(string xFile, string out_file)
        {
            if (System.IO.File.Exists(xFile))
            {
                try
                {
                    FileInfo xpath_file = new FileInfo(xFile);
                    System.Web.HttpContext.Current.Response.Clear(); //清除buffer
                    System.Web.HttpContext.Current.Response.ClearHeaders(); //清除 buffer 表頭
                    System.Web.HttpContext.Current.Response.Buffer = false;
                    System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                    // 檔案類型還有下列幾種"application/pdf"、"application/vnd.ms-excel"、"text/xml"、"text/HTML"、"image/JPEG"、"image/GIF"
                    //System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(out_file, System.Text.Encoding.UTF8));
                    System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(xpath_file.Name, System.Text.Encoding.UTF8));
                    // 考慮 utf-8 檔名問題，以 out_file 設定另存的檔名
                    System.Web.HttpContext.Current.Response.AppendHeader("Content-Length", xpath_file.Length.ToString()); //表頭加入檔案大小
                    System.Web.HttpContext.Current.Response.WriteFile(xpath_file.FullName);

                    // 將檔案輸出
                    System.Web.HttpContext.Current.Response.Flush();
                    // 強制 Flush buffer 內容
                    System.Web.HttpContext.Current.Response.End();
                    return true;

                }
                catch (Exception)
                { return false; }

            }
            else
                return false;
        }

        public ActionResult QueryRainStatistics(string stid)
        {


            //string cityid1 = Request.QueryString["cityId"];
            ViewBag.SelectCountry = stid;


            ssql = @"   select * from WeaRainStatistics where stid = @stid order by year ";


            List<WeaRainStatistics> wrs = dbDapper.Query<WeaRainStatistics>(ssql, new { stid = stid });

            List<dynamic> data = dbDapper.Query(ssql, new { stid = stid });
            foreach (var LoopItem in data)
            {

                if (LoopItem.m01 == -99) LoopItem.m01 = "*";
                if (LoopItem.m02 == -99) LoopItem.m02 = "*";
                if (LoopItem.m03 == -99) LoopItem.m03 = "*";
                if (LoopItem.m04 == -99) LoopItem.m04 = "*";
                if (LoopItem.m05 == -99) LoopItem.m05 = "*";
                if (LoopItem.m06 == -99) LoopItem.m06 = "*";
                if (LoopItem.m07 == -99) LoopItem.m07 = "*";
                if (LoopItem.m08 == -99) LoopItem.m08 = "*";
                if (LoopItem.m09 == -99) LoopItem.m09 = "*";
                if (LoopItem.m10 == -99) LoopItem.m10 = "*";
                if (LoopItem.m11 == -99) LoopItem.m11 = "*";
                if (LoopItem.m12 == -99) LoopItem.m12 = "*";

                if (LoopItem.yearsum == -99) LoopItem.yearsum = "*";
            }
            //1月平均雨量


            List<decimal> m01List = wrs.Select(c => c.m01).ToList<decimal>();
            List<decimal> m02List = wrs.Select(c => c.m02).ToList<decimal>();
            List<decimal> m03List = wrs.Select(c => c.m03).ToList<decimal>();
            List<decimal> m04List = wrs.Select(c => c.m04).ToList<decimal>();
            List<decimal> m05List = wrs.Select(c => c.m05).ToList<decimal>();
            List<decimal> m06List = wrs.Select(c => c.m06).ToList<decimal>();
            List<decimal> m07List = wrs.Select(c => c.m07).ToList<decimal>();
            List<decimal> m08List = wrs.Select(c => c.m08).ToList<decimal>();
            List<decimal> m09List = wrs.Select(c => c.m09).ToList<decimal>();
            List<decimal> m10List = wrs.Select(c => c.m10).ToList<decimal>();
            List<decimal> m11List = wrs.Select(c => c.m11).ToList<decimal>();
            List<decimal> m12List = wrs.Select(c => c.m12).ToList<decimal>();
            List<decimal> mavgList = wrs.Select(c => c.mavg).ToList<decimal>();
            List<decimal> yearsumList = wrs.Select(c => c.yearsum).ToList<decimal>();






            //平均雨量統計
            List<dynamic> DataAvg = new List<dynamic>();
            dynamic test = new WeaRainStatistics();
            test.m01 = RainControlUtil.CalcRainAvg(m01List);
            test.m02 = RainControlUtil.CalcRainAvg(m02List);
            test.m03 = RainControlUtil.CalcRainAvg(m03List);
            test.m04 = RainControlUtil.CalcRainAvg(m04List);
            test.m05 = RainControlUtil.CalcRainAvg(m05List);
            test.m06 = RainControlUtil.CalcRainAvg(m06List);
            test.m07 = RainControlUtil.CalcRainAvg(m07List);
            test.m08 = RainControlUtil.CalcRainAvg(m08List);
            test.m09 = RainControlUtil.CalcRainAvg(m09List);
            test.m10 = RainControlUtil.CalcRainAvg(m10List);
            test.m11 = RainControlUtil.CalcRainAvg(m11List);
            test.m12 = RainControlUtil.CalcRainAvg(m12List);
            test.mavg = RainControlUtil.CalcRainAvg(mavgList);
            test.yearsum = RainControlUtil.CalcRainAvg(yearsumList);
            DataAvg.Add(test);








            //資料筆數
            ViewBag.count = data.Count;
            ViewData["RainData"] = data;
            ViewData["DataAvg"] = DataAvg;





            return View();
        }

        public ActionResult DownRainStatistics(string stid)
        {
            string ssql = @"   select * from WeaRainStatistics where stid = @stid ";


            List<WeaRainStatistics> wrs = dbDapper.Query<WeaRainStatistics>(ssql, new { stid = stid });

            List<dynamic> data = dbDapper.Query(ssql, new { stid = stid });
            foreach (var LoopItem in data)
            {

                if (LoopItem.m01 == -99) LoopItem.m01 = "*";
                if (LoopItem.m02 == -99) LoopItem.m02 = "*";
                if (LoopItem.m03 == -99) LoopItem.m03 = "*";
                if (LoopItem.m04 == -99) LoopItem.m04 = "*";
                if (LoopItem.m05 == -99) LoopItem.m05 = "*";
                if (LoopItem.m06 == -99) LoopItem.m06 = "*";
                if (LoopItem.m07 == -99) LoopItem.m07 = "*";
                if (LoopItem.m08 == -99) LoopItem.m08 = "*";
                if (LoopItem.m09 == -99) LoopItem.m09 = "*";
                if (LoopItem.m10 == -99) LoopItem.m10 = "*";
                if (LoopItem.m11 == -99) LoopItem.m11 = "*";
                if (LoopItem.m12 == -99) LoopItem.m12 = "*";

                if (LoopItem.yearsum == -99) LoopItem.yearsum = "*";
            }

            List<decimal> m01List = wrs.Select(c => c.m01).ToList<decimal>();
            List<decimal> m02List = wrs.Select(c => c.m02).ToList<decimal>();
            List<decimal> m03List = wrs.Select(c => c.m03).ToList<decimal>();
            List<decimal> m04List = wrs.Select(c => c.m04).ToList<decimal>();
            List<decimal> m05List = wrs.Select(c => c.m05).ToList<decimal>();
            List<decimal> m06List = wrs.Select(c => c.m06).ToList<decimal>();
            List<decimal> m07List = wrs.Select(c => c.m07).ToList<decimal>();
            List<decimal> m08List = wrs.Select(c => c.m08).ToList<decimal>();
            List<decimal> m09List = wrs.Select(c => c.m09).ToList<decimal>();
            List<decimal> m10List = wrs.Select(c => c.m10).ToList<decimal>();
            List<decimal> m11List = wrs.Select(c => c.m11).ToList<decimal>();
            List<decimal> m12List = wrs.Select(c => c.m12).ToList<decimal>();
            List<decimal> mavgList = wrs.Select(c => c.mavg).ToList<decimal>();
            List<decimal> yearsumList = wrs.Select(c => c.yearsum).ToList<decimal>();

            //平均雨量統計

            WeaRainStatistics test = new WeaRainStatistics();
            test.year = "平均雨量";
            test.m01 = RainControlUtil.CalcRainAvg(m01List);
            test.m02 = RainControlUtil.CalcRainAvg(m02List);
            test.m03 = RainControlUtil.CalcRainAvg(m03List);
            test.m04 = RainControlUtil.CalcRainAvg(m04List);
            test.m05 = RainControlUtil.CalcRainAvg(m05List);
            test.m06 = RainControlUtil.CalcRainAvg(m06List);
            test.m07 = RainControlUtil.CalcRainAvg(m07List);
            test.m08 = RainControlUtil.CalcRainAvg(m08List);
            test.m09 = RainControlUtil.CalcRainAvg(m09List);
            test.m10 = RainControlUtil.CalcRainAvg(m10List);
            test.m11 = RainControlUtil.CalcRainAvg(m11List);
            test.m12 = RainControlUtil.CalcRainAvg(m12List);
            test.mavg = RainControlUtil.CalcRainAvg(mavgList);
            test.yearsum = RainControlUtil.CalcRainAvg(yearsumList);

            wrs.Add(test);

            List<string> head = new List<string>();
            head.Add("年份");
            head.Add("1月");
            head.Add("2月");
            head.Add("3月");
            head.Add("4月");
            head.Add("5月");
            head.Add("6月");
            head.Add("7月");
            head.Add("8月");
            head.Add("9月");
            head.Add("10月");
            head.Add("11月");
            head.Add("12月");
            head.Add("月平均");
            head.Add("年雨量");
            head.Add("最大1日雨量");
            head.Add("最大1日發生日");
            head.Add("最大2日雨量");
            head.Add("最大2日發生日");
            head.Add("最大3日雨量");
            head.Add("最大3日發生日");
            head.Add("降雨日數");


            List<string[]> datas = new List<string[]>();
            foreach (WeaRainStatistics item in wrs)
            {
                List<string> cols = new List<string>();
                cols.Add(item.year);
                cols.Add(item.m01 == -99 ? "*" : item.m01.ToString());
                cols.Add(item.m02 == -99 ? "*" : item.m02.ToString());
                cols.Add(item.m03 == -99 ? "*" : item.m03.ToString());
                cols.Add(item.m04 == -99 ? "*" : item.m04.ToString());
                cols.Add(item.m05 == -99 ? "*" : item.m05.ToString());
                cols.Add(item.m06 == -99 ? "*" : item.m06.ToString());
                cols.Add(item.m07 == -99 ? "*" : item.m07.ToString());
                cols.Add(item.m08 == -99 ? "*" : item.m08.ToString());
                cols.Add(item.m09 == -99 ? "*" : item.m09.ToString());
                cols.Add(item.m10 == -99 ? "*" : item.m10.ToString());
                cols.Add(item.m11 == -99 ? "*" : item.m11.ToString());
                cols.Add(item.m12 == -99 ? "*" : item.m12.ToString());

                cols.Add(item.mavg.ToString());
                cols.Add(item.yearsum == -99 ? "*" : item.yearsum.ToString());
                cols.Add(item.max1.ToString());
                cols.Add(item.max1date);
                cols.Add(item.max2.ToString());
                cols.Add(item.max2date);
                cols.Add(item.max3.ToString());
                cols.Add(item.max3date);
                cols.Add(item.raindatecount.ToString());



                datas.Add(cols.ToArray());
            }




            //產生檔案路徑
            string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
            //建立資料夾
            Directory.CreateDirectory(sTempPath);

            string sFileName = "WeaRainStatistics_" + Guid.NewGuid().ToString() + ".xlsx";
            string sSaveFilePath = Path.Combine(sTempPath, "WeaRainStatistics_" + Guid.NewGuid().ToString() + ".xlsx");

            //DataTable dt = Utils.ConvertToDataTable<RainStation>(wrs);
            //DataTable dt = new DataTable();


            DataExport de = new DataExport();
            //Boolean bSuccess = de.ExportBigDataToCsv(sSaveFilePath, dt);
            Boolean bSuccess = de.ExportListToExcel(sSaveFilePath, head, datas);







            if (bSuccess)
            {
                string filename = string.Format("WeaRainStatistics_{0}_{1}.xlsx", stid, stid);

                //ASP.NET 回應大型檔案的注意事項
                //http://blog.miniasp.com/post/2008/03/11/Caution-about-ASPNET-Response-a-Large-File.aspx


                //***** 下載檔案過大，使用特殊方法 *****
                HttpContext context = System.Web.HttpContext.Current;
                context.Response.TransmitFile(sSaveFilePath);
                context.Response.ContentType = "application/vnd.ms-excel";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.End();
            }

            return null;
        }

        public ActionResult DownRainStatisticsAll(string ExpAllType)
        {

            //取得Excel需要的資料
            string ssql = @" 
                        select WeaRainStatistics.*, BasStationData.stname from WeaRainStatistics 
                        left join BasStationData on WeaRainStatistics.stid = BasStationData.stid
                        order by WeaRainStatistics.stid
                    ";
            List<BasStationData> lBasStationData = dbDapper.Query<BasStationData>(ssql);

            ssql = @" 
                        select * from WeaRainStatistics 
                        order by stid,year
                    ";
            List<WeaRainStatistics> wrs = dbDapper.Query<WeaRainStatistics>(ssql);

            //取得所有站號
            List<string> allStid = wrs.DistinctBy(x => x.stid).Select(x => x.stid).ToList<string>();

            switch (ExpAllType)
            {
                //僅匯出歷年平均雨量資料
                case "ExpAllAvgRain":
                                        
                    List<WeaRainStatistics> wrsAllByStid = new List<WeaRainStatistics>();
                    //每個站號進行統計
                    foreach (string item in allStid)
                    {   
                        //取得測站名稱
                        BasStationData bsd = lBasStationData.FirstOrDefault(x => x.stid == item);

                        //取得某個站號雨量統計資料
                        List<WeaRainStatistics> dataByStid = wrs.Where(x => x.stid == item).ToList();

                        //計算平均值
                        List<decimal> m01List = dataByStid.Select(c => c.m01).ToList<decimal>();
                        List<decimal> m02List = dataByStid.Select(c => c.m02).ToList<decimal>();
                        List<decimal> m03List = dataByStid.Select(c => c.m03).ToList<decimal>();
                        List<decimal> m04List = dataByStid.Select(c => c.m04).ToList<decimal>();
                        List<decimal> m05List = dataByStid.Select(c => c.m05).ToList<decimal>();
                        List<decimal> m06List = dataByStid.Select(c => c.m06).ToList<decimal>();
                        List<decimal> m07List = dataByStid.Select(c => c.m07).ToList<decimal>();
                        List<decimal> m08List = dataByStid.Select(c => c.m08).ToList<decimal>();
                        List<decimal> m09List = dataByStid.Select(c => c.m09).ToList<decimal>();
                        List<decimal> m10List = dataByStid.Select(c => c.m10).ToList<decimal>();
                        List<decimal> m11List = dataByStid.Select(c => c.m11).ToList<decimal>();
                        List<decimal> m12List = dataByStid.Select(c => c.m12).ToList<decimal>();
                        List<decimal> mavgList = dataByStid.Select(c => c.mavg).ToList<decimal>();
                        List<decimal> yearsumList = dataByStid.Select(c => c.yearsum).ToList<decimal>();

                        WeaRainStatistics wrsItem = new WeaRainStatistics();
                        wrsItem.stid = item;
                        //拿Year欄位當作測站名稱                  
                        wrsItem.year = bsd == null ? "" : bsd.stname;
                        wrsItem.m01 = RainControlUtil.CalcRainAvg(m01List);
                        wrsItem.m02 = RainControlUtil.CalcRainAvg(m02List);
                        wrsItem.m03 = RainControlUtil.CalcRainAvg(m03List);
                        wrsItem.m04 = RainControlUtil.CalcRainAvg(m04List);
                        wrsItem.m05 = RainControlUtil.CalcRainAvg(m05List);
                        wrsItem.m06 = RainControlUtil.CalcRainAvg(m06List);
                        wrsItem.m07 = RainControlUtil.CalcRainAvg(m07List);
                        wrsItem.m08 = RainControlUtil.CalcRainAvg(m08List);
                        wrsItem.m09 = RainControlUtil.CalcRainAvg(m09List);
                        wrsItem.m10 = RainControlUtil.CalcRainAvg(m10List);
                        wrsItem.m11 = RainControlUtil.CalcRainAvg(m11List);
                        wrsItem.m12 = RainControlUtil.CalcRainAvg(m12List);
                        wrsItem.mavg = RainControlUtil.CalcRainAvg(mavgList);
                        wrsItem.yearsum = RainControlUtil.CalcRainAvg(yearsumList);

                        wrsAllByStid.Add(wrsItem);
                    }

                    //準備Excel資料
                    List<string> head = new List<string>();
                    head.Add("測站編號");
                    head.Add("測站名稱");
                    head.Add("1月");
                    head.Add("2月");
                    head.Add("3月");
                    head.Add("4月");
                    head.Add("5月");
                    head.Add("6月");
                    head.Add("7月");
                    head.Add("8月");
                    head.Add("9月");
                    head.Add("10月");
                    head.Add("11月");
                    head.Add("12月");
                    head.Add("月平均");
                    head.Add("年雨量");

                    List<string[]> datas = new List<string[]>();
                    foreach (WeaRainStatistics item in wrsAllByStid)
                    {
                        List<string> cols = new List<string>();
                        cols.Add(item.stid);
                        cols.Add(item.year);
                        cols.Add(item.m01 == -99 ? "*" : item.m01.ToString());
                        cols.Add(item.m02 == -99 ? "*" : item.m02.ToString());
                        cols.Add(item.m03 == -99 ? "*" : item.m03.ToString());
                        cols.Add(item.m04 == -99 ? "*" : item.m04.ToString());
                        cols.Add(item.m05 == -99 ? "*" : item.m05.ToString());
                        cols.Add(item.m06 == -99 ? "*" : item.m06.ToString());
                        cols.Add(item.m07 == -99 ? "*" : item.m07.ToString());
                        cols.Add(item.m08 == -99 ? "*" : item.m08.ToString());
                        cols.Add(item.m09 == -99 ? "*" : item.m09.ToString());
                        cols.Add(item.m10 == -99 ? "*" : item.m10.ToString());
                        cols.Add(item.m11 == -99 ? "*" : item.m11.ToString());
                        cols.Add(item.m12 == -99 ? "*" : item.m12.ToString());

                        cols.Add(item.mavg.ToString());
                        cols.Add(item.yearsum == -99 ? "*" : item.yearsum.ToString());
                        datas.Add(cols.ToArray());
                    }

                    //產生Excel

                    //產生檔案路徑
                    string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
                    //建立資料夾
                    Directory.CreateDirectory(sTempPath);

                    string sFileName = "WeaRainStatisticsAll_" + Guid.NewGuid().ToString() + ".xlsx";
                    string sSaveFilePath = Path.Combine(sTempPath, "WeaRainStatisticsAll_" + Guid.NewGuid().ToString() + ".xlsx");

                    DataExport de = new DataExport();
                    Boolean bSuccess = de.ExportListToExcel(sSaveFilePath, head, datas);

                    //下載Excel
                    if (bSuccess)
                    {
                        string filename = string.Format("WeaRainStatisticsAll_雨量站統計資料僅匯出歷年平均雨量資料.xlsx", ExpAllType, ExpAllType);

                        //***** 下載檔案過大，使用特殊方法 *****
                        HttpContext context = System.Web.HttpContext.Current;
                        context.Response.TransmitFile(sSaveFilePath);
                        context.Response.ContentType = "application/vnd.ms-excel";
                        context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                        Response.End();
                    }

                    break;

                //匯出個年度統計結果
                case "ExpAllStaResult":

                    List<string[]> datasResult = new List<string[]>();
                    List<WeaRainStatistics> wrsAllResultByStid = new List<WeaRainStatistics>();
                    //每個站號進行統計
                    foreach (string item in allStid)
                    {
                        //取得測站名稱
                        BasStationData bsd = lBasStationData.FirstOrDefault(x => x.stid == item);

                        //取得某個站號雨量統計資料
                        List<WeaRainStatistics> dataByStids = wrs.Where(x => x.stid == item).ToList();

                        foreach (var dataByStid in dataByStids)
                        {
                            List<string> cols = new List<string>();
                            cols.Add(item);
                            cols.Add(bsd == null ? "" : bsd.stname);
                            cols.Add(dataByStid.year);
                            cols.Add(dataByStid.m01 == -99 ? "*" : dataByStid.m01.ToString());
                            cols.Add(dataByStid.m02 == -99 ? "*" : dataByStid.m02.ToString());
                            cols.Add(dataByStid.m03 == -99 ? "*" : dataByStid.m03.ToString());
                            cols.Add(dataByStid.m04 == -99 ? "*" : dataByStid.m04.ToString());
                            cols.Add(dataByStid.m05 == -99 ? "*" : dataByStid.m05.ToString());
                            cols.Add(dataByStid.m06 == -99 ? "*" : dataByStid.m06.ToString());
                            cols.Add(dataByStid.m07 == -99 ? "*" : dataByStid.m07.ToString());
                            cols.Add(dataByStid.m08 == -99 ? "*" : dataByStid.m08.ToString());
                            cols.Add(dataByStid.m09 == -99 ? "*" : dataByStid.m09.ToString());
                            cols.Add(dataByStid.m10 == -99 ? "*" : dataByStid.m10.ToString());
                            cols.Add(dataByStid.m11 == -99 ? "*" : dataByStid.m11.ToString());
                            cols.Add(dataByStid.m12 == -99 ? "*" : dataByStid.m12.ToString());

                            cols.Add(dataByStid.mavg.ToString());
                            cols.Add(dataByStid.yearsum == -99 ? "*" : dataByStid.yearsum.ToString());
                            cols.Add(dataByStid.max1.ToString());
                            cols.Add(dataByStid.max1date);
                            cols.Add(dataByStid.max2.ToString());
                            cols.Add(dataByStid.max2date);
                            cols.Add(dataByStid.max3.ToString());
                            cols.Add(dataByStid.max3date);
                            cols.Add(dataByStid.raindatecount.ToString());

                            datasResult.Add(cols.ToArray());
                        }
                    }

                    //準備Excel資料
                    List<string> headResult = new List<string>();
                    headResult.Add("測站編號");
                    headResult.Add("測站名稱");
                    headResult.Add("年份");
                    headResult.Add("1月");
                    headResult.Add("2月");
                    headResult.Add("3月");
                    headResult.Add("4月");
                    headResult.Add("5月");
                    headResult.Add("6月");
                    headResult.Add("7月");
                    headResult.Add("8月");
                    headResult.Add("9月");
                    headResult.Add("10月");
                    headResult.Add("11月");
                    headResult.Add("12月");
                    headResult.Add("月平均");
                    headResult.Add("年雨量");
                    headResult.Add("最大1日雨量");
                    headResult.Add("最大1日發生日");
                    headResult.Add("最大2日雨量");
                    headResult.Add("最大2日發生日");
                    headResult.Add("最大3日雨量");
                    headResult.Add("最大3日發生日");
                    headResult.Add("降雨日數");

                    //產生Excel

                    //產生檔案路徑
                    string sTempPathResult = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
                    //建立資料夾
                    Directory.CreateDirectory(sTempPathResult);

                    string sFileNameResult = "WeaRainStatisticsAllResult_" + Guid.NewGuid().ToString() + ".xlsx";
                    string sSaveFilePathResult = Path.Combine(sTempPathResult, "WeaRainStatisticsAll_" + Guid.NewGuid().ToString() + ".xlsx");

                    DataExport deResult = new DataExport();
                    Boolean bSuccessResult = deResult.ExportListToExcel(sSaveFilePathResult, headResult, datasResult);

                    //下載Excel
                    if (bSuccessResult)
                    {
                        string filename = string.Format("WeaRainStatisticsAllResult_雨量站統計資料匯出各年度統計結果.xlsx", ExpAllType, ExpAllType);

                        //***** 下載檔案過大，使用特殊方法 *****
                        HttpContext context = System.Web.HttpContext.Current;
                        context.Response.TransmitFile(sSaveFilePathResult);
                        context.Response.ContentType = "application/vnd.ms-excel";
                        context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                        Response.End();
                    }

                    break;

            }
            

            return null;
        }

        public ActionResult DownRainArea(string stid)
        {
            try
            {
                //產生檔案路徑
                string sTempPath = Server.MapPath("~/doc/WeaRainArea/");
                //建立資料夾
                Directory.CreateDirectory(sTempPath);

                string sFileName = stid + ".xlsx";
                string sSaveFilePath = Path.Combine(sTempPath, sFileName);




                string filename = string.Format("WeaRainArea_{0}_{1}.xlsx", stid, DateTime.Now.ToString("yyyyMMddHH"));

                //ASP.NET 回應大型檔案的注意事項
                //http://blog.miniasp.com/post/2008/03/11/Caution-about-ASPNET-Response-a-Large-File.aspx


                //***** 下載檔案過大，使用特殊方法 *****
                HttpContext context = System.Web.HttpContext.Current;
                context.Response.TransmitFile(sSaveFilePath);
                context.Response.ContentType = "application/vnd.ms-excel";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.End();

            }
            catch (Exception)
            {
                   
            }
            

            return null;
        }

        public ActionResult DownRainAreaAll()
        {
            try
            {
                //產生檔案路徑
                string sTempPath = Server.MapPath("~/doc/WeaRainArea/");
                //建立資料夾
                Directory.CreateDirectory(sTempPath);

                string sFileName = "all.xlsx";
                string sSaveFilePath = Path.Combine(sTempPath, sFileName);

                //***** 下載檔案過大，使用特殊方法 *****
                HttpContext context = System.Web.HttpContext.Current;
                context.Response.TransmitFile(sSaveFilePath);
                //context.Response.ContentType = "application/zip";
                context.Response.ContentType = "application/vnd.ms-excel";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", sFileName));
                Response.End();

            }
            catch (Exception)
            {

            }


            return null;
        }

        public ActionResult DownRainAreaOption(string TimeStart, string TimeEnd, string UnitType)
        {
            //M10Const.RainStationUnit.A1

            //建立Excel表頭
            List<string> head = new List<string>();
            head.Add("雨量站編號");
            head.Add("雨量站名稱");
            head.Add("場次序號");
            head.Add("開始降雨時間");
            head.Add("結束降雨時間");
            head.Add("降雨延時");
            head.Add("總降雨量");
            head.Add("最大時雨量發生時間");
            head.Add("最大時雨量");
            head.Add("最大3時累積雨量");
            head.Add("最大6時累積雨量");
            head.Add("最大12時累積雨量");
            head.Add("最大24時累積雨量");
            head.Add("最大48時累積雨量");
            head.Add("七天前期雨量(0.6)");
            head.Add("七天前期雨量(0.7)");
            head.Add("七天前期雨量(0.8)");
            head.Add("尖零_尖峰");
            head.Add("Rt(0.6)");
            head.Add("Rt(0.7)");
            head.Add("Rt(0.8)");
            head.Add("時雨量");

            //取得測站名稱取得所有雨量站基本資料
            string ssql = @" select * from BasStationData ";
            List<BasStationData> lBasStationData = dbDapper.Query<BasStationData>(ssql);

            //取得條件雨場分割資料
            ssql = @"";
            if (UnitType == "") //全部
            {
                ssql = @" 
                select * from WeaRainArea where TimeStart between '{0}010100' and '{1}123124'
                order by stid,TimeStart
                ";
                ssql = string.Format(ssql, TimeStart, TimeEnd);
            }
            else //依照不同雨量站單位
            {
                ssql = @" 
                select * from WeaRainArea where unittype = '{2}' and TimeStart between '{0}010100' and '{1}123124'
                order by stid,TimeStart
                ";
                ssql = string.Format(ssql, TimeStart, TimeEnd, UnitType);
            }
            List<WeaRainArea> WeaRainAreaList = dbDapper.Query<WeaRainArea>(ssql);

            DateTime dttest = DateTime.Now;
            int iIndex = 0;
            List<string[]> datas = new List<string[]>();
            foreach (WeaRainArea StidItem in WeaRainAreaList)
            {
                iIndex++;
                //雨量站
                string sStid = StidItem.stid;

                int iTimeStart = Convert.ToInt32(StidItem.TimeStart);
                int iTimeEnd = Convert.ToInt32(StidItem.TimeEnd);


                //雨量站(1-24)時間格式與C#(0-23)不同
                StidItem.TimeStart = string.Format("{0}/{1}/{2} {3}:00"
                    , StidItem.TimeStart.Substring(0, 4)
                    , StidItem.TimeStart.Substring(4, 2)
                    , StidItem.TimeStart.Substring(6, 2)
                    , StidItem.TimeStart.Substring(8, 2)
                    );
                StidItem.TimeEnd = string.Format("{0}/{1}/{2} {3}:00"
                , StidItem.TimeEnd.Substring(0, 4)
                    , StidItem.TimeEnd.Substring(4, 2)
                    , StidItem.TimeEnd.Substring(6, 2)
                    , StidItem.TimeEnd.Substring(8, 2)
                    );
                StidItem.MaxRainTime = string.Format("{0}/{1}/{2} {3}:00"
                    , StidItem.MaxRainTime.Substring(0, 4)
                    , StidItem.MaxRainTime.Substring(4, 2)
                    , StidItem.MaxRainTime.Substring(6, 2)
                    , StidItem.MaxRainTime.Substring(8, 2)
                    );

                //取得測站名稱
                BasStationData bsd = lBasStationData.FirstOrDefault(x => x.stid == StidItem.stid);

                List<string> cols = new List<string>();
                cols.Add(StidItem.stid);
                cols.Add(bsd == null ? "" : bsd.stname);
                cols.Add(StidItem.RainAreaSeq.ToString().PadLeft(4, '0'));
                cols.Add(StidItem.TimeStart);
                cols.Add(StidItem.TimeEnd);
                cols.Add(StidItem.RainHour.ToString());
                cols.Add(StidItem.TotalRain.ToString());
                cols.Add(StidItem.MaxRainTime);
                cols.Add(StidItem.MaxRain.ToString());
                cols.Add(StidItem.Max3Sum.ToString());
                cols.Add(StidItem.Max6Sum.ToString());
                cols.Add(StidItem.Max12Sum.ToString());
                cols.Add(StidItem.Max24Sum.ToString());
                cols.Add(StidItem.Max48Sum.ToString());
                cols.Add(StidItem.Pre7DayRain6.ToString());
                cols.Add(StidItem.Pre7DayRain7.ToString());
                cols.Add(StidItem.Pre7DayRain8.ToString());
                cols.Add(StidItem.CumRain.ToString());
                cols.Add(StidItem.RT6.ToString());
                cols.Add(StidItem.RT7.ToString());
                cols.Add(StidItem.RT8.ToString());

                //加入時雨量資料
                if (StidItem.RainHourList != null)
                {
                    List<string> RainHourList = StidItem.RainHourList.Split('|').ToList<string>();
                    foreach (string subItem in RainHourList)
                    {
                        cols.Add(subItem);
                    }
                }
                

                datas.Add(cols.ToArray());

            }

            //產生檔案路徑
            string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
            //建立資料夾
            Directory.CreateDirectory(sTempPath);

            string sSaveFilePath = Path.Combine(sTempPath, "WeaRainAreaOption_" + Guid.NewGuid().ToString() + ".xlsx");

            DataExport de = new DataExport();            
            Boolean bSuccess = de.ExportListToExcel(sSaveFilePath, head, datas);

            if (bSuccess)
            {
                string UnitTypeName = "全部";

                if (UnitType == "A1") UnitTypeName = M10Const.RainStationUnit.A1;
                if (UnitType == "A2") UnitTypeName = M10Const.RainStationUnit.A2;
                if (UnitType == "A3") UnitTypeName = M10Const.RainStationUnit.A3;
                if (UnitType == "A4") UnitTypeName = M10Const.RainStationUnit.A4;
                

                string filename = string.Format("WeaRainAreaOption_{0}_{1}_{2}.xlsx", TimeStart, TimeEnd, UnitTypeName);

                //***** 下載檔案過大，使用特殊方法 *****
                HttpContext context = System.Web.HttpContext.Current;
                context.Response.TransmitFile(sSaveFilePath);
                context.Response.ContentType = "application/vnd.ms-excel";
                context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
                Response.End();
            }


            return null;
        }

    }
}