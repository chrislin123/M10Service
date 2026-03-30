using CL.Data;
using Dapper;
using HtmlAgilityPack;
using M10.lib;
using M10.lib.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static M10.lib.M10Const;


namespace M10Tools
{
    public partial class MainForm : BaseForm
    {
        public MainForm()
        {
            InitializeComponent();

            //載入BaseForm資料
            base.InitForm();


            //StockRuntime sr = Stockhelper.getStockRealtimeYahooApi("9999");


        }

        private void btnImpErrLRTI_Click(object sender, EventArgs e)
        {
            DataTable dtresult = new DataTable();
            dtresult.Columns.Add("STID");
            dtresult.Columns.Add("ELRTI");


            string sFilePath = @"C:\ErrLRTI.csv";

            List<string> ll = new List<string>();


            Dictionary<string, string> dd = new Dictionary<string, string>();

            using (StreamReader SR = new StreamReader(sFilePath, System.Text.Encoding.Default))
            {
                string Line;
                while ((Line = SR.ReadLine()) != null)
                {

                    string[] sSplit = Line.Split('|');

                    if (dd.ContainsKey(sSplit[3]) == false)
                    {
                        dd.Add(sSplit[3], sSplit[5]);
                    }

                    ll.Add(Line);
                }
            }

            foreach (KeyValuePair<string, string> item in dd)
            {
                StationErrLRTI Temp = new StationErrLRTI();
                Temp.STID = item.Key;
                Temp.ELRTI = item.Value;

                dbDapper.Insert(Temp);
            }

        }

        private void btnKML_Click(object sender, EventArgs e)
        {
            M10kml M10kmlFrom = new M10kml();
            M10kmlFrom.ShowDialog();
        }

        private void btnStockTrans_Click(object sender, EventArgs e)
        {

            try
            {
                List<string> TypeList = new List<string>();
                TypeList.Add("tse");
                TypeList.Add("otc");

                foreach (string sType in TypeList)
                {
                    string surl = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=2";

                    if (sType == "tse") surl = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=2";
                    if (sType == "otc") surl = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=4";


                    HtmlWeb webClient = new HtmlWeb();
                    //網頁特殊編碼
                    webClient.OverrideEncoding = Encoding.GetEncoding(950);

                    // 載入網頁資料 
                    HtmlAgilityPack.HtmlDocument doc = webClient.Load(surl);

                    // 裝載查詢結果 
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table[2]/tr");

                    int idx = 1;
                    foreach (HtmlNode node in nodes)
                    {
                        string sCode = "";
                        string sName = "";
                        string sStatus = "";
                        HtmlNodeCollection tdnodes = node.SelectNodes("td");

                        if (tdnodes.Count > 0)
                        {
                            HtmlNode tdnode = tdnodes[0];
                            string[] StockInfoSplit = tdnode.InnerText.Split('　');

                            if (StockInfoSplit.Length != 2) continue;

                            sCode = StockInfoSplit[0];
                            sName = StockInfoSplit[1];

                            //判斷代碼存在則更新，不存在新增
                            ssql = " select * from stockinfo where stockcode = '{0}' ";
                            StockInfo StockInfoItem = dbDapper.QuerySingleOrDefault<StockInfo>(string.Format(ssql, sCode));

                            if (StockInfoItem == null) //不存在新增
                            {
                                sStatus = "新增";
                                StockInfoItem = new StockInfo();
                                StockInfoItem.stockcode = sCode;
                                StockInfoItem.stockname = sName;
                                StockInfoItem.type = sType;
                                dbDapper.Insert(StockInfoItem);


                            }
                            else
                            {
                                sStatus = "比對";
                                //有異動則更新資料
                                if (StockInfoItem.type != sType)
                                {
                                    sStatus = "更新";
                                    StockInfoItem.type = sType;
                                    dbDapper.Update(StockInfoItem);
                                }


                            }
                        }



                        toolStripStatusLabel1.Text =
                          string.Format("{0}進度({1}/{2})=>[{3}]{4} 狀態：{5}", sType, idx, nodes.Count, sCode, sName, sStatus);
                        //this.Refresh();
                        Application.DoEvents();

                        idx++;

                    }

                }

            }
            catch (Exception)
            {

                throw;
            }

            MessageBox.Show("Finish");


        }

        private void btnBuildFolder_Click(object sender, EventArgs e)
        {
            DataTable dtresult = new DataTable();
            dtresult.Columns.Add("country");
            dtresult.Columns.Add("week");
            dtresult.Columns.Add("car");
            dtresult.Columns.Add("time");

            string sFilepath = @"d:\list.csv";

            //string sType = "";

            string sRootFolder = @"D:\google電影";


            List<string> ll = new List<string>();

            using (StreamReader SR = new StreamReader(sFilepath))
            {
                string Line;
                while ((Line = SR.ReadLine()) != null)
                {

                    Line = Line.Replace(" ", "");

                    string[] aCol = Line.Split(',');
                    if (aCol[1] == "") continue;



                    string sTargetPath = Path.Combine(sRootFolder, aCol[0]);
                    sTargetPath = Path.Combine(sTargetPath, aCol[1]);

                    Directory.CreateDirectory(sTargetPath);
                }
            }



            MessageBox.Show("Test");
        }


        double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime st = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - st;
            return Math.Floor(diff.TotalMilliseconds);
        }
        private void button1_Click(object sender, EventArgs e)
        {

            DataStaticLog dsl = new DataStaticLog();

            ssql = @"
                select * from datastaticlog where type = 'test' and key1 = '1'
                ";

            dsl = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);

            if (dsl == null)
            {
                dsl = new DataStaticLog();
                //dsl.no = 1;
                dsl.type = "test";
                dsl.key1 = "1";
                dsl.key2 = "2";
                dsl.key3 = "3";
                dsl.logtime = DateTime.Now;
                dbDapper.Insert<DataStaticLog>(dsl);
            }
            else
            {
                dsl.logtime = DateTime.Now;
                dbDapper.Update<DataStaticLog>(dsl);
            }


            return;




            StockRuntime srtest = Stockhelper.getStockRealtimeYahooApi("9999");

            return;

            logger.Fatal("我是致命錯誤:Fatal");

            return;


            try
            {

                string a = "";

                int.Parse(a);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "stock after:");
                System.Threading.Thread.Sleep(10000);
            }

            var client = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)

            {

                EnableSsl = true,

                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,

                UseDefaultCredentials = false,

                Credentials = new NetworkCredential("eswcrc.ncku@gmail.com", "ncku201102eswcrc")

            };



            client.Send("eswcrc.ncku@gmail.com", "chris.lin.tw123@gmail.com", "test", "testbody");


            return;

            //System.Threading.Thread.Sleep(100000);

            return;

            string Url = "http://{0}/stock/api/getStockInfo.jsp?ex_ch=tse_t00.tw%7cotc_o00.tw%7ctse_FRMSA.tw&json=1&delay=0&_={1}";
            double dd = ConvertToUnixTimestamp(DateTime.Now.AddMilliseconds(60000));


            string MISTWSE = "mis.twse.com.tw";
            string host = "61.57.47.179";
            //string USER_AGENT = "curl/7.32.0";

            Url = string.Format(Url, host, dd);





            string url2 = "http://{0}/stock/api/getStockInfo.jsp?ex_ch=otc_4506.tw&json=1&delay=0&_={1}";
            double dd2 = ConvertToUnixTimestamp(DateTime.Now.AddMilliseconds(60000));

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;


            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime utcNow = DateTime.UtcNow;

            TimeSpan elapsedTime = utcNow - unixEpoch;
            //elapsedTime.TotalMilliseconds.
            Int32 millis = (Int32)elapsedTime.TotalMilliseconds;

            url2 = string.Format(url2, host, dd2);

            HttpWebClient hwc = new HttpWebClient();
            hwc.Encoding = Encoding.UTF8;
            hwc.Headers.Add("Host", host);
            string url3 = "http://{0}/stock/api/getStock.jsp?ch=1475.tw&json=0";
            url3 = string.Format(url3, host);


            string url4 = "http://{0}/stock/fibest.jsp?stock=4506";
            url4 = string.Format(url4, host);
            hwc.Headers.Add("Host", host);
            hwc.Headers.Add("Referer", url4);
            //string bbbb = "";

            HttpClient hc = new HttpClient();

            hc.DefaultRequestHeaders.Host = MISTWSE;
            hc.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            hc.DefaultRequestHeaders.Add("Accept-Language", "zh-TW,zh;q=0.8,en-US;q=0.5,en;q=0.3");
            hc.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            hc.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            hc.DefaultRequestHeaders.Add("Referer", string.Format("http://{0}/stock/fibest.jsp?lang=zh_tw", MISTWSE));
            hc.DefaultRequestHeaders.Add("Cookie", "1");

            CookieContainer cc = new CookieContainer();

            string Cookiesstr = string.Empty;
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url4);
            myRequest.CookieContainer = cc;
            myRequest.AllowAutoRedirect = false;
            myRequest.Host = host;
            myRequest.Method = "GET";
            string sCookie = "";
            WebResponse myResponse = myRequest.GetResponse();


            //myResponse.Cookies = 
            CookieCollection cook = myRequest.CookieContainer.GetCookies(myRequest.RequestUri);
            string strcrook = myRequest.CookieContainer.GetCookieHeader(myRequest.RequestUri);
            Cookiesstr = strcrook;

            using (StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.Default))
            {
                string result = sr.ReadToEnd();

                sCookie = myResponse.Headers["Set-Cookie"];
                sCookie = sCookie.Substring(0, sCookie.IndexOf(';'));

                myResponse.Close();
            }


            myRequest = (HttpWebRequest)WebRequest.Create(url2);
            myRequest.Method = "GET";
            myRequest.KeepAlive = true;
            myRequest.Headers.Add("Cookie:" + Cookiesstr);
            myRequest.CookieContainer = cc;
            myRequest.AllowAutoRedirect = false;
            myRequest.Host = host;
            myRequest.Referer = url4;
            WebResponse myResponse2 = myRequest.GetResponse();
            using (StreamReader sr = new StreamReader(myResponse2.GetResponseStream(), System.Text.Encoding.Default))
            {
                string result = sr.ReadToEnd();
                myResponse2.Close();
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            string surl = "https://tw.stock.yahoo.com/q/q?s=1475";

            HtmlWeb webClient = new HtmlWeb();
            //網頁特殊編碼
            webClient.OverrideEncoding = Encoding.GetEncoding(950);
            //webClient.OverrideEncoding = Encoding;

            // 載入網頁資料 
            HtmlAgilityPack.HtmlDocument doc = webClient.Load(surl);
            //*[@id="yui_3_5_1_13_1503571196918_6"]/table[2]/tbody/tr/td/table
            //*[@id="yui_3_5_1_13_1503571196918_6"]/table[2]/tbody/tr/td/table/tbody/tr[2]
            // 裝載查詢結果 
            HtmlNode nnn = doc.DocumentNode.SelectSingleNode("//table[2]/tr/td/table/tr[2]");
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table[2]/tr/td/table/tr[2]");

            HtmlNodeCollection tdnodes = nnn.SelectNodes("td");

            //HtmlNodeCollection nodes1 = doc.DocumentNode.SelectNodes("//table[2]/tbody/tr/td/table/tbody/tr[2]");

            var temp = new { a = "", b = "", c = "" };
            int idx = 0;
            foreach (HtmlNode node in tdnodes)
            {
                //string sCode = "";


                //成交價
                if (idx == 2)
                {
                    //temp.a = node.InnerText;
                }

                //漲跌
                if (idx == 4)
                {

                }

                //昨收
                if (idx == 7)
                {

                }




                idx++;



                //if (tdnodes.Count > 0)
                //{
                //  HtmlNode tdnode = tdnodes[0];
                //  string[] StockInfoSplit = tdnode.InnerText.Split('　');

                //  if (StockInfoSplit.Length != 2) continue;

                //  sCode = StockInfoSplit[0];
                //  sName = StockInfoSplit[1];

                //  //判斷代碼存在則更新，不存在新增
                //  ssql = " select * from stockinfo where stockcode = '{0}' ";
                //  StockInfo StockInfoItem = dbDapper.QuerySingleOrDefault<StockInfo>(string.Format(ssql, sCode));

                //  if (StockInfoItem == null) //不存在新增
                //  {
                //    sStatus = "新增";
                //    StockInfoItem = new StockInfo();
                //    StockInfoItem.stockcode = sCode;
                //    StockInfoItem.stockname = sName;
                //    StockInfoItem.type = sType;
                //    dbDapper.Insert(StockInfoItem);


                //  }
                //  else
                //  {
                //    sStatus = "比對";
                //    //有異動則更新資料
                //    if (StockInfoItem.type != sType)
                //    {
                //      sStatus = "更新";
                //      StockInfoItem.type = sType;
                //      dbDapper.Update(StockInfoItem);
                //    }


                //  }
                //}




            }
        }


        public class HttpWebClient : WebClient
        {
            // Cookie 容器
            private CookieContainer cookieContainer;

            public HttpWebClient()
            {
                this.cookieContainer = new CookieContainer();
            }

            public HttpWebClient(CookieContainer cc)
            {
                this.cookieContainer = cc;
            }

            /// <summary>
            /// Cookie 容器
            /// </summary>
            public CookieContainer MyCookies
            {
                get { return this.cookieContainer; }
                set { this.cookieContainer = value; }
            }

            /// <summary>
            /// 覆寫web request方法，讓webclient能保持session
            /// </summary>
            /// <param name="address"></param>
            /// <returns></returns>
            protected override WebRequest GetWebRequest(Uri address)
            {
                //throw new Exception(); 
                WebRequest request;
                request = base.GetWebRequest(address);
                //判斷是不是HttpWebRequest.只有HttpWebRequest才有此属性 
                if (request is HttpWebRequest)
                {
                    HttpWebRequest httpRequest = request as HttpWebRequest;
                    httpRequest.CookieContainer = this.cookieContainer;
                }
                return request;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //開始日期
            DateTime dt = new DateTime(2017, 9, 8);
            //結束日期
            DateTime dtEnd = new DateTime(2017, 9, 12);

            for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
            {
                string sLineTrans = "";
                try
                {
                    toolStripStatusLabel1.Text = string.Format("{0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    if (Stockhelper.GetStockThreeTradeTse(LoopDatetime) == false)
                    {
                        System.Threading.Thread.Sleep(3000);
                        continue;
                    }

                    toolStripStatusLabel1.Text = string.Format("{0}-{1}", M10Const.StockType.tse, "完成");
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "stock after:" + sLineTrans);
                    System.Threading.Thread.Sleep(10000);
                }
            }

            toolStripStatusLabel1.Text = "完成";
        }

        private void button4_Click(object sender, EventArgs e)
        {

            //開始日期
            DateTime dt = new DateTime(2017, 9, 8);
            //結束日期
            DateTime dtEnd = new DateTime(2017, 9, 12);

            for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
            {
                string sLineTrans = "";
                try
                {
                    toolStripStatusLabel1.Text = string.Format("{0}-{1}", M10Const.StockType.otc, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    if (Stockhelper.GetStockThreeTradeOtc(LoopDatetime) == false)
                    {
                        System.Threading.Thread.Sleep(3000);
                        continue;
                    }

                    System.Threading.Thread.Sleep(3000);

                    toolStripStatusLabel1.Text = string.Format("{0}-{1}", M10Const.StockType.otc, "完成");
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "stock after:" + sLineTrans);
                    System.Threading.Thread.Sleep(10000);
                }
            }
            toolStripStatusLabel1.Text = "完成";
        }


        /// <summary>
        /// StockAfter_Tse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            //開始日期
            DateTime dt = new DateTime(2022, 6, 9);
            //DateTime dt = new DateTime(2019, 8, 2);
            //結束日期
            DateTime dtEnd = new DateTime(2022, 6, 9);

            ////取得資料庫最後一天
            //ssql = " select  distinct stockdate  from stockafter where stocktype = '{0}' order by stockdate asc ";

            //var vDbDate = dbDapper.ExecuteScale(string.Format(ssql, M10Const.StockType.tse));
            //if (vDbDate != null)
            //{
            //  string sDbDate = vDbDate.ToString();
            //  dt = new DateTime(Convert.ToInt32(sDbDate.Substring(0, 4))
            //    , Convert.ToInt32(sDbDate.Substring(4, 2))
            //    , Convert.ToInt32(sDbDate.Substring(6, 2)));
            //}

            //for (DateTime date = checkBgn; date <= checkEnd; date = date.AddDays(1))
            for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
            {
                string sLineTrans = "";
                try
                {
                    toolStripStatusLabel1.Text = string.Format("盤後資料轉檔(TSE){0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    //判斷是否已經轉檔成功
                    ssql = @" select * from stocklog where logtype = '{0}' and logdate = '{1}' and logstatus = 'success' ";
                    ssql = string.Format(ssql, M10Const.StockLogType.StockAfterTse, Utils.getDateString(LoopDatetime, M10Const.DateStringType.ADT1));
                    StockLog sl = dbDapper.QuerySingleOrDefault<StockLog>(ssql);
                    if (sl != null)
                    {
                        //System.Threading.Thread.Sleep(3000);
                        continue;
                    }

                    if (Stockhelper.GetStockAfterTse(LoopDatetime) == false)
                    {
                        System.Threading.Thread.Sleep(15000);
                        continue;
                    }

                    System.Threading.Thread.Sleep(5000);

                    toolStripStatusLabel1.Text = string.Format("盤後資料轉檔(TSE){0}-{1}", M10Const.StockType.tse, "完成");
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "stock after:" + sLineTrans);
                    System.Threading.Thread.Sleep(60000);
                }
            }
            toolStripStatusLabel1.Text = "盤後資料轉檔(TSE)完成";
        }



        private void button6_Click(object sender, EventArgs e)
        {
            //開始日期
            DateTime dt = new DateTime(2007, 1, 2);
            //結束日期
            DateTime dtEnd = new DateTime(2019, 8, 2);

            //取得資料庫最後一天
            //ssql = " select  distinct stockdate  from stockafter where stocktype = '{0}' order by stockdate asc ";

            //var vDbDate = dbDapper.ExecuteScale(string.Format(ssql, M10Const.StockType.otc));
            //if (vDbDate != null)
            //{
            //  string sDbDate = vDbDate.ToString();
            //  dt = new DateTime(Convert.ToInt32(sDbDate.Substring(0, 4))
            //    , Convert.ToInt32(sDbDate.Substring(4, 2))
            //    , Convert.ToInt32(sDbDate.Substring(6, 2)));
            //}

            for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
            {

                string sLineTrans = "";
                try
                {
                    toolStripStatusLabel1.Text = string.Format("盤後資料轉檔(OTC){0}-{1}", M10Const.StockType.otc, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    //判斷是否已經轉檔成功
                    ssql = @" select * from stocklog where logtype = '{0}' and logdate = '{1}' and logstatus = 'success' ";
                    ssql = string.Format(ssql, M10Const.StockLogType.StockAfterOtc, Utils.getDateString(LoopDatetime, M10Const.DateStringType.ADT1));
                    StockLog sl = dbDapper.QuerySingleOrDefault<StockLog>(ssql);
                    if (sl != null)
                    {
                        //System.Threading.Thread.Sleep(3000);
                        continue;
                    }

                    if (Stockhelper.GetStockAfterOtc(LoopDatetime) == false)
                    {
                        System.Threading.Thread.Sleep(3000);
                        continue;
                    }

                    toolStripStatusLabel1.Text = string.Format("盤後資料轉檔(OTC){0}-{1}", M10Const.StockType.otc, "完成");
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "stock after:" + sLineTrans);
                    System.Threading.Thread.Sleep(10000);
                }
            }
            toolStripStatusLabel1.Text = "盤後資料轉檔(OTC)完成";
        }


        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                string stockcode = "3162";
                if (stockcode == "0000") stockcode = "%23001";

                string sUrl = "https://tw.quote.finance.yahoo.net/quote/q?type=tick&sym={0}";
                string sDate = DateTime.Now.ToString("yyyyMMdd");


                sUrl = string.Format(sUrl, stockcode);



                using (WebClient wc = StockHelper.getNewWebClient())
                {
                    //wc.Encoding = Encoding.GetEncoding(950);
                    string text = wc.DownloadString(sUrl);

                    text = text.Replace("null(", "");
                    text = text.Replace(");", "");


                    text = text.Insert(text.IndexOf(",\"143\":") + 7, "\"").Insert(text.IndexOf(",\"143\":") + 14, "\"");
                    JObject jobj = JObject.Parse(text);

                    StockRuntime sr = new StockRuntime();
                    //Price
                    sr.z = jobj["mem"]["125"].ToString();


                    ////昨收
                    sr.y = jobj["mem"]["129"].ToString();
                    //最高
                    sr.u = jobj["mem"]["130"].ToString();
                    //最低
                    sr.w = jobj["mem"]["131"].ToString();



                }









                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sUrl);
                req.Proxy = null;
                //改為寫入資料庫格式

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                //判斷http回應狀態(HttpStatusCode.OK=200)
                //if (resp.StatusCode != HttpStatusCode.OK)
                //{
                //  return sr;
                //}

                //using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.GetEncoding(950)))
                using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    string Line;
                    while ((Line = SR.ReadLine()) != null)
                    {
                        Line = Line.Replace("null(", "");
                        Line = Line.Replace(");", "");

                        //JObject jobj = JsonConvert.DeserializeObject(Line) as JObject;

                        Line = Line.Insert(Line.IndexOf(",\"143\":") + 7, "\"").Insert(Line.IndexOf(",\"143\":") + 14, "\"");
                        JObject jobj = JObject.Parse(Line);
                        //jobj["mem"]["125"];

                        //Price
                        //sr.z = jobj["mem"]["125"].ToString();


                        ////昨收
                        //sr.y = jobj["mem"]["129"].ToString();
                        //最高
                        //sr.u = jobj["mem"]["130"].ToString();
                        //最低
                        //sr.w = jobj["mem"]["131"].ToString();

                        //  if(StockInfo.z == StockInfo.u) mark="▲";
                        //  if(StockInfo.z == StockInfo.w) mark="▼";
                        //  if(ud>0) mark="△";
                        //  if(ud<0) mark="▽";
                        //var mark = "±";
                    }

                }


                //取得個股資訊
                //ssql = " select * from StockInfo where stockcode = '{0}' ";
                //StockInfo si = dbDapper.QuerySingleOrDefault<StockInfo>(string.Format(ssql, stockcode));
                //if (si != null)
                //{
                //  //個股名稱
                //  sr.n = si.stockname;
                //  //個股代碼
                //  sr.c = si.stockcode;
                //}
            }
            catch
            {
                //logger.Error(ex);
                //throw ex;
            }



            //return sr;

        }

        private void button8_Click(object sender, EventArgs e)
        {
            //預設今天資料
            string sRunDate = DateTime.Now.ToString("yyyyMMdd");

            //轉最新資料
            ssql = " select Max(stockdate) from stockafter where stockcode = '2330' ";
            object oMax = dbDapper.ExecuteScale(ssql);
            if (oMax != null)
            {
                sRunDate = Convert.ToString(Convert.ToInt32(oMax.ToString()));
            }


            sRunDate = "20180329";

            List<StockInfo> siList = new List<StockInfo>();
            ssql = " select * from stockinfo where status = 'Y' and LEN(stockcode) = 4 order by stockcode ";
            siList = dbDapper.Query<StockInfo>(ssql);

            int iListTotal = siList.Count();
            int idex = 0;

            foreach (StockInfo item in siList)
            {
                idex++;

                toolStripStatusLabel1.Text = string.Format("[{2}/{3}]{0}({1})"
                     , item.stockcode, sRunDate, idex.ToString(), iListTotal.ToString());
                Application.DoEvents();

                Stockhelper.GetHugeTurnover(item.stockcode, sRunDate);

            }

            toolStripStatusLabel1.Text += "轉檔完畢";
            Application.DoEvents();




        }


        static async Task Main()
        {
            string url = "https://isin.twse.com.tw/isin/C_public.jsp?strMode=2";


            //url = "https://ithelp.ithome.com.tw/questions/10209952";


            try
            {
                HttpClientHandler handler = new HttpClientHandler
                {
                    AllowAutoRedirect = true,  // 確保允許跳轉
                    UseCookies = true          // 啟用 Cookie
                };
                //HttpClient client = new HttpClient(handler);
                HttpClient client = new HttpClient();
                //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
                //client.DefaultRequestHeaders.Add("Accept-Language", "zh-TW,zh;q=0.9");
                //client.DefaultRequestHeaders.Add("Referer", "https://isin.twse.com.tw/isin/C_public.jsp?strMode=2");


                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {

                    byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                    string html1 = Encoding.GetEncoding("big5").GetString(bytes);  // 轉換 Big5 編碼

                    //string html = await response.Content.ReadAsStringAsync();
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html1);

                    // 找到所有的表格行
                    var rows = doc.DocumentNode.SelectNodes("//table[2]/tr");
                    if (rows == null)
                    {
                        Console.WriteLine("未找到資料表格");
                        return;
                    }

                    List<string[]> stockData = new List<string[]>();

                    foreach (var row in rows)
                    {
                        var cols = row.SelectNodes("td");
                        if (cols != null && cols.Count > 1)
                        {
                            string stockInfo = cols[0].InnerText.Trim();
                            string isinCode = cols[1].InnerText.Trim();

                            // 只處理有股票代碼的行
                            if (stockInfo.Contains(" "))
                            {
                                string[] parts = stockInfo.Split(' ');
                                string stockName = parts[0];  // 股票名稱
                                string stockId = parts[1];    // 股票代號

                                stockData.Add(new string[] { stockId, stockName, isinCode });
                            }
                        }
                    }

                    // 輸出結果
                    Console.WriteLine("股票代號\t股票名稱\tISIN Code");
                    foreach (var data in stockData)
                    {
                        Console.WriteLine($"{data[0]}\t{data[1]}\t{data[2]}");
                    }
                }
                else
                {
                    Console.WriteLine("❌ 取得資料失敗");
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {

            Main();

            return;
            toolStripStatusLabel1.Text = string.Format("StockInfor Trans 開始");
            Application.DoEvents();

            Stockhelper.GetStockInfo();
            //Stockhelper.GetStockInfoSub(M10Const.StockType.otc1);



            toolStripStatusLabel1.Text = string.Format("StockInfor Trans 完成");
            Application.DoEvents();

        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            try
            {
                //Excel檔案，先轉csv檔案
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "csv files (*.*)|*.csv";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string sFilePath = dialog.FileName;

                    using (StreamReader SR = new StreamReader(sFilePath, System.Text.Encoding.Default))
                    {
                        string Line;
                        while ((Line = SR.ReadLine()) != null)
                        {

                            string[] sSplit = Line.Split('|');

                            if (sSplit.Length < 3) continue;

                            Decimal iCheck = -1;

                            if (Decimal.TryParse(sSplit[1], out iCheck) == false)
                            {
                                continue;
                            }

                            LrtiBasic lbItem = new LrtiBasic();
                            lbItem.type = Utils.getDateString(DateTime.Now, M10Const.DateStringType.ADT1);
                            lbItem.stid = sSplit[0];
                            lbItem.elrti = sSplit[1];
                            lbItem.villageid = sSplit[2];
                            dbDapper.Insert(lbItem);

                            toolStripStatusLabel1.Text = string.Format(sSplit[0]);
                            Application.DoEvents();

                        }
                    }


                }

                toolStripStatusLabel1.Text = string.Format("完成");
                Application.DoEvents();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {



            while (true)
            {
                ssql = " select top 1 * from stockafter where priceyesterday is null ";
                if (dbDapper.QueryTotalCount(ssql) == 0)
                {
                    toolStripStatusLabel1.Text = string.Format("完成");
                    Application.DoEvents();

                    return;
                }


                ssql = "select top 1000 * from stockafter where priceyesterday is null  ";
                List<Stockafter> SaList = dbDapper.Query<Stockafter>(ssql);

                foreach (Stockafter LoopItem in SaList)
                {

                    toolStripStatusLabel1.Text = string.Format("[{0}]{1}", LoopItem.stockdate, LoopItem.stockcode);
                    Application.DoEvents();

                    Decimal dPriceYesterday = Stockhelper.CalcPriceYesterday(LoopItem.pricelast.ToString(), LoopItem.updown, LoopItem.pricediff);

                    LoopItem.priceyesterday = dPriceYesterday;

                    dbDapper.Update(LoopItem);
                }
            }





        }




        private void button10_Click(object sender, EventArgs e)
        {

            Stockhelper.GetStockBrokerBS();
        }




        private void button11_Click(object sender, EventArgs e)
        {

            //建立資料夾副本
            //Utils.CreateDirByCopy(@"G:\我的雲端硬碟\!MyRoot", @"d:\");

            string strPath = "C:\\Test.csv";
            string sPath = @"D:\HIS\OPDVGHTC\SourceCode\HISAssembly\Report\HIS.Report.ExmPrj";
            DirectoryInfo di = new DirectoryInfo(sPath);


            List<string> FileList = new List<string>();
            foreach (FileInfo item in di.GetFiles("*.cs"))
            {
                FileList.Add(item.Name.Replace("HIS.DataObject.", "").Replace(".cs", ""));
            }

            using (FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default))
                {

                    string sss = string.Join("\n", FileList.ToArray());

                    sw.Write(sss);

                }
            }




        }

        private void StockCodeTextBox_Enter(object sender, EventArgs e)
        {
            button11_Click(sender, new EventArgs());
        }

        private void StockCodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                button11_Click(sender, new EventArgs());
            }
        }

        private void StockAfterRushTseButton_Click(object sender, EventArgs e)
        {
            string sPath = @"d:\Temp\StockAfterRush\";

            Directory.CreateDirectory(sPath);








            //開始日期
            DateTime dt = new DateTime(2014, 1, 6);

            //結束日期
            DateTime dtEnd = DateTime.Now.AddDays(-1);

            //Stockhelper.GetStockAfterRushTse(dt);
            //Stockhelper.GetStockAfterRushOtc(dt);


            for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
            {
                string sLineTrans = "";
                try
                {
                    toolStripStatusLabel1.Text = string.Format("當沖資料轉檔(TSE){0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    string sDate = Utils.getDateString(LoopDatetime, M10Const.DateStringType.ADT1);
                    string sUrl = string.Format(M10Const.StockAfterRushTse, sDate);

                    string sFileName = @"StockAfterRush_TSE_{0}.csv";
                    sFileName = string.Format(sFileName, LoopDatetime.ToString("yyyyMMdd"));
                    sFileName = sPath + sFileName;

                    //判斷檔案是否存在
                    if (File.Exists(sFileName) == true)
                    {
                        continue;
                    }

                    using (WebClient wc = StockHelper.getNewWebClient())
                    {
                        wc.Encoding = Encoding.GetEncoding(950);

                        wc.DownloadFile(sUrl, sFileName);
                    }

                    toolStripStatusLabel1.Text = string.Format("當沖資料轉檔(TSE){0}-{1}", LoopDatetime.ToString("yyyyMMdd"), "完成");
                    Application.DoEvents();

                    toolStripStatusLabel1.Text = string.Format("當沖資料轉檔(OTC){0}-{1}", M10Const.StockType.otc, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    sDate = Utils.getDateString(LoopDatetime, M10Const.DateStringType.ChineseT2);
                    sUrl = string.Format(M10Const.StockAfterRushOtc, sDate);

                    sFileName = @"StockAfterRush_OTC_{0}.csv";
                    sFileName = string.Format(sFileName, LoopDatetime.ToString("yyyyMMdd"));
                    sFileName = sPath + sFileName;

                    //判斷檔案是否存在
                    if (File.Exists(sFileName) == true)
                    {
                        continue;
                    }

                    using (WebClient wc = StockHelper.getNewWebClient())
                    {
                        wc.Encoding = Encoding.GetEncoding(950);

                        wc.DownloadFile(sUrl, sFileName);
                    }

                    toolStripStatusLabel1.Text = string.Format("當沖資料轉檔(OTC){0}-{1}", LoopDatetime.ToString("yyyyMMdd"), "完成");
                    Application.DoEvents();



                    System.Threading.Thread.Sleep(15000);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "當沖資料轉檔:" + sLineTrans);
                    System.Threading.Thread.Sleep(60000);
                }
            }
            toolStripStatusLabel1.Text = "當沖資料轉檔(ALL)完成";






            //return;


            ////for (DateTime date = checkBgn; date <= checkEnd; date = date.AddDays(1))
            //for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
            //{
            //    string sLineTrans = "";
            //    try
            //    {
            //        toolStripStatusLabel1.Text = string.Format("{0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
            //        Application.DoEvents();

            //        if (Stockhelper.GetStockAfterTse(LoopDatetime) == false)
            //        {
            //            System.Threading.Thread.Sleep(3000);
            //            continue;
            //        }

            //        toolStripStatusLabel1.Text = string.Format("{0}-{1}", M10Const.StockType.tse, "完成");
            //        Application.DoEvents();
            //    }
            //    catch (Exception ex)
            //    {
            //        logger.Error(ex, "stock after:" + sLineTrans);
            //        System.Threading.Thread.Sleep(10000);
            //    }
            //}
            //toolStripStatusLabel1.Text = "完成";
        }




        private void ShowStatus(string sMsg)
        {
            toolStripStatusLabel1.Text = sMsg;
            Application.DoEvents();
        }

        /// <summary>
        /// (1)氣象署匯入雨量資料(自動站auto+氣象站cwb)
        ///    2026 新增(農業站agr)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImpWeatherDataold_Click(object sender, EventArgs e)
        {
            //1. 自動站auto站跟氣象站cwb跟農業站agr都放到"程式根目錄\Weather"目錄下，開啟轉檔

            //時間格式已經調整為00-23，但是auto站跟氣象站cwb都是1-24，所以轉檔要注意
            //auto站(全年原始)與氣象站cwb

            string sConstDirectoryPath = Directory.GetCurrentDirectory() + @"\Weather";
            Directory.CreateDirectory(sConstDirectoryPath);

            //auto站跟氣象站cwb都放到"程式根目錄\Weather"目錄下，開啟轉檔
            FileInfo[] fiList = new DirectoryInfo(sConstDirectoryPath).GetFiles("*.txt", SearchOption.TopDirectoryOnly);

            //try
            //{
            foreach (FileInfo item in fiList)
            {
                ShowStatus(item.FullName);
                string readText = File.ReadAllText(item.FullName, Encoding.Default);


                List<string> StringList = readText.Split(new string[] { "\n" }, StringSplitOptions.None).ToList<string>();

                for (int i = 0; i < StringList.Count - 1; i++)
                {
                    StringList[i] = StringList[i].Replace("\r", "");
                }

                DataTable dt = new DataTable();

                string sCols = StringList.Where(x => x.Contains("# ")).ToList<string>()[0];
                List<string> ColsList = sCols.Split(' ').Where(x => x != "" && x != "#").ToList<string>();

                foreach (string ColsItem in ColsList)
                {
                    dt.Columns.Add(ColsItem);
                }

                //選擇非必要資料
                List<string> temp = StringList.Where(x => x != "").Where(y => y.Substring(0, 1) != "*" && y.Substring(0, 1) != "#").ToList<string>();


                foreach (string RowItem in temp)
                {
                    List<string> CellList = RowItem.Split(' ').Where(x => x != "" && x != "#").ToList<string>();

                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < CellList.Count; i++)
                    {
                        dr[i] = CellList[i];
                    }
                    dt.Rows.Add(dr);

                }

                int idx = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    Decimal tt = 0;
                    ShowStatus(string.Format("[{0}/{1}]{2}", idx, dt.Rows.Count, item.FullName));

                    WeaRainData WeaRainDataTemp = new WeaRainData();
                    WeaRainDataTemp.STID = dr["stno"].ToString();
                    // 20230514 調整Auto站與Cwb站時間格式1-24轉換為0-23
                    WeaRainDataTemp.time = Convert.ToString(Convert.ToInt32(dr["yyyymmddhh"].ToString()) - 1);
                    // 20240504 時間格式來源為0-23
                    //WeaRainDataTemp.time = dr["yyyymmddhh"].ToString();

                    // 20240504 新增檢核時間格式如果來源為0-23，則顯示錯誤
                    if (WeaRainDataTemp.time.Contains("99") == true)
                    {
                        throw new Exception(string.Format("{0}-{1}==時間格式異常", WeaRainDataTemp.STID, dr["yyyymmddhh"].ToString()));
                    }

                    // 20240504 判斷來源資料是否為數值，否則預設為0
                    if (Decimal.TryParse(dr["PP01"].ToString(), out tt))
                    {
                        WeaRainDataTemp.PP01 = Convert.ToDecimal(dr["PP01"].ToString()) < 0 ? 0 : Convert.ToDecimal(dr["PP01"].ToString());
                        WeaRainDataTemp.PP01old = Convert.ToDecimal(dr["PP01"].ToString());
                    }
                    else
                    {
                        WeaRainDataTemp.PP01 = 0;
                        WeaRainDataTemp.PP01old = 0;
                    }


                    ShowStatus(string.Format("[{0}/{1}]{3}-{4}-{2}", idx, dt.Rows.Count, item.FullName, WeaRainDataTemp.STID, WeaRainDataTemp.time));

                    ssql = @" select * from WeaRainData where stid = '{0}' and time = '{1}' ";
                    ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
                    WeaRainData dsl_new = dbDapper.QuerySingleOrDefault<WeaRainData>(ssql);

                    //沒有轉檔紀錄
                    if (dsl_new == null)
                    {
                        //如果沒有轉檔紀錄，則新增一筆執行中
                        //寫入統計LOG
                        dsl_new = new WeaRainData();
                        dsl_new.STID = WeaRainDataTemp.STID;
                        dsl_new.time = WeaRainDataTemp.time;
                        dsl_new.PP01 = WeaRainDataTemp.PP01;
                        dsl_new.PP01old = WeaRainDataTemp.PP01old;

                        dbDapper.Insert(dsl_new);
                    }
                    else//已經有轉檔紀錄
                    {
                        //更新
                        dsl_new.PP01 = WeaRainDataTemp.PP01;
                        dsl_new.PP01old = WeaRainDataTemp.PP01old;
                        dbDapper.Update(dsl_new);
                    }


                    //20250218 同步寫入WeaRainDataHis，避免資料同步寫入問題
                    ssql = @" select * from WeaRainDataHis where stid = '{0}' and time = '{1}' ";
                    ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
                    //20260327 stid,time，已經新增unique Key，所以不會重複，這段註解
                    ////20250218 發生重複的問題，所以判斷是否有重複資料，如果已經有重複資料則刪除
                    //if (dbDapper.QueryTotalCount(ssql) > 1)
                    //{
                    //    dbDapper.Execute(string.Format(" delete WeaRainDataHis where Stid = '{0}' and Time = '{1}' "
                    //        , WeaRainDataTemp.STID, WeaRainDataTemp.time));
                    //}

                    WeaRainDataHis WeaRainDataHis_new = dbDapper.QuerySingleOrDefault<WeaRainDataHis>(ssql);
                    //沒有轉檔紀錄
                    if (WeaRainDataHis_new == null)
                    {
                        //如果沒有轉檔紀錄，則新增一筆執行中
                        //寫入統計LOG
                        WeaRainDataHis_new = new WeaRainDataHis();
                        WeaRainDataHis_new.Stid = WeaRainDataTemp.STID;
                        WeaRainDataHis_new.Time = WeaRainDataTemp.time;
                        WeaRainDataHis_new.Hour = WeaRainDataTemp.PP01;
                        WeaRainDataHis_new.TransYN = "N";
                        dbDapper.Insert(WeaRainDataHis_new);
                    }
                    else//已經有轉檔紀錄
                    {
                        //更新
                        WeaRainDataHis_new.Hour = WeaRainDataTemp.PP01;
                        dbDapper.Update(WeaRainDataHis_new);
                    }


                    Application.DoEvents();
                    idx++;
                }

                Directory.CreateDirectory(Path.Combine(item.DirectoryName, "OK"));
                string sNewFullName = Path.Combine(item.DirectoryName, "OK", item.Name);

                item.MoveTo(sNewFullName);
                Application.DoEvents();
            }

        }

        private async void btnImpWeatherData_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            var progress = new Progress<string>(msg => { ShowStatus(msg); });

            try
            {
                await Task.Run(() => ProcessWeatherDataBulk(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"轉檔發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
            }
        }

        private void ProcessWeatherDataBulk(IProgress<string> progress)
        {
            string sConstDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Weather");
            Directory.CreateDirectory(sConstDirectoryPath);

            FileInfo[] fiList = new DirectoryInfo(sConstDirectoryPath).GetFiles("*.txt", SearchOption.TopDirectoryOnly);

            // 💡 注意：這邊需要抓取你原本 dbDapper 底層的連線字串。
            // 如果 dbDapper 沒有提供，請自行替換成你的 SQL Server 連線字串。
            //string connString = dbDapper.Connection.ConnectionString; 
            string connString = dbDapper.ConnStr;

            int idx = 0;
            foreach (FileInfo item in fiList)
            {
                progress.Report($"讀取與解析檔案: {item.FullName}");
                idx++;
                // =========================================================
                // 階段 1：在記憶體中準備給 BulkCopy 專用的 DataTable 
                // =========================================================
                DataTable dtTemp = new DataTable();
                dtTemp.Columns.Add("STID", typeof(string));
                dtTemp.Columns.Add("time", typeof(string));
                dtTemp.Columns.Add("PP01", typeof(decimal));

                var allLines = File.ReadLines(item.FullName, Encoding.Default)
                                   .Select(line => line.Replace("\r", "").Trim())
                                   .Where(line => !string.IsNullOrEmpty(line))
                                   .ToList();

                string headerLine = allLines.FirstOrDefault(x => x.StartsWith("#"));
                if (headerLine == null) continue;

                List<string> colsList = headerLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Where(x => x != "#").ToList();

                int idxStno = colsList.IndexOf("stno");
                int idxTime = colsList.IndexOf("yyyymmddhh");
                int idxPP01 = colsList.IndexOf("PP01");

                if (idxStno == -1 || idxTime == -1 || idxPP01 == -1)
                    throw new Exception($"檔案 {item.Name} 缺少必要欄位");

                var dataLines = allLines.Where(x => !x.StartsWith("*") && !x.StartsWith("#")).ToList();

                foreach (var rowItem in dataLines)
                {
                    string[] cells = rowItem.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (cells.Length <= Math.Max(idxStno, Math.Max(idxTime, idxPP01))) continue;

                    string stid = cells[idxStno];
                    string timeRaw = cells[idxTime];
                    if (timeRaw.Contains("99")) throw new Exception($"{stid}-{timeRaw}==時間格式異常");

                    string timeStr = Convert.ToString(Convert.ToInt32(timeRaw) - 1);
                    decimal pp01 = decimal.TryParse(cells[idxPP01], out decimal tempVal) ? (tempVal < 0 ? 0 : tempVal) : 0;

                    // 將資料單純塞入記憶體的 DataTable (不用呼叫資料庫，速度極快)
                    dtTemp.Rows.Add(stid, timeStr, pp01);
                }

                if (dtTemp.Rows.Count == 0) continue;

                progress.Report($"({idx}/{fiList.Count()})[{item.Name}]開始進行 SqlBulkCopy 大批次寫入 ({dtTemp.Rows.Count} 筆)...");

                // =========================================================
                // 階段 2 & 3：開啟資料庫連線，執行 SqlBulkCopy 與 MERGE
                // =========================================================
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // 使用交易(Transaction)包起來，確保失敗時不會寫入一半
                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. 在 SQL Server 中建立暫存表 (# 開頭代表連線中斷自動消失)
                            string createTempTableSql = @"
                        CREATE TABLE #TempWeaRain (
                            STID VARCHAR(50),
                            [time] VARCHAR(20),
                            PP01 DECIMAL(18,2)
                        )";
                            using (SqlCommand cmd = new SqlCommand(createTempTableSql, conn, tx))
                            {
                                cmd.CommandTimeout = 1200;
                                cmd.ExecuteNonQuery();
                            }

                            // 2. 啟動 SqlBulkCopy，將 DataTable 瞬間倒入暫存表
                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tx))
                            {
                                bulkCopy.DestinationTableName = "#TempWeaRain";
                                bulkCopy.BatchSize = 10000;  // 每一萬筆送一次
                                bulkCopy.BulkCopyTimeout = 60; // 避免超大檔案逾時

                                bulkCopy.ColumnMappings.Add("STID", "STID");
                                bulkCopy.ColumnMappings.Add("time", "time");
                                bulkCopy.ColumnMappings.Add("PP01", "PP01");

                                bulkCopy.WriteToServer(dtTemp);
                            }

                            progress.Report($"({idx}/{fiList.Count()})[{item.Name}-WeaRainData]開始進行 SqlBulkCopy 大批次寫入 ({dtTemp.Rows.Count} 筆)...");
                            // 3. 讓 SQL Server 自己用 MERGE 去比對資料 (WeaRainData)
                            string mergeSql1 = @"
                        MERGE INTO WeaRainData AS Target
                        USING #TempWeaRain AS Source
                        ON Target.STID = Source.STID AND Target.[time] = Source.[time]
                        WHEN MATCHED THEN
                            UPDATE SET PP01 = Source.PP01, PP01old = Source.PP01
                        WHEN NOT MATCHED BY TARGET THEN
                            INSERT (STID, [time], PP01, PP01old) 
                            VALUES (Source.STID, Source.[time], Source.PP01, Source.PP01);";

                            using (SqlCommand cmd = new SqlCommand(mergeSql1, conn, tx))
                            {
                                cmd.CommandTimeout = 1200;
                                cmd.ExecuteNonQuery();
                            }

                            progress.Report($"({idx}/{fiList.Count()})[{item.Name}-WeaRainDataHis]開始進行 SqlBulkCopy 大批次寫入 ({dtTemp.Rows.Count} 筆)...");
                            // 4. 同樣的方法處理歷史資料表 (WeaRainDataHis)
                            string mergeSql2 = @"
                        MERGE INTO WeaRainDataHis AS Target
                        USING #TempWeaRain AS Source
                        ON Target.Stid = Source.STID AND Target.[Time] = Source.[time]
                        WHEN MATCHED THEN
                            UPDATE SET Hour = Source.PP01
                        WHEN NOT MATCHED BY TARGET THEN
                            INSERT (Stid, [Time], Hour, TransYN) 
                            VALUES (Source.STID, Source.[time], Source.PP01, 'N');";

                            using (SqlCommand cmd = new SqlCommand(mergeSql2, conn, tx))
                            {
                                cmd.CommandTimeout = 1200;
                                cmd.ExecuteNonQuery();
                            }

                            tx.Commit(); // 全部成功，正式寫入
                        }
                        catch
                        {
                            tx.Rollback(); // 有任何問題，整批退回
                            throw;
                        }
                    }
                }

                // =========================================================
                // 階段 4：搬移檔案
                // =========================================================
                string okDirPath = Path.Combine(item.DirectoryName, "OK");
                Directory.CreateDirectory(okDirPath);
                string newFullName = Path.Combine(okDirPath, item.Name);

                if (File.Exists(newFullName)) File.Delete(newFullName);
                item.MoveTo(newFullName);
            }

            progress.Report("所有氣象資料 SqlBulkCopy 轉檔完成！");
        }






        /// <summary>
        /// 氣象年度雨量資料統計
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTransWeaRainStaticOld_Click(object sender, EventArgs e)
        {
            //1.for迴圈中，調整要跑的年度後，直接執行，只跑單一程式         


            ShowStatus("取得所有要執行的雨量站");
            //全部的站都要跑都要跑
            ssql = " select distinct stid from WeaRainData order by STID ";
            //ssql = " select distinct stid from WeaRainData where stid = '00H710' order by STID ";
            //如果要跑特定的的站，要抓取需要跑資料的STID
            //ssql = " select distinct stid from WraRainDataHis ";

            List<dynamic> StidList = dbDapper.Query(ssql);

            int iIndex = 0;
            foreach (var StidItem in StidList)
            {
                Application.DoEvents();
                iIndex++;

                //StidItem.stid = "466880";
                //每個STID都要跑1987-2017的資料
                //20210504 雨量站統計資訊年度更新，要改成要跑的年度
                for (int y = 2024; y <= 2024; y++)
                {
                    Application.DoEvents();
                    //雨量站
                    string sStid = StidItem.stid;
                    //統計年度
                    int iDataYear = y;

                    //sStid = "466920";
                    //iDataYear = 1997;
                    ShowStatus(string.Format("[{2}/{3}]{0}-{1}", sStid, iDataYear, iIndex, StidList.Count));


                    string sDataStaticLogType = "TransWeaRainStatic";
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' and key2 = '{2}' ";
                    ssql = string.Format(ssql, sDataStaticLogType, sStid, iDataYear.ToString());
                    //已轉過
                    if (dbDapper.QueryTotalCount(ssql) > 0) continue;

                    #region 雨量年度資料統計
                    WeaRainStatistics wrs = new WeaRainStatistics();

                    DateTime dtStart = DateTime.ParseExact(iDataYear.ToString() + "0101", "yyyyMMdd", null);
                    DateTime dtFinish = DateTime.ParseExact(iDataYear.ToString() + "1231", "yyyyMMdd", null);

                    wrs.stid = sStid;
                    wrs.year = dtStart.Year.ToString();

                    ssql = @"
                        select * from WeaRainData where STID =  '{0}' and time between '{1}000000' and '{1}123200'
                        ";

                    ssql = string.Format(ssql, sStid, iDataYear.ToString());

                    List<WeaRainData> wrdList = dbDapper.Query<WeaRainData>(ssql);


                    //沒資料
                    if (wrdList == null && wrdList.Count == 0)
                    {

                        //寫入統計LOG
                        DataStaticLog dslTemp = new DataStaticLog();
                        dslTemp.type = sDataStaticLogType;
                        dslTemp.key1 = sStid;
                        dslTemp.key2 = iDataYear.ToString();
                        dslTemp.logtime = DateTime.Now;
                        dbDapper.Insert(dslTemp);

                        continue;
                    }

                    //月雨量
                    Decimal dM01Sum = -99;
                    Decimal dM02Sum = -99;
                    Decimal dM03Sum = -99;
                    Decimal dM04Sum = -99;
                    Decimal dM05Sum = -99;
                    Decimal dM06Sum = -99;
                    Decimal dM07Sum = -99;
                    Decimal dM08Sum = -99;
                    Decimal dM09Sum = -99;
                    Decimal dM10Sum = -99;
                    Decimal dM11Sum = -99;
                    Decimal dM12Sum = -99;
                    //判斷該月是否有資料，如無資料則標示-99
                    if (wrdList.Where(s => s.time.Substring(4, 2) == "01").Count() > 0)
                    {
                        dM01Sum = (Decimal)wrdList.Where(s => s.time.Substring(4, 2) == "01").Sum(t => t.PP01);
                    }
                    if (wrdList.Where(s => s.time.Substring(4, 2) == "02").Count() > 0)
                    {
                        dM02Sum = (Decimal)wrdList.Where(s => s.time.Substring(4, 2) == "02").Sum(t => t.PP01);
                    }
                    if (wrdList.Where(s => s.time.Substring(4, 2) == "03").Count() > 0)
                    {
                        dM03Sum = (Decimal)wrdList.Where(s => s.time.Substring(4, 2) == "03").Sum(t => t.PP01);
                    }
                    if (wrdList.Where(s => s.time.Substring(4, 2) == "04").Count() > 0)
                    {
                        dM04Sum = (Decimal)wrdList.Where(s => s.time.Substring(4, 2) == "04").Sum(t => t.PP01);
                    }
                    if (wrdList.Where(s => s.time.Substring(4, 2) == "05").Count() > 0)
                    {
                        dM05Sum = (Decimal)wrdList.Where(s => s.time.Substring(4, 2) == "05").Sum(t => t.PP01);
                    }
                    if (wrdList.Where(s => s.time.Substring(4, 2) == "06").Count() > 0)
                    {
                        dM06Sum = (Decimal)wrdList.Where(s => s.time.Substring(4, 2) == "06").Sum(t => t.PP01);
                    }
                    if (wrdList.Where(s => s.time.Substring(4, 2) == "07").Count() > 0)
                    {
                        dM07Sum = (Decimal)wrdList.Where(s => s.time.Substring(4, 2) == "07").Sum(t => t.PP01);
                    }
                    if (wrdList.Where(s => s.time.Substring(4, 2) == "08").Count() > 0)
                    {
                        dM08Sum = (Decimal)wrdList.Where(s => s.time.Substring(4, 2) == "08").Sum(t => t.PP01);
                    }
                    if (wrdList.Where(s => s.time.Substring(4, 2) == "09").Count() > 0)
                    {
                        dM09Sum = (Decimal)wrdList.Where(s => s.time.Substring(4, 2) == "09").Sum(t => t.PP01);
                    }
                    if (wrdList.Where(s => s.time.Substring(4, 2) == "10").Count() > 0)
                    {
                        dM10Sum = (Decimal)wrdList.Where(s => s.time.Substring(4, 2) == "10").Sum(t => t.PP01);
                    }
                    if (wrdList.Where(s => s.time.Substring(4, 2) == "11").Count() > 0)
                    {
                        dM11Sum = (Decimal)wrdList.Where(s => s.time.Substring(4, 2) == "11").Sum(t => t.PP01);
                    }
                    if (wrdList.Where(s => s.time.Substring(4, 2) == "12").Count() > 0)
                    {
                        dM12Sum = (Decimal)wrdList.Where(s => s.time.Substring(4, 2) == "12").Sum(t => t.PP01);
                    }

                    //1070928 黃亭茵 判斷 當月連續超過24筆當做資料異常 月平均、年平均都不列入
                    List<WeaRainData> wrdTempList = new List<WeaRainData>();

                    wrdTempList = wrdList.Where(s => s.time.Substring(4, 2) == "01").ToList<WeaRainData>();
                    if (CalcRainDataError(wrdTempList, 24))
                    {
                        dM01Sum = -99;
                    }
                    wrdTempList = wrdList.Where(s => s.time.Substring(4, 2) == "02").ToList<WeaRainData>();
                    if (CalcRainDataError(wrdTempList, 24))
                    {
                        dM02Sum = -99;
                    }
                    wrdTempList = wrdList.Where(s => s.time.Substring(4, 2) == "03").ToList<WeaRainData>();
                    if (CalcRainDataError(wrdTempList, 24))
                    {
                        dM03Sum = -99;
                    }
                    wrdTempList = wrdList.Where(s => s.time.Substring(4, 2) == "04").ToList<WeaRainData>();
                    if (CalcRainDataError(wrdTempList, 24))
                    {
                        dM04Sum = -99;
                    }
                    wrdTempList = wrdList.Where(s => s.time.Substring(4, 2) == "05").ToList<WeaRainData>();
                    if (CalcRainDataError(wrdTempList, 24))
                    {
                        dM05Sum = -99;
                    }
                    wrdTempList = wrdList.Where(s => s.time.Substring(4, 2) == "06").ToList<WeaRainData>();
                    if (CalcRainDataError(wrdTempList, 24))
                    {
                        dM06Sum = -99;
                    }
                    wrdTempList = wrdList.Where(s => s.time.Substring(4, 2) == "07").ToList<WeaRainData>();
                    if (CalcRainDataError(wrdTempList, 24))
                    {
                        dM07Sum = -99;
                    }
                    wrdTempList = wrdList.Where(s => s.time.Substring(4, 2) == "08").ToList<WeaRainData>();
                    if (CalcRainDataError(wrdTempList, 24))
                    {
                        dM08Sum = -99;
                    }
                    wrdTempList = wrdList.Where(s => s.time.Substring(4, 2) == "09").ToList<WeaRainData>();
                    if (CalcRainDataError(wrdTempList, 24))
                    {
                        dM09Sum = -99;
                    }
                    wrdTempList = wrdList.Where(s => s.time.Substring(4, 2) == "10").ToList<WeaRainData>();
                    if (CalcRainDataError(wrdTempList, 24))
                    {
                        dM10Sum = -99;
                    }
                    wrdTempList = wrdList.Where(s => s.time.Substring(4, 2) == "11").ToList<WeaRainData>();
                    if (CalcRainDataError(wrdTempList, 24))
                    {
                        dM11Sum = -99;
                    }
                    wrdTempList = wrdList.Where(s => s.time.Substring(4, 2) == "12").ToList<WeaRainData>();
                    if (CalcRainDataError(wrdTempList, 24))
                    {
                        dM12Sum = -99;
                    }

                    wrs.m01 = dM01Sum;
                    wrs.m02 = dM02Sum;
                    wrs.m03 = dM03Sum;
                    wrs.m04 = dM04Sum;
                    wrs.m05 = dM05Sum;
                    wrs.m06 = dM06Sum;
                    wrs.m07 = dM07Sum;
                    wrs.m08 = dM08Sum;
                    wrs.m09 = dM09Sum;
                    wrs.m10 = dM10Sum;
                    wrs.m11 = dM11Sum;
                    wrs.m12 = dM12Sum;

                    //月平均
                    int iMonAvgCount = 0;
                    Decimal dMonAvgSum = 0;
                    if (dM01Sum != -99)
                    {
                        iMonAvgCount++;
                        dMonAvgSum += dM01Sum;
                    }
                    if (dM02Sum != -99)
                    {
                        iMonAvgCount++;
                        dMonAvgSum += dM02Sum;
                    }
                    if (dM03Sum != -99)
                    {
                        iMonAvgCount++;
                        dMonAvgSum += dM03Sum;
                    }
                    if (dM04Sum != -99)
                    {
                        iMonAvgCount++;
                        dMonAvgSum += dM04Sum;
                    }
                    if (dM05Sum != -99)
                    {
                        iMonAvgCount++;
                        dMonAvgSum += dM05Sum;
                    }
                    if (dM06Sum != -99)
                    {
                        iMonAvgCount++;
                        dMonAvgSum += dM06Sum;
                    }
                    if (dM07Sum != -99)
                    {
                        iMonAvgCount++;
                        dMonAvgSum += dM07Sum;
                    }
                    if (dM08Sum != -99)
                    {
                        iMonAvgCount++;
                        dMonAvgSum += dM08Sum;
                    }
                    if (dM09Sum != -99)
                    {
                        iMonAvgCount++;
                        dMonAvgSum += dM09Sum;
                    }
                    if (dM10Sum != -99)
                    {
                        iMonAvgCount++;
                        dMonAvgSum += dM10Sum;
                    }
                    if (dM11Sum != -99)
                    {
                        iMonAvgCount++;
                        dMonAvgSum += dM11Sum;
                    }
                    if (dM12Sum != -99)
                    {
                        iMonAvgCount++;
                        dMonAvgSum += dM12Sum;
                    }

                    Decimal dMonAvg = 0;
                    if (iMonAvgCount != 0)
                    {
                        dMonAvg = dMonAvgSum / iMonAvgCount;
                    }
                    wrs.mavg = dMonAvg;

                    //年雨量計算：若該年度有某一或數個月之月雨量資料出現異常，則該年度不計算年雨量。
                    if (dM01Sum == -99 || dM02Sum == -99 || dM03Sum == -99 || dM04Sum == -99 || dM05Sum == -99 || dM06Sum == -99 || dM07Sum == -99 || dM08Sum == -99 || dM09Sum == -99 || dM10Sum == -99 || dM11Sum == -99 || dM12Sum == -99)
                    {
                        wrs.yearsum = -99;
                    }
                    else
                    {
                        wrs.yearsum = dM01Sum + dM02Sum + dM03Sum + dM04Sum + dM05Sum + dM06Sum + dM07Sum + dM08Sum + dM09Sum + dM10Sum + dM11Sum + dM12Sum;
                    }


                    //計算日降雨量清單
                    List<WeaRainData> WeaRainDayData = new List<WeaRainData>();
                    for (DateTime x = dtStart; x <= dtFinish; x = x.AddDays(1))
                    {
                        //計算日降雨量
                        decimal dRainDaySum = wrdList.Where(s => s.time.Substring(0, 8) == x.ToString("yyyyMMdd")).Sum(t => t.PP01);

                        WeaRainData wrd = new WeaRainData();
                        wrd.STID = sStid;
                        wrd.time = x.ToString("yyyyMMdd");
                        wrd.PP01 = dRainDaySum;
                        WeaRainDayData.Add(wrd);

                        //todo 可規劃記錄在資料庫中
                    }

                    //降雨日數：一年內日雨量達0.1毫米以上之總日數(單位為日)
                    int iRainDayCount = 0;
                    iRainDayCount = WeaRainDayData.Where(s => s.PP01 > decimal.Parse("0.1")).Count();
                    wrs.raindatecount = iRainDayCount;


                    //最大一日雨量：一年中日雨量之最大值；
                    Decimal dMax1 = WeaRainDayData.Max(t => t.PP01);
                    WeaRainData wwww = WeaRainDayData.Where(s => s.PP01 == dMax1).ToList<WeaRainData>()[0];
                    wrs.max1 = dMax1;
                    wrs.max1date = wwww.time;

                    //最大二日雨量：一年中連續二日雨量之最大值
                    Decimal dMax2 = 0;
                    string sMax2Date = "";
                    for (int i = 0; i < WeaRainDayData.Count - 1; i++)
                    {
                        if (i + 1 <= WeaRainDayData.Count - 1)
                        {
                            //兩天雨量相加
                            Decimal dSum = WeaRainDayData[i].PP01 + WeaRainDayData[i + 1].PP01;
                            if (dSum > dMax2)
                            {
                                dMax2 = dSum;
                                sMax2Date = WeaRainDayData[i].time;
                            }
                        }
                    }
                    wrs.max2 = dMax2;
                    wrs.max2date = sMax2Date;

                    //最大三日雨量：一年中連續三日雨量之最大值
                    Decimal dMax3 = 0;
                    string sMax3Date = "";
                    for (int i = 0; i < WeaRainDayData.Count - 1; i++)
                    {
                        if (i + 2 == WeaRainDayData.Count - 1)
                        {
                            ssql = "";
                        }
                        if (i + 2 <= WeaRainDayData.Count - 1)
                        {
                            //兩天雨量相加
                            Decimal dSum = WeaRainDayData[i].PP01 + WeaRainDayData[i + 1].PP01 + WeaRainDayData[i + 2].PP01;
                            if (dSum > dMax3)
                            {
                                dMax3 = dSum;
                                sMax3Date = WeaRainDayData[i].time;
                            }
                        }
                    }
                    wrs.max3 = dMax3;
                    wrs.max3date = sMax3Date;


                    ssql = " select * from WeaRainStatistics where stid = '{0}' and year = '{1}' ";
                    ssql = string.Format(ssql, wrs.stid, wrs.year);
                    WeaRainStatistics Temp = dbDapper.QuerySingleOrDefault<WeaRainStatistics>(ssql);
                    if (Temp == null)
                    {
                        //新增
                        dbDapper.Insert(wrs);
                    }
                    else
                    {
                        //更新
                        Temp.m01 = wrs.m01;
                        Temp.m02 = wrs.m02;
                        Temp.m03 = wrs.m03;
                        Temp.m04 = wrs.m04;
                        Temp.m05 = wrs.m05;
                        Temp.m06 = wrs.m06;
                        Temp.m07 = wrs.m07;
                        Temp.m08 = wrs.m08;
                        Temp.m09 = wrs.m09;
                        Temp.m10 = wrs.m10;
                        Temp.m11 = wrs.m11;
                        Temp.m12 = wrs.m12;
                        Temp.mavg = wrs.mavg;
                        Temp.yearsum = wrs.yearsum;
                        Temp.max1 = wrs.max1;
                        Temp.max1date = wrs.max1date;
                        Temp.max2 = wrs.max2;
                        Temp.max2date = wrs.max2date;
                        Temp.max3 = wrs.max3;
                        Temp.max3date = wrs.max3date;
                        Temp.raindatecount = wrs.raindatecount;
                        dbDapper.Update(Temp);
                    }

                    #endregion

                    //寫入統計LOG
                    DataStaticLog dsl = new DataStaticLog();
                    dsl.type = sDataStaticLogType;
                    dsl.key1 = sStid;
                    dsl.key2 = iDataYear.ToString();
                    dsl.logtime = DateTime.Now;
                    dbDapper.Insert(dsl);


                }

            }
        }
        // =========================================================
        // UI 按鈕觸發事件
        // =========================================================
        private async void btnTransWeaRainStatic_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            // 將進度訊息導向 MsgText (RichTextBox)
            var progress = new Progress<string>(msg => { LogToMsgText(msg); });

            try
            {
                await Task.Run(() => ProcessWeaRainStatistics(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"統計發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
            }
        }

        // =========================================================
        // 核心統計處理邏輯 (純同步 dbDapper 寫法相容)
        // =========================================================
        private void ProcessWeaRainStatistics(IProgress<string> progress)
        {
            string sDataStaticLogType = "TransWeaRainStatic";
            progress.Report("開始取得需要統計的雨量站清單...");

            // 1. 取得測站清單
            string getStidSql = "SELECT DISTINCT STID FROM WeaRainData ORDER BY STID";
            List<dynamic> stidDynamicList = dbDapper.Query(getStidSql);
            List<string> stidList = stidDynamicList.Select(d => (string)d.STID).ToList();

            int iIndex = 0;
            int targetYearStart = 2010; // 💡 這裡設定您要跑的開始年度
            int targetYearEnd = 2025;   // 💡 這裡設定您要跑的結束年度

            // 外層迴圈：遍歷每一個測站
            foreach (string sStid in stidList)
            {
                iIndex++;

                // 內層迴圈：遍歷該測站的每一個年度
                for (int targetYear = targetYearStart; targetYear <= targetYearEnd; targetYear++)
                {
                    progress.Report($"[{sStid}-{targetYear}] ({iIndex}/{stidList.Count}) 統計執行中...");

                    // 2. 檢查是否已經轉檔過
                    string checkLogSql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' and key2 = '{2}' ";
                    checkLogSql = string.Format(checkLogSql, sDataStaticLogType, sStid, targetYear.ToString());
                    if (dbDapper.QueryTotalCount(checkLogSql) > 0)
                    {
                        progress.Report($"  - 已有統計紀錄，跳過。");
                        continue;
                    }

                    // =========================================================
                    // 🔥 終極 SQL：一次算完「日雨量」、「連續多日雨量」、「連續24筆異常檢核」
                    // =========================================================
                    string getDailyRainSql = @"
                        WITH HourlyData AS (
                            SELECT 
                                SUBSTRING([time], 1, 8) AS DailyDate,
                                SUBSTRING([time], 5, 2) AS [Month],
                                [time],
                                PP01,
                                -- 步驟 1：標記異常資料 (PP01old < 0 視為異常 1，正常為 0)
                                CASE WHEN ISNULL(PP01old, 0) < 0 THEN 1 ELSE 0 END AS IsError
                            FROM WeaRainData 
                            WHERE STID = @Stid 
                              AND [time] >= @StartTime AND [time] <= @EndTime
                        ),
                        ErrorCheck AS (
                            SELECT 
                                [Month],
                                -- 步驟 2：核心邏輯！在同一個月內，往前算23筆+自己=24筆。
                                -- 如果這 24 筆的 IsError 加總等於 24，代表連續 24 筆都是異常！
                                CASE WHEN SUM(IsError) OVER (
                                    PARTITION BY [Month]
                                    ORDER BY [time] 
                                    ROWS BETWEEN 23 PRECEDING AND CURRENT ROW
                                ) = 24 THEN 1 ELSE 0 END AS Hit24Error
                            FROM HourlyData
                        ),
                        MonthErrorSummary AS (
                            -- 步驟 3：只要該月發生過一次連續 24 筆異常，整個月份就標示為 1 (異常作廢)
                            SELECT [Month], MAX(Hit24Error) AS IsMonthInvalid
                            FROM ErrorCheck
                            GROUP BY [Month]
                        ),
                        DailyRain AS (
                            SELECT 
                                DailyDate,
                                [Month],
                                SUM(PP01) AS DailySum
                            FROM HourlyData
                            GROUP BY DailyDate, [Month]
                        ),
                        DailyRainWithWindow AS (
                            SELECT 
                                DailyDate, 
                                [Month], 
                                DailySum,
                                SUM(DailySum) OVER(ORDER BY DailyDate ROWS BETWEEN 1 PRECEDING AND CURRENT ROW) AS Sum2Days,
                                SUM(DailySum) OVER(ORDER BY DailyDate ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS Sum3Days
                            FROM DailyRain
                        )
                        -- 步驟 4：將算好的日資料，與該月的「異常標記」合併輸出給 C#
                        SELECT 
                            d.DailyDate, 
                            d.[Month], 
                            d.DailySum,
                            d.Sum2Days,
                            d.Sum3Days,
                            ISNULL(m.IsMonthInvalid, 0) AS IsMonthInvalid
                        FROM DailyRainWithWindow d
                        LEFT JOIN MonthErrorSummary m ON d.[Month] = m.[Month]
                        ORDER BY d.DailyDate;
                    ";

                    // 執行 SQL
                    List<dynamic> dailyData = dbDapper.Query(getDailyRainSql, new
                    {
                        Stid = sStid,
                        StartTime = $"{targetYear}010100",
                        EndTime = $"{targetYear}123124"
                    });

                    if (dailyData == null || dailyData.Count == 0)
                    {
                        InsertLog(sDataStaticLogType, sStid, targetYear.ToString());
                        continue; // 如果這個年度沒資料，直接跳到下一個年度
                    }

                    // =========================================================
                    // 階段 3：在 C# 中快速組裝結果
                    // =========================================================
                    WeaRainStatistics wrs = new WeaRainStatistics { stid = sStid, year = targetYear.ToString() };
                    decimal[] monthSums = new decimal[13];

                    // 預設填滿 -99
                    for (int i = 1; i <= 12; i++) monthSums[i] = -99;

                    // 計算月雨量 (並套用連續 24 筆異常規則)
                    var monthGroups = dailyData.GroupBy(d => (string)d.Month);
                    foreach (var mg in monthGroups)
                    {
                        int mIdx = int.Parse(mg.Key);

                        // 讀取 SQL 算好的異常旗標
                        bool isMonthInvalid = mg.First().IsMonthInvalid == 1;

                        if (isMonthInvalid)
                        {
                            // 該月有連續 24 筆異常，資料作廢
                            monthSums[mIdx] = -99;
                        }
                        else
                        {
                            // 資料正常，加總日雨量
                            monthSums[mIdx] = mg.Sum(d => (decimal)d.DailySum);
                        }
                    }

                    // 塞回物件
                    wrs.m01 = monthSums[1]; wrs.m02 = monthSums[2]; wrs.m03 = monthSums[3]; wrs.m04 = monthSums[4];
                    wrs.m05 = monthSums[5]; wrs.m06 = monthSums[6]; wrs.m07 = monthSums[7]; wrs.m08 = monthSums[8];
                    wrs.m09 = monthSums[9]; wrs.m10 = monthSums[10]; wrs.m11 = monthSums[11]; wrs.m12 = monthSums[12];

                    // 計算月平均與年總和 (排除 -99)
                    var validMonths = monthSums.Skip(1).Where(m => m != -99).ToList();
                    wrs.mavg = validMonths.Count > 0 ? validMonths.Average() : 0;
                    wrs.yearsum = monthSums.Skip(1).Any(m => m == -99) ? -99 : validMonths.Sum();

                    // 計算降雨日數 (>0.1)
                    wrs.raindatecount = dailyData.Count(d => (decimal)d.DailySum > 0.1m);

                    // 計算最大 1, 2, 3 日雨量
                    var max1Row = dailyData.OrderByDescending(d => (decimal)d.DailySum).First();
                    wrs.max1 = max1Row.DailySum; wrs.max1date = max1Row.DailyDate;

                    if (dailyData.Count >= 2)
                    {
                        var max2Row = dailyData.OrderByDescending(d => (decimal)d.Sum2Days).First();
                        wrs.max2 = max2Row.Sum2Days; wrs.max2date = max2Row.DailyDate;
                    }
                    if (dailyData.Count >= 3)
                    {
                        var max3Row = dailyData.OrderByDescending(d => (decimal)d.Sum3Days).First();
                        wrs.max3 = max3Row.Sum3Days; wrs.max3date = max3Row.DailyDate;
                    }

                    // =========================================================
                    // 階段 4：寫回資料庫
                    // =========================================================
                    string upsertStatSql = @"
            MERGE INTO WeaRainStatistics AS T
            USING (SELECT @stid AS stid, @year AS year) AS S ON T.stid = S.stid AND T.year = S.year
            WHEN MATCHED THEN
                UPDATE SET m01=@m01, m02=@m02, m03=@m03, m04=@m04, m05=@m05, m06=@m06, 
                           m07=@m07, m08=@m08, m09=@m09, m10=@m10, m11=@m11, m12=@m12, 
                           mavg=@mavg, yearsum=@yearsum, max1=@max1, max1date=@max1date, 
                           max2=@max2, max2date=@max2date, max3=@max3, max3date=@max3date, raindatecount=@raindatecount
            WHEN NOT MATCHED THEN
                INSERT (stid, year, m01, m02, m03, m04, m05, m06, m07, m08, m09, m10, m11, m12, mavg, yearsum, 
                        max1, max1date, max2, max2date, max3, max3date, raindatecount)
                VALUES (@stid, @year, @m01, @m02, @m03, @m04, @m05, @m06, @m07, @m08, @m09, @m10, @m11, @m12, @mavg, @yearsum, 
                        @max1, @max1date, @max2, @max2date, @max3, @max3date, @raindatecount);";

                    dbDapper.Execute(upsertStatSql, wrs);

                    InsertLog(sDataStaticLogType, sStid, targetYear.ToString());
                } // 結束年度迴圈
            } // 結束測站迴圈

            progress.Report("所有雨量站統計完成！");
        }

        // =========================================================
        // 輔助方法：寫入 Log
        // =========================================================
        private void InsertLog(string type, string key1, string key2)
        {
            string insertLogSql = @"INSERT INTO DataStaticLog (type, key1, key2, logtime) VALUES (@type, @key1, @key2, @logtime)";
            dbDapper.Execute(insertLogSql, new { type = type, key1 = key1, key2 = key2, logtime = DateTime.Now });
        }

        private void btnRainAreaSplitOld_Click(object sender, EventArgs e)
        {
            //0-可開多個程式一起跑

            //1-每年重跑要先刪除所有資料
            //select * from DataStaticLog where type = 'RainAreaSplit'

            //1-1檢核是否有重複的資料
            /*
                -檢核            
                select * from (
                    select STID,time,count(*) as tt from WeaRainData group by STID,time 
                ) a where a.tt > 1 
                -刪除重複的序號
                delete WeaRainData where no = '1213387'
             */

            //2-調整要重新跑雨場分割的時間區間(dtStart、dtFinish)
            //每年的批次大概就是調整為前一年的0101到本年度的年底1231


            //3-比對執行進度(10執行中，60已完成，90異常)
            //比對全部的站號都要是60才是全部完成
            /*
            select * from (
            select distinct stid from WeaRainData  ) a
            left join DataStaticLog b on  b.type = 'RainAreaSplit' and a.stid = b.key1
            where(b.status != '60' or b.status is null)
            order by a.stid
            */


            string sDataStaticLogType = "RainAreaSplit";

            ssql = " select distinct stid from WeaRainData order by STID ";
            //只轉水保局雨量站資料
            //ssql = " select distinct stid from WraRainDataHis order by STID ";
            List<dynamic> StidList = dbDapper.Query(ssql);

            DateTime dttest = DateTime.Now;
            int iIndex = 0;
            foreach (var StidItem in StidList)
            {
                iIndex++;
                //雨量站
                string sStid = StidItem.stid;
                //sStid = "81M800";
                //sStid = "C0A680";


                try
                {
                    //10:執行中 60:已完成
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'  ";
                    ssql = string.Format(ssql, sDataStaticLogType, sStid);

                    DataStaticLog dsl_new = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);

                    //已經有轉檔紀錄
                    if (dsl_new == null)
                    {
                        //如果沒有轉檔紀錄，則新增一筆執行中
                        //寫入統計LOG
                        dsl_new = new DataStaticLog();
                        dsl_new.type = sDataStaticLogType;
                        dsl_new.key1 = sStid;
                        dsl_new.status = "10";
                        dsl_new.logtime = DateTime.Now;
                        dbDapper.Insert(dsl_new);

                    }
                    else
                    {
                        //10:執行中 60:已完成 90:執行失敗
                        if (dsl_new.status == "10" || dsl_new.status == "60")
                        {
                            continue;
                        }

                        //重新更新為執行中
                        dsl_new.status = "10";
                        dbDapper.Update(dsl_new);
                    }

                    DateTime dtStart = DateTime.ParseExact("2023080100", "yyyyMMddHH", null);
                    //dtStart = DateTime.ParseExact("2017060200", "yyyyMMddHH", null);
                    DateTime dtFinish = DateTime.ParseExact("2024123123", "yyyyMMddHH", null);
                    //dtFinish = DateTime.ParseExact("2009080000", "yyyyMMddHH", null);


                    //取得該雨量站所有資料
                    ssql = " select * from WeaRainData where stid = '{0}' ";
                    ssql = string.Format(ssql, sStid);
                    List<WeaRainData> RainList = dbDapper.Query<WeaRainData>(ssql);

                    string sRainAreaS = "";
                    string sRainAreaE = "";
                    DateTime temp = DateTime.Now;
                    for (DateTime i = dtStart; i <= dtFinish; i = i.AddHours(1))
                    {
                        dttest = i;
                        int iTime = Convert.ToInt32(i.ToString("yyyyMMddHH"));

                        WeaRainData wrd = RainList.SingleOrDefault(s => s.time == iTime.ToString());

                        if (wrd == null)
                        {
                            wrd = new WeaRainData();
                            wrd.time = iTime.ToString();
                            wrd.PP01 = 0;
                        }


                        ShowStatus(string.Format("[{2}/{3}]{0}-{1}", sStid, i.ToString("yyyyMMddHH"), iIndex.ToString(), StidList.Count.ToString()));


                        //非雨場且時雨量>4，則開始記錄雨場起始
                        if (wrd.PP01 > 4 && sRainAreaS == "")
                        {
                            sRainAreaS = wrd.time;

                            continue;
                        }

                        //在雨場中且時雨量<=4，則準備記錄雨場結束(持續6小時<=4)
                        if (wrd.PP01 <= 4 && sRainAreaS != "")
                        {

                            int iTime2 = Convert.ToInt32(i.AddHours(1).ToString("yyyyMMddHH"));
                            int iTime3 = Convert.ToInt32(i.AddHours(2).ToString("yyyyMMddHH"));
                            int iTime4 = Convert.ToInt32(i.AddHours(3).ToString("yyyyMMddHH"));
                            int iTime5 = Convert.ToInt32(i.AddHours(4).ToString("yyyyMMddHH"));
                            int iTime6 = Convert.ToInt32(i.AddHours(5).ToString("yyyyMMddHH"));
                            WeaRainData wrd2 = RainList.SingleOrDefault(s => s.time == iTime2.ToString());
                            WeaRainData wrd3 = RainList.SingleOrDefault(s => s.time == iTime3.ToString());
                            WeaRainData wrd4 = RainList.SingleOrDefault(s => s.time == iTime4.ToString());
                            WeaRainData wrd5 = RainList.SingleOrDefault(s => s.time == iTime5.ToString());
                            WeaRainData wrd6 = RainList.SingleOrDefault(s => s.time == iTime6.ToString());

                            //沒資料則，時雨量則補上０
                            //1071018 雨場不列入計算
                            //if (wrd2 == null) wrd2.PP01 = 0;
                            //if (wrd3 == null) wrd3.PP01 = 0;
                            //if (wrd4 == null) wrd4.PP01 = 0;
                            //if (wrd5 == null) wrd5.PP01 = 0;
                            //if (wrd6 == null) wrd6.PP01 = 0;
                            if (wrd2 == null)
                            {
                                sRainAreaS = "";
                                sRainAreaE = "";
                                continue;
                            }
                            if (wrd3 == null)
                            {
                                sRainAreaS = "";
                                sRainAreaE = "";
                                continue;
                            }
                            if (wrd4 == null)
                            {
                                sRainAreaS = "";
                                sRainAreaE = "";
                                continue;
                            }
                            if (wrd5 == null)
                            {
                                sRainAreaS = "";
                                sRainAreaE = "";
                                continue;
                            }
                            if (wrd6 == null)
                            {
                                sRainAreaS = "";
                                sRainAreaE = "";
                                continue;
                            }

                            //記錄雨場結束(持續6小時 <= 4)
                            if (wrd2.PP01 <= 4 && wrd3.PP01 <= 4 && wrd4.PP01 <= 4 && wrd5.PP01 <= 4 && wrd6.PP01 <= 4)
                            {
                                sRainAreaE = wrd.time;

                                sRainAreaE = Convert.ToString(Convert.ToInt32(i.AddHours(-1).ToString("yyyyMMddHH")));



                                WeaRainArea wraTemp = new WeaRainArea();
                                wraTemp.stid = sStid;
                                wraTemp.TimeStart = sRainAreaS;
                                wraTemp.TimeEnd = sRainAreaE;
                                wraTemp.RainHour = 0;
                                wraTemp.TotalRain = 0;
                                wraTemp.MaxRain = 0;
                                wraTemp.MaxRainTime = "";
                                wraTemp.Max3Sum = 0;
                                wraTemp.Max6Sum = 0;
                                wraTemp.Max12Sum = 0;
                                wraTemp.Max24Sum = 0;
                                wraTemp.Max48Sum = 0;
                                wraTemp.Pre7DayRain6 = 0;
                                wraTemp.Pre7DayRain7 = 0;
                                wraTemp.Pre7DayRain8 = 0;
                                wraTemp.CumRain = 0;
                                wraTemp.RT6 = 0;
                                wraTemp.RT7 = 0;
                                wraTemp.RT8 = 0;


                                List<WeaRainData> RainAreaList = RainList.Where(s => Convert.ToInt32(s.time) >= Convert.ToInt32(sRainAreaS) && Convert.ToInt32(s.time) <= Convert.ToInt32(sRainAreaE)).ToList<WeaRainData>();

                                //D：降雨延時，開始至結束時間(單位：小時)
                                wraTemp.RainHour = RainAreaList.Count;

                                decimal dTempSum = 0;
                                //E：該雨場總降雨量，本次雨場的總降雨量
                                decimal dTotalRain = RainAreaList.Sum(s => s.PP01);
                                wraTemp.TotalRain = dTotalRain;
                                dTempSum = dTotalRain;

                                //G：最大時雨量，本次雨場的最大時雨量
                                decimal dMaxRain = RainAreaList.Max(s => s.PP01);
                                wraTemp.MaxRain = dMaxRain;


                                //F：最大時雨量發生時間，格式2012/1/5 18:00，24小時制
                                //如果多筆，抓第一筆發生的時間
                                string sMaxRainTime = RainAreaList.Where(s => s.PP01 == dMaxRain).Select(s => s.time).ToList<string>()[0];
                                wraTemp.MaxRainTime = sMaxRainTime;

                                //H：最大3，雨場範圍內，最大連續3小時累積雨量
                                wraTemp.Max3Sum = CalcRainMaxOld(RainAreaList, 3);
                                if (wraTemp.Max3Sum == 0)
                                {
                                    wraTemp.Max3Sum = dTempSum;
                                }
                                else
                                {
                                    //dTempSum = wraTemp.Max3Sum;
                                }
                                //if (wraTemp.Max3Sum != 0) dTempSum = wraTemp.Max3Sum;

                                //I：最大6，雨場範圍內，最大連續6小時累積雨量(如果最大6、12、24、48沒有雨的話，要填入最大的3小時)
                                wraTemp.Max6Sum = CalcRainMaxOld(RainAreaList, 6);
                                if (wraTemp.Max6Sum == 0)
                                {
                                    wraTemp.Max6Sum = dTempSum;
                                }
                                else
                                {
                                    //dTempSum = wraTemp.Max6Sum;
                                }

                                //J：最大12，雨場範圍內，最大連續12小時累積雨量(如果最大6、12、24、48沒有雨的話，要填入最大的3小時)
                                wraTemp.Max12Sum = CalcRainMaxOld(RainAreaList, 12);
                                if (wraTemp.Max12Sum == 0)
                                {
                                    wraTemp.Max12Sum = dTempSum;
                                }
                                else
                                {
                                    //dTempSum = wraTemp.Max12Sum;
                                }

                                //K：最大24，雨場範圍內，最大連續24小時累積雨量(如果最大6、12、24、48沒有雨的話，要填入最大的3小時)
                                wraTemp.Max24Sum = CalcRainMaxOld(RainAreaList, 24);
                                if (wraTemp.Max24Sum == 0)
                                {
                                    wraTemp.Max24Sum = dTempSum;
                                }
                                else
                                {
                                    //dTempSum = wraTemp.Max24Sum;
                                }

                                //L：最大48，雨場範圍內，最大連續48小時累積雨量(如果最大6、12、24、48沒有雨的話，要填入最大的3小時)
                                wraTemp.Max48Sum = CalcRainMaxOld(RainAreaList, 48);
                                if (wraTemp.Max48Sum == 0)
                                {
                                    wraTemp.Max48Sum = dTempSum;
                                }
                                else
                                {
                                    //dTempSum = wraTemp.Max48Sum;
                                }

                                //M：七天前期雨量，前頁P的計算
                                //α=0.6 
                                wraTemp.Pre7DayRain6 = CalcValuePOld(RainList, wraTemp, 0.6);
                                //四捨五入
                                wraTemp.Pre7DayRain6 = Math.Round(wraTemp.Pre7DayRain6, 1, MidpointRounding.AwayFromZero);
                                //α=0.7
                                wraTemp.Pre7DayRain7 = CalcValuePOld(RainList, wraTemp, 0.7);
                                //四捨五入
                                wraTemp.Pre7DayRain7 = Math.Round(wraTemp.Pre7DayRain7, 1, MidpointRounding.AwayFromZero);
                                //α=0.8
                                wraTemp.Pre7DayRain8 = CalcValuePOld(RainList, wraTemp, 0.8);
                                //四捨五入
                                wraTemp.Pre7DayRain8 = Math.Round(wraTemp.Pre7DayRain8, 1, MidpointRounding.AwayFromZero);

                                //N：尖零_尖峰，最大時雨量發生的時間至本次雨場開始日期0點間的累積雨量(下圖示意)
                                decimal dStart0ToSpike = 0;
                                string sStart0 = wraTemp.TimeStart.Substring(0, 8) + "00";
                                List<WeaRainData> Start0ToSpikeList = RainList.Where(s => Convert.ToInt32(s.time) >= Convert.ToInt32(sStart0) && Convert.ToInt32(s.time) <= Convert.ToInt32(wraTemp.MaxRainTime)).ToList<WeaRainData>();
                                dStart0ToSpike = Start0ToSpikeList.Sum(s => s.PP01);
                                wraTemp.CumRain = dStart0ToSpike;

                                //O：R td ，即(R t )，欄位M+欄位N
                                wraTemp.RT6 = wraTemp.Pre7DayRain6 + wraTemp.CumRain;
                                wraTemp.RT7 = wraTemp.Pre7DayRain7 + wraTemp.CumRain;
                                wraTemp.RT8 = wraTemp.Pre7DayRain8 + wraTemp.CumRain;

                                //P：時雨量，此場降雨的每小時降雨分布情形，例如：若此次雨場降雨延時為6小時，由P欄位開始往
                                //右邊欄位延續共6格，P、Q、R、S、T、U。

                                //紀錄雨場
                                ssql = " select * from wearainarea where stid = '{0}' and TimeStart = '{1}' ";
                                ssql = string.Format(ssql, sStid, sRainAreaS);
                                WeaRainArea wra = dbDapper.QuerySingleOrDefault<WeaRainArea>(ssql);
                                if (wra == null)
                                {
                                    wra = wraTemp;

                                    dbDapper.Insert(wra);
                                }
                                else
                                {
                                    wra.TimeStart = wraTemp.TimeStart;
                                    wra.TimeEnd = wraTemp.TimeEnd;
                                    wra.RainHour = wraTemp.RainHour;
                                    wra.TotalRain = wraTemp.TotalRain;
                                    wra.MaxRain = wraTemp.MaxRain;
                                    wra.MaxRainTime = wraTemp.MaxRainTime;
                                    wra.Max3Sum = wraTemp.Max3Sum;
                                    wra.Max6Sum = wraTemp.Max6Sum;
                                    wra.Max12Sum = wraTemp.Max12Sum;
                                    wra.Max24Sum = wraTemp.Max24Sum;
                                    wra.Max48Sum = wraTemp.Max48Sum;
                                    wra.Pre7DayRain6 = wraTemp.Pre7DayRain6;
                                    wra.Pre7DayRain7 = wraTemp.Pre7DayRain7;
                                    wra.Pre7DayRain8 = wraTemp.Pre7DayRain8;
                                    wra.CumRain = wraTemp.CumRain;
                                    wra.RT6 = wraTemp.RT6;
                                    wra.RT7 = wraTemp.RT7;
                                    wra.RT8 = wraTemp.RT8;
                                    dbDapper.Update(wra);
                                }

                                //記錄之後清空狀態
                                sRainAreaS = "";
                                sRainAreaE = "";
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    //該雨量站雨場轉檔完成後，該雨量站更新所有雨場的序號(RainAreaSeq)
                    ssql = " select * from WeaRainArea where stid = '{0}' order by TimeStart ";
                    ssql = string.Format(ssql, sStid);
                    List<WeaRainArea> RainAreaSeqList = dbDapper.Query<WeaRainArea>(ssql);

                    int iIdx = 0;
                    foreach (WeaRainArea item in RainAreaSeqList)
                    {
                        iIdx++;
                        //ShowStatus(string.Format("[{2}/{3}]{0}-{1}", sStid, item.TimeStart, iIndex.ToString(), StidList.Count.ToString()));
                        item.RainAreaSeq = iIdx;
                        dbDapper.Update<WeaRainArea>(item);
                    }

                    //註記已經轉檔完成
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'  ";
                    ssql = string.Format(ssql, sDataStaticLogType, sStid);

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "60";
                        dbDapper.Update(dslu);
                    }

                }
                catch (Exception ex)
                {
                    logger.Error(ex, string.Format("[{0}]{1}", sStid, dttest.ToString("yyyyMMddHH")));

                    //註記轉檔失敗
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'  ";
                    ssql = string.Format(ssql, sDataStaticLogType, sStid);

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "90";
                        dbDapper.Update(dslu);
                    }


                    continue;
                }
            }
        }

        /// <summary>
        /// 計算連續時雨量最大值
        /// </summary>
        /// <param name="wrdList">雨量資料來源</param>
        /// <param name="MaxCon">累積幾筆資料的最大值</param>
        /// <returns></returns>
        private decimal CalcRainMaxOld(List<WeaRainData> wrdList, int MaxCon)
        {

            decimal dResult = 0;

            //如果筆數不足，則預設為0
            if (wrdList.Count() < MaxCon)
            {
                return dResult;
            }

            for (int j = 0; j < wrdList.Count - 1; j++)
            {
                //累積資料不超過所有資料的比數
                if (j + MaxCon <= wrdList.Count)
                {
                    //時雨量相加
                    Decimal dSum = 0;
                    for (int i = j; i < j + MaxCon; i++)
                    {
                        dSum += wrdList[i].PP01;
                    }

                    if (dSum > dResult)
                    {
                        dResult = dSum;
                    }
                }
            }

            return dResult;
        }

        private decimal CalcValuePOld(List<WeaRainData> wrdList, WeaRainArea wra, double dEffData)
        {

            decimal dResult = 0;

            //M：七天前期雨量，前頁P的計算
            //如：2012 / 1 / 8 5:00起始，2012 / 1 / 8 13:00尖峰
            //P = 2012 / 1 / 8 5:00~13:00累積雨量加總，雨量不折減
            //＋2012 / 1 / 7 日累積雨量* 折減係數α＋2012 / 1 / 6 日累積雨量* 折減係數α 2
            //＋2012 / 1 / 5 日累積雨量* 折減係數α 3 ＋2012 / 1 / 4 日累積雨量* 折減係數α 4
            //＋2012 / 1 / 3 日累積雨量* 折減係數α 5 ＋2012 / 1 / 2 日累積雨量* 折減係數α 6
            //＋2012 / 1 / 1 日累積雨量* 折減係數α


            //α=0.6
            //decimal dEffDate = Convert.ToDecimal(0.6);
            //起始到尖峰累積雨量
            //List<WeaRainData> StartToSpikeList = wrdList.Where(s => Convert.ToInt32(s.time) >= Convert.ToInt32(wra.TimeStart) && Convert.ToInt32(s.time) <= Convert.ToInt32(wra.MaxRainTime)).ToList<WeaRainData>();

            //decimal StartToSpikeSum = StartToSpikeList.Sum(s => s.PP01);

            //1071005 前期雨量，不包含尖零尖峰
            decimal StartToSpikeSum = 0;

            //取得起始日期
            DateTime dtPStart = DateTime.ParseExact(wra.TimeStart.Substring(0, 8), "yyyyMMdd", null);


            for (int i = 1; i <= 7; i++)
            {
                decimal PreDay = 0;
                dtPStart = dtPStart.AddDays(-1);
                string sPreDay = dtPStart.ToString("yyyyMMdd");
                List<WeaRainData> PreDayList = wrdList.Where(s => Convert.ToInt32(s.time) >= Convert.ToInt32(sPreDay + "01") && Convert.ToInt32(s.time) <= Convert.ToInt32(sPreDay + "24")).ToList<WeaRainData>();
                PreDay = PreDayList.Sum(s => s.PP01) * Convert.ToDecimal(Math.Pow(dEffData, i));

                StartToSpikeSum += PreDay;
            }

            dResult = StartToSpikeSum;

            return dResult;
        }


        // =========================================================
        // UI 按鈕觸發事件 (非同步防卡死)
        // =========================================================
        private async void btnRainAreaSplit_Click(object sender, EventArgs e)
        {
            //0-可開多個程式一起跑

            //1-每年重跑要先刪除所有資料
            //select * from DataStaticLog where type = 'RainAreaSplit'

            //1-1(取消-資料庫已經加上unique不會重複了)檢核是否有重複的資料
            /*
                -檢核            
                select * from (
                    select STID,time,count(*) as tt from WeaRainData group by STID,time 
                ) a where a.tt > 1 
                -刪除重複的序號
                delete WeaRainData where no = '1213387'
             */

            //2-調整要重新跑雨場分割的時間區間(dtStart、dtFinish)
            //每年的批次大概就是調整為前一年的0101到本年度的年底1231


            //3-比對執行進度(10執行中，60已完成，90異常)
            //比對全部的站號都要是60才是全部完成
            /*
            select * from (
            select distinct stid from WeaRainData  ) a
            left join DataStaticLog b on  b.type = 'RainAreaSplit' and a.stid = b.key1
            where(b.status != '60' or b.status is null)
            order by a.stid
            */

            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            // 將進度訊息導向 MsgText (RichTextBox)
            var progress = new Progress<string>(msg => { LogToMsgText(msg); });

            try
            {
                await Task.Run(() => ProcessRainAreaSplit(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"雨場分割發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
            }
        }

        // =========================================================
        // 核心雨場分割邏輯 (支援多程式併發分工防衝突破解版)
        // =========================================================
        private void ProcessRainAreaSplit(IProgress<string> progress)
        {
            string sDataStaticLogType = "RainAreaSplit";
            progress.Report("開始取得需要分割的雨量站清單...");

            // 只撈出尚未轉檔完成 (非 60)，且沒有其他程式正在執行中 (非 10) 的測站
            //string ssql = @"
            //    SELECT a.STID 
            //    FROM (SELECT DISTINCT STID FROM WeaRainData) a
            //    LEFT JOIN DataStaticLog b ON b.type = 'RainAreaSplit' AND a.STID = b.key1
            //    WHERE b.status IS NULL OR (b.status != '60' AND b.status != '10')
            //    ORDER BY a.STID";

            string ssql = @"
                SELECT a.STID 
                FROM (SELECT DISTINCT STID FROM WeaRainData where STID in (select distinct stid from TCGRainDataHis)) a
                LEFT JOIN DataStaticLog b ON b.type = 'RainAreaSplit' AND a.STID = b.key1
                WHERE b.status IS NULL OR (b.status != '60' AND b.status != '10')
                ORDER BY a.STID";


            List<dynamic> stidDynamicList = dbDapper.Query(ssql);
            List<string> StidList = stidDynamicList.Select(d => (string)d.STID).ToList();

            DateTime dtStart = DateTime.ParseExact("2010010100", "yyyyMMddHH", null);
            DateTime dtFinish = DateTime.ParseExact("2025123123", "yyyyMMddHH", null);

            int iIndex = 0;
            foreach (string sStid in StidList)
            {
                iIndex++;
                progress.Report($"[{sStid}] ({iIndex}/{StidList.Count}) 嘗試爭取處理權...");

                try
                {
                    // =========================================================
                    // 1. 極速搶佔機制 (原子操作 + 排他鎖，完全避免併發衝突)
                    // =========================================================
                    string claimSql = @"
                        BEGIN TRAN;
                        DECLARE @Claimed INT = 0;
                        
                        -- 使用 UPDLOCK 和 HOLDLOCK 鎖定這筆測站。其他程式若同時抵達，必須排隊等候。
                        IF NOT EXISTS (SELECT 1 FROM DataStaticLog WITH (UPDLOCK, HOLDLOCK) WHERE type = @Type AND key1 = @Stid)
                        BEGIN
                            -- 如果沒有紀錄，我是第一個來的，搶佔成功！
                            INSERT INTO DataStaticLog (type, key1, status, logtime) VALUES (@Type, @Stid, '10', GETDATE());
                            SET @Claimed = 1;
                        END
                        ELSE
                        BEGIN
                            -- 已經有紀錄了，檢查是不是別人正在做(10)或已完成(60)
                            UPDATE DataStaticLog
                            SET status = '10', logtime = GETDATE()
                            WHERE type = @Type AND key1 = @Stid AND status NOT IN ('10', '60');
                            
                            -- 如果有更新到資料 (@@ROWCOUNT > 0)，代表原本可能是 90 (失敗)，我搶佔成功！
                            IF @@ROWCOUNT > 0 SET @Claimed = 1;
                        END
                        
                        COMMIT TRAN;
                        SELECT @Claimed;
                    ";

                    // 執行 SQL 並取得回傳值 (1 代表搶佔成功，0 代表搶佔失敗)
                    object objClaimed = dbDapper.ExecuteScale(claimSql, new { Type = sDataStaticLogType, Stid = sStid });
                    int isClaimed = Convert.ToInt32(objClaimed);

                    if (isClaimed == 0)
                    {
                        // 搶佔失敗！代表別的程式已經拿走這筆測站，自動跳過換下一站
                        progress.Report($"  - [{sStid}] 已由其他程式處理中或已完成，自動跳過。");
                        continue;
                    }

                    progress.Report($"[{sStid}] ({iIndex}/{StidList.Count}) 取得處理權，雨場分割執行中...");

                    // =========================================================
                    // 2. 取得該雨量站所有資料
                    // =========================================================
                    string dataSql = $"SELECT * FROM WeaRainData WHERE STID = '{sStid}'";
                    List<WeaRainData> RainList = dbDapper.Query<WeaRainData>(dataSql);

                    // 🔥 核心優化：將 List 轉為 Dictionary (時間字串當 Key) 查詢速度提升千倍
                    Dictionary<string, WeaRainData> rainDict = RainList.ToDictionary(r => r.time, r => r);

                    // 輔助方法：快速從 Dictionary 取得雨量，若無資料則回傳 0
                    WeaRainData GetRainData(DateTime t)
                    {
                        string timeKey = t.ToString("yyyyMMddHH");
                        return rainDict.TryGetValue(timeKey, out WeaRainData data) ? data : new WeaRainData { time = timeKey, PP01 = 0 };
                    }

                    string sRainAreaS = "";
                    string sRainAreaE = "";

                    // =========================================================
                    // 3. 開始逐時尋找雨場
                    // =========================================================
                    for (DateTime i = dtStart; i <= dtFinish; i = i.AddHours(1))
                    {
                        WeaRainData wrd = GetRainData(i);

                        // 條件A：非雨場且時雨量 > 4，開始記錄雨場起始
                        if (wrd.PP01 > 4 && sRainAreaS == "")
                        {
                            sRainAreaS = wrd.time;
                            continue;
                        }

                        // 條件B：在雨場中且時雨量 <= 4，檢查是否連續6小時 <= 4
                        if (wrd.PP01 <= 4 && sRainAreaS != "")
                        {
                            WeaRainData wrd2 = GetRainData(i.AddHours(1));
                            WeaRainData wrd3 = GetRainData(i.AddHours(2));
                            WeaRainData wrd4 = GetRainData(i.AddHours(3));
                            WeaRainData wrd5 = GetRainData(i.AddHours(4));
                            WeaRainData wrd6 = GetRainData(i.AddHours(5));

                            // 記錄雨場結束 (持續6小時 <= 4)
                            if (wrd2.PP01 <= 4 && wrd3.PP01 <= 4 && wrd4.PP01 <= 4 && wrd5.PP01 <= 4 && wrd6.PP01 <= 4)
                            {
                                sRainAreaE = i.AddHours(-1).ToString("yyyyMMddHH");

                                WeaRainArea wraTemp = new WeaRainArea();
                                wraTemp.stid = sStid;
                                wraTemp.TimeStart = sRainAreaS;
                                wraTemp.TimeEnd = sRainAreaE;

                                // 截取此場雨的資料區間
                                long startNum = Convert.ToInt64(sRainAreaS);
                                long endNum = Convert.ToInt64(sRainAreaE);
                                List<WeaRainData> RainAreaList = RainList.Where(s => Convert.ToInt64(s.time) >= startNum && Convert.ToInt64(s.time) <= endNum).ToList();

                                wraTemp.RainHour = RainAreaList.Count;
                                decimal dTotalRain = RainAreaList.Sum(s => s.PP01);
                                wraTemp.TotalRain = dTotalRain;

                                wraTemp.MaxRain = RainAreaList.Max(s => s.PP01);
                                wraTemp.MaxRainTime = RainAreaList.First(s => s.PP01 == wraTemp.MaxRain).time;

                                wraTemp.Max3Sum = CalcRainMax(RainAreaList, 3) == 0 ? dTotalRain : CalcRainMax(RainAreaList, 3);
                                wraTemp.Max6Sum = CalcRainMax(RainAreaList, 6) == 0 ? dTotalRain : CalcRainMax(RainAreaList, 6);
                                wraTemp.Max12Sum = CalcRainMax(RainAreaList, 12) == 0 ? dTotalRain : CalcRainMax(RainAreaList, 12);
                                wraTemp.Max24Sum = CalcRainMax(RainAreaList, 24) == 0 ? dTotalRain : CalcRainMax(RainAreaList, 24);
                                wraTemp.Max48Sum = CalcRainMax(RainAreaList, 48) == 0 ? dTotalRain : CalcRainMax(RainAreaList, 48);

                                // 計算 API (七天前期雨量) - 傳入 rainDict 大幅加速
                                wraTemp.Pre7DayRain6 = Math.Round(CalcValueP(rainDict, wraTemp.TimeStart, 0.6), 1, MidpointRounding.AwayFromZero);
                                wraTemp.Pre7DayRain7 = Math.Round(CalcValueP(rainDict, wraTemp.TimeStart, 0.7), 1, MidpointRounding.AwayFromZero);
                                wraTemp.Pre7DayRain8 = Math.Round(CalcValueP(rainDict, wraTemp.TimeStart, 0.8), 1, MidpointRounding.AwayFromZero);

                                // 計算尖零_尖峰 (運用 Dictionary O(1) 尋找)
                                DateTime tStart0 = DateTime.ParseExact(wraTemp.TimeStart.Substring(0, 8) + "00", "yyyyMMddHH", null);
                                DateTime tMaxRain = DateTime.ParseExact(wraTemp.MaxRainTime, "yyyyMMddHH", null);
                                decimal dStart0ToSpike = 0;
                                for (DateTime t = tStart0; t <= tMaxRain; t = t.AddHours(1))
                                {
                                    if (rainDict.TryGetValue(t.ToString("yyyyMMddHH"), out var d)) dStart0ToSpike += d.PP01;
                                }
                                wraTemp.CumRain = dStart0ToSpike;

                                wraTemp.RT6 = wraTemp.Pre7DayRain6 + wraTemp.CumRain;
                                wraTemp.RT7 = wraTemp.Pre7DayRain7 + wraTemp.CumRain;
                                wraTemp.RT8 = wraTemp.Pre7DayRain8 + wraTemp.CumRain;

                                // =========================================================
                                // 4. 極速寫入資料庫：使用 MERGE 替代 IF/ELSE
                                // =========================================================
                                string upsertAreaSql = @"
                                MERGE INTO WeaRainArea AS T
                                USING (SELECT @stid AS stid, @TimeStart AS TimeStart) AS S 
                                   ON T.stid = S.stid AND T.TimeStart = S.TimeStart
                                WHEN MATCHED THEN
                                    UPDATE SET TimeEnd=@TimeEnd, RainHour=@RainHour, TotalRain=@TotalRain, MaxRain=@MaxRain, 
                                               MaxRainTime=@MaxRainTime, Max3Sum=@Max3Sum, Max6Sum=@Max6Sum, Max12Sum=@Max12Sum, 
                                               Max24Sum=@Max24Sum, Max48Sum=@Max48Sum, Pre7DayRain6=@Pre7DayRain6, 
                                               Pre7DayRain7=@Pre7DayRain7, Pre7DayRain8=@Pre7DayRain8, CumRain=@CumRain, 
                                               RT6=@RT6, RT7=@RT7, RT8=@RT8
                                WHEN NOT MATCHED THEN
                                    INSERT (stid, TimeStart, TimeEnd, RainHour, TotalRain, MaxRain, MaxRainTime, Max3Sum, Max6Sum, 
                                            Max12Sum, Max24Sum, Max48Sum, Pre7DayRain6, Pre7DayRain7, Pre7DayRain8, CumRain, RT6, RT7, RT8)
                                    VALUES (@stid, @TimeStart, @TimeEnd, @RainHour, @TotalRain, @MaxRain, @MaxRainTime, @Max3Sum, @Max6Sum, 
                                            @Max12Sum, @Max24Sum, @Max48Sum, @Pre7DayRain6, @Pre7DayRain7, @Pre7DayRain8, @CumRain, @RT6, @RT7, @RT8);";

                                dbDapper.Execute(upsertAreaSql, wraTemp);

                                // 清空狀態，尋找下一場雨
                                sRainAreaS = "";
                                sRainAreaE = "";
                            }
                        }
                    }

                    // =========================================================
                    // 5. 更新 RainAreaSeq (一次性 SQL 更新，消滅迴圈)
                    // =========================================================
                    string updateSeqSql = @"
                        WITH CTE AS (
                            SELECT stid, TimeStart, RainAreaSeq, 
                                   ROW_NUMBER() OVER(ORDER BY TimeStart) AS NewSeq
                            FROM WeaRainArea
                            WHERE stid = @stid
                        )
                        UPDATE CTE SET RainAreaSeq = NewSeq;";
                    dbDapper.Execute(updateSeqSql, new { stid = sStid });

                    // =========================================================
                    // 6. 註記轉檔完成 (直接使用 UPDATE)
                    // =========================================================
                    string finishSql = "UPDATE DataStaticLog SET status = '60', logtime = GETDATE() WHERE type = @Type AND key1 = @Stid";
                    dbDapper.Execute(finishSql, new { Type = sDataStaticLogType, Stid = sStid });
                }
                catch (Exception ex)
                {
                    // 發生異常，註記 90 (直接使用 UPDATE)
                    string errSql = "UPDATE DataStaticLog SET status = '90', logtime = GETDATE() WHERE type = @Type AND key1 = @Stid";
                    dbDapper.Execute(errSql, new { Type = sDataStaticLogType, Stid = sStid });
                    continue;
                }
            }

            progress.Report("所有雨場分割計算完成！");
        }

        // =========================================================
        // 輔助方法：計算連續時雨量最大值 (使用滑動視窗法優化)
        // =========================================================
        private decimal CalcRainMax(List<WeaRainData> wrdList, int MaxCon)
        {
            if (wrdList.Count < MaxCon) return 0;

            decimal maxResult = 0;
            decimal currentSum = 0;

            // 先算出第一個 Window 的總和
            for (int i = 0; i < MaxCon; i++) currentSum += wrdList[i].PP01;
            maxResult = currentSum;

            // 接著往後滑動，加新的、減舊的 (消滅巢狀迴圈)
            for (int i = MaxCon; i < wrdList.Count; i++)
            {
                currentSum = currentSum + wrdList[i].PP01 - wrdList[i - MaxCon].PP01;
                if (currentSum > maxResult) maxResult = currentSum;
            }

            return maxResult;
        }

        // =========================================================
        // 輔助方法：計算七天前期雨量 (引入 Dictionary 極速查詢)
        // =========================================================
        private decimal CalcValueP(Dictionary<string, WeaRainData> rainDict, string timeStart, double dEffData)
        {
            decimal StartToSpikeSum = 0;
            DateTime dtPStart = DateTime.ParseExact(timeStart.Substring(0, 8), "yyyyMMdd", null);

            for (int i = 1; i <= 7; i++)
            {
                dtPStart = dtPStart.AddDays(-1);
                string sPreDay = dtPStart.ToString("yyyyMMdd");

                decimal PreDaySum = 0;
                // 每日 24 小時，直接利用 Dictionary 做 O(1) 取值加總，不再進行 Where 迴圈掃描
                for (int h = 1; h <= 24; h++)
                {
                    string hrStr = sPreDay + h.ToString("D2");
                    if (rainDict.TryGetValue(hrStr, out WeaRainData d))
                    {
                        PreDaySum += d.PP01;
                    }
                }

                StartToSpikeSum += PreDaySum * Convert.ToDecimal(Math.Pow(dEffData, i));
            }

            return StartToSpikeSum;
        }



        /// <summary>
        /// 雨量資料，判斷月資料是否異常(超過連續24筆異常)
        /// </summary>
        /// <param name="wrdList">雨量資料來源</param>
        /// <param name="iCon">連續幾筆資料異常</param>
        /// <returns></returns>
        private bool CalcRainDataError(List<WeaRainData> wrdList, int iCon)
        {
            bool bResult = false;
            //decimal dResult = 0;

            //如果筆數不足，則預設為0
            if (wrdList.Count() < iCon)
            {
                return bResult;
            }

            for (int j = 0; j < wrdList.Count - 1; j++)
            {
                //累積資料不超過所有資料的筆數
                if (j + iCon <= wrdList.Count)
                {
                    //判斷是否連續24筆ERROR
                    bool bAllError = true;
                    for (int i = j; i < j + iCon; i++)
                    {
                        //有正常資料
                        if (wrdList[i].PP01old >= 0)
                        {
                            bAllError = false;
                            break;
                        }
                    }

                    //有連續
                    if (bAllError == true)
                    {
                        return true;
                    }

                }
            }

            return bResult;
        }

        private void button12Old_Click(object sender, EventArgs e)
        {
            // 1.每年第一次轉檔時，請執行以下SQL，清除已轉檔完成或是執行中未清除的資料
            // delete DataStaticLog where type = 'RainAreaToExcel'

            // 1-1.比對尚未完成的數量
            /*
                select * from (
                select distinct stid from WeaRainData  ) a
                left join DataStaticLog b on  b.type = 'RainAreaToExcel' and a.stid = b.key1
                where  (b.status != '60' or b.status is null)
                order by a.stid
             */

            // 2.Excel檔案產生路徑
            // @"c:\temp\WeaRainArea\";

            // 3.全部產生後更新至網頁主機路徑如下
            // D:\Project\M10\M10主程式\M10ApiDoc\doc\WeaRainArea

            string sDataStaticLogType = M10Const.DataStaticLogType.RainAreaToExcel;

            //取得測站名稱取得所有雨量站基本資料
            string ssql = @" select * from BasStationData ";
            List<BasStationData> lBasStationData = dbDapper.Query<BasStationData>(ssql);

            ssql = " select distinct stid from WeaRainData order by STID ";
            //只轉水保局雨量站資料
            //ssql = " select distinct stid from SoilWaterRainStation order by STID ";
            List<dynamic> StidList = dbDapper.Query(ssql);

            DateTime dttest = DateTime.Now;
            int iIndex = 0;
            foreach (var StidItem in StidList)
            {
                iIndex++;
                //雨量站
                string sStid = StidItem.stid;

                //if (sStid != "81M690") continue;

                //if (sStid == "81M690")
                //{
                //    string sss = "";
                //}
                //
                try
                {
                    //10:執行中 60:已完成
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'  ";
                    ssql = string.Format(ssql, sDataStaticLogType, sStid);

                    //因為多程式啟動判斷，寫入可能重複導致抓取資料異常，如果判斷有重複筆數則先刪除後重新跑
                    int iTC = dbDapper.QueryTotalCount(ssql);
                    if (iTC > 1)
                    {
                        string ssqdel = @" delete from DataStaticLog where type = '{0}' and key1 = '{1}'  ";
                        ssqdel = string.Format(ssqdel, sDataStaticLogType, sStid);
                        dbDapper.Execute(ssqdel);
                    }

                    DataStaticLog dsl_new = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);

                    //已經有轉檔紀錄
                    if (dsl_new == null)
                    {
                        //如果沒有轉檔紀錄，則新增一筆執行中
                        //寫入統計LOG
                        dsl_new = new DataStaticLog();
                        dsl_new.type = sDataStaticLogType;
                        dsl_new.key1 = sStid;
                        dsl_new.status = "10";
                        dsl_new.logtime = DateTime.Now;
                        dbDapper.Insert(dsl_new);

                    }
                    else
                    {
                        if (dsl_new.status == "10" || dsl_new.status == "60") //10:執行中 60:已完成
                        {
                            continue;
                        }

                        //重新更新為執行中
                        dsl_new.status = "10";
                        dbDapper.Update(dsl_new);
                    }


                    //取得雨量站歷年雨量資料
                    ssql = @" select * from WeaRainData where stid = @stid  ";
                    List<WeaRainData> wrdList = dbDapper.Query<WeaRainData>(ssql, new { stid = sStid });

                    ssql = @" select * from WeaRainArea where stid = @stid order by TimeStart ";
                    List<WeaRainArea> wrs = dbDapper.Query<WeaRainArea>(ssql, new { stid = sStid });

                    //建立表頭
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
                    List<string[]> datas = new List<string[]>();
                    foreach (WeaRainArea item in wrs)
                    {
                        ShowStatus(string.Format("[{2}/{3}]{0}-{1}", sStid, item.TimeStart, iIndex.ToString(), StidList.Count.ToString()));

                        int iTimeStart = Convert.ToInt32(item.TimeStart);
                        int iTimeEnd = Convert.ToInt32(item.TimeEnd);


                        //雨量站(1-24)時間格式與C#(0-23)不同
                        item.TimeStart = string.Format("{0}/{1}/{2} {3}:00"
                            , item.TimeStart.Substring(0, 4)
                            , item.TimeStart.Substring(4, 2)
                            , item.TimeStart.Substring(6, 2)
                            , item.TimeStart.Substring(8, 2)
                            );
                        item.TimeEnd = string.Format("{0}/{1}/{2} {3}:00"
                            , item.TimeEnd.Substring(0, 4)
                            , item.TimeEnd.Substring(4, 2)
                            , item.TimeEnd.Substring(6, 2)
                            , item.TimeEnd.Substring(8, 2)
                            );
                        item.MaxRainTime = string.Format("{0}/{1}/{2} {3}:00"
                            , item.MaxRainTime.Substring(0, 4)
                            , item.MaxRainTime.Substring(4, 2)
                            , item.MaxRainTime.Substring(6, 2)
                            , item.MaxRainTime.Substring(8, 2)
                            );

                        //取得測站名稱
                        BasStationData bsd = lBasStationData.FirstOrDefault(x => x.stid == item.stid);

                        List<string> cols = new List<string>();
                        cols.Add(item.stid);
                        cols.Add(bsd == null ? "" : bsd.stname);
                        cols.Add(item.RainAreaSeq.ToString().PadLeft(4, '0'));
                        cols.Add(item.TimeStart);
                        cols.Add(item.TimeEnd);
                        cols.Add(item.RainHour.ToString());
                        cols.Add(item.TotalRain.ToString());
                        cols.Add(item.MaxRainTime);
                        cols.Add(item.MaxRain.ToString());
                        cols.Add(item.Max3Sum.ToString());
                        cols.Add(item.Max6Sum.ToString());
                        cols.Add(item.Max12Sum.ToString());
                        cols.Add(item.Max24Sum.ToString());
                        cols.Add(item.Max48Sum.ToString());
                        cols.Add(item.Pre7DayRain6.ToString());
                        cols.Add(item.Pre7DayRain7.ToString());
                        cols.Add(item.Pre7DayRain8.ToString());
                        cols.Add(item.CumRain.ToString());
                        cols.Add(item.RT6.ToString());
                        cols.Add(item.RT7.ToString());
                        cols.Add(item.RT8.ToString());

                        //加入時雨量資料
                        List<WeaRainData> RainDataList = wrdList.Where(s => Convert.ToInt32(s.time) >= iTimeStart && Convert.ToInt32(s.time) <= iTimeEnd).ToList<WeaRainData>();

                        foreach (WeaRainData subItem in RainDataList)
                        {
                            cols.Add(subItem.PP01.ToString());
                        }

                        datas.Add(cols.ToArray());
                    }

                    //產生檔案路徑
                    //string sTempPath = Path.Combine(Server.MapPath("~/temp/"), DateTime.Now.ToString("yyyyMMdd"));
                    string sTempPath = @"c:\temp\WeaRainArea\";
                    //建立資料夾
                    Directory.CreateDirectory(sTempPath);

                    string sFileName = string.Format("{0}.xlsx", sStid);
                    string sSaveFilePath = Path.Combine(sTempPath, sFileName);

                    if (File.Exists(sSaveFilePath)) File.Delete(sSaveFilePath);

                    DataExport de = new DataExport();
                    //Boolean bSuccess = de.ExportBigDataToCsv(sSaveFilePath, dt);
                    Boolean bSuccess = de.ExportListToExcel(sSaveFilePath, head, datas);


                    Random rnd = new Random();
                    int month = rnd.Next(1, 2000);

                    //System.Threading.Thread.Sleep(month);

                    //註記已經轉檔完成
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'  ";
                    ssql = string.Format(ssql, sDataStaticLogType, sStid);

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "60";
                        dbDapper.Update(dslu);
                    }
                }
                catch (Exception ex)
                {

                    //註記轉檔失敗
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'  ";
                    ssql = string.Format(ssql, sDataStaticLogType, sStid);

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "90";
                        dbDapper.Update(dslu);
                    }


                    continue;
                }
            }




            //if (bSuccess)
            //{
            //    string filename = string.Format("WeaRainArea_{0}_{1}.xlsx", stid, DateTime.Now.ToString("yyyyMMddHH"));

            //    //ASP.NET 回應大型檔案的注意事項
            //    //http://blog.miniasp.com/post/2008/03/11/Caution-about-ASPNET-Response-a-Large-File.aspx


            //    /**** 下載檔案過大，使用特殊方法 *****
            //    HttpContext context = System.Web.HttpContext.Current;
            //    context.Response.TransmitFile(sSaveFilePath);
            //    context.Response.ContentType = "application/vnd.ms-excel";
            //    context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
            //    Response.End();
            //}
        }

        // =========================================================
        // UI 按鈕觸發事件 (非同步防卡死)
        // =========================================================
        private async void button12_Click(object sender, EventArgs e)
        {
            // 1.每年第一次轉檔時，請執行以下SQL，清除已轉檔完成或是執行中未清除的資料
            // delete DataStaticLog where type = 'RainAreaToExcel'

            // 1-1.比對尚未完成的數量
            /*
                select * from (
                select distinct stid from WeaRainData  ) a
                left join DataStaticLog b on  b.type = 'RainAreaToExcel' and a.stid = b.key1
                where  (b.status != '60' or b.status is null)
                order by a.stid
             */

            // 2.Excel檔案產生路徑
            // @"c:\temp\WeaRainArea\";

            // 3.全部產生後更新至網頁主機路徑如下
            // D:\Project\M10\M10主程式\M10ApiDoc\doc\WeaRainArea

            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            // 將進度訊息導向 MsgText (RichTextBox) 或原本的 ShowStatus
            var progress = new Progress<string>(msg => { LogToMsgText(msg); });

            try
            {
                await Task.Run(() => ProcessRainAreaToExcel(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"匯出發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
            }
        }

        // =========================================================
        // 核心匯出 Excel 邏輯 (支援多程式併發分工版)
        // =========================================================
        private void ProcessRainAreaToExcel(IProgress<string> progress)
        {
            string sDataStaticLogType = M10Const.DataStaticLogType.RainAreaToExcel;
            progress.Report("開始取得需要匯出 Excel 的雨量站清單...");

            // 1. 建立 Excel 輸出目錄 (移到迴圈外，執行一次就好)
            string sTempPath = @"c:\temp\WeaRainArea\";
            Directory.CreateDirectory(sTempPath);

            // 2. 取得所有雨量站基本資料，並轉為 Dictionary 實現 O(1) 極速查詢
            string bsdSql = "SELECT * FROM BasStationData";
            List<BasStationData> lBasStationData = dbDapper.Query<BasStationData>(bsdSql);
            Dictionary<string, string> stationNameDict = lBasStationData.ToDictionary(x => x.stid, x => x.stname);

            // 3. 只撈出尚未轉檔完成 (非 60)，且沒有其他程式正在執行中 (非 10) 的測站
            string ssql = @"
                SELECT a.STID 
                FROM (SELECT DISTINCT STID FROM WeaRainData) a
                LEFT JOIN DataStaticLog b ON b.type = @Type AND a.STID = b.key1
                WHERE b.status IS NULL OR (b.status != '60' AND b.status != '10')
                ORDER BY a.STID";
            List<dynamic> stidDynamicList = dbDapper.Query(ssql, new { Type = sDataStaticLogType });
            List<string> StidList = stidDynamicList.Select(d => (string)d.STID).ToList();

            int iIndex = 0;
            foreach (string sStid in StidList)
            {
                iIndex++;
                progress.Report($"[{sStid}] ({iIndex}/{StidList.Count}) 嘗試爭取匯出權限...");

                try
                {
                    // =========================================================
                    // 4. 極速搶佔機制 (原子操作 + 排他鎖，完全避免併發衝突)
                    // =========================================================
                    string claimSql = @"
                        BEGIN TRAN;
                        DECLARE @Claimed INT = 0;
                
                        IF NOT EXISTS (SELECT 1 FROM DataStaticLog WITH (UPDLOCK, HOLDLOCK) WHERE type = @Type AND key1 = @Stid)
                        BEGIN
                            INSERT INTO DataStaticLog (type, key1, status, logtime) VALUES (@Type, @Stid, '10', GETDATE());
                            SET @Claimed = 1;
                        END
                        ELSE
                        BEGIN
                            UPDATE DataStaticLog
                            SET status = '10', logtime = GETDATE()
                            WHERE type = @Type AND key1 = @Stid AND status NOT IN ('10', '60');
                    
                            IF @@ROWCOUNT > 0 SET @Claimed = 1;
                        END
                
                        COMMIT TRAN;
                        SELECT @Claimed;
                    ";

                    object objClaimed = dbDapper.ExecuteScale(claimSql, new { Type = sDataStaticLogType, Stid = sStid });
                    int isClaimed = Convert.ToInt32(objClaimed);

                    if (isClaimed == 0)
                    {
                        progress.Report($"  - [{sStid}] 已由其他程式匯出中或已完成，自動跳過。");
                        continue;
                    }

                    progress.Report($"[{sStid}] ({iIndex}/{StidList.Count}) 取得權限，匯出資料中...");

                    // 5. 取得該雨量站歷年雨量與雨場資料
                    string wrdSql = "SELECT [time], PP01 FROM WeaRainData WHERE stid = @stid ORDER BY [time]";
                    List<WeaRainData> wrdList = dbDapper.Query<WeaRainData>(wrdSql, new { stid = sStid });

                    // 🔥 核心優化：預先將字串時間轉換為數字，避免迴圈內重複轉換造成效能低落
                    var parsedRainData = wrdList.Select(w => new
                    {
                        TimeNum = long.Parse(w.time),
                        PP01Str = w.PP01.ToString()
                    }).ToList();

                    string wraSql = "SELECT * FROM WeaRainArea WHERE stid = @stid ORDER BY TimeStart";
                    List<WeaRainArea> wrs = dbDapper.Query<WeaRainArea>(wraSql, new { stid = sStid });

                    // 6. 建立表頭 (移至迴圈外側，不用每次宣告)
                    List<string> head = new List<string> {
                        "雨量站編號", "雨量站名稱", "場次序號", "開始降雨時間", "結束降雨時間",
                        "降雨延時", "總降雨量", "最大時雨量發生時間", "最大時雨量", "最大3時累積雨量",
                        "最大6時累積雨量", "最大12時累積雨量", "最大24時累積雨量", "最大48時累積雨量",
                        "七天前期雨量(0.6)", "七天前期雨量(0.7)", "七天前期雨量(0.8)", "尖零_尖峰",
                        "Rt(0.6)", "Rt(0.7)", "Rt(0.8)", "時雨量"
                    };

                    List<string[]> datas = new List<string[]>();

                    // O(1) 取得站名
                    string stName = stationNameDict.TryGetValue(sStid, out string name) ? name : "";

                    foreach (WeaRainArea item in wrs)
                    {
                        // 處理時間格式，使用 DateTime.ParseExact 更優雅且安全
                        long iTimeStartNum = long.Parse(item.TimeStart);
                        long iTimeEndNum = long.Parse(item.TimeEnd);

                        string fTimeStart = DateTime.ParseExact(item.TimeStart, "yyyyMMddHH", null).ToString("yyyy/MM/dd HH:00");
                        string fTimeEnd = DateTime.ParseExact(item.TimeEnd, "yyyyMMddHH", null).ToString("yyyy/MM/dd HH:00");
                        string fMaxRainTime = DateTime.ParseExact(item.MaxRainTime, "yyyyMMddHH", null).ToString("yyyy/MM/dd HH:00");

                        List<string> cols = new List<string> {
                            item.stid,
                            stName,
                            item.RainAreaSeq.ToString().PadLeft(4, '0'),
                            fTimeStart,
                            fTimeEnd,
                            item.RainHour.ToString(),
                            item.TotalRain.ToString(),
                            fMaxRainTime,
                            item.MaxRain.ToString(),
                            item.Max3Sum.ToString(),
                            item.Max6Sum.ToString(),
                            item.Max12Sum.ToString(),
                            item.Max24Sum.ToString(),
                            item.Max48Sum.ToString(),
                            item.Pre7DayRain6.ToString(),
                            item.Pre7DayRain7.ToString(),
                            item.Pre7DayRain8.ToString(),
                            item.CumRain.ToString(),
                            item.RT6.ToString(),
                            item.RT7.ToString(),
                            item.RT8.ToString()
                        };

                        // 加入時雨量資料 (利用預處理的 parsedRainData 進行極速過濾)
                        var rainDataStrs = parsedRainData
                            .Where(s => s.TimeNum >= iTimeStartNum && s.TimeNum <= iTimeEndNum)
                            .Select(s => s.PP01Str);

                        cols.AddRange(rainDataStrs);
                        datas.Add(cols.ToArray());
                    }

                    // 7. 匯出 Excel
                    string sFileName = string.Format("{0}.xlsx", sStid);
                    string sSaveFilePath = Path.Combine(sTempPath, sFileName);

                    if (File.Exists(sSaveFilePath)) File.Delete(sSaveFilePath);

                    DataExport de = new DataExport();
                    bool bSuccess = de.ExportListToExcel(sSaveFilePath, head, datas);

                    // 8. 註記轉檔完成 (直接用 UPDATE)
                    string finishSql = "UPDATE DataStaticLog SET status = '60', logtime = GETDATE() WHERE type = @Type AND key1 = @Stid";
                    dbDapper.Execute(finishSql, new { Type = sDataStaticLogType, Stid = sStid });
                }
                catch (Exception ex)
                {
                    // 發生異常，註記 90 (直接用 UPDATE)
                    string errSql = "UPDATE DataStaticLog SET status = '90', logtime = GETDATE() WHERE type = @Type AND key1 = @Stid";
                    dbDapper.Execute(errSql, new { Type = sDataStaticLogType, Stid = sStid });
                    continue;
                }
            }

            progress.Report("所有雨場 Excel 匯出完成！");
        }


        private void button13Old_Click(object sender, EventArgs e)
        {

            //1.產生路徑@"c:\temp\all.xlsx"

            //2.檔案產生後，移動到網頁主機路徑
            //D:\Project\M10\M10主程式\M10ApiDoc\doc\WeaRainArea

            // PS:如果遠端主機因為遠端會斷線，無法轉檔完成，就要把所有excel檔案移到本機，重新做轉檔

            string sTempPath = @"c:\temp\WeaRainArea\";
            //建立資料夾
            Directory.CreateDirectory(sTempPath);

            string sFileName = "all.xlsx";
            string sSaveFilePath = Path.Combine(sTempPath, sFileName);
            sSaveFilePath = @"c:\temp\" + sFileName;
            FileInfo[] fiList = new DirectoryInfo(sTempPath).GetFiles("*.xlsx", SearchOption.AllDirectories);
            DataExport de = new DataExport();

            //建立表頭
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

            List<string[]> datas = new List<string[]>();

            Boolean bSuccess = de.ExportListToExcel(sSaveFilePath, head, datas);

            //int i = 0;
            //foreach (FileInfo item in fiList)
            //{
            //    i++;
            //    ShowStatus(string.Format("({0}/{1}){2}", i.ToString(), fiList.Count().ToString(), item.Name));
            //    List<string[]> temp = de.ReadExcelToList(item.FullName);
            //    //datas.AddRange(temp);

            //    temp.RemoveAt(0);

            //    de.AppendListToExcel(sSaveFilePath, temp);
            //}

            //20210507 原本的方式很慢，調整作法，把所有的資料先彙整成List再塞入
            List<string[]> lstImpTemp = new List<string[]>();
            int i = 0;
            foreach (FileInfo item in fiList)
            {
                i++;
                ShowStatus(string.Format("[彙整所有Excel到清單中]({0}/{1}){2}", i.ToString(), fiList.Count().ToString(), item.Name));
                this.Text = string.Format("[彙整所有Excel到清單中]({0}/{1}){2}", i.ToString(), fiList.Count().ToString(), item.Name);
                List<string[]> temp = de.ReadExcelToList(item.FullName);
                //datas.AddRange(temp);

                //移除原本的表頭
                temp.RemoveAt(0);

                lstImpTemp.AddRange(temp);
            }

            //20240505 資料量太大，一次寫入會造成OutOfMomery的錯誤，所以改寫一次寫入固定筆數
            //所有的資料一次寫入Excel中
            ShowStatus(string.Format("清單中({0}筆)所有資料一次寫入，非常耗時，預估五分鐘左右。", lstImpTemp.Count()));
            this.Text = string.Format("清單中({0}筆)所有資料一次寫入，非常耗時，預估五分鐘左右。", lstImpTemp.Count());
            //de.AppendListToExcel(sSaveFilePath, lstImpTemp);

            int iWriteCount = 100000;
            while (lstImpTemp.Count != 0)
            {
                List<string[]> lstAppend1000 = lstImpTemp.Take(iWriteCount).ToList();

                //如果剩餘筆數不足，則移除剩餘的數量
                if (lstImpTemp.Count < iWriteCount) iWriteCount = lstImpTemp.Count;

                lstImpTemp.RemoveRange(0, iWriteCount);

                de.AppendListToExcel(sSaveFilePath, lstAppend1000);

                ShowStatus(string.Format("清單中({0}筆)剩餘筆數，非常耗時，預估五分鐘左右。", lstImpTemp.Count()));
            }


            ShowStatus("完成");

            ssql = "";
        }

        private async void button13_Click(object sender, EventArgs e)
        {
            //1.產生路徑@"c:\temp\all.xlsx"

            //2.檔案產生後，移動到網頁主機路徑
            //D:\Project\M10\M10主程式\M10ApiDoc\doc\WeaRainArea

            // PS:如果遠端主機因為遠端會斷線，無法轉檔完成，就要把所有excel檔案移到本機，重新做轉檔

            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            // 將進度訊息導向 MsgText (RichTextBox) 與 視窗標題
            var progress = new Progress<string>(msg =>
            {
                LogToMsgText(msg);   // 替換為您的 ShowStatus
                this.Text = msg;     // 更新 Form 的標題
            });

            try
            {
                await Task.Run(() => ProcessMergeExcel(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"合併 Excel 發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
            }
        }

        // =========================================================
        // 核心 Excel 彙整邏輯 (超低記憶體佔用版)
        // =========================================================
        private void ProcessMergeExcel(IProgress<string> progress)
        {
            string sTempPath = @"c:\temp\WeaRainArea\";
            Directory.CreateDirectory(sTempPath);

            string sFileName = "all.xlsx";
            string sSaveFilePath = @"c:\temp\" + sFileName;

            // 如果舊的大檔存在，先刪除
            if (File.Exists(sSaveFilePath)) File.Delete(sSaveFilePath);

            FileInfo[] fiList = new DirectoryInfo(sTempPath).GetFiles("*.xlsx", SearchOption.AllDirectories);
            DataExport de = new DataExport();

            // 建立表頭
            List<string> head = new List<string> {
        "雨量站編號", "雨量站名稱", "場次序號", "開始降雨時間", "結束降雨時間",
        "降雨延時", "總降雨量", "最大時雨量發生時間", "最大時雨量", "最大3時累積雨量",
        "最大6時累積雨量", "最大12時累積雨量", "最大24時累積雨量", "最大48時累積雨量",
        "七天前期雨量(0.6)", "七天前期雨量(0.7)", "七天前期雨量(0.8)", "尖零_尖峰",
        "Rt(0.6)", "Rt(0.7)", "Rt(0.8)", "時雨量"
    };

            // 1. 先產生一個只有表頭的空檔案
            progress.Report("正在建立空白的總表檔案...");
            de.ExportListToExcel(sSaveFilePath, head, new List<string[]>());

            // 2. 準備緩衝區 (Buffer)
            List<string[]> buffer = new List<string[]>();
            int batchSize = 100000; // 每累積10萬筆寫入一次
            int i = 0;
            int totalRows = 0;

            foreach (FileInfo item in fiList)
            {
                i++;
                progress.Report($"[合併中] ({i}/{fiList.Length}) 讀取 {item.Name}...");

                try
                {
                    // 讀取單一檔案
                    List<string[]> temp = de.ReadExcelToList(item.FullName);

                    // 確保檔案內有資料 (至少要有表頭 + 一筆資料)
                    if (temp != null && temp.Count > 1)
                    {
                        temp.RemoveAt(0); // 移除這份檔案的表頭
                        buffer.AddRange(temp);
                        totalRows += temp.Count;
                    }

                    // 🔥 核心優化：當緩衝區達到 10 萬筆，立刻寫入並清空記憶體！
                    if (buffer.Count >= batchSize)
                    {
                        progress.Report($"[寫入硬碟] 緩衝區已滿，正在將 {buffer.Count} 筆資料寫入 Excel (請稍候)...");
                        de.AppendListToExcel(sSaveFilePath, buffer);

                        // 這是避免 OutOfMemory 的關鍵動作！釋放記憶體！
                        buffer.Clear();
                    }
                }
                catch (Exception ex)
                {
                    // 若某個檔案損壞或被鎖定，記錄下來並跳過，不讓整個程序崩潰
                    progress.Report($"[錯誤] 讀取 {item.Name} 失敗: {ex.Message}");
                }
            }

            // 3. 迴圈結束後，把緩衝區剩下的「尾數」寫入
            if (buffer.Count > 0)
            {
                progress.Report($"[寫入硬碟] 正在將最後剩餘的 {buffer.Count} 筆資料寫入 Excel...");
                de.AppendListToExcel(sSaveFilePath, buffer);
                buffer.Clear();
            }

            progress.Report($"🎉 完成！共彙整 {fiList.Length} 個檔案，總計 {totalRows} 筆資料。");
        }


        private void button14_Click(object sender, EventArgs e)
        {
            //資料補遺WeaRainData,WraRainDataHis，資料可能再轉檔的時候筆數不正確，兩邊都可以有缺漏，比較後互相補上
            ssql = @"
                    select * from (
                        select STID,count(*) ttData from WeaRainData
	                    where STID in (
		                    select distinct STID from WraRainDataHis 
	                    )
	                    group by STID
                    ) a 
                    left join (
                        select STID,count(*) ttDataHis from WraRainDataHis  group by STID
                    ) b on a.STID = b.STID 
                    where a.ttData <> b.ttDataHis
                ";

            List<dynamic> checkList = dbDapper.Query<dynamic>(ssql);

            foreach (var StidItem in checkList)
            {
                //歷史資料比較多，寫回WeaRainData
                if (StidItem.ttDataHis > StidItem.ttData)
                {
                    Application.DoEvents();
                    ssql = @"
                            select * from WraRainDataHis a 
                            left join WeaRainData b on a.STID = b.Stid and a.time = b.Time
                            where a.STID = '{0}' and b.no is null
                        ";
                    ssql = string.Format(ssql, StidItem.STID);
                    List<WraRainDataHis> TempList = dbDapper.Query<WraRainDataHis>(ssql);
                    foreach (WraRainDataHis WraRainDataHisitem in TempList)
                    {

                        //同步寫入 WeaRainData
                        WeaRainData WeaRainDataTemp = new WeaRainData();
                        WeaRainDataTemp.STID = WraRainDataHisitem.Stid;
                        WeaRainDataTemp.time = WraRainDataHisitem.Time;
                        WeaRainDataTemp.PP01 = WraRainDataHisitem.Hour;
                        WeaRainDataTemp.PP01old = WraRainDataHisitem.Hour;

                        ssql = @" select * from WeaRainData where stid = '{0}' and time = '{1}' ";
                        ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
                        WeaRainData WeaRainData_new = dbDapper.QuerySingleOrDefault<WeaRainData>(ssql);

                        //沒有轉檔紀錄
                        if (WeaRainData_new == null)
                        {
                            //如果沒有轉檔紀錄，則新增一筆
                            dbDapper.Insert(WeaRainDataTemp);
                        }
                        else//已經有轉檔紀錄
                        {
                            //更新
                            WeaRainData_new.PP01 = WeaRainDataTemp.PP01;
                            WeaRainData_new.PP01old = WeaRainDataTemp.PP01old;
                            dbDapper.Update(WeaRainData_new);
                        }
                    }
                    Application.DoEvents();
                }

                //WeaRainData比較多，寫回歷史資料
                if (StidItem.ttData > StidItem.ttDataHis)
                {
                    Application.DoEvents();
                    ssql = @"
                            select * from WeaRainData a 
                            left join WraRainDataHis b on a.STID = b.Stid and a.time = b.Time
                            where a.STID = '{0}' and b.no is null
                        ";
                    ssql = string.Format(ssql, StidItem.STID);
                    List<WeaRainData> TempList = dbDapper.Query<WeaRainData>(ssql);
                    foreach (WeaRainData WeaRainDataitem in TempList)
                    {
                        //同步寫入 WraRainDataHis
                        ssql = @" select * from WraRainDataHis where stid = '{0}' and time = '{1}' ";
                        ssql = string.Format(ssql, WeaRainDataitem.STID, WeaRainDataitem.time);
                        WraRainDataHis WraRainDataHis_new = dbDapper.QuerySingleOrDefault<WraRainDataHis>(ssql);

                        //沒有轉檔紀錄
                        if (WraRainDataHis_new == null)
                        {
                            //如果沒有轉檔紀錄，則新增一筆
                            WraRainDataHis_new = new WraRainDataHis();
                            WraRainDataHis_new.Stid = WeaRainDataitem.STID;
                            WraRainDataHis_new.Time = WeaRainDataitem.time;
                            WraRainDataHis_new.Hour = WeaRainDataitem.PP01;
                            dbDapper.Insert(WraRainDataHis_new);
                        }
                        else//已經有轉檔紀錄
                        {
                            //更新
                            WraRainDataHis_new.Hour = WeaRainDataitem.PP01;
                            dbDapper.Update(WraRainDataHis_new);
                        }
                    }
                    Application.DoEvents();
                }



            }
            ShowStatus("完成");
        }

        private void button15_Click(object sender, EventArgs e)
        {

            /* === 執行完畢 要調整RTI及RTI3資料表 要先將原本線上資料(Version= 'new')備份後，再將新轉檔資料(Version= 'temp')改為線上資料(Version= 'new')生效
             * 
             * **執行前**
             * (1-1)先調整程式中變數sVer="轉檔日期(如:20250214)"
             *    
             *    string sVer = "20250214";
             * 
             * (1-2)SQL 線上資料(Version= 'new')備份
             *    update RtiDetail  set version = '轉檔日期(如:20250214)' where version = 'new'
             *    update Rti3Detail set version = '轉檔日期(如:20250214)' where version = 'new'
             * 
             * **執行後**
             * (2)SQL 新轉檔資料(Version= 'temp')改為線上資料(Version= 'new')生效
             *    update RtiDetail  set version = 'new' where version = 'temp'
             *    update Rti3Detail  set version = 'new' where version = 'temp'             * 
             


            /*    ====執行前 請先將新版的雨場分割資料匯入RtiData2中====         
             *    20220503 改到程式中執行
             *    WeaRainArea直接匯入 RtiData2        
             *    
                INSERT INTO [dbo].[RtiData2]
                ([station],[ver],[raindelay],[Rtd6],[Rtd7],[Rtd8],[Rti6],[Rti7]
                ,[Rti8],[Rti36],[Rti37],[Rti38],[R3],[date])

                select STID,'20210502' as ver,RainHour,rt6,rt7,RT8
                ,RT6*MaxRain as rti6,RT7*MaxRain as rti7,RT8*MaxRain as rti8
                ,RT6*Max3Sum/3 as rti36,RT7*Max3Sum/3 as rti37,RT8*Max3Sum/3 as rti38
                ,max3sum,SUBSTRING(TimeStart,1,8) as date 
                from WeaRainArea 
            */

            //加入這個輔助方法，用來在記憶體中抓取對應欄位
            IEnumerable<double> GetColumnValues(List<RtiData2> list, string type, string coeff)
            {
                try
                {
                    if (type == "RTI")
                    {
                        if (coeff == "6") return list.Select(x => Convert.ToDouble(x.Rti6 ?? 0));
                        if (coeff == "7") return list.Select(x => Convert.ToDouble(x.Rti7 ?? 0));
                        if (coeff == "8") return list.Select(x => Convert.ToDouble(x.Rti8 ?? 0));
                    }
                    else if (type == "RTI3")
                    {
                        if (coeff == "6") return list.Select(x => Convert.ToDouble(x.Rti36 ?? 0));
                        if (coeff == "7") return list.Select(x => Convert.ToDouble(x.Rti37 ?? 0));
                        if (coeff == "8") return list.Select(x => Convert.ToDouble(x.Rti38 ?? 0));
                    }
                    else if (type == "RT")
                    {
                        if (coeff == "6") return list.Select(x => Convert.ToDouble(x.Rtd6 ?? 0));
                        if (coeff == "7") return list.Select(x => Convert.ToDouble(x.Rtd7 ?? 0));
                        if (coeff == "8") return list.Select(x => Convert.ToDouble(x.Rtd8 ?? 0));
                    }
                    else if (type == "R3")
                    {
                        if (coeff == "6") return list.Select(x => Convert.ToDouble(x.R3 ?? 0));
                        if (coeff == "7") return list.Select(x => Convert.ToDouble(x.R3 ?? 0));
                        if (coeff == "8") return list.Select(x => Convert.ToDouble(x.R3 ?? 0));
                    }
                }
                catch { }
                return new List<double>();
            }



            string sVer = "20260329";

            // 1. 極速檢查：確認該版本是否已經存在 RtiData2 中
            string checkSql = "SELECT COUNT(1) FROM [dbo].[RtiData2] WHERE ver = @ver";
            int dataCount = Convert.ToInt32(dbDapper.ExecuteScale(checkSql, new { ver = sVer }));

            // 2. 判斷是否需要執行匯入
            if (dataCount == 0)
            {
                // 如果找不到資料，才執行刪除與匯入的動作
                string ssql = @"
                    -- 預防性刪除 (確保萬無一失)
                    DELETE FROM [dbo].[RtiData2] WHERE ver = @ver;

                    -- 將新版的雨場分割資料匯入 RtiData2 中
                    INSERT INTO [dbo].[RtiData2] (
                        [station], [ver], [raindelay], [Rtd6], [Rtd7], [Rtd8], 
                        [Rti6], [Rti7], [Rti8], [Rti36], [Rti37], [Rti38], [R3], [date]
                    )
                    SELECT 
                        STID, @ver AS ver, RainHour, rt6, rt7, RT8,
                        RT6 * MaxRain AS rti6, 
                        RT7 * MaxRain AS rti7, 
                        RT8 * MaxRain AS rti8,
                        RT6 * Max3Sum / 3 AS rti36, 
                        RT7 * Max3Sum / 3 AS rti37, 
                        RT8 * Max3Sum / 3 AS rti38,
                        max3sum, 
                        SUBSTRING(TimeStart, 1, 8) AS date 
                    FROM WeaRainArea;
                ";
                dbDapper.Execute(ssql, new { ver = sVer });

                // progress.Report($"版本 {sVer} 匯入 RtiData2 完成！");
            }


            List<RtiDetail> rdList = new List<RtiDetail>();

            ssql = " select distinct stid from WeaRainArea order by STID ";
            List<dynamic> StidList = dbDapper.Query(ssql);

            DateTime dttest = DateTime.Now;
            int iIndex = 0;
            foreach (var StidItem in StidList)
            {

                iIndex++;
                string sStid = StidItem.stid;

                // 測試限定
                // if (sStid != "00H710") continue;

                //string sDALConnStr = "M10VPN";
                //string sDALConnStr = "DbM10";
                string[] typeList = { "RTI", "RTI3" };
                string[] DelaytimeList = { "0", "1", "2", "3" };
                string[] CoefficientList = { "6", "7", "8" };

                //刪除已經存在的temp資料
                // 將兩個 DELETE 語法合併為單一批次，並使用參數化寫法 (@station, @version)
                string ssql = @"
                    DELETE FROM RtiDetail WHERE station = @station AND version = @version;
                    DELETE FROM Rti3Detail WHERE station = @station AND version = @version;
                ";

                // 透過 Dapper 傳入參數，一趟連線瞬間刪除兩張表的資料！
                dbDapper.Execute(ssql, new { station = sStid, version = "temp" });

                //foreach (string itemtype in typeList)
                //{ }

                //計算RTI
                foreach (string itemDelaytime in DelaytimeList)
                {
                    foreach (string itemCoefficient in CoefficientList)
                    {
                        ShowStatus($"[{iIndex}/{StidList.Count}][{sStid} RTI] Delaytime:{itemDelaytime} Coefficient:{itemCoefficient}");
                        //todo 延時及係數迴圈
                        RtiDetail rd = new RtiDetail();

                        ssql = @" select * from RtiData2  
                            where station = '{0}' 
                            and ver = '{1}'
                            ";

                        //延時判斷
                        if (itemDelaytime != "0")
                        {
                            ssql += " and raindelay > " + itemDelaytime;
                        }
                        ssql = string.Format(ssql, sStid, sVer);
                        List<RtiData2> wraList = dbDapper.Query<RtiData2>(ssql);

                        //取得總筆數   
                        rd.totalcount = wraList.Count();

                        //取得開始時間
                        rd.startdate = wraList.Min(s => s.date);
                        //取得結束時間
                        rd.enddate = wraList.Max(s => s.date);

                        // 20260329 -Old 改新的
                        //RtiProcOld oRtiProc = new RtiProcOld(sVer, sStid, itemDelaytime, itemCoefficient, "RTI", sDALConnStr);
                        ////進行計算
                        //oRtiProc.RtiCal();

                        RtiProc oRtiProc = new RtiProc();
                        IEnumerable<double> rtiVals = GetColumnValues(wraList, "RTI", itemCoefficient);
                        oRtiProc.RtiCal(rtiVals);

                        rd.station = sStid;
                        rd.delaytime = itemDelaytime;
                        rd.coefficient = itemCoefficient;
                        rd.version = "temp";

                        rd.rti10 = Math.Round(oRtiProc.dRTI10, 2);
                        rd.rti30 = Math.Round(oRtiProc.dRTI30, 2);
                        rd.rti50 = Math.Round(oRtiProc.dRTI50, 2);
                        rd.rti70 = Math.Round(oRtiProc.dRTI70, 2);
                        rd.rti90 = Math.Round(oRtiProc.dRTI90, 2);

                        dbDapper.Insert(rd);

                    }
                }

                //計算RTI3
                foreach (string itemDelaytime in DelaytimeList)
                {
                    foreach (string itemCoefficient in CoefficientList)
                    {
                        // 優化後：易讀性最高
                        ShowStatus($"[{iIndex}/{StidList.Count}][{sStid} RTI3] Delaytime:{itemDelaytime} Coefficient:{itemCoefficient}");
                        //todo 延時及係數迴圈
                        Rti3Detail rd = new Rti3Detail();

                        ssql = @" select * from RtiData2  
                            where station = '{0}' 
                            and ver = '{1}'
                            ";

                        //延時判斷
                        if (itemDelaytime != "0")
                        {
                            ssql += "and raindelay > " + itemDelaytime;
                        }
                        ssql = string.Format(ssql, sStid, sVer);
                        List<RtiData2> wraList = dbDapper.Query<RtiData2>(ssql);

                        //取得總筆數   
                        rd.totalcount = wraList.Count();

                        //取得開始時間
                        rd.startdate = wraList.Min(s => s.date);
                        //取得結束時間
                        rd.enddate = wraList.Max(s => s.date);

                        // 20260329 -Old 改新的
                        //RtiProcOld oRtiProc = new RtiProcOld(sVer, sStid, itemDelaytime, itemCoefficient, "RTI3", sDALConnStr);
                        //////進行計算
                        //oRtiProc.Rti3Cal();

                        RtiProc oRtiProc = new RtiProc();
                        IEnumerable<double> rti3Vals = GetColumnValues(wraList, "RTI3", itemCoefficient);
                        IEnumerable<double> rtVals = GetColumnValues(wraList, "RT", itemCoefficient);
                        IEnumerable<double> r3Vals = GetColumnValues(wraList, "R3", itemCoefficient);

                        oRtiProc.Rti3Cal(rti3Vals, rtVals, r3Vals);


                        rd.station = sStid;
                        rd.delaytime = itemDelaytime;
                        rd.coefficient = itemCoefficient;
                        rd.version = "temp";

                        rd.rti10 = Math.Round(oRtiProc.dRTI10, 2);
                        rd.rti30 = Math.Round(oRtiProc.dRTI30, 2);
                        rd.rti50 = Math.Round(oRtiProc.dRTI50, 2);
                        rd.rti70 = Math.Round(oRtiProc.dRTI70, 2);
                        rd.rti90 = Math.Round(oRtiProc.dRTI90, 2);

                        rd.rt10 = Math.Round(oRtiProc.dRT10, 2);
                        rd.rt30 = Math.Round(oRtiProc.dRT30, 2);
                        rd.rt50 = Math.Round(oRtiProc.dRT50, 2);
                        rd.rt70 = Math.Round(oRtiProc.dRT70, 2);
                        rd.rt90 = Math.Round(oRtiProc.dRT90, 2);

                        rd.r310 = Math.Round(oRtiProc.dR310, 2);
                        rd.r330 = Math.Round(oRtiProc.dR330, 2);
                        rd.r350 = Math.Round(oRtiProc.dR350, 2);
                        rd.r370 = Math.Round(oRtiProc.dR370, 2);
                        rd.r390 = Math.Round(oRtiProc.dR390, 2);

                        dbDapper.Insert(rd);

                    }
                }



            }

            ShowStatus("完成");
        }


        public RtiDetail RtiCal(string sStid, string sDelaytime, string sCoefficient)
        {
            /*             
             *    WeaRainArea直接匯入 RtiData2        
                INSERT INTO [dbo].[RtiData2]
                ([station],[ver],[raindelay],[Rtd6],[Rtd7],[Rtd8],[Rti6],[Rti7]
                ,[Rti8],[Rti36],[Rti37],[Rti38],[R3],[date])

                select STID,'20181031' as ver,RainHour,rt6,rt7,RT8
                ,RT6*MaxRain as rti6,RT7*MaxRain as rti7,RT8*MaxRain as rti8
                ,RT6*Max3Sum/3 as rti36,RT7*Max3Sum/3 as rti37,RT8*Max3Sum/3 as rti38
                ,max3sum,SUBSTRING(TimeStart,1,8) as date 
                from WeaRainArea 
             
            */

            RtiDetail rd = new RtiDetail();
            try
            {
                rd.station = sStid;
                rd.delaytime = sDelaytime;
                rd.coefficient = sCoefficient;

                DataTable dt = new DataTable();
                string sVersion = "20181026";

                ssql = @" select * from RtiData2  
                            where station = '{0}' 
                            and version = '{1}'
                        ";

                //延時判斷
                if (sDelaytime != "0")
                {
                    ssql += "and raindelay > " + sDelaytime;
                }
                ssql = string.Format(ssql, sStid, sVersion);
                List<RtiData2> wraList = dbDapper.Query<RtiData2>(ssql);

                //取得總筆數   
                rd.totalcount = wraList.Count();

                //取得開始時間
                rd.startdate = wraList.Min(s => s.date);
                //取得結束時間
                rd.enddate = wraList.Max(s => s.date);

                Application.DoEvents();


                //wraList = wraList.OrderBy(s => s.Rti7).ToList<RtiData2>();

                ////取得所有站號
                //ssql = " select * from RtiData"
                //     + "  where station = '" + sstation + "'  "
                //     + " and ver = '" + sver + "' "
                //     ;

                //if (sDelaytime != "0")
                //{
                //    ssql += " and raindelay > " + sDelaytime + " ";
                //}

                //if (stype == "RTI") ssql += " order by rti  ";
                //if (stype == "RTI3") ssql += " order by rti3  ";


                //oDal.CommandText = ssql;
                //dt.Clear();
                //dt = oDal.DataTable();
                //int iIndex = 1;
                //dt_rti.Clear();
                //foreach (DataRow dr in dt.Rows)
                //{
                //    DataRow newdr = dt_rti.NewRow();

                //    newdr["index"] = iIndex.ToString();
                //    newdr["station"] = dr["station"].ToString();
                //    if (stype == "RTI") newdr["rti"] = dr["rti"].ToString();
                //    if (stype == "RTI3") newdr["rti"] = dr["rti3"].ToString();
                //    newdr["totalcount"] = sTotalcount;
                //    dt_rti.Rows.Add(newdr);



                //    double dtotalcount = double.Parse(sTotalcount);
                //    double dIndex = double.Parse(iIndex.ToString());

                //    //前三個小時平均 * RT值
                //    double dResult = dIndex / (dtotalcount + 1) * 100;
                //    string sResult = Math.Round(dResult, 2).ToString();

                //    newdr["pas"] = sResult;

                //    iIndex++;
                //}


                ////找出最接近10%的數值
                //double dRTIA = 0;
                //double dRTIB = 0;
                //double dPASa = 0;
                //double dPASb = 0;


                ////bool bCopy = false;
                //foreach (DataRow dr in dt_rti.Rows)
                //{
                //    //暫存前一筆
                //    dPASa = dPASb;
                //    dRTIA = dRTIB;

                //    //取得目前這一筆
                //    dPASb = double.Parse(dr["pas"].ToString());
                //    dRTIB = double.Parse(dr["rti"].ToString());

                //    //數值剛好等於10，則該數值為rti10
                //    if (dPASb == 10)
                //    {
                //        dRTI10 = dRTIB;
                //    }
                //    else
                //    {
                //        //判斷兩組資料是否為10%中間
                //        if (dPASa < 10 && dPASb > 10)
                //        {
                //            //進行計算RTI10
                //            dRTI10 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (10 - dPASa)) + dRTIA;
                //            dRTI10 = Math.Round(dRTI10, 2);
                //        }
                //    }

                //    //複製除了rti10之前的資料到dt_rti90
                //    if (dPASb > 10)
                //    {
                //        DataRow drn = dt_rti90.NewRow();
                //        drn["index"] = dr["index"];
                //        drn["station"] = dr["station"];
                //        drn["rti"] = dr["rti"];
                //        drn["totalcount"] = dr["totalcount"];
                //        drn["pas"] = dr["pas"];

                //        dt_rti90.Rows.Add(drn);
                //    }
                //}

                ////dt_rti90重整
                //int irti90 = 1;
                //foreach (DataRow dr in dt_rti90.Rows)
                //{
                //    double dtotalcount = double.Parse(dt_rti90.Rows.Count.ToString());
                //    double dIndex = double.Parse(irti90.ToString());

                //    //前三個小時平均 * RT值
                //    double dResult = dIndex / (dtotalcount + 1) * 100;
                //    string sResult = Math.Round(dResult, 2).ToString();

                //    dr["index"] = irti90.ToString();
                //    dr["pas"] = sResult;

                //    irti90++;
                //}

                //dRTIA = 0;
                //dRTIB = 0;
                //dPASa = 0;
                //dPASb = 0;

                //foreach (DataRow dr in dt_rti90.Rows)
                //{
                //    //暫存前一筆
                //    dPASa = dPASb;
                //    dRTIA = dRTIB;

                //    //取得目前這一筆
                //    dPASb = double.Parse(dr["pas"].ToString());
                //    dRTIB = double.Parse(dr["rti"].ToString());


                //    if (dPASb == 90)
                //    {
                //        dRTI90 = dRTIB;
                //    }
                //    else
                //    {
                //        //判斷兩組資料是否為90%中間
                //        if (dPASa < 90 && dPASb > 90)
                //        {
                //            //進行計算RTI90
                //            dRTI90 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (90 - dPASa)) + dRTIA;
                //            dRTI90 = Math.Round(dRTI90, 2);
                //        }
                //    }



                //}


                //dRTI30 = ((dRTI90 - dRTI10) / 8 * 2) + dRTI10;
                //dRTI50 = ((dRTI90 - dRTI10) / 8 * 4) + dRTI10;
                //dRTI70 = ((dRTI90 - dRTI10) / 8 * 6) + dRTI10;

                ////小數點以下兩位進位
                //dRTI30 = Math.Round(dRTI30, 2);
                //dRTI50 = Math.Round(dRTI50, 2);
                //dRTI70 = Math.Round(dRTI70, 2);

            }
            catch (Exception)
            {

                throw;
            }

            return rd;
        }

        private void BtnGetThreeDailyData_Click(object sender, EventArgs e)
        {

            //Stockhelper.GetStockThreeTradeTse(DateTime.Now);

            Stockhelper.GetStockThreeTradeOtc(DateTime.Now);


            //

            //Stockhelper.getThreeFuturesDailyData(DateTime.Now);
            ////開始日期
            //DateTime dt = new DateTime(2019, 6, 26);
            ////結束日期
            //DateTime dtEnd = new DateTime(2019, 6, 26);

            //for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
            //{
            //    string sLineTrans = "";
            //    try
            //    {
            //        toolStripStatusLabel1.Text = string.Format("{0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
            //        Application.DoEvents();

            //        if (Stockhelper.GetStockThreeTradeTse(LoopDatetime) == false)
            //        {
            //            System.Threading.Thread.Sleep(3000);
            //            continue;
            //        }

            //        toolStripStatusLabel1.Text = string.Format("{0}-{1}", M10Const.StockType.tse, "完成");
            //        Application.DoEvents();
            //    }
            //    catch (Exception ex)
            //    {
            //        logger.Error(ex, "stock after:" + sLineTrans);
            //        System.Threading.Thread.Sleep(10000);
            //    }
            //}

            //toolStripStatusLabel1.Text = "完成";
        }

        /// <summary>
        /// 個股盤後轉CSV檔StockAfter_Tse_ToFile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button16_Click(object sender, EventArgs e)
        {
            string sPath = @"d:\Temp\StockAfter\";

            Directory.CreateDirectory(sPath);
            //開始日期
            DateTime dt = new DateTime(2007, 1, 2);
            //DateTime dt = new DateTime(2018, 1, 1);
            //結束日期
            DateTime dtEnd = DateTime.Now.AddDays(-1);

            for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
            {
                string sLineTrans = "";
                try
                {
                    toolStripStatusLabel1.Text = string.Format("盤後資料轉檔(TSE){0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    string sDate = Utils.getDateString(LoopDatetime, M10Const.DateStringType.ADT1);
                    string sUrl = string.Format(M10Const.StockAfterTseUrl, sDate);

                    string sFileName = @"TSE_{1}.csv";
                    sFileName = string.Format(sFileName, LoopDatetime.ToString("yyyyMMdd"));
                    sFileName = sPath + sFileName;

                    //判斷檔案是否存在
                    if (File.Exists(sFileName) == true)
                    {
                        continue;
                    }

                    using (WebClient wc = StockHelper.getNewWebClient())
                    {
                        wc.Encoding = Encoding.GetEncoding(950);

                        wc.DownloadFile(sUrl, sFileName);
                    }

                    toolStripStatusLabel1.Text = string.Format("盤後資料轉檔(TSE){0}-{1}", M10Const.StockType.tse, "完成");
                    Application.DoEvents();

                    toolStripStatusLabel1.Text = string.Format("盤後資料轉檔(OTC){0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    sDate = Utils.getDateString(LoopDatetime, M10Const.DateStringType.ChineseT2);
                    sUrl = string.Format(M10Const.StockAfterOtcUrl, sDate);


                    sFileName = @"OTC_{1}.csv";
                    sFileName = string.Format(sFileName, LoopDatetime.ToString("yyyyMMdd"));
                    sFileName = sPath + sFileName;

                    //判斷檔案是否存在
                    if (File.Exists(sFileName) == true)
                    {
                        continue;
                    }


                    using (WebClient wc = StockHelper.getNewWebClient())
                    {
                        wc.Encoding = Encoding.GetEncoding(950);

                        wc.DownloadFile(sUrl, sFileName);
                    }

                    toolStripStatusLabel1.Text = string.Format("盤後資料轉檔(OTC){0}-{1}", M10Const.StockType.tse, "完成");
                    Application.DoEvents();

                    System.Threading.Thread.Sleep(10000);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "stock after:" + sLineTrans);
                    System.Threading.Thread.Sleep(60000);
                }
            }
            toolStripStatusLabel1.Text = "盤後資料轉檔(TSE)完成";
        }

        /// <summary>
        /// 個股盤後轉CSV檔StockAfter_Otc_ToFile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button17_Click(object sender, EventArgs e)
        {
            string sPath = @"d:\Temp\StockAfter\";

            Directory.CreateDirectory(sPath);

            //開始日期
            DateTime dt = new DateTime(2019, 3, 2);
            //DateTime dt = new DateTime(2019, 8, 2);
            //結束日期
            DateTime dtEnd = DateTime.Now.AddDays(-1);

            for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
            {
                string sLineTrans = "";
                try
                {
                    toolStripStatusLabel1.Text = string.Format("盤後資料轉檔(OTC){0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    string sDate = Utils.getDateString(LoopDatetime, M10Const.DateStringType.ChineseT2);
                    string sUrl = string.Format(M10Const.StockAfterOtcUrl, sDate);


                    string sFileName = @"OTC_{1}.csv";
                    sFileName = string.Format(sFileName, LoopDatetime.ToString("yyyyMMdd"));
                    sFileName = sPath + sFileName;

                    //判斷檔案是否存在
                    if (File.Exists(sFileName) == true)
                    {
                        continue;
                    }


                    using (WebClient wc = StockHelper.getNewWebClient())
                    {
                        wc.Encoding = Encoding.GetEncoding(950);

                        wc.DownloadFile(sUrl, sFileName);
                    }

                    toolStripStatusLabel1.Text = string.Format("盤後資料轉檔(OTC){0}-{1}", M10Const.StockType.tse, "完成");
                    Application.DoEvents();

                    System.Threading.Thread.Sleep(3000);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "stock after:" + sLineTrans);
                    System.Threading.Thread.Sleep(60000);
                }
            }
            toolStripStatusLabel1.Text = "盤後資料轉檔(OTC)完成";
        }

        /// <summary>
        /// 個股盤後CSV匯入DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button18_Click(object sender, EventArgs e)
        {
            string sPath = @"d:\Temp\StockAfter\";
            string sPathBak = @"d:\Temp\StockAfter\bak\";
            Directory.CreateDirectory(sPath);
            Directory.CreateDirectory(sPathBak);


            FileInfo[] FileList = new DirectoryInfo(sPath).GetFiles("*.csv", SearchOption.TopDirectoryOnly);

            foreach (FileInfo item in FileList)
            {
                try
                {
                    string[] slTemp = item.Name.Replace(item.Extension, "").Split('_');
                    string sDate = slTemp[1];

                    if (File.Exists(item.FullName) == false)
                    {
                        continue;
                    }

                    string text = File.ReadAllText(item.FullName, Encoding.GetEncoding(950));


                    if (item.Name.Contains("TSE") == true)
                    {

                        List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

                        Int32 iIndex = 0;
                        foreach (string LoopItem in StringList)
                        {
                            iIndex++;

                            toolStripStatusLabel1.Text = string.Format("盤後資料匯入資料庫(TSE)-{0}-({1}/{2})", sDate
                                , iIndex.ToString(), StringList.Count.ToString());
                            Application.DoEvents();


                            string Line = LoopItem;
                            Line = Line.Replace(" ", "");
                            Line = Line.Replace("\",\"", "|");
                            Line = Line.Replace("\"", "");
                            Line = Line.Replace(",", "");
                            Line = Line.Replace("=", "");
                            string[] aCol = Line.Split('|');

                            if (aCol.Length == 16)
                            {
                                //檢核資料
                                Decimal iCheck = -1;

                                if (Decimal.TryParse(aCol[8], out iCheck) == false)
                                {
                                    continue;
                                }

                                ssql = " select * from stockafter  where stockdate = '{0}' and stockcode = '{1}'  ";
                                Stockafter sa = dbDapper.QuerySingleOrDefault<Stockafter>(string.Format(ssql, sDate, aCol[0]));

                                decimal dpricelastbuy = 0;
                                decimal.TryParse(aCol[11], out dpricelastbuy);

                                decimal dpricelastsell = 0;
                                decimal.TryParse(aCol[13], out dpricelastsell);

                                if (aCol[9] == "") aCol[9] = "X";

                                //計算昨收
                                Decimal dPriceYesterday = Stockhelper.CalcPriceYesterday(aCol[8], aCol[9], aCol[10]);

                                if (sa == null)
                                {
                                    sa = new Stockafter();
                                    sa.stockdate = sDate;
                                    sa.stocktype = M10Const.StockType.tse;
                                    sa.stockcode = aCol[0];
                                    sa.pricelast = Convert.ToDecimal(aCol[8]);
                                    sa.updown = aCol[9];
                                    sa.pricediff = aCol[10];
                                    sa.priceopen = Convert.ToDecimal(aCol[5]);
                                    sa.pricetop = Convert.ToDecimal(aCol[6]);
                                    sa.pricelow = Convert.ToDecimal(aCol[7]);
                                    sa.priceavg = 0;
                                    sa.dealnum = Convert.ToInt64(aCol[2]);
                                    sa.dealmoney = Convert.ToInt64(aCol[4]);
                                    sa.dealamount = Convert.ToInt64(aCol[3]);
                                    sa.pricelastbuy = dpricelastbuy;
                                    sa.pricelastsell = dpricelastsell;
                                    sa.publicnum = 0;
                                    sa.pricenextday = Convert.ToDecimal(aCol[8]);
                                    sa.pricenextlimittop = 0;
                                    sa.pricenextlimitlow = 0;
                                    sa.priceyesterday = dPriceYesterday;
                                    sa.updatetime = Utils.getDatatimeString();

                                    dbDapper.Insert(sa);
                                }
                                else
                                {
                                    sa.stocktype = M10Const.StockType.tse;
                                    sa.pricelast = Convert.ToDecimal(aCol[8]);
                                    sa.updown = aCol[9];
                                    sa.pricediff = aCol[10];
                                    sa.priceopen = Convert.ToDecimal(aCol[5]);
                                    sa.pricetop = Convert.ToDecimal(aCol[6]);
                                    sa.pricelow = Convert.ToDecimal(aCol[7]);
                                    sa.priceavg = 0;
                                    sa.dealnum = Convert.ToInt64(aCol[2]);
                                    sa.dealmoney = Convert.ToInt64(aCol[4]);
                                    sa.dealamount = Convert.ToInt64(aCol[3]);
                                    sa.pricelastbuy = dpricelastbuy;
                                    sa.pricelastsell = dpricelastsell;
                                    sa.publicnum = 0;
                                    sa.pricenextday = Convert.ToDecimal(aCol[8]);
                                    sa.pricenextlimittop = 0;
                                    sa.pricenextlimitlow = 0;
                                    sa.updatetime = Utils.getDatatimeString();
                                    sa.priceyesterday = dPriceYesterday;
                                    dbDapper.Update(sa);
                                }
                            }

                        }
                    }

                    if (item.Name.Contains("OTC") == true)
                    {
                        List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

                        Int32 iIndex = 0;
                        foreach (string LoopItem in StringList)
                        {
                            iIndex++;

                            toolStripStatusLabel1.Text = string.Format("盤後資料匯入資料庫(OTC)-{0}-({1}/{2})", sDate
                                , iIndex.ToString(), StringList.Count.ToString());
                            Application.DoEvents();

                            string Line = LoopItem;
                            Line = Line.Replace(" ", "");
                            Line = Line.Replace("\",\"", "|");
                            Line = Line.Replace("\"", "");
                            Line = Line.Replace(",", "");
                            Line = Line.Replace("=", "");

                            string[] aCol = Line.Split('|');

                            if (aCol.Length == 17)
                            {
                                //檢核資料
                                Decimal iCheck = -1;

                                if (Decimal.TryParse(aCol[2], out iCheck) == false)
                                {
                                    continue;
                                }

                                //資訊整理
                                string sDiff = aCol[3];
                                string sUpdown = "X";
                                string sPricediff = "0.00";
                                if (sDiff.Length > 0)
                                {
                                    string sCheck = sDiff.Substring(0, 1);
                                    if (sCheck == "+")
                                    {
                                        sUpdown = "+";
                                        sPricediff = sDiff.Replace("+", "");
                                    }
                                    if (sCheck == "-")
                                    {
                                        sUpdown = "-";
                                        sPricediff = sDiff.Replace("-", "");
                                    }
                                    if (sCheck == "0")
                                    {
                                        sUpdown = "X";
                                    }

                                    if (sCheck != "+" && sCheck != "-" && sCheck != "0")
                                    {
                                        sUpdown = "X";
                                    }
                                }

                                //計算昨收
                                Decimal dPriceYesterday = Stockhelper.CalcPriceYesterday(aCol[2], sUpdown, sPricediff);


                                ssql = " select * from stockafter  where stockdate = '{0}' and stockcode = '{1}'  ";
                                Stockafter sa = dbDapper.QuerySingleOrDefault<Stockafter>(string.Format(ssql, sDate, aCol[0]));

                                if (aCol[0] == "3226")
                                {
                                    string aaaa = string.Empty;
                                }

                                if (sa == null)
                                {
                                    sa = new Stockafter();
                                    sa.stockdate = sDate;
                                    sa.stocktype = M10Const.StockType.otc;
                                    sa.stockcode = aCol[0];
                                    sa.pricelast = Convert.ToDecimal(aCol[2]);
                                    sa.updown = sUpdown;
                                    sa.pricediff = sPricediff;
                                    sa.priceopen = Convert.ToDecimal(aCol[4]);
                                    sa.pricetop = Convert.ToDecimal(aCol[5]);
                                    sa.pricelow = Convert.ToDecimal(aCol[6]);
                                    sa.priceavg = Convert.ToDecimal(aCol[7]);
                                    sa.dealnum = Convert.ToInt64(aCol[8]);
                                    sa.dealmoney = Convert.ToInt64(aCol[9]);
                                    sa.dealamount = Convert.ToInt64(aCol[10]);
                                    sa.pricelastbuy = Convert.ToDecimal(aCol[11]);
                                    sa.pricelastsell = Convert.ToDecimal(aCol[12]);
                                    sa.publicnum = Convert.ToInt64(aCol[13]);
                                    sa.pricenextday = Convert.ToDecimal(aCol[14]);
                                    sa.pricenextlimittop = Convert.ToDecimal(aCol[15]);
                                    sa.pricenextlimitlow = Convert.ToDecimal(aCol[16]);
                                    sa.updatetime = Utils.getDatatimeString();
                                    sa.priceyesterday = dPriceYesterday;
                                    dbDapper.Insert(sa);
                                }
                                else
                                {
                                    sa.stocktype = M10Const.StockType.otc;
                                    sa.pricelast = Convert.ToDecimal(aCol[2]);
                                    sa.updown = sUpdown;
                                    sa.pricediff = sPricediff;
                                    sa.priceopen = Convert.ToDecimal(aCol[4]);
                                    sa.pricetop = Convert.ToDecimal(aCol[5]);
                                    sa.pricelow = Convert.ToDecimal(aCol[6]);
                                    sa.priceavg = Convert.ToDecimal(aCol[7]);
                                    sa.dealnum = Convert.ToInt64(aCol[8]);
                                    sa.dealmoney = Convert.ToInt64(aCol[9]);
                                    sa.dealamount = Convert.ToInt64(aCol[10]);
                                    sa.pricelastbuy = Convert.ToDecimal(aCol[11]);
                                    sa.pricelastsell = Convert.ToDecimal(aCol[12]);
                                    sa.publicnum = Convert.ToInt64(aCol[13]);
                                    sa.pricenextday = Convert.ToDecimal(aCol[14]);
                                    sa.pricenextlimittop = Convert.ToDecimal(aCol[15]);
                                    sa.pricenextlimitlow = Convert.ToDecimal(aCol[16]);
                                    sa.updatetime = Utils.getDatatimeString();
                                    sa.priceyesterday = dPriceYesterday;
                                    dbDapper.Update(sa);
                                }


                            }


                        }
                    }

                    //移動到備份區
                    string sFullRename = Path.Combine(sPathBak, item.Name);
                    if (File.Exists(sFullRename) == false)
                    {
                        item.MoveTo(sFullRename);
                    }

                }
                catch (Exception ex)
                {
                    logger.Error(ex, "盤後資料匯入資料庫 發生錯誤:" + item.FullName);
                    //System.Threading.Thread.Sleep(60000);
                }
            }

            toolStripStatusLabel1.Text = string.Format("盤後資料匯入資料庫-完成");
            Application.DoEvents();
        }

        private void Button19_Click(object sender, EventArgs e)
        {
            string sPath = @"d:\Temp\StockThreeTrade\";

            Directory.CreateDirectory(sPath);
            //開始日期
            DateTime dt = new DateTime(2012, 1, 1);
            //DateTime dt = DateTime.Now.AddDays(-1);
            //DateTime dt = new DateTime(2018, 1, 1);
            //結束日期
            DateTime dtEnd = DateTime.Now.AddDays(-1);

            for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
            {
                string sLineTrans = "";
                try
                {
                    toolStripStatusLabel1.Text = string.Format("三大法人盤後資料轉檔(TSE){0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    string sDate = Utils.getDateString(LoopDatetime, M10Const.DateStringType.ADT1);
                    string sUrl = string.Format(M10Const.StockThreeTradeTse, sDate);

                    string sFileName = @"ThreeTrade_TSE_{0}.csv";
                    sFileName = string.Format(sFileName, LoopDatetime.ToString("yyyyMMdd"));
                    sFileName = sPath + sFileName;

                    //判斷檔案是否存在
                    if (File.Exists(sFileName) == true)
                    {
                        continue;
                    }

                    using (WebClient wc = StockHelper.getNewWebClient())
                    {
                        wc.Encoding = Encoding.GetEncoding(950);

                        wc.DownloadFile(sUrl, sFileName);
                    }

                    toolStripStatusLabel1.Text = string.Format("三大法人盤後資料轉檔(TSE){0}-{1}", M10Const.StockType.tse, "完成");
                    Application.DoEvents();

                    toolStripStatusLabel1.Text = string.Format("三大法人盤後資料轉檔(OTC){0}-{1}", M10Const.StockType.otc, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    sDate = Utils.getDateString(LoopDatetime, M10Const.DateStringType.ChineseT2);
                    sUrl = string.Format(M10Const.StockThreeTradeOtc, sDate);

                    sFileName = @"ThreeTrade_OTC_{0}.csv";
                    sFileName = string.Format(sFileName, LoopDatetime.ToString("yyyyMMdd"));
                    sFileName = sPath + sFileName;

                    //判斷檔案是否存在
                    if (File.Exists(sFileName) == true)
                    {
                        continue;
                    }

                    using (WebClient wc = StockHelper.getNewWebClient())
                    {
                        wc.Encoding = Encoding.GetEncoding(950);

                        wc.DownloadFile(sUrl, sFileName);
                    }

                    toolStripStatusLabel1.Text = string.Format("三大法人盤後資料轉檔(OTC){0}-{1}", M10Const.StockType.otc, "完成");
                    Application.DoEvents();



                    System.Threading.Thread.Sleep(15000);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "三大法人盤後資料轉檔:" + sLineTrans);
                    System.Threading.Thread.Sleep(60000);
                }
            }
            toolStripStatusLabel1.Text = "盤後資料轉檔(ALL)完成";
        }

        /// <summary>
        /// 三大法人每日交易資料CSV匯入DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button20_Click(object sender, EventArgs e)
        {
            string sPath = @"d:\Temp\StockThreeTrade\";
            string sPathBak = @"d:\Temp\StockThreeTrade\bak\";
            Directory.CreateDirectory(sPath);
            Directory.CreateDirectory(sPathBak);


            FileInfo[] FileList = new DirectoryInfo(sPath).GetFiles("*.csv", SearchOption.TopDirectoryOnly);

            Int32 iIndex = 0;
            foreach (FileInfo item in FileList)
            {
                iIndex++;

                try
                {
                    string[] slTemp = item.Name.Replace(item.Extension, "").Split('_');
                    string sDate = slTemp[2];

                    if (File.Exists(item.FullName) == false)
                    {
                        continue;
                    }

                    string text = File.ReadAllText(item.FullName, Encoding.GetEncoding(950));
                    List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

                    foreach (string LoopItem in StringList)
                    {
                        string Line = LoopItem;
                        Line = Line.Replace(" ", "");
                        Line = Line.Replace("\",\"", "|");
                        Line = Line.Replace("\"", "");
                        Line = Line.Replace(",", "");
                        Line = Line.Replace("=", "");
                        string[] aCol = Line.Split('|');

                        if (aCol.Length == 1) continue;

                        //檢核資料
                        int iCheck = -1;

                        if (int.TryParse(aCol[2], out iCheck) == false)
                        {
                            continue;
                        }

                        Stockthreetrade stTemp = new Stockthreetrade();

                        if (item.Name.Contains("TSE") == true)
                        {
                            toolStripStatusLabel1.Text = string.Format("三大法人每日交易匯入資料庫(TSE)-{0}-({1}/{2})", sDate
                                , iIndex.ToString(), FileList.Length.ToString());
                            Application.DoEvents();


                            //第三版格式
                            if (aCol.Length == 19)
                            {
                                stTemp.type = M10Const.StockType.tse;
                                stTemp.foreigninv = Convert.ToInt32(aCol[4]);
                                stTemp.trustinv = Convert.ToInt32(aCol[7]);
                                stTemp.selfempinv = Convert.ToInt32(aCol[14]);
                                stTemp.threeinv = Convert.ToInt32(aCol[18]);
                            }

                            //第二版格式
                            if (aCol.Length == 16)
                            {
                                stTemp.type = M10Const.StockType.tse;
                                stTemp.foreigninv = Convert.ToInt32(aCol[4]);
                                stTemp.trustinv = Convert.ToInt32(aCol[7]);
                                stTemp.selfempinv = Convert.ToInt32(aCol[14]);
                                stTemp.threeinv = Convert.ToInt32(aCol[15]);
                            }

                            //第一版格式
                            if (aCol.Length == 12)
                            {
                                stTemp.type = M10Const.StockType.tse;
                                stTemp.foreigninv = Convert.ToInt32(aCol[4]);
                                stTemp.trustinv = Convert.ToInt32(aCol[7]);
                                stTemp.selfempinv = Convert.ToInt32(aCol[10]);
                                stTemp.threeinv = Convert.ToInt32(aCol[11]);
                            }
                        }

                        if (item.Name.Contains("OTC") == true)
                        {
                            toolStripStatusLabel1.Text = string.Format("三大法人每日交易匯入資料庫(OTC)-{0}-({1}/{2})", sDate
                                , iIndex.ToString(), FileList.Length.ToString());
                            Application.DoEvents();

                            //新版格式
                            if (aCol.Length == 24)
                            {
                                stTemp.type = M10Const.StockType.otc;
                                stTemp.foreigninv = Convert.ToInt32(aCol[4]);
                                stTemp.trustinv = Convert.ToInt32(aCol[13]);
                                stTemp.selfempinv = Convert.ToInt32(aCol[22]);
                                stTemp.threeinv = Convert.ToInt32(aCol[23]);
                            }

                            //舊版格式
                            if (aCol.Length == 16)
                            {
                                stTemp.type = M10Const.StockType.otc;
                                stTemp.foreigninv = Convert.ToInt32(aCol[4]);
                                stTemp.trustinv = Convert.ToInt32(aCol[7]);
                                stTemp.selfempinv = Convert.ToInt32(aCol[14]);
                                stTemp.threeinv = Convert.ToInt32(aCol[15]);
                            }
                        }

                        //寫入資料
                        ssql = " select * from Stockthreetrade where date = '{0}' and stockcode = '{1}' ";
                        Stockthreetrade st = dbDapper.QuerySingleOrDefault<Stockthreetrade>(string.Format(ssql, sDate, aCol[0]));
                        if (st == null)
                        {
                            st = new Stockthreetrade();
                            st.stockcode = aCol[0];
                            st.date = sDate;
                            st.type = stTemp.type;
                            st.foreigninv = stTemp.foreigninv;
                            st.trustinv = stTemp.trustinv;
                            st.selfempinv = stTemp.selfempinv;
                            st.threeinv = stTemp.threeinv;
                            st.updatetime = Utils.getDatatimeString();
                            dbDapper.Insert(st);
                        }
                        else
                        {
                            st.type = stTemp.type;
                            st.foreigninv = stTemp.foreigninv;
                            st.trustinv = stTemp.trustinv;
                            st.selfempinv = stTemp.selfempinv;
                            st.threeinv = stTemp.threeinv;
                            st.updatetime = Utils.getDatatimeString();
                            dbDapper.Update(st);
                        }

                        //完成後，移動到備份區
                        string sFullRename = Path.Combine(sPathBak, item.Name);
                        if (File.Exists(sFullRename) == false)
                        {
                            item.MoveTo(sFullRename);
                        }
                    }


                }
                catch (Exception ex)
                {
                    logger.Error(ex, "三大法人每日交易匯入資料庫 發生錯誤:" + item.FullName);
                }
            }

            toolStripStatusLabel1.Text = string.Format("三大法人每日交易匯入資料庫-完成");
            Application.DoEvents();
        }

        /// <summary>
        /// 盤後當沖資料匯入DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button17_Click_1(object sender, EventArgs e)
        {
            string sPath = @"d:\Temp\StockAfterRush\";
            string sPathBak = @"d:\Temp\StockAfterRush\bak\";
            Directory.CreateDirectory(sPath);
            Directory.CreateDirectory(sPathBak);


            FileInfo[] FileList = new DirectoryInfo(sPath).GetFiles("*.csv", SearchOption.TopDirectoryOnly);


            foreach (FileInfo item in FileList)
            {


                try
                {
                    string[] slTemp = item.Name.Replace(item.Extension, "").Split('_');
                    string sDate = slTemp[2];

                    if (File.Exists(item.FullName) == false)
                    {
                        continue;
                    }

                    string text = File.ReadAllText(item.FullName, Encoding.GetEncoding(950));
                    List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

                    Int32 iIndex = 0;
                    foreach (string LoopItem in StringList)
                    {
                        iIndex++;

                        string Line = LoopItem;
                        Line = Line.Replace(" ", "");
                        Line = Line.Replace("\",\"", "|");
                        Line = Line.Replace("\"", "");
                        Line = Line.Replace(",", "");
                        Line = Line.Replace("=", "");
                        string[] aCol = Line.Split('|');

                        if (aCol.Length == 1) continue;
                        if (aCol.Length == 3) continue;

                        //檢核資料
                        Decimal iCheck = -1;
                        if (Decimal.TryParse(aCol[1], out iCheck) == true) continue;
                        if (Decimal.TryParse(aCol[3], out iCheck) == false) continue;

                        string StockCode = "";
                        string StopRush = "";

                        StockAfterRush saTemp = new StockAfterRush();

                        if (item.Name.Contains("TSE") == true)
                        {
                            toolStripStatusLabel1.Text = string.Format("盤後當沖資料匯入DB(TSE)-{0}-({1}/{2})", sDate
                                , iIndex.ToString(), StringList.Count.ToString());
                            Application.DoEvents();

                            if (aCol.Length == 6)
                            {
                                StockCode = aCol[0];
                                StopRush = aCol[2];


                                decimal dRushDealNum = 0;
                                decimal.TryParse(aCol[3], out dRushDealNum);

                                decimal dRushMoneyBuy = 0;
                                decimal.TryParse(aCol[4], out dRushMoneyBuy);

                                decimal dRushMoneySell = 0;
                                decimal.TryParse(aCol[5], out dRushMoneySell);

                                saTemp.type = M10Const.StockType.tse;
                                saTemp.stockcode = StockCode;
                                saTemp.stoprush = StopRush;
                                saTemp.rushdealnum = Convert.ToInt64(dRushDealNum);
                                saTemp.rushmoneybuy = Convert.ToInt64(dRushMoneyBuy);
                                saTemp.rushmoneysell = Convert.ToInt64(dRushMoneySell);
                            }
                        }

                        if (item.Name.Contains("OTC") == true)
                        {
                            toolStripStatusLabel1.Text = string.Format("盤後當沖資料匯入DB(OTC)-{0}-({1}/{2})", sDate
                                , iIndex.ToString(), StringList.Count.ToString());
                            Application.DoEvents();

                            if (aCol.Length == 6)
                            {
                                StockCode = aCol[0];
                                StopRush = aCol[2] == "＊" ? "Y" : "";

                                decimal dRushDealNum = 0;
                                decimal.TryParse(aCol[3], out dRushDealNum);

                                decimal dRushMoneyBuy = 0;
                                decimal.TryParse(aCol[4], out dRushMoneyBuy);

                                decimal dRushMoneySell = 0;
                                decimal.TryParse(aCol[5], out dRushMoneySell);

                                saTemp.type = M10Const.StockType.otc;
                                saTemp.stockcode = StockCode;
                                saTemp.stoprush = StopRush;
                                saTemp.rushdealnum = Convert.ToInt64(dRushDealNum);
                                saTemp.rushmoneybuy = Convert.ToInt64(dRushMoneyBuy);
                                saTemp.rushmoneysell = Convert.ToInt64(dRushMoneySell);
                            }

                            if (aCol.Length == 5)
                            {
                                StockCode = aCol[0];
                                StopRush = "";

                                decimal dRushDealNum = 0;
                                decimal.TryParse(aCol[2], out dRushDealNum);

                                decimal dRushMoneyBuy = 0;
                                decimal.TryParse(aCol[3], out dRushMoneyBuy);

                                decimal dRushMoneySell = 0;
                                decimal.TryParse(aCol[4], out dRushMoneySell);

                                saTemp.type = M10Const.StockType.otc;
                                saTemp.stockcode = StockCode;
                                saTemp.stoprush = StopRush;
                                saTemp.rushdealnum = Convert.ToInt64(dRushDealNum);
                                saTemp.rushmoneybuy = Convert.ToInt64(dRushMoneyBuy);
                                saTemp.rushmoneysell = Convert.ToInt64(dRushMoneySell);
                            }
                        }

                        //寫入資料
                        ssql = " select * from stockafterrush  where stockdate = @stockdate and stockcode = @stockcode ";
                        var p = dbDapper.GetNewDynamicParameters();
                        p.Add("@stockdate", sDate);
                        p.Add("@stockcode", StockCode);
                        StockAfterRush sa = dbDapper.QuerySingleOrDefault<StockAfterRush>(ssql, p);

                        if (sa == null)
                        {
                            sa = new StockAfterRush();
                            sa.stockdate = sDate;
                            sa.type = saTemp.type;
                            sa.stockcode = StockCode;
                            sa.stoprush = saTemp.stoprush;
                            sa.rushdealnum = saTemp.rushdealnum;
                            sa.rushmoneybuy = saTemp.rushmoneybuy;
                            sa.rushmoneysell = saTemp.rushmoneysell;
                            sa.createdatetime = Utils.getDatatimeString();
                            sa.updatetime = Utils.getDatatimeString();

                            dbDapper.Insert(sa);
                        }
                        else
                        {
                            sa.rushdealnum = saTemp.rushdealnum;
                            sa.rushmoneybuy = saTemp.rushmoneybuy;
                            sa.rushmoneysell = saTemp.rushmoneysell;
                            sa.updatetime = Utils.getDatatimeString();

                            dbDapper.Update(sa);
                        }
                    }

                    //完成後，移動到備份區
                    string sFullRename = Path.Combine(sPathBak, item.Name);
                    if (File.Exists(sFullRename) == false)
                    {
                        item.MoveTo(sFullRename);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "盤後當沖資料匯入DB 發生錯誤:" + item.FullName);
                }
            }

            toolStripStatusLabel1.Text = string.Format("盤後當沖資料匯入DB-完成");
            Application.DoEvents();
        }

        private void Button21_Click(object sender, EventArgs e)
        {

            //開盤昨高之上選股策略
            try
            {
                ssql = @"select* from StockAfter where  dealnum / 1000 > 1000 and stockdate = '20190830' and LEN(stockcode) = 4";

                List<Stockafter> listSa = dbDapper.Query<Stockafter>(ssql);

                int i = 0;

                string sss = "";

                List<string> lsss = new List<string>();

                foreach (Stockafter item in listSa)
                {
                    i++;

                    //if (item.stockcode != "2412") continue;

                    //取得昨高資料
                    ssql = @"select top 1 * from StockAfter where stockcode = '{0}' and stockdate != '{1}' order by stockdate desc";
                    //ssql = string.Format(ssql, item.stockcode, DateTime.Now.ToString("yyyyMMdd"));
                    ssql = string.Format(ssql, item.stockcode, "20190830");
                    Stockafter saYesterday = dbDapper.QuerySingleOrDefault<Stockafter>(ssql);

                    if (saYesterday == null) continue;


                    StockRuntime sr = Stockhelper.getStockRealtimeYahooApi(item.stockcode);


                    double dOpen = 0;
                    double.TryParse(sr.open, out dOpen);
                    double dYPrice = (double)saYesterday.pricetop;
                    //double.TryParse(saYesterday.pricetop, out dYPrice);




                    toolStripStatusLabel1.Text = string.Format("[{3}/{4}]{0}({1})-收盤價：{2}", sr.n, sr.c, sr.z, i.ToString(), listSa.Count.ToString());

                    Application.DoEvents();


                    //昨高之上選股策略1--開盤價>昨高
                    if (dOpen > dYPrice)
                    {
                        lsss.Add(item.stockcode);
                    }

                    //成交量

                }



                MessageBox.Show(string.Join(",", lsss));


                //StockRuntime sr = Stockhelper.getStockRealtimeYahooApi(StockCodeTextBox.Text);

                //toolStripStatusLabel1.Text = string.Format("[{3}/{4}]{0}({1})-開盤價價：{2}", sr.n, sr.c, sr.open, i.ToString(), i.ToString());



                //Application.DoEvents();

            }
            catch (Exception ex)
            {


            }
        }

        /// <summary>
        /// 氣象局歷史資料轉檔
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button22Old_Click(object sender, EventArgs e)
        {
            //20250218 新版轉檔內容

            //(1-1)每年第一次轉檔時，請執行以下SQL，清除已轉檔完成或是執行中未清除的資料
            // delete DataStaticLog where type = 'WeaRainTransHisSTID' and status in ('10','60')
            //2.直接執行多個程式一起跑

            //3.檢核是否有轉檔失敗，如果有轉檔失敗則要刪除失敗紀錄重新轉檔或者找出轉檔失敗原因
            /* 
             * 3-1檢核是否有轉檔失敗
             * SQL:
             *  select * from  DataStaticLog where type = '109WeaRainTrans' and status = '90'
             * 
             * 3-2刪除失敗紀錄
             * SQL:
                    delete DataStaticLog where type = '109WeaRainTrans' and status = '90'
                
               3-3重新轉檔補上失敗，如果再失敗則找出失敗原因
             * */

            //4.最後比對是否有全部轉檔成功到歷史區
            /*  1.SQL:比對筆數
                    select * from (
                        select STID,count(*) ttData from WeaRainData group by STID
                    ) a 
                    left join (
                        select STID,count(*) ttDataHis from WeaRainDataHis  group by STID
                    ) b on a.STID = b.STID 
                    where a.ttData <> b.ttDataHis
                2.SQL:確認轉檔是否都完成
                    select * from WeaRainDataHis where TransYN <> 'Y'
             * */

            string sStid = "";

            ShowStatus(string.Format("正在統計尚未轉檔到歷史資料的雨量站...."));
            //取得所有雨量站
            string ssqlMain = @"
                        select * from (
                            select distinct STID from WeaRainData 
                        ) a
                        left join DataStaticLog b  on b.type = 'WeaRainTransHisSTID' and b.key1 = a.STID and b.status = '60'
                        where  b.no is null
                        order by STID
                    ";
            List<dynamic> TransList = dbDapper.Query(ssqlMain);
            Boolean bFirst = true;

            while (true)
            {
                try
                {


                    //第一次判斷，且已經沒有需要轉檔，則程式停止
                    if (bFirst = true && TransList.Count() == 0)
                    {
                        break;
                    }

                    //非第一次判斷
                    bFirst = false;



                    //剩餘總筆數，使用Random取得資料避免重複執行問題
                    int iRandom = new Random().Next(0, TransList.Count());
                    sStid = TransList[iRandom].STID;
                    TransList.RemoveAt(iRandom);
                    this.Text = string.Format("{0}-剩餘數量：{1}", sStid, TransList.Count());
                    Application.DoEvents();

                    //==判斷是否執行過
                    //10:執行中 60:已完成 90:轉檔失敗
                    string sDataStaticLogType = "WeaRainTransHisSTID";
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' ";
                    ssql = string.Format(ssql, sDataStaticLogType, sStid);
                    //20250219 避免重複刪除重複資料
                    dbDapper.Execute(string.Format(@"
                        delete FROM DataStaticLog WHERE type = '{0}' and  key1 = '{1}' and no NOT IN
                        (SELECT MAX(no) From DataStaticLog where type = '{0}' and  key1 = '{1}'
	                    GROUP BY key1)
                    ", sDataStaticLogType, sStid));

                    DataStaticLog dsl_new = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);

                    //沒有轉檔紀錄
                    if (dsl_new == null)
                    {
                        //如果沒有轉檔紀錄，則新增一筆執行中
                        //寫入統計LOG
                        dsl_new = new DataStaticLog();
                        dsl_new.type = sDataStaticLogType;
                        dsl_new.key1 = sStid;
                        dsl_new.key2 = "";
                        dsl_new.status = "10";
                        dsl_new.logtime = DateTime.Now;
                        dbDapper.Insert(dsl_new);
                    }
                    else//已經有轉檔紀錄
                    {
                        //如果執行中，則不進行該站轉檔
                        if (dsl_new.status == "10")
                        {
                            continue;
                        }

                        //重新更新為執行中
                        dsl_new.status = "10";
                        dbDapper.Update(dsl_new);
                    }

                    //1-1移除重複原始資料與歷史資料
                    //改由資料庫設定唯一，直接避免資料重複問題

                    //1-2比對原始資料與歷史資料互相補遺
                    ssql = @"
                        select * from (
                            select STID,count(*) ttData from WeaRainData where stid = '{0}' group by STID
                        ) a 
                        left join (
                            select STID,count(*) ttDataHis from WeaRainDataHis where stid = '{0}' group by STID
                        ) b on a.STID = b.STID 
                        where a.ttData <> b.ttDataHis
                    ";
                    ssql = string.Format(ssql, sStid);
                    List<dynamic> checkList = dbDapper.Query<dynamic>(ssql);

                    foreach (var StidItem in checkList)
                    {
                        //歷史資料比較多，寫回WeaRainData
                        if (StidItem.ttDataHis > StidItem.ttData)
                        {
                            Application.DoEvents();
                            ssql = @"
                                select * from WeaRainDataHis a 
                                left join WeaRainData b on a.STID = b.Stid and a.time = b.Time
                                where a.STID = '{0}' and b.no is null
                            ";
                            ssql = string.Format(ssql, StidItem.STID);
                            List<WraRainDataHis> TempList = dbDapper.Query<WraRainDataHis>(ssql);
                            foreach (WraRainDataHis WraRainDataHisitem in TempList)
                            {
                                //同步寫入 WeaRainData
                                WeaRainData WeaRainDataTemp = new WeaRainData();
                                WeaRainDataTemp.STID = WraRainDataHisitem.Stid;
                                WeaRainDataTemp.time = WraRainDataHisitem.Time;
                                WeaRainDataTemp.PP01 = WraRainDataHisitem.Hour;
                                WeaRainDataTemp.PP01old = WraRainDataHisitem.Hour;

                                ssql = @" select * from WeaRainData where stid = '{0}' and time = '{1}' ";
                                ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
                                WeaRainData WeaRainData_new = dbDapper.QuerySingleOrDefault<WeaRainData>(ssql);

                                //沒有轉檔紀錄
                                if (WeaRainData_new == null)
                                {
                                    //如果沒有轉檔紀錄，則新增一筆
                                    dbDapper.Insert(WeaRainDataTemp);
                                }
                                else//已經有轉檔紀錄
                                {
                                    //更新
                                    WeaRainData_new.PP01 = WeaRainDataTemp.PP01;
                                    WeaRainData_new.PP01old = WeaRainDataTemp.PP01old;
                                    dbDapper.Update(WeaRainData_new);
                                }
                            }
                            Application.DoEvents();
                        }

                        //WeaRainData比較多，寫回歷史資料(只寫入Hour，其他資料後面轉檔會補齊)
                        if (StidItem.ttData > StidItem.ttDataHis)
                        {
                            Application.DoEvents();
                            ssql = @"
                                select * from  WeaRainData a 
                                left join WeaRainDataHis b on a.STID = b.Stid and a.time = b.Time
                                where a.STID = '{0}' and b.no is null
                            ";
                            ssql = string.Format(ssql, StidItem.STID);
                            List<WeaRainData> TempList = dbDapper.Query<WeaRainData>(ssql);
                            foreach (WeaRainData WeaRainDataitem in TempList)
                            {
                                //同步寫入 歷史資料
                                ssql = @" select * from WeaRainDataHis where stid = '{0}' and time = '{1}' ";
                                ssql = string.Format(ssql, WeaRainDataitem.STID, WeaRainDataitem.time);
                                WeaRainDataHis WeaRainDataHis_new = dbDapper.QuerySingleOrDefault<WeaRainDataHis>(ssql);

                                //沒有轉檔紀錄
                                if (WeaRainDataHis_new == null)
                                {
                                    //如果沒有轉檔紀錄，則新增一筆
                                    WeaRainDataHis_new = new WeaRainDataHis();
                                    WeaRainDataHis_new.Stid = WeaRainDataitem.STID;
                                    WeaRainDataHis_new.Time = WeaRainDataitem.time;
                                    WeaRainDataHis_new.Hour = WeaRainDataitem.PP01;
                                    WeaRainDataHis_new.TransYN = "N";
                                    dbDapper.Insert(WeaRainDataHis_new);
                                }
                                else//已經有轉檔紀錄
                                {
                                    //更新
                                    WeaRainDataHis_new.Hour = WeaRainDataitem.PP01;
                                    dbDapper.Update(WeaRainDataHis_new);
                                }
                            }
                            Application.DoEvents();
                        }
                    }

                    //2-1執行轉檔
                    //主要轉檔程序
                    //ProcWeaRainDataHis(sStid);
                    ProcWeaRainDataHisV2025Old(sStid);


                    //註記已經轉檔完成
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' ";
                    ssql = string.Format(ssql, sDataStaticLogType, sStid);

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "60";
                        dbDapper.Update(dslu);
                    }

                    //判斷是否還有還沒檢查的資料
                    if (TransList.Count() > 0) continue;

                    //非第一次
                    if (bFirst == false && TransList.Count() == 0)
                    {
                        bFirst = true;
                        TransList = dbDapper.Query(ssqlMain);
                        continue;
                    }

                }
                catch (Exception ex)
                {
                    //發生錯誤繼續後面的轉檔，直到全部雨量站執行完畢
                    continue;
                }
            }


            this.Text = "OK-ALL";
        }

        private void ProcWeaRainDataHisV2025Old(string STID)
        {
            //雨量站
            string sStid = STID;
            //sStid = "466900";

            //string sDataStaticLogType = "109WeaRainTrans";
            //每個雨量站的每個小時都要重新計算所有數值
            ssql = @" select * from WeaRainData where STID = '{0}' order by time ";
            ssql = string.Format(ssql, sStid);

            ShowStatus(string.Format("取得雨量站[{0}]的所有氣象局雨量資料....", sStid));
            List<WeaRainData> WeaRainList = dbDapper.Query<WeaRainData>(ssql);

            //取得尚未轉檔的資料
            ssql = @" select * from WeaRainDataHis where STID = '{0}' and TransYN <> 'Y' order by time ";
            ssql = string.Format(ssql, sStid);
            ShowStatus(string.Format("取得雨量站[{0}]尚未執行的資料....", sStid));
            List<WeaRainDataHis> NoExecList = dbDapper.Query<WeaRainDataHis>(ssql);

            int iIndex1 = 0;
            foreach (WeaRainDataHis item in NoExecList)
            {
                try
                {
                    iIndex1++;

                    //目前該筆資料主要時間
                    DateTime MainTime = DateTime.ParseExact(item.Time, "yyyyMMddHH", null);

                    ShowStatus(string.Format("站號：{2}[{3}/{4}]正在轉檔：{5}", "", "", sStid
                        , iIndex1.ToString(), NoExecList.Count().ToString(), MainTime.ToString("yyyy/MM/dd-HH")));

                    //取得當月及前一個月的資料
                    List<WeaRainData> wrdList = new List<WeaRainData>();
                    //取得當月資料
                    string sNowYM = MainTime.ToString("yyyyMM");
                    //取得前月資料
                    string sPreYM = MainTime.AddMonths(-1).ToString("yyyyMM");

                    List<WeaRainData> NowYMList = WeaRainList.Where(element => element.time.StartsWith(sNowYM)).ToList<WeaRainData>();
                    List<WeaRainData> PreYMList = WeaRainList.Where(element => element.time.StartsWith(sPreYM)).ToList<WeaRainData>();
                    wrdList.AddRange(NowYMList);
                    wrdList.AddRange(PreYMList);


                    //==計算Hour3
                    decimal dHour3 = getSumRangeRainV2025Old(wrdList, item, 0, 2);

                    //==計算Hour6
                    decimal dHour6 = getSumRangeRainV2025Old(wrdList, item, 0, 5);

                    //==計算Hour12
                    decimal dHour12 = getSumRangeRainV2025Old(wrdList, item, 0, 11);

                    //==計算Hour24
                    decimal dHour24 = getSumRangeRainV2025Old(wrdList, item, 0, 23);

                    //==計算HourDayRainFall
                    string sFallDay = item.Time.Substring(0, 8);
                    List<WeaRainData> DayRainFallList = wrdList.Where(element => element.time.StartsWith(sFallDay)).ToList<WeaRainData>();
                    decimal dFallDay = 0;
                    foreach (WeaRainData DayRainFall in DayRainFallList)
                    {
                        if (DayRainFall != null)
                        {
                            dFallDay += DayRainFall.PP01;
                        }
                    }

                    //==計算RT
                    decimal dRT = 0;
                    double α = 0.7;
                    //M：七天前期雨量，前頁P的計算
                    //如：2012 / 1 / 8 5:00起始，2012 / 1 / 8 13:00尖峰
                    //P = 2012 / 1 / 8 5:00~13:00累積雨量加總，雨量不折減
                    //＋2012 / 1 / 7 日累積雨量* 折減係數α＋2012 / 1 / 6 日累積雨量* 折減係數α 2
                    //＋2012 / 1 / 5 日累積雨量* 折減係數α 3 ＋2012 / 1 / 4 日累積雨量* 折減係數α 4
                    //＋2012 / 1 / 3 日累積雨量* 折減係數α 5 ＋2012 / 1 / 2 日累積雨量* 折減係數α 6
                    //＋2012 / 1 / 1 日累積雨量* 折減係數α

                    //Rt = 當下時刻~24小時內雨量加總 + 25hr~48hr雨量加總 * 0.7 + 49hr~72hr雨量加總 * 0.7 ^ 2 + .....+167hr~192hr雨量加總 * 0.7 ^ 7之總和


                    decimal dRange = getSumRangeRainV2025Old(wrdList, item, 0, 23);
                    decimal dRange1 = getSumRangeRainV2025Old(wrdList, item, 24, 47) * Convert.ToDecimal(Math.Pow(α, 1));
                    decimal dRange2 = getSumRangeRainV2025Old(wrdList, item, 48, 71) * Convert.ToDecimal(Math.Pow(α, 2));
                    decimal dRange3 = getSumRangeRainV2025Old(wrdList, item, 72, 95) * Convert.ToDecimal(Math.Pow(α, 3));
                    decimal dRange4 = getSumRangeRainV2025Old(wrdList, item, 96, 119) * Convert.ToDecimal(Math.Pow(α, 4));
                    decimal dRange5 = getSumRangeRainV2025Old(wrdList, item, 120, 143) * Convert.ToDecimal(Math.Pow(α, 5));
                    decimal dRange6 = getSumRangeRainV2025Old(wrdList, item, 144, 167) * Convert.ToDecimal(Math.Pow(α, 6));
                    decimal dRange7 = getSumRangeRainV2025Old(wrdList, item, 168, 191) * Convert.ToDecimal(Math.Pow(α, 7));

                    dRT = dRange + dRange1 + dRange2 + dRange3 + dRange4 + dRange5 + dRange6 + dRange7;


                    WeaRainDataHis wraTemp = new WeaRainDataHis();
                    wraTemp.Stid = item.Stid;
                    wraTemp.Time = MainTime.ToString("yyyyMMddHH");
                    wraTemp.Hour = item.Hour;
                    wraTemp.Hour3 = dHour3;
                    wraTemp.Hour6 = dHour6;
                    wraTemp.Hour12 = dHour12;
                    wraTemp.Hour24 = dHour24;
                    wraTemp.DayRainfall = dFallDay;
                    wraTemp.RT = dRT;
                    wraTemp.TransYN = "Y";

                    //紀錄氣象局雨量歷史資料(時間序改為微軟標準0-23為一天)
                    ssql = " select * from WeaRainDataHis where Stid = '{0}' and Time = '{1}' ";
                    ssql = string.Format(ssql, wraTemp.Stid, wraTemp.Time);
                    WeaRainDataHis wra = dbDapper.QuerySingleOrDefault<WeaRainDataHis>(ssql);
                    if (wra == null)
                    {
                        wra = wraTemp;

                        dbDapper.Insert(wra);
                    }
                    else
                    {
                        wra.Hour = wraTemp.Hour;
                        wra.Hour3 = wraTemp.Hour3;
                        wra.Hour6 = wraTemp.Hour6;
                        wra.Hour12 = wraTemp.Hour12;
                        wra.Hour24 = wraTemp.Hour24;
                        wra.DayRainfall = wraTemp.DayRainfall;
                        wra.RT = wraTemp.RT;
                        wra.TransYN = wraTemp.TransYN;
                        dbDapper.Update(wra);
                    }
                }
                catch (Exception ex)
                {
                    //註記轉檔失敗
                    //ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'    ";
                    //ssql = string.Format(ssql, sDataStaticLogType, item.no.ToString());

                    //DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    //if (dslu != null)
                    //{
                    //    dslu.status = "90";
                    //    dbDapper.Update(dslu);
                    //}
                    //continue;
                }
            }
        }

        /// <summary>
        /// 計算區間雨量總和
        /// </summary>
        /// <param name="AllList">該站號所有雨量資料</param>
        /// <param name="Main">主要雨量資料</param>
        /// <param name="iStart">計算的起始雨量區間</param>
        /// <param name="iEnd">計算的結束雨量區間</param>
        /// <returns></returns>
        private decimal getSumRangeRainV2025Old(List<WeaRainData> AllList, WeaRainDataHis Main, int iStart, int iEnd)
        {
            decimal dSum = 0;

            //目前該筆資料主要時間
            DateTime MainTime = DateTime.ParseExact(Convert.ToString(Convert.ToInt32(Main.Time)), "yyyyMMddHH", null);

            //取得起始時間
            DateTime StartTime = MainTime.AddHours(-iStart);

            //總共要計算多少次
            int iCalCount = iEnd - iStart + 1;

            for (int i = 0; i < iCalCount; i++)
            {
                DateTime CalTime = StartTime.AddHours(-i);
                int iTime = Convert.ToInt32(CalTime.ToString("yyyyMMddHH"));
                WeaRainData wrdCal = AllList.SingleOrDefault(s => s.time == iTime.ToString());

                if (wrdCal != null)
                {
                    dSum += wrdCal.PP01;
                }
            }

            return dSum;
        }

        private async void button22_Click(object sender, EventArgs e)
        {
            //20250218 新版轉檔內容

            //(1-1)每年第一次轉檔時，請執行以下SQL，清除已轉檔完成或是執行中未清除的資料
            // delete DataStaticLog where type = 'WeaRainTransHisSTID' and status in ('10','60')
            //2.直接執行多個程式一起跑

            //3.檢核是否有轉檔失敗，如果有轉檔失敗則要刪除失敗紀錄重新轉檔或者找出轉檔失敗原因
            /* 
             * 3-1檢核是否有轉檔失敗
             * SQL:
             *  select * from  DataStaticLog where type = '109WeaRainTrans' and status = '90'
             * 
             * 3-2刪除失敗紀錄
             * SQL:
                    delete DataStaticLog where type = '109WeaRainTrans' and status = '90'
                
               3-3重新轉檔補上失敗，如果再失敗則找出失敗原因
             * */

            //4.最後比對是否有全部轉檔成功到歷史區
            /*  1.SQL:比對筆數
                    select * from (
                        select STID,count(*) ttData from WeaRainData group by STID
                    ) a 
                    left join (
                        select STID,count(*) ttDataHis from WeaRainDataHis  group by STID
                    ) b on a.STID = b.STID 
                    where a.ttData <> b.ttDataHis
                2.SQL:確認轉檔是否都完成
                    select * from WeaRainDataHis where TransYN <> 'Y'
             * */

            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            var progress = new Progress<string>(msg =>
            {
                LogToMsgText(msg);
                this.Text = msg;
            });

            try
            {
                await Task.Run(() => ProcessHisDataTrans(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"轉檔發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
                this.Text = "OK-ALL";
            }
        }

        // =========================================================
        // 核心歷史資料轉檔邏輯 (支援多程式併發分工)
        // =========================================================
        private void ProcessHisDataTrans(IProgress<string> progress)
        {
            string sDataStaticLogType = "WeaRainTransHisSTID";

            progress.Report("正在取得尚未轉檔到歷史資料的雨量站....");

            // 取得所有尚未完成的雨量站清單
            // 🔥 終極合體版 SQL：先用迴圈極速抓出不重複站號，再過濾掉已完成 (60) 的測站
            string ssqlMain = @"
                -- 建立一個記憶體暫存表來存放結果
                DECLARE @DistinctStid TABLE (STID VARCHAR(50));
                DECLARE @CurrentStid VARCHAR(50);

                -- 1. 先抓出第一個 (最小的) 站號
                SELECT TOP 1 @CurrentStid = STID 
                FROM WeaRainData 
                ORDER BY STID;

                -- 2. 開始迴圈跳躍搜尋 (Index Skip Scan)
                WHILE @CurrentStid IS NOT NULL
                BEGIN
                    -- 將找到的站號存入暫存表
                    INSERT INTO @DistinctStid (STID) VALUES (@CurrentStid);

                    -- 找大於目前站號的「下一個最小站號」 (強迫 SQL 走 Index Seek)
                    SET @CurrentStid = NULL; 

                    SELECT TOP 1 @CurrentStid = STID 
                    FROM WeaRainData 
                    WHERE STID > (SELECT TOP 1 STID FROM @DistinctStid ORDER BY STID DESC) 
                    ORDER BY STID;
                END

                -- 3. 結合您原本的邏輯：從剛剛抓出的極速名單中，排除已經轉檔完成 (60) 的測站
                SELECT a.STID 
                FROM @DistinctStid a
                LEFT JOIN DataStaticLog b ON b.type = @Type AND b.key1 = a.STID AND b.status = '60'
                WHERE b.no IS NULL
                ORDER BY a.STID;
            ";

            List<dynamic> TransList = dbDapper.Query(ssqlMain, new { Type = sDataStaticLogType });
            string ssqlStatus10 = $"select * from  DataStaticLog where type = '{sDataStaticLogType}' and status in ('10')";
            int iStatus10 = dbDapper.QueryTotalCount(ssqlStatus10);

            if (TransList.Count == 0 && iStatus10 == 0)
            {
                progress.Report("檢核資料完整性後，氣象局轉檔-全部完成....");
                return;
            }
            else if (TransList.Count == 0 && iStatus10 != 0)
            {
                progress.Report($"檢核資料完整性後，氣象局轉檔-尚未完成....執行中(10)狀態筆數：{iStatus10}");
                return;
            }


            List<string> stidList = TransList.Select(d => (string)d.STID).ToList();

            // 為了模擬原本的 Random 機制 (防多程式集中撞車)，將清單洗牌
            stidList = stidList.OrderBy(x => Guid.NewGuid()).ToList();

            int totalCount = stidList.Count;
            int currentIndex = 0;

            foreach (string sStid in stidList)
            {
                currentIndex++;
                try
                {
                    progress.Report($"[{sStid}] ({currentIndex}/{totalCount}) 爭取轉檔權限...");

                    // =========================================================
                    // 1. 極速搶佔機制 (原子操作 + 排他鎖)
                    // =========================================================
                    string claimSql = @"
                        BEGIN TRAN;
                        DECLARE @Claimed INT = 0;
                
                        IF NOT EXISTS (SELECT 1 FROM DataStaticLog WITH (UPDLOCK, HOLDLOCK) WHERE type = @Type AND key1 = @Stid)
                        BEGIN
                            INSERT INTO DataStaticLog (type, key1, status, logtime) VALUES (@Type, @Stid, '10', GETDATE());
                            SET @Claimed = 1;
                        END
                        ELSE
                        BEGIN
                            UPDATE DataStaticLog
                            SET status = '10', logtime = GETDATE()
                            WHERE type = @Type AND key1 = @Stid AND status NOT IN ('10', '60');
                    
                            IF @@ROWCOUNT > 0 SET @Claimed = 1;
                        END
                
                        COMMIT TRAN;
                        SELECT @Claimed;
                    ";

                    int isClaimed = Convert.ToInt32(dbDapper.ExecuteScale(claimSql, new { Type = sDataStaticLogType, Stid = sStid }));
                    if (isClaimed == 0) continue; // 搶佔失敗，跳過換下一站

                    progress.Report($"[{sStid}] 權限取得，進行資料庫雙向補遺...");

                    // =========================================================
                    // 2. 🔥 極速雙向補遺 (利用 SQL MERGE，瞬間消滅 C# 幾萬次的迴圈)
                    // =========================================================
                    // (2-1) 將 WeaRainDataHis 補回 WeaRainData (只補缺漏)
                    progress.Report($"[{sStid}] 將 WeaRainDataHis 補回 WeaRainData...");

                    string syncToNormalSql = @"
                        INSERT INTO WeaRainData (STID, time, PP01, PP01old)
                        SELECT S.Stid, S.Time, S.Hour, S.Hour
                        FROM WeaRainDataHis S
                        WHERE S.STID = @stid
                          AND NOT EXISTS (
                              -- 檢查如果 WeaRainData 已經有了，就不要 INSERT
                              SELECT 1 FROM WeaRainData T 
                              WHERE T.STID = S.Stid AND T.time = S.Time
                          );
                    ";
                    dbDapper.Execute(syncToNormalSql, new { stid = sStid });

                    // (2-2) 將 WeaRainData 補回 WeaRainDataHis (只補缺漏)
                    string syncToHisSql = @"
                        INSERT INTO WeaRainDataHis (Stid, Time, Hour, TransYN)
                        SELECT S.STID, S.time, S.PP01, 'N'
                        FROM WeaRainData S
                        WHERE S.STID = @stid
                          AND NOT EXISTS (
                              -- 檢查如果 WeaRainDataHis 已經有了，就不要 INSERT
                              SELECT 1 FROM WeaRainDataHis T 
                              WHERE T.Stid = S.STID AND T.Time = S.time
                          );
                    ";
                    dbDapper.Execute(syncToHisSql, new { stid = sStid });

                    // =========================================================
                    // 3. 執行主要計算程式
                    // =========================================================
                    ProcWeaRainDataHisV2025(sStid, progress, currentIndex, totalCount);

                    // 4. 註記轉檔完成
                    string finishSql = "UPDATE DataStaticLog SET status = '60', logtime = GETDATE() WHERE type = @Type AND key1 = @Stid";
                    dbDapper.Execute(finishSql, new { Type = sDataStaticLogType, Stid = sStid });
                }
                catch (Exception ex)
                {
                    // 發生錯誤，註記為 90
                    string errSql = "UPDATE DataStaticLog SET status = '90', logtime = GETDATE() WHERE type = @Type AND key1 = @Stid";
                    dbDapper.Execute(errSql, new { Type = sDataStaticLogType, Stid = sStid });
                }
            }

            //最後在檢核一次是否還有沒跑完的資料
            TransList.Clear();
            TransList = dbDapper.Query(ssqlMain, new { Type = sDataStaticLogType }, commandTimeout: 300);
            iStatus10 = dbDapper.QueryTotalCount(ssqlStatus10);

            if (TransList.Count == 0 && iStatus10 == 0)
            {
                progress.Report("檢核資料完整性後，氣象局轉檔-全部完成....");
            }
            else
            {
                progress.Report($"檢核資料完整性後，氣象局轉檔-尚未完成....執行中(10)狀態筆數：{iStatus10}");
                return;
            }

        }

        private void ProcWeaRainDataHisV2025(string sStid, IProgress<string> progress, int currentIdx, int totalCount)
        {
            progress.Report($"[{sStid}] 讀取氣象局雨量資料至記憶體...");

            // 取得該站所有雨量資料
            string ssql = "SELECT [time], PP01 FROM WeaRainData WHERE STID = @stid ORDER BY [time]";
            List<WeaRainData> WeaRainList = dbDapper.Query<WeaRainData>(ssql, new { stid = sStid });

            // 🔥 核心優化：將 List 轉成 Dictionary，讓後面的 GetSumRangeRain 查詢速度變為 O(1)
            Dictionary<string, decimal> rainDict = WeaRainList.ToDictionary(r => r.time, r => r.PP01);

            // 取得尚未轉檔的資料
            ssql = "SELECT * FROM WeaRainDataHis WHERE STID = @stid AND TransYN <> 'Y' ORDER BY [Time]";
            List<WeaRainDataHis> NoExecList = dbDapper.Query<WeaRainDataHis>(ssql, new { stid = sStid });

            if (NoExecList.Count == 0) return;

            List<WeaRainDataHis> updateBatch = new List<WeaRainDataHis>();
            int totalNoExec = NoExecList.Count;

            for (int i = 0; i < totalNoExec; i++)
            {
                WeaRainDataHis item = NoExecList[i];
                DateTime MainTime = DateTime.ParseExact(item.Time, "yyyyMMddHH", null);

                // 降低 UI 更新頻率，避免畫面閃爍卡頓
                if (i % 100 == 0 || i == totalNoExec - 1)
                {
                    progress.Report($"[{sStid}] ({currentIdx}/{totalCount}) 計算進度: {i + 1}/{totalNoExec} ({MainTime:yyyy/MM/dd-HH})");
                }

                // == 極速計算各項指標 ==
                decimal dHour3 = GetSumRangeRain(rainDict, MainTime, 0, 2);
                decimal dHour6 = GetSumRangeRain(rainDict, MainTime, 0, 5);
                decimal dHour12 = GetSumRangeRain(rainDict, MainTime, 0, 11);
                decimal dHour24 = GetSumRangeRain(rainDict, MainTime, 0, 23);

                // 計算當天總雨量 (00 到 23 時)
                DateTime dayStart = new DateTime(MainTime.Year, MainTime.Month, MainTime.Day, 23, 0, 0); // 將時間推到該日的 23 點
                decimal dFallDay = GetSumRangeRain(rainDict, dayStart, 0, 23);

                // == 計算 RT ==
                double alpha = 0.7;
                decimal dRT = GetSumRangeRain(rainDict, MainTime, 0, 23)
                            + GetSumRangeRain(rainDict, MainTime, 24, 47) * (decimal)Math.Pow(alpha, 1)
                            + GetSumRangeRain(rainDict, MainTime, 48, 71) * (decimal)Math.Pow(alpha, 2)
                            + GetSumRangeRain(rainDict, MainTime, 72, 95) * (decimal)Math.Pow(alpha, 3)
                            + GetSumRangeRain(rainDict, MainTime, 96, 119) * (decimal)Math.Pow(alpha, 4)
                            + GetSumRangeRain(rainDict, MainTime, 120, 143) * (decimal)Math.Pow(alpha, 5)
                            + GetSumRangeRain(rainDict, MainTime, 144, 167) * (decimal)Math.Pow(alpha, 6)
                            + GetSumRangeRain(rainDict, MainTime, 168, 191) * (decimal)Math.Pow(alpha, 7);

                // 寫入暫存 List
                item.Hour3 = dHour3;
                item.Hour6 = dHour6;
                item.Hour12 = dHour12;
                item.Hour24 = dHour24;
                item.DayRainfall = dFallDay;
                item.RT = dRT;
                item.TransYN = "Y";

                updateBatch.Add(item);
            }

            // =========================================================
            // 🔥 終極極速寫入：SqlBulkCopy + 暫存表 + Transaction 批次更新
            // =========================================================
            progress.Report($"[{sStid}] 運算完成，正在啟動 SqlBulkCopy 巨量寫入 ({updateBatch.Count} 筆)...");

            // 1. 將 List 轉為 DataTable (SqlBulkCopy 所需格式)
            DataTable dtUpdate = new DataTable();
            dtUpdate.Columns.Add("Stid", typeof(string));
            dtUpdate.Columns.Add("Time", typeof(string));
            dtUpdate.Columns.Add("Hour", typeof(decimal));
            dtUpdate.Columns.Add("Hour3", typeof(decimal));
            dtUpdate.Columns.Add("Hour6", typeof(decimal));
            dtUpdate.Columns.Add("Hour12", typeof(decimal));
            dtUpdate.Columns.Add("Hour24", typeof(decimal));
            dtUpdate.Columns.Add("DayRainfall", typeof(decimal));
            dtUpdate.Columns.Add("RT", typeof(decimal));

            foreach (var x in updateBatch)
            {
                dtUpdate.Rows.Add(x.Stid, x.Time, x.Hour, x.Hour3, x.Hour6, x.Hour12, x.Hour24, x.DayRainfall, x.RT);
            }

            // 2. 開啟資料庫連線並啟動 Transaction 保護
            using (SqlConnection conn = new SqlConnection(dbDapper.ConnStr)) // 請替換成您的連線字串
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // (A) 在 MSSQL 建立輕量級 #Temp 暫存表 (加入複合 Primary Key 加速 JOIN)
                        string createTempSql = @"
                            CREATE TABLE #TempWeaUpdate (
                                Stid varchar(50), 
                                [Time] varchar(20),
                                [Hour] decimal(18,4), Hour3 decimal(18,4), Hour6 decimal(18,4), 
                                Hour12 decimal(18,4), Hour24 decimal(18,4), 
                                DayRainfall decimal(18,4), RT decimal(18,4),
                                PRIMARY KEY (Stid, [Time])
                            );";
                        using (SqlCommand cmd = new SqlCommand(createTempSql, conn, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // (B) 透過 SqlBulkCopy 將資料瞬間倒進暫存表
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "#TempWeaUpdate";
                            bulkCopy.BatchSize = 10000;
                            bulkCopy.WriteToServer(dtUpdate);
                        }

                        // (C) 讓 MSSQL 透過 JOIN 進行整批 UPDATE，完成後丟棄暫存表
                        string bulkUpdateSql = @"
                            UPDATE t
                            SET t.[Hour] = tmp.[Hour], 
                                t.Hour3 = tmp.Hour3, 
                                t.Hour6 = tmp.Hour6, 
                                t.Hour12 = tmp.Hour12, 
                                t.Hour24 = tmp.Hour24, 
                                t.DayRainfall = tmp.DayRainfall, 
                                t.RT = tmp.RT,
                                t.TransYN = 'Y'
                            FROM WeaRainDataHis t
                            INNER JOIN #TempWeaUpdate tmp ON t.Stid = tmp.Stid AND t.[Time] = tmp.[Time];
                
                            DROP TABLE #TempWeaUpdate;
                        ";
                        using (SqlCommand cmd = new SqlCommand(bulkUpdateSql, conn, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // 💥 成功執行，提交交易
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // 🚨 發生錯誤，全部退回原狀！
                        transaction.Rollback();
                        throw; // 把錯誤往外拋，讓外層 Catch 接手處理 (標記狀態 90)
                    }
                }
            }
        }

        /// <summary>
        /// 計算區間雨量總和 (Dictionary 極速版)
        /// </summary>
        private decimal GetSumRangeRain(Dictionary<string, decimal> rainDict, DateTime mainTime, int iStart, int iEnd)
        {
            decimal sum = 0;
            DateTime startTime = mainTime.AddHours(-iStart);
            int count = iEnd - iStart + 1;

            for (int i = 0; i < count; i++)
            {
                string timeKey = startTime.AddHours(-i).ToString("yyyyMMddHH");

                // 瞬間 O(1) 尋找，不再需要從整個 List 中慢慢 SingleOrDefault
                if (rainDict.TryGetValue(timeKey, out decimal val))
                {
                    sum += val;
                }
            }
            return sum;
        }

        private void ProcWeaRainDataHis(string STID)
        {
            //雨量站
            string sStid = STID;
            //sStid = "466900";

            string sDataStaticLogType = "109WeaRainTrans";
            //每個雨量站的每個小時都要重新計算所有數值
            ssql = @" select * from WeaRainData where STID = '{0}' order by time ";
            ssql = string.Format(ssql, sStid);

            ShowStatus(string.Format("取得雨量站[{0}]的所有氣象局雨量資料....", sStid));
            List<WeaRainData> WeaRainList = dbDapper.Query<WeaRainData>(ssql);

            //取得尚未轉檔的資料
            //ssql = @" 
            //        select a.* from WeaRainData a
            //        left join DataStaticLog b on CAST( a.no as varchar) = b.key1 and  b.type = '109WeaRainTrans' 
            //        where a.STID = '{0}' and b.no is  null 
            //        order by a.time ";
            //for mysql
            ssql = @" 
                    select a.* from WeaRainData a
                    left join DataStaticLog b on a.no = b.key1 and  b.type = '109WeaRainTrans' 
                    where a.STID = '{0}' and b.no is  null 
                    order by a.time ";
            ssql = string.Format(ssql, sStid);
            ShowStatus(string.Format("取得雨量站[{0}]尚未執行的資料....", sStid));
            List<WeaRainData> NoExecList = dbDapper.Query<WeaRainData>(ssql);

            int iIndex1 = 0;
            foreach (WeaRainData item in NoExecList)
            {
                try
                {
                    iIndex1++;

                    //目前該筆資料主要時間
                    DateTime MainTime = DateTime.ParseExact(item.time, "yyyyMMddHH", null);

                    ShowStatus(string.Format("站號：{2}[{3}/{4}]正在轉檔：{5}", "", "", sStid
                        , iIndex1.ToString(), NoExecList.Count().ToString(), MainTime.ToString("yyyy/MM/dd-HH")));

                    //==判斷是否執行過
                    //10:執行中 60:已完成 90:轉檔失敗
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'    ";
                    ssql = string.Format(ssql, sDataStaticLogType, item.no.ToString());
                    DataStaticLog dsl_new = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);

                    //沒有轉檔紀錄
                    if (dsl_new == null)
                    {
                        //如果沒有轉檔紀錄，則新增一筆執行中
                        //寫入統計LOG
                        dsl_new = new DataStaticLog();
                        dsl_new.type = sDataStaticLogType;
                        dsl_new.key1 = item.no.ToString();
                        dsl_new.key2 = item.STID;
                        dsl_new.status = "10";
                        dsl_new.logtime = DateTime.Now;
                        dbDapper.Insert(dsl_new);
                    }
                    else//已經有轉檔紀錄
                    {
                        if (dsl_new.status == "10" || dsl_new.status == "60") //10:執行中 60:已完成
                        {
                            continue;
                        }

                        //重新更新為執行中
                        dsl_new.status = "10";
                        dbDapper.Update(dsl_new);
                    }


                    //取得當月及前一個月的資料
                    List<WeaRainData> wrdList = new List<WeaRainData>();
                    //取得當月資料
                    string sNowYM = MainTime.ToString("yyyyMM");
                    //取得前月資料
                    string sPreYM = MainTime.AddMonths(-1).ToString("yyyyMM");

                    List<WeaRainData> NowYMList = WeaRainList.Where(element => element.time.StartsWith(sNowYM)).ToList<WeaRainData>();
                    List<WeaRainData> PreYMList = WeaRainList.Where(element => element.time.StartsWith(sPreYM)).ToList<WeaRainData>();
                    wrdList.AddRange(NowYMList);
                    wrdList.AddRange(PreYMList);


                    //==計算Hour3
                    decimal dHour3 = getSumRangeRain(wrdList, item, 0, 2);

                    //==計算Hour6
                    decimal dHour6 = getSumRangeRain(wrdList, item, 0, 5);

                    //==計算Hour12
                    decimal dHour12 = getSumRangeRain(wrdList, item, 0, 11);

                    //==計算Hour24
                    decimal dHour24 = getSumRangeRain(wrdList, item, 0, 23);

                    //==計算HourDayRainFall
                    string sFallDay = item.time.Substring(0, 8);
                    List<WeaRainData> DayRainFallList = wrdList.Where(element => element.time.StartsWith(sFallDay)).ToList<WeaRainData>();
                    decimal dFallDay = 0;
                    foreach (WeaRainData DayRainFall in DayRainFallList)
                    {
                        if (DayRainFall != null)
                        {
                            dFallDay += DayRainFall.PP01;
                        }
                    }

                    //==計算RT
                    decimal dRT = 0;
                    double α = 0.7;
                    //M：七天前期雨量，前頁P的計算
                    //如：2012 / 1 / 8 5:00起始，2012 / 1 / 8 13:00尖峰
                    //P = 2012 / 1 / 8 5:00~13:00累積雨量加總，雨量不折減
                    //＋2012 / 1 / 7 日累積雨量* 折減係數α＋2012 / 1 / 6 日累積雨量* 折減係數α 2
                    //＋2012 / 1 / 5 日累積雨量* 折減係數α 3 ＋2012 / 1 / 4 日累積雨量* 折減係數α 4
                    //＋2012 / 1 / 3 日累積雨量* 折減係數α 5 ＋2012 / 1 / 2 日累積雨量* 折減係數α 6
                    //＋2012 / 1 / 1 日累積雨量* 折減係數α

                    //Rt = 當下時刻~24小時內雨量加總 + 25hr~48hr雨量加總 * 0.7 + 49hr~72hr雨量加總 * 0.7 ^ 2 + .....+167hr~192hr雨量加總 * 0.7 ^ 7之總和


                    decimal dRange = getSumRangeRain(wrdList, item, 0, 23);
                    decimal dRange1 = getSumRangeRain(wrdList, item, 24, 47) * Convert.ToDecimal(Math.Pow(α, 1));
                    decimal dRange2 = getSumRangeRain(wrdList, item, 48, 71) * Convert.ToDecimal(Math.Pow(α, 2));
                    decimal dRange3 = getSumRangeRain(wrdList, item, 72, 95) * Convert.ToDecimal(Math.Pow(α, 3));
                    decimal dRange4 = getSumRangeRain(wrdList, item, 96, 119) * Convert.ToDecimal(Math.Pow(α, 4));
                    decimal dRange5 = getSumRangeRain(wrdList, item, 120, 143) * Convert.ToDecimal(Math.Pow(α, 5));
                    decimal dRange6 = getSumRangeRain(wrdList, item, 144, 167) * Convert.ToDecimal(Math.Pow(α, 6));
                    decimal dRange7 = getSumRangeRain(wrdList, item, 168, 191) * Convert.ToDecimal(Math.Pow(α, 7));

                    dRT = dRange + dRange1 + dRange2 + dRange3 + dRange4 + dRange5 + dRange6 + dRange7;


                    WeaRainDataHis wraTemp = new WeaRainDataHis();
                    wraTemp.Stid = item.STID;
                    wraTemp.Time = MainTime.ToString("yyyyMMddHH");
                    wraTemp.Hour = item.PP01;
                    wraTemp.Hour3 = dHour3;
                    wraTemp.Hour6 = dHour6;
                    wraTemp.Hour12 = dHour12;
                    wraTemp.Hour24 = dHour24;
                    wraTemp.DayRainfall = dFallDay;
                    wraTemp.RT = dRT;

                    //紀錄氣象局雨量歷史資料(時間序改為微軟標準0-23為一天)
                    ssql = " select * from WeaRainDataHis where Stid = '{0}' and Time = '{1}' ";
                    ssql = string.Format(ssql, wraTemp.Stid, wraTemp.Time);
                    //20250218 發生重複的問題，所以判斷是否有重複資料，如果已經有重複資料則刪除
                    if (dbDapper.QueryTotalCount(ssql) > 1)
                    {
                        dbDapper.Execute(string.Format(" delete WeaRainDataHis where Stid = '{0}' and Time = '{1}' "
                            , wraTemp.Stid, wraTemp.Time));
                    }

                    WeaRainDataHis wra = dbDapper.QuerySingleOrDefault<WeaRainDataHis>(ssql);
                    if (wra == null)
                    {
                        wra = wraTemp;

                        dbDapper.Insert(wra);
                    }
                    else
                    {
                        wra.Hour = wraTemp.Hour;
                        wra.Hour3 = wraTemp.Hour3;
                        wra.Hour6 = wraTemp.Hour6;
                        wra.Hour12 = wraTemp.Hour12;
                        wra.Hour24 = wraTemp.Hour24;
                        wra.DayRainfall = wraTemp.DayRainfall;
                        wra.RT = wraTemp.RT;

                        dbDapper.Update(wra);
                    }

                    //註記已經轉檔完成
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'    ";
                    ssql = string.Format(ssql, sDataStaticLogType, item.no.ToString());

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "60";
                        dbDapper.Update(dslu);
                    }


                }
                catch (Exception ex)
                {
                    //註記轉檔失敗
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'    ";
                    ssql = string.Format(ssql, sDataStaticLogType, item.no.ToString());

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "90";
                        dbDapper.Update(dslu);
                    }
                    continue;
                }
            }
        }


        /// <summary>
        /// 計算區間雨量總和
        /// </summary>
        /// <param name="AllList">該站號所有雨量資料</param>
        /// <param name="Main">主要雨量資料</param>
        /// <param name="iStart">計算的起始雨量區間</param>
        /// <param name="iEnd">計算的結束雨量區間</param>
        /// <returns></returns>
        private decimal getSumRangeRain(List<WeaRainData> AllList, WeaRainData Main, int iStart, int iEnd)
        {
            decimal dSum = 0;

            //目前該筆資料主要時間
            DateTime MainTime = DateTime.ParseExact(Convert.ToString(Convert.ToInt32(Main.time)), "yyyyMMddHH", null);

            //取得起始時間
            DateTime StartTime = MainTime.AddHours(-iStart);

            //總共要計算多少次
            int iCalCount = iEnd - iStart + 1;

            for (int i = 0; i < iCalCount; i++)
            {
                DateTime CalTime = StartTime.AddHours(-i);
                int iTime = Convert.ToInt32(CalTime.ToString("yyyyMMddHH"));
                WeaRainData wrdCal = AllList.SingleOrDefault(s => s.time == iTime.ToString());

                if (wrdCal != null)
                {
                    dSum += wrdCal.PP01;
                }
            }

            return dSum;
        }




        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string sConstDirectoryPath = Directory.GetCurrentDirectory() + @"\SoilWaterRainDataHis";
            Directory.CreateDirectory(sConstDirectoryPath);

            sConstDirectoryPath = Directory.GetCurrentDirectory() + @"\Weather";
            Directory.CreateDirectory(sConstDirectoryPath);


            //StockRuntime sr = Stockhelper.getStockRealtimeYahooApi("2330");






            //DateTime MainTime = DateTime.ParseExact("2015080100", "yyyyMMddHH", null);

        }

        private void TUMDButton_Click(object sender, EventArgs e)
        {
            string ss = "https://cronjob.uanalyze.com.tw/fetch/IndividualStock_TUMD_Free/3008";


            List<string> ResultList = Stockhelper.GetOnlineTUMD("Q3008");




        }


        int iss = 0;
        int issidx = 0;
        private void button23_Click(object sender, EventArgs e)
        {
            //FileInfo fi = new FileInfo(@"C:\VidCoder-5.21-Portable.exe");
            //DirectoryInfo id = new DirectoryInfo(@"G:\我的雲端硬碟\Temp");
            //id.CreateSubdirectory("Name");

            //fi.CopyTo(@"G:\我的雲端硬碟\Temp\Name"+fi.Name); 


            //getStockTUMD("1315","");

            //return;

            //修改為只轉未完成的項目
            //ssql = @"select * from StockInfo where LEN(stockcode) = 4 and status = 'Y'";
            ssql = @"
                select * from StockInfo 
                left join stockLog on StockInfo.stockcode = stockLog.memo
                and stockLog.logtype = 'tumdtrans'
                and stockLog.logstatus = 'E' and stockLog.logdate = stockLog.logdatetime
                where LEN(StockInfo.stockcode) = 4 and StockInfo.status = 'Y' 
                and StockLog.no is null
            ";


            List<StockInfo> ss = dbDapper.Query<StockInfo>(ssql);
            //iss = ss.Count();
            //foreach (StockInfo item in dbDapper.Query<StockInfo>(ssql))
            //{
            //    issidx++;
            //    getStockTUMD(item.stockcode);
            //}

            List<string> StockInfoList = new List<string>();
            StockInfoList = ss.Select(o => o.stockcode).ToList<string>();




            //Random r = new Random();
            //var items = Enumerable.Range(0, 100).Select(x => r.Next(100, 200)).ToList();


            try
            {
                ParallelQueue(StockInfoList, DoWork);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"完成非同步工作，返回的執行緒 ID :  ");
                Console.WriteLine(ex.Message);

            }



        }

        private static void ParallelQueue<T>(List<T> items, Func<T, string> action)
        {
            Queue pending = new Queue(items);
            List<Task> working = new List<Task>();

            while (pending.Count + working.Count != 0)
            {
                if (pending.Count != 0 && working.Count < 12)  // Maximum tasks
                {
                    var item = pending.Dequeue(); // get item from queue
                    System.Threading.Thread.Sleep(2000);
                    Console.WriteLine(string.Format("[{0}]開始轉檔", item as string));
                    working.Add(Task.Run<string>(() => action((T)item))); // run task
                }
                else
                {
                    Task.WaitAny(working.ToArray());
                    List<Task> CompLists = working.Where(x => x.IsCompleted).ToList<Task>();
                    foreach (Task item in CompLists)
                    {
                        var tt = item as Task<string>;
                        Console.WriteLine(tt.Result);

                        //item.r
                    }
                    working.RemoveAll(x => x.IsCompleted); // remove finished tasks
                }
            }
        }

        private static string DoWork(string StockCode) // do your work here.
        {

            getStockTUMD1(StockCode, "ASYNC");
            // this is just an example
            //Task.Delay(i).Wait();
            //Console.WriteLine(i);
            return String.Format("Work[{0}]:已完成", StockCode);
        }

        public static List<string> getStockTUMD1(string StockCode, string sType)
        {
            string sStockCode = StockCode;
            List<string> ResultList = new List<string>();
            string ssql = "";
            string sTU = "";
            string sTM = "";
            string sTD = "";
            string sStockName = "";
            string sStockValue = "";
            DALDapper dbDapper = new DALDapper("Data Source=10.8.0.2;Integrated Security=False;User ID=sa;Password=pass@word1;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=m10");


            string sJson = "";
            try
            {

                string sUrl = "https://cronjob.uanalyze.com.tw/fetch/IndividualStock_TUMD/{0}";
                using (WebClient wc = StockHelper.getNewWebClient())
                {
                    sUrl = string.Format(sUrl, sStockCode);
                    sJson = wc.DownloadString(sUrl);
                }


                List<TumdOnlineData> ema5 = new List<TumdOnlineData>();
                List<TumdOnlineData> ema10 = new List<TumdOnlineData>();
                List<TumdOnlineData> ema20 = new List<TumdOnlineData>();
                List<TumdOnlineData> ema60 = new List<TumdOnlineData>();
                List<TumdOnlineData> ema120 = new List<TumdOnlineData>();
                List<TumdOnlineData> ema240 = new List<TumdOnlineData>();
                List<TumdOnlineData> defWeek = new List<TumdOnlineData>();
                List<TumdOnlineData> defMonth = new List<TumdOnlineData>();
                List<TumdOnlineData> LineTU = new List<TumdOnlineData>();
                List<TumdOnlineData> LineTM = new List<TumdOnlineData>();
                List<TumdOnlineData> LineTD = new List<TumdOnlineData>();
                //list
                //refline 5EMA

                dynamic dd = ((dynamic)JObject.Parse(sJson)).data.refline.ua80240_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    ema5.Add(ddd);
                }

                //refline 10EMA
                dd = ((dynamic)JObject.Parse(sJson)).data.refline.ua80241_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    ema10.Add(ddd);
                }
                //refline 20EMA
                dd = ((dynamic)JObject.Parse(sJson)).data.refline.ua80242_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    ema20.Add(ddd);
                }
                //refline 60EMA
                dd = ((dynamic)JObject.Parse(sJson)).data.refline.ua80243_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    ema60.Add(ddd);
                }
                //refline 120EMA
                dd = ((dynamic)JObject.Parse(sJson)).data.refline.ua80244_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    ema120.Add(ddd);
                }
                //refline 240EMA
                dd = ((dynamic)JObject.Parse(sJson)).data.refline.ua80245_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    ema240.Add(ddd);
                }
                //tooltip 周守
                dd = ((dynamic)JObject.Parse(sJson)).data.tooltips.ua80233_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    defWeek.Add(ddd);
                }
                //tooltip 月守
                dd = ((dynamic)JObject.Parse(sJson)).data.tooltips.ua80234_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    defMonth.Add(ddd);
                }

                //Line TU
                dd = ((dynamic)JObject.Parse(sJson)).data.Line.ua80230_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    LineTU.Add(ddd);
                }

                //Line TM
                dd = ((dynamic)JObject.Parse(sJson)).data.Line.ua80231_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    LineTM.Add(ddd);
                }

                //Line TD
                dd = ((dynamic)JObject.Parse(sJson)).data.Line.ua80232_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    LineTD.Add(ddd);
                }

                //取得所有時間集合
                List<string> AllDateLists = new List<string>();
                foreach (TumdOnlineData item in ema5) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in ema10) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in ema20) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in ema60) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in ema120) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in ema240) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in defWeek) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in defMonth) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in LineTU) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in LineTM) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in LineTD) AllDateLists.Add(item.date);

                AllDateLists = AllDateLists.Distinct<string>().ToList();
                AllDateLists.Sort();

                string slogtype = "tumdtrans";
                ssql = @" select * from stocklog where logtype = '{1}' and memo = '{0}' ";
                ssql = string.Format(ssql, StockCode, slogtype);
                StockLog sl = dbDapper.QuerySingleOrDefault<StockLog>(ssql);

                if (sl == null)
                {
                    sl = new StockLog();
                    sl.logtype = slogtype;
                    sl.memo = StockCode;
                    sl.logdate = AllDateLists.Count.ToString();
                    sl.logstatus = "S"; // START
                    dbDapper.Insert<StockLog>(sl);
                }

                int idx = 0;
                foreach (string QueryDate in AllDateLists)
                {
                    idx++;


                    ssql = @"
                        select * from StockTumdRec where stockcode = '{0}' and stockdate = '{1}'
                    ";
                    ssql = string.Format(ssql, StockCode, QueryDate);
                    StockTumdRec strDb = dbDapper.QuerySingleOrDefault<StockTumdRec>(ssql);


                    //string sema5 = ema5.Select(o => o.date = QueryDate).FirstOrDefault();
                    if (strDb == null)
                    {
                        strDb = new StockTumdRec();
                        strDb.stockcode = StockCode;
                        strDb.stockdate = QueryDate;
                        strDb.Ema5 = Convert.ToDecimal(ema5.Find(o => o.date == QueryDate) == null ? "0" : ema5.Find(o => o.date == QueryDate).value);
                        strDb.Ema10 = Convert.ToDecimal(ema10.Find(o => o.date == QueryDate) == null ? "0" : ema10.Find(o => o.date == QueryDate).value);
                        strDb.Ema20 = Convert.ToDecimal(ema20.Find(o => o.date == QueryDate) == null ? "0" : ema20.Find(o => o.date == QueryDate).value);
                        strDb.Ema60 = Convert.ToDecimal(ema60.Find(o => o.date == QueryDate) == null ? "0" : ema60.Find(o => o.date == QueryDate).value);
                        strDb.Ema120 = Convert.ToDecimal(ema120.Find(o => o.date == QueryDate) == null ? "0" : ema120.Find(o => o.date == QueryDate).value);
                        strDb.Ema240 = Convert.ToDecimal(ema240.Find(o => o.date == QueryDate) == null ? "0" : ema240.Find(o => o.date == QueryDate).value);
                        strDb.DefWeek = Convert.ToDecimal(defWeek.Find(o => o.date == QueryDate) == null ? "0" : defWeek.Find(o => o.date == QueryDate).value);
                        strDb.DefMonth = Convert.ToDecimal(defMonth.Find(o => o.date == QueryDate) == null ? "0" : defMonth.Find(o => o.date == QueryDate).value);
                        strDb.Tu = Convert.ToDecimal(LineTU.Find(o => o.date == QueryDate) == null ? "0" : LineTU.Find(o => o.date == QueryDate).value);
                        strDb.Tm = Convert.ToDecimal(LineTM.Find(o => o.date == QueryDate) == null ? "0" : LineTM.Find(o => o.date == QueryDate).value);
                        strDb.Td = Convert.ToDecimal(LineTD.Find(o => o.date == QueryDate) == null ? "0" : LineTD.Find(o => o.date == QueryDate).value);

                        dbDapper.Insert<StockTumdRec>(strDb);
                    }
                    else
                    {
                        strDb.Ema5 = Convert.ToDecimal(ema5.Find(o => o.date == QueryDate) == null ? "0" : ema5.Find(o => o.date == QueryDate).value);
                        strDb.Ema10 = Convert.ToDecimal(ema10.Find(o => o.date == QueryDate) == null ? "0" : ema10.Find(o => o.date == QueryDate).value);
                        strDb.Ema20 = Convert.ToDecimal(ema20.Find(o => o.date == QueryDate) == null ? "0" : ema20.Find(o => o.date == QueryDate).value);
                        strDb.Ema60 = Convert.ToDecimal(ema60.Find(o => o.date == QueryDate) == null ? "0" : ema60.Find(o => o.date == QueryDate).value);
                        strDb.Ema120 = Convert.ToDecimal(ema120.Find(o => o.date == QueryDate) == null ? "0" : ema120.Find(o => o.date == QueryDate).value);
                        strDb.Ema240 = Convert.ToDecimal(ema240.Find(o => o.date == QueryDate) == null ? "0" : ema240.Find(o => o.date == QueryDate).value);
                        strDb.DefWeek = Convert.ToDecimal(defWeek.Find(o => o.date == QueryDate) == null ? "0" : defWeek.Find(o => o.date == QueryDate).value);
                        strDb.DefMonth = Convert.ToDecimal(defMonth.Find(o => o.date == QueryDate) == null ? "0" : defMonth.Find(o => o.date == QueryDate).value);
                        strDb.Tu = Convert.ToDecimal(LineTU.Find(o => o.date == QueryDate) == null ? "0" : LineTU.Find(o => o.date == QueryDate).value);
                        strDb.Tm = Convert.ToDecimal(LineTM.Find(o => o.date == QueryDate) == null ? "0" : LineTM.Find(o => o.date == QueryDate).value);
                        strDb.Td = Convert.ToDecimal(LineTD.Find(o => o.date == QueryDate) == null ? "0" : LineTD.Find(o => o.date == QueryDate).value);

                        dbDapper.Update<StockTumdRec>(strDb);
                    }

                }



                //結束註記並記錄轉檔數量                
                sl.logstatus = "E"; // START
                sl.logdatetime = idx.ToString();
                dbDapper.Update<StockLog>(sl);


            }
            catch (Exception ex)
            {
                //logger.Error(ex, sJson);
            }

            return ResultList;
        }


        public List<string> getStockTUMD(string StockCode, string sType)
        {
            string sStockCode = StockCode;
            List<string> ResultList = new List<string>();
            string sTU = "";
            string sTM = "";
            string sTD = "";
            string sStockName = "";
            string sStockValue = "";


            string sJson = "";
            try
            {

                string sUrl = "https://cronjob.uanalyze.com.tw/fetch/IndividualStock_TUMD/{0}";
                using (WebClient wc = StockHelper.getNewWebClient())
                {
                    sUrl = string.Format(sUrl, sStockCode);
                    sJson = wc.DownloadString(sUrl);
                }


                List<TumdOnlineData> ema5 = new List<TumdOnlineData>();
                List<TumdOnlineData> ema10 = new List<TumdOnlineData>();
                List<TumdOnlineData> ema20 = new List<TumdOnlineData>();
                List<TumdOnlineData> ema60 = new List<TumdOnlineData>();
                List<TumdOnlineData> ema120 = new List<TumdOnlineData>();
                List<TumdOnlineData> ema240 = new List<TumdOnlineData>();
                List<TumdOnlineData> defWeek = new List<TumdOnlineData>();
                List<TumdOnlineData> defMonth = new List<TumdOnlineData>();
                List<TumdOnlineData> LineTU = new List<TumdOnlineData>();
                List<TumdOnlineData> LineTM = new List<TumdOnlineData>();
                List<TumdOnlineData> LineTD = new List<TumdOnlineData>();
                //list
                //refline 5EMA

                dynamic dd = ((dynamic)JObject.Parse(sJson)).data.refline.ua80240_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    ema5.Add(ddd);
                }

                //refline 10EMA
                dd = ((dynamic)JObject.Parse(sJson)).data.refline.ua80241_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    ema10.Add(ddd);
                }
                //refline 20EMA
                dd = ((dynamic)JObject.Parse(sJson)).data.refline.ua80242_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    ema20.Add(ddd);
                }
                //refline 60EMA
                dd = ((dynamic)JObject.Parse(sJson)).data.refline.ua80243_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    ema60.Add(ddd);
                }
                //refline 120EMA
                dd = ((dynamic)JObject.Parse(sJson)).data.refline.ua80244_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    ema120.Add(ddd);
                }
                //refline 240EMA
                dd = ((dynamic)JObject.Parse(sJson)).data.refline.ua80245_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    ema240.Add(ddd);
                }
                //tooltip 周守
                dd = ((dynamic)JObject.Parse(sJson)).data.tooltips.ua80233_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    defWeek.Add(ddd);
                }
                //tooltip 月守
                dd = ((dynamic)JObject.Parse(sJson)).data.tooltips.ua80234_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    defMonth.Add(ddd);
                }

                //Line TU
                dd = ((dynamic)JObject.Parse(sJson)).data.Line.ua80230_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    LineTU.Add(ddd);
                }

                //Line TM
                dd = ((dynamic)JObject.Parse(sJson)).data.Line.ua80231_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    LineTM.Add(ddd);
                }

                //Line TD
                dd = ((dynamic)JObject.Parse(sJson)).data.Line.ua80232_cp.Data;
                foreach (var child in dd.Children())
                {
                    TumdOnlineData ddd = new TumdOnlineData();
                    ddd.date = child.Name;
                    ddd.value = child.Value;

                    LineTD.Add(ddd);
                }

                //取得所有時間集合
                List<string> AllDateLists = new List<string>();
                foreach (TumdOnlineData item in ema5) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in ema10) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in ema20) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in ema60) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in ema120) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in ema240) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in defWeek) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in defMonth) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in LineTU) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in LineTM) AllDateLists.Add(item.date);
                foreach (TumdOnlineData item in LineTD) AllDateLists.Add(item.date);

                AllDateLists = AllDateLists.Distinct<string>().ToList();
                AllDateLists.Sort();
                int idx = 0;
                foreach (string QueryDate in AllDateLists)
                {
                    idx++;
                    if (sType != "ASYNC")
                    {
                        toolStripStatusLabel1.Text =
                          string.Format("[{5}/{6}]TUMD更新進度({1}/{2})=>[{3}]{4} ", "", idx, AllDateLists.Count, StockCode, QueryDate, issidx, iss);
                        Application.DoEvents();
                    }


                    ssql = @"
                        select * from StockTumdRec where stockcode = '{0}' and stockdate = '{1}'
                    ";
                    ssql = string.Format(ssql, StockCode, QueryDate);
                    StockTumdRec strDb = dbDapper.QuerySingleOrDefault<StockTumdRec>(ssql);


                    //string sema5 = ema5.Select(o => o.date = QueryDate).FirstOrDefault();
                    if (strDb == null)
                    {
                        strDb = new StockTumdRec();
                        strDb.stockcode = StockCode;
                        strDb.stockdate = QueryDate;
                        strDb.Ema5 = Convert.ToDecimal(ema5.Find(o => o.date == QueryDate) == null ? "0" : ema5.Find(o => o.date == QueryDate).value);
                        strDb.Ema10 = Convert.ToDecimal(ema10.Find(o => o.date == QueryDate) == null ? "0" : ema10.Find(o => o.date == QueryDate).value);
                        strDb.Ema20 = Convert.ToDecimal(ema20.Find(o => o.date == QueryDate) == null ? "0" : ema20.Find(o => o.date == QueryDate).value);
                        strDb.Ema60 = Convert.ToDecimal(ema60.Find(o => o.date == QueryDate) == null ? "0" : ema60.Find(o => o.date == QueryDate).value);
                        strDb.Ema120 = Convert.ToDecimal(ema120.Find(o => o.date == QueryDate) == null ? "0" : ema120.Find(o => o.date == QueryDate).value);
                        strDb.Ema240 = Convert.ToDecimal(ema240.Find(o => o.date == QueryDate) == null ? "0" : ema240.Find(o => o.date == QueryDate).value);
                        strDb.DefWeek = Convert.ToDecimal(defWeek.Find(o => o.date == QueryDate) == null ? "0" : defWeek.Find(o => o.date == QueryDate).value);
                        strDb.DefMonth = Convert.ToDecimal(defMonth.Find(o => o.date == QueryDate) == null ? "0" : defMonth.Find(o => o.date == QueryDate).value);
                        strDb.Tu = Convert.ToDecimal(LineTU.Find(o => o.date == QueryDate) == null ? "0" : LineTU.Find(o => o.date == QueryDate).value);
                        strDb.Tm = Convert.ToDecimal(LineTM.Find(o => o.date == QueryDate) == null ? "0" : LineTM.Find(o => o.date == QueryDate).value);
                        strDb.Td = Convert.ToDecimal(LineTD.Find(o => o.date == QueryDate) == null ? "0" : LineTD.Find(o => o.date == QueryDate).value);

                        dbDapper.Insert<StockTumdRec>(strDb);
                    }
                    else
                    {
                        strDb.Ema5 = Convert.ToDecimal(ema5.Find(o => o.date == QueryDate) == null ? "0" : ema5.Find(o => o.date == QueryDate).value);
                        strDb.Ema10 = Convert.ToDecimal(ema10.Find(o => o.date == QueryDate) == null ? "0" : ema10.Find(o => o.date == QueryDate).value);
                        strDb.Ema20 = Convert.ToDecimal(ema20.Find(o => o.date == QueryDate) == null ? "0" : ema20.Find(o => o.date == QueryDate).value);
                        strDb.Ema60 = Convert.ToDecimal(ema60.Find(o => o.date == QueryDate) == null ? "0" : ema60.Find(o => o.date == QueryDate).value);
                        strDb.Ema120 = Convert.ToDecimal(ema120.Find(o => o.date == QueryDate) == null ? "0" : ema120.Find(o => o.date == QueryDate).value);
                        strDb.Ema240 = Convert.ToDecimal(ema240.Find(o => o.date == QueryDate) == null ? "0" : ema240.Find(o => o.date == QueryDate).value);
                        strDb.DefWeek = Convert.ToDecimal(defWeek.Find(o => o.date == QueryDate) == null ? "0" : defWeek.Find(o => o.date == QueryDate).value);
                        strDb.DefMonth = Convert.ToDecimal(defMonth.Find(o => o.date == QueryDate) == null ? "0" : defMonth.Find(o => o.date == QueryDate).value);
                        strDb.Tu = Convert.ToDecimal(LineTU.Find(o => o.date == QueryDate) == null ? "0" : LineTU.Find(o => o.date == QueryDate).value);
                        strDb.Tm = Convert.ToDecimal(LineTM.Find(o => o.date == QueryDate) == null ? "0" : LineTM.Find(o => o.date == QueryDate).value);
                        strDb.Td = Convert.ToDecimal(LineTD.Find(o => o.date == QueryDate) == null ? "0" : LineTD.Find(o => o.date == QueryDate).value);

                        dbDapper.Update<StockTumdRec>(strDb);
                    }

                }


                ////Row
                //List<StockTumdRec> TumdLists = new List<StockTumdRec>();




                //StockTumdRec str = new StockTumdRec();






                string sss = "";



            }
            catch (Exception ex)
            {
                logger.Error(ex, sJson);
            }

            return ResultList;
        }



        List<BackgroundWorker> bws = new List<BackgroundWorker>();
        int t = 20;




        /// <summary>
        /// (1-1)匯入雨量資料(水保局CVS)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button24Old_Click(object sender, EventArgs e)
        {
            //1. 水保局CVS都放到"程式根目錄\SoilWaterRainDataHis"目錄下，開啟轉檔

            string sConstDirectoryPath = Directory.GetCurrentDirectory() + @"\SoilWaterRainDataHis";
            Directory.CreateDirectory(sConstDirectoryPath);

            //水保局CVS都放到"程式根目錄\SoilWaterRainDataHis"目錄下，開啟轉檔
            FileInfo[] fiList = new DirectoryInfo(sConstDirectoryPath).GetFiles("*.csv", SearchOption.TopDirectoryOnly);

            foreach (FileInfo item in fiList)
            {

                string readText = File.ReadAllText(item.FullName, Encoding.Default);
                string sss = readText.Substring(0, 8);
                List<string> StringList = new List<string>();

                using (FileStream fs = File.Open(item.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BufferedStream bs = new BufferedStream(fs))
                    {
                        using (StreamReader sr = new StreamReader(bs))
                        {
                            string line;
                            int i = 0;
                            while ((line = sr.ReadLine()) != null)
                            {
                                StringList.Add(line);
                            }
                        }
                    }
                }

                int iTotal = StringList.Count;

                StringList.Remove(StringList[0]);
                while (StringList.Count > 0)
                {
                    //顯示目前狀態
                    ShowStatus(string.Format("({0}/{1}),同步正在執行筆數：{2}", StringList.Count, iTotal, bws.Count));

                    if (bws.Count < t)
                    {
                        string sTemp = StringList[0];
                        BackgroundWorker bw = new BackgroundWorker();
                        bw.DoWork += new DoWorkEventHandler(bw_DoWork_SoilWaterRainDataHis);
                        bw.WorkerSupportsCancellation = true;
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                        bws.Add(bw);

                        bw.RunWorkerAsync(sTemp);

                        StringList.Remove(StringList[0]);
                    }
                }

                //資料補遺WeaRainData,SoilWaterRainDataHis，資料可能再轉檔的時候筆數不正確，兩邊都可以有缺漏，比較後互相補上
                ssql = @"
                    select * from (
                        select STID,count(*) ttData from WeaRainData
	                    where STID in (
		                    select distinct STID from SoilWaterRainDataHis 
	                    )
	                    group by STID
                    ) a 
                    left join (
                        select STID,count(*) ttDataHis from SoilWaterRainDataHis  group by STID
                    ) b on a.STID = b.STID 
                    where a.ttData <> b.ttDataHis
                ";

                List<dynamic> checkList = dbDapper.Query<dynamic>(ssql);

                foreach (var StidItem in checkList)
                {
                    //歷史資料比較多，寫回WeaRainData
                    if (StidItem.ttDataHis > StidItem.ttData)
                    {
                        Application.DoEvents();
                        ssql = @"
                            select * from SoilWaterRainDataHis a 
                            left join WeaRainData b on a.STID = b.Stid and a.time = b.Time
                            where a.STID = '{0}' and b.no is null
                        ";
                        ssql = string.Format(ssql, StidItem.STID);
                        List<WraRainDataHis> TempList = dbDapper.Query<WraRainDataHis>(ssql);
                        foreach (WraRainDataHis WraRainDataHisitem in TempList)
                        {

                            //同步寫入 WeaRainData
                            WeaRainData WeaRainDataTemp = new WeaRainData();
                            WeaRainDataTemp.STID = WraRainDataHisitem.Stid;
                            WeaRainDataTemp.time = WraRainDataHisitem.Time;
                            WeaRainDataTemp.PP01 = WraRainDataHisitem.Hour;
                            WeaRainDataTemp.PP01old = WraRainDataHisitem.Hour;

                            ssql = @" select * from WeaRainData where stid = '{0}' and time = '{1}' ";
                            ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
                            WeaRainData WeaRainData_new = dbDapper.QuerySingleOrDefault<WeaRainData>(ssql);

                            //沒有轉檔紀錄
                            if (WeaRainData_new == null)
                            {
                                //如果沒有轉檔紀錄，則新增一筆
                                dbDapper.Insert(WeaRainDataTemp);
                            }
                            else//已經有轉檔紀錄
                            {
                                //更新
                                WeaRainData_new.PP01 = WeaRainDataTemp.PP01;
                                WeaRainData_new.PP01old = WeaRainDataTemp.PP01old;
                                dbDapper.Update(WeaRainData_new);
                            }
                        }
                        Application.DoEvents();
                    }

                    //WeaRainData比較多，寫回歷史資料(只寫入Hour，其他資料後面轉檔會補齊)
                    if (StidItem.ttData > StidItem.ttDataHis)
                    {
                        Application.DoEvents();
                        ssql = @"
                            select * from  WeaRainData a 
                            left join SoilWaterRainDataHis b on a.STID = b.Stid and a.time = b.Time
                            where a.STID = '{0}' and b.no is null
                        ";
                        ssql = string.Format(ssql, StidItem.STID);
                        List<WeaRainData> TempList = dbDapper.Query<WeaRainData>(ssql);
                        foreach (WeaRainData WeaRainDataitem in TempList)
                        {
                            //同步寫入 歷史資料
                            ssql = @" select * from SoilWaterRainDataHis where stid = '{0}' and time = '{1}' ";
                            ssql = string.Format(ssql, WeaRainDataitem.STID, WeaRainDataitem.time);
                            SoilWaterRainDataHis SoilWaterRainDataHis_new = dbDapper.QuerySingleOrDefault<SoilWaterRainDataHis>(ssql);

                            //沒有轉檔紀錄
                            if (SoilWaterRainDataHis_new == null)
                            {
                                //如果沒有轉檔紀錄，則新增一筆
                                SoilWaterRainDataHis_new = new SoilWaterRainDataHis();
                                SoilWaterRainDataHis_new.Stid = WeaRainDataitem.STID;
                                SoilWaterRainDataHis_new.Time = WeaRainDataitem.time;
                                SoilWaterRainDataHis_new.Hour = WeaRainDataitem.PP01;
                                dbDapper.Insert(SoilWaterRainDataHis_new);
                            }
                            else//已經有轉檔紀錄
                            {
                                //更新
                                SoilWaterRainDataHis_new.Hour = WeaRainDataitem.PP01;
                                dbDapper.Update(SoilWaterRainDataHis_new);
                            }
                        }
                        Application.DoEvents();
                    }


                }



                return;
            }

        }

        /// <summary>
        /// (1-1) 匯入雨量資料(水保局CVS) - 極速 BulkCopy + 純 SQL 補遺版
        /// </summary>
        private async void button24_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            var progress = new Progress<string>(msg => { LogToMsgText(msg); ShowStatus(msg); });

            try
            {
                await Task.Run(async () => await ProcessSoilWaterRainDataBulk(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"轉檔發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
            }
        }

        private async Task ProcessSoilWaterRainDataBulk(IProgress<string> progress)
        {
            string sConstDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "SoilWaterRainDataHis");
            Directory.CreateDirectory(sConstDirectoryPath);

            FileInfo[] fiList = new DirectoryInfo(sConstDirectoryPath).GetFiles("*.csv", SearchOption.TopDirectoryOnly);

            // 💡 請替換成你的連線字串
            string connString = dbDapper.ConnStr;

            foreach (FileInfo item in fiList)
            {
                progress.Report($"讀取與解析檔案: {item.Name}");

                // =========================================================
                // 階段 1：記憶體內整理資料 (捨棄 File.ReadAllText，改用延遲讀取)
                // =========================================================
                DataTable dtTemp = new DataTable();
                dtTemp.Columns.Add("Stid", typeof(string));
                dtTemp.Columns.Add("Time", typeof(string));
                dtTemp.Columns.Add("Min10", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour3", typeof(decimal)).DefaultValue = 0m;  // 新增
                dtTemp.Columns.Add("Hour6", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour12", typeof(decimal)).DefaultValue = 0m; // 新增
                dtTemp.Columns.Add("Hour24", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("DayRainfall", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("RT", typeof(decimal)).DefaultValue = 0m;     // 新增

                // 直接跳過第一行(標題)，逐行解析
                var lines = File.ReadLines(item.FullName, Encoding.Default).Skip(1);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] cols = line.Split(new[] { ',' }, StringSplitOptions.None);
                    if (cols.Length < 7 || cols[0].StartsWith("#")) continue;

                    DateTime dtTime = DateTime.ParseExact(cols[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    string sTime = dtTime.ToString("yyyyMMddHH");

                    decimal dMin10 = decimal.TryParse(cols[2], out decimal m10) ? (m10 < 0 ? 0 : m10) : 0;
                    decimal dHour = decimal.TryParse(cols[3], out decimal h1) ? (h1 < 0 ? 0 : h1) : 0;
                    decimal dHour6 = decimal.TryParse(cols[4], out decimal h6) ? (h6 < 0 ? 0 : h6) : 0;
                    decimal dHour24 = decimal.TryParse(cols[5], out decimal h24) ? (h24 < 0 ? 0 : h24) : 0;
                    decimal dDayRainfall = decimal.TryParse(cols[6], out decimal dr) ? (dr < 0 ? 0 : dr) : 0;

                    // 注意順序：對應 DataTable 結構，Hour3, Hour12, RT 傳入 null 以觸發預設值 0
                    dtTemp.Rows.Add(cols[0], sTime, dMin10, dHour, null, dHour6, null, dHour24, dDayRainfall, null);
                }

                if (dtTemp.Rows.Count == 0) continue;

                progress.Report($"寫入資料庫 ({dtTemp.Rows.Count} 筆)...");

                // =========================================================
                // 階段 2：大批次寫入與 MERGE (SoilWaterRainDataHis & WeaRainData)
                // =========================================================
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            // 暫存表加入 Hour3, Hour12, RT
                            string createTempSql = @"
                        CREATE TABLE #TempSoilRain (
                            Stid VARCHAR(50), Time VARCHAR(20), Min10 DECIMAL(18,2), 
                            Hour DECIMAL(18,2), Hour3 DECIMAL(18,2), Hour6 DECIMAL(18,2), 
                            Hour12 DECIMAL(18,2), Hour24 DECIMAL(18,2), DayRainfall DECIMAL(18,2), RT DECIMAL(18,2)
                        )";
                            using (SqlCommand cmd = new SqlCommand(createTempSql, conn, tx)) await cmd.ExecuteNonQueryAsync();

                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tx))
                            {
                                bulkCopy.DestinationTableName = "#TempSoilRain";
                                bulkCopy.BatchSize = 10000;
                                bulkCopy.BulkCopyTimeout = 300;

                                foreach (DataColumn col in dtTemp.Columns)
                                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);

                                await RunWithLiveProgressAsync(() => bulkCopy.WriteToServerAsync(dtTemp), "SqlBulkCopy 寫入暫存表", progress);
                            }

                            // 合併至 SoilWaterRainDataHis
                            string mergeSoilSql = @"
                        MERGE INTO SoilWaterRainDataHis AS T
                        USING #TempSoilRain AS S ON T.Stid = S.Stid AND T.Time = S.Time
                        WHEN MATCHED THEN
                            UPDATE SET Min10 = S.Min10, Hour = S.Hour, Hour3 = S.Hour3, Hour6 = S.Hour6, 
                                       Hour12 = S.Hour12, Hour24 = S.Hour24, DayRainfall = S.DayRainfall, RT = S.RT
                        WHEN NOT MATCHED THEN
                            INSERT (Stid, Time, Min10, Hour, Hour3, Hour6, Hour12, Hour24, DayRainfall, RT) 
                            VALUES (S.Stid, S.Time, S.Min10, S.Hour, S.Hour3, S.Hour6, S.Hour12, S.Hour24, S.DayRainfall, S.RT);";
                            using (SqlCommand cmd = new SqlCommand(mergeSoilSql, conn, tx))
                            {
                                cmd.CommandTimeout = 1200;
                                await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "MERGE 寫入 SoilWaterRainDataHis", progress);
                            }

                            // 合併至 WeaRainData
                            string mergeWeaSql = @"
                        MERGE INTO WeaRainData AS T
                        USING #TempSoilRain AS S ON T.STID = S.Stid AND T.time = S.Time
                        WHEN MATCHED THEN
                            UPDATE SET PP01 = S.Hour, PP01old = S.Hour
                        WHEN NOT MATCHED THEN
                            INSERT (STID, time, PP01, PP01old) VALUES (S.Stid, S.Time, S.Hour, S.Hour);";
                            using (SqlCommand cmd = new SqlCommand(mergeWeaSql, conn, tx))
                            {
                                cmd.CommandTimeout = 1200;
                                await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "MERGE 寫入 WeaRainData", progress);
                            }


                            // 合併至 WeaRainDataHis (加入 Hour3, Hour12, RT 等相關欄位)
                            string mergeWeaHisSql = @"
                        MERGE INTO WeaRainDataHis AS T
                        USING #TempSoilRain AS S ON T.Stid = S.Stid AND T.Time = S.Time
                        WHEN MATCHED THEN
                            UPDATE SET Hour = S.Hour, Hour3 = S.Hour3, Hour6 = S.Hour6, 
                                       Hour12 = S.Hour12, Hour24 = S.Hour24, DayRainfall = S.DayRainfall, RT = S.RT
                        WHEN NOT MATCHED THEN
                            INSERT (Stid, Time, Hour, Hour3, Hour6, Hour12, Hour24, DayRainfall, RT, TransYN) 
                            VALUES (S.Stid, S.Time, S.Hour, S.Hour3, S.Hour6, S.Hour12, S.Hour24, S.DayRainfall, S.RT, 'N');";
                            using (SqlCommand cmd = new SqlCommand(mergeWeaHisSql, conn, tx))
                            {
                                cmd.CommandTimeout = 1200;
                                await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "MERGE 寫入 WeaRainDataHis", progress);
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                // 移至 OK 資料夾
                string okDirPath = Path.Combine(item.DirectoryName, "OK");
                Directory.CreateDirectory(okDirPath);
                string newFullName = Path.Combine(okDirPath, item.Name);
                if (File.Exists(newFullName)) File.Delete(newFullName);
                item.MoveTo(newFullName);
            }

            // =========================================================
            // 階段 3：100% 純 SQL 資料補遺 (完全取代下半部的 C# Dapper 迴圈)
            // =========================================================
            progress.Report("開始進行資料庫雙向補遺 (純 SQL 執行中)...");
            using (SqlConnection conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();

                // 補遺 1：將 SoilWaterRainDataHis 中有，但 WeaRainData 沒有的資料，一口氣塞進 WeaRainData
                string syncToWeaSql = @"
            INSERT INTO WeaRainData (STID, time, PP01, PP01old)
            SELECT a.Stid, a.Time, a.Hour, a.Hour
            FROM SoilWaterRainDataHis a
            LEFT JOIN WeaRainData b ON a.Stid = b.STID AND a.Time = b.time
            WHERE b.STID IS NULL;";

                using (SqlCommand cmd = new SqlCommand(syncToWeaSql, conn))
                {
                    cmd.CommandTimeout = 1200;
                    await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "補遺：寫回 WeaRainData", progress);
                }

                // 補遺 2：將 WeaRainData 中有，但 SoilWaterRainDataHis 沒有的資料，一口氣塞回 SoilWaterRainDataHis
                string syncToSoilSql = @"
                    INSERT INTO SoilWaterRainDataHis (Stid, Time, Hour, Min10, Hour3, Hour6, Hour12, Hour24, DayRainfall, RT)
                    SELECT a.STID, a.time, a.PP01, 0, 0, 0, 0, 0, 0, 0
                    FROM WeaRainData a
                    LEFT JOIN SoilWaterRainDataHis b ON a.STID = b.Stid AND a.time = b.Time
                    WHERE b.Stid IS NULL 
                    AND a.STID IN (SELECT DISTINCT Stid FROM SoilWaterRainDataHis);";

                using (SqlCommand cmd = new SqlCommand(syncToSoilSql, conn))
                {
                    cmd.CommandTimeout = 1200;
                    await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "補遺：寫回 SoilWaterRainDataHis", progress);
                }
            }

            progress.Report("水保局雨量資料轉檔與補遺全數完成！");
        }

        /// <summary>
        /// 【新增的輔助方法】：在執行非同步工作時，每 0.5 秒回報一次目前的耗時給 UI
        /// </summary>
        private async Task RunWithLiveProgressAsync(Func<Task> dbAction, string taskName, IProgress<string> progress)
        {
            var sw = Stopwatch.StartNew();
            using (var cts = new CancellationTokenSource())
            {
                // 啟動一個背景計時任務，只要還沒被取消，就一直回報時間
                var updateTask = Task.Run(async () =>
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        progress.Report($"{taskName} 執行中... {sw.Elapsed.TotalSeconds:F1} 秒");
                        await Task.Delay(500, cts.Token); // 暫停 0.5 秒
                    }
                });

                try
                {
                    // 這裡才是真正開始執行資料庫動作 (BulkCopy 或 ExecuteNonQuery)
                    await dbAction();
                }
                finally
                {
                    // 資料庫動作完成 (或失敗) 後，立刻發送取消訊號停止計時迴圈
                    cts.Cancel();
                    try { await updateTask; } catch (TaskCanceledException) { /* 忽略 Task 取消的例外 */ }

                    sw.Stop();
                    progress.Report($"{taskName} 完成！總耗時: {sw.Elapsed.TotalSeconds:F2} 秒");
                }
            }
        }

        /// <summary>
        /// 將訊息輸出到 MsgText (RichTextBox)，並自動捲動到最底端
        /// </summary>
        private void LogToMsgText(string msg)
        {
            // 確保跨執行緒呼叫時的安全性 (雖然 IProgress 已經在 UI 執行緒，但加上 Invoke 更萬無一失)
            if (this.MsgText.InvokeRequired)
            {
                this.MsgText.Invoke(new Action<string>(LogToMsgText), msg);
                return;
            }

            // 加上目前時間與換行符號
            MsgText.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\r\n");

            // 將游標移到最後面並自動捲動
            MsgText.SelectionStart = MsgText.TextLength;
            MsgText.ScrollToCaret();
        }

        private void bw_DoWork_SoilWaterRainDataHis(object sender, DoWorkEventArgs e)
        {

            String line = Convert.ToString(e.Argument);
            if (line == "") return;
            List<string> ColsList1 = line.Split(',').Where(x => x != "" && x != "#").ToList<string>();

            //處理時間轉換
            DateTime dtTime = DateTime.ParseExact(ColsList1[1], "yyyy-MM-dd HH:mm:ss", new CultureInfo(""));

            //string sTime = Convert.ToString(Int32.Parse(dtTime.ToString("yyyyMMddHH")) + 1);
            string sTime = dtTime.ToString("yyyyMMddHH");
            //處理數據轉換
            decimal dMin10 = 0;
            decimal dHour = 0;
            decimal dHour6 = 0;
            decimal dHour24 = 0;
            decimal dDayRainfall = 0;

            dMin10 = Convert.ToDecimal(ColsList1[2]) < 0 ? 0 : Convert.ToDecimal(ColsList1[2]);
            dHour = Convert.ToDecimal(ColsList1[3]) < 0 ? 0 : Convert.ToDecimal(ColsList1[3]);
            dHour6 = Convert.ToDecimal(ColsList1[4]) < 0 ? 0 : Convert.ToDecimal(ColsList1[4]);
            dHour24 = Convert.ToDecimal(ColsList1[5]) < 0 ? 0 : Convert.ToDecimal(ColsList1[5]);
            dDayRainfall = Convert.ToDecimal(ColsList1[6]) < 0 ? 0 : Convert.ToDecimal(ColsList1[6]);

            //產生預寫資料
            SoilWaterRainDataHis SoilWaterRainDataHisTemp = new SoilWaterRainDataHis();
            SoilWaterRainDataHisTemp.Stid = ColsList1[0];
            SoilWaterRainDataHisTemp.Time = sTime;
            SoilWaterRainDataHisTemp.Min10 = dMin10;
            SoilWaterRainDataHisTemp.Hour = dHour;
            SoilWaterRainDataHisTemp.Hour6 = dHour6;
            SoilWaterRainDataHisTemp.Hour24 = dHour24;
            SoilWaterRainDataHisTemp.DayRainfall = dDayRainfall;

            ssql = @" select * from SoilWaterRainDataHis where stid = '{0}' and time = '{1}' ";
            ssql = string.Format(ssql, SoilWaterRainDataHisTemp.Stid, SoilWaterRainDataHisTemp.Time);
            SoilWaterRainDataHis dsl_new = dbDapper.QuerySingleOrDefault<SoilWaterRainDataHis>(ssql);

            //沒有轉檔紀錄
            if (dsl_new == null)
            {
                //如果沒有轉檔紀錄，則新增一筆執行中
                dbDapper.Insert(SoilWaterRainDataHisTemp);
            }
            else//已經有轉檔紀錄
            {
                //更新
                dsl_new.Min10 = SoilWaterRainDataHisTemp.Min10;
                dsl_new.Hour = SoilWaterRainDataHisTemp.Hour;
                dsl_new.Hour6 = SoilWaterRainDataHisTemp.Hour6;
                dsl_new.Hour24 = SoilWaterRainDataHisTemp.Hour24;
                dsl_new.DayRainfall = SoilWaterRainDataHisTemp.DayRainfall;

                dbDapper.Update(dsl_new);
            }

            //同步寫入 WeaRainData
            WeaRainData WeaRainDataTemp = new WeaRainData();
            WeaRainDataTemp.STID = SoilWaterRainDataHisTemp.Stid;
            WeaRainDataTemp.time = SoilWaterRainDataHisTemp.Time;
            WeaRainDataTemp.PP01 = SoilWaterRainDataHisTemp.Hour;
            WeaRainDataTemp.PP01old = SoilWaterRainDataHisTemp.Hour;

            ssql = @" select * from WeaRainData where stid = '{0}' and time = '{1}' ";
            ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
            WeaRainData WeaRainData_new = dbDapper.QuerySingleOrDefault<WeaRainData>(ssql);

            //沒有轉檔紀錄
            if (WeaRainData_new == null)
            {
                //如果沒有轉檔紀錄，則新增一筆
                dbDapper.Insert(WeaRainDataTemp);
            }
            else//已經有轉檔紀錄
            {
                //更新
                WeaRainData_new.PP01 = WeaRainDataTemp.PP01;
                WeaRainData_new.PP01old = WeaRainDataTemp.PP01old;
                dbDapper.Update(WeaRainData_new);
            }

            //20250218 同步寫入WeaRainDataHis，避免資料同步寫入問題
            ssql = @" select * from WeaRainDataHis where stid = '{0}' and time = '{1}' ";
            ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
            //20250218 發生重複的問題，所以判斷是否有重複資料，如果已經有重複資料則刪除
            if (dbDapper.QueryTotalCount(ssql) > 1)
            {
                dbDapper.Execute(string.Format(" delete WeaRainDataHis where Stid = '{0}' and Time = '{1}' "
                    , WeaRainDataTemp.STID, WeaRainDataTemp.time));
            }

            WeaRainDataHis WeaRainDataHis_new = dbDapper.QuerySingleOrDefault<WeaRainDataHis>(ssql);
            //沒有轉檔紀錄
            if (WeaRainDataHis_new == null)
            {
                //如果沒有轉檔紀錄，則新增一筆執行中
                //寫入統計LOG
                WeaRainDataHis_new = new WeaRainDataHis();
                WeaRainDataHis_new.Stid = WeaRainDataTemp.STID;
                WeaRainDataHis_new.Time = WeaRainDataTemp.time;
                WeaRainDataHis_new.Hour = WeaRainDataTemp.PP01;
                WeaRainDataHis_new.TransYN = "N";
                dbDapper.Insert(WeaRainDataHis_new);
            }
            else//已經有轉檔紀錄
            {
                //更新
                WeaRainDataHis_new.Hour = WeaRainDataTemp.PP01;
                dbDapper.Update(WeaRainDataHis_new);
            }

            //dbDapper.Insert(wd);
            Application.DoEvents();
        }

        private void bw_DoWork_WraRainDataHis(object sender, DoWorkEventArgs e)
        {

            String line = Convert.ToString(e.Argument);
            if (line == "") return;
            List<string> ColsList1 = line.Split(',').Where(x => x != "" && x != "#").ToList<string>();

            //處理時間轉換
            DateTime dtTime = DateTime.ParseExact(ColsList1[1], "yyyy-MM-dd HH:mm:ss", new CultureInfo(""));

            //string sTime = Convert.ToString(Int32.Parse(dtTime.ToString("yyyyMMddHH")) + 1);
            string sTime = dtTime.ToString("yyyyMMddHH");
            //處理數據轉換
            decimal dMin10 = 0;
            decimal dHour = 0;
            decimal dHour6 = 0;
            decimal dHour24 = 0;
            decimal dDayRainfall = 0;

            dMin10 = Convert.ToDecimal(ColsList1[2]) < 0 ? 0 : Convert.ToDecimal(ColsList1[2]);
            dHour = Convert.ToDecimal(ColsList1[3]) < 0 ? 0 : Convert.ToDecimal(ColsList1[3]);
            dHour6 = Convert.ToDecimal(ColsList1[4]) < 0 ? 0 : Convert.ToDecimal(ColsList1[4]);
            dHour24 = Convert.ToDecimal(ColsList1[5]) < 0 ? 0 : Convert.ToDecimal(ColsList1[5]);
            dDayRainfall = Convert.ToDecimal(ColsList1[6]) < 0 ? 0 : Convert.ToDecimal(ColsList1[6]);


            //產生預寫資料
            WraRainDataHis WraRainDataHisTemp = new WraRainDataHis();
            WraRainDataHisTemp.Stid = ColsList1[0];
            WraRainDataHisTemp.Time = sTime;
            WraRainDataHisTemp.Min10 = dMin10;
            WraRainDataHisTemp.Hour = dHour;
            WraRainDataHisTemp.Hour6 = dHour6;
            WraRainDataHisTemp.Hour24 = dHour24;
            WraRainDataHisTemp.DayRainfall = dDayRainfall;

            ssql = @" select * from WraRainDataHis where stid = '{0}' and time = '{1}' ";
            ssql = string.Format(ssql, WraRainDataHisTemp.Stid, WraRainDataHisTemp.Time);
            WraRainDataHis dsl_new = dbDapper.QuerySingleOrDefault<WraRainDataHis>(ssql);

            //沒有轉檔紀錄
            if (dsl_new == null)
            {
                //如果沒有轉檔紀錄，則新增一筆執行中
                dbDapper.Insert(WraRainDataHisTemp);
            }
            else//已經有轉檔紀錄
            {
                //更新
                dsl_new.Min10 = WraRainDataHisTemp.Min10;
                dsl_new.Hour = WraRainDataHisTemp.Hour;
                dsl_new.Hour6 = WraRainDataHisTemp.Hour6;
                dsl_new.Hour24 = WraRainDataHisTemp.Hour24;
                dsl_new.DayRainfall = WraRainDataHisTemp.DayRainfall;

                dbDapper.Update(dsl_new);
            }

            //同步寫入 WeaRainData
            WeaRainData WeaRainDataTemp = new WeaRainData();
            WeaRainDataTemp.STID = WraRainDataHisTemp.Stid;
            WeaRainDataTemp.time = WraRainDataHisTemp.Time;
            WeaRainDataTemp.PP01 = WraRainDataHisTemp.Hour;
            WeaRainDataTemp.PP01old = WraRainDataHisTemp.Hour;

            ssql = @" select * from WeaRainData where stid = '{0}' and time = '{1}' ";
            ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
            WeaRainData WeaRainData_new = dbDapper.QuerySingleOrDefault<WeaRainData>(ssql);

            //沒有轉檔紀錄
            if (WeaRainData_new == null)
            {
                //如果沒有轉檔紀錄，則新增一筆
                dbDapper.Insert(WeaRainDataTemp);
            }
            else//已經有轉檔紀錄
            {
                //更新
                WeaRainData_new.PP01 = WeaRainDataTemp.PP01;
                WeaRainData_new.PP01old = WeaRainDataTemp.PP01old;
                dbDapper.Update(WeaRainData_new);
            }

            //20250218 同步寫入WeaRainDataHis，避免資料同步寫入問題
            ssql = @" select * from WeaRainDataHis where stid = '{0}' and time = '{1}' ";
            ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
            //20250218 發生重複的問題，所以判斷是否有重複資料，如果已經有重複資料則刪除
            if (dbDapper.QueryTotalCount(ssql) > 1)
            {
                dbDapper.Execute(string.Format(" delete WeaRainDataHis where Stid = '{0}' and Time = '{1}' "
                    , WeaRainDataTemp.STID, WeaRainDataTemp.time));
            }

            WeaRainDataHis WeaRainDataHis_new = dbDapper.QuerySingleOrDefault<WeaRainDataHis>(ssql);
            //沒有轉檔紀錄
            if (WeaRainDataHis_new == null)
            {
                //如果沒有轉檔紀錄，則新增一筆執行中
                //寫入統計LOG
                WeaRainDataHis_new = new WeaRainDataHis();
                WeaRainDataHis_new.Stid = WeaRainDataTemp.STID;
                WeaRainDataHis_new.Time = WeaRainDataTemp.time;
                WeaRainDataHis_new.Hour = WeaRainDataTemp.PP01;
                WeaRainDataHis_new.TransYN = "N";
                dbDapper.Insert(WeaRainDataHis_new);
            }
            else//已經有轉檔紀錄
            {
                //更新
                WeaRainDataHis_new.Hour = WeaRainDataTemp.PP01;
                dbDapper.Update(WeaRainDataHis_new);
            }

            Application.DoEvents();
        }

        /// <summary>
        /// [TCG]北市府DoWork
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_DoWork_TCGRainDataHis(object sender, DoWorkEventArgs e)
        {

            String line = Convert.ToString(e.Argument);
            if (line == "") return;
            List<string> ColsList1 = line.Split('\t').Where(x => x != "" && x != "#").ToList<string>();

            //處理時間轉換
            DateTime dtTime = DateTime.ParseExact(ColsList1[1], "yyyy/M/d HH:mm", new CultureInfo(""));

            //string sTime = Convert.ToString(Int32.Parse(dtTime.ToString("yyyyMMddHH")) + 1);
            string sTime = dtTime.ToString("yyyyMMddHH");
            //處理數據轉換
            decimal dMin10 = 0;
            decimal dHour = 0;
            decimal dHour3 = 0;
            decimal dHour6 = 0;
            decimal dHour12 = 0;
            decimal dHour24 = 0;
            decimal dDayRainfall = 0;
            decimal dRT = 0;


            dMin10 = Convert.ToDecimal(ColsList1[2]) < 0 ? 0 : Convert.ToDecimal(ColsList1[2]);
            dHour = Convert.ToDecimal(ColsList1[3]) < 0 ? 0 : Convert.ToDecimal(ColsList1[3]);
            dHour3 = 0;
            dHour6 = Convert.ToDecimal(ColsList1[4]) < 0 ? 0 : Convert.ToDecimal(ColsList1[4]);
            dHour12 = 0;
            dHour24 = Convert.ToDecimal(ColsList1[5]) < 0 ? 0 : Convert.ToDecimal(ColsList1[5]);
            dDayRainfall = Convert.ToDecimal(ColsList1[6]) < 0 ? 0 : Convert.ToDecimal(ColsList1[6]);
            dRT = 0;

            //產生預寫資料
            TCGRainDataHis TCGRainDataHisTemp = new TCGRainDataHis();
            TCGRainDataHisTemp.Stid = ColsList1[0];
            TCGRainDataHisTemp.Time = sTime;
            TCGRainDataHisTemp.Min10 = dMin10;
            TCGRainDataHisTemp.Hour = dHour;
            TCGRainDataHisTemp.Hour3 = dHour3;
            TCGRainDataHisTemp.Hour6 = dHour6;
            TCGRainDataHisTemp.Hour12 = dHour12;
            TCGRainDataHisTemp.Hour24 = dHour24;
            TCGRainDataHisTemp.DayRainfall = dDayRainfall;
            TCGRainDataHisTemp.RT = dRT;

            ssql = @" select * from TCGRainDataHis where stid = '{0}' and time = '{1}' ";
            ssql = string.Format(ssql, TCGRainDataHisTemp.Stid, TCGRainDataHisTemp.Time);
            TCGRainDataHis dsl_new = dbDapper.QuerySingleOrDefault<TCGRainDataHis>(ssql);

            //沒有轉檔紀錄
            if (dsl_new == null)
            {
                //如果沒有轉檔紀錄，則新增一筆執行中
                dbDapper.Insert(TCGRainDataHisTemp);
            }
            else//已經有轉檔紀錄
            {
                //更新
                dsl_new.Min10 = TCGRainDataHisTemp.Min10;
                dsl_new.Hour = TCGRainDataHisTemp.Hour;
                dsl_new.Hour3 = TCGRainDataHisTemp.Hour3;
                dsl_new.Hour6 = TCGRainDataHisTemp.Hour6;
                dsl_new.Hour12 = TCGRainDataHisTemp.Hour12;
                dsl_new.Hour24 = TCGRainDataHisTemp.Hour24;
                dsl_new.DayRainfall = TCGRainDataHisTemp.DayRainfall;
                dsl_new.RT = TCGRainDataHisTemp.RT;

                dbDapper.Update(dsl_new);
            }

            //同步寫入 WeaRainData
            WeaRainData WeaRainDataTemp = new WeaRainData();
            WeaRainDataTemp.STID = TCGRainDataHisTemp.Stid;
            WeaRainDataTemp.time = TCGRainDataHisTemp.Time;
            WeaRainDataTemp.PP01 = TCGRainDataHisTemp.Hour;
            WeaRainDataTemp.PP01old = TCGRainDataHisTemp.Hour;

            ssql = @" select * from WeaRainData where stid = '{0}' and time = '{1}' ";
            ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
            WeaRainData WeaRainData_new = dbDapper.QuerySingleOrDefault<WeaRainData>(ssql);

            //沒有轉檔紀錄
            if (WeaRainData_new == null)
            {
                //如果沒有轉檔紀錄，則新增一筆
                dbDapper.Insert(WeaRainDataTemp);
            }
            else//已經有轉檔紀錄
            {
                //更新
                WeaRainData_new.PP01 = WeaRainDataTemp.PP01;
                WeaRainData_new.PP01old = WeaRainDataTemp.PP01old;
                dbDapper.Update(WeaRainData_new);
            }

            //20250218 同步寫入WeaRainDataHis，避免資料同步寫入問題
            ssql = @" select * from WeaRainDataHis where stid = '{0}' and time = '{1}' ";
            ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
            //20250218 發生重複的問題，所以判斷是否有重複資料，如果已經有重複資料則刪除
            if (dbDapper.QueryTotalCount(ssql) > 1)
            {
                dbDapper.Execute(string.Format(" delete WeaRainDataHis where Stid = '{0}' and Time = '{1}' "
                    , WeaRainDataTemp.STID, WeaRainDataTemp.time));
            }

            WeaRainDataHis WeaRainDataHis_new = dbDapper.QuerySingleOrDefault<WeaRainDataHis>(ssql);
            //沒有轉檔紀錄
            if (WeaRainDataHis_new == null)
            {
                //如果沒有轉檔紀錄，則新增一筆執行中
                //寫入統計LOG
                WeaRainDataHis_new = new WeaRainDataHis();
                WeaRainDataHis_new.Stid = WeaRainDataTemp.STID;
                WeaRainDataHis_new.Time = WeaRainDataTemp.time;
                WeaRainDataHis_new.Hour = WeaRainDataTemp.PP01;
                WeaRainDataHis_new.TransYN = "N";
                dbDapper.Insert(WeaRainDataHis_new);
            }
            else//已經有轉檔紀錄
            {
                //更新
                WeaRainDataHis_new.Hour = WeaRainDataTemp.PP01;
                dbDapper.Update(WeaRainDataHis_new);
            }

            Application.DoEvents();
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bws.Remove(sender as BackgroundWorker);
            if (bws.Count == 0)
            {
                if (e.Cancelled)
                {
                    //this.richTextBox1.Text = “執行緒已經停止”;
                }
                else
                {
                    //this.richTextBox1.Text = “執行緒已經完成”;
                }
            }
        }

        /// <summary>
        /// 水保局歷史資料轉檔(包含計算RT)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button25Old_Click(object sender, EventArgs e)
        {
            //1.處理資料來源，已經由1-2匯入檔案資料到(SoilWaterRainDataHis及WeaRainData)
            //(1-1) 按鈕匯入水保局CSV檔
            //比對兩個資料表的資料內容是否有相同筆數，如果筆數不相同則找出問題互相補遺
            /* SQL:
                select * from (
                    select STID,count(*) ttData from WeaRainData
	                where STID in (
		                select distinct STID from SoilWaterRainDataHis 
	                )
	                group by STID
                ) a 
                left join (
                    select STID,count(*) ttDataHis from SoilWaterRainDataHis  group by STID
                ) b on a.STID = b.STID 
                where a.ttData <> b.ttDataHis
            */

            //20240510 在(1-1)按鈕已經加上WraRainDataHis補遺到WeaRainData的功能
            //**但是WeaRainData補遺到WraRainDataHis，功能還沒完成

            //(2-1)每年第一次轉檔時，請執行以下SQL，清除已轉檔完成或是執行中未清除的資料
            // delete DataStaticLog where type = 'SoilWaterRainHisSTID' and status in ('10','60')

            //(2-2)不需要刪除任何的LOG紀錄，因為歷年已經跑過的資料，已經有LOG，如果篩除，會全部重跑
            //delete DataStaticLog where type = 'SoilWaterRainHisSTIDTime'


            //2.直接執行多個程式一起跑

            //3.最後比對是否有全部轉檔成功到歷史區
            /* SQL:
                select * from (
                    select STID,count(*) ttData from WeaRainData
	                where STID in (
		                select distinct STID from SoilWaterRainDataHis 
	                )
	                group by STID
                ) a 
                left join (
                    select STID,count(*) ttDataHis from SoilWaterRainDataHis  group by STID
                ) b on a.STID = b.STID 
                where a.ttData <> b.ttDataHis
             * */

            string sStid = "";

            //轉檔註記
            //每個雨量站
            string sSTIDLogType = "SoilWaterRainHisSTID";
            //每個雨量站的時刻
            string sSTIDTimeLogType = "SoilWaterRainHisSTIDTime";


            //有輸入資料，則轉該雨量站的資料
            if (Trans109Text.Text.Trim() != "")
            {
                sStid = Trans109Text.Text;
                this.Text = sStid;
                ProcSoilWaterRainDataHisOld(sStid, sSTIDTimeLogType);
            }
            else //如果沒輸入，則由資料庫判斷尚未執行或還沒執行完全的雨量站進行轉檔
            {
                while (true)
                {
                    try
                    {
                        //比較該雨量站資料數量與轉檔LOG紀錄數量，如果沒轉過或是資料數不符，則進行轉檔
                        //20210506 新增排除已經在轉檔的雨量站
                        ssql = @" 
                                select * from (
                                 select * from (
                                     select STID,count(*) ttData from SoilWaterRainDataHis group by STID
                                 ) a 
                                 left join (
                                     select key2,count(*) ttLog from DataStaticLog where type = '{1}'  group by key2
                                 ) b on a.STID = b.key2 
                                ) r 
                                left join DataStaticLog c  on c.type = '{0}' and c.key1 = r.STID and c.status = '10'
                                where (ttLog != ttData or ttLog is null) and c.key1 is null    
                                order by r.STID
                            ";
                        ssql = string.Format(ssql, sSTIDLogType, sSTIDTimeLogType);

                        ShowStatus(string.Format("正在統計尚未轉檔到歷史資料的雨量站...."));

                        List<dynamic> TransList = dbDapper.Query(ssql);

                        //如果沒資料則停止
                        if (TransList.Count() == 0) break;

                        //總筆數，使用Random取得資料避免重複執行問題
                        int iRandom = new Random().Next(0, TransList.Count());

                        //預防重複執行後，LOG紀錄筆數>WeaRainData的筆數導致前面判斷未轉完畢的篩選，所以如果超過筆數，則刪除LOG重新跑
                        int ittLog = TransList[iRandom].ttLog == null ? 0 : TransList[iRandom].ttLog;
                        int ittData = TransList[iRandom].ttData == null ? 0 : TransList[iRandom].ttData;
                        if (ittLog > ittData)
                        {
                            //20240507 查詢問題後，發現LOG多餘的資料對應到SoilWaterRainDataHis，是NULL，所以刪除LOG紀錄NULL的資料，以免全部重跑耗費太多時間
                            ssql = @" select * from DataStaticLog a 
                                        left join SoilWaterRainDataHis b on  b.no = a.key1
                                        where a.type = '{0}' and b.STID is null and a.key2 = '{1}' ";
                            ssql = string.Format(ssql, sSTIDTimeLogType, TransList[iRandom].STID);
                            List<dynamic> ErrorList = dbDapper.Query(ssql);

                            foreach (dynamic item in ErrorList)
                            {
                                ssql = @" delete DataStaticLog where type = '{0}' and key2 = '{1}' and no = '{2}' ";
                                ssql = string.Format(ssql, sSTIDTimeLogType, item.key2, item.no);
                                dbDapper.Execute(ssql);
                            }

                            continue;
                        }

                        sStid = TransList[iRandom].STID;
                        this.Text = string.Format("{0}-剩餘數量：{1}", sStid, TransList.Count());
                        Application.DoEvents();

                        //==判斷是否執行過
                        //10:執行中 60:已完成 90:轉檔失敗
                        //string sDataStaticLogType = "WeaRainTransHisSTID";
                        ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' ";
                        ssql = string.Format(ssql, sSTIDLogType, sStid);
                        DataStaticLog dsl_new = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);

                        //沒有轉檔紀錄
                        if (dsl_new == null)
                        {
                            //如果沒有轉檔紀錄，則新增一筆執行中
                            //寫入統計LOG
                            dsl_new = new DataStaticLog();
                            dsl_new.type = sSTIDLogType;
                            dsl_new.key1 = sStid;
                            dsl_new.key2 = "";
                            dsl_new.status = "10";
                            dsl_new.logtime = DateTime.Now;
                            dbDapper.Insert(dsl_new);
                        }
                        else//已經有轉檔紀錄
                        {
                            //如果執行中，則不進行該站轉檔
                            if (dsl_new.status == "10")
                            {
                                continue;
                            }

                            //重新更新為執行中
                            dsl_new.status = "10";
                            dbDapper.Update(dsl_new);
                        }

                        //主要轉檔程序
                        ProcSoilWaterRainDataHisOld(sStid, sSTIDTimeLogType);

                        //註記已經轉檔完成
                        ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' ";
                        ssql = string.Format(ssql, sSTIDLogType, sStid);

                        DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                        if (dslu != null)
                        {
                            dslu.status = "60";
                            dbDapper.Update(dslu);
                        }
                    }
                    catch (Exception ex)
                    {
                        //發生錯誤繼續後面的轉檔，直到全部雨量站執行完畢
                        continue;
                    }
                }
            }

            this.Text = "OK-ALL";



        }

        private void ProcSoilWaterRainDataHisOld(string STID, string sSTIDTimeLogType)
        {
            //雨量站
            string sStid = STID;
            //sStid = "466900";

            //string sDataStaticLogType = "109WeaRainTrans";
            //每個雨量站的每個小時都要重新計算所有數值
            ssql = @" select * from SoilWaterRainDataHis where STID = '{0}' order by time ";
            ssql = string.Format(ssql, sStid);

            ShowStatus(string.Format("取得雨量站[{0}]的所有氣象局雨量資料....", sStid));
            List<SoilWaterRainDataHis> WeaRainList = dbDapper.Query<SoilWaterRainDataHis>(ssql);

            //取得尚未轉檔的資料
            ssql = @" 
                    select a.* from SoilWaterRainDataHis a
                    left join DataStaticLog b on CAST( a.no as varchar) = b.key1 and  b.type = '{1}' 
                    where a.STID = '{0}' and b.no is  null 
                    order by a.time ";
            ssql = string.Format(ssql, sStid, sSTIDTimeLogType);
            ShowStatus(string.Format("取得雨量站[{0}]尚未執行的資料....", sStid));
            List<SoilWaterRainDataHis> NoExecList = dbDapper.Query<SoilWaterRainDataHis>(ssql);

            int iIndex1 = 0;
            foreach (SoilWaterRainDataHis item in NoExecList)
            {
                try
                {
                    iIndex1++;

                    //目前該筆資料主要時間
                    DateTime MainTime = DateTime.ParseExact(item.Time, "yyyyMMddHH", null);

                    ShowStatus(string.Format("站號：{2}[{3}/{4}]正在轉檔：{5}", "", "", sStid
                        , iIndex1.ToString(), NoExecList.Count().ToString(), MainTime.ToString("yyyy/MM/dd-HH")));

                    //==判斷是否執行過
                    //10:執行中 60:已完成 90:轉檔失敗
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'    ";
                    ssql = string.Format(ssql, sSTIDTimeLogType, item.no.ToString());
                    DataStaticLog dsl_new = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);

                    //沒有轉檔紀錄
                    if (dsl_new == null)
                    {
                        //如果沒有轉檔紀錄，則新增一筆執行中
                        //寫入統計LOG
                        dsl_new = new DataStaticLog();
                        dsl_new.type = sSTIDTimeLogType;
                        dsl_new.key1 = item.no.ToString();
                        dsl_new.key2 = item.Stid;
                        dsl_new.status = "10";
                        dsl_new.logtime = DateTime.Now;
                        dbDapper.Insert(dsl_new);
                    }
                    else//已經有轉檔紀錄
                    {
                        if (dsl_new.status == "10" || dsl_new.status == "60") //10:執行中 60:已完成
                        {
                            continue;
                        }

                        //重新更新為執行中
                        dsl_new.status = "10";
                        dbDapper.Update(dsl_new);
                    }

                    //取得當月及前一個月的資料
                    List<SoilWaterRainDataHis> wrdList = new List<SoilWaterRainDataHis>();
                    //取得當月資料
                    string sNowYM = MainTime.ToString("yyyyMM");
                    //取得前月資料
                    string sPreYM = MainTime.AddMonths(-1).ToString("yyyyMM");

                    List<SoilWaterRainDataHis> NowYMList = WeaRainList.Where(element => element.Time.StartsWith(sNowYM)).ToList<SoilWaterRainDataHis>();
                    List<SoilWaterRainDataHis> PreYMList = WeaRainList.Where(element => element.Time.StartsWith(sPreYM)).ToList<SoilWaterRainDataHis>();
                    wrdList.AddRange(NowYMList);
                    wrdList.AddRange(PreYMList);


                    //==計算Hour3
                    decimal dHour3 = getSumRangeRainSoilWaterOld(wrdList, item, 0, 2);

                    //==計算Hour6
                    decimal dHour6 = getSumRangeRainSoilWaterOld(wrdList, item, 0, 5);

                    //==計算Hour12
                    decimal dHour12 = getSumRangeRainSoilWaterOld(wrdList, item, 0, 11);

                    //==計算Hour24
                    decimal dHour24 = getSumRangeRainSoilWaterOld(wrdList, item, 0, 23);

                    //==計算HourDayRainFall
                    string sFallDay = item.Time.Substring(0, 8);
                    List<SoilWaterRainDataHis> DayRainFallList = wrdList.Where(element => element.Time.StartsWith(sFallDay)).ToList<SoilWaterRainDataHis>();
                    decimal dFallDay = 0;
                    foreach (SoilWaterRainDataHis DayRainFall in DayRainFallList)
                    {
                        if (DayRainFall != null)
                        {
                            dFallDay += DayRainFall.Hour;
                        }
                    }

                    //==計算RT
                    decimal dRT = 0;
                    double α = 0.7;
                    //M：七天前期雨量，前頁P的計算
                    //如：2012 / 1 / 8 5:00起始，2012 / 1 / 8 13:00尖峰
                    //P = 2012 / 1 / 8 5:00~13:00累積雨量加總，雨量不折減
                    //＋2012 / 1 / 7 日累積雨量* 折減係數α＋2012 / 1 / 6 日累積雨量* 折減係數α 2
                    //＋2012 / 1 / 5 日累積雨量* 折減係數α 3 ＋2012 / 1 / 4 日累積雨量* 折減係數α 4
                    //＋2012 / 1 / 3 日累積雨量* 折減係數α 5 ＋2012 / 1 / 2 日累積雨量* 折減係數α 6
                    //＋2012 / 1 / 1 日累積雨量* 折減係數α

                    //Rt = 當下時刻~24小時內雨量加總 + 25hr~48hr雨量加總 * 0.7 + 49hr~72hr雨量加總 * 0.7 ^ 2 + .....+167hr~192hr雨量加總 * 0.7 ^ 7之總和


                    decimal dRange = getSumRangeRainSoilWaterOld(wrdList, item, 0, 23);
                    decimal dRange1 = getSumRangeRainSoilWaterOld(wrdList, item, 24, 47) * Convert.ToDecimal(Math.Pow(α, 1));
                    decimal dRange2 = getSumRangeRainSoilWaterOld(wrdList, item, 48, 71) * Convert.ToDecimal(Math.Pow(α, 2));
                    decimal dRange3 = getSumRangeRainSoilWaterOld(wrdList, item, 72, 95) * Convert.ToDecimal(Math.Pow(α, 3));
                    decimal dRange4 = getSumRangeRainSoilWaterOld(wrdList, item, 96, 119) * Convert.ToDecimal(Math.Pow(α, 4));
                    decimal dRange5 = getSumRangeRainSoilWaterOld(wrdList, item, 120, 143) * Convert.ToDecimal(Math.Pow(α, 5));
                    decimal dRange6 = getSumRangeRainSoilWaterOld(wrdList, item, 144, 167) * Convert.ToDecimal(Math.Pow(α, 6));
                    decimal dRange7 = getSumRangeRainSoilWaterOld(wrdList, item, 168, 191) * Convert.ToDecimal(Math.Pow(α, 7));

                    dRT = dRange + dRange1 + dRange2 + dRange3 + dRange4 + dRange5 + dRange6 + dRange7;


                    SoilWaterRainDataHis wraTemp = new SoilWaterRainDataHis();
                    wraTemp.Stid = item.Stid;
                    wraTemp.Time = MainTime.ToString("yyyyMMddHH");
                    wraTemp.Hour = item.Hour;
                    wraTemp.Hour3 = dHour3;
                    wraTemp.Hour6 = dHour6;
                    wraTemp.Hour12 = dHour12;
                    wraTemp.Hour24 = dHour24;
                    wraTemp.DayRainfall = dFallDay;
                    wraTemp.RT = dRT;

                    //紀錄氣象局雨量歷史資料(時間序改為微軟標準0-23為一天)
                    ssql = " select * from SoilWaterRainDataHis where Stid = '{0}' and Time = '{1}' ";
                    ssql = string.Format(ssql, wraTemp.Stid, wraTemp.Time);
                    SoilWaterRainDataHis wra = dbDapper.QuerySingleOrDefault<SoilWaterRainDataHis>(ssql);
                    if (wra == null)
                    {
                        wra = wraTemp;

                        dbDapper.Insert(wra);
                    }
                    else
                    {
                        wra.Hour = wraTemp.Hour;
                        wra.Hour3 = wraTemp.Hour3;
                        wra.Hour6 = wraTemp.Hour6;
                        wra.Hour12 = wraTemp.Hour12;
                        wra.Hour24 = wraTemp.Hour24;
                        wra.DayRainfall = wraTemp.DayRainfall;
                        wra.RT = wraTemp.RT;

                        dbDapper.Update(wra);
                    }

                    //註記已經轉檔完成
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'    ";
                    ssql = string.Format(ssql, sSTIDTimeLogType, item.no.ToString());

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "60";
                        dbDapper.Update(dslu);
                    }


                }
                catch (Exception ex)
                {
                    //註記轉檔失敗
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'    ";
                    ssql = string.Format(ssql, sSTIDTimeLogType, item.no.ToString());

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "90";
                        dbDapper.Update(dslu);
                    }
                    continue;
                }
            }
        }

        /// <summary>
        /// 計算區間雨量總和
        /// </summary>
        /// <param name="AllList">該站號所有雨量資料</param>
        /// <param name="Main">主要雨量資料</param>
        /// <param name="iStart">計算的起始雨量區間</param>
        /// <param name="iEnd">計算的結束雨量區間</param>
        /// <returns></returns>
        private decimal getSumRangeRainSoilWaterOld(List<SoilWaterRainDataHis> AllList, SoilWaterRainDataHis Main, int iStart, int iEnd)
        {
            decimal dSum = 0;

            //目前該筆資料主要時間
            DateTime MainTime = DateTime.ParseExact(Convert.ToString(Convert.ToInt32(Main.Time)), "yyyyMMddHH", null);

            //取得起始時間
            DateTime StartTime = MainTime.AddHours(-iStart);

            //總共要計算多少次
            int iCalCount = iEnd - iStart + 1;

            for (int i = 0; i < iCalCount; i++)
            {
                DateTime CalTime = StartTime.AddHours(-i);
                int iTime = Convert.ToInt32(CalTime.ToString("yyyyMMddHH"));
                SoilWaterRainDataHis wrdCal = AllList.SingleOrDefault(s => s.Time == iTime.ToString());

                if (wrdCal != null)
                {
                    dSum += wrdCal.Hour;
                }
            }

            return dSum;
        }

        private async void button25_Click(object sender, EventArgs e)
        {
            //1.處理資料來源，已經由1-2匯入檔案資料到(SoilWaterRainDataHis及WeaRainData)
            //(1-1) 按鈕匯入水保局CSV檔
            //比對兩個資料表的資料內容是否有相同筆數，如果筆數不相同則找出問題互相補遺
            /* SQL:
                select * from (
                    select STID,count(*) ttData from WeaRainData
	                where STID in (
		                select distinct STID from SoilWaterRainDataHis 
	                )
	                group by STID
                ) a 
                left join (
                    select STID,count(*) ttDataHis from SoilWaterRainDataHis  group by STID
                ) b on a.STID = b.STID 
                where a.ttData <> b.ttDataHis
            */

            //20240510 在(1-1)按鈕已經加上WraRainDataHis補遺到WeaRainData的功能
            //**但是WeaRainData補遺到WraRainDataHis，功能還沒完成

            //(2-1)每年第一次轉檔時，請執行以下SQL，清除已轉檔完成或是執行中未清除的資料
            // delete DataStaticLog where type = 'SoilWaterRainHisSTID' and status in ('10','60')

            //(2-2)不需要刪除任何的LOG紀錄，因為歷年已經跑過的資料，已經有LOG，如果篩除，會全部重跑
            //delete DataStaticLog where type = 'SoilWaterRainHisSTIDTime'


            //2.直接執行多個程式一起跑

            //3.最後比對是否有全部轉檔成功到歷史區
            /* SQL:
                WITH WeaSummary AS (
                    SELECT STID, COUNT(*) AS ttData
                    FROM WeaRainData
                    GROUP BY STID
                ),
                HisSummary AS (
                    SELECT STID, COUNT(*) AS ttDataHis
                    FROM SoilWaterRainDataHis
                    GROUP BY STID
                )
                SELECT 
                    w.STID, 
                    w.ttData, 
                    h.ttDataHis
                FROM WeaSummary w
                INNER JOIN HisSummary h ON w.STID = h.STID
                WHERE w.ttData <> h.ttDataHis;
             * */

            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            var progress = new Progress<string>(msg =>
            {
                LogToMsgText(msg); // 替換為您的 ShowStatus
                this.Text = msg;
            });

            try
            {
                await Task.Run(() => ProcessSoilWaterDataTrans(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"水保局轉檔發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
                this.Text = "OK-ALL";
            }
        }

        // =========================================================
        // 核心歷史資料轉檔邏輯 (支援多程式併發分工)
        // =========================================================
        private void ProcessSoilWaterDataTrans(IProgress<string> progress)
        {
            string sSTIDLogType = "SoilWaterRainHisSTID";
            string sSTIDTimeLogType = "SoilWaterRainHisSTIDTime";
            string sStid = "";

            // 如果 UI 有輸入特定站號
            if (!string.IsNullOrWhiteSpace(Trans109Text.Text))
            {
                sStid = Trans109Text.Text.Trim();
                ProcSoilWaterRainDataHis(sStid, sSTIDTimeLogType, progress, 1, 1);
                return;
            }

            progress.Report("正在統計尚未完成水保局轉檔的雨量站....");

            // 取得所有尚未完成的雨量站清單 (利用 NOT EXISTS 提升效能)
            string ssqlMain = @"
                SELECT a.STID 
                FROM (
                    SELECT STID, COUNT(*) AS ttData FROM SoilWaterRainDataHis GROUP BY STID
                ) a
                LEFT JOIN (
                    SELECT key2, COUNT(*) AS ttLog FROM DataStaticLog WHERE type = @TypeTime GROUP BY key2
                ) b ON a.STID = b.key2
                WHERE (b.ttLog IS NULL OR b.ttLog != a.ttData)
                  AND NOT EXISTS (
                      SELECT 1 FROM DataStaticLog c 
                      WHERE c.type = @TypeStid AND c.key1 = a.STID AND c.status = '10'
                  )
                ORDER BY a.STID";

            List<dynamic> TransList = dbDapper.Query(ssqlMain, new { TypeTime = sSTIDTimeLogType, TypeStid = sSTIDLogType }, commandTimeout: 300);
            string ssqlStatus10 = $"select * from  DataStaticLog where type = '{sSTIDLogType}' and status in ('10')";
            int iStatus10 = dbDapper.QueryTotalCount(ssqlStatus10);

            if (TransList.Count == 0 && iStatus10 == 0)
            {
                progress.Report("檢核資料完整性後，水保局轉檔-全部完成....");
                return;
            }
            else if (TransList.Count == 0 && iStatus10 != 0)
            {
                progress.Report($"檢核資料完整性後，水保局轉檔-尚未完成....執行中(10)狀態筆數：{iStatus10}");
                return;
            }

            List<string> stidList = TransList.Select(d => (string)d.STID).ToList();

            // 將清單洗牌，防止多程式撞車
            stidList = stidList.OrderBy(x => Guid.NewGuid()).ToList();

            int totalCount = stidList.Count;
            int currentIndex = 0;

            foreach (string stid in stidList)
            {
                currentIndex++;
                sStid = stid;

                try
                {
                    progress.Report($"[{sStid}] ({currentIndex}/{totalCount}) 爭取轉檔權限...");

                    // =========================================================
                    // 1. 極速搶佔機制 (原子操作 + 排他鎖)
                    // =========================================================
                    string claimSql = @"
                        BEGIN TRAN;
                        DECLARE @Claimed INT = 0;
                
                        IF NOT EXISTS (SELECT 1 FROM DataStaticLog WITH (UPDLOCK, HOLDLOCK) WHERE type = @Type AND key1 = @Stid)
                        BEGIN
                            INSERT INTO DataStaticLog (type, key1, key2, status, logtime) VALUES (@Type, @Stid, '', '10', GETDATE());
                            SET @Claimed = 1;
                        END
                        ELSE
                        BEGIN
                            UPDATE DataStaticLog
                            SET status = '10', logtime = GETDATE()
                            WHERE type = @Type AND key1 = @Stid AND status NOT IN ('10', '60');
                    
                            IF @@ROWCOUNT > 0 SET @Claimed = 1;
                        END
                
                        COMMIT TRAN;
                        SELECT @Claimed;
                    ";

                    int isClaimed = Convert.ToInt32(dbDapper.ExecuteScale(claimSql, new { Type = sSTIDLogType, Stid = sStid }));
                    if (isClaimed == 0) continue;

                    // =========================================================
                    // 2. 執行主要計算程式
                    // =========================================================
                    ProcSoilWaterRainDataHis(sStid, sSTIDTimeLogType, progress, currentIndex, totalCount);

                    // 3. 註記該站點轉檔完成
                    string finishSql = "UPDATE DataStaticLog SET status = '60', logtime = GETDATE() WHERE type = @Type AND key1 = @Stid";
                    dbDapper.Execute(finishSql, new { Type = sSTIDLogType, Stid = sStid });
                }
                catch (Exception ex)
                {
                    // 發生錯誤，跳過並處理下一站
                    progress.Report($"[{sStid}] 發生錯誤跳過: {ex.Message}");
                    continue;
                }
            }

            //最後在檢核一次是否還有沒跑完的資料
            TransList.Clear();
            TransList = dbDapper.Query(ssqlMain, new { TypeTime = sSTIDTimeLogType, TypeStid = sSTIDLogType }, commandTimeout: 300);
            iStatus10 = dbDapper.QueryTotalCount(ssqlStatus10);

            if (TransList.Count == 0 && iStatus10 == 0)
            {
                progress.Report("檢核資料完整性後，水保局轉檔-全部完成....");
            }
            else
            {
                progress.Report($"檢核資料完整性後，水保局轉檔-尚未完成....執行中(10)狀態筆數：{iStatus10}");
                return;
            }

        }

        private void ProcSoilWaterRainDataHis(string sStid, string sSTIDTimeLogType, IProgress<string> progress, int currentIdx, int totalCount)
        {
            progress.Report($"[{sStid}] 讀取水保局雨量資料至記憶體...");

            // 取得該站所有歷史資料
            string ssql = "SELECT * FROM SoilWaterRainDataHis WHERE STID = @stid ORDER BY [Time]";
            List<SoilWaterRainDataHis> WeaRainList = dbDapper.Query<SoilWaterRainDataHis>(ssql, new { stid = sStid });

            // 🔥 核心優化：建立 Dictionary 快取，查詢速度 O(1)
            Dictionary<string, decimal> rainDict = WeaRainList.ToDictionary(r => r.Time, r => r.Hour);

            // 取得尚未執行的資料
            string noExecSql = @" 
                SELECT a.* FROM SoilWaterRainDataHis a
                LEFT JOIN DataStaticLog b ON CAST(a.no AS varchar) = b.key1 AND b.type = @LogType 
                WHERE a.STID = @stid AND b.no IS NULL 
                ORDER BY a.Time";
            List<SoilWaterRainDataHis> NoExecList = dbDapper.Query<SoilWaterRainDataHis>(noExecSql, new { stid = sStid, LogType = sSTIDTimeLogType });

            if (NoExecList.Count == 0) return;

            List<SoilWaterRainDataHis> updateBatch = new List<SoilWaterRainDataHis>();
            List<DataStaticLog> logBatch = new List<DataStaticLog>();
            int totalNoExec = NoExecList.Count;

            for (int i = 0; i < totalNoExec; i++)
            {
                SoilWaterRainDataHis item = NoExecList[i];
                DateTime MainTime = DateTime.ParseExact(item.Time, "yyyyMMddHH", null);

                if (i % 100 == 0 || i == totalNoExec - 1)
                {
                    progress.Report($"[{sStid}] ({currentIdx}/{totalCount}) 計算進度: {i + 1}/{totalNoExec} ({MainTime:yyyy/MM/dd-HH})");
                }

                // == 計算各項指標 ==
                decimal dHour3 = GetSumRangeRain(rainDict, MainTime, 0, 2);
                decimal dHour6 = GetSumRangeRain(rainDict, MainTime, 0, 5);
                decimal dHour12 = GetSumRangeRain(rainDict, MainTime, 0, 11);
                decimal dHour24 = GetSumRangeRain(rainDict, MainTime, 0, 23);

                // 計算日雨量
                DateTime dayStart = new DateTime(MainTime.Year, MainTime.Month, MainTime.Day, 23, 0, 0);
                decimal dFallDay = GetSumRangeRain(rainDict, dayStart, 0, 23);

                // == 計算 RT ==
                double alpha = 0.7;
                decimal dRT = GetSumRangeRain(rainDict, MainTime, 0, 23)
                            + GetSumRangeRain(rainDict, MainTime, 24, 47) * (decimal)Math.Pow(alpha, 1)
                            + GetSumRangeRain(rainDict, MainTime, 48, 71) * (decimal)Math.Pow(alpha, 2)
                            + GetSumRangeRain(rainDict, MainTime, 72, 95) * (decimal)Math.Pow(alpha, 3)
                            + GetSumRangeRain(rainDict, MainTime, 96, 119) * (decimal)Math.Pow(alpha, 4)
                            + GetSumRangeRain(rainDict, MainTime, 120, 143) * (decimal)Math.Pow(alpha, 5)
                            + GetSumRangeRain(rainDict, MainTime, 144, 167) * (decimal)Math.Pow(alpha, 6)
                            + GetSumRangeRain(rainDict, MainTime, 168, 191) * (decimal)Math.Pow(alpha, 7);

                // 賦值給物件
                item.Hour3 = dHour3;
                item.Hour6 = dHour6;
                item.Hour12 = dHour12;
                item.Hour24 = dHour24;
                item.DayRainfall = dFallDay;
                item.RT = dRT;

                updateBatch.Add(item);

                // 準備 Log 資料
                logBatch.Add(new DataStaticLog
                {
                    type = sSTIDTimeLogType,
                    key1 = item.no.ToString(),
                    key2 = item.Stid,
                    status = "60",
                    logtime = DateTime.Now
                });
            }

            // =========================================================
            // 🔥 終極極速寫入：SqlBulkCopy + 暫存表批次更新 (包含 Transaction)
            // =========================================================
            progress.Report($"[{sStid}] 運算完成，準備啟動 SqlBulkCopy 巨量寫入 (更新 {updateBatch.Count} 筆, 寫入 Log {logBatch.Count} 筆)...");

            // 1. 準備 Update 需要的 DataTable (水保局歷史資料)
            DataTable dtUpdate = new DataTable();
            dtUpdate.Columns.Add("no", typeof(int));
            dtUpdate.Columns.Add("Hour", typeof(decimal));
            dtUpdate.Columns.Add("Hour3", typeof(decimal));
            dtUpdate.Columns.Add("Hour6", typeof(decimal));
            dtUpdate.Columns.Add("Hour12", typeof(decimal));
            dtUpdate.Columns.Add("Hour24", typeof(decimal));
            dtUpdate.Columns.Add("DayRainfall", typeof(decimal));
            dtUpdate.Columns.Add("RT", typeof(decimal));

            foreach (var x in updateBatch)
            {
                // 將計算好的物件轉為 DataTable 列
                dtUpdate.Rows.Add(x.no, x.Hour, x.Hour3, x.Hour6, x.Hour12, x.Hour24, x.DayRainfall, x.RT);
            }

            // 2. 準備 Log 需要的 DataTable (轉檔紀錄)
            DataTable dtLog = new DataTable();
            dtLog.Columns.Add("type", typeof(string));
            dtLog.Columns.Add("key1", typeof(string));
            dtLog.Columns.Add("key2", typeof(string));
            dtLog.Columns.Add("status", typeof(string));
            dtLog.Columns.Add("logtime", typeof(DateTime));

            foreach (var log in logBatch)
            {
                dtLog.Rows.Add(log.type, log.key1, log.key2, log.status, log.logtime);
            }

            // 3. 開啟單一資料庫連線並啟動 Transaction 保護
            using (SqlConnection conn = new SqlConnection(dbDapper.ConnStr)) // 💡請記得替換為您的連線字串
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // (A) 在 MSSQL 建立輕量級 #Temp 暫存表 (使用 no 當作 PRIMARY KEY 加速 JOIN)
                        string createTempSql = @"
                        CREATE TABLE #TempSoilWaterUpdate (
                            no int PRIMARY KEY, 
                            [Hour] decimal(18,4), Hour3 decimal(18,4), Hour6 decimal(18,4), 
                            Hour12 decimal(18,4), Hour24 decimal(18,4), 
                            DayRainfall decimal(18,4), RT decimal(18,4)
                        );";
                        using (SqlCommand cmd = new SqlCommand(createTempSql, conn, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // (B) 透過 SqlBulkCopy 瞬間將資料倒進暫存表 (綁定 transaction)
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "#TempSoilWaterUpdate";
                            bulkCopy.BatchSize = 10000;
                            foreach (DataColumn col in dtUpdate.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            bulkCopy.WriteToServer(dtUpdate);
                        }

                        // (C) 讓 MSSQL 透過 JOIN 進行整批 UPDATE，完成後自動丟棄暫存表
                        string bulkUpdateSql = @"
                            UPDATE t
                            SET t.[Hour] = tmp.[Hour], 
                                t.Hour3 = tmp.Hour3, 
                                t.Hour6 = tmp.Hour6, 
                                t.Hour12 = tmp.Hour12, 
                                t.Hour24 = tmp.Hour24, 
                                t.DayRainfall = tmp.DayRainfall, 
                                t.RT = tmp.RT
                            FROM SoilWaterRainDataHis t
                            INNER JOIN #TempSoilWaterUpdate tmp ON t.no = tmp.no;
                
                            DROP TABLE #TempSoilWaterUpdate;
                        ";
                        using (SqlCommand cmd = new SqlCommand(bulkUpdateSql, conn, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // (D) 將 DataStaticLog 直接透過 SqlBulkCopy 寫入實體表 (綁定 transaction)
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "DataStaticLog";
                            bulkCopy.BatchSize = 10000;
                            // 🔥 同樣必須加上明確對應，跳過資料庫的 auto-increment (no) 欄位
                            foreach (DataColumn col in dtLog.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }

                            bulkCopy.WriteToServer(dtLog);
                        }

                        // 💥 所有動作皆成功，正式提交交易！
                        transaction.Commit();
                        progress.Report($"[{sStid}] 批次更新與 Log 寫入大成功！");
                    }
                    catch (Exception ex)
                    {
                        // 🚨 發生任何錯誤，全部退回原狀 (Rollback)！不會有任何髒資料殘留
                        transaction.Rollback();
                        progress.Report($"[{sStid}] 寫入發生嚴重錯誤: {ex.Message}，資料已還原 (Rollback)。");

                        // 拋出例外，讓外層 Catch 去接手 (標記轉檔狀態為 90)
                        throw;
                    }
                }
            }
        }



        /// <summary>
        /// 20230501
        /// 水利署(Wra)歷史資料轉檔()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button26Old_Click(object sender, EventArgs e)
        {
            //1.處理資料來源，已經由1-2匯入檔案資料到(WraRainDataHis及WeaRainData)
            //(1-2)按鈕匯入水利署CSV檔
            //比對兩個資料表的資料內容是否有相同筆數，如果筆數不相同則找出問題互相補遺
            /* SQL:
                select * from (
                    select STID,count(*) ttData from WeaRainData
	                where STID in (
		                select distinct STID from WraRainDataHis 
	                )
	                group by STID
                ) a 
                left join (
                    select STID,count(*) ttDataHis from WraRainDataHis  group by STID
                ) b on a.STID = b.STID 
                where a.ttData <> b.ttDataHis
            */
            //20240510 在(1-2)按鈕已經加上WraRainDataHis補遺到WeaRainData的功能
            //**但是WeaRainData補遺到WraRainDataHis，功能還沒完成
            //2.清除原本已經轉檔紀錄
            //(2-1)每年第一次轉檔時，請執行以下SQL，清除已轉檔完成或是執行中未清除的資料
            //      SQL：delete DataStaticLog where type = 'WraRainHisSTID' and status in ('10','60')
            //(2-2)不需要刪除任何的LOG紀錄，
            //     因為歷年已經跑過的資料，已經有LOG，如果篩除，會全部重跑
            //      SQL：delete DataStaticLog where type = 'WraRainHisSTIDTime'            

            //3.直接執行多個程式一起跑

            //4.最後比對是否有全部轉檔成功到歷史區
            /* SQL:
                    select * from (
                        select STID,count(*) ttData from WeaRainData
	                    where STID in (
		                    select distinct STID from WraRainDataHis 
	                    )
	                    group by STID
                    ) a 
                    left join (
                        select STID,count(*) ttDataHis from WraRainDataHis  group by STID
                    ) b on a.STID = b.STID 
                    where a.ttData <> b.ttDataHis
             * */

            string sStid = "";

            //轉檔註記
            //每個雨量站
            string sSTIDLogType = "WraRainHisSTID";
            //每個雨量站的時刻
            string sSTIDTimeLogType = "WraRainHisSTIDTime";


            //有輸入資料，則轉該雨量站的資料
            if (Trans109Text.Text.Trim() != "")
            {
                sStid = Trans109Text.Text;
                this.Text = sStid;
                ProcWraRainDataHisOld(sStid, sSTIDTimeLogType);
            }
            else //如果沒輸入，則由資料庫判斷尚未執行或還沒執行完全的雨量站進行轉檔
            {
                while (true)
                {
                    try
                    {
                        //比較該雨量站資料數量與轉檔LOG紀錄數量，如果沒轉過或是資料數不符，則進行轉檔
                        //20210506 新增排除已經在轉檔的雨量站
                        ssql = @" 
                                select * from (
                                    select * from (
                                        select STID,count(*) ttData from WraRainDataHis group by STID
                                    ) a 
                                    left join (
                                        select key2,count(*) ttLog from DataStaticLog where type = '{1}'  group by key2
                                    ) b on a.STID = b.key2 
                                ) r 
                                left join DataStaticLog c  on c.type = '{0}' and c.key1 = r.STID and c.status = '10'
                                where (ttLog != ttData or ttLog is null) and c.key1 is null    
                                order by r.STID
                            ";
                        ssql = string.Format(ssql, sSTIDLogType, sSTIDTimeLogType);

                        ShowStatus(string.Format("正在統計尚未轉檔到歷史資料的雨量站...."));

                        List<dynamic> TransList = dbDapper.Query(ssql);

                        //如果沒資料則停止
                        if (TransList.Count() == 0) break;

                        //總筆數，使用Random取得資料避免重複執行問題
                        int iRandom = new Random().Next(0, TransList.Count());

                        //預防重複執行後，LOG紀錄筆數>WeaRainData的筆數導致前面判斷未轉完畢的篩選，所以如果超過筆數，則刪除LOG重新跑
                        int ittLog = TransList[iRandom].ttLog == null ? 0 : TransList[iRandom].ttLog;
                        int ittData = TransList[iRandom].ttData == null ? 0 : TransList[iRandom].ttData;
                        if (ittLog > ittData)
                        {
                            //20240507 查詢問題後，發現LOG多餘的資料對應到WraRainDataHis，是NULL，所以刪除LOG紀錄NULL的資料，以免全部重跑耗費太多時間
                            ssql = @" select * from DataStaticLog a 
                                        left join WraRainDataHis b on  b.no = a.key1
                                        where a.type = '{0}' and b.STID is null and a.key2 = '{1}' ";
                            ssql = string.Format(ssql, sSTIDTimeLogType, TransList[iRandom].STID);
                            List<dynamic> ErrorList = dbDapper.Query(ssql);

                            foreach (dynamic item in ErrorList)
                            {
                                ssql = @" delete DataStaticLog where type = '{0}' and key2 = '{1}' and no = '{2}' ";
                                ssql = string.Format(ssql, sSTIDTimeLogType, item.key2, item.no);
                                dbDapper.Execute(ssql);
                            }

                            continue;

                            //ssql = @" delete from DataStaticLog where type = '{1}' and key2 = '{0}' ";
                            //ssql = string.Format(ssql, TransList[iRandom].STID, sSTIDTimeLogType);
                            //dbDapper.Execute(ssql);
                        }

                        sStid = TransList[iRandom].STID;
                        this.Text = string.Format("{0}-剩餘數量：{1}", sStid, TransList.Count());
                        Application.DoEvents();

                        //==判斷是否執行過
                        //10:執行中 60:已完成 90:轉檔失敗
                        //string sDataStaticLogType = "WeaRainTransHisSTID";
                        ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' ";
                        ssql = string.Format(ssql, sSTIDLogType, sStid);
                        DataStaticLog dsl_new = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);

                        //沒有轉檔紀錄
                        if (dsl_new == null)
                        {
                            //如果沒有轉檔紀錄，則新增一筆執行中
                            //寫入統計LOG
                            dsl_new = new DataStaticLog();
                            dsl_new.type = sSTIDLogType;
                            dsl_new.key1 = sStid;
                            dsl_new.key2 = "";
                            dsl_new.status = "10";
                            dsl_new.logtime = DateTime.Now;
                            dbDapper.Insert(dsl_new);
                        }
                        else//已經有轉檔紀錄
                        {
                            //如果執行中，則不進行該站轉檔
                            if (dsl_new.status == "10")
                            {
                                continue;
                            }

                            //重新更新為執行中
                            dsl_new.status = "10";
                            dbDapper.Update(dsl_new);
                        }

                        //主要轉檔程序
                        ProcWraRainDataHisOld(sStid, sSTIDTimeLogType);

                        //註記已經轉檔完成
                        ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' ";
                        ssql = string.Format(ssql, sSTIDLogType, sStid);

                        DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                        if (dslu != null)
                        {
                            dslu.status = "60";
                            dbDapper.Update(dslu);
                        }
                    }
                    catch (Exception ex)
                    {
                        //發生錯誤繼續後面的轉檔，直到全部雨量站執行完畢
                        continue;
                    }
                }
            }

            this.Text = "OK-ALL";
        }


        private void ProcWraRainDataHisOld(string STID, string sSTIDTimeLogType)
        {
            //雨量站
            string sStid = STID;
            //sStid = "466900";

            //string sDataStaticLogType = "109WeaRainTrans";


            //取得尚未轉檔完成的資料(KEY1:STID , KEY2:Time )
            ssql = @" 
                    select * from WraRainDataHis where STID = '{0}' order by time 
                    ";
            ssql = string.Format(ssql, sStid);

            ShowStatus(string.Format("取得雨量站[{0}]的所有水利署雨量資料....", sStid));
            List<WraRainDataHis> WraRainList = dbDapper.Query<WraRainDataHis>(ssql);


            //取得尚未轉檔的資料
            ssql = @" 
                    select a.* from WraRainDataHis a
                    left join DataStaticLog b on CAST( a.no as varchar) = b.key1 and  b.type = '{1}' 
                    where a.STID = '{0}' and b.no is  null 
                    order by a.time ";
            ssql = string.Format(ssql, sStid, sSTIDTimeLogType);
            ShowStatus(string.Format("取得雨量站[{0}]尚未執行的資料....", sStid));
            List<WraRainDataHis> NoExecList = dbDapper.Query<WraRainDataHis>(ssql);

            int iIndex1 = 0;
            foreach (WraRainDataHis item in NoExecList)
            {
                try
                {
                    iIndex1++;

                    //目前該筆資料主要時間
                    DateTime MainTime = DateTime.ParseExact(item.Time, "yyyyMMddHH", null);

                    ShowStatus(string.Format("站號：{2}[{3}/{4}]正在轉檔：{5}", "", "", sStid
                        , iIndex1.ToString(), NoExecList.Count().ToString(), MainTime.ToString("yyyy/MM/dd-HH")));

                    //==判斷是否執行過
                    //10:執行中 60:已完成 90:轉檔失敗
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'    ";
                    ssql = string.Format(ssql, sSTIDTimeLogType, item.no.ToString());
                    DataStaticLog dsl_new = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);

                    //沒有轉檔紀錄
                    if (dsl_new == null)
                    {
                        //如果沒有轉檔紀錄，則新增一筆執行中
                        //寫入統計LOG
                        dsl_new = new DataStaticLog();
                        dsl_new.type = sSTIDTimeLogType;
                        dsl_new.key1 = item.no.ToString();
                        dsl_new.key2 = item.Stid;
                        dsl_new.status = "10";
                        dsl_new.logtime = DateTime.Now;
                        dbDapper.Insert(dsl_new);
                    }
                    else//已經有轉檔紀錄
                    {
                        if (dsl_new.status == "10" || dsl_new.status == "60") //10:執行中 60:已完成
                        {
                            continue;
                        }

                        //重新更新為執行中
                        dsl_new.status = "10";
                        dbDapper.Update(dsl_new);
                    }


                    //取得當月及前一個月的資料
                    List<WraRainDataHis> wrdList = new List<WraRainDataHis>();
                    //取得當月資料
                    string sNowYM = MainTime.ToString("yyyyMM");
                    //取得前月資料
                    string sPreYM = MainTime.AddMonths(-1).ToString("yyyyMM");

                    List<WraRainDataHis> NowYMList = WraRainList.Where(element => element.Time.StartsWith(sNowYM)).ToList<WraRainDataHis>();
                    List<WraRainDataHis> PreYMList = WraRainList.Where(element => element.Time.StartsWith(sPreYM)).ToList<WraRainDataHis>();
                    wrdList.AddRange(NowYMList);
                    wrdList.AddRange(PreYMList);


                    //==計算Hour3
                    decimal dHour3 = getSumRangeRainWraOld(wrdList, item, 0, 2);

                    //==計算Hour6
                    decimal dHour6 = getSumRangeRainWraOld(wrdList, item, 0, 5);

                    //==計算Hour12
                    decimal dHour12 = getSumRangeRainWraOld(wrdList, item, 0, 11);

                    //==計算Hour24
                    decimal dHour24 = getSumRangeRainWraOld(wrdList, item, 0, 23);
                    //decimal dtestTest = getSumRangeRain(wrdList, item, 0, 23);
                    //if (dHour24 != dtestTest)
                    //{
                    //    string sErroe = "";
                    //}

                    //==計算HourDayRainFall
                    string sFallDay = item.Time.Substring(0, 8);
                    List<WraRainDataHis> DayRainFallList = WraRainList.Where(element => element.Time.StartsWith(sFallDay)).ToList<WraRainDataHis>();
                    decimal dFallDay = 0;
                    foreach (WraRainDataHis DayRainFall in DayRainFallList)
                    {
                        if (DayRainFall != null)
                        {
                            dFallDay += DayRainFall.Hour;
                        }
                    }

                    //==計算RT
                    decimal dRT = 0;
                    double α = 0.7;
                    //M：七天前期雨量，前頁P的計算
                    //如：2012 / 1 / 8 5:00起始，2012 / 1 / 8 13:00尖峰
                    //P = 2012 / 1 / 8 5:00~13:00累積雨量加總，雨量不折減
                    //＋2012 / 1 / 7 日累積雨量* 折減係數α＋2012 / 1 / 6 日累積雨量* 折減係數α 2
                    //＋2012 / 1 / 5 日累積雨量* 折減係數α 3 ＋2012 / 1 / 4 日累積雨量* 折減係數α 4
                    //＋2012 / 1 / 3 日累積雨量* 折減係數α 5 ＋2012 / 1 / 2 日累積雨量* 折減係數α 6
                    //＋2012 / 1 / 1 日累積雨量* 折減係數α

                    //Rt = 當下時刻~24小時內雨量加總 + 25hr~48hr雨量加總 * 0.7 + 49hr~72hr雨量加總 * 0.7 ^ 2 + .....+167hr~192hr雨量加總 * 0.7 ^ 7之總和


                    decimal dRange = getSumRangeRainWraOld(wrdList, item, 0, 23);
                    decimal dRange1 = getSumRangeRainWraOld(wrdList, item, 24, 47) * Convert.ToDecimal(Math.Pow(α, 1));
                    decimal dRange2 = getSumRangeRainWraOld(wrdList, item, 48, 71) * Convert.ToDecimal(Math.Pow(α, 2));
                    decimal dRange3 = getSumRangeRainWraOld(wrdList, item, 72, 95) * Convert.ToDecimal(Math.Pow(α, 3));
                    decimal dRange4 = getSumRangeRainWraOld(wrdList, item, 96, 119) * Convert.ToDecimal(Math.Pow(α, 4));
                    decimal dRange5 = getSumRangeRainWraOld(wrdList, item, 120, 143) * Convert.ToDecimal(Math.Pow(α, 5));
                    decimal dRange6 = getSumRangeRainWraOld(wrdList, item, 144, 167) * Convert.ToDecimal(Math.Pow(α, 6));
                    decimal dRange7 = getSumRangeRainWraOld(wrdList, item, 168, 191) * Convert.ToDecimal(Math.Pow(α, 7));

                    dRT = dRange + dRange1 + dRange2 + dRange3 + dRange4 + dRange5 + dRange6 + dRange7;


                    WraRainDataHis wraTemp = new WraRainDataHis();
                    wraTemp.Stid = item.Stid;
                    wraTemp.Time = MainTime.ToString("yyyyMMddHH");
                    wraTemp.Hour = item.Hour;
                    wraTemp.Hour3 = dHour3;
                    wraTemp.Hour6 = dHour6;
                    wraTemp.Hour12 = dHour12;
                    wraTemp.Hour24 = dHour24;
                    wraTemp.DayRainfall = dFallDay;
                    wraTemp.RT = dRT;

                    //紀錄氣象局雨量歷史資料(時間序改為微軟標準0-23為一天)
                    ssql = " select * from WraRainDataHis where Stid = '{0}' and Time = '{1}' ";
                    ssql = string.Format(ssql, wraTemp.Stid, wraTemp.Time);
                    WraRainDataHis wra = dbDapper.QuerySingleOrDefault<WraRainDataHis>(ssql);
                    if (wra == null)
                    {
                        wra = wraTemp;

                        dbDapper.Insert(wra);
                    }
                    else
                    {
                        wra.Hour = wraTemp.Hour;
                        wra.Hour3 = wraTemp.Hour3;
                        wra.Hour6 = wraTemp.Hour6;
                        wra.Hour12 = wraTemp.Hour12;
                        wra.Hour24 = wraTemp.Hour24;
                        wra.DayRainfall = wraTemp.DayRainfall;
                        wra.RT = wraTemp.RT;

                        dbDapper.Update(wra);
                    }

                    //註記已經轉檔完成
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'    ";
                    ssql = string.Format(ssql, sSTIDTimeLogType, item.no.ToString());

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "60";
                        dbDapper.Update(dslu);
                    }


                }
                catch (Exception ex)
                {
                    //註記轉檔失敗
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' ";
                    ssql = string.Format(ssql, sSTIDTimeLogType, item.no.ToString());

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "90";
                        dbDapper.Update(dslu);
                    }
                    continue;
                }
            }
        }

        /// <summary>
        /// 計算區間雨量總和
        /// </summary>
        /// <param name="AllList">該站號所有雨量資料</param>
        /// <param name="Main">主要雨量資料</param>
        /// <param name="iStart">計算的起始雨量區間</param>
        /// <param name="iEnd">計算的結束雨量區間</param>
        /// <returns></returns>
        private decimal getSumRangeRainWraOld(List<WraRainDataHis> AllList, WraRainDataHis Main, int iStart, int iEnd)
        {
            decimal dSum = 0;

            //目前該筆資料主要時間
            DateTime MainTime = DateTime.ParseExact(Convert.ToString(Convert.ToInt32(Main.Time)), "yyyyMMddHH", null);

            //取得起始時間
            DateTime StartTime = MainTime.AddHours(-iStart);

            //總共要計算多少次
            int iCalCount = iEnd - iStart + 1;

            for (int i = 0; i < iCalCount; i++)
            {
                DateTime CalTime = StartTime.AddHours(-i);
                int iTime = Convert.ToInt32(CalTime.ToString("yyyyMMddHH"));
                WraRainDataHis wrdCal = AllList.SingleOrDefault(s => s.Time == iTime.ToString());

                if (wrdCal != null)
                {
                    dSum += wrdCal.Hour;
                }
            }

            return dSum;
        }

        private async void button26_Click(object sender, EventArgs e)
        {
            //1.處理資料來源，已經由1-2匯入檔案資料到(WraRainDataHis及WeaRainData)
            //(1-2)按鈕匯入水利署CSV檔
            //比對兩個資料表的資料內容是否有相同筆數，如果筆數不相同則找出問題互相補遺
            /* SQL:
                select * from (
                    select STID,count(*) ttData from WeaRainData
	                where STID in (
		                select distinct STID from WraRainDataHis 
	                )
	                group by STID
                ) a 
                left join (
                    select STID,count(*) ttDataHis from WraRainDataHis  group by STID
                ) b on a.STID = b.STID 
                where a.ttData <> b.ttDataHis
            */
            //20240510 在(1-2)按鈕已經加上WraRainDataHis補遺到WeaRainData的功能
            //**但是WeaRainData補遺到WraRainDataHis，功能還沒完成
            //2.清除原本已經轉檔紀錄
            //(2-1)每年第一次轉檔時，請執行以下SQL，清除已轉檔完成或是執行中未清除的資料
            //      SQL：delete DataStaticLog where type = 'WraRainHisSTID' and status in ('10','60')
            //(2-2)不需要刪除任何的LOG紀錄，
            //     因為歷年已經跑過的資料，已經有LOG，如果篩除，會全部重跑
            //      SQL：delete DataStaticLog where type = 'WraRainHisSTIDTime'            

            //3.直接執行多個程式一起跑

            //4.最後比對是否有全部轉檔成功到歷史區
            /* SQL:
                    select * from (
                        select STID,count(*) ttData from WeaRainData
	                    where STID in (
		                    select distinct STID from WraRainDataHis 
	                    )
	                    group by STID
                    ) a 
                    left join (
                        select STID,count(*) ttDataHis from WraRainDataHis  group by STID
                    ) b on a.STID = b.STID 
                    where a.ttData <> b.ttDataHis
             * */

            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            var progress = new Progress<string>(msg =>
            {
                LogToMsgText(msg); // 替換為您的 ShowStatus
                this.Text = msg;
            });

            try
            {
                await Task.Run(() => ProcessWraDataTrans(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"水利署轉檔發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
                this.Text = "OK-ALL";
            }
        }

        // =========================================================
        // 核心歷史資料轉檔邏輯 (支援多程式併發分工)
        // =========================================================
        private void ProcessWraDataTrans(IProgress<string> progress)
        {
            string sSTIDLogType = "WraRainHisSTID";
            string sSTIDTimeLogType = "WraRainHisSTIDTime";
            string sStid = "";

            // 如果 UI 有輸入特定站號
            if (!string.IsNullOrWhiteSpace(Trans109Text.Text))
            {
                sStid = Trans109Text.Text.Trim();
                ProcWraRainDataHis(sStid, sSTIDTimeLogType, progress, 1, 1);
                return;
            }

            progress.Report("正在統計尚未完成水利署轉檔的雨量站....");

            // 取得所有尚未完成的雨量站清單 (利用 NOT EXISTS 與數量比對提升效能)
            string ssqlMain = @"
                SELECT a.STID 
                FROM (
                    SELECT STID, COUNT(*) AS ttData FROM WraRainDataHis GROUP BY STID
                ) a
                LEFT JOIN (
                    SELECT key2, COUNT(*) AS ttLog FROM DataStaticLog WHERE type = @TypeTime GROUP BY key2
                ) b ON a.STID = b.key2
                WHERE (b.ttLog IS NULL OR b.ttLog != a.ttData)
                  AND NOT EXISTS (
                      SELECT 1 FROM DataStaticLog c 
                      WHERE c.type = @TypeStid AND c.key1 = a.STID AND c.status = '10'
                  )
                ORDER BY a.STID";

            List<dynamic> TransList = dbDapper.Query(ssqlMain, new { TypeTime = sSTIDTimeLogType, TypeStid = sSTIDLogType }, commandTimeout: 300);
            string ssqlStatus10 = $"select * from  DataStaticLog where type = '{sSTIDLogType}' and status in ('10')";
            int iStatus10 = dbDapper.QueryTotalCount(ssqlStatus10);

            if (TransList.Count == 0 && iStatus10 == 0)
            {
                progress.Report("檢核資料完整性後，水利署轉檔-全部完成....");
                return;
            }
            else if (TransList.Count == 0 && iStatus10 != 0)
            {
                progress.Report($"檢核資料完整性後，水利署轉檔-尚未完成....執行中(10)狀態筆數：{iStatus10}");
                return;
            }

            List<string> stidList = TransList.Select(d => (string)d.STID).ToList();

            // 將清單洗牌，防止多開程式撞車
            stidList = stidList.OrderBy(x => Guid.NewGuid()).ToList();

            int totalCount = stidList.Count;
            int currentIndex = 0;

            foreach (string stid in stidList)
            {
                currentIndex++;
                sStid = stid;

                try
                {
                    progress.Report($"[{sStid}] ({currentIndex}/{totalCount}) 爭取轉檔權限...");

                    // =========================================================
                    // 1. 極速搶佔機制 (原子操作 + 排他鎖)
                    // =========================================================
                    string claimSql = @"
                        BEGIN TRAN;
                        DECLARE @Claimed INT = 0;
                
                        IF NOT EXISTS (SELECT 1 FROM DataStaticLog WITH (UPDLOCK, HOLDLOCK) WHERE type = @Type AND key1 = @Stid)
                        BEGIN
                            INSERT INTO DataStaticLog (type, key1, key2, status, logtime) VALUES (@Type, @Stid, '', '10', GETDATE());
                            SET @Claimed = 1;
                        END
                        ELSE
                        BEGIN
                            UPDATE DataStaticLog
                            SET status = '10', logtime = GETDATE()
                            WHERE type = @Type AND key1 = @Stid AND status NOT IN ('10', '60');
                    
                            IF @@ROWCOUNT > 0 SET @Claimed = 1;
                        END
                
                        COMMIT TRAN;
                        SELECT @Claimed;
                    ";

                    int isClaimed = Convert.ToInt32(dbDapper.ExecuteScale(claimSql, new { Type = sSTIDLogType, Stid = sStid }));
                    if (isClaimed == 0) continue; // 搶佔失敗，換下一站

                    // =========================================================
                    // 2. 執行主要計算程式
                    // =========================================================
                    ProcWraRainDataHis(sStid, sSTIDTimeLogType, progress, currentIndex, totalCount);

                    // 3. 註記該站點轉檔完成
                    string finishSql = "UPDATE DataStaticLog SET status = '60', logtime = GETDATE() WHERE type = @Type AND key1 = @Stid";
                    dbDapper.Execute(finishSql, new { Type = sSTIDLogType, Stid = sStid });
                }
                catch (Exception ex)
                {
                    // 發生錯誤，跳過並處理下一站
                    progress.Report($"[{sStid}] 發生錯誤跳過: {ex.Message}");

                    // 註記發生錯誤 90
                    string errSql = "UPDATE DataStaticLog SET status = '90', logtime = GETDATE() WHERE type = @Type AND key1 = @Stid";
                    dbDapper.Execute(errSql, new { Type = sSTIDLogType, Stid = sStid });
                    continue;
                }
            }


            //最後在檢核一次是否還有沒跑完的資料
            TransList.Clear();
            TransList = dbDapper.Query(ssqlMain, new { TypeTime = sSTIDTimeLogType, TypeStid = sSTIDLogType }, commandTimeout: 300);
            iStatus10 = dbDapper.QueryTotalCount(ssqlStatus10);

            if (TransList.Count == 0 && iStatus10 == 0)
            {
                progress.Report("檢核資料完整性後，水利署轉檔-全部完成....");
            }
            else
            {
                progress.Report($"檢核資料完整性後，水利署轉檔-尚未完成....執行中(10)狀態筆數：{iStatus10}");
                return;
            }
        }

        private void ProcWraRainDataHis(string sStid, string sSTIDTimeLogType, IProgress<string> progress, int currentIdx, int totalCount)
        {
            progress.Report($"[{sStid}] 讀取水利署雨量資料至記憶體...");

            // 取得該站所有歷史資料
            string ssql = "SELECT * FROM WraRainDataHis WHERE STID = @stid ORDER BY [Time]";
            List<WraRainDataHis> WeaRainList = dbDapper.Query<WraRainDataHis>(ssql, new { stid = sStid });

            // 🔥 核心優化：建立 Dictionary 快取，查詢速度 O(1)
            Dictionary<string, decimal> rainDict = WeaRainList.ToDictionary(r => r.Time, r => r.Hour);

            // 取得尚未執行的資料
            string noExecSql = @" 
                SELECT a.* FROM WraRainDataHis a
                LEFT JOIN DataStaticLog b ON CAST(a.no AS varchar) = b.key1 AND b.type = @LogType 
                WHERE a.STID = @stid AND b.no IS NULL 
                ORDER BY a.Time";
            List<WraRainDataHis> NoExecList = dbDapper.Query<WraRainDataHis>(noExecSql, new { stid = sStid, LogType = sSTIDTimeLogType });

            if (NoExecList.Count == 0) return;

            List<WraRainDataHis> updateBatch = new List<WraRainDataHis>();
            List<DataStaticLog> logBatch = new List<DataStaticLog>();
            int totalNoExec = NoExecList.Count;

            for (int i = 0; i < totalNoExec; i++)
            {
                WraRainDataHis item = NoExecList[i];
                DateTime MainTime = DateTime.ParseExact(item.Time, "yyyyMMddHH", null);

                // 每 100 筆更新一次 UI 避免閃爍
                if (i % 100 == 0 || i == totalNoExec - 1)
                {
                    progress.Report($"[{sStid}] ({currentIdx}/{totalCount}) 計算進度: {i + 1}/{totalNoExec} ({MainTime:yyyy/MM/dd-HH})");
                }

                // == 計算各項指標 ==
                decimal dHour3 = GetSumRangeRain(rainDict, MainTime, 0, 2);
                decimal dHour6 = GetSumRangeRain(rainDict, MainTime, 0, 5);
                decimal dHour12 = GetSumRangeRain(rainDict, MainTime, 0, 11);
                decimal dHour24 = GetSumRangeRain(rainDict, MainTime, 0, 23);

                // 計算日雨量：推移至該日的 23 點往前算 24 小時，等同於擷取 "yyyyMMdd" 的總和
                DateTime dayStart = new DateTime(MainTime.Year, MainTime.Month, MainTime.Day, 23, 0, 0);
                decimal dFallDay = GetSumRangeRain(rainDict, dayStart, 0, 23);

                // == 計算 RT ==
                double alpha = 0.7;
                decimal dRT = GetSumRangeRain(rainDict, MainTime, 0, 23)
                            + GetSumRangeRain(rainDict, MainTime, 24, 47) * (decimal)Math.Pow(alpha, 1)
                            + GetSumRangeRain(rainDict, MainTime, 48, 71) * (decimal)Math.Pow(alpha, 2)
                            + GetSumRangeRain(rainDict, MainTime, 72, 95) * (decimal)Math.Pow(alpha, 3)
                            + GetSumRangeRain(rainDict, MainTime, 96, 119) * (decimal)Math.Pow(alpha, 4)
                            + GetSumRangeRain(rainDict, MainTime, 120, 143) * (decimal)Math.Pow(alpha, 5)
                            + GetSumRangeRain(rainDict, MainTime, 144, 167) * (decimal)Math.Pow(alpha, 6)
                            + GetSumRangeRain(rainDict, MainTime, 168, 191) * (decimal)Math.Pow(alpha, 7);

                // 賦值給物件
                item.Hour3 = dHour3;
                item.Hour6 = dHour6;
                item.Hour12 = dHour12;
                item.Hour24 = dHour24;
                item.DayRainfall = dFallDay;
                item.RT = dRT;

                updateBatch.Add(item);

                // 準備 Log 資料
                logBatch.Add(new DataStaticLog
                {
                    type = sSTIDTimeLogType,
                    key1 = item.no.ToString(),
                    key2 = item.Stid,
                    status = "60",
                    logtime = DateTime.Now
                });
            }


            // =========================================================
            // 🔥 終極極速寫入：SqlBulkCopy + 暫存表 + 完整 Transaction 保護
            // =========================================================
            progress.Report($"[{sStid}] 運算完成，準備啟動具備 Transaction 保護的巨量寫入 ({updateBatch.Count} 筆)...");

            // 1. 準備 Update 需要的 DataTable
            DataTable dtUpdate = new DataTable();
            dtUpdate.Columns.Add("no", typeof(int));
            dtUpdate.Columns.Add("Hour", typeof(decimal));
            dtUpdate.Columns.Add("Hour3", typeof(decimal));
            dtUpdate.Columns.Add("Hour6", typeof(decimal));
            dtUpdate.Columns.Add("Hour12", typeof(decimal));
            dtUpdate.Columns.Add("Hour24", typeof(decimal));
            dtUpdate.Columns.Add("DayRainfall", typeof(decimal));
            dtUpdate.Columns.Add("RT", typeof(decimal));

            foreach (var x in updateBatch)
            {
                dtUpdate.Rows.Add(x.no, x.Hour, x.Hour3, x.Hour6, x.Hour12, x.Hour24, x.DayRainfall, x.RT);
            }

            // 2. 準備 Log 需要的 DataTable
            DataTable dtLog = new DataTable();
            dtLog.Columns.Add("type", typeof(string));
            dtLog.Columns.Add("key1", typeof(string));
            dtLog.Columns.Add("key2", typeof(string));
            dtLog.Columns.Add("status", typeof(string));
            dtLog.Columns.Add("logtime", typeof(DateTime));

            foreach (var log in logBatch)
            {
                dtLog.Rows.Add(sSTIDTimeLogType, log.key1, log.key2, "60", DateTime.Now);
            }

            // 3. 開啟單一資料庫連線並啟動 Transaction
            // 若您使用 dbDapper.Connection.ConnectionString，可以直接帶入
            using (SqlConnection conn = new SqlConnection(dbDapper.ConnStr))
            {
                conn.Open();

                // 啟動交易
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // (A) 建立 #Temp 暫存表 (綁定 transaction)
                        string createTempSql = @"
                        CREATE TABLE #TempWraUpdate (
                            no int PRIMARY KEY, 
                            [Hour] decimal(18,4), Hour3 decimal(18,4), Hour6 decimal(18,4), 
                            Hour12 decimal(18,4), Hour24 decimal(18,4), 
                            DayRainfall decimal(18,4), RT decimal(18,4)
                        );";
                        using (SqlCommand cmd = new SqlCommand(createTempSql, conn, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // (B) 透過 SqlBulkCopy 瞬間將資料倒進暫存表 (綁定 transaction)
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "#TempWraUpdate";
                            bulkCopy.BatchSize = 10000;
                            foreach (DataColumn col in dtUpdate.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            bulkCopy.WriteToServer(dtUpdate);
                        }

                        // (C) 讓 MSSQL 進行整批 UPDATE 並丟棄暫存表 (綁定 transaction)
                        string updateSql = @"
                            UPDATE t
                            SET t.[Hour] = tmp.[Hour], 
                                t.Hour3 = tmp.Hour3, 
                                t.Hour6 = tmp.Hour6, 
                                t.Hour12 = tmp.Hour12, 
                                t.Hour24 = tmp.Hour24, 
                                t.DayRainfall = tmp.DayRainfall, 
                                t.RT = tmp.RT
                            FROM WraRainDataHis t
                            INNER JOIN #TempWraUpdate tmp ON t.no = tmp.no;
                
                            DROP TABLE #TempWraUpdate;
                        ";
                        using (SqlCommand cmd = new SqlCommand(updateSql, conn, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        // (D) 將 DataStaticLog 也透過 SqlBulkCopy 寫入 (綁定 transaction)
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "DataStaticLog";
                            foreach (DataColumn col in dtLog.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            bulkCopy.WriteToServer(dtLog);
                        }

                        // 💥 所有動作皆成功，正式提交交易！
                        transaction.Commit();
                        progress.Report($"[{sStid}] 批次更新與 Log 寫入成功！");
                    }
                    catch (Exception ex)
                    {
                        // 🚨 發生任何錯誤，全部還原 (Rollback)！不會留下半殘的資料
                        transaction.Rollback();
                        progress.Report($"[{sStid}] 寫入發生嚴重錯誤，資料已全部還原 (Rollback)。");

                        // 將錯誤往外拋，讓最外層的 catch 區塊去捕捉並將測站狀態標記為 90 (失敗)
                        throw;
                    }
                }
            }
        }


        /// <summary>
        /// 20260329
        /// 北市府(TCG)歷史資料轉檔()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button34Old_Click(object sender, EventArgs e)
        {
            //1.處理資料來源，已經由1-2匯入檔案資料到(TCGRainDataHis及WeaRainData)
            //(1-2)按鈕匯入北市府CSV檔
            //比對兩個資料表的資料內容是否有相同筆數，如果筆數不相同則找出問題互相補遺
            /* SQL:
                select * from (
                    select STID,count(*) ttData from WeaRainData
	                where STID in (
		                select distinct STID from TCGRainDataHis 
	                )
	                group by STID
                ) a 
                left join (
                    select STID,count(*) ttDataHis from TCGRainDataHis  group by STID
                ) b on a.STID = b.STID 
                where a.ttData <> b.ttDataHis
            */
            //20240510 在(1-2)按鈕已經加上TCGRainDataHis補遺到WeaRainData的功能
            //**但是WeaRainData補遺到TCGRainDataHis，功能還沒完成
            //2.清除原本已經轉檔紀錄
            //(2-1)每年第一次轉檔時，請執行以下SQL，清除已轉檔完成或是執行中未清除的資料
            //      SQL：delete DataStaticLog where type = 'TCGRainHisSTID' and status in ('10','60')
            //(2-2)不需要刪除任何的LOG紀錄，
            //     因為歷年已經跑過的資料，已經有LOG，如果篩除，會全部重跑
            //      SQL：delete DataStaticLog where type = 'TCGRainHisSTIDTime'            

            //3.直接執行多個程式一起跑

            //4.最後比對是否有全部轉檔成功到歷史區
            /* SQL:
                    select * from (
                        select STID,count(*) ttData from WeaRainData
	                    where STID in (
		                    select distinct STID from TCGRainDataHis 
	                    )
	                    group by STID
                    ) a 
                    left join (
                        select STID,count(*) ttDataHis from TCGRainDataHis  group by STID
                    ) b on a.STID = b.STID 
                    where a.ttData <> b.ttDataHis
             * */

            string sStid = "";

            //轉檔註記
            //每個雨量站
            string sSTIDLogType = "TCGRainHisSTID";
            //每個雨量站的時刻
            string sSTIDTimeLogType = "TCGRainHisSTIDTime";


            //有輸入資料，則轉該雨量站的資料
            if (Trans109Text.Text.Trim() != "")
            {
                sStid = Trans109Text.Text;
                this.Text = sStid;
                ProcTCGRainDataHisOld(sStid, sSTIDTimeLogType);
            }
            else //如果沒輸入，則由資料庫判斷尚未執行或還沒執行完全的雨量站進行轉檔
            {
                while (true)
                {
                    try
                    {
                        //比較該雨量站資料數量與轉檔LOG紀錄數量，如果沒轉過或是資料數不符，則進行轉檔
                        //20210506 新增排除已經在轉檔的雨量站
                        ssql = @" 
                                select * from (
                                    select * from (
                                        select STID,count(*) ttData from TCGRainDataHis group by STID
                                    ) a 
                                    left join (
                                        select key2,count(*) ttLog from DataStaticLog where type = '{1}'  group by key2
                                    ) b on a.STID = b.key2 
                                ) r 
                                left join DataStaticLog c  on c.type = '{0}' and c.key1 = r.STID and c.status = '10'
                                where (ttLog != ttData or ttLog is null) and c.key1 is null    
                                order by r.STID
                            ";
                        ssql = string.Format(ssql, sSTIDLogType, sSTIDTimeLogType);

                        ShowStatus(string.Format("正在統計尚未轉檔到歷史資料的雨量站...."));

                        List<dynamic> TransList = dbDapper.Query(ssql);

                        //如果沒資料則停止
                        if (TransList.Count() == 0) break;

                        //總筆數，使用Random取得資料避免重複執行問題
                        int iRandom = new Random().Next(0, TransList.Count());

                        //預防重複執行後，LOG紀錄筆數>WeaRainData的筆數導致前面判斷未轉完畢的篩選，所以如果超過筆數，則刪除LOG重新跑
                        int ittLog = TransList[iRandom].ttLog == null ? 0 : TransList[iRandom].ttLog;
                        int ittData = TransList[iRandom].ttData == null ? 0 : TransList[iRandom].ttData;
                        if (ittLog > ittData)
                        {
                            //20240507 查詢問題後，發現LOG多餘的資料對應到WraRainDataHis，是NULL，所以刪除LOG紀錄NULL的資料，以免全部重跑耗費太多時間
                            ssql = @" select * from DataStaticLog a 
                                        left join TCGRainDataHis b on  b.no = a.key1
                                        where a.type = '{0}' and b.STID is null and a.key2 = '{1}' ";
                            ssql = string.Format(ssql, sSTIDTimeLogType, TransList[iRandom].STID);
                            List<dynamic> ErrorList = dbDapper.Query(ssql);

                            foreach (dynamic item in ErrorList)
                            {
                                ssql = @" delete DataStaticLog where type = '{0}' and key2 = '{1}' and no = '{2}' ";
                                ssql = string.Format(ssql, sSTIDTimeLogType, item.key2, item.no);
                                dbDapper.Execute(ssql);
                            }

                            continue;

                            //ssql = @" delete from DataStaticLog where type = '{1}' and key2 = '{0}' ";
                            //ssql = string.Format(ssql, TransList[iRandom].STID, sSTIDTimeLogType);
                            //dbDapper.Execute(ssql);
                        }

                        sStid = TransList[iRandom].STID;
                        this.Text = string.Format("{0}-剩餘數量：{1}", sStid, TransList.Count());
                        Application.DoEvents();

                        //==判斷是否執行過
                        //10:執行中 60:已完成 90:轉檔失敗
                        //string sDataStaticLogType = "WeaRainTransHisSTID";
                        ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' ";
                        ssql = string.Format(ssql, sSTIDLogType, sStid);
                        DataStaticLog dsl_new = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);

                        //沒有轉檔紀錄
                        if (dsl_new == null)
                        {
                            //如果沒有轉檔紀錄，則新增一筆執行中
                            //寫入統計LOG
                            dsl_new = new DataStaticLog();
                            dsl_new.type = sSTIDLogType;
                            dsl_new.key1 = sStid;
                            dsl_new.key2 = "";
                            dsl_new.status = "10";
                            dsl_new.logtime = DateTime.Now;
                            dbDapper.Insert(dsl_new);
                        }
                        else//已經有轉檔紀錄
                        {
                            //如果執行中，則不進行該站轉檔
                            if (dsl_new.status == "10")
                            {
                                continue;
                            }

                            //重新更新為執行中
                            dsl_new.status = "10";
                            dbDapper.Update(dsl_new);
                        }

                        //主要轉檔程序
                        ProcTCGRainDataHisOld(sStid, sSTIDTimeLogType);

                        //註記已經轉檔完成
                        ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' ";
                        ssql = string.Format(ssql, sSTIDLogType, sStid);

                        DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                        if (dslu != null)
                        {
                            dslu.status = "60";
                            dbDapper.Update(dslu);
                        }
                    }
                    catch (Exception ex)
                    {
                        //發生錯誤繼續後面的轉檔，直到全部雨量站執行完畢
                        continue;
                    }
                }
            }

            this.Text = "OK-ALL";
        }

        private void ProcTCGRainDataHisOld(string STID, string sSTIDTimeLogType)
        {
            //雨量站
            string sStid = STID;

            //取得尚未轉檔完成的資料(KEY1:STID , KEY2:Time )
            ssql = @" 
                    select * from TCGRainDataHis where STID = '{0}' order by time 
                    ";
            ssql = string.Format(ssql, sStid);

            ShowStatus(string.Format("取得雨量站[{0}]的所有水利署雨量資料....", sStid));
            List<TCGRainDataHis> TCGRainList = dbDapper.Query<TCGRainDataHis>(ssql);


            //取得尚未轉檔的資料
            ssql = @" 
                    select a.* from TCGRainDataHis a
                    left join DataStaticLog b on CAST( a.no as varchar) = b.key1 and  b.type = '{1}' 
                    where a.STID = '{0}' and b.no is  null 
                    order by a.time ";
            ssql = string.Format(ssql, sStid, sSTIDTimeLogType);
            ShowStatus(string.Format("取得雨量站[{0}]尚未執行的資料....", sStid));
            List<TCGRainDataHis> NoExecList = dbDapper.Query<TCGRainDataHis>(ssql);

            int iIndex1 = 0;
            foreach (TCGRainDataHis item in NoExecList)
            {
                try
                {
                    iIndex1++;

                    //目前該筆資料主要時間
                    DateTime MainTime = DateTime.ParseExact(item.Time, "yyyyMMddHH", null);

                    ShowStatus(string.Format("站號：{2}[{3}/{4}]正在轉檔：{5}", "", "", sStid
                        , iIndex1.ToString(), NoExecList.Count().ToString(), MainTime.ToString("yyyy/MM/dd-HH")));

                    //==判斷是否執行過
                    //10:執行中 60:已完成 90:轉檔失敗
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'    ";
                    ssql = string.Format(ssql, sSTIDTimeLogType, item.no.ToString());
                    DataStaticLog dsl_new = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);

                    //沒有轉檔紀錄
                    if (dsl_new == null)
                    {
                        //如果沒有轉檔紀錄，則新增一筆執行中
                        //寫入統計LOG
                        dsl_new = new DataStaticLog();
                        dsl_new.type = sSTIDTimeLogType;
                        dsl_new.key1 = item.no.ToString();
                        dsl_new.key2 = item.Stid;
                        dsl_new.status = "10";
                        dsl_new.logtime = DateTime.Now;
                        dbDapper.Insert(dsl_new);
                    }
                    else//已經有轉檔紀錄
                    {
                        if (dsl_new.status == "10" || dsl_new.status == "60") //10:執行中 60:已完成
                        {
                            continue;
                        }

                        //重新更新為執行中
                        dsl_new.status = "10";
                        dbDapper.Update(dsl_new);
                    }


                    //取得當月及前一個月的資料
                    List<TCGRainDataHis> TCGList = new List<TCGRainDataHis>();
                    //取得當月資料
                    string sNowYM = MainTime.ToString("yyyyMM");
                    //取得前月資料
                    string sPreYM = MainTime.AddMonths(-1).ToString("yyyyMM");

                    List<TCGRainDataHis> NowYMList = TCGRainList.Where(element => element.Time.StartsWith(sNowYM)).ToList<TCGRainDataHis>();
                    List<TCGRainDataHis> PreYMList = TCGRainList.Where(element => element.Time.StartsWith(sPreYM)).ToList<TCGRainDataHis>();
                    TCGList.AddRange(NowYMList);
                    TCGList.AddRange(PreYMList);


                    //==計算Hour3
                    decimal dHour3 = getSumRangeRainTCGOld(TCGList, item, 0, 2);

                    //==計算Hour6
                    decimal dHour6 = getSumRangeRainTCGOld(TCGList, item, 0, 5);

                    //==計算Hour12
                    decimal dHour12 = getSumRangeRainTCGOld(TCGList, item, 0, 11);

                    //==計算Hour24
                    decimal dHour24 = getSumRangeRainTCGOld(TCGList, item, 0, 23);
                    //decimal dtestTest = getSumRangeRain(wrdList, item, 0, 23);
                    //if (dHour24 != dtestTest)
                    //{
                    //    string sErroe = "";
                    //}

                    //==計算HourDayRainFall
                    string sFallDay = item.Time.Substring(0, 8);
                    List<TCGRainDataHis> DayRainFallList = TCGRainList.Where(element => element.Time.StartsWith(sFallDay)).ToList<TCGRainDataHis>();
                    decimal dFallDay = 0;
                    foreach (TCGRainDataHis DayRainFall in DayRainFallList)
                    {
                        if (DayRainFall != null)
                        {
                            dFallDay += DayRainFall.Hour;
                        }
                    }

                    //==計算RT
                    decimal dRT = 0;
                    double α = 0.7;
                    //M：七天前期雨量，前頁P的計算
                    //如：2012 / 1 / 8 5:00起始，2012 / 1 / 8 13:00尖峰
                    //P = 2012 / 1 / 8 5:00~13:00累積雨量加總，雨量不折減
                    //＋2012 / 1 / 7 日累積雨量* 折減係數α＋2012 / 1 / 6 日累積雨量* 折減係數α 2
                    //＋2012 / 1 / 5 日累積雨量* 折減係數α 3 ＋2012 / 1 / 4 日累積雨量* 折減係數α 4
                    //＋2012 / 1 / 3 日累積雨量* 折減係數α 5 ＋2012 / 1 / 2 日累積雨量* 折減係數α 6
                    //＋2012 / 1 / 1 日累積雨量* 折減係數α

                    //Rt = 當下時刻~24小時內雨量加總 + 25hr~48hr雨量加總 * 0.7 + 49hr~72hr雨量加總 * 0.7 ^ 2 + .....+167hr~192hr雨量加總 * 0.7 ^ 7之總和


                    decimal dRange = getSumRangeRainTCGOld(TCGList, item, 0, 23);
                    decimal dRange1 = getSumRangeRainTCGOld(TCGList, item, 24, 47) * Convert.ToDecimal(Math.Pow(α, 1));
                    decimal dRange2 = getSumRangeRainTCGOld(TCGList, item, 48, 71) * Convert.ToDecimal(Math.Pow(α, 2));
                    decimal dRange3 = getSumRangeRainTCGOld(TCGList, item, 72, 95) * Convert.ToDecimal(Math.Pow(α, 3));
                    decimal dRange4 = getSumRangeRainTCGOld(TCGList, item, 96, 119) * Convert.ToDecimal(Math.Pow(α, 4));
                    decimal dRange5 = getSumRangeRainTCGOld(TCGList, item, 120, 143) * Convert.ToDecimal(Math.Pow(α, 5));
                    decimal dRange6 = getSumRangeRainTCGOld(TCGList, item, 144, 167) * Convert.ToDecimal(Math.Pow(α, 6));
                    decimal dRange7 = getSumRangeRainTCGOld(TCGList, item, 168, 191) * Convert.ToDecimal(Math.Pow(α, 7));

                    dRT = dRange + dRange1 + dRange2 + dRange3 + dRange4 + dRange5 + dRange6 + dRange7;


                    TCGRainDataHis TCGTemp = new TCGRainDataHis();
                    TCGTemp.Stid = item.Stid;
                    TCGTemp.Time = MainTime.ToString("yyyyMMddHH");
                    TCGTemp.Hour = item.Hour;
                    TCGTemp.Hour3 = dHour3;
                    TCGTemp.Hour6 = dHour6;
                    TCGTemp.Hour12 = dHour12;
                    TCGTemp.Hour24 = dHour24;
                    TCGTemp.DayRainfall = dFallDay;
                    TCGTemp.RT = dRT;

                    //紀錄氣象局雨量歷史資料(時間序改為微軟標準0-23為一天)
                    ssql = " select * from TCGRainDataHis where Stid = '{0}' and Time = '{1}' ";
                    ssql = string.Format(ssql, TCGTemp.Stid, TCGTemp.Time);
                    TCGRainDataHis TCG = dbDapper.QuerySingleOrDefault<TCGRainDataHis>(ssql);
                    if (TCG == null)
                    {
                        TCG = TCGTemp;

                        dbDapper.Insert(TCG);
                    }
                    else
                    {
                        TCG.Hour = TCGTemp.Hour;
                        TCG.Hour3 = TCGTemp.Hour3;
                        TCG.Hour6 = TCGTemp.Hour6;
                        TCG.Hour12 = TCGTemp.Hour12;
                        TCG.Hour24 = TCGTemp.Hour24;
                        TCG.DayRainfall = TCGTemp.DayRainfall;
                        TCG.RT = TCGTemp.RT;

                        dbDapper.Update(TCG);
                    }

                    //註記已經轉檔完成
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}'    ";
                    ssql = string.Format(ssql, sSTIDTimeLogType, item.no.ToString());

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "60";
                        dbDapper.Update(dslu);
                    }


                }
                catch (Exception ex)
                {
                    //註記轉檔失敗
                    ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' ";
                    ssql = string.Format(ssql, sSTIDTimeLogType, item.no.ToString());

                    DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
                    if (dslu != null)
                    {
                        dslu.status = "90";
                        dbDapper.Update(dslu);
                    }
                    continue;
                }
            }
        }

        /// <summary>
        /// 計算區間雨量總和
        /// </summary>
        /// <param name="AllList">該站號所有雨量資料</param>
        /// <param name="Main">主要雨量資料</param>
        /// <param name="iStart">計算的起始雨量區間</param>
        /// <param name="iEnd">計算的結束雨量區間</param>
        /// <returns></returns>
        private decimal getSumRangeRainTCGOld(List<TCGRainDataHis> AllList, TCGRainDataHis Main, int iStart, int iEnd)
        {
            decimal dSum = 0;

            //目前該筆資料主要時間
            DateTime MainTime = DateTime.ParseExact(Convert.ToString(Convert.ToInt32(Main.Time)), "yyyyMMddHH", null);

            //取得起始時間
            DateTime StartTime = MainTime.AddHours(-iStart);

            //總共要計算多少次
            int iCalCount = iEnd - iStart + 1;

            for (int i = 0; i < iCalCount; i++)
            {
                DateTime CalTime = StartTime.AddHours(-i);
                int iTime = Convert.ToInt32(CalTime.ToString("yyyyMMddHH"));
                TCGRainDataHis wrdCal = AllList.SingleOrDefault(s => s.Time == iTime.ToString());

                if (wrdCal != null)
                {
                    dSum += wrdCal.Hour;
                }
            }

            return dSum;
        }


        private async void button34_Click(object sender, EventArgs e)
        {
            //1.處理資料來源，已經由1-2匯入檔案資料到(TCGRainDataHis及WeaRainData)
            //(1-2)按鈕匯入北市府CSV檔
            //比對兩個資料表的資料內容是否有相同筆數，如果筆數不相同則找出問題互相補遺
            /* SQL:
                select * from (
                    select STID,count(*) ttData from WeaRainData
	                where STID in (
		                select distinct STID from TCGRainDataHis 
	                )
	                group by STID
                ) a 
                left join (
                    select STID,count(*) ttDataHis from TCGRainDataHis  group by STID
                ) b on a.STID = b.STID 
                where a.ttData <> b.ttDataHis
            */
            //20240510 在(1-2)按鈕已經加上TCGRainDataHis補遺到WeaRainData的功能
            //**但是WeaRainData補遺到TCGRainDataHis，功能還沒完成
            //2.清除原本已經轉檔紀錄
            //(2-1)每年第一次轉檔時，請執行以下SQL，清除已轉檔完成或是執行中未清除的資料
            //      SQL：delete DataStaticLog where type = 'TCGRainHisSTID' and status in ('10','60')
            //(2-2)不需要刪除任何的LOG紀錄，
            //     因為歷年已經跑過的資料，已經有LOG，如果篩除，會全部重跑
            //      SQL：delete DataStaticLog where type = 'TCGRainHisSTIDTime'            

            //3.直接執行多個程式一起跑

            //4.最後比對是否有全部轉檔成功到歷史區
            /* SQL:
                    select * from (
                        select STID,count(*) ttData from WeaRainData
	                    where STID in (
		                    select distinct STID from TCGRainDataHis 
	                    )
	                    group by STID
                    ) a 
                    left join (
                        select STID,count(*) ttDataHis from TCGRainDataHis  group by STID
                    ) b on a.STID = b.STID 
                    where a.ttData <> b.ttDataHis
             * */


            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            var progress = new Progress<string>(msg =>
            {
                LogToMsgText(msg); // 替換為您的 ShowStatus
                this.Text = msg;
            });

            try
            {
                await Task.Run(() => ProcessTCGDataTrans(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"北市府轉檔發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
                this.Text = "OK-ALL";
            }
        }

        // =========================================================
        // 核心歷史資料轉檔邏輯 (支援多開程式併發分工)
        // =========================================================
        private void ProcessTCGDataTrans(IProgress<string> progress)
        {
            string sSTIDLogType = "TCGRainHisSTID";
            string sSTIDTimeLogType = "TCGRainHisSTIDTime";
            string sStid = "";

            // 判斷 UI 是否有手動輸入特定站號
            if (!string.IsNullOrWhiteSpace(Trans109Text.Text))
            {
                sStid = Trans109Text.Text.Trim();
                ProcTCGRainDataHis(sStid, sSTIDTimeLogType, progress, 1, 1);
                return;
            }

            progress.Report("正在統計尚未完成北市府轉檔的雨量站....");

            // 取得所有尚未完成的雨量站清單 (利用 NOT EXISTS 提升效能)
            string ssqlMain = @"
            SELECT a.STID 
            FROM (
                SELECT STID, COUNT(*) AS ttData FROM TCGRainDataHis GROUP BY STID
            ) a
            LEFT JOIN (
                SELECT key2, COUNT(*) AS ttLog FROM DataStaticLog WHERE type = @TypeTime GROUP BY key2
            ) b ON a.STID = b.key2
            WHERE (b.ttLog IS NULL OR b.ttLog != a.ttData)
              AND NOT EXISTS (
                  SELECT 1 FROM DataStaticLog c 
                  WHERE c.type = @TypeStid AND c.key1 = a.STID AND c.status = '10'
              )
            ORDER BY a.STID";

            List<dynamic> TransList = dbDapper.Query(ssqlMain, new { TypeTime = sSTIDTimeLogType, TypeStid = sSTIDLogType }, commandTimeout: 300);
            string ssqlStatus10 = $"select * from  DataStaticLog where type = '{sSTIDLogType}' and status in ('10')";
            int iStatus10 = dbDapper.QueryTotalCount(ssqlStatus10);

            if (TransList.Count == 0 && iStatus10 == 0)
            {
                progress.Report("檢核資料完整性後，北市府轉檔-全部完成....");
                return;
            }
            else if (TransList.Count == 0 && iStatus10 != 0)
            {
                progress.Report($"檢核資料完整性後，北市府轉檔-尚未完成....執行中(10)狀態筆數：{iStatus10}");
                return;
            }

            List<string> stidList = TransList.Select(d => (string)d.STID).ToList();

            // 隨機洗牌，防止多個程式實體同時搶同一個站
            stidList = stidList.OrderBy(x => Guid.NewGuid()).ToList();

            int totalCount = stidList.Count;
            int currentIndex = 0;

            foreach (string stid in stidList)
            {
                currentIndex++;
                sStid = stid;

                try
                {
                    progress.Report($"[{sStid}] ({currentIndex}/{totalCount}) 爭取轉檔權限...");

                    // =========================================================
                    // 1. 極速搶佔機制 (原子操作 + 排他鎖)
                    // =========================================================
                    string claimSql = @"
                        BEGIN TRAN;
                        DECLARE @Claimed INT = 0;
                
                        IF NOT EXISTS (SELECT 1 FROM DataStaticLog WITH (UPDLOCK, HOLDLOCK) WHERE type = @Type AND key1 = @Stid)
                        BEGIN
                            INSERT INTO DataStaticLog (type, key1, key2, status, logtime) VALUES (@Type, @Stid, '', '10', GETDATE());
                            SET @Claimed = 1;
                        END
                        ELSE
                        BEGIN
                            UPDATE DataStaticLog
                            SET status = '10', logtime = GETDATE()
                            WHERE type = @Type AND key1 = @Stid AND status NOT IN ('10', '60');
                    
                            IF @@ROWCOUNT > 0 SET @Claimed = 1;
                        END
                
                        COMMIT TRAN;
                        SELECT @Claimed;
                    ";

                    int isClaimed = Convert.ToInt32(dbDapper.ExecuteScale(claimSql, new { Type = sSTIDLogType, Stid = sStid }));
                    if (isClaimed == 0) continue; // 搶佔失敗，跳過換下一站

                    // =========================================================
                    // 2. 執行主要計算與轉檔程式
                    // =========================================================
                    ProcTCGRainDataHis(sStid, sSTIDTimeLogType, progress, currentIndex, totalCount);

                    // 3. 註記該站點全部完工
                    string finishSql = "UPDATE DataStaticLog SET status = '60', logtime = GETDATE() WHERE type = @Type AND key1 = @Stid";
                    dbDapper.Execute(finishSql, new { Type = sSTIDLogType, Stid = sStid });
                }
                catch (Exception ex)
                {
                    progress.Report($"[{sStid}] 發生錯誤跳過: {ex.Message}");

                    // 標記站點狀態為 90 (失敗)
                    string errSql = "UPDATE DataStaticLog SET status = '90', logtime = GETDATE() WHERE type = @Type AND key1 = @Stid";
                    dbDapper.Execute(errSql, new { Type = sSTIDLogType, Stid = sStid });
                    continue;
                }
            }


            //最後在檢核一次是否還有沒跑完的資料
            TransList.Clear();
            TransList = dbDapper.Query(ssqlMain, new { TypeTime = sSTIDTimeLogType, TypeStid = sSTIDLogType }, commandTimeout: 300);
            iStatus10 = dbDapper.QueryTotalCount(ssqlStatus10);

            if (TransList.Count == 0 && iStatus10 == 0)
            {
                progress.Report("檢核資料完整性後，北市府轉檔-全部完成....");
            }
            else
            {
                progress.Report($"檢核資料完整性後，北市府轉檔-尚未完成....執行中(10)狀態筆數：{iStatus10}");
                return;
            }
        }

        private void ProcTCGRainDataHis(string sStid, string sSTIDTimeLogType, IProgress<string> progress, int currentIdx, int totalCount)
        {
            progress.Report($"[{sStid}] 讀取北市府雨量資料至記憶體...");

            // 取得該站所有歷史資料
            string ssql = "SELECT * FROM TCGRainDataHis WHERE STID = @stid ORDER BY [Time]";
            List<TCGRainDataHis> TCGRainList = dbDapper.Query<TCGRainDataHis>(ssql, new { stid = sStid });

            // 🔥 核心優化：建立 Dictionary 快取，查詢速度瞬間降至 O(1)
            Dictionary<string, decimal> rainDict = TCGRainList.ToDictionary(r => r.Time, r => r.Hour);

            // 取得尚未執行的資料
            string noExecSql = @" 
                SELECT a.* FROM TCGRainDataHis a
                LEFT JOIN DataStaticLog b ON CAST(a.no AS varchar) = b.key1 AND b.type = @LogType 
                WHERE a.STID = @stid AND b.no IS NULL 
                ORDER BY a.Time";
            List<TCGRainDataHis> NoExecList = dbDapper.Query<TCGRainDataHis>(noExecSql, new { stid = sStid, LogType = sSTIDTimeLogType });

            if (NoExecList.Count == 0) return;

            List<TCGRainDataHis> updateBatch = new List<TCGRainDataHis>();
            List<DataStaticLog> logBatch = new List<DataStaticLog>();
            int totalNoExec = NoExecList.Count;

            for (int i = 0; i < totalNoExec; i++)
            {
                TCGRainDataHis item = NoExecList[i];
                DateTime MainTime = DateTime.ParseExact(item.Time, "yyyyMMddHH", null);

                // 降低更新頻率，避免 UI 閃爍卡頓
                if (i % 100 == 0 || i == totalNoExec - 1)
                {
                    progress.Report($"[{sStid}] ({currentIdx}/{totalCount}) 計算進度: {i + 1}/{totalNoExec} ({MainTime:yyyy/MM/dd-HH})");
                }

                // == 極速計算各項指標 ==
                decimal dHour3 = GetSumRangeRain(rainDict, MainTime, 0, 2);
                decimal dHour6 = GetSumRangeRain(rainDict, MainTime, 0, 5);
                decimal dHour12 = GetSumRangeRain(rainDict, MainTime, 0, 11);
                decimal dHour24 = GetSumRangeRain(rainDict, MainTime, 0, 23);

                // 計算日雨量：將時間推到該日的 23 點往下算 24 小時
                DateTime dayStart = new DateTime(MainTime.Year, MainTime.Month, MainTime.Day, 23, 0, 0);
                decimal dFallDay = GetSumRangeRain(rainDict, dayStart, 0, 23);

                // == 計算 RT ==
                double alpha = 0.7;
                decimal dRT = GetSumRangeRain(rainDict, MainTime, 0, 23)
                            + GetSumRangeRain(rainDict, MainTime, 24, 47) * (decimal)Math.Pow(alpha, 1)
                            + GetSumRangeRain(rainDict, MainTime, 48, 71) * (decimal)Math.Pow(alpha, 2)
                            + GetSumRangeRain(rainDict, MainTime, 72, 95) * (decimal)Math.Pow(alpha, 3)
                            + GetSumRangeRain(rainDict, MainTime, 96, 119) * (decimal)Math.Pow(alpha, 4)
                            + GetSumRangeRain(rainDict, MainTime, 120, 143) * (decimal)Math.Pow(alpha, 5)
                            + GetSumRangeRain(rainDict, MainTime, 144, 167) * (decimal)Math.Pow(alpha, 6)
                            + GetSumRangeRain(rainDict, MainTime, 168, 191) * (decimal)Math.Pow(alpha, 7);

                // 賦值給物件
                item.Hour3 = dHour3;
                item.Hour6 = dHour6;
                item.Hour12 = dHour12;
                item.Hour24 = dHour24;
                item.DayRainfall = dFallDay;
                item.RT = dRT;

                updateBatch.Add(item);

                // 準備 Log 資料
                logBatch.Add(new DataStaticLog
                {
                    type = sSTIDTimeLogType,
                    key1 = item.no.ToString(),
                    key2 = item.Stid,
                    status = "60",
                    logtime = DateTime.Now
                });
            }

            // =========================================================
            // 🔥 SqlBulkCopy + 暫存表 + Transaction 批次寫入
            // =========================================================
            progress.Report($"[{sStid}] 運算完成，準備啟動 SqlBulkCopy 巨量寫入 (更新 {updateBatch.Count} 筆, 寫入 Log {logBatch.Count} 筆)...");

            DataTable dtUpdate = new DataTable();
            dtUpdate.Columns.Add("no", typeof(int));
            dtUpdate.Columns.Add("Hour", typeof(decimal));
            dtUpdate.Columns.Add("Hour3", typeof(decimal));
            dtUpdate.Columns.Add("Hour6", typeof(decimal));
            dtUpdate.Columns.Add("Hour12", typeof(decimal));
            dtUpdate.Columns.Add("Hour24", typeof(decimal));
            dtUpdate.Columns.Add("DayRainfall", typeof(decimal));
            dtUpdate.Columns.Add("RT", typeof(decimal));

            foreach (var x in updateBatch)
            {
                dtUpdate.Rows.Add(x.no, x.Hour, x.Hour3, x.Hour6, x.Hour12, x.Hour24, x.DayRainfall, x.RT);
            }

            DataTable dtLog = new DataTable();
            dtLog.Columns.Add("type", typeof(string));
            dtLog.Columns.Add("key1", typeof(string));
            dtLog.Columns.Add("key2", typeof(string));
            dtLog.Columns.Add("status", typeof(string));
            dtLog.Columns.Add("logtime", typeof(DateTime));

            foreach (var log in logBatch)
            {
                dtLog.Rows.Add(log.type, log.key1, log.key2, log.status, log.logtime);
            }

            // 💡 請記得將連線字串替換為您系統中實際的字串
            using (SqlConnection conn = new SqlConnection(dbDapper.ConnStr))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string createTempSql = @"
                            CREATE TABLE #TempTCGUpdate (
                                no int PRIMARY KEY, 
                                [Hour] decimal(18,4), Hour3 decimal(18,4), Hour6 decimal(18,4), 
                                Hour12 decimal(18,4), Hour24 decimal(18,4), 
                                DayRainfall decimal(18,4), RT decimal(18,4)
                            );";
                        using (SqlCommand cmd = new SqlCommand(createTempSql, conn, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "#TempTCGUpdate";
                            bulkCopy.BatchSize = 10000;
                            foreach (DataColumn col in dtUpdate.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            bulkCopy.WriteToServer(dtUpdate);
                        }

                        string bulkUpdateSql = @"
                            UPDATE t
                            SET t.[Hour] = tmp.[Hour], 
                                t.Hour3 = tmp.Hour3, 
                                t.Hour6 = tmp.Hour6, 
                                t.Hour12 = tmp.Hour12, 
                                t.Hour24 = tmp.Hour24, 
                                t.DayRainfall = tmp.DayRainfall, 
                                t.RT = tmp.RT
                            FROM TCGRainDataHis t
                            INNER JOIN #TempTCGUpdate tmp ON t.no = tmp.no;
                    
                            DROP TABLE #TempTCGUpdate;
                        ";
                        using (SqlCommand cmd = new SqlCommand(bulkUpdateSql, conn, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "DataStaticLog";
                            bulkCopy.BatchSize = 10000;
                            foreach (DataColumn col in dtLog.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            bulkCopy.WriteToServer(dtLog);
                        }

                        transaction.Commit();
                        progress.Report($"[{sStid}] 批次更新與 Log 寫入大成功！");
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }



        ///// <summary>
        ///// 20230501
        ///// 水利署(Wra)歷史資料轉檔()
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    //1.處理資料來源，已經由1-2匯入檔案資料到(WraRainDataHis及WeaRainData)
        //    //(1-2)按鈕匯入水利署CSV檔
        //    //比對兩個資料表的資料內容是否有相同筆數，如果筆數不相同則找出問題互相補遺
        //    /* SQL:
        //        select * from (
        //            select STID,count(*) ttData from WeaRainData
        //         where STID in (
        //          select distinct STID from WraRainDataHis 
        //         )
        //         group by STID
        //        ) a 
        //        left join (
        //            select STID,count(*) ttDataHis from WraRainDataHis  group by STID
        //        ) b on a.STID = b.STID 
        //        where a.ttData <> b.ttDataHis
        //    */
        //    //20240510 在(1-2)按鈕已經加上WraRainDataHis補遺到WeaRainData的功能
        //    //**但是WeaRainData補遺到WraRainDataHis，功能還沒完成
        //    //2.清除原本已經轉檔紀錄
        //    //(2-1)每年第一次轉檔時，請執行以下SQL，清除已轉檔完成或是執行中未清除的資料
        //    //      SQL：delete DataStaticLog where type = 'WraRainHisSTID' and status in ('10','60')
        //    //(2-2)不需要刪除任何的LOG紀錄，
        //    //     因為歷年已經跑過的資料，已經有LOG，如果篩除，會全部重跑
        //    //      SQL：delete DataStaticLog where type = 'WraRainHisSTIDTime'            

        //    //3.直接執行多個程式一起跑

        //    //4.最後比對是否有全部轉檔成功到歷史區
        //    /* SQL:
        //            select * from (
        //                select STID,count(*) ttData from WeaRainData
        //             where STID in (
        //              select distinct STID from WraRainDataHis 
        //             )
        //             group by STID
        //            ) a 
        //            left join (
        //                select STID,count(*) ttDataHis from WraRainDataHis  group by STID
        //            ) b on a.STID = b.STID 
        //            where a.ttData <> b.ttDataHis
        //     * */

        //    string sStid = "";

        //    //轉檔註記
        //    //每個雨量站
        //    string sSTIDLogType = "WraRainHisSTID";
        //    //每個雨量站的時刻
        //    string sSTIDTimeLogType = "WraRainHisSTIDTime";


        //    //有輸入資料，則轉該雨量站的資料
        //    if (Trans109Text.Text.Trim() != "")
        //    {
        //        sStid = Trans109Text.Text;
        //        this.Text = sStid;
        //        ProcWraRainDataHis(sStid, sSTIDTimeLogType);
        //    }
        //    else //如果沒輸入，則由資料庫判斷尚未執行或還沒執行完全的雨量站進行轉檔
        //    {
        //        while (true)
        //        {
        //            try
        //            {
        //                //比較該雨量站資料數量與轉檔LOG紀錄數量，如果沒轉過或是資料數不符，則進行轉檔
        //                //20210506 新增排除已經在轉檔的雨量站
        //                ssql = @" 
        //                        select * from (
        //                            select * from (
        //                                select STID,count(*) ttData from WraRainDataHis group by STID
        //                            ) a 
        //                            left join (
        //                                select key2,count(*) ttLog from DataStaticLog where type = '{1}'  group by key2
        //                            ) b on a.STID = b.key2 
        //                        ) r 
        //                        left join DataStaticLog c  on c.type = '{0}' and c.key1 = r.STID and c.status = '10'
        //                        where (ttLog != ttData or ttLog is null) and c.key1 is null    
        //                        order by r.STID
        //                    ";
        //                ssql = string.Format(ssql, sSTIDLogType, sSTIDTimeLogType);

        //                ShowStatus(string.Format("正在統計尚未轉檔到歷史資料的雨量站...."));

        //                List<dynamic> TransList = dbDapper.Query(ssql);

        //                //如果沒資料則停止
        //                if (TransList.Count() == 0) break;

        //                //總筆數，使用Random取得資料避免重複執行問題
        //                int iRandom = new Random().Next(0, TransList.Count());

        //                //預防重複執行後，LOG紀錄筆數>WeaRainData的筆數導致前面判斷未轉完畢的篩選，所以如果超過筆數，則刪除LOG重新跑
        //                int ittLog = TransList[iRandom].ttLog == null ? 0 : TransList[iRandom].ttLog;
        //                int ittData = TransList[iRandom].ttData == null ? 0 : TransList[iRandom].ttData;
        //                if (ittLog > ittData)
        //                {
        //                    //20240507 查詢問題後，發現LOG多餘的資料對應到WraRainDataHis，是NULL，所以刪除LOG紀錄NULL的資料，以免全部重跑耗費太多時間
        //                    ssql = @" select * from DataStaticLog a 
        //                                left join WraRainDataHis b on  b.no = a.key1
        //                                where a.type = '{0}' and b.STID is null and a.key2 = '{1}' ";
        //                    ssql = string.Format(ssql, sSTIDTimeLogType, TransList[iRandom].STID);
        //                    List<dynamic> ErrorList = dbDapper.Query(ssql);

        //                    foreach (dynamic item in ErrorList)
        //                    {
        //                        ssql = @" delete DataStaticLog where type = '{0}' and key2 = '{1}' and no = '{2}' ";
        //                        ssql = string.Format(ssql, sSTIDTimeLogType, item.key2, item.no);
        //                        dbDapper.Execute(ssql);
        //                    }

        //                    continue;

        //                    //ssql = @" delete from DataStaticLog where type = '{1}' and key2 = '{0}' ";
        //                    //ssql = string.Format(ssql, TransList[iRandom].STID, sSTIDTimeLogType);
        //                    //dbDapper.Execute(ssql);
        //                }

        //                sStid = TransList[iRandom].STID;
        //                this.Text = string.Format("{0}-剩餘數量：{1}", sStid, TransList.Count());
        //                Application.DoEvents();

        //                //==判斷是否執行過
        //                //10:執行中 60:已完成 90:轉檔失敗
        //                //string sDataStaticLogType = "WeaRainTransHisSTID";
        //                ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' ";
        //                ssql = string.Format(ssql, sSTIDLogType, sStid);
        //                DataStaticLog dsl_new = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);

        //                //沒有轉檔紀錄
        //                if (dsl_new == null)
        //                {
        //                    //如果沒有轉檔紀錄，則新增一筆執行中
        //                    //寫入統計LOG
        //                    dsl_new = new DataStaticLog();
        //                    dsl_new.type = sSTIDLogType;
        //                    dsl_new.key1 = sStid;
        //                    dsl_new.key2 = "";
        //                    dsl_new.status = "10";
        //                    dsl_new.logtime = DateTime.Now;
        //                    dbDapper.Insert(dsl_new);
        //                }
        //                else//已經有轉檔紀錄
        //                {
        //                    //如果執行中，則不進行該站轉檔
        //                    if (dsl_new.status == "10")
        //                    {
        //                        continue;
        //                    }

        //                    //重新更新為執行中
        //                    dsl_new.status = "10";
        //                    dbDapper.Update(dsl_new);
        //                }

        //                //主要轉檔程序
        //                ProcWraRainDataHis(sStid, sSTIDTimeLogType);

        //                //註記已經轉檔完成
        //                ssql = @" select * from DataStaticLog where type = '{0}' and key1 = '{1}' ";
        //                ssql = string.Format(ssql, sSTIDLogType, sStid);

        //                DataStaticLog dslu = dbDapper.QuerySingleOrDefault<DataStaticLog>(ssql);
        //                if (dslu != null)
        //                {
        //                    dslu.status = "60";
        //                    dbDapper.Update(dslu);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                //發生錯誤繼續後面的轉檔，直到全部雨量站執行完畢
        //                continue;
        //            }
        //        }
        //    }

        //    this.Text = "OK-ALL";
        //}

        /// <summary>
        /// (1-2)匯入雨量資料(水利署CVS)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button27Old_Click(object sender, EventArgs e)
        {
            //1. 水利署CVS都放到"程式根目錄\WraRainDataHis"目錄下，開啟轉檔

            string sConstDirectoryPath = Directory.GetCurrentDirectory() + @"\WraRainDataHis";
            Directory.CreateDirectory(sConstDirectoryPath);

            //水利署CVS都放到"程式根目錄\WraRainDataHis"目錄下，開啟轉檔
            FileInfo[] fiList = new DirectoryInfo(sConstDirectoryPath).GetFiles("*.csv", SearchOption.TopDirectoryOnly);

            //try
            //{
            foreach (FileInfo item in fiList)
            {

                string readText = File.ReadAllText(item.FullName, Encoding.Default);
                string sss = readText.Substring(0, 8);
                List<string> StringList = new List<string>();

                using (FileStream fs = File.Open(item.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BufferedStream bs = new BufferedStream(fs))
                    {
                        using (StreamReader sr = new StreamReader(bs))
                        {
                            string line;
                            int i = 0;
                            while ((line = sr.ReadLine()) != null)
                            {
                                StringList.Add(line);
                            }
                        }
                    }
                }

                int iTotal = StringList.Count;

                StringList.Remove(StringList[0]);
                while (StringList.Count > 0)
                {
                    //顯示目前狀態
                    ShowStatus(string.Format("({0}/{1}),同步正在執行筆數：{2}", StringList.Count, iTotal, bws.Count));

                    //==單獨測試一筆
                    //string sTempTest = StringList[0];
                    //BackgroundWorker bwTest = new BackgroundWorker();
                    //bwTest.DoWork += new DoWorkEventHandler(bw_DoWork_WraRainDataHis);
                    //bwTest.WorkerSupportsCancellation = true;
                    //bwTest.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                    //bws.Add(bwTest);
                    //bwTest.RunWorkerAsync(sTempTest);
                    //break;
                    //==單獨測試一筆


                    if (bws.Count < t)
                    {
                        string sTemp = StringList[0];
                        BackgroundWorker bw = new BackgroundWorker();
                        bw.DoWork += new DoWorkEventHandler(bw_DoWork_WraRainDataHis);
                        bw.WorkerSupportsCancellation = true;
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                        bws.Add(bw);

                        bw.RunWorkerAsync(sTemp);

                        StringList.Remove(StringList[0]);
                    }
                }


                //資料補遺WeaRainData,WraRainDataHis，資料可能再轉檔的時候筆數不正確，兩邊都可以有缺漏，比較後互相補上
                ssql = @"
                    select * from (
                        select STID,count(*) ttData from WeaRainData
	                    where STID in (
		                    select distinct STID from WraRainDataHis 
	                    )
	                    group by STID
                    ) a 
                    left join (
                        select STID,count(*) ttDataHis from WraRainDataHis  group by STID
                    ) b on a.STID = b.STID 
                    where a.ttData <> b.ttDataHis
                ";

                List<dynamic> checkList = dbDapper.Query<dynamic>(ssql);

                foreach (var StidItem in checkList)
                {
                    //歷史資料比較多，寫回WeaRainData
                    if (StidItem.ttDataHis > StidItem.ttData)
                    {
                        Application.DoEvents();
                        ssql = @"
                            select * from WraRainDataHis a 
                            left join WeaRainData b on a.STID = b.Stid and a.time = b.Time
                            where a.STID = '{0}' and b.no is null
                        ";
                        ssql = string.Format(ssql, StidItem.STID);
                        List<WraRainDataHis> TempList = dbDapper.Query<WraRainDataHis>(ssql);
                        foreach (WraRainDataHis WraRainDataHisitem in TempList)
                        {

                            //同步寫入 WeaRainData
                            WeaRainData WeaRainDataTemp = new WeaRainData();
                            WeaRainDataTemp.STID = WraRainDataHisitem.Stid;
                            WeaRainDataTemp.time = WraRainDataHisitem.Time;
                            WeaRainDataTemp.PP01 = WraRainDataHisitem.Hour;
                            WeaRainDataTemp.PP01old = WraRainDataHisitem.Hour;

                            ssql = @" select * from WeaRainData where stid = '{0}' and time = '{1}' ";
                            ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
                            WeaRainData WeaRainData_new = dbDapper.QuerySingleOrDefault<WeaRainData>(ssql);

                            //沒有轉檔紀錄
                            if (WeaRainData_new == null)
                            {
                                //如果沒有轉檔紀錄，則新增一筆
                                dbDapper.Insert(WeaRainDataTemp);
                            }
                            else//已經有轉檔紀錄
                            {
                                //更新
                                WeaRainData_new.PP01 = WeaRainDataTemp.PP01;
                                WeaRainData_new.PP01old = WeaRainDataTemp.PP01old;
                                dbDapper.Update(WeaRainData_new);
                            }
                        }
                        Application.DoEvents();
                    }

                    //WeaRainData比較多，寫回歷史資料
                    if (StidItem.ttData > StidItem.ttDataHis)
                    {
                        Application.DoEvents();
                        ssql = @"
                            select * from WeaRainData a 
                            left join WraRainDataHis b on a.STID = b.Stid and a.time = b.Time
                            where a.STID = '{0}' and b.no is null
                        ";
                        ssql = string.Format(ssql, StidItem.STID);
                        List<WeaRainData> TempList = dbDapper.Query<WeaRainData>(ssql);
                        foreach (WeaRainData WeaRainDataitem in TempList)
                        {
                            //同步寫入 WraRainDataHis
                            ssql = @" select * from WraRainDataHis where stid = '{0}' and time = '{1}' ";
                            ssql = string.Format(ssql, WeaRainDataitem.STID, WeaRainDataitem.time);
                            WraRainDataHis WraRainDataHis_new = dbDapper.QuerySingleOrDefault<WraRainDataHis>(ssql);

                            //沒有轉檔紀錄
                            if (WraRainDataHis_new == null)
                            {
                                //如果沒有轉檔紀錄，則新增一筆
                                WraRainDataHis_new = new WraRainDataHis();
                                WraRainDataHis_new.Stid = WeaRainDataitem.STID;
                                WraRainDataHis_new.Time = WeaRainDataitem.time;
                                WraRainDataHis_new.Hour = WeaRainDataitem.PP01;
                                dbDapper.Insert(WraRainDataHis_new);
                            }
                            else//已經有轉檔紀錄
                            {
                                //更新
                                WraRainDataHis_new.Hour = WeaRainDataitem.PP01;
                                dbDapper.Update(WraRainDataHis_new);
                            }
                        }
                        Application.DoEvents();
                    }



                }

                Directory.CreateDirectory(Path.Combine(item.DirectoryName, "OK"));
                string sNewFullName = Path.Combine(item.DirectoryName, "OK", item.Name);

                item.MoveTo(sNewFullName);
                Application.DoEvents();
            }










        }

        /// <summary>
        /// (1-2) 匯入雨量資料(水利署CVS) - 極速 BulkCopy + 純 SQL 補遺版
        /// </summary>
        private async void button27_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            // 這裡沿用我們之前新增的 RichTextBox (MsgText) 輸出，若無則改回 ShowStatus
            var progress = new Progress<string>(msg => { LogToMsgText(msg); });

            try
            {
                await Task.Run(async () => await ProcessWraRainDataBulk(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"水利署轉檔發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
            }
        }

        private async Task ProcessWraRainDataBulk(IProgress<string> progress)
        {
            string sConstDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "WraRainDataHis");
            Directory.CreateDirectory(sConstDirectoryPath);

            FileInfo[] fiList = new DirectoryInfo(sConstDirectoryPath).GetFiles("*.csv", SearchOption.TopDirectoryOnly);

            // 💡 替換成您的資料庫連線字串 (如果 dbDapper 有提供屬性，也可從那邊抓)
            string connString = dbDapper.ConnStr;

            foreach (FileInfo item in fiList)
            {
                progress.Report($"讀取與解析水利署檔案: {item.Name}");

                // =========================================================
                // 階段 1：記憶體內整理資料 (捨棄 BackgroundWorker，改用延遲讀取)
                // =========================================================
                DataTable dtTemp = new DataTable();
                dtTemp.Columns.Add("Stid", typeof(string));
                dtTemp.Columns.Add("Time", typeof(string));
                dtTemp.Columns.Add("Min10", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour3", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour6", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour12", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour24", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("DayRainfall", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("RT", typeof(decimal)).DefaultValue = 0m;

                // 直接跳過第一行(標題)，逐行解析
                var lines = File.ReadLines(item.FullName, Encoding.Default).Skip(1);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // 處理逗號分隔
                    string[] cols = line.Split(new[] { ',' }, StringSplitOptions.None);
                    if (cols.Length < 7 || cols[0].StartsWith("#")) continue;

                    DateTime dtTime = DateTime.ParseExact(cols[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    string sTime = dtTime.ToString("yyyyMMddHH");

                    // 處理數值並防呆 (<0 轉為 0)
                    decimal dMin10 = decimal.TryParse(cols[2], out decimal m10) ? (m10 < 0 ? 0 : m10) : 0;
                    decimal dHour = decimal.TryParse(cols[3], out decimal h1) ? (h1 < 0 ? 0 : h1) : 0;
                    decimal dHour3 = 0;
                    decimal dHour6 = decimal.TryParse(cols[4], out decimal h6) ? (h6 < 0 ? 0 : h6) : 0;
                    decimal dHour12 = 0;
                    decimal dHour24 = decimal.TryParse(cols[5], out decimal h24) ? (h24 < 0 ? 0 : h24) : 0;
                    decimal dDayRainfall = decimal.TryParse(cols[6], out decimal dr) ? (dr < 0 ? 0 : dr) : 0;
                    decimal dRT = 0;

                    dtTemp.Rows.Add(cols[0], sTime, dMin10, dHour, dHour3, dHour6, dHour12, dHour24, dDayRainfall, dRT);
                }

                if (dtTemp.Rows.Count == 0) continue;

                progress.Report($"開始進行 SqlBulkCopy 大批次寫入 ({dtTemp.Rows.Count} 筆)...");

                // =========================================================
                // 階段 2：大批次寫入與 MERGE (WraRainDataHis, WeaRainData, WeaRainDataHis)
                // =========================================================
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            string createTempSql = @"
                        CREATE TABLE #TempWraRain (
                            Stid VARCHAR(50), Time VARCHAR(20), Min10 DECIMAL(18,2), 
                            Hour DECIMAL(18,2), Hour3 DECIMAL(18,2), Hour6 DECIMAL(18,2), Hour12 DECIMAL(18,2), 
                            Hour24 DECIMAL(18,2), DayRainfall DECIMAL(18,2), RT DECIMAL(18,2)
                        )";
                            using (SqlCommand cmd = new SqlCommand(createTempSql, conn, tx)) await cmd.ExecuteNonQueryAsync();

                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tx))
                            {
                                bulkCopy.DestinationTableName = "#TempWraRain";
                                bulkCopy.BatchSize = 10000;
                                bulkCopy.BulkCopyTimeout = 300;

                                foreach (DataColumn col in dtTemp.Columns)
                                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);

                                await RunWithLiveProgressAsync(() => bulkCopy.WriteToServerAsync(dtTemp), "SqlBulkCopy 寫入暫存表", progress);
                            }

                            // 1. 寫入或更新 水利署歷史資料 (WraRainDataHis)
                            string mergeWraSql = @"
                                MERGE INTO WraRainDataHis AS T
                                USING #TempWraRain AS S ON T.Stid = S.Stid AND T.Time = S.Time
                                WHEN MATCHED THEN
                                    UPDATE SET Min10 = S.Min10, Hour = S.Hour, Hour3 = S.Hour3, Hour6 = S.Hour6, Hour12 = S.Hour12, 
                                    Hour24 = S.Hour24, DayRainfall = S.DayRainfall , RT = S.RT
                                WHEN NOT MATCHED THEN
                                    INSERT (Stid, Time, Min10, Hour, Hour3, Hour6, Hour12, Hour24, DayRainfall, RT) 
                                    VALUES (S.Stid, S.Time, S.Min10, S.Hour, S.Hour3, S.Hour6, S.Hour12, S.Hour24, S.DayRainfall, S.RT);";

                            using (SqlCommand cmd = new SqlCommand(mergeWraSql, conn, tx))
                            {
                                cmd.CommandTimeout = 1200;
                                // 👇 替換這裡
                                await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "MERGE 寫入 WraRainDataHis", progress);
                            }

                            // 2. 同步寫入 氣象總表 (WeaRainData)
                            string mergeWeaSql = @"
                                MERGE INTO WeaRainData AS T
                                USING #TempWraRain AS S ON T.STID = S.Stid AND T.time = S.Time
                                WHEN MATCHED THEN
                                    UPDATE SET PP01 = S.Hour, PP01old = S.Hour
                                WHEN NOT MATCHED THEN
                                    INSERT (STID, time, PP01, PP01old) VALUES (S.Stid, S.Time, S.Hour, S.Hour);";

                            using (SqlCommand cmd = new SqlCommand(mergeWeaSql, conn, tx))
                            {
                                cmd.CommandTimeout = 1200;
                                // 👇 替換這裡
                                await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "MERGE 同步 WeaRainData", progress);
                            }

                            // 3. 同步寫入 氣象總表歷史 (WeaRainDataHis) 避免重複
                            string mergeWeaHisSql = @"
                                MERGE INTO WeaRainDataHis AS T
                                USING #TempWraRain AS S ON T.Stid = S.Stid AND T.Time = S.Time
                                WHEN MATCHED THEN
                                    UPDATE SET Hour = S.Hour
                                WHEN NOT MATCHED THEN
                                    INSERT (Stid, Time, Hour, TransYN) VALUES (S.Stid, S.Time, S.Hour, 'N');";

                            using (SqlCommand cmd = new SqlCommand(mergeWeaHisSql, conn, tx))
                            {
                                cmd.CommandTimeout = 1200;
                                // 👇 替換這裡
                                await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "MERGE 同步 WeaRainDataHis", progress);
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                // 搬移至 OK 資料夾
                string okDirPath = Path.Combine(item.DirectoryName, "OK");
                Directory.CreateDirectory(okDirPath);
                string newFullName = Path.Combine(okDirPath, item.Name);
                if (File.Exists(newFullName)) File.Delete(newFullName);
                item.MoveTo(newFullName);
            }

            // =========================================================
            // 階段 3：100% 純 SQL 資料庫雙向補遺 
            // (解決原程式碼沒有等待 BackgroundWorker 就直接補遺的 Bug)
            // =========================================================
            progress.Report("所有檔案轉檔完成，開始進行資料庫雙向補遺 (純 SQL 執行中)...");

            using (SqlConnection conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();

                // 補遺 1：將 WraRainDataHis 中有，但 WeaRainData 沒有的資料，一口氣塞進 WeaRainData
                string syncToWeaSql = @"
            INSERT INTO WeaRainData (STID, time, PP01, PP01old)
            SELECT a.Stid, a.Time, a.Hour, a.Hour
            FROM WraRainDataHis a
            LEFT JOIN WeaRainData b ON a.Stid = b.STID AND a.Time = b.time
            WHERE b.STID IS NULL;";

                using (SqlCommand cmd = new SqlCommand(syncToWeaSql, conn))
                {
                    cmd.CommandTimeout = 1200;
                    await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "補遺：寫回 WeaRainData", progress);
                }

                // 補遺 2：將 WeaRainData 中有，但 WraRainDataHis 沒有的資料，一口氣塞回 WraRainDataHis
                string syncToWraSql = @"
                    INSERT INTO WraRainDataHis (Stid, Time, Hour, Min10, Hour3, Hour6, Hour12, Hour24, DayRainfall, RT)
                    SELECT a.STID, a.time, a.PP01, 0, 0, 0, 0, 0, 0, 0
                    FROM WeaRainData a
                    LEFT JOIN WraRainDataHis b ON a.STID = b.Stid AND a.time = b.Time
                    WHERE b.Stid IS NULL 
                    AND a.STID IN (SELECT DISTINCT Stid FROM WraRainDataHis);";

                using (SqlCommand cmd = new SqlCommand(syncToWraSql, conn))
                {
                    cmd.CommandTimeout = 1200;
                    await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "補遺：寫回 WraRainDataHis", progress);
                }
            }

            progress.Report("水利署雨量資料轉檔與雙向補遺全數完成！");
        }



        /// <summary>
        /// (1-3)匯入雨量資料([TCG]北市府CVS)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button33old_Click(object sender, EventArgs e)
        {
            //1. 北市府CVS都放到"程式根目錄\TCGRainDataHis"目錄下，開啟轉檔

            string sConstDirectoryPath = Directory.GetCurrentDirectory() + @"\TCGRainDataHis";
            Directory.CreateDirectory(sConstDirectoryPath);

            //北市府CVS都放到"程式根目錄\TCGRainDataHis"目錄下，開啟轉檔
            FileInfo[] fiList = new DirectoryInfo(sConstDirectoryPath).GetFiles("*.csv", SearchOption.TopDirectoryOnly);

            //try
            //{
            foreach (FileInfo item in fiList)
            {

                string readText = File.ReadAllText(item.FullName, Encoding.Default);
                string sss = readText.Substring(0, 8);
                List<string> StringList = new List<string>();

                using (FileStream fs = File.Open(item.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (BufferedStream bs = new BufferedStream(fs))
                    {
                        using (StreamReader sr = new StreamReader(bs))
                        {
                            string line;
                            int i = 0;
                            while ((line = sr.ReadLine()) != null)
                            {
                                StringList.Add(line);
                            }
                        }
                    }
                }

                int iTotal = StringList.Count;

                StringList.Remove(StringList[0]);
                while (StringList.Count > 0)
                {
                    //顯示目前狀態
                    ShowStatus(string.Format("({0}/{1}),同步正在執行筆數：{2}", StringList.Count, iTotal, bws.Count));

                    //==單獨測試一筆
                    //string sTempTest = StringList[0];
                    //BackgroundWorker bwTest = new BackgroundWorker();
                    //bwTest.DoWork += new DoWorkEventHandler(bw_DoWork_WraRainDataHis);
                    //bwTest.WorkerSupportsCancellation = true;
                    //bwTest.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                    //bws.Add(bwTest);
                    //bwTest.RunWorkerAsync(sTempTest);
                    //break;
                    //==單獨測試一筆


                    if (bws.Count < t)
                    {
                        string sTemp = StringList[0];
                        BackgroundWorker bw = new BackgroundWorker();
                        bw.DoWork += new DoWorkEventHandler(bw_DoWork_TCGRainDataHis);
                        bw.WorkerSupportsCancellation = true;
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                        bws.Add(bw);

                        bw.RunWorkerAsync(sTemp);

                        StringList.Remove(StringList[0]);
                    }
                }


                //資料補遺WeaRainData,TCGRainDataHis，資料可能再轉檔的時候筆數不正確，兩邊都可以有缺漏，比較後互相補上
                ssql = @"
                    select * from (
                        select STID,count(*) ttData from WeaRainData
	                    where STID in (
		                    select distinct STID from TCGRainDataHis 
	                    )
	                    group by STID
                    ) a 
                    left join (
                        select STID,count(*) ttDataHis from TCGRainDataHis  group by STID
                    ) b on a.STID = b.STID 
                    where a.ttData <> b.ttDataHis
                ";

                List<dynamic> checkList = dbDapper.Query<dynamic>(ssql);

                foreach (var StidItem in checkList)
                {
                    //歷史資料比較多，寫回WeaRainData
                    if (StidItem.ttDataHis > StidItem.ttData)
                    {
                        Application.DoEvents();
                        ssql = @"
                            select * from TCGRainDataHis a 
                            left join WeaRainData b on a.STID = b.Stid and a.time = b.Time
                            where a.STID = '{0}' and b.no is null
                        ";
                        ssql = string.Format(ssql, StidItem.STID);
                        List<TCGRainDataHis> TempList = dbDapper.Query<TCGRainDataHis>(ssql);
                        foreach (TCGRainDataHis WraRainDataHisitem in TempList)
                        {

                            //同步寫入 WeaRainData
                            WeaRainData WeaRainDataTemp = new WeaRainData();
                            WeaRainDataTemp.STID = WraRainDataHisitem.Stid;
                            WeaRainDataTemp.time = WraRainDataHisitem.Time;
                            WeaRainDataTemp.PP01 = WraRainDataHisitem.Hour;
                            WeaRainDataTemp.PP01old = WraRainDataHisitem.Hour;

                            ssql = @" select * from WeaRainData where stid = '{0}' and time = '{1}' ";
                            ssql = string.Format(ssql, WeaRainDataTemp.STID, WeaRainDataTemp.time);
                            WeaRainData WeaRainData_new = dbDapper.QuerySingleOrDefault<WeaRainData>(ssql);

                            //沒有轉檔紀錄
                            if (WeaRainData_new == null)
                            {
                                //如果沒有轉檔紀錄，則新增一筆
                                dbDapper.Insert(WeaRainDataTemp);
                            }
                            else//已經有轉檔紀錄
                            {
                                //更新
                                WeaRainData_new.PP01 = WeaRainDataTemp.PP01;
                                WeaRainData_new.PP01old = WeaRainDataTemp.PP01old;
                                dbDapper.Update(WeaRainData_new);
                            }
                        }
                        Application.DoEvents();
                    }

                    //WeaRainData比較多，寫回歷史資料
                    if (StidItem.ttData > StidItem.ttDataHis)
                    {
                        Application.DoEvents();
                        ssql = @"
                            select * from WeaRainData a 
                            left join TCGRainDataHis b on a.STID = b.Stid and a.time = b.Time
                            where a.STID = '{0}' and b.no is null
                        ";
                        ssql = string.Format(ssql, StidItem.STID);
                        List<WeaRainData> TempList = dbDapper.Query<WeaRainData>(ssql);
                        foreach (WeaRainData WeaRainDataitem in TempList)
                        {
                            //同步寫入 TCGRainDataHis
                            ssql = @" select * from TCGRainDataHis where stid = '{0}' and time = '{1}' ";
                            ssql = string.Format(ssql, WeaRainDataitem.STID, WeaRainDataitem.time);
                            TCGRainDataHis TCGRainDataHis_new = dbDapper.QuerySingleOrDefault<TCGRainDataHis>(ssql);

                            //沒有轉檔紀錄
                            if (TCGRainDataHis_new == null)
                            {
                                //如果沒有轉檔紀錄，則新增一筆
                                TCGRainDataHis_new = new TCGRainDataHis();
                                TCGRainDataHis_new.Stid = WeaRainDataitem.STID;
                                TCGRainDataHis_new.Time = WeaRainDataitem.time;
                                TCGRainDataHis_new.Hour = WeaRainDataitem.PP01;
                                dbDapper.Insert(TCGRainDataHis_new);
                            }
                            else//已經有轉檔紀錄
                            {
                                //更新
                                TCGRainDataHis_new.Hour = WeaRainDataitem.PP01;
                                dbDapper.Update(TCGRainDataHis_new);
                            }
                        }
                        Application.DoEvents();
                    }



                }



                Directory.CreateDirectory(Path.Combine(item.DirectoryName, "OK"));
                string sNewFullName = Path.Combine(item.DirectoryName, "OK", item.Name);

                item.MoveTo(sNewFullName);
                Application.DoEvents();
            }
        }

        private async void button33_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            // 將進度訊息導向 MsgText (RichTextBox)
            var progress = new Progress<string>(msg => { LogToMsgText(msg); });

            try
            {
                await Task.Run(async () => await ProcessTCGRainDataBulk(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"北市府雨量轉檔發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
            }
        }

        private async Task ProcessTCGRainDataBulk(IProgress<string> progress)
        {
            string sConstDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "TCGRainDataHis");
            Directory.CreateDirectory(sConstDirectoryPath);

            FileInfo[] fiList = new DirectoryInfo(sConstDirectoryPath).GetFiles("*.csv", SearchOption.TopDirectoryOnly);

            // 💡 替換成您的資料庫連線字串
            string connString = dbDapper.ConnStr;

            foreach (FileInfo item in fiList)
            {
                progress.Report($"讀取與解析北市府檔案: {item.Name}");

                // =========================================================
                // 階段 1：記憶體內整理資料
                // =========================================================
                DataTable dtTemp = new DataTable();
                dtTemp.Columns.Add("Stid", typeof(string));
                dtTemp.Columns.Add("Time", typeof(string));
                dtTemp.Columns.Add("Min10", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour3", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour6", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour12", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("Hour24", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("DayRainfall", typeof(decimal)).DefaultValue = 0m;
                dtTemp.Columns.Add("RT", typeof(decimal)).DefaultValue = 0m;

                var lines = File.ReadLines(item.FullName, Encoding.Default).Skip(1);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // ⚠️ 依據原本的 bw_DoWork 邏輯，這裡是使用 \t 來分割
                    string[] cols = line.Split(new[] { '\t' }, StringSplitOptions.None);
                    if (cols.Length < 7 || cols[0].StartsWith("#")) continue;

                    DateTime dtTime = DateTime.ParseExact(cols[1], "yyyy/M/d HH:mm", CultureInfo.InvariantCulture);
                    string sTime = dtTime.ToString("yyyyMMddHH");

                    // 處理數值並防呆 (<0 轉為 0)
                    decimal dMin10 = decimal.TryParse(cols[2], out decimal m10) ? (m10 < 0 ? 0 : m10) : 0;
                    decimal dHour = decimal.TryParse(cols[3], out decimal h1) ? (h1 < 0 ? 0 : h1) : 0;
                    decimal dHour6 = decimal.TryParse(cols[4], out decimal h6) ? (h6 < 0 ? 0 : h6) : 0;
                    decimal dHour24 = decimal.TryParse(cols[5], out decimal h24) ? (h24 < 0 ? 0 : h24) : 0;
                    decimal dDayRainfall = decimal.TryParse(cols[6], out decimal dr) ? (dr < 0 ? 0 : dr) : 0;

                    // Hour3, Hour12, RT 原本邏輯皆為 0，這裡傳入 null 觸發 DataTable 的 DefaultValue (0m)
                    dtTemp.Rows.Add(cols[0], sTime, dMin10, dHour, null, dHour6, null, dHour24, dDayRainfall, null);


                }

                if (dtTemp.Rows.Count == 0) continue;

                progress.Report($"開始進行 SqlBulkCopy 大批次寫入 ({dtTemp.Rows.Count} 筆)...");

                // =========================================================
                // 階段 2：大批次寫入與 MERGE
                // =========================================================
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    using (SqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            string createTempSql = @"
                        CREATE TABLE #TempTCGRain (
                            Stid VARCHAR(50), Time VARCHAR(20), Min10 DECIMAL(18,2), 
                            Hour DECIMAL(18,2), Hour3 DECIMAL(18,2), Hour6 DECIMAL(18,2), 
                            Hour12 DECIMAL(18,2), Hour24 DECIMAL(18,2), DayRainfall DECIMAL(18,2), RT DECIMAL(18,2)
                        )";
                            using (SqlCommand cmd = new SqlCommand(createTempSql, conn, tx)) await cmd.ExecuteNonQueryAsync();

                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tx))
                            {
                                bulkCopy.DestinationTableName = "#TempTCGRain";
                                bulkCopy.BatchSize = 10000;
                                bulkCopy.BulkCopyTimeout = 300;

                                foreach (DataColumn col in dtTemp.Columns)
                                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);

                                await RunWithLiveProgressAsync(() => bulkCopy.WriteToServerAsync(dtTemp), "SqlBulkCopy 寫入暫存表", progress);
                            }

                            // 1. 寫入或更新 北市府歷史資料 (TCGRainDataHis)
                            string mergeTCGSql = @"
                        MERGE INTO TCGRainDataHis AS T
                        USING #TempTCGRain AS S ON T.Stid = S.Stid AND T.Time = S.Time
                        WHEN MATCHED THEN
                            UPDATE SET Min10 = S.Min10, Hour = S.Hour, Hour3 = S.Hour3, Hour6 = S.Hour6, 
                                       Hour12 = S.Hour12, Hour24 = S.Hour24, DayRainfall = S.DayRainfall, RT = S.RT
                        WHEN NOT MATCHED THEN
                            INSERT (Stid, Time, Min10, Hour, Hour3, Hour6, Hour12, Hour24, DayRainfall, RT) 
                            VALUES (S.Stid, S.Time, S.Min10, S.Hour, S.Hour3, S.Hour6, S.Hour12, S.Hour24, S.DayRainfall, S.RT);";
                            using (SqlCommand cmd = new SqlCommand(mergeTCGSql, conn, tx))
                            {
                                cmd.CommandTimeout = 1200;
                                await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "MERGE 寫入 TCGRainDataHis", progress);
                            }

                            // 2. 同步寫入 氣象總表 (WeaRainData)
                            string mergeWeaSql = @"
                        MERGE INTO WeaRainData AS T
                        USING #TempTCGRain AS S ON T.STID = S.Stid AND T.time = S.Time
                        WHEN MATCHED THEN
                            UPDATE SET PP01 = S.Hour, PP01old = S.Hour
                        WHEN NOT MATCHED THEN
                            INSERT (STID, time, PP01, PP01old) VALUES (S.Stid, S.Time, S.Hour, S.Hour);";
                            using (SqlCommand cmd = new SqlCommand(mergeWeaSql, conn, tx))
                            {
                                cmd.CommandTimeout = 1200;
                                await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "MERGE 同步 WeaRainData", progress);
                            }

                            // 3. 同步寫入 氣象總表歷史 (WeaRainDataHis) 避免重複
                            string mergeWeaHisSql = @"
                        MERGE INTO WeaRainDataHis AS T
                        USING #TempTCGRain AS S ON T.Stid = S.Stid AND T.Time = S.Time
                        WHEN MATCHED THEN
                            UPDATE SET Hour = S.Hour
                        WHEN NOT MATCHED THEN
                            INSERT (Stid, Time, Hour, TransYN) VALUES (S.Stid, S.Time, S.Hour, 'N');";
                            using (SqlCommand cmd = new SqlCommand(mergeWeaHisSql, conn, tx))
                            {
                                cmd.CommandTimeout = 1200;
                                await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "MERGE 同步 WeaRainDataHis", progress);
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                // 搬移至 OK 資料夾
                string okDirPath = Path.Combine(item.DirectoryName, "OK");
                Directory.CreateDirectory(okDirPath);
                string newFullName = Path.Combine(okDirPath, item.Name);
                if (File.Exists(newFullName)) File.Delete(newFullName);
                item.MoveTo(newFullName);
            }

            // =========================================================
            // 階段 3：100% 純 SQL 資料庫雙向補遺 
            // =========================================================
            progress.Report("所有檔案轉檔完成，開始進行資料庫雙向補遺 (純 SQL 執行中)...");

            using (SqlConnection conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();

                // 補遺 1：將 TCGRainDataHis 中有，但 WeaRainData 沒有的資料塞進 WeaRainData
                string syncToWeaSql = @"
            INSERT INTO WeaRainData (STID, time, PP01, PP01old)
            SELECT a.Stid, a.Time, a.Hour, a.Hour
            FROM TCGRainDataHis a
            LEFT JOIN WeaRainData b ON a.Stid = b.STID AND a.Time = b.time
            WHERE b.STID IS NULL;";

                using (SqlCommand cmd = new SqlCommand(syncToWeaSql, conn))
                {
                    cmd.CommandTimeout = 1200;
                    await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "補遺：寫回 WeaRainData", progress);
                }

                // 補遺 2：將 WeaRainData 中有，但 TCGRainDataHis 沒有的資料塞回 TCGRainDataHis
                // 注意！這裡必須給未查詢到的欄位預設值 0，否則會發生 NOT NULL 的錯誤
                string syncToTCGSql = @"
                    INSERT INTO TCGRainDataHis (Stid, Time, Hour, Hour3, Hour6, Hour12, Hour24, DayRainfall, RT)
                    SELECT a.STID, a.time, a.PP01, 0, 0, 0, 0, 0, 0
                    FROM WeaRainData a
                    LEFT JOIN TCGRainDataHis b ON a.STID = b.Stid AND a.time = b.Time
                    WHERE b.Stid IS NULL 
                    AND a.STID IN (SELECT DISTINCT Stid FROM TCGRainDataHis);";

                using (SqlCommand cmd = new SqlCommand(syncToTCGSql, conn))
                {
                    cmd.CommandTimeout = 1200;
                    await RunWithLiveProgressAsync(() => cmd.ExecuteNonQueryAsync(), "補遺：寫回 TCGRainDataHis", progress);
                }
            }

            progress.Report("北市府雨量資料轉檔與雙向補遺全數完成！");
        }


        private void button28_Click(object sender, EventArgs e)
        {
            //全部的站都要跑都要跑
            ssql = @" select * from (
                select stid,time,count(*) tt  from SoilWaterRainDataHis  group by stid,time
                ) a where a.tt > 1 ";
            //如果要跑特定的的站，要抓取需要跑資料的STID
            //ssql = " select distinct stid from WraRainDataHis ";

            List<dynamic> SoilWaterRainDataHisList = dbDapper.Query(ssql);

            int iIndex = 0;
            foreach (var StidItem in SoilWaterRainDataHisList)
            {
                iIndex++;
                ShowStatus(string.Format("[{0}/{1}]{2}-{3}", iIndex, SoilWaterRainDataHisList.Count, StidItem.stid, StidItem.time));
                Application.DoEvents();
                ssql = @"
                    select * from SoilWaterRainDataHis where Stid = '{0}' and time = '{1}' 
                ";
                ssql = string.Format(ssql, StidItem.stid, StidItem.time);
                List<SoilWaterRainDataHis> TempList = dbDapper.Query<SoilWaterRainDataHis>(ssql);
                foreach (SoilWaterRainDataHis item in TempList)
                {
                    ssql = @"delete SoilWaterRainDataHis where no = '{0}'";
                    ssql = string.Format(ssql, item.no);
                    dbDapper.Execute(ssql);


                    Application.DoEvents();
                    break;
                }
                Application.DoEvents();
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {


            Stockhelper.getbrokerbuysellrank(null, null);


            return;



            //開始日期
            DateTime dt = new DateTime(2022, 6, 9);
            //DateTime dt = new DateTime(2019, 8, 2);
            //結束日期
            DateTime dtEnd = new DateTime(2022, 6, 9);

            ////取得資料庫最後一天
            //ssql = " select  distinct stockdate  from stockafter where stocktype = '{0}' order by stockdate asc ";

            //var vDbDate = dbDapper.ExecuteScale(string.Format(ssql, M10Const.StockType.tse));
            //if (vDbDate != null)
            //{
            //  string sDbDate = vDbDate.ToString();
            //  dt = new DateTime(Convert.ToInt32(sDbDate.Substring(0, 4))
            //    , Convert.ToInt32(sDbDate.Substring(4, 2))
            //    , Convert.ToInt32(sDbDate.Substring(6, 2)));
            //}

            //for (DateTime date = checkBgn; date <= checkEnd; date = date.AddDays(1))
            for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
            {
                string sLineTrans = "";
                try
                {
                    toolStripStatusLabel1.Text = string.Format("盤後資料轉檔(TSE){0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    //判斷是否已經轉檔成功
                    ssql = @" select * from stocklog where logtype = '{0}' and logdate = '{1}' and logstatus = 'success' ";
                    ssql = string.Format(ssql, M10Const.StockLogType.StockAfterTse, Utils.getDateString(LoopDatetime, M10Const.DateStringType.ADT1));
                    StockLog sl = dbDapper.QuerySingleOrDefault<StockLog>(ssql);
                    if (sl != null)
                    {
                        //System.Threading.Thread.Sleep(3000);
                        continue;
                    }

                    if (Stockhelper.GetStockAfterTse(LoopDatetime) == false)
                    {
                        System.Threading.Thread.Sleep(15000);
                        continue;
                    }

                    System.Threading.Thread.Sleep(5000);

                    toolStripStatusLabel1.Text = string.Format("盤後資料轉檔(TSE){0}-{1}", M10Const.StockType.tse, "完成");
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "stock after:" + sLineTrans);
                    System.Threading.Thread.Sleep(60000);
                }
            }
            toolStripStatusLabel1.Text = "盤後資料轉檔(TSE)完成";
        }

        private void button30_Click(object sender, EventArgs e)
        {
            string sSavePath = @"D:\專案資料\M15_113\20240510_四月份資料補遺處理\Temp\";
            string sOrgFilePath = @"D:\專案資料\M15_113\20240510_四月份資料補遺處理\2120_10min_a_ds_data.xml";
            string sOrgTime = "2024-04-03 21:20:00";
            FileInfo fiOrg = new FileInfo(sOrgFilePath);


            DateTime dtStart = DateTime.ParseExact("202404032130", "yyyyMMddHHmm", null);
            DateTime dtFinish = DateTime.ParseExact("202404251100", "yyyyMMddHHmm", null);


            for (DateTime x = dtStart; x <= dtFinish; x = x.AddMinutes(10))
            {
                //產生當日資料夾
                string sDayFolder = Path.Combine(sSavePath, x.ToString("MMdd"));
                Directory.CreateDirectory(sDayFolder);

                //複製XML檔案，另存在當日資料夾，修改檔案名稱
                string sNewFile = Path.Combine(sDayFolder, string.Format("{0}_10min_a_ds_data.xml", x.ToString("HHmm")));
                fiOrg.CopyTo(sNewFile, true);

                //取代XML檔案內容，修改為目前時段資料

                string sNewTime = x.ToString("yyyy-MM-dd HH:mm:00");

                string text = File.ReadAllText(sNewFile);
                text = text.Replace(sOrgTime, sNewTime);
                File.WriteAllText(sNewFile, text);

            }


        }

        /// <summary>
        /// 更新雨量站基本資料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button31_Click(object sender, EventArgs e)
        {
            ssql = @" select *  from BasStationDataTemp ";
            //更新來源-學長提供Excel匯入的來源
            List<dynamic> stidBasStationDataTemp = dbDapper.Query(ssql);

            foreach (var item in stidBasStationDataTemp)
            {
                ssql = @" select * from BasStationData where stid = '{0}' ";
                ssql = string.Format(ssql, item.stid);
                BasStationData dsl_new = dbDapper.QuerySingleOrDefault<BasStationData>(ssql);


                if (dsl_new == null)
                {
                    //如果沒有轉檔紀錄，則新增一筆執行中
                    //寫入統計LOG
                    dsl_new = new BasStationData();
                    dsl_new.stid = item.stid;
                    dsl_new.stname = item.stname;
                    dsl_new.unittype = item.unittype;
                    dsl_new.remark = item.remark;
                    dsl_new.enable = "Y";

                    dbDapper.Insert(dsl_new);
                }
                else//已經有轉檔紀錄
                {
                    //更新
                    dsl_new.stname = item.stname;
                    dsl_new.unittype = item.unittype;
                    dsl_new.enable = "Y";
                    dsl_new.remark = item.remark;
                    dbDapper.Update(dsl_new);
                }
            }

        }

        private void button32Old_Click(object sender, EventArgs e)
        {
            //目的1-更新雨場單位
            //目的2-更新雨場時雨量

            //0-單獨執行
            //1-跑完雨場分割後，執行該程式



            ssql = @" select distinct stid from WeaRainArea order by STID ";
            List<dynamic> StidList = dbDapper.Query(ssql);

            int iIndex = 0;
            foreach (var StidItem in StidList)
            {
                iIndex++;

                //取得雨場單位              
                ssql = @" select * from BasStationData where stid = @stid ";
                BasStationData bsd = dbDapper.QuerySingleOrDefault<BasStationData>(ssql, new { stid = StidItem.stid });

                ShowStatus(string.Format("更新雨量站單位[{2}/{3}]{0}-{1}", StidItem.stid, "", iIndex.ToString(), StidList.Count.ToString()));
                if (bsd != null)
                {
                    ssql = @" update WeaRainArea set UnitType = '{0}' where stid = '{1}' ";
                    ssql = string.Format(ssql, bsd.unittype, StidItem.stid);
                    dbDapper.Execute(ssql);
                }

                //取得雨量站歷年雨量資料               
                ssql = @" select * from WeaRainData where stid = @stid order by time ";
                List<WeaRainData> wrdList = dbDapper.Query<WeaRainData>(ssql, new { stid = StidItem.stid });

                //取得條件雨場分割資料
                ssql = @" select * from WeaRainArea where stid = @stid order by stid,TimeStart";
                List<WeaRainArea> WeaRainAreaList = dbDapper.Query<WeaRainArea>(ssql, new { stid = StidItem.stid });

                foreach (WeaRainArea item in WeaRainAreaList)
                {
                    ShowStatus(string.Format("更新雨場時雨量列表[{2}/{3}]{0}-{1}", item.stid, item.TimeStart, iIndex.ToString(), StidList.Count.ToString()));

                    //取得雨場時間起迄
                    int iTimeStart = Convert.ToInt32(item.TimeStart);
                    int iTimeEnd = Convert.ToInt32(item.TimeEnd);

                    //加入時雨量資料
                    List<WeaRainData> RainDataList = wrdList.Where(s => Convert.ToInt32(s.time) >= iTimeStart && Convert.ToInt32(s.time) <= iTimeEnd).ToList<WeaRainData>();

                    //更新資料
                    item.RainHourList = string.Join("|", RainDataList.Select(x => x.PP01.ToString()).ToList<string>());
                    dbDapper.Update<WeaRainArea>(item);

                }
            }
        }

        // =========================================================
        // UI 按鈕觸發事件 (非同步防卡死)
        // =========================================================
        private async void button32_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null) btn.Enabled = false;

            // 將進度訊息導向 MsgText (RichTextBox) 或您原本的 ShowStatus
            var progress = new Progress<string>(msg =>
            {
                // 替換成您原本的 ShowStatus 寫法，例如: ShowStatus(msg);
                LogToMsgText(msg);
            });

            try
            {
                await Task.Run(() => ProcessRainAreaUpdate(progress));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新發生錯誤: {ex.Message}");
            }
            finally
            {
                if (btn != null) btn.Enabled = true;
            }
        }

        // =========================================================
        // 核心雨場更新邏輯 (極速批次版)
        // =========================================================
        private void ProcessRainAreaUpdate(IProgress<string> progress)
        {
            progress.Report("開始整批更新全台雨場單位 (UnitType)...");

            // 🔥 目的1 極速優化：用一句 SQL 直接更新全台灣所有測站的 UnitType，完全消滅第一層的 SELECT 與迴圈 UPDATE
            string updateUnitSql = @"
                UPDATE a
                SET a.UnitType = b.unittype
                FROM WeaRainArea a
                INNER JOIN BasStationData b ON a.stid = b.stid;";
            dbDapper.Execute(updateUnitSql);

            progress.Report("雨場單位更新完成！開始處理各站雨場時雨量...");

            // =========================================================
            // 1. 外層過濾：只抓取「還有未更新雨場」的測站清單
            // =========================================================
            string getStidSql = "SELECT DISTINCT stid FROM WeaRainArea WHERE RainHourList IS NULL ORDER BY STID";
            List<dynamic> stidDynamicList = dbDapper.Query(getStidSql);
            List<string> stidList = stidDynamicList.Select(d => (string)d.stid).ToList();

            int iIndex = 0;
            foreach (string sStid in stidList)
            {
                iIndex++;
                progress.Report($"[{sStid}] ({iIndex}/{stidList.Count}) 讀取資料與計算中...");

                // 取得該站歷年雨量資料 (這段不變，因為要用來找區間內的時雨量)
                string dataSql = "SELECT [time], PP01 FROM WeaRainData WHERE stid = @stid ORDER BY [time]";
                List<WeaRainData> wrdList = dbDapper.Query<WeaRainData>(dataSql, new { stid = sStid });

                var parsedRainData = wrdList.Select(w => new
                {
                    TimeNum = long.Parse(w.time),
                    PP01Str = w.PP01.ToString()
                }).ToList();

                // =========================================================
                // 2. 內層過濾：該測站「只抓出 RainHourList 是空的」雨場出來算就好！
                // =========================================================
                string areaSql = "SELECT * FROM WeaRainArea WHERE stid = @stid AND RainHourList IS NULL ORDER BY TimeStart";
                List<WeaRainArea> WeaRainAreaList = dbDapper.Query<WeaRainArea>(areaSql, new { stid = sStid });

                var updateParams = new List<object>();

                foreach (WeaRainArea item in WeaRainAreaList)
                {
                    long iTimeStart = long.Parse(item.TimeStart);
                    long iTimeEnd = long.Parse(item.TimeEnd);

                    var rainHourStrs = parsedRainData
                        .Where(s => s.TimeNum >= iTimeStart && s.TimeNum <= iTimeEnd)
                        .Select(x => x.PP01Str);

                    string rainHourListStr = string.Join("|", rainHourStrs);

                    updateParams.Add(new
                    {
                        RainHourList = rainHourListStr,
                        stid = item.stid,
                        TimeStart = item.TimeStart
                    });
                }

                if (updateParams.Count > 0)
                {
                    progress.Report($"[{sStid}] ({iIndex}/{stidList.Count}) 寫回資料庫 ({updateParams.Count}筆需更新的雨場)...");

                    string batchUpdateSql = @"
                        UPDATE WeaRainArea 
                        SET RainHourList = @RainHourList 
                        WHERE stid = @stid AND TimeStart = @TimeStart;";

                    dbDapper.Execute(batchUpdateSql, updateParams);
                }
            }

            progress.Report("所有雨場時雨量更新完成！");
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            try
            {
                string batchUpdateSql = @"
                        delete DataStaticLog where type = 'WeaRainTransHisSTID' and status  = '10'";

                dbDapper.Execute(batchUpdateSql);

                LogToMsgText($"氣象局歷史資料轉檔-狀態10清除完成。");
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {   
                string batchUpdateSql = @"
                        delete DataStaticLog where type = 'SoilWaterRainHisSTID' and status  = '10'";

                dbDapper.Execute(batchUpdateSql);

                LogToMsgText($"水保局歷史資料轉檔-狀態10清除完成。");

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            try
            {   
                string batchUpdateSql = @"
                        delete DataStaticLog where type = 'WraRainHisSTID' and status  = '10'";

                dbDapper.Execute(batchUpdateSql);

                LogToMsgText($"水利署歷史資料轉檔-狀態10清除完成。");

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            try
            {
                string batchUpdateSql = @"
                        delete DataStaticLog where type = 'TCGRainHisSTID' and status  = '10'";

                dbDapper.Execute(batchUpdateSql);

                LogToMsgText($"北市府歷史資料轉檔-狀態10清除完成。");

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class TumdOnlineData
    {
        public string date { get; set; }

        public string value { get; set; }

    }


    public class RootObject
    {
        public List<TumdOnlineData> Datas { get; set; }
    }

    public class Data1
    {
        public string Name { get; set; }

        public string Value { get; set; }

    }


    public class RootObject1
    {
        public List<Data1> Datas { get; set; }
    }



    public class RtiProcOld
    {
        ODAL oDal;
        string ssql = string.Empty;
        string sver = string.Empty;
        string sstation = string.Empty;
        string sdelaytime = string.Empty;
        string stype = string.Empty;
        string sCoefficient = string.Empty;
        //string sConnectionString = Properties.Settings.Default.DBConnectionString;
        //ODAL oDal;
        public DataTable dt_rti = new DataTable();
        public DataTable dt_rti90 = new DataTable();

        public double dRTI10 = 0;
        public double dRTI30 = 0;
        public double dRTI50 = 0;
        public double dRTI70 = 0;
        public double dRTI90 = 0;

        public double dRT10 = 0;
        public double dRT30 = 0;
        public double dRT50 = 0;
        public double dRT70 = 0;
        public double dRT90 = 0;

        public double dR310 = 0;
        public double dR330 = 0;
        public double dR350 = 0;
        public double dR370 = 0;
        public double dR390 = 0;



        public RtiProcOld(string ver, string station, string delaytime, string Coefficient, string type, string sConnString)
        {
            sver = ver;
            sstation = station;
            sdelaytime = delaytime;
            stype = type;
            sCoefficient = Coefficient;

            oDal = new ODAL(sConnString);
            buildtableRTI();
        }


        public void RtiCal()
        {
            try
            {
                DataTable dt = new DataTable();

                //取得總筆數                
                ssql = " select count(*) as total from RtiData2 "
                     + " where station = '" + sstation + "' "
                     + " and ver = '" + sver + "' "
                     ;

                if (sdelaytime != "0")
                {
                    ssql += " and raindelay > " + sdelaytime + " ";
                }

                oDal.CommandText = ssql;
                string sTotalcount = oDal.Value().ToString().Trim();


                //取得所有站號
                ssql = " select * from RtiData2"
                     + "  where station = '" + sstation + "'  "
                     + " and ver = '" + sver + "' "
                     ;

                if (sdelaytime != "0")
                {
                    ssql += " and raindelay > " + sdelaytime + " ";
                }

                //if (stype == "RTI") ssql += " order by rti  ";
                //if (stype == "RTI3") ssql += " order by rti3  ";

                if (stype == "RTI")
                {
                    if (sCoefficient == "6") ssql += " order by rti6  ";
                    if (sCoefficient == "7") ssql += " order by rti7  ";
                    if (sCoefficient == "8") ssql += " order by rti8  ";
                }
                if (stype == "RTI3")
                {
                    if (sCoefficient == "6") ssql += " order by rti36 ";
                    if (sCoefficient == "7") ssql += " order by rti37 ";
                    if (sCoefficient == "8") ssql += " order by rti38 ";
                }

                oDal.CommandText = ssql;
                dt.Clear();
                dt = oDal.DataTable();
                int iIndex = 1;
                dt_rti.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow newdr = dt_rti.NewRow();

                    newdr["index"] = iIndex.ToString();
                    newdr["station"] = dr["station"].ToString();
                    if (stype == "RTI")
                    {
                        if (sCoefficient == "6") newdr["rti"] = dr["rti6"].ToString();
                        if (sCoefficient == "7") newdr["rti"] = dr["rti7"].ToString();
                        if (sCoefficient == "8") newdr["rti"] = dr["rti8"].ToString();
                    }
                    if (stype == "RTI3")
                    {
                        if (sCoefficient == "6") newdr["rti"] = dr["rti36"].ToString();
                        if (sCoefficient == "7") newdr["rti"] = dr["rti37"].ToString();
                        if (sCoefficient == "8") newdr["rti"] = dr["rti38"].ToString();
                    }
                    newdr["totalcount"] = sTotalcount;
                    dt_rti.Rows.Add(newdr);



                    double dtotalcount = double.Parse(sTotalcount);
                    double dIndex = double.Parse(iIndex.ToString());

                    //前三個小時平均 * RT值
                    double dResult = dIndex / (dtotalcount + 1) * 100;
                    string sResult = Math.Round(dResult, 2).ToString();

                    newdr["pas"] = sResult;

                    iIndex++;
                }


                //找出最接近10%的數值
                double dRTIA = 0;
                double dRTIB = 0;
                double dPASa = 0;
                double dPASb = 0;


                //bool bCopy = false;
                foreach (DataRow dr in dt_rti.Rows)
                {
                    //暫存前一筆
                    dPASa = dPASb;
                    dRTIA = dRTIB;

                    //取得目前這一筆
                    dPASb = double.Parse(dr["pas"].ToString());
                    dRTIB = double.Parse(dr["rti"].ToString());

                    //數值剛好等於10，則該數值為rti10
                    if (dPASb == 10)
                    {
                        dRTI10 = dRTIB;
                    }
                    else
                    {
                        //判斷兩組資料是否為10%中間
                        if (dPASa < 10 && dPASb > 10)
                        {
                            //進行計算RTI10
                            dRTI10 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (10 - dPASa)) + dRTIA;
                            dRTI10 = Math.Round(dRTI10, 2);
                        }
                    }

                    //複製除了rti10之前的資料到dt_rti90
                    if (dPASb > 10)
                    {
                        DataRow drn = dt_rti90.NewRow();
                        drn["index"] = dr["index"];
                        drn["station"] = dr["station"];
                        drn["rti"] = dr["rti"];
                        drn["totalcount"] = dr["totalcount"];
                        drn["pas"] = dr["pas"];

                        dt_rti90.Rows.Add(drn);
                    }
                }

                //dt_rti90重整
                int irti90 = 1;
                foreach (DataRow dr in dt_rti90.Rows)
                {
                    double dtotalcount = double.Parse(dt_rti90.Rows.Count.ToString());
                    double dIndex = double.Parse(irti90.ToString());

                    //前三個小時平均 * RT值
                    double dResult = dIndex / (dtotalcount + 1) * 100;
                    string sResult = Math.Round(dResult, 2).ToString();

                    dr["index"] = irti90.ToString();
                    dr["pas"] = sResult;

                    irti90++;
                }

                dRTIA = 0;
                dRTIB = 0;
                dPASa = 0;
                dPASb = 0;

                foreach (DataRow dr in dt_rti90.Rows)
                {
                    //暫存前一筆
                    dPASa = dPASb;
                    dRTIA = dRTIB;

                    //取得目前這一筆
                    dPASb = double.Parse(dr["pas"].ToString());
                    dRTIB = double.Parse(dr["rti"].ToString());


                    if (dPASb == 90)
                    {
                        dRTI90 = dRTIB;
                    }
                    else
                    {
                        //判斷兩組資料是否為90%中間
                        if (dPASa < 90 && dPASb > 90)
                        {
                            //進行計算RTI90
                            dRTI90 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (90 - dPASa)) + dRTIA;
                            dRTI90 = Math.Round(dRTI90, 2);
                        }
                    }



                }


                dRTI30 = ((dRTI90 - dRTI10) / 8 * 2) + dRTI10;
                dRTI50 = ((dRTI90 - dRTI10) / 8 * 4) + dRTI10;
                dRTI70 = ((dRTI90 - dRTI10) / 8 * 6) + dRTI10;

                //小數點以下兩位進位
                dRTI30 = Math.Round(dRTI30, 2);
                dRTI50 = Math.Round(dRTI50, 2);
                dRTI70 = Math.Round(dRTI70, 2);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Rti3Cal()
        {
            try
            {
                //====================================================================================
                //計算RTI
                DataTable dt = new DataTable();

                //取得總筆數                
                ssql = " select count(*) as total from RtiData2 "
                     + " where station = '" + sstation + "' "
                     + " and ver = '" + sver + "' "
                     ;

                if (sdelaytime != "0")
                {
                    ssql += " and raindelay > " + sdelaytime + " ";
                }

                oDal.CommandText = ssql;
                string sTotalcount = oDal.Value().ToString().Trim();


                //取得所有站號
                ssql = " select * from RtiData2"
                     + "  where station = '" + sstation + "'  "
                     + " and ver = '" + sver + "' "
                     ;

                if (sdelaytime != "0")
                {
                    ssql += " and raindelay > " + sdelaytime + " ";
                }


                if (stype == "RTI3")
                {
                    if (sCoefficient == "6") ssql += " order by rti36 ";
                    if (sCoefficient == "7") ssql += " order by rti37 ";
                    if (sCoefficient == "8") ssql += " order by rti38 ";
                }

                oDal.CommandText = ssql;
                dt.Clear();
                dt = oDal.DataTable();
                int iIndex = 1;
                dt_rti.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow newdr = dt_rti.NewRow();

                    newdr["index"] = iIndex.ToString();
                    newdr["station"] = dr["station"].ToString();
                    if (stype == "RTI3")
                    {
                        if (sCoefficient == "6") newdr["rti"] = dr["rti36"].ToString();
                        if (sCoefficient == "7") newdr["rti"] = dr["rti37"].ToString();
                        if (sCoefficient == "8") newdr["rti"] = dr["rti38"].ToString();
                    }
                    newdr["totalcount"] = sTotalcount;
                    dt_rti.Rows.Add(newdr);

                    double dtotalcount = double.Parse(sTotalcount);
                    double dIndex = double.Parse(iIndex.ToString());

                    //前三個小時平均 * RT值
                    double dResult = dIndex / (dtotalcount + 1) * 100;
                    string sResult = Math.Round(dResult, 2).ToString();

                    newdr["pas"] = sResult;

                    iIndex++;
                }


                //找出最接近10%的數值
                double dRTIA = 0;
                double dRTIB = 0;
                double dPASa = 0;
                double dPASb = 0;


                //bool bCopy = false;
                foreach (DataRow dr in dt_rti.Rows)
                {
                    //暫存前一筆
                    dPASa = dPASb;
                    dRTIA = dRTIB;

                    //取得目前這一筆
                    dPASb = double.Parse(dr["pas"].ToString());
                    dRTIB = double.Parse(dr["rti"].ToString());

                    //數值剛好等於10，則該數值為rti10
                    if (dPASb == 10)
                    {
                        dRTI10 = dRTIB;
                    }
                    else
                    {
                        //判斷兩組資料是否為10%中間
                        if (dPASa < 10 && dPASb > 10)
                        {
                            //進行計算RTI10
                            dRTI10 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (10 - dPASa)) + dRTIA;
                            dRTI10 = Math.Round(dRTI10, 2);
                        }
                    }

                    //複製除了rti10之前的資料到dt_rti90
                    if (dPASb > 10)
                    {
                        DataRow drn = dt_rti90.NewRow();
                        drn["index"] = dr["index"];
                        drn["station"] = dr["station"];
                        drn["rti"] = dr["rti"];
                        drn["totalcount"] = dr["totalcount"];
                        drn["pas"] = dr["pas"];

                        dt_rti90.Rows.Add(drn);
                    }
                }

                //dt_rti90重整
                int irti90 = 1;
                foreach (DataRow dr in dt_rti90.Rows)
                {
                    double dtotalcount = double.Parse(dt_rti90.Rows.Count.ToString());
                    double dIndex = double.Parse(irti90.ToString());

                    //前三個小時平均 * RT值
                    double dResult = dIndex / (dtotalcount + 1) * 100;
                    string sResult = Math.Round(dResult, 2).ToString();

                    dr["index"] = irti90.ToString();
                    dr["pas"] = sResult;

                    irti90++;
                }

                dRTIA = 0;
                dRTIB = 0;
                dPASa = 0;
                dPASb = 0;

                foreach (DataRow dr in dt_rti90.Rows)
                {
                    //暫存前一筆
                    dPASa = dPASb;
                    dRTIA = dRTIB;

                    //取得目前這一筆
                    dPASb = double.Parse(dr["pas"].ToString());
                    dRTIB = double.Parse(dr["rti"].ToString());


                    if (dPASb == 90)
                    {
                        dRTI90 = dRTIB;
                    }
                    else
                    {
                        //判斷兩組資料是否為90%中間
                        if (dPASa < 90 && dPASb > 90)
                        {
                            //進行計算RTI90
                            dRTI90 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (90 - dPASa)) + dRTIA;
                            dRTI90 = Math.Round(dRTI90, 2);
                        }
                    }



                }


                dRTI30 = ((dRTI90 - dRTI10) / 8 * 2) + dRTI10;
                dRTI50 = ((dRTI90 - dRTI10) / 8 * 4) + dRTI10;
                dRTI70 = ((dRTI90 - dRTI10) / 8 * 6) + dRTI10;

                //小數點以下兩位進位
                dRTI30 = Math.Round(dRTI30, 2);
                dRTI50 = Math.Round(dRTI50, 2);
                dRTI70 = Math.Round(dRTI70, 2);


                //====================================================================================
                //計算RT
                dt.Clear();
                dt_rti.Clear();
                dt_rti90.Clear();
                //取得所有站號
                ssql = " select * from RtiData2"
                     + "  where station = '" + sstation + "'  "
                     + " and ver = '" + sver + "' "
                     ;

                if (sdelaytime != "0")
                {
                    ssql += " and raindelay > " + sdelaytime + " ";
                }


                if (stype == "RTI3")
                {
                    if (sCoefficient == "6") ssql += " order by rtd6 ";
                    if (sCoefficient == "7") ssql += " order by rtd7 ";
                    if (sCoefficient == "8") ssql += " order by rtd8 ";
                }

                oDal.CommandText = ssql;
                dt.Clear();
                dt = oDal.DataTable();
                iIndex = 1;
                dt_rti.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow newdr = dt_rti.NewRow();

                    newdr["index"] = iIndex.ToString();
                    newdr["station"] = dr["station"].ToString();
                    if (stype == "RTI3")
                    {
                        if (sCoefficient == "6") newdr["rti"] = dr["rtd6"].ToString();
                        if (sCoefficient == "7") newdr["rti"] = dr["rtd7"].ToString();
                        if (sCoefficient == "8") newdr["rti"] = dr["rtd8"].ToString();
                    }
                    newdr["totalcount"] = sTotalcount;
                    dt_rti.Rows.Add(newdr);

                    double dtotalcount = double.Parse(sTotalcount);
                    double dIndex = double.Parse(iIndex.ToString());

                    //前三個小時平均 * RT值
                    double dResult = dIndex / (dtotalcount + 1) * 100;
                    string sResult = Math.Round(dResult, 2).ToString();

                    newdr["pas"] = sResult;

                    iIndex++;
                }


                //找出最接近10%的數值
                dRTIA = 0;
                dRTIB = 0;
                dPASa = 0;
                dPASb = 0;


                //bool bCopy = false;
                foreach (DataRow dr in dt_rti.Rows)
                {
                    //暫存前一筆
                    dPASa = dPASb;
                    dRTIA = dRTIB;

                    //取得目前這一筆
                    dPASb = double.Parse(dr["pas"].ToString());
                    dRTIB = double.Parse(dr["rti"].ToString());

                    //數值剛好等於10，則該數值為rti10
                    if (dPASb == 10)
                    {
                        dRT10 = dRTIB;
                    }
                    else
                    {
                        //判斷兩組資料是否為10%中間
                        if (dPASa < 10 && dPASb > 10)
                        {
                            //進行計算RTI10
                            dRT10 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (10 - dPASa)) + dRTIA;
                            dRT10 = Math.Round(dRT10, 2);
                        }
                    }

                    //複製除了rti10之前的資料到dt_rti90
                    if (dPASb > 10)
                    {
                        DataRow drn = dt_rti90.NewRow();
                        drn["index"] = dr["index"];
                        drn["station"] = dr["station"];
                        drn["rti"] = dr["rti"];
                        drn["totalcount"] = dr["totalcount"];
                        drn["pas"] = dr["pas"];

                        dt_rti90.Rows.Add(drn);
                    }
                }

                //dt_rti90重整
                irti90 = 1;
                foreach (DataRow dr in dt_rti90.Rows)
                {
                    double dtotalcount = double.Parse(dt_rti90.Rows.Count.ToString());
                    double dIndex = double.Parse(irti90.ToString());

                    //前三個小時平均 * RT值
                    double dResult = dIndex / (dtotalcount + 1) * 100;
                    string sResult = Math.Round(dResult, 2).ToString();

                    dr["index"] = irti90.ToString();
                    dr["pas"] = sResult;

                    irti90++;
                }

                dRTIA = 0;
                dRTIB = 0;
                dPASa = 0;
                dPASb = 0;

                foreach (DataRow dr in dt_rti90.Rows)
                {
                    //暫存前一筆
                    dPASa = dPASb;
                    dRTIA = dRTIB;

                    //取得目前這一筆
                    dPASb = double.Parse(dr["pas"].ToString());
                    dRTIB = double.Parse(dr["rti"].ToString());


                    if (dPASb == 90)
                    {
                        dRT90 = dRTIB;
                    }
                    else
                    {
                        //判斷兩組資料是否為90%中間
                        if (dPASa < 90 && dPASb > 90)
                        {
                            //進行計算RTI90
                            dRT90 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (90 - dPASa)) + dRTIA;
                            dRT90 = Math.Round(dRT90, 2);
                        }
                    }



                }


                dRT30 = ((dRT90 - dRT10) / 8 * 2) + dRT10;
                dRT50 = ((dRT90 - dRT10) / 8 * 4) + dRT10;
                dRT70 = ((dRT90 - dRT10) / 8 * 6) + dRT10;

                //小數點以下兩位進位
                dRT30 = Math.Round(dRT30, 2);
                dRT50 = Math.Round(dRT50, 2);
                dRT70 = Math.Round(dRT70, 2);

                //====================================================================================
                //計算R3
                dt.Clear();
                dt_rti.Clear();
                dt_rti90.Clear();
                //取得所有站號
                ssql = " select * from RtiData2"
                     + "  where station = '" + sstation + "'  "
                     + " and ver = '" + sver + "' "
                     ;

                if (sdelaytime != "0")
                {
                    ssql += " and raindelay > " + sdelaytime + " ";
                }


                if (stype == "RTI3")
                {
                    if (sCoefficient == "6") ssql += " order by R3 ";
                    if (sCoefficient == "7") ssql += " order by R3 ";
                    if (sCoefficient == "8") ssql += " order by R3 ";
                }

                oDal.CommandText = ssql;
                dt.Clear();
                dt = oDal.DataTable();
                iIndex = 1;
                dt_rti.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow newdr = dt_rti.NewRow();

                    newdr["index"] = iIndex.ToString();
                    newdr["station"] = dr["station"].ToString();
                    if (stype == "RTI3")
                    {
                        if (sCoefficient == "6") newdr["rti"] = dr["R3"].ToString();
                        if (sCoefficient == "7") newdr["rti"] = dr["R3"].ToString();
                        if (sCoefficient == "8") newdr["rti"] = dr["R3"].ToString();
                    }
                    newdr["totalcount"] = sTotalcount;
                    dt_rti.Rows.Add(newdr);

                    double dtotalcount = double.Parse(sTotalcount);
                    double dIndex = double.Parse(iIndex.ToString());

                    //前三個小時平均 * RT值
                    double dResult = dIndex / (dtotalcount + 1) * 100;
                    string sResult = Math.Round(dResult, 2).ToString();

                    newdr["pas"] = sResult;

                    iIndex++;
                }


                //找出最接近10%的數值
                dRTIA = 0;
                dRTIB = 0;
                dPASa = 0;
                dPASb = 0;


                //bool bCopy = false;
                foreach (DataRow dr in dt_rti.Rows)
                {
                    //暫存前一筆
                    dPASa = dPASb;
                    dRTIA = dRTIB;

                    //取得目前這一筆
                    dPASb = double.Parse(dr["pas"].ToString());
                    dRTIB = double.Parse(dr["rti"].ToString());

                    //數值剛好等於10，則該數值為rti10
                    if (dPASb == 10)
                    {
                        dR310 = dRTIB;
                    }
                    else
                    {
                        //判斷兩組資料是否為10%中間
                        if (dPASa < 10 && dPASb > 10)
                        {
                            //進行計算RTI10
                            dR310 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (10 - dPASa)) + dRTIA;
                            dR310 = Math.Round(dR310, 2);
                        }
                    }

                    //複製除了rti10之前的資料到dt_rti90
                    if (dPASb > 10)
                    {
                        DataRow drn = dt_rti90.NewRow();
                        drn["index"] = dr["index"];
                        drn["station"] = dr["station"];
                        drn["rti"] = dr["rti"];
                        drn["totalcount"] = dr["totalcount"];
                        drn["pas"] = dr["pas"];

                        dt_rti90.Rows.Add(drn);
                    }
                }

                //dt_rti90重整
                irti90 = 1;
                foreach (DataRow dr in dt_rti90.Rows)
                {
                    double dtotalcount = double.Parse(dt_rti90.Rows.Count.ToString());
                    double dIndex = double.Parse(irti90.ToString());

                    //前三個小時平均 * RT值
                    double dResult = dIndex / (dtotalcount + 1) * 100;
                    string sResult = Math.Round(dResult, 2).ToString();

                    dr["index"] = irti90.ToString();
                    dr["pas"] = sResult;

                    irti90++;
                }

                dRTIA = 0;
                dRTIB = 0;
                dPASa = 0;
                dPASb = 0;

                foreach (DataRow dr in dt_rti90.Rows)
                {
                    //暫存前一筆
                    dPASa = dPASb;
                    dRTIA = dRTIB;

                    //取得目前這一筆
                    dPASb = double.Parse(dr["pas"].ToString());
                    dRTIB = double.Parse(dr["rti"].ToString());


                    if (dPASb == 90)
                    {
                        dR390 = dRTIB;
                    }
                    else
                    {
                        //判斷兩組資料是否為90%中間
                        if (dPASa < 90 && dPASb > 90)
                        {
                            //進行計算RTI90
                            dR390 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (90 - dPASa)) + dRTIA;
                            dR390 = Math.Round(dR390, 2);
                        }
                    }



                }


                dR330 = ((dR390 - dR310) / 8 * 2) + dR310;
                dR350 = ((dR390 - dR310) / 8 * 4) + dR310;
                dR370 = ((dR390 - dR310) / 8 * 6) + dR310;

                //小數點以下兩位進位
                dR330 = Math.Round(dR330, 2);
                dR350 = Math.Round(dR350, 2);
                dR370 = Math.Round(dR370, 2);

            }
            catch (Exception)
            {

                throw;
            }
        }



        private void buildtableRTI()
        {
            dt_rti.Columns.Add("index");
            dt_rti.Columns.Add("station");
            dt_rti.Columns.Add("rti");
            dt_rti.Columns.Add("totalcount");
            dt_rti.Columns.Add("pas");

            dt_rti90.Columns.Add("index");
            dt_rti90.Columns.Add("station");
            dt_rti90.Columns.Add("rti");
            dt_rti90.Columns.Add("totalcount");
            dt_rti90.Columns.Add("pas");
        }
    }

    public class RtiProc
    {
        public double dRTI10, dRTI30, dRTI50, dRTI70, dRTI90;
        public double dRT10, dRT30, dRT50, dRT70, dRT90;
        public double dR310, dR330, dR350, dR370, dR390;

        // 不再需要傳入資料庫連線，直接傳入已經撈好的數值陣列即可！
        public RtiProc() { }

        public void RtiCal(IEnumerable<double> rtiValues)
        {
            var res = CalculatePercentiles(rtiValues);
            dRTI10 = res[0]; dRTI30 = res[1]; dRTI50 = res[2]; dRTI70 = res[3]; dRTI90 = res[4];
        }

        public void Rti3Cal(IEnumerable<double> rti3Values, IEnumerable<double> rtValues, IEnumerable<double> r3Values)
        {
            var res1 = CalculatePercentiles(rti3Values);
            dRTI10 = res1[0]; dRTI30 = res1[1]; dRTI50 = res1[2]; dRTI70 = res1[3]; dRTI90 = res1[4];

            var res2 = CalculatePercentiles(rtValues);
            dRT10 = res2[0]; dRT30 = res2[1]; dRT50 = res2[2]; dRT70 = res2[3]; dRT90 = res2[4];

            var res3 = CalculatePercentiles(r3Values);
            dR310 = res3[0]; dR330 = res3[1]; dR350 = res3[2]; dR370 = res3[3]; dR390 = res3[4];
        }

        /// <summary>
        /// 核心演算法：完美還原您原本的 DataTable 計算邏輯，但速度快上萬倍
        /// </summary>
        private double[] CalculatePercentiles(IEnumerable<double> rawValues)
        {
            double[] results = new double[5]; // 存放 10, 30, 50, 70, 90 的結果
            if (rawValues == null || !rawValues.Any()) return results;

            // 確保數值是由小到大排序 (取代原本 SQL 的 ORDER BY)
            var values = rawValues.OrderBy(v => v).ToList();
            int N = values.Count;

            // ==========================================
            // 1. 計算 RTI10
            // ==========================================
            double rti10 = 0;
            List<double> remainingValues = new List<double>();
            double pasA = 0, valA = 0;
            bool rti10Found = false;

            for (int i = 0; i < N; i++)
            {
                double valB = values[i];
                // 保留小數點精確度，不先轉字串再轉回 Double，數學更精準
                double pasB = Math.Round((i + 1.0) / (N + 1.0) * 100.0, 2);

                if (!rti10Found)
                {
                    if (Math.Abs(pasB - 10.0) < 0.0001) // 剛好等於 10
                    {
                        rti10 = valB;
                        rti10Found = true;
                    }
                    else if (pasA < 10.0 && pasB > 10.0) // 跨過 10，進行內插法計算
                    {
                        rti10 = ((valB - valA) / (pasB - pasA) * (10.0 - pasA)) + valA;
                        rti10Found = true;
                    }
                }

                // 對應原本「複製除了 rti10 之前的資料到 dt_rti90」的邏輯
                if (pasB > 10.0) remainingValues.Add(valB);

                pasA = pasB;
                valA = valB;
            }

            // 防呆：如果資料太少算不出 10%，抓第一筆
            if (!rti10Found) rti10 = values.FirstOrDefault();

            // ==========================================
            // 2. 計算 RTI90 (針對剔除後的 remainingValues 重算)
            // ==========================================
            double rti90 = 0;
            int M = remainingValues.Count;
            pasA = 0; valA = 0;
            bool rti90Found = false;

            for (int i = 0; i < M; i++)
            {
                double valB = remainingValues[i];
                double pasB = Math.Round((i + 1.0) / (M + 1.0) * 100.0, 2);

                if (!rti90Found)
                {
                    if (Math.Abs(pasB - 90.0) < 0.0001)
                    {
                        rti90 = valB;
                        rti90Found = true;
                    }
                    else if (pasA < 90.0 && pasB > 90.0)
                    {
                        rti90 = ((valB - valA) / (pasB - pasA) * (90.0 - pasA)) + valA;
                        rti90Found = true;
                    }
                }
                pasA = pasB;
                valA = valB;
            }

            // 防呆
            if (!rti90Found) rti90 = remainingValues.LastOrDefault();
            if (M == 0) rti90 = rti10;

            // ==========================================
            // 3. 結算 30, 50, 70
            // ==========================================
            double rti30 = ((rti90 - rti10) / 8.0 * 2.0) + rti10;
            double rti50 = ((rti90 - rti10) / 8.0 * 4.0) + rti10;
            double rti70 = ((rti90 - rti10) / 8.0 * 6.0) + rti10;

            results[0] = rti10;
            results[1] = rti30;
            results[2] = rti50;
            results[3] = rti70;
            results[4] = rti90;

            return results;
        }
    }


}

