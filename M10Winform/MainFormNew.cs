﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using CL.Data;
using System.Globalization;
using NLog;
using FluentFTP;
using System.Net;
using M10.lib.model;

namespace M10Winform
{
  public partial class MainFormNew : BaseForm
  {
    public ODAL oDal = new ODAL(Properties.Settings.Default.DBDefault);

    string folderName = @"D:\m10\temp\";
    string folderBack = @"D:\m10\back\";
    string folderError = @"D:\m10\xmlerror\";
    
    //1040806 新的ftp主機
    string sIP = "140.116.38.196";
    string sUser = "m10sys";
    string sPassword = "m10sys";
    
    public MainFormNew()
    { 
      InitializeComponent();
      base.InitForm();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      try
      {
        //相關建立資料夾
        if (!Directory.Exists(folderBack)) Directory.CreateDirectory(folderBack);
        if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);
        if (!Directory.Exists(folderError)) Directory.CreateDirectory(folderError);
      }
      catch (Exception ex)
      {

      }

      timer1.Enabled = true;
    }


    private void timer1_Tick(object sender, EventArgs e)
    {
      timer1.Enabled = false;

      if (chktimer.Checked == false) return;      

      try
      {
        ShowMessageToFront("轉檔啟動");
        
        ProceStart();

        ShowMessageToFront("轉檔完畢");
      }
      catch (Exception ex)
      { 
        logger.Error(ex, "M10Winform轉檔錯誤:");
      }

      System.Threading.Thread.Sleep(5000);

      this.Close();
    }

    private void ProceStart()
    {
      try
      {
        // FTP 下載資料到本機
        if (chkdownload.Checked == true)
        {
          //1060523 FTP改寫使用 FluentFTP
          //FtpDownload();
          FtpDownloadNew();
        }

        //return;

        // 取得資料夾內所有檔案
        foreach (string fname in System.IO.Directory.GetFiles(folderName))
        { 
          //轉檔到DB
          TransToDB(fname);
          
          FileInfo fi = new FileInfo(fname);

          //記錄轉檔資料
          FileTransLog(fi.Name);

          //存至備份資料夾
          fi.CopyTo(folderBack + fi.Name, true);
          fi.Delete();
        }

      }
      catch (Exception ex)
      {
        
      }
    }

    private void FileTransLog(string pFileName)
    { 
      FileTransLog item = new FileTransLog();
      item.FileTransName = pFileName;
      item.FileTransTime = DateTime.Now;

      //寫入
      dbDapper.Insert(item);
    }

    
    private void FtpDownloadNew()
    {
      //FluentFTP 起始路徑都是跟目錄開始，目錄結尾都是/
      string sFTPArrangePath = "/M10_System/M10/XML/";
      string sFTPXmlPath = "/14_FCU_raindata/M10/";
      
      FtpClient client = new FtpClient();      
      try
      { 
        client.Host = sIP;

        // if you don't specify login credentials, we use the "anonymous" user account
        client.Credentials = new NetworkCredential(sUser, sPassword);

        ShowMessageToFront("Ftp連線");
        // begin connecting to the server
        client.Connect();

        ShowMessageToFront("Ftp取得清單");

        //取得檔案名稱清單
        List<string> lstFileName = new List<string>();
        foreach (FtpListItem  item in client.GetListing(sFTPXmlPath))
        {
          if (item.Type == FtpFileSystemObjectType.File)
          {
            lstFileName.Add(item.Name);
          }
        }        

        //FTP檔案整理
        foreach (string sFileName in lstFileName)
        {
          //判斷是否已轉檔
          ssql = @" select * from FileTransLog where FileTransName = '{0}' ";
          var DataList = dbDapper.Query(string.Format(ssql, sFileName));

          //沒有下載紀錄則下載
          if (DataList.Count == 0)
          {
            /* Download a File */            
            client.DownloadFile(folderName + sFileName, string.Format(@"{0}{1}", sFTPXmlPath, sFileName));
            ShowMessageToFront("Ftp下載檔案到" + folderName + sFileName);
          }
          else //已下載，且時間超過1天，則移動FTP檔案到bak資料夾
          { 
            //1060408 修改為1天
            DateTime dtRunTimeRainData = DateTime.Now.AddDays(-1);

            IFormatProvider yyyymmddFormat = new CultureInfo(String.Empty, false);
            string f = "yyyy_MM_dd_HH_mm";
            DateTime dtRTime = DateTime.ParseExact(sFileName.Split('.')[0], f, yyyymmddFormat);

            if (DateTime.Compare(dtRTime, dtRunTimeRainData) <= 0)
            {
              //==1060406 自動化歸檔-建立歸屬資料夾
              List<string> slist = sFileName.Split('_').ToList<string>();              
              string sSaveFolder = string.Format(@"{0}{1}/{2}/{3}/", sFTPArrangePath, slist[0], slist[1], slist[2]);
              
              //建立資料夾
              client.CreateDirectory(sSaveFolder);

              ShowMessageToFront(string.Format("Ftp[{0}]檔案移動到{0}", sFileName, sSaveFolder + sFileName));
              //移動檔案至歸屬資料夾
              //client.Rename(string.Format(@"{0}{1}", sFTPXmlPath, sFileName), string.Format(@"{0}{1}", sSaveFolder, sFileName));
              client.MoveFile(string.Format(@"{0}{1}", sFTPXmlPath, sFileName), string.Format(@"{0}{1}", sSaveFolder, sFileName));
            }

          }
        }

      }
      catch (Exception ex)
      {
        ShowMessageToFront(ex.ToString());        
      }finally
      {
        client.Disconnect();        
      }

      //等待五秒
      System.Threading.Thread.Sleep(5000);
    }

    private void TransToDB(string sFilePath)
    {
      try
      {
        ShowMessageToFront("轉檔:" + sFilePath);

        XmlDocument xd = new XmlDocument();

        try
        {
          xd.Load(sFilePath);
        }
        catch (Exception)
        {
          //XML解析錯誤，將檔案搬到錯誤資料夾

          //存至備份資料夾
          FileInfo fi = new FileInfo(sFilePath);
          //記錄轉檔資料
          FileTransLog(fi.Name);
          if (File.Exists(folderError + fi.Name))
          {
            fi.Delete();
          }
          else
          {
            fi.MoveTo(folderError + fi.Name);
          }

          return;
          //
        }

        XmlNodeList nodelist = xd.SelectNodes("//Rain");

        DataTable dt = new DataTable();

        foreach (XmlNode OneNode in nodelist)
        {
          //檢查欄位，不足則新增
          foreach (XmlNode ChildNode in OneNode)
          {
            if (dt.Columns.Contains(ChildNode.Name) == false)
            {
              dt.Columns.Add(ChildNode.Name);
            }
          }

          DataRow dr = dt.NewRow();
          foreach (XmlNode ChildNode in OneNode)
          {
            dr[ChildNode.Name] = ChildNode.InnerText;
          }

          dt.Rows.Add(dr);
        }

        //1040730 DataTable加入兩欄位
        dt.Columns.Add("LRTI");
        dt.Columns.Add("WLRTI");

        //計算LRTI 與 WLRTI
        foreach (DataRow dr in dt.Rows)
        {
          string sLRTI = "0";
          string sWLRTI = "0";

          sLRTI = CalLRTI(dr);
          dr["LRTI"] = sLRTI;
        }

        //變更縣市資料 台北縣->新北市 台中縣->台中市 桃園縣->桃園市
        //1050703 變更縣市資料 臺北市->台北市 臺中市->台中市
        //1050705 變更縣市資料 台北市->臺北市 台中市->臺中市 台南市->臺南市
        foreach (DataRow dr in dt.Rows)
        {
          //新北
          if (dr["COUNTY"].ToString() == "台北縣") dr["COUNTY"] = "新北市";

          //臺北                    
          if (dr["COUNTY"].ToString() == "台北市") dr["COUNTY"] = "臺北市";

          //桃園
          if (dr["COUNTY"].ToString() == "桃園縣") dr["COUNTY"] = "桃園市";

          //臺中
          if (dr["COUNTY"].ToString() == "台中縣") dr["COUNTY"] = "臺中市";
          if (dr["COUNTY"].ToString() == "台中市") dr["COUNTY"] = "臺中市";

          //臺南
          if (dr["COUNTY"].ToString() == "台南市") dr["COUNTY"] = "臺南市";
        }
        
        //轉入StationData
        foreach (DataRow dr in dt.Rows)
        {
          //判斷異常資料，則不進行轉入資料
          string sSTNAME = dr["STNAME"].ToString();
          if (sSTNAME.Contains("?"))
          {
            continue;
          }
          
          string sSTID = dr["STID"].ToString();

          ssql = " select * from StationData where STID = @STID ";       
          StationData NewStationData = dbDapper.QuerySingleOrDefault<StationData>(ssql, new { STID = sSTID });

          //沒資料則寫入一筆新資料
          if (NewStationData == null)
          {
            NewStationData = new StationData();
            
            NewStationData.STID = dr["STID"].ToString();
            NewStationData.STNAME = dr["STNAME"].ToString();
            NewStationData.COUNTY = dr["COUNTY"].ToString();
            NewStationData.TOWN = dr["TOWN"].ToString();

            dbDapper.Insert(NewStationData);
          }
        }

        //清空runtime 資料表
        dbDapper.Execute("delete RunTimeRainData");

        //寫入RuntimeRainData
        foreach (DataRow dr in dt.Rows)
        {
          RunTimeRainData RuntimeData = new RunTimeRainData();
          RuntimeData.STID = dr["STID"].ToString();
          RuntimeData.STNAME = dr["STNAME"].ToString();
          RuntimeData.STTIME = dr["STTIME"].ToString();
          RuntimeData.LAT = dr["LAT"].ToString();
          RuntimeData.LON = dr["LON"].ToString();
          RuntimeData.ELEV = dr["ELEV"].ToString();
          RuntimeData.RAIN = RainDataValid(dr["RAIN"].ToString());
          RuntimeData.MIN10 = RainDataValid(dr["MIN10"].ToString());
          RuntimeData.HOUR3 = RainDataValid(dr["HOUR3"].ToString());
          RuntimeData.HOUR6 = RainDataValid(dr["HOUR6"].ToString());
          RuntimeData.HOUR12 = RainDataValid(dr["HOUR12"].ToString());
          RuntimeData.HOUR24 = RainDataValid(dr["HOUR24"].ToString());
          RuntimeData.NOW = RainDataValid(dr["NOW"].ToString());
          RuntimeData.COUNTY = dr["COUNTY"].ToString();
          RuntimeData.TOWN = dr["TOWN"].ToString();
          RuntimeData.ATTRIBUTE = dr["ATTRIBUTE"].ToString();
          RuntimeData.diff = dr["diff"].ToString();
          RuntimeData.STATUS = dr["STATUS"].ToString();
          RuntimeData.TMX = dr["TMX"].ToString();
          RuntimeData.TMY = dr["TMY"].ToString();
          RuntimeData.RTime = dr["RTime"].ToString();
          RuntimeData.SWCBID = dr["SWCBID"].ToString();
          RuntimeData.DebrisRefStation = dr["DebrisRefStation"].ToString();
          RuntimeData.EffectiveRainfall = dr["EffectiveRainfall"].ToString();
          RuntimeData.RT = dr["RT"].ToString();
          RuntimeData.Cumulation = dr["Cumulation"].ToString();
          RuntimeData.Day1 = dr["Day1"].ToString();
          RuntimeData.Day2 = dr["Day2"].ToString();
          RuntimeData.Day3 = dr["Day3"].ToString();
          RuntimeData.Hour2 = RainDataValid(dr["Hour2"].ToString());
          RuntimeData.WGS84_lon = dr["WGS84_lon"].ToString().Trim();
          RuntimeData.LRTI = dr["LRTI"].ToString().Trim();
          RuntimeData.WLRTI = dr["WLRTI"].ToString().Trim();
          RuntimeData.WGS84_lat = dr["WGS84_lat"].ToString().Trim();

          dbDapper.Insert(RuntimeData);
        }

        //資料寫入sql
        foreach (DataRow dr in dt.Rows)
        {
          //刪除資料
          ssql = string.Format(" delete RainStation  where STID = '{0}' and RTime = '{1}' "
                , dr["STID"].ToString()
                , dr["RTime"].ToString());
          dbDapper.Execute(ssql);

          //新增資料
          RainStation NewData = new RainStation();
          NewData.STID = dr["STID"].ToString();
          NewData.STNAME = dr["STNAME"].ToString();
          NewData.STTIME = dr["STTIME"].ToString();
          NewData.LAT = dr["LAT"].ToString();
          NewData.LON = dr["LON"].ToString();
          NewData.ELEV = dr["ELEV"].ToString();
          NewData.RAIN = RainDataValid(dr["RAIN"].ToString());
          NewData.MIN10 = RainDataValid(dr["MIN10"].ToString());
          NewData.HOUR3 = RainDataValid(dr["HOUR3"].ToString());
          NewData.HOUR6 = RainDataValid(dr["HOUR6"].ToString());
          NewData.HOUR12 = RainDataValid(dr["HOUR12"].ToString());
          NewData.HOUR24 = RainDataValid(dr["HOUR24"].ToString());
          NewData.NOW = RainDataValid(dr["NOW"].ToString());
          NewData.COUNTY = dr["COUNTY"].ToString();
          NewData.TOWN = dr["TOWN"].ToString();
          NewData.ATTRIBUTE = dr["ATTRIBUTE"].ToString();
          NewData.diff = dr["diff"].ToString();
          NewData.STATUS = dr["STATUS"].ToString();
          NewData.TMX = dr["TMX"].ToString();
          NewData.TMY = dr["TMY"].ToString();
          NewData.RTime = dr["RTime"].ToString();
          NewData.SWCBID = dr["SWCBID"].ToString();
          NewData.DebrisRefStation = dr["DebrisRefStation"].ToString();
          NewData.EffectiveRainfall = dr["EffectiveRainfall"].ToString();
          NewData.RT = dr["RT"].ToString();
          NewData.Cumulation = dr["Cumulation"].ToString();
          NewData.Day1 = dr["Day1"].ToString();
          NewData.Day2 = dr["Day2"].ToString();
          NewData.Day3 = dr["Day3"].ToString();
          NewData.Hour2 = RainDataValid(dr["Hour2"].ToString());
          NewData.WGS84_lon = dr["WGS84_lon"].ToString().Trim();
          NewData.LRTI = dr["LRTI"].ToString().Trim();
          NewData.WLRTI = dr["WLRTI"].ToString().Trim();
          NewData.WGS84_lat = dr["WGS84_lat"].ToString().Trim();

          dbDapper.Insert<RainStation>(NewData);
        }
      }
      catch (Exception)
      {

      }
    }

    private DateTime RTimeToDateTime(string pRTime)
    {
      DateTime dt = new DateTime();

      string syyyy, sMM, sdd, shh, smm, sss;
      syyyy = pRTime.Substring(0, 4);
      sMM = pRTime.Substring(5, 2);
      sdd = pRTime.Substring(8, 2);
      shh = pRTime.Substring(11, 2);
      smm = pRTime.Substring(14, 2);
      sss = pRTime.Substring(17, 2);
      return dt;
    }


    private string RainDataValid(string pRainData)
    {
      string sResult = "0";
      try
      {
        double dResult = 0;
        double.TryParse(pRainData, out dResult);

        if (dResult < 0)
        {
          dResult = 0;
        }

        sResult = dResult.ToString();
      }
      catch (Exception ex)
      {

      }

      return sResult;
    }


    private void ShowMessageToFront(string pMsg)
    {
      richTextBox1.AppendText(pMsg + "\r\n");
      this.Update();
      Application.DoEvents();
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
      //測試帳號密碼
      sIP = "192.168.1.100";
      sUser = "m10sys";
      sPassword = "m10sys";

      ProceStart();
    }

    private string CalLRTI(DataRow pDr)
    {
      string sSTID = pDr["STID"].ToString().Trim();
      string sRTIME = pDr["RTime"].ToString();
      string sResult = string.Empty;

      //轉換datetime
      DateTime dtRTime1 = Convert.ToDateTime(sRTIME);
      DateTime dtRTime2 = dtRTime1.AddHours(-1);
      DateTime dtRTime3 = dtRTime1.AddHours(-2);

      //取得這三個小時的雨量資料
      double dRain1 = 0;
      double dRain2 = 0;
      double dRain3 = 0;
      double dRT = 0;

      string sFsql = string.Empty;
      //sFsql = " select * from RainStation "
      //                + " where STID = '{0}' "
      //                + " and RTime = '{1}' "
      //                + "  "
      //                + "  ";

      //ssql = string.Format(sFsql, sSTID, dtRTime1.ToString("s"));
      //oDal.CommandText = ssql;
      //DataRow dr = oDal.DataRow();
      //if (dr != null)
      //{
      //    double.TryParse(dr["RAIN"].ToString(), out dRain1);
      //    double.TryParse(dr["RT"].ToString(), out dRT);
      //}

      //由傳進來的資料解析
      double.TryParse(pDr["RAIN"].ToString(), out dRain1);
      double.TryParse(pDr["RT"].ToString(), out dRT);


      
      sFsql = " select RAIN from RainStation "
                 + " where STID = @STID "
                 + " and RTime = @RTime "
                 ;      
      object oRainTemp2 = dbDapper.ExecuteScale(sFsql, new { STID = sSTID, RTime = dtRTime2.ToString("s") });
      if (oRainTemp2 != null)
      {
        double.TryParse(oRainTemp2.ToString(), out dRain2);
      }
      
      object oRainTemp3 = dbDapper.ExecuteScale(sFsql, new { STID = sSTID, RTime = dtRTime3.ToString("s") });
      if (oRainTemp3 != null)
      {
        double.TryParse(oRainTemp3.ToString(), out dRain3);
      }

      //sFsql = " select RAIN from RainStation "
      //            + " where STID = '{0}' "
      //            + " and RTime = '{1}' "
      //            ;

      //ssql = string.Format(sFsql, sSTID, dtRTime2.ToString("s"));
      //oDal.CommandText = ssql;
      //object oRainTemp2 = oDal.Value();
      //if (oRainTemp2 != null)
      //{
      //  double.TryParse(oRainTemp2.ToString(), out dRain2);
      //}

      //ssql = string.Format(sFsql, sSTID, dtRTime3.ToString("s"));
      //oDal.CommandText = ssql;
      //object oRainTemp3 = oDal.Value();
      //if (oRainTemp3 != null)
      //{
      //  double.TryParse(oRainTemp3.ToString(), out dRain3);
      //}

      //前三個小時平均 * RT值
      double dResult = (dRain1 + dRain2 + dRain3) / 3 * dRT;
      sResult = Math.Round(dResult, 2).ToString();

      return sResult;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      string sSTID = "01U8801";
      ssql = " select * from StationData where STID = @STID ";
      var DataList = dbDapper.Query(ssql, new { STID = sSTID });
      StationData NewStationData = new StationData();
      NewStationData = dbDapper.QuerySingleOrDefault<StationData>(ssql, new { STID = sSTID });


      
      string sRTIME = "2017-03-21T10:10:00";
     

      //轉換datetime
      DateTime dtRTime1 = Convert.ToDateTime(sRTIME);
      DateTime dtRTime2 = dtRTime1.AddHours(-1);
      DateTime dtRTime3 = dtRTime1.AddHours(-2);
      double dRain2;
      double dRain3;
      string sFsql = " select RAIN from RainStation "
                 + " where STID = @STID "
                 + " and RTime = @RTime "
                 ;
      object oRainTemp2 = dbDapper.ExecuteScale(sFsql, new { @STID = sSTID, @RTime = dtRTime2.ToString("s") });
      if (oRainTemp2 != null)
      {
        double.TryParse(oRainTemp2.ToString(), out dRain2);
      }

      object oRainTemp3 = dbDapper.ExecuteScale(sFsql, new { @STID = sSTID, @RTime = dtRTime3.ToString("s") });
      if (oRainTemp3 != null)
      {
        double.TryParse(oRainTemp3.ToString(), out dRain3);
      }

      return;


      //測試帳號密碼
      sIP = "192.168.1.100";
      sUser = "m10sys";
      sPassword = "m10sys";

      FtpClient client = new FtpClient();
      client.Host = sIP;

      // if you don't specify login credentials, we use the "anonymous" user account
      client.Credentials = new NetworkCredential(sUser, sPassword);

      // begin connecting to the server
      client.Connect();


      client.CreateDirectory("/456/789/測試/");

      ShowMessageToFront("aaaa");
      //foreach (FtpListItem item in client.GetListing("/123", FtpListOption.))
      //{
      //  ShowMessageToFront(item.FullName);
      //}

      // get a list of files and directories in the "/htdocs" folder
      //foreach (FtpListItem item in client.GetListing("/123")) {

      //  // if this is a file
      //  if (item.Type == FtpFileSystemObjectType.File)
      //  {

      //    // get the file size
      //    long size = client.GetFileSize(item.FullName);

      //  }

      //  // get modified date/time of the file or folder
      //  DateTime time = client.GetModifiedTime(item.FullName);

      //  // calculate a hash for the file on the server side (default algorithm)
      //  //FtpHash hash = client.GetHash(item.FullName);

      //}



      //// rename a file
      //client.Rename("/123/index.txt", "/123/新目錄/index.txt");

      //// delete a file
      //client.DeleteFile("/123/新目錄/index.txt");

      //// delete a folder recursively
      //client.DeleteDirectory("/htdocs/extras/", FtpListOption.Size);

      //// check if a file exists
      if (client.FileExists("/123/新文字文件.txt")) { ShowMessageToFront("have data."); }

      //// check if a folder exists
      //if (client.DirectoryExists("/htdocs/extras/")) { }

      // disconnect! good bye!
      client.Disconnect();

      ShowMessageToFront("完畢");
    }
  }
}