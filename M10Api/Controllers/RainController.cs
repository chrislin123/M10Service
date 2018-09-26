using System;
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



            //if (Stations.Count != 0)
            //{
            //  items.Insert(0, new SelectListItem() { Text = "全部", Value = "全部" });
            //}

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

        public ActionResult QueryRTI(string type)
        {
            ViewBag.type = "0";
            if (string.IsNullOrEmpty(type) == false) ViewBag.type = type;

            ssql = " select * from RtiDetail where delaytime = @type  order by station ";

            List<dynamic> data = dbDapper.Query(ssql, new { type = ViewBag.type });

            //資料筆數
            ViewBag.count = data.Count;
            ViewData["RtiDetail"] = data;

            //
            return View();
        }

        public ActionResult QueryRTI3(string type)
        {
            ViewBag.type = "0";
            if (string.IsNullOrEmpty(type) == false) ViewBag.type = type;

            ssql = " select * from Rti3Detail where delaytime = @type order by station ";
            List<dynamic> data = dbDapper.Query(ssql, new { type = ViewBag.type });

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


            ssql = @"   select * from WeaRainStatistics where stid = @stid ";


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
            DataTable dt = new DataTable();


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

        public ActionResult DownRainArea(string stid)
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
            DataTable dt = new DataTable();


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

    }
}