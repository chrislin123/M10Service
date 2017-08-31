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



            StatusLabel.Text =
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

      string sType = "";

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
      //return Math.Floor(diff.TotalSeconds);
      return Math.Floor(diff.TotalMilliseconds);
    }
    private void button1_Click(object sender, EventArgs e)
    {
      string Url = "http://{0}/stock/api/getStockInfo.jsp?ex_ch=tse_t00.tw%7cotc_o00.tw%7ctse_FRMSA.tw&json=1&delay=0&_={1}";
      double dd = ConvertToUnixTimestamp(DateTime.Now.AddMilliseconds(60000));


      string MISTWSE = "mis.twse.com.tw";
      string host = "61.57.47.179";
      string USER_AGENT = "curl/7.32.0";

      Url = string.Format(Url, host, dd);

      //HttpClient client = new HttpClient();
      //client.BaseAddress = new Uri("http://mis.twse.com.tw/");
      ////client.DefaultRequestHeaders
      ////      .Accept
      ////      .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
      //client.DefaultRequestHeaders.Host = MISTWSE;
      ////hc.DefaultRequestHeaders.UserAgent = USER_AGENT;
      ////hc.DefaultRequestHeaders.Add("Host", MISTWSE);
      ////hc.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
      //client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
      ////client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
      //client.DefaultRequestHeaders.Add("Accept-Language", "zh-TW,zh;q=0.8,en-US;q=0.5,en;q=0.3");
      //client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
      //client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
      ////client.DefaultRequestHeaders.Add("Referer", string.Format("http://{0}/stock/fibest.jsp?lang=zh_tw", MISTWSE));
      //client.DefaultRequestHeaders.Referrer = new Uri(string.Format("http://{0}/stock/fibest.jsp?lang=zh_tw", host));
      //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Url);
      ////request.Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}",
      ////                                    Encoding.UTF8,
      ////                                    "application/json");//CONTENT-TYPE header

      //client.SendAsync(request)
      //      .ContinueWith(responseTask =>
      //      {
      //        Console.WriteLine("Response: {0}", responseTask.Result);
      //      });



      string ssss = "";

      //var client = new HttpClient();
      //var request = new HttpRequestMessage()
      //{
      //  RequestUri = new Uri("http://www.someURI.com"),
      //  Method = HttpMethod.Get,
      //};
      //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
      //var task = client.SendAsync(request)
      //    .ContinueWith((taskwithmsg) =>
      //    {
      //      HttpResponseMessage response = taskwithmsg.Result;
      //       ssss = response.Content.ToString();
      //      //string ssss = response.Content
      //      //var jsonTask = response.Content.ReadAsAsync<JsonObject>();
      //      //jsonTask.Wait();
      //      //var jsonObject = jsonTask.Result;
      //    });
      //task.Wait();





      //string url = "http://140.116.38.211/C10Mvc/StockApi/getStockType?StockCode=00672L";







      string url2 = "http://{0}/stock/api/getStockInfo.jsp?ex_ch=otc_4506.tw&json=1&delay=0&_={1}";
      double dd2 = ConvertToUnixTimestamp(DateTime.Now.AddMilliseconds(60000));

      Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
      //string mistwse = "mis.twse.com.tw";
      //string host = "61.57.47.179";
      //string user_agent = "curl/7.32.0";


      DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      DateTime utcNow = DateTime.UtcNow;

      TimeSpan elapsedTime = utcNow - unixEpoch;
      //elapsedTime.TotalMilliseconds.
      Int32 millis = (Int32)elapsedTime.TotalMilliseconds;

      url2 = string.Format(url2, host, dd2);


      //client.Headers.Add("Host", MISTWSE);
      //client.Headers.Add("User-Agent", USER_AGENT);
      //client.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
      //client.Headers.Add("Accept-Language", "zh-TW,zh;q=0.8,en-US;q=0.5,en;q=0.3");
      //client.Headers.Add("Accept-Encoding", "gzip, deflate");
      //client.Headers.Add("X-Requested-With", "XMLHttpRequest");
      //client.Headers.Add("Upgrade-Insecure-Requests", "1");


      //string reply2 = client.DownloadString(url2);
      ////KeyValuePair<string, string> headers = (KeyValuePair<string, string>)client.ResponseHeaders.;



      HttpWebClient hwc = new HttpWebClient();
      hwc.Encoding = Encoding.UTF8;
      hwc.Headers.Add("Host", host);
      //hwc.Headers.Add("User-Agent", USER_AGENT);
      //hwc.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
      //hwc.Headers.Add("Accept-Language", "zh-TW,zh;q=0.8,en-US;q=0.5,en;q=0.3");
      //hwc.Headers.Add("Accept-Encoding", "gzip, deflate");
      //hwc.Headers.Add("Upgrade-Insecure-Requests", "1");
      //hwc.Headers.Add("Referer", string.Format("http://{0}/stock/fibest.jsp?lang=zh_tw", host));

      //client.Encoding = Encoding.UTF8;
      string url3 = "http://{0}/stock/api/getStock.jsp?ch=1475.tw&json=0";
      url3 = string.Format(url3, host);


      string url4 = "http://{0}/stock/fibest.jsp?stock=4506";
      url4 = string.Format(url4, host);

      //string reply = hwc.DownloadString(url4);

      //Set - Cookie

      //string sCookie = hwc.ResponseHeaders["Set-Cookie"];
      //sCookie = sCookie.Substring(0, sCookie.IndexOf(';') );



      //GetAllCookies(hwc.MyCookies);

      //foreach (Cookie cookieValue in hwc.MyCookies)
      //{
      //  sw.WriteLine("Cookie: " + cookieValue.ToString());
      //}


      hwc.Headers.Add("Host", host);
      //hwc.Headers.Add("User-Agent", USER_AGENT);
      //hwc.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
      //hwc.Headers.Add("Accept-Language", "zh-TW,zh;q=0.8,en-US;q=0.5,en;q=0.3");
      //hwc.Headers.Add("Accept-Encoding", "gzip, deflate");
      //hwc.Headers.Add("X-Requested-With", "XMLHttpRequest");
      //hwc.Headers.Add("Upgrade-Insecure-Requests", "1");
      //hwc.Headers.Add("Referer", string.Format("http://{0}/stock/fibest.jsp?stock=4506", MISTWSE));
      hwc.Headers.Add("Referer", url4);
      //hwc.Headers.Add("Cookie", sCookie);
      //string reply2 = hwc.DownloadString(url2);




      string bbbb = "";


      //ExeCommand();


      HttpClient hc = new HttpClient();

      hc.DefaultRequestHeaders.Host = MISTWSE;
      //hc.DefaultRequestHeaders.UserAgent = USER_AGENT;
      //hc.DefaultRequestHeaders.Add("Host", MISTWSE);
      //hc.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
      hc.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
      hc.DefaultRequestHeaders.Add("Accept-Language", "zh-TW,zh;q=0.8,en-US;q=0.5,en;q=0.3");
      hc.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
      hc.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
      hc.DefaultRequestHeaders.Add("Referer", string.Format("http://{0}/stock/fibest.jsp?lang=zh_tw", MISTWSE));
      hc.DefaultRequestHeaders.Add("Cookie", "1");




      //string content = hc.GetStringAsync(Url).Result;


      //string content2 = hc.GetStringAsync(url2).Result;





      //string Url = "http://{0}/stock/api/getStockInfo.jsp?ex_ch=tse_t00.tw%7cotc_o00.tw%7ctse_FRMSA.tw&json=1&delay=0&_={1}";
      //double dd = ConvertToUnixTimestamp(DateTime.Now);


      //string MISTWSE = "mis.twse.com.tw";
      //string host = "61.57.47.179";
      //string USER_AGENT = "curl/7.32.0";

      //Url = string.Format(Url, host, dd);

      CookieContainer cc = new CookieContainer();

      string Cookiesstr = string.Empty;
      HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url4);
      myRequest.CookieContainer = cc;
      myRequest.AllowAutoRedirect = false;

      myRequest.Host = host;
      //myRequest.UserAgent = USER_AGENT;
      //myRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
      //myRequest.Referer = string.Format("http://{0}/stock/fibest.jsp?lang=zh_tw", host);
      //myRequest.ContentType = "application/json; charset=utf-8";
      //myRequest.Headers.Add("Host", MISTWSE);
      //myRequest.Headers.Add("User-Agent", USER_AGENT);
      //myRequest.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
      //myRequest.Headers.Add("Accept-Language", "zh-TW,zh;q=0.8,en-US;q=0.5,en;q=0.3");
      //myRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
      //myRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
      //myRequest.Headers.Add("Referer", string.Format("http://%s/stock/fibest.jsp?lang=zh_tw", host));

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

      //HttpWebRequest myRequest2 = (HttpWebRequest)WebRequest.Create(url2);

      myRequest.Host = host;
      //hwc.Headers.Add("User-Agent", USER_AGENT);
      myRequest.Accept = "application/json, text/javascript, */*; q=0.01";
      myRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.101 Safari/537.36";
      //myRequest2.ContentType = "application/x-www-form-urlencoded";
      //myRequest2.Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
      //hwc.Headers.Add("Accept-Language", "zh-TW,zh;q=0.8,en-US;q=0.5,en;q=0.3");
      //hwc.Headers.Add("Accept-Encoding", "gzip, deflate");
      //hwc.Headers.Add("X-Requested-With", "XMLHttpRequest");
      //hwc.Headers.Add("Upgrade-Insecure-Requests", "1");
      //hwc.Headers.Add("Referer", string.Format("http://{0}/stock/fibest.jsp?stock=4506", MISTWSE));
      myRequest.Referer = url4;


      //myRequest2.Headers.Add("Accept-Language", "zh-TW");
      //myRequest2.Headers.Add("Referer", url4);
      //myRequest2.Headers.Add("Cookie", sCookie);
      WebResponse myResponse2 = myRequest.GetResponse();
      using (StreamReader sr = new StreamReader(myResponse2.GetResponseStream(), System.Text.Encoding.Default))
      {
        string result = sr.ReadToEnd();


        myResponse2.Close();
      }



    }

    //public static CookieCollection GetAllCookies(this CookieContainer container)
    //{
    //  var allCookies = new CookieCollection();
    //  var domainTableField = container.GetType().GetRuntimeFields().FirstOrDefault(x => x.Name == "m_domainTable");
    //  var domains = (IDictionary)domainTableField.GetValue(container);

    //  foreach (var val in domains.Values)
    //  {
    //    var type = val.GetType().GetRuntimeFields().First(x => x.Name == "m_list");
    //    var values = (IDictionary)type.GetValue(val);
    //    foreach (CookieCollection cookies in values.Values)
    //    {
    //      allCookies.Add(cookies);
    //    }
    //  }
    //  return allCookies;
    //}



    //private static string[] ExeCommand(string[] pCommandText1, string[] pCommandText2)
    private static string[] ExeCommand()
    {
      Process p = new Process();

      p.StartInfo.FileName = "cmd.exe";
      p.StartInfo.UseShellExecute = false;
      p.StartInfo.RedirectStandardInput = true;
      p.StartInfo.RedirectStandardOutput = true;
      p.StartInfo.RedirectStandardError = true;
      p.StartInfo.CreateNoWindow = false;
      //string temp = null;
      string[] strOutput = new string[2];

      try
      {
        p.Start();
        //p.StandardInput.WriteLine(pCommandText2[1]);
        //java - jar StockBest5.jar 
        //p.StandardInput.AutoFlush = true;
        p.StandardInput.WriteLine(@"java - jar C:\StockBest5.jar 3019");
        //p.StandardInput.WriteLine(pCommandText1[0] + " " + pCommandText1[1]
        //+ " " + pCommandText1[2] + " " + pCommandText1[3]
        //+ " " + pCommandText1[4]);
        //strOutput[0] = p.StandardOutput.ReadLine();
        //strOutput[0] = p.StandardOutput.ReadLine();
        //strOutput[0] = p.StandardOutput.ReadLine();
        //strOutput[0] = p.StandardOutput.ReadLine();
        //strOutput[0] = p.StandardOutput.ReadLine();
        //strOutput[0] = p.StandardOutput.ReadLine();
        ////strOutput[0]+=p.StandardOutput.ReadLine();
        p.StandardInput.WriteLine("exit");
        strOutput[0] = p.StandardOutput.ReadLine();
        strOutput[1] = p.StandardOutput.ReadToEnd();

        p.WaitForExit();


        //p.StandardInput.WriteLine("exit");




      }
      catch (Exception e)
      {
        strOutput[0] = e.Message;
        strOutput[1] = e.Message;
      }
      finally
      {

        p.Close();
      }
      return strOutput;
    }

    #region ExecPlateform,接收輸入的Command
    //private string[] ExecPlateform(string[] pCmds, string[] pEnv)
    //{
    //  string[] tExeValue = new string[2];
    //  try
    //  {
    //    //ExeCommand(pCmds, pEnv);	
    //    tExeValue = ExeCommand(pCmds, pEnv);
    //  }
    //  catch (Exception)
    //  {
    //    throw new Exception("File " + pCmds[2] + " not found");
    //  }

    //  return tExeValue;
    //}
    #endregion

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
        string sCode = "";


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
      try
      {

        DateTime dt = new DateTime(2014, 11, 21);
        DateTime dttarget = new DateTime(2017, 1, 1);
        DateTime dtnew = dt.AddDays(1);

        while (true)
        {
          if (dt.ToString("yyyyMMdd") == dttarget.ToString("yyyyMMdd")) break;

          string sUrl = "http://www.tse.com.tw/fund/T86?response=csv&date={0}&selectType=ALLBUT0999";
          string sDate = DateTime.Now.ToString("yyyyMMdd");
          sDate = dt.ToString("yyyyMMdd");

          //System.Threading.Thread.Sleep(500);
          StatusLabel.Text = string.Format("{0}進度({1})", sDate, "");
          Application.DoEvents();

          HttpWebRequest req = (HttpWebRequest)WebRequest.Create(string.Format(sUrl, sDate));
          HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
          //判斷http回應狀態(HttpStatusCode.OK=200)
          if (resp.StatusCode != HttpStatusCode.OK)
          {
            StatusLabel.Text = string.Format("{0}進度({1})", sDate, "連線異常，資料重新取得");
            Application.DoEvents();

            System.Threading.Thread.Sleep(3000);
            continue;
          }
          using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.GetEncoding(950)))
          {
            string Line;
            while ((Line = SR.ReadLine()) != null)
            {
              Line = Line.Replace(" ", "");
              Line = Line.Replace("\",\"", "|");
              Line = Line.Replace("\"", "");
              Line = Line.Replace(",", "");
              Line = Line.Replace("=", "");

              string[] aCol = Line.Split('|');

              if (aCol.Length == 16)
              {
                //檢核資料
                int iCheck = -1;

                if (int.TryParse(aCol[15], out iCheck) == false)
                {
                  continue;
                }

                ssql = " select * from Stockthreetrade where date = '{0}' and stockcode = '{1}' ";
                Stockthreetrade st = dbDapper.QuerySingleOrDefault<Stockthreetrade>(string.Format(ssql, sDate, aCol[0]));

                if (st == null)
                {
                  st = new Stockthreetrade();
                  st.stockcode = aCol[0];
                  st.date = sDate;
                  st.type = "tse";
                  st.foreigninv = Convert.ToInt32(aCol[4]);
                  st.trustinv = Convert.ToInt32(aCol[7]);
                  st.selfempinv = Convert.ToInt32(aCol[14]);
                  st.threeinv = Convert.ToInt32(aCol[15]);
                  st.updatetime = Utils.getDatatimeString();
                  dbDapper.Insert(st);
                }
                else
                {
                  st.stockcode = aCol[0];
                  st.date = sDate;
                  st.type = "tse";
                  st.foreigninv = Convert.ToInt32(aCol[4]);
                  st.trustinv = Convert.ToInt32(aCol[7]);
                  st.selfempinv = Convert.ToInt32(aCol[14]);
                  st.threeinv = Convert.ToInt32(aCol[15]);
                  st.updatetime = Utils.getDatatimeString();
                  dbDapper.Update(st);
                }


                StatusLabel.Text = string.Format("{0}進度({1})", sDate, aCol[0]);


                Application.DoEvents();
              }

              if (aCol.Length == 12)
              {
                //檢核資料
                int iCheck = -1;

                if (int.TryParse(aCol[11], out iCheck) == false)
                {
                  continue;
                }

                ssql = " select * from Stockthreetrade where date = '{0}' and stockcode = '{1}' ";
                Stockthreetrade st = dbDapper.QuerySingleOrDefault<Stockthreetrade>(string.Format(ssql, sDate, aCol[0]));

                if (st == null)
                {
                  st = new Stockthreetrade();
                  st.stockcode = aCol[0];
                  st.date = sDate;
                  st.type = "tse";
                  st.foreigninv = Convert.ToInt32(aCol[4]);
                  st.trustinv = Convert.ToInt32(aCol[7]);
                  st.selfempinv = Convert.ToInt32(aCol[10]);
                  st.threeinv = Convert.ToInt32(aCol[11]);
                  st.updatetime = Utils.getDatatimeString();
                  dbDapper.Insert(st);
                }
                else
                {
                  st.foreigninv = Convert.ToInt32(aCol[4]);
                  st.trustinv = Convert.ToInt32(aCol[7]);
                  st.selfempinv = Convert.ToInt32(aCol[10]);
                  st.threeinv = Convert.ToInt32(aCol[11]);
                  st.updatetime = Utils.getDatatimeString();
                  dbDapper.Update(st);
                }


                StatusLabel.Text = string.Format("{0}進度({1})", sDate, aCol[0]);


                Application.DoEvents();
              }
            }
          }


          dt = dt.AddDays(1);
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        throw ex;
      }
    }

    private void button4_Click(object sender, EventArgs e)
    {
      try
      {
        DateTime dt = new DateTime(2014, 12, 1);
        DateTime dttarget = new DateTime(2017, 8, 27);
        DateTime dtnew = dt.AddDays(1);

        while (true)
        {
          if (dt.ToString("yyyyMMdd") == dttarget.ToString("yyyyMMdd")) break;


          //http://www.tpex.org.tw/web/stock/3insti/daily_trade/3itrade_hedge_download.php?l=zh-tw&se=EW&t=D&d=106/08/25&s=0,asc
          string sUrl = "http://www.tpex.org.tw/web/stock/3insti/daily_trade/3itrade_hedge_download.php?l=zh-tw&se=EW&t=D&d={0}&s=0,asc";
          string sDate = DateTime.Now.ToString("yyyyMMdd");
          int iyear = dt.Year - 1911;
          sDate = string.Format("{0}/{1}/{2}", iyear.ToString(), dt.ToString("MM"), dt.ToString("dd"));

          //sDate = dt.ToString("yyyyMMdd");

          //System.Threading.Thread.Sleep(500);
          StatusLabel.Text = string.Format("{0}進度({1})", sDate, "");
          Application.DoEvents();


          HttpWebRequest req = (HttpWebRequest)WebRequest.Create(string.Format(sUrl, sDate));

          //改為寫入資料庫格式
          sDate = dt.ToString("yyyyMMdd");

          HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

          //判斷http回應狀態(HttpStatusCode.OK=200)
          if (resp.StatusCode != HttpStatusCode.OK)
          {
            StatusLabel.Text = string.Format("{0}進度({1})", sDate, "連線異常，資料重新取得");
            Application.DoEvents();

            System.Threading.Thread.Sleep(3000);
            continue;
          }

          using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.GetEncoding(950)))
          {
            string Line;
            while ((Line = SR.ReadLine()) != null)
            {
              Line = Line.Replace(" ", "");
              Line = Line.Replace("\",\"", "|");
              Line = Line.Replace("\"", "");
              Line = Line.Replace(",", "");
              Line = Line.Replace("=", "");

              string[] aCol = Line.Split('|');

              if (aCol.Length == 16)
              {
                //檢核資料
                int iCheck = -1;

                if (int.TryParse(aCol[15], out iCheck) == false)
                {
                  continue;
                }

                ssql = " select * from Stockthreetrade where date = '{0}' and stockcode = '{1}' ";
                Stockthreetrade st = dbDapper.QuerySingleOrDefault<Stockthreetrade>(string.Format(ssql, sDate, aCol[0]));

                if (st == null)
                {
                  st = new Stockthreetrade();
                  st.stockcode = aCol[0];
                  st.date = sDate;
                  st.type = "otc";
                  st.foreigninv = Convert.ToInt32(aCol[4]);
                  st.trustinv = Convert.ToInt32(aCol[7]);
                  st.selfempinv = Convert.ToInt32(aCol[14]);
                  st.threeinv = Convert.ToInt32(aCol[15]);
                  st.updatetime = Utils.getDatatimeString();
                  dbDapper.Insert(st);
                }
                else
                {
                  st.foreigninv = Convert.ToInt32(aCol[4]);
                  st.trustinv = Convert.ToInt32(aCol[7]);
                  st.selfempinv = Convert.ToInt32(aCol[14]);
                  st.threeinv = Convert.ToInt32(aCol[15]);
                  st.updatetime = Utils.getDatatimeString();
                  dbDapper.Update(st);
                }


                StatusLabel.Text = string.Format("{0}進度({1})", sDate, aCol[0]);


                Application.DoEvents();
              }


            }
          }


          dt = dt.AddDays(1);
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        throw ex;
      }
    }

    private void button5_Click(object sender, EventArgs e)
    {
      //開始日期
      DateTime dt = new DateTime(2017, 8, 30);
      //結束日期
      DateTime dttarget = new DateTime(2007, 1, 1);

      //取得資料庫最後一天
      ssql = " select  distinct stockdate  from stockafter where stocktype = '{0}' order by stockdate asc ";

      var vDbDate = dbDapper.ExecuteScale(string.Format(ssql, M10Const.StockType.tse));
      if (vDbDate != null)
      {
        string sDbDate = vDbDate.ToString();
        dt = new DateTime(Convert.ToInt32(sDbDate.Substring(0, 4))
          , Convert.ToInt32(sDbDate.Substring(4, 2))
          , Convert.ToInt32(sDbDate.Substring(6, 2)));
      }


      while (true)
      {
        string sLineTrans = "";
        try
        {
          if (dt.ToString("yyyyMMdd") == dttarget.ToString("yyyyMMdd")) break;

          string sUrl = "http://www.tse.com.tw/exchangeReport/MI_INDEX?response=csv&date={0}&type=ALLBUT0999";
          string sDate = dt.ToString("yyyyMMdd");

          StatusLabel.Text = string.Format("{2}-{0}進度({1})", sDate, "", M10Const.StockType.tse);
          Application.DoEvents();

          sUrl = string.Format(sUrl, sDate);
          HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sUrl);
          req.Proxy = null;
          string sTemp = "";
          using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
          {
            //判斷http回應狀態(HttpStatusCode.OK=200)
            if (resp.StatusCode != HttpStatusCode.OK)
            {
              StatusLabel.Text = string.Format("{0}進度({1})", sDate, "連線異常，資料重新取得");
              Application.DoEvents();

              System.Threading.Thread.Sleep(3000);
              continue;
            }

            using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.GetEncoding(950)))
            {
              sTemp = SR.ReadToEnd();
            }

          }

          using (Stream s = GenerateStreamFromString(sTemp))
          {

            using (StreamReader sr = new StreamReader(s))
            {


              string Line;
              while ((Line = sr.ReadLine()) != null)
              {
                Line = Line.Replace(" ", "");
                Line = Line.Replace("\",\"", "|");
                Line = Line.Replace("\"", "");
                Line = Line.Replace(",", "");
                Line = Line.Replace("=", "");
                sLineTrans = Line;
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
                    sa.updatetime = Utils.getDatatimeString();
                    dbDapper.Insert(sa);
                  }
                  else
                  {
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
                    dbDapper.Update(sa);
                  }


                  StatusLabel.Text = string.Format("{2}-{0}進度({1})", sDate, aCol[0], M10Const.StockType.tse);
                  Application.DoEvents();
                }
              }
            }
          }

          dt = dt.AddDays(-1);

        }
        catch (Exception ex)
        {
          logger.Error(ex, "stock after:" + sLineTrans);
          System.Threading.Thread.Sleep(10000);
        }
      }

    }



    public static Stream GenerateStreamFromString(string s)
    {
      MemoryStream stream = new MemoryStream();
      StreamWriter writer = new StreamWriter(stream);
      writer.Write(s);
      writer.Flush();
      stream.Position = 0;
      return stream;
    }


    private void button6_Click(object sender, EventArgs e)
    {
      //開始日期
      DateTime dt = new DateTime(2017, 8, 29);
      //結束日期
      DateTime dttarget = new DateTime(2014, 12, 31);

      //取得資料庫最後一天
      ssql = " select  distinct stockdate  from stockafter where stocktype = '{0}' order by stockdate asc ";

      var vDbDate = dbDapper.ExecuteScale(string.Format(ssql, M10Const.StockType.otc));
      if (vDbDate != null)
      {
        string sDbDate = vDbDate.ToString();
        dt = new DateTime(Convert.ToInt32(sDbDate.Substring(0, 4))
          , Convert.ToInt32(sDbDate.Substring(4, 2))
          , Convert.ToInt32(sDbDate.Substring(6, 2)));
      }

      while (true)
      {
        System.Threading.Thread.Sleep(2000);
        try
        {
          if (dt.ToString("yyyyMMdd") == dttarget.ToString("yyyyMMdd")) break;

          string sUrl = "http://www.tpex.org.tw/web/stock/aftertrading/daily_close_quotes/stk_quote_download.php?l=zh-tw&d={0}&s=0,asc,0";
          string sDate = DateTime.Now.ToString("yyyyMMdd");
          int iyear = dt.Year - 1911;
          sDate = string.Format("{0}/{1}/{2}", iyear.ToString(), dt.ToString("MM"), dt.ToString("dd"));

          StatusLabel.Text = string.Format("{2}-{0}進度({1})", sDate, "", M10Const.StockType.otc);
          Application.DoEvents();

          sUrl = string.Format(sUrl, sDate);
          HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sUrl);
          req.Proxy = null;

          //改為寫入資料庫格式
          sDate = dt.ToString("yyyyMMdd");

          string sTemp = "";
          using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
          {
            //判斷http回應狀態(HttpStatusCode.OK=200)
            if (resp.StatusCode != HttpStatusCode.OK)
            {
              StatusLabel.Text = string.Format("{0}進度({1})", sDate, "連線異常，資料重新取得");
              Application.DoEvents();

              System.Threading.Thread.Sleep(10000);
              continue;
            }

            using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.GetEncoding(950)))
            {
              sTemp = SR.ReadToEnd();
            }
          }


          using (Stream s = GenerateStreamFromString(sTemp))
          {

            using (StreamReader sr = new StreamReader(s))
            {

              string Line;
              while ((Line = sr.ReadLine()) != null)
              {
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
                    dbDapper.Insert(sa);
                  }
                  else
                  {
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
                    dbDapper.Update(sa);
                  }

                  StatusLabel.Text = string.Format("{2}-{0}進度({1})", sDate, aCol[0], M10Const.StockType.otc);
                  Application.DoEvents();
                }
              }
            }
          }


          dt = dt.AddDays(-1);

        }
        catch (Exception ex)
        {
          logger.Error(ex);
          System.Threading.Thread.Sleep(10000);
        }
      }

    }

    private void button7_Click(object sender, EventArgs e)
    {
      try
      {
        string stockcode = "0000";
        if (stockcode == "0000") stockcode = "%23001";

        string sUrl = "https://tw.quote.finance.yahoo.net/quote/q?type=tick&sym={0}";
        string sDate = DateTime.Now.ToString("yyyyMMdd");


        sUrl = string.Format(sUrl, stockcode);
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
      catch (Exception ex)
      {
        //logger.Error(ex);
        //throw ex;
      }



      //return sr;

    }
  }
}
