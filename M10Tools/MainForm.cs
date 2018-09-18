﻿using System;
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

        private void button5_Click(object sender, EventArgs e)
        {
            //開始日期
            DateTime dt = new DateTime(2017, 9, 8);
            //結束日期
            DateTime dtEnd = new DateTime(2017, 9, 12);

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
                    toolStripStatusLabel1.Text = string.Format("{0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    if (Stockhelper.GetStockAfterTse(LoopDatetime) == false)
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



        private void button6_Click(object sender, EventArgs e)
        {
            //開始日期
            DateTime dt = new DateTime(2017, 9, 8);
            //結束日期
            DateTime dtEnd = new DateTime(2017, 9, 12);

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
                    toolStripStatusLabel1.Text = string.Format("{0}-{1}", M10Const.StockType.otc, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    if (Stockhelper.GetStockAfterOtc(LoopDatetime) == false)
                    {
                        System.Threading.Thread.Sleep(3000);
                        continue;
                    }

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

            //Stockhelper.GetStockInfo();
            Stockhelper.GetStockInfoSub(M10Const.StockType.otc1);



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

            ////Creates the client
            //var client = new RestClient("http://example.com");
            //client.CookieContainer = new System.Net.CookieContainer();
            ////client.Authenticator = new SimpleAuthenticator("username", "xxx", "password", "xxx");

            ////Creates the request
            //var request = new RestRequest("/login", Method.POST);

            ////Executes the login request
            //var response = client.Execute(request);

            ////This executes a seperate request, hence creating a new requestion
            //client.DownloadData(new RestRequest("/file", Method.GET)).SaveAs("example.csv");

            //Console.WriteLine(response.Content);
            //Stockhelper.GetStockBrokerBS();

            var client = new RestClient("http://www.tpex.org.tw/web/stock/aftertrading/broker_trading/download_ALLCSV.php");
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "331fb74b-2e3d-bfe6-fcc3-eeb3eb21df64");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("Origin", "http://www.tpex.org.tw");
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("Referer", "http://www.tpex.org.tw/web/stock/aftertrading/broker_trading/brokerBS.php?l=zh-tw");
            request.AddHeader("Accept-Language", "zh-TW,zh;q=0.9,en-US;q=0.8,en;q=0.7");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36");

            request.AddParameter("application/x-www-form-urlencoded", "stk_code=6180", ParameterType.RequestBody);



            //byte[] byteArray = client.DownloadData(request);



            //string result = System.Text.Encoding.UTF8.GetString(byteArray);

            IRestResponse response = client.Execute(request);

            string aaaa = response.Content;


            //Stockhelper.GetStockBrokerBStest();
            //Stockhelper.GetStockBrokerBSTSE();
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



            string ssss = "";



            //StockInfo st = new StockInfo();



            //Boolean bResult = false;


            //StockRuntime sr = Stockhelper.getStockRealtimeYahooApi(StockCodeTextBox.Text);

            ////u = up 
            ////w = down

            //Decimal dup =  Stockhelper.getPriceLimitUpOrDown("up", sr.y);
            //Decimal ddown = Stockhelper.getPriceLimitUpOrDown("down", sr.y);


            //string ssss = string.Empty;

            //if (sr.u == dup.ToString() && sr.w == ddown.ToString())
            //{
            //  bResult = true;
            //}



            //label1.Text = string.Format("{0}={1}", StockCodeTextBox.Text, bResult.ToString());

            //StockCodeTextBox.Text = "";

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
            DateTime dt = new DateTime(2018, 5, 4);
            //結束日期
            DateTime dtEnd = new DateTime(2017, 9, 12);


            Stockhelper.GetStockAfterRushTse(dt);
            Stockhelper.GetStockAfterRushOtc(dt);



            ShowStatus("完成");





            return;


            //for (DateTime date = checkBgn; date <= checkEnd; date = date.AddDays(1))
            for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
            {
                string sLineTrans = "";
                try
                {
                    toolStripStatusLabel1.Text = string.Format("{0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
                    Application.DoEvents();

                    if (Stockhelper.GetStockAfterTse(LoopDatetime) == false)
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




        private void ShowStatus(string sMsg)
        {
            toolStripStatusLabel1.Text = sMsg;
            Application.DoEvents();
        }

        private void btnTransToPrice_Click(object sender, EventArgs e)
        {
            //取得所有日期清單
            ssql = " select distinct stockdate from StockAfter order by stockdate ";
            List<dynamic> DateList = dbDapper.Query(ssql);

            foreach (var item in DateList)
            {
                //依照日期取得StockAfter資料
                ssql = " select * from stockafter where stockdate = '{0}' and updYN <> 'Y' ";
                ssql = string.Format(ssql, Convert.ToString(item.stockdate));
                List<Stockafter> StockAfterList = dbDapper.Query<Stockafter>(ssql);

                foreach (Stockafter StockAferItem in StockAfterList)
                {
                    //判斷資料是否存在
                    ssql = " select * from Price1 where date = '{0}' and StockID = '{1}' ";
                    ssql = string.Format(ssql, StockAferItem.stockdate, StockAferItem.stockcode);
                    if (dbDapper.QueryTotalCount(ssql) != 0) continue;

                    Price1 oPrice1 = new Price1();
                    oPrice1.UID = Guid.NewGuid().ToString().ToUpper();
                    oPrice1.StockID = StockAferItem.stockcode;
                    oPrice1.Date = DateTime.ParseExact(StockAferItem.stockdate, "yyyyMMdd", null);
                    oPrice1.Open = StockAferItem.priceopen;
                    oPrice1.Close = StockAferItem.pricelast;
                    oPrice1.Low = StockAferItem.pricelow;
                    oPrice1.High = StockAferItem.pricetop;
                    oPrice1.Turnover = StockAferItem.dealmoney;
                    oPrice1.Capacity = StockAferItem.dealnum;
                    oPrice1.Transaction = StockAferItem.dealamount;
                    string aaa = StockAferItem.updown == "X" ? "" : StockAferItem.updown;
                    string sss = aaa + StockAferItem.pricediff;
                    oPrice1.Change = Convert.ToDecimal(sss);

                    dbDapper.Insert<Price1>(oPrice1);

                    ShowStatus(string.Format("{0}-{1}", item.stockdate, StockAferItem.stockcode));

                    //註記已轉檔
                    StockAferItem.updYN = "Y";
                    dbDapper.Update<Stockafter>(StockAferItem);
                }
            }

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

                    dbDapper.Insert(wd);
                }

                string sNewFullName = Path.Combine(item.DirectoryName, "OK", item.Name);

                item.MoveTo(sNewFullName);

            }
            //}
            //catch (Exception ex)
            //{
            //    string sss = ex.ToString();
            //}






            string ss = "";


            //List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

            //foreach (string LoopItem in StringList)
            //{
            //    string Line = LoopItem;
            //    Line = Line.Replace(" ", "");
            //    Line = Line.Replace("\",\"", "|");
            //    Line = Line.Replace("\"", "");
            //    Line = Line.Replace(",", "");
            //    Line = Line.Replace("=", "");
            //    sLineTrans = Line;
            //    string[] aCol = Line.Split('|');

            //    if (aCol.Length == 16)
            //    {
            //        //檢核資料
            //        Decimal iCheck = -1;

            //        if (Decimal.TryParse(aCol[8], out iCheck) == false)
            //        {
            //            continue;
            //        }

            //        ssql = " select * from stockafter  where stockdate = '{0}' and stockcode = '{1}'  ";
            //        Stockafter sa = dbDapper.QuerySingleOrDefault<Stockafter>(string.Format(ssql, sDate, aCol[0]));

            //        decimal dpricelastbuy = 0;
            //        decimal.TryParse(aCol[11], out dpricelastbuy);

            //        decimal dpricelastsell = 0;
            //        decimal.TryParse(aCol[13], out dpricelastsell);

            //        if (aCol[9] == "") aCol[9] = "X";

            //        //計算昨收
            //        Decimal dPriceYesterday = CalcPriceYesterday(aCol[8], aCol[9], aCol[10]);

            //        if (sa == null)
            //        {
            //            sa = new Stockafter();
            //            sa.stockdate = sDate;
            //            sa.stocktype = M10Const.StockType.tse;
            //            sa.stockcode = aCol[0];
            //            sa.pricelast = Convert.ToDecimal(aCol[8]);
            //            sa.updown = aCol[9];
            //            sa.pricediff = aCol[10];
            //            sa.priceopen = Convert.ToDecimal(aCol[5]);
            //            sa.pricetop = Convert.ToDecimal(aCol[6]);
            //            sa.pricelow = Convert.ToDecimal(aCol[7]);
            //            sa.priceavg = 0;
            //            sa.dealnum = Convert.ToInt64(aCol[2]);
            //            sa.dealmoney = Convert.ToInt64(aCol[4]);
            //            sa.dealamount = Convert.ToInt64(aCol[3]);
            //            sa.pricelastbuy = dpricelastbuy;
            //            sa.pricelastsell = dpricelastsell;
            //            sa.publicnum = 0;
            //            sa.pricenextday = Convert.ToDecimal(aCol[8]);
            //            sa.pricenextlimittop = 0;
            //            sa.pricenextlimitlow = 0;
            //            sa.priceyesterday = dPriceYesterday;
            //            sa.updatetime = Utils.getDatatimeString();

            //            dbDapper.Insert(sa);
            //        }
            //        else
            //        {
            //            sa.stocktype = M10Const.StockType.tse;
            //            sa.pricelast = Convert.ToDecimal(aCol[8]);
            //            sa.updown = aCol[9];
            //            sa.pricediff = aCol[10];
            //            sa.priceopen = Convert.ToDecimal(aCol[5]);
            //            sa.pricetop = Convert.ToDecimal(aCol[6]);
            //            sa.pricelow = Convert.ToDecimal(aCol[7]);
            //            sa.priceavg = 0;
            //            sa.dealnum = Convert.ToInt64(aCol[2]);
            //            sa.dealmoney = Convert.ToInt64(aCol[4]);
            //            sa.dealamount = Convert.ToInt64(aCol[3]);
            //            sa.pricelastbuy = dpricelastbuy;
            //            sa.pricelastsell = dpricelastsell;
            //            sa.publicnum = 0;
            //            sa.pricenextday = Convert.ToDecimal(aCol[8]);
            //            sa.pricenextlimittop = 0;
            //            sa.pricenextlimitlow = 0;
            //            sa.updatetime = Utils.getDatatimeString();
            //            sa.priceyesterday = dPriceYesterday;
            //            dbDapper.Update(sa);
            //        }
            //    }

            //}




        }

        private void btnTransWeaRainStatic_Click(object sender, EventArgs e)
        {
            WeaRainStatistics wrs = new WeaRainStatistics();

            
            string sStid = "466900";

            DateTime dtStart = DateTime.ParseExact("19870101", "yyyyMMdd", null);
            DateTime dtFinish = DateTime.ParseExact("19871231", "yyyyMMdd", null);

            wrs.stid = sStid;
            wrs.year = dtStart.Year.ToString();

            ssql = @"
                select * from WeaRainData where STID =  '{0}' and time between '1987000000' and '1987123200'
                ";

            ssql = string.Format(ssql, sStid);

            List<WeaRainData> wrdList = dbDapper.Query<WeaRainData>(ssql);           

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
            Decimal dMonAvg = dMonAvgSum / iMonAvgCount;
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
            WeaRainData wwww =  WeaRainDayData.Where(s => s.PP01 == dMax1).ToList<WeaRainData>()[0];
            wrs.max1 = dMax1;
            wrs.max1date = wwww.time;

            //最大二日雨量：一年中連續二日雨量之最大值
            Decimal dMax2 = 0;
            string sMax2Date = "";
            for (int i = 0; i < WeaRainDayData.Count -1; i++)
            {
                if (i+1 <= WeaRainDayData.Count -1)
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
            WeaRainStatistics Temp  = dbDapper.QuerySingleOrDefault<WeaRainStatistics>(ssql);
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
            }
        }
    }
}
