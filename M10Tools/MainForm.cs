using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using M10.lib.model;
using M10.lib;
using HtmlAgilityPack;
using System.Net;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using CL.Data;


namespace M10Tools
{
    public partial class MainForm : BaseForm
    {
        public MainForm()
        {
            InitializeComponent();

            //載入BaseForm資料
            base.InitForm();


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


            logger.Fatal("我是致命錯誤:Fatal");

            return;


            try
            {

                string a = "";

                int.Parse(a);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "stock after:" );
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
            DateTime dt = new DateTime(2007, 1, 2);
            //DateTime dt = new DateTime(2019, 8, 2);
            //結束日期
            DateTime dtEnd = new DateTime(2019, 8, 2);

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



        private void button2_Click_1(object sender, EventArgs e)
        {
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

            //開始日期
            DateTime dt = new DateTime(2019, 8, 2);
            //結束日期
            DateTime dtEnd = new DateTime(2017, 9, 12);


            Stockhelper.GetStockAfterRushTse(dt);
            Stockhelper.GetStockAfterRushOtc(dt);



            ShowStatus("完成");





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


        private void btnImpWeatherData_Click(object sender, EventArgs e)
        {
            FileInfo[] fiList = new DirectoryInfo(@"C:\temp\Weather").GetFiles("*.txt", SearchOption.TopDirectoryOnly);

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

                foreach (DataRow dr in dt.Rows)
                {


                    WeaRainData wd = new WeaRainData();
                    wd.STID = dr["stno"].ToString();
                    wd.time = dr["yyyymmddhh"].ToString();
                    wd.PP01 = Convert.ToDecimal(dr["PP01"].ToString());
                    wd.PP01old = Convert.ToDecimal(dr["PP01"].ToString());

                    dbDapper.Insert(wd);
                    Application.DoEvents();
                }

                string sNewFullName = Path.Combine(item.DirectoryName, "OK", item.Name);

                item.MoveTo(sNewFullName);
                Application.DoEvents();
            }

        }

        /// <summary>
        /// 氣象雨量資料統計
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTransWeaRainStatic_Click(object sender, EventArgs e)
        {
            ssql = " select distinct stid from WeaRainData order by STID ";

            List<dynamic> StidList = dbDapper.Query(ssql);

            int iIndex = 0;
            foreach (var StidItem in StidList)
            {
                Application.DoEvents();
                iIndex++;
                //每個STID都要跑1987-2017的資料
                //每個STID都要跑1987-2017的資料
                for (int y = 2018; y <= 2018; y++)
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
                    if (wrdList.Count == 0)
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




                    //if (wrdList.Where(s => s.time.Substring(4, 2) == "01" && s.PP01old < 0).Count() > 24)
                    //{
                    //    dM01Sum = -99;
                    //}
                    //if (wrdList.Where(s => s.time.Substring(4, 2) == "02" && s.PP01old < 0).Count() > 24)
                    //{
                    //    dM02Sum = -99;
                    //}
                    //if (wrdList.Where(s => s.time.Substring(4, 2) == "03" && s.PP01old < 0).Count() > 24)
                    //{
                    //    dM03Sum = -99;
                    //}
                    //if (wrdList.Where(s => s.time.Substring(4, 2) == "04" && s.PP01old < 0).Count() > 24)
                    //{
                    //    dM04Sum = -99;
                    //}
                    //if (wrdList.Where(s => s.time.Substring(4, 2) == "05" && s.PP01old < 0).Count() > 24)
                    //{
                    //    dM05Sum = -99;
                    //}
                    //if (wrdList.Where(s => s.time.Substring(4, 2) == "06" && s.PP01old < 0).Count() > 24)
                    //{
                    //    dM06Sum = -99;
                    //}
                    //if (wrdList.Where(s => s.time.Substring(4, 2) == "07" && s.PP01old < 0).Count() > 24)
                    //{
                    //    dM07Sum = -99;
                    //}
                    //if (wrdList.Where(s => s.time.Substring(4, 2) == "08" && s.PP01old < 0).Count() > 24)
                    //{
                    //    dM08Sum = -99;
                    //}
                    //if (wrdList.Where(s => s.time.Substring(4, 2) == "09" && s.PP01old < 0).Count() > 24)
                    //{
                    //    dM09Sum = -99;
                    //}
                    //if (wrdList.Where(s => s.time.Substring(4, 2) == "10" && s.PP01old < 0).Count() > 24)
                    //{
                    //    dM10Sum = -99;
                    //}
                    //if (wrdList.Where(s => s.time.Substring(4, 2) == "11" && s.PP01old < 0).Count() > 24)
                    //{
                    //    dM11Sum = -99;
                    //}
                    //if (wrdList.Where(s => s.time.Substring(4, 2) == "12" && s.PP01old < 0).Count() > 24)
                    //{
                    //    dM12Sum = -99;
                    //}

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

        private void btnRainAreaSplit_Click(object sender, EventArgs e)
        {
            string sDataStaticLogType = "RainAreaSplit";

            ssql = " select distinct stid from WeaRainData order by STID ";
            List<dynamic> StidList = dbDapper.Query(ssql);

            DateTime dttest = DateTime.Now;
            int iIndex = 0;
            foreach (var StidItem in StidList)
            {
                iIndex++;
                //雨量站
                string sStid = StidItem.stid;
                //sStid = "C0A530";
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
                        if (dsl_new.status == "10" || dsl_new.status == "60") //10:執行中 60:已完成
                        {
                            continue;
                        }

                        //重新更新為執行中
                        dsl_new.status = "10";
                        dbDapper.Update(dsl_new);
                    }

                    DateTime dtStart = DateTime.ParseExact("2018010100", "yyyyMMddHH", null);
                    //dtStart = DateTime.ParseExact("2017060200", "yyyyMMddHH", null);
                    DateTime dtFinish = DateTime.ParseExact("2018123123", "yyyyMMddHH", null);
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
                        int iTime = Convert.ToInt32(i.ToString("yyyyMMddHH")) + 1;

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

                            int iTime2 = Convert.ToInt32(i.AddHours(1).ToString("yyyyMMddHH")) + 1;
                            int iTime3 = Convert.ToInt32(i.AddHours(2).ToString("yyyyMMddHH")) + 1;
                            int iTime4 = Convert.ToInt32(i.AddHours(3).ToString("yyyyMMddHH")) + 1;
                            int iTime5 = Convert.ToInt32(i.AddHours(4).ToString("yyyyMMddHH")) + 1;
                            int iTime6 = Convert.ToInt32(i.AddHours(5).ToString("yyyyMMddHH")) + 1;
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

                                sRainAreaE = Convert.ToString(Convert.ToInt32(i.AddHours(-1).ToString("yyyyMMddHH")) + 1);



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
                                wraTemp.Max3Sum = CalcRainMax(RainAreaList, 3);
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
                                wraTemp.Max6Sum = CalcRainMax(RainAreaList, 6);
                                if (wraTemp.Max6Sum == 0)
                                {
                                    wraTemp.Max6Sum = dTempSum;
                                }
                                else
                                {
                                    //dTempSum = wraTemp.Max6Sum;
                                }

                                //J：最大12，雨場範圍內，最大連續12小時累積雨量(如果最大6、12、24、48沒有雨的話，要填入最大的3小時)
                                wraTemp.Max12Sum = CalcRainMax(RainAreaList, 12);
                                if (wraTemp.Max12Sum == 0)
                                {
                                    wraTemp.Max12Sum = dTempSum;
                                }
                                else
                                {
                                    //dTempSum = wraTemp.Max12Sum;
                                }

                                //K：最大24，雨場範圍內，最大連續24小時累積雨量(如果最大6、12、24、48沒有雨的話，要填入最大的3小時)
                                wraTemp.Max24Sum = CalcRainMax(RainAreaList, 24);
                                if (wraTemp.Max24Sum == 0)
                                {
                                    wraTemp.Max24Sum = dTempSum;
                                }
                                else
                                {
                                    //dTempSum = wraTemp.Max24Sum;
                                }

                                //L：最大48，雨場範圍內，最大連續48小時累積雨量(如果最大6、12、24、48沒有雨的話，要填入最大的3小時)
                                wraTemp.Max48Sum = CalcRainMax(RainAreaList, 48);
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
                                wraTemp.Pre7DayRain6 = CalcValueP(RainList, wraTemp, 0.6);
                                //四捨五入
                                wraTemp.Pre7DayRain6 = Math.Round(wraTemp.Pre7DayRain6, 1, MidpointRounding.AwayFromZero);
                                //α=0.7
                                wraTemp.Pre7DayRain7 = CalcValueP(RainList, wraTemp, 0.7);
                                //四捨五入
                                wraTemp.Pre7DayRain7 = Math.Round(wraTemp.Pre7DayRain7, 1, MidpointRounding.AwayFromZero);
                                //α=0.8
                                wraTemp.Pre7DayRain8 = CalcValueP(RainList, wraTemp, 0.8);
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
                    logger.Error(ex, dttest.ToString("yyyyMMddHH"));

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
        private decimal CalcRainMax(List<WeaRainData> wrdList, int MaxCon)
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

        private decimal CalcValueP(List<WeaRainData> wrdList, WeaRainArea wra, double dEffData)
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

        private void button12_Click(object sender, EventArgs e)
        {
            string sDataStaticLogType = M10Const.DataStaticLogType.RainAreaToExcel;




            ssql = " select distinct stid from WeaRainData order by STID ";
            List<dynamic> StidList = dbDapper.Query(ssql);

            DateTime dttest = DateTime.Now;
            int iIndex = 0;
            foreach (var StidItem in StidList)
            {
                iIndex++;
                //雨量站
                string sStid = StidItem.stid;

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



                        List<string> cols = new List<string>();
                        cols.Add(item.stid);
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

                    System.Threading.Thread.Sleep(month);
                }
                catch (Exception)
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


            //    //***** 下載檔案過大，使用特殊方法 *****
            //    HttpContext context = System.Web.HttpContext.Current;
            //    context.Response.TransmitFile(sSaveFilePath);
            //    context.Response.ContentType = "application/vnd.ms-excel";
            //    context.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", filename));
            //    Response.End();
            //}
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string sTempPath = @"c:\temp\WeaRainArea\";
            //建立資料夾
            Directory.CreateDirectory(sTempPath);

            string sFileName = "all.xlsx";
            string sSaveFilePath = Path.Combine(sTempPath, sFileName);
            sSaveFilePath = @"c:\" + sFileName;
            FileInfo[] fiList = new DirectoryInfo(sTempPath).GetFiles("*.xlsx", SearchOption.AllDirectories);
            DataExport de = new DataExport();

            //建立表頭
            List<string> head = new List<string>();
            head.Add("雨量站編號");
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


            int i = 0;
            foreach (FileInfo item in fiList)
            {
                i++;
                ShowStatus(string.Format("({0}/{1}){2}", i.ToString(), fiList.Count().ToString(), item.Name));
                List<string[]> temp = de.ReadExcelToList(item.FullName);
                //datas.AddRange(temp);

                temp.RemoveAt(0);

                de.AppendListToExcel(sSaveFilePath, temp);


            }

            ShowStatus("完成");

            ssql = "";
        }



        private void button14_Click(object sender, EventArgs e)
        {
            string sTempPath = @"c:\temp\WeaRainArea\";
            //建立資料夾
            Directory.CreateDirectory(sTempPath);

            string sFileName = "all.xlsx";
            string sSaveFilePath = Path.Combine(sTempPath, sFileName);
            sSaveFilePath = @"c:\" + sFileName;
            FileInfo[] fiList = new DirectoryInfo(sTempPath).GetFiles("*.xlsx", SearchOption.AllDirectories);
            DataExport de = new DataExport();

            //建立表頭
            List<string> head = new List<string>();
            head.Add("雨量站編號");
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



            //string strDoc = @"C:\Users\Public\Documents\Sheet11.xlsx";
            //;
            //Stream stream = File.Open(strDoc, FileMode.Open);
            //OpenAndAddToSpreadsheetStream(stream);
            //stream.Close();


            using (Stream stream = File.Open(sSaveFilePath, FileMode.Open))
            {
                int i = 0;
                foreach (FileInfo item in fiList)
                {
                    i++;
                    ShowStatus(string.Format("({0}/{1}){2}", i.ToString(), fiList.Count().ToString(), item.Name));
                    List<string[]> temp = de.ReadExcelToList(item.FullName);
                    //datas.AddRange(temp);

                    temp.RemoveAt(0);

                    DataExport.OpenAndAddToSpreadsheetStream(stream, temp);


                    //de.AppendListToExcel(sSaveFilePath, temp);
                }

            }

            ShowStatus("完成");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            /*    ====執行前請先將新版的雨場分割資料匯入RtiData2中====         
             *    WeaRainArea直接匯入 RtiData2        
             *    
                INSERT INTO [dbo].[RtiData2]
                ([station],[ver],[raindelay],[Rtd6],[Rtd7],[Rtd8],[Rti6],[Rti7]
                ,[Rti8],[Rti36],[Rti37],[Rti38],[R3],[date])

                select STID,'20190515' as ver,RainHour,rt6,rt7,RT8
                ,RT6*MaxRain as rti6,RT7*MaxRain as rti7,RT8*MaxRain as rti8
                ,RT6*Max3Sum/3 as rti36,RT7*Max3Sum/3 as rti37,RT8*Max3Sum/3 as rti38
                ,max3sum,SUBSTRING(TimeStart,1,8) as date 
                from WeaRainArea 
            */


            string sVer = "20190515";
            List<RtiDetail> rdList = new List<RtiDetail>();

            ssql = " select distinct stid from WeaRainArea order by STID ";
            List<dynamic> StidList = dbDapper.Query(ssql);

            DateTime dttest = DateTime.Now;
            int iIndex = 0;
            foreach (var StidItem in StidList)
            {
                iIndex++;
                string sStid = StidItem.stid;

                string sDALConnStr = "M10VPN";
                string[] typeList = { "RTI", "RTI3" };
                string[] DelaytimeList = { "0", "1", "2", "3" };
                string[] CoefficientList = { "6", "7", "8" };


                //foreach (string itemtype in typeList)
                //{ }
                foreach (string itemDelaytime in DelaytimeList)
                {
                    foreach (string itemCoefficient in CoefficientList)
                    {
                        ShowStatus(string.Format("[{4}/{5}][{0} {3}]Delaytime:{1} Coefficient:{2} ", sStid, itemDelaytime, itemCoefficient, "RTI", iIndex.ToString(), StidList.Count.ToString()));
                        //todo 延時及係數迴圈
                        RtiDetail rd = new RtiDetail();

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

                        RtiProc oRtiProc = new RtiProc(sVer, sStid, itemDelaytime, itemCoefficient, "RTI", sDALConnStr);

                        //進行計算
                        oRtiProc.RtiCal();

                        rd.station = sStid;
                        rd.delaytime = itemDelaytime;
                        rd.coefficient = itemCoefficient;
                        rd.version = "temp";

                        rd.rti10 = oRtiProc.dRTI10;
                        rd.rti30 = oRtiProc.dRTI30;
                        rd.rti50 = oRtiProc.dRTI50;
                        rd.rti70 = oRtiProc.dRTI70;
                        rd.rti90 = oRtiProc.dRTI90;

                        dbDapper.Insert(rd);

                    }
                }

                foreach (string itemDelaytime in DelaytimeList)
                {
                    foreach (string itemCoefficient in CoefficientList)
                    {
                        ShowStatus(string.Format("[{0} {3}]Delaytime:{1} Coefficient:{2} ", sStid, itemDelaytime, itemCoefficient, "RTI3"));
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

                        RtiProc oRtiProc = new RtiProc(sVer, sStid, itemDelaytime, itemCoefficient, "RTI3", sDALConnStr);

                        //進行計算
                        oRtiProc.Rti3Cal();

                        rd.station = sStid;
                        rd.delaytime = itemDelaytime;
                        rd.coefficient = itemCoefficient;
                        rd.version = "temp";

                        rd.rti10 = oRtiProc.dRTI10;
                        rd.rti30 = oRtiProc.dRTI30;
                        rd.rti50 = oRtiProc.dRTI50;
                        rd.rti70 = oRtiProc.dRTI70;
                        rd.rti90 = oRtiProc.dRTI90;

                        rd.rt10 = oRtiProc.dRT10;
                        rd.rt30 = oRtiProc.dRT30;
                        rd.rt50 = oRtiProc.dRT50;
                        rd.rt70 = oRtiProc.dRT70;
                        rd.rt90 = oRtiProc.dRT90;

                        rd.r310 = oRtiProc.dR310;
                        rd.r330 = oRtiProc.dR330;
                        rd.r350 = oRtiProc.dR350;
                        rd.r370 = oRtiProc.dR370;
                        rd.r390 = oRtiProc.dR390;

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


            Stockhelper.getThreeFuturesDailyData(DateTime.Now);
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



        //private void buildtableRTI()
        //{
        //    dt_rti.Columns.Add("index");
        //    dt_rti.Columns.Add("station");
        //    dt_rti.Columns.Add("rti");
        //    dt_rti.Columns.Add("totalcount");
        //    dt_rti.Columns.Add("pas");

        //    dt_rti90.Columns.Add("index");
        //    dt_rti90.Columns.Add("station");
        //    dt_rti90.Columns.Add("rti");
        //    dt_rti90.Columns.Add("totalcount");
        //    dt_rti90.Columns.Add("pas");
        //}
    }

    public class RtiProc
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



        public RtiProc(string ver, string station, string delaytime, string Coefficient, string type, string sConnString)
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
}
