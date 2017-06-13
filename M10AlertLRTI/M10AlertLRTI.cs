using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
using CL.Data;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet; //excel
using ClosedXML.Excel;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Configuration;
using M10.lib;
using M10.lib.model;
using M10AlertLRTI.Models;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;

namespace M10AlertLRTI
{
  public partial class M10AlertLRTI : BaseForm
  {
    DateTime dtNow = DateTime.Now;

    string folderName = @"D:\m10\LRTIAlert\";
    string sLritAlertTimeString = "";

    static private DataTable LrtiAlertAll = new DataTable();
    static private DataTable LrtiAlertNew = new DataTable();
    static private DataTable LrtiAlertDel = new DataTable();


    public M10AlertLRTI()
    {
      InitializeComponent();

      //載入BaseForm資料
      base.InitForm();
    }


    private void OpenExcel(string FileName)
    {
      System.Diagnostics.Process p = new System.Diagnostics.Process();

      //OpenFileDialog dlg = new OpenFileDialog(); // dig是我自己設定的//


      //dlg.Filter = "Excel Worksheets|*.xls"; // Filter by Excel Worksheet 限定EXCEL

      string MS_Excel;// <-------指定你的Excel執行檔位置所在給它(底下這行是我的Excel執行檔的位置//




      MS_Excel = @"C:\Program Files\Microsoft Office\OFFICE15\EXCEL.EXE";

      if (File.Exists(MS_Excel) == false)
      {
        MS_Excel = @"C:\Program Files (x86)\Microsoft Office\Office14\EXCEL.EXE";
      }




      p.StartInfo.FileName = MS_Excel;

      p.StartInfo.Arguments = FileName;

      p.Start();
    }

    private void btnStart_Click(object sender, EventArgs e)
    {

      TransKML();

      return;

      try
      {

        //string ss = "";
        //int aa = int.Parse(ss);

        //紀錄資料更新時間(2017-05-16T10:31:14)
        LRTIAlertUpdateTime();

        //Alert LRTI資料處理
        LRTIAlertProc();

        //AlertLRTI寫入歷史資料
        LRTIAlertRecToHis();

        //取得警戒通知資料
        getLRTIAlertData();

        //1050615 判斷有資料才進行警戒提醒
        if (LrtiAlertAll.Rows.Count == 0 && LrtiAlertNew.Rows.Count == 0 && LrtiAlertDel.Rows.Count == 0) return;

        //文件產生      
        string sAttachFileName = LRTIAlertReport();

        //1060519 判斷是否啟動發報功能，修改使用dapper
        var chkMailFlag = dbDapper.ExecuteScale("select value from LRTIAlertMail where type = 'isal' and value = 'Y'");
        if (string.IsNullOrEmpty(chkMailFlag as string)) return;

        //Alert LRTI寄送發布mail
        LRTIAlertSendMail(sAttachFileName);


        //開啟excel
        //OpenExcel(sAttachFileName);
      }
      catch (Exception ex)
      {
        logger.Error(ex, "M10AlertLRTI-btnStart_Click");
      }
    }

    private void TransKML()
    {
      try
      {


        string sFilePath = @"c:\doc.kml";

        XDocument kml1 = XDocument.Load(sFilePath);

        var ns = XNamespace.Get("http://www.opengis.net/kml/2.2");
        var placemarks = kml1.Element(ns + "kml").Element(ns + "Document").Element(ns + "Folder").Elements(ns + "Placemark");

        foreach (var item in placemarks)
        {
          //取得名稱
          string Name = item.Element(ns + "name").Value;

          ssql = " select * from StationVillageLRTI where village = '{0}' ";
          StationVillageLRTI RelData = dbDapper.QuerySingleOrDefault<StationVillageLRTI>(string.Format(ssql, Name));

          if (RelData != null)
          {

            var LinearRing = item.Element(ns + "MultiGeometry").Element(ns + "Polygon").Element(ns + "outerBoundaryIs").Element(ns + "LinearRing");
            string sAllCoord = LinearRing.Element(ns + "coordinates").Value;

            //依照格式拆解
            string[] CoorDataList = sAllCoord.Replace(" ", "").Replace(",0", "|").Split('|');

            int idx = 1;
            foreach (string LoopItem in CoorDataList)
            {
              //資料空白去除
              if (LoopItem == "") continue;

              string[] aItem = LoopItem.Split(',');

              Coordinate insData = new Coordinate();
              insData.type = "stvillage";
              insData.relano = RelData.no;
              insData.lat = aItem[1];
              insData.lng = aItem[0];
              insData.pointseq = idx;

              dbDapper.Insert(insData);

              idx++;
            }
          }
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex, "");

      }


      MessageBox.Show("Test");
    }




    private void LRTIAlertSendMail(string sAttachFileName)
    {
      string sSubject = "全台村里崩塌警戒提醒" + sLritAlertTimeString;
      List<string> AddressList = new List<string>();

      List<Attachment> AttachmentList = new List<Attachment>();
      AttachmentList.Add(new Attachment(sAttachFileName));

      string SenderMail = string.Empty;
      string SenderPass = string.Empty;
      var TempData = dbDapper.ExecuteScale("select value from LRTIAlertMail where type = 'main' ");
      if (TempData != null) SenderMail = TempData.ToString();
      TempData = dbDapper.ExecuteScale("select value from LRTIAlertMail where type = 'pass' ");
      if (TempData != null) SenderPass = TempData.ToString();

      var TempDataList = dbDapper.Query("select * from LRTIAlertMail where type = 'list'");
      foreach (var item in TempDataList)
      {
        AddressList.Add(item.value as string);
      }

      List<string> HtmlContentList = new List<string>();
      HtmlContentList.Add("降雨已達崩塌警戒即時資料：");
      HtmlContentList.Add("<br>");
      HtmlContentList.Add("http://140.116.38.211/M10api/warn/warnlist");
      HtmlContentList.Add("<br>");
      HtmlContentList.Add("降雨已達崩塌警戒即時地圖資料：");
      HtmlContentList.Add("<br>");
      HtmlContentList.Add("http://140.116.38.211/M10api/map/map");

      //1060519 寄送Gmail
      Gmail.SendMailByGmail(SenderMail, SenderPass, HtmlContentList, sSubject, AddressList, AttachmentList);
    }

    private void LRTIAlertRecToHis()
    {
      string sDt = dtNow.ToString("yyyy-MM-ddTHH:mm:ss");

      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnectionString))
        {
          List<LRTIAlert> lData = cn.Query<LRTIAlert>(" select * from LRTIAlert ").ToList<LRTIAlert>();
          List<LRTIAlertHis> HisData = new List<LRTIAlertHis>();

          foreach (LRTIAlert item in lData)
          {
            LRTIAlertHis oHis = new LRTIAlertHis();
            oHis.country = item.country;
            oHis.ELRTI = item.ELRTI;
            oHis.HOUR1 = item.HOUR1;
            oHis.HOUR2 = item.HOUR2;
            oHis.HOUR3 = item.HOUR3;
            oHis.LRTI = item.LRTI;
            oHis.RT = item.RT;
            oHis.status = item.status;
            oHis.STID = item.STID;
            oHis.town = item.town;
            oHis.village = item.village;
            oHis.RecTime = sDt;

            HisData.Add(oHis);
          }

          cn.Insert(HisData);
        }
      }
      catch (Exception)
      {

        //throw ex;
      }

    }

    private void LRTIAlertUpdateTime()
    {

      string sDt = dtNow.ToString("yyyy-MM-ddTHH:mm:ss");

      LRTIAlertMail item = dbDapper.QuerySingleOrDefault<LRTIAlertMail>(
        " select * from LRTIAlertMail where type = 'altm' ");
      if (item != null)
      {
        item.value = sDt;
        dbDapper.Update(item);
      }

    }

    private void getLRTIAlertData()
    {

      try
      {
        ssql = " select country,town,village,HOUR1,HOUR2,HOUR3,RT,LRTI,ELRTI from LRTIAlert "
             + " where status in ('C','I') "
             + " order by country,town ";
        oDal.CommandText = ssql;
        LrtiAlertAll.Clear();
        LrtiAlertAll = oDal.DataTable();
        ssql = " select country,town,village,HOUR1,HOUR2,HOUR3,RT,LRTI,ELRTI from LRTIAlert "
                 + " where status = 'I' "
                 + " order by country,town ";
        oDal.CommandText = ssql;
        LrtiAlertNew.Clear();
        LrtiAlertNew = oDal.DataTable();
        ssql = " select country,town,village,HOUR1,HOUR2,HOUR3,RT,LRTI,ELRTI from LRTIAlert "
                 + " where status = 'D' "
                 + " order by country,town ";
        oDal.CommandText = ssql;
        LrtiAlertDel.Clear();
        LrtiAlertDel = oDal.DataTable();
      }
      catch (Exception)
      {

      }
    }



    private string LRTIAlertReport()
    {
      DateTime dt = DateTime.Now;

      string sLrtiTime = string.Format("預報時間：{0}年 {1}月{2}日{3}時"
                          , Convert.ToString(dt.Year - 1911)
                          , Convert.ToString(dt.Month)
                          , Convert.ToString(dt.Day)
                          , Convert.ToString(dt.Hour));

      string fileName = folderName + string.Format("LrtiAlert_{0}.xlsx", sLritAlertTimeString);

      //Delete the file if it exists. 
      if (File.Exists(fileName))
      {
        File.Delete(fileName);
      }


      XLWorkbook workBook = new XLWorkbook();
      IXLWorksheet workSheet = workBook.AddWorksheet("預報單");
      IXLStyle style = workSheet.Style;

      //style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
      //style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;

      ////设置底部边框及颜色
      //style.Border.BottomBorder = XLBorderStyleValues.MediumDashDot;
      //style.Border.BottomBorderColor = XLColor.Red;

      ////设置顶部边框及颜色
      //style.Border.TopBorder = XLBorderStyleValues.SlantDashDot;
      //style.Border.TopBorderColor = XLColor.Black;

      ////设置左部边框及颜色
      //style.Border.LeftBorder = XLBorderStyleValues.MediumDashDotDot;
      //style.Border.LeftBorderColor = XLColor.Green;

      ////设置右部边框及颜色
      //style.Border.RightBorder = XLBorderStyleValues.Hair;
      //style.Border.RightBorderColor = XLColor.Yellow;

      //style.Font.Bold = true;
      //style.Font.FontColor = XLColor.Red;
      //style.Font.FontName = "微软雅黑";
      //style.Font.FontSize = 12;
      //style.Font.Italic = true;
      //style.Font.Shadow = false;
      //style.Font.Underline = XLFontUnderlineValues.Double;

      ////设置A1，B1的字体颜色为灰色
      //workSheet.Range("A1", "B1").Style.Font.FontColor = XLColor.Gray;
      ////把第5行第1列和第2列合并单元格
      //workSheet.Range(5, 1, 5, 2).Merge();
      ////设置第5行第1列和第2列内容左对齐
      //workSheet.Range(5, 1, 5, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
      ////设置第二列的宽度
      //workSheet.Column(2).Width = 30;
      ////设置第4到7列的宽度
      //workSheet.Columns(4, 7).Width = 40;

      int iRowIndex = 1;
      List<string> lHead = new List<string>();
      lHead.Add("縣市");
      lHead.Add("鄉鎮區");
      lHead.Add("警戒區範圍");
      lHead.Add("1hr");
      lHead.Add("2hr");
      lHead.Add("3hr");
      lHead.Add("Rt");
      lHead.Add("LRTI");
      lHead.Add("警戒LRTI");


      workSheet.Cell(iRowIndex, 1).Value = sLrtiTime;
      workSheet.Range(iRowIndex, 1, iRowIndex, 1).Style.Font.Bold = true;

      //目前警戒明細
      if (LrtiAlertAll.Rows.Count > 0)
      {
        iRowIndex++;
        workSheet.Cell(iRowIndex, 1).Value = "降雨已達崩塌警戒值之區域一覽表";
        workSheet.Range(iRowIndex, 1, iRowIndex, 1).Style.Font.Bold = true;
        workSheet.Range(iRowIndex, 1, iRowIndex, 1).Style.Fill.BackgroundColor = XLColor.GrannySmithApple;
        iRowIndex++;
        int iColIndex = 1;
        foreach (string item in lHead)
        {
          workSheet.Cell(iRowIndex, iColIndex).Value = item;
          workSheet.Range(iRowIndex, iColIndex, iRowIndex, iColIndex).Style.Fill.BackgroundColor = XLColor.LightGray;
          iColIndex++;
        }

        foreach (DataRow dr in LrtiAlertAll.Rows)
        {
          iRowIndex++;
          for (int i = 1; i <= lHead.Count; i++)
          {
            workSheet.Cell(iRowIndex, i).Value = dr[i - 1].ToString();
          }
        }
      }


      //新增明細
      if (LrtiAlertNew.Rows.Count > 0)
      {
        iRowIndex++;
        workSheet.Cell(iRowIndex, 1).Value = "崩塌警戒區域 新增 明細";
        workSheet.Range(iRowIndex, 1, iRowIndex, 1).Style.Font.Bold = true;
        workSheet.Range(iRowIndex, 1, iRowIndex, 1).Style.Fill.BackgroundColor = XLColor.GrannySmithApple;
        iRowIndex++;
        int iColIndex = 1;
        foreach (string item in lHead)
        {
          workSheet.Cell(iRowIndex, iColIndex).Value = item;
          workSheet.Range(iRowIndex, iColIndex, iRowIndex, iColIndex).Style.Fill.BackgroundColor = XLColor.LightGray;
          iColIndex++;
        }

        foreach (DataRow dr in LrtiAlertNew.Rows)
        {
          iRowIndex++;
          for (int i = 1; i <= lHead.Count; i++)
          {
            workSheet.Cell(iRowIndex, i).Value = dr[i - 1].ToString();
          }
        }
      }

      //解除明細
      if (LrtiAlertDel.Rows.Count > 0)
      {
        iRowIndex++;
        workSheet.Cell(iRowIndex, 1).Value = "崩塌警戒區域 解除 明細";
        workSheet.Range(iRowIndex, 1, iRowIndex, 1).Style.Font.Bold = true;
        workSheet.Range(iRowIndex, 1, iRowIndex, 1).Style.Fill.BackgroundColor = XLColor.GrannySmithApple;
        iRowIndex++;
        int iColIndex = 1;
        foreach (string item in lHead)
        {
          workSheet.Cell(iRowIndex, iColIndex).Value = item;
          workSheet.Range(iRowIndex, iColIndex, iRowIndex, iColIndex).Style.Fill.BackgroundColor = XLColor.LightGray;
          iColIndex++;
        }

        foreach (DataRow dr in LrtiAlertDel.Rows)
        {
          iRowIndex++;
          for (int i = 1; i <= lHead.Count; i++)
          {
            workSheet.Cell(iRowIndex, i).Value = dr[i - 1].ToString();
          }
        }
      }


      workSheet.Columns().AdjustToContents();

      //存檔
      workBook.SaveAs(fileName);

      return fileName;
    }


    public void LRTIAlertProc()
    {
      try
      {
        //dbDapper.Execute("", new { status = 'D' });
        //刪除已解除註記資料
        ssql = " delete LRTIAlert "
             + " where status = 'D'  "
             ;
        oDal.CommandText = ssql;
        oDal.ExecuteSql();

        //取消狀態註記
        ssql = " update LRTIAlert "
             + " set status = ''  "
             ;
        oDal.CommandText = ssql;
        oDal.ExecuteSql();


        DataTable dt = new DataTable();

        //更新目前警戒中的即時雨量資料
        //取得目前超過警戒值的雨量站
        ssql = " select * from LRTIAlert ";
        oDal.CommandText = ssql;
        dt.Clear();
        dt = oDal.DataTable();
        foreach (DataRow dr in dt.Rows)
        {
          DataTable dt_temp = new DataTable();
          ssql = " select * from RunTimeRainData "
           + " where STID = '" + dr["STID"].ToString() + "' ";
          oDal.CommandText = ssql;
          dt_temp = oDal.DataTable();
          foreach (DataRow dr_temp in dt_temp.Rows)
          {
            string sHOUR3 = dr_temp["HOUR3"].ToString();
            string sHOUR1 = dr_temp["RAIN"].ToString();
            string sHOUR2 = dr_temp["HOUR2"].ToString();
            string sRT = dr_temp["RT"].ToString();
            string sLRTI = dr_temp["LRTI"].ToString();
            //string sELRTI = dr_temp["ELRTI"].ToString();

            if (dr_temp["STATUS"].ToString() == "-99")
            {
              sHOUR3 = "異常";
              sHOUR1 = "異常";
              sHOUR2 = "異常";
              sRT = "異常";
              sLRTI = "異常";
              //sELRTI = "異常";
            }

            ssql = " update LRTIAlert "
                + " set HOUR3 = '" + sHOUR3 + "'  "
                + " , HOUR1 = '" + sHOUR1 + "'  "
                + " , HOUR2 = '" + sHOUR2 + "'  "
                + " , RT = '" + sRT + "'  "
                + " , LRTI = '" + sLRTI + "'  "
                //+ " , ELRTI = '" + sELRTI + "'  "
                + " where STID = '" + dr["STID"].ToString() + "' "
                ;
            oDal.CommandText = ssql;
            oDal.ExecuteSql();
          }
        }

        //取得目前超過警戒值的雨量站
        ssql = " select * from RunTimeRainData a "
             + " left join StationErrLRTI b on a.STID = b.STID "
             + " where CAST(a.LRTI AS decimal(8, 2))  > CAST(b.ELRTI AS decimal(8, 2)) ";
        oDal.CommandText = ssql;
        dt.Clear();
        dt = oDal.DataTable();
        foreach (DataRow dr in dt.Rows)
        {
          //1050715 即時雨量資料異常，則不發報
          if (dr["STATUS"].ToString() == "-99") continue;


          DataTable dt_temp = new DataTable();
          string sSTID = dr["STID"].ToString();

          //判斷狀態
          ssql = " select STID from  LRTIAlert "
                  + " where STID = '" + sSTID + "' "
                  ;
          oDal.CommandText = ssql;
          object oValue = oDal.Value();


          decimal dELRTI = 0;
          decimal.TryParse(dr["ELRTI"].ToString(), out dELRTI);
          dELRTI = Math.Round(dELRTI, 2);
          if (oValue == null)
          {
            //取得雨量站相關資料
            //ssql = " select * from StationData a "
            //     + " left join StationVillageLRTI b on a.STID = b.STID "
            //     + " where a.STID = '" + sSTID + "' "
            //     ;
            //1050715 修改抓警戒資料的鄉鎮資料
            ssql = " select * from StationVillageLRTI "
                 + " where STID = '" + sSTID + "' "
                 ;
            oDal.CommandText = ssql;
            dt_temp = oDal.DataTable();

            foreach (DataRow dr_temp in dt_temp.Rows)
            {
              //寫入警戒雨量站資料，註記新增(I)
              ssql = " insert into LRTIAlert "
              + " values  "
              + " ( "
              + " '" + dr_temp["STID"].ToString() + "' "
              + " ,'" + dr_temp["Country"].ToString() + "' "
              + " ,'" + dr_temp["Town"].ToString() + "' "
              + " ,'" + dr_temp["Village"].ToString() + "' "
              + " ,'I' "
              + " ,'" + dr["HOUR3"].ToString() + "' "
              + " ,'" + dr["RT"].ToString() + "' "
              + " ,'" + dr["LRTI"].ToString() + "' "
              + " ,'" + dELRTI.ToString() + "' "
              + " ,'" + dr["HOUR2"].ToString() + "' "
              + " ,'" + dr["RAIN"].ToString() + "' "
              + " ) "
              ;
              oDal.CommandText = ssql;
              oDal.ExecuteSql();
            }
          }
          else
          {
            //寫入警戒雨量站資料，註記持續(C)
            ssql = " update LRTIAlert "
                + " set status = 'C'  "
                + " , HOUR3 = '" + dr["HOUR3"].ToString() + "'  "
                + " , HOUR1 = '" + dr["RAIN"].ToString() + "'  "
                + " , HOUR2 = '" + dr["HOUR2"].ToString() + "'  "
                + " , RT = '" + dr["RT"].ToString() + "'  "
                + " , LRTI = '" + dr["LRTI"].ToString() + "'  "
                + " , ELRTI = '" + dELRTI.ToString() + "'  "
                + " where STID = '" + sSTID + "' "
                ;
            oDal.CommandText = ssql;
            oDal.ExecuteSql();
          }
        }

        //寫入警戒雨量站資料，註記新增(D)
        ssql = " update LRTIAlert "
             + " set status = 'D' "
             + " where status = '' "
             ;
        oDal.CommandText = ssql;
        oDal.ExecuteSql();
      }
      catch (Exception ex)
      {
        logger.Error(ex, "");

      }
    }



    private void M10AlertLRTI_Load(object sender, EventArgs e)
    {
      //建立資料夾
      if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);

      //取得資料時間
      sLritAlertTimeString = dtNow.ToString("yyyyMMddHHmm");
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      timer1.Enabled = false;

      btnStart_Click(sender, e);

      System.Threading.Thread.Sleep(2000);

      this.Close();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      DataTable dtresult = new DataTable();
      dtresult.Columns.Add("country");
      dtresult.Columns.Add("week");
      dtresult.Columns.Add("car");
      dtresult.Columns.Add("time");

      string sFilepath = @"c:\test.csv";

      //DataTable dt = CSVtoDataTable(sFilepath);



      List<string> ll = new List<string>();

      using (StreamReader SR = new StreamReader(sFilepath))
      {
        string Line;
        while ((Line = SR.ReadLine()) != null)
        {
          if (Line == "") continue;
          if (Line == "\"             車次") continue;
          if (Line == ",,,,,,,,,,,,") continue;
          if (Line == "北上(Northbound),,,,,,,,,,,,") continue;
          if (Line == "台   灣   高   鐵   列   車   時   刻   表,,,,,,,,,,,,") continue;
          if (Line == ",=,=,=,=,=,=,=,=,=,=,,") continue;
          if (Line == "註 : 本時刻表所列各次列車，於起站與中間站均為開車時刻，於終點站為到達時刻。,,,,,,,,,,,,") continue;
          string[] ReadLine_Array = Line.Split(',');
          if (ReadLine_Array[0] == "" && ReadLine_Array[1] == "" && ReadLine_Array[2] == "")
          {
            continue;
          }
          if (ReadLine_Array[0] == "站名\"")
          {
            continue;
          }
          if (Line == ",=,,,,,,,,,,,") continue;
          if (Line == ",,=,,,,,,,,,,") continue;



          ll.Add(Line);
          //MessageBox.Show(Line);

          //string[] ReadLine_Array = Line.Split(',');
          //這邊可以自行發揮
        }
      }


      DataTable dt = new DataTable();

      for (int i = 0; i < ll.Count; i++)
      {
        string[] ReadLine_Array = ll[i].Split(',');

        if (i == 0)
        {
          foreach (string item in ReadLine_Array)
          {
            dt.Columns.Add();
          }
        }


        DataRow newRow = dt.NewRow();
        for (int j = 0; j < dt.Columns.Count; j++)
        {
          newRow[j] = ReadLine_Array[j];
        }
        dt.Rows.Add(newRow);
      }
      dt.Columns.RemoveAt(12);
      dt.Columns.RemoveAt(11);



      List<string> runday = new List<string>();
      List<string> car = new List<string>();

      foreach (DataRow LoopRow in dt.Rows)
      {
        if (LoopRow[0].ToString() == "行駛日")
        {
          runday.Clear();
          for (int i = 0; i < dt.Columns.Count; i++)
          {
            runday.Add(LoopRow[i].ToString());
          }
        }
        if (LoopRow[0].ToString() == "")
        {
          car.Clear();
          for (int i = 0; i < dt.Columns.Count; i++)
          {
            car.Add(LoopRow[i].ToString());
          }
        }

        if (LoopRow[0].ToString() != "行駛日" && LoopRow[0].ToString() != "")
        {
          string sCountry = LoopRow[0].ToString();
          for (int i = 1; i < dt.Columns.Count; i++)
          {
            string scar = car[i];
            string srunday = runday[i];
            if (LoopRow[i].ToString() == "" || LoopRow[i].ToString() == "↓") continue;
            string stime = LoopRow[i].ToString();

            //解析星期
            List<string> lweek = analyweek(srunday);

            foreach (string Loopweek in lweek)
            {
              DataRow NewRow = dtresult.NewRow();
              NewRow["country"] = sCountry;
              NewRow["week"] = Loopweek;
              NewRow["car"] = scar;
              NewRow["time"] = stime;
              dtresult.Rows.Add(NewRow);
            }
          }
        }



      }



      foreach (DataRow insRow in dtresult.Rows)
      {
        ssql = @"INSERT INTO [dbo].[highrail]
           ([country]
           ,[week]
           ,[car]
           ,[time])
            VALUES
           ('{0}'
           ,'{1}'
           ,'{2}'
           ,'{3}')";
        oDal.CommandText = string.Format(ssql, insRow[0].ToString()
          , insRow[1].ToString(), insRow[2].ToString(), insRow[3].ToString());
        oDal.ExecuteSql();
      }



      MessageBox.Show("Test");

    }

    private List<string> analyweek(string sweek)
    {
      List<string> lResult = new List<string>();

      if (sweek == "")
      {
        lResult.Add("1");
        lResult.Add("2");
        lResult.Add("3");
        lResult.Add("4");
        lResult.Add("5");
        lResult.Add("6");
        lResult.Add("7");
      }
      else
      {
        string[] aweek1 = sweek.Split('、');
        foreach (string item in aweek1)
        {
          if (item == "一") lResult.Add("1");
          if (item == "二") lResult.Add("2");
          if (item == "三") lResult.Add("3");
          if (item == "四") lResult.Add("4");
          if (item == "五") lResult.Add("5");
          if (item == "六") lResult.Add("6");
          if (item == "日") lResult.Add("7");

          if (item.Contains("~"))
          {
            string[] aweek2 = item.Split('~');

            int iStart = parseweek(aweek2[0]);
            int iEnd = parseweek(aweek2[1]);

            for (int i = iStart; i <= iEnd; i++)
            {
              lResult.Add(i.ToString());
            }
          }
        }
      }




      return lResult;
    }

    private int parseweek(string sw)
    {
      int ir = 0;

      if (sw == "一") ir = 1;
      if (sw == "二") ir = 2;
      if (sw == "三") ir = 3;
      if (sw == "四") ir = 4;
      if (sw == "五") ir = 5;
      if (sw == "六") ir = 6;
      if (sw == "日") ir = 7;

      return ir;
    }


  }

}
