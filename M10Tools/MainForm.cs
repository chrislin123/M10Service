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
      string bbbb = "";

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
      //開始日期
      DateTime dt = new DateTime(2017, 9, 8);
      //結束日期
      DateTime dtEnd = new DateTime(2017, 9, 12);

      for (DateTime LoopDatetime = dt; LoopDatetime <= dtEnd; LoopDatetime = LoopDatetime.AddDays(1))
      {
        string sLineTrans = "";
        try
        {
          StatusLabel.Text = string.Format("{0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
          Application.DoEvents();

          if (Stockhelper.GetStockThreeTradeTse(LoopDatetime) == false)
          {
            System.Threading.Thread.Sleep(3000);
            continue;
          }

          StatusLabel.Text = string.Format("{0}-{1}", M10Const.StockType.tse, "完成");
          Application.DoEvents();
        }
        catch (Exception ex)
        {
          logger.Error(ex, "stock after:" + sLineTrans);
          System.Threading.Thread.Sleep(10000);
        }
      }

      StatusLabel.Text = "完成";
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
          StatusLabel.Text = string.Format("{0}-{1}", M10Const.StockType.otc, LoopDatetime.ToString("yyyyMMdd"));
          Application.DoEvents();

          if (Stockhelper.GetStockThreeTradeOtc(LoopDatetime) == false)
          {
            System.Threading.Thread.Sleep(3000);
            continue;
          }

          StatusLabel.Text = string.Format("{0}-{1}", M10Const.StockType.otc, "完成");
          Application.DoEvents();
        }
        catch (Exception ex)
        {
          logger.Error(ex, "stock after:" + sLineTrans);
          System.Threading.Thread.Sleep(10000);
        }
      }
      StatusLabel.Text = "完成";
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
          StatusLabel.Text = string.Format("{0}-{1}", M10Const.StockType.tse, LoopDatetime.ToString("yyyyMMdd"));
          Application.DoEvents();

          if (Stockhelper.GetStockAfterTse(LoopDatetime) == false)
          {
            System.Threading.Thread.Sleep(3000);
            continue;
          }

          StatusLabel.Text = string.Format("{0}-{1}", M10Const.StockType.tse, "完成");
          Application.DoEvents();
        }
        catch (Exception ex)
        {
          logger.Error(ex, "stock after:" + sLineTrans);
          System.Threading.Thread.Sleep(10000);
        }
      }
      StatusLabel.Text = "完成";
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
          StatusLabel.Text = string.Format("{0}-{1}", M10Const.StockType.otc, LoopDatetime.ToString("yyyyMMdd"));
          Application.DoEvents();

          if (Stockhelper.GetStockAfterOtc(LoopDatetime) == false)
          {
            System.Threading.Thread.Sleep(3000);
            continue;
          }

          StatusLabel.Text = string.Format("{0}-{1}", M10Const.StockType.otc, "完成");
          Application.DoEvents();
        }
        catch (Exception ex)
        {
          logger.Error(ex, "stock after:" + sLineTrans);
          System.Threading.Thread.Sleep(10000);
        }
      }
      StatusLabel.Text = "完成";
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
      catch (Exception ex)
      {
        //logger.Error(ex);
        //throw ex;
      }



      //return sr;

    }

    private void button8_Click(object sender, EventArgs e)
    {

      try
      {
        string sRunDate = DateTime.Now.ToString("yyyyMMdd");
        ssql = "  select Max(getdate) from StockGet  ";
        object oMax = dbDapper.ExecuteScale(ssql);
        if (oMax != null)
        {
          sRunDate = Convert.ToString(Convert.ToInt32(oMax.ToString()) + 1);
        }

        //刪除目前存在資料
        //dbDapper.Execute("delete stockget");

        List<StockInfo> siList = new List<StockInfo>();
        ssql = " select * from stockinfo where status = 'Y' and LEN(stockcode) = 4 order by stockcode ";
        siList = dbDapper.Query<StockInfo>(ssql);

        int iListTotal = siList.Count();
        int idex = 0;

        foreach (StockInfo item in siList)
        {
          idex++;
          string stockcode = item.stockcode;

          //stockcode = "3162";

          //判斷盤後資料是否存在
          ssql = " select * from StockAfter where stockcode  = '{0}' ";
          ssql = string.Format(ssql, stockcode);
          int iTotal = dbDapper.QueryTotalCount(ssql);
          if (iTotal == 0)
          {
            continue;
          }

          DateTime dt = DateTime.Now;
          DateTime dtTarget = dt.AddDays(-180);


          while (true)
          {
            if (dt.ToString("yyyyMMdd") == dtTarget.ToString("yyyyMMdd")) break;

            //if (stockcode == "1413")
            //{
            //  ssql = "";
            //}

            //if (dt.ToString("yyyyMMdd") == "20170818" && stockcode == "3162")
            //{
            //  ssql = "";
            //}

            StatusLabel.Text = string.Format("[{2}/{3}]{0}({1})"
              , stockcode, dt.ToString("yyyyMMdd"), idex.ToString(), iListTotal.ToString());
            Application.DoEvents();

            ssql = @" select top 3 * from stockafter 
               where stockdate <= '{0}' and stockcode = '{1}' order by stockdate desc
               ";
            ssql = string.Format(ssql, dt.ToString("yyyyMMdd"), stockcode);
            List<Stockafter> saList = dbDapper.Query<Stockafter>(ssql);

            if (saList.Count != 3)
            {
              dt = dt.AddDays(-1);
              continue;
            }

            if (saList.Count == 3)
            {
              //to
              Stockafter saToday = saList[0];
              //yes
              Stockafter saYes = saList[1];
              //per
              Stockafter saPer = saList[2];

              //判斷前一天+9%
              Boolean bCheckUp9 = checkUp9(saYes, saPer);
              if (bCheckUp9 == false)
              {
                dt = dt.AddDays(-1);
                continue;
              }
              //判斷當天為180天最大量
              Boolean bCheck180Max = check180Max(saToday);

              //今天量是昨天的兩倍
              Boolean bCheckTodayOver2 = checkTodayOver2(saToday, saYes);


              if (bCheckUp9 == true && bCheck180Max == true && bCheckTodayOver2 == true)
              {
                //確認資料是否已存在，如果存在則判斷下一天
                ssql = " select * from StockGet where stockcode = '{0}' and stockdate = '{1}' and getdate  = '{2}' ";
                ssql = string.Format(ssql, stockcode, saToday.stockdate, sRunDate);
                StockGet sgcheck = dbDapper.QuerySingleOrDefault<StockGet>(ssql);
                if (sgcheck != null)
                {
                  dt = dt.AddDays(-1);
                  continue;
                }


                StockGet sg = new StockGet();
                sg.getdate = sRunDate;
                sg.stockcode = saToday.stockcode;
                sg.stockdate = saToday.stockdate;

                dbDapper.Insert(sg);
              }


            }

            dt = dt.AddDays(-1);
            continue;

          }
        }


        StatusLabel.Text = "轉檔完畢";
        Application.DoEvents();


      }
      catch (Exception ex)
      {
        logger.Error(ex, "巨量換手轉檔異常通報");
      }
    }

    /// <summary>
    /// 判斷前一天上漲>9%
    /// </summary>
    /// <param name="saYes"></param>
    /// <param name="saPer"></param>
    /// <returns></returns>
    private Boolean checkUp9(Stockafter saYes, Stockafter saPer)
    {
      Boolean bResult = false;


      if (saYes.updown == "+")
      {
        double aa = 0.09;
        decimal dd = 0;
        dd = (Convert.ToDecimal(saYes.pricelast) - Convert.ToDecimal(saPer.pricelast)) / Convert.ToDecimal(saPer.pricelast);
        if (dd > Convert.ToDecimal(aa))
        {
          bResult = true;
        }
      }



      return bResult;
    }

    /// <summary>
    /// 當天是180天 最大量
    /// </summary>
    /// <param name="saYes"></param>
    /// <param name="saPer"></param>
    /// <returns></returns>
    private Boolean check180Max(Stockafter saToday)
    {
      Boolean bResult = false;


      ssql = @" select max(dealnum) from (
                      select top 180 * from stockafter where stockcode = '{1}' 
                      and stockdate <= '{0}'  order by stockdate desc
                      ) as dd 
                     ";
      ssql = string.Format(ssql, saToday.stockdate, saToday.stockcode);
      object sa = dbDapper.ExecuteScale(ssql);
      if (sa != null)
      {
        long lDealNum = Convert.ToInt64(sa.ToString());

        if (saToday.dealnum == lDealNum)
        {
          bResult = true;
        }
      }


      return bResult;
    }


    /// <summary>
    /// 今天的量是昨天的兩倍
    /// </summary>
    /// <param name="saToday"></param>
    /// <param name="saYes"></param>
    /// <returns></returns>
    private Boolean checkTodayOver2(Stockafter saToday, Stockafter saYes)
    {
      Boolean bResult = false;

      if (saToday.dealnum > saYes.dealnum * 2)
      {
        bResult = true;
      }

      return bResult;
    }

    private void button2_Click_1(object sender, EventArgs e)
    {
      StatusLabel.Text = string.Format("StockInfor Trans 開始");
      Application.DoEvents();

      Stockhelper.GetStockInfo();

      StatusLabel.Text = string.Format("StockInfor Trans 完成");
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

              StatusLabel.Text = string.Format(sSplit[0]);
              Application.DoEvents();

            }
          }


        }

        StatusLabel.Text = string.Format("完成");
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
          StatusLabel.Text = string.Format("完成");
          Application.DoEvents();

          return;
        }


        ssql = "select top 1000 * from stockafter where priceyesterday is null  ";
        List<Stockafter> SaList = dbDapper.Query<Stockafter>(ssql);

        foreach (Stockafter LoopItem in SaList)
        {

          StatusLabel.Text = string.Format("[{0}]{1}",LoopItem.stockdate,LoopItem.stockcode);
          Application.DoEvents();

          Decimal dPriceYesterday = Stockhelper.CalcPriceYesterday(LoopItem.pricelast.ToString(), LoopItem.updown, LoopItem.pricediff);

          LoopItem.priceyesterday = dPriceYesterday;

          dbDapper.Update(LoopItem);
        }




      }





    }
  }
}
