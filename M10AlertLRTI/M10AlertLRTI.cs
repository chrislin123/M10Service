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
//using DocumentFormat.OpenXml.Wordprocessing; //word
using DocumentFormat.OpenXml.Spreadsheet; //excel
using ClosedXML.Excel;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Configuration;
using M10.lib;
//using Microsoft.CSharp;


namespace M10AlertLRTI
{
  public partial class M10AlertLRTI : Form
  {
    string ssql = string.Empty;
    string sConnectionString = ConfigurationManager.ConnectionStrings[Properties.Settings.Default.vghtc].ConnectionString;
    ODAL oDal = new ODAL(Properties.Settings.Default.vghtc);    
    DALDapper dbDapper;
    
    //ODAL oDal = new ODAL(Properties.Settings.Default.DBConnectionString);

    string folderName = @"D:\m10\LRTIAlert\";

    //string sMainAddress = string.Empty;
    //string sMainPass = string.Empty;
    //string sMailSendList = string.Empty;

    string sLritAlertTimeString = "";

    static private DataTable LrtiAlertAll = new DataTable();
    static private DataTable LrtiAlertNew = new DataTable();
    static private DataTable LrtiAlertDel = new DataTable();


    public M10AlertLRTI()
    {
      InitializeComponent();

      dbDapper = new DALDapper(sConnectionString);
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


      //List<Attachment> oAttachments = new List<Attachment>();
      //oAttachments.Add(new Attachment(sAttachFileName));


      //send_gmail("", "全台村里崩塌警戒提醒" + sLritAlertTimeString, sMailSendList, oAttachments);//呼叫send_gmail函式測試

      //return;

      //LRTIAlertSendMail("");
      //測試
      //oDal.CommandText = " select * from RunTimeRainData ";
      //DataTable TempDataTable = oDal.DataTable();


      //DataExport de = new DataExport();
      //de.RowsPerSheet = 300;
      //de.ExportBigDataToExcel(@"c:\test.xls", TempDataTable);
      //return;



      DateTime dt = DateTime.Now;

      sLritAlertTimeString = Convert.ToString(dt.Year - 1911)
          + Convert.ToString(dt.Month).PadLeft(2, '0')
          + Convert.ToString(dt.Day).PadLeft(2, '0')
          + Convert.ToString(dt.Hour).PadLeft(2, '0')
          + Convert.ToString(dt.Minute).PadLeft(2, '0');

      //紀錄資料更新時間(2017-05-16T10:31:14)
      LRTIAlertUpdateTime(dt);

      //Alert LRTI資料處理
      LRTIAlertProc();

      //AlertLRTI寫入歷史資料
      LRTIAlertRecToHis(dt);

      //取得警戒通知資料
      getLRTIAlertData();

      //1050615 判斷有資料才進行警戒提醒
      if (LrtiAlertAll.Rows.Count == 0 && LrtiAlertNew.Rows.Count == 0 && LrtiAlertDel.Rows.Count == 0) return;

      //文件產生
      //LRTIAlertReport();
      string sAttachFileName = LRTIAlertReport();


      //1050802 判斷是否啟動發報功能
      ssql = " select value from LRTIAlertMail where type = 'isal' and value = 'Y' ";
      oDal.CommandText = ssql;
      DataTable dt1 = oDal.DataTable();

      if (dt1.Rows.Count == 0) return;


      //Alert LRTI寄送發布mail
      LRTIAlertSendMail(sAttachFileName);

      
      //開啟excel
      //OpenExcel(sAttachFileName);

    }

    private void LRTIAlertSendMail(string sAttachFileName)
    {
      string sSubject = "全台村里崩塌警戒提醒" + sLritAlertTimeString;
      List<string> AddressList = new List<string>();

      List<Attachment> AttachmentList = new List<Attachment>();
      //AttachmentList.Add(new Attachment(sAttachFileName));

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

      //寄送Gmail
      Gmail.SendMailByGmail(SenderMail, SenderPass, "", sSubject, AddressList, AttachmentList);
      
      //寄送mail
      //List<Attachment> oAttachments = new List<Attachment>();
      //oAttachments.Add(new Attachment(sAttachFileName));
      

      //send_gmail("", "全台村里崩塌警戒提醒" + sLritAlertTimeString, sMailSendList, oAttachments);//呼叫send_gmail函式測試

    }

    private void LRTIAlertRecToHis(DateTime dt)
    {
      string sDt = dt.ToString("yyyy-MM-ddTHH:mm:ss");

      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(sConnectionString))
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

        //LRTIAlertHis his = new LRTIAlertHis();
        ////his.no = 1;
        //his.country = "test";     

        //using (var cn = new System.Data.SqlClient.SqlConnection(sConnectionString))
        //{
        //  cn.Insert(his);

        //  //cn.Insert<LRTIAlertHis>(his);
        //  //return cn.Query(sql).ToList();
        //}
      }
      catch (Exception )
      {

        //throw ex;
      }

    }

    private void LRTIAlertUpdateTime(DateTime dt)
    {
      string sDt = dt.ToString("yyyy-MM-ddTHH:mm:ss");
      ssql = string.Format(" update LRTIAlertMail set value = '{0}' where type = 'altm' ", sDt);
      oDal.CommandText = ssql;
      oDal.ExecuteSql();
    }

    private void getLRTIAlertData()
    {

      try
      {
        ssql = " select country,town,village,HOUR3,RT,LRTI,ELRTI from LRTIAlert "
             + " where status in ('C','I') "
             + " order by country,town ";
        oDal.CommandText = ssql;
        LrtiAlertAll.Clear();
        LrtiAlertAll = oDal.DataTable();
        ssql = " select country,town,village,HOUR3,RT,LRTI,ELRTI from LRTIAlert "
                 + " where status = 'I' "
                 + " order by country,town ";
        oDal.CommandText = ssql;
        LrtiAlertNew.Clear();
        LrtiAlertNew = oDal.DataTable();
        ssql = " select country,town,village,HOUR3,RT,LRTI,ELRTI from LRTIAlert "
                 + " where status = 'D' "
                 + " order by country,town ";
        oDal.CommandText = ssql;
        LrtiAlertDel.Clear();
        LrtiAlertDel = oDal.DataTable();
      }
      catch (Exception )
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
      catch (Exception )
      {


      }
    }


    public void send_gmail(string msg, string mysubject, string address, List<Attachment> oAttachements)
    {
      //MailMessage message = new MailMessage(sMainAddress, address);//MailMessage(寄信者, 收信者)
      //SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);//設定gmail的smtp
      //try
      //{
      //  message.IsBodyHtml = true;
      //  message.BodyEncoding = System.Text.Encoding.UTF8;//E-mail編碼
      //  message.SubjectEncoding = System.Text.Encoding.UTF8;//E-mail編碼
      //  message.Priority = MailPriority.Normal;//設定優先權
      //  message.Subject = mysubject;//E-mail主旨
      //  message.Body = msg;//E-mail內容

      //  //Attachment attachment = new Attachment(@"C:\hsrv.txt");//<-這是附件部分~先用附件的物件把路徑指定進去~
      //  //message.Attachments.Add(attachment);//<-郵件訊息中加入附件
      //  foreach (Attachment item in oAttachements)
      //  {
      //    message.Attachments.Add(item);
      //  }

      //  MySmtp.Credentials = new System.Net.NetworkCredential(sMainAddress, sMainPass);//gmail的帳號密碼System.Net.NetworkCredential(帳號,密碼)
      //  MySmtp.EnableSsl = true;//開啟ssl
      //  MySmtp.Send(message);

      //  //如果出現權限不足，可登入google帳號後，選擇下方網址，啟用低安全設定
      //  //https://www.google.com/settings/security/lesssecureapps


      //}
      //catch (Exception ex )
      //{

      //}
      //finally
      //{
      //  MySmtp = null;
      //  message.Dispose();
      //}
    }

    private void M10AlertLRTI_Load(object sender, EventArgs e)
    {
      //timer1.Enabled = false;

      //建立資料夾
      if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);

      ////取得寄件者帳號密碼
      //DataTable dtmail = new DataTable();
      //ssql = " select * from LRTIAlertMail "
      //         + " where 1=1 "
      //         + " and type in  ('main','pass') ";
      //oDal.CommandText = ssql;
      //dtmail.Clear();
      //dtmail = oDal.DataTable();
      //foreach (DataRow dr in dtmail.Rows)
      //{
      //  if (dr["type"].ToString() == "main") sMainAddress = dr["value"].ToString();
      //  if (dr["type"].ToString() == "pass") sMainPass = dr["value"].ToString();
      //}


      ////取得收件者            
      //ssql = " select * from LRTIAlertMail "
      //         + " where 1=1 "
      //         + " and type = 'list' ";
      //oDal.CommandText = ssql;
      //dtmail.Clear();
      //dtmail = oDal.DataTable();

      //List<string> lMail = new List<string>();
      //foreach (DataRow dr in dtmail.Rows)
      //{
      //  lMail.Add(dr["value"].ToString());
      //  //sb.Append();
      //}
      //sMailSendList = string.Join(",", lMail);
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      timer1.Enabled = false;

      btnStart_Click(sender, e);

      System.Threading.Thread.Sleep(2000);

      this.Close();
    }
  }

  public class LRTIAlert
  {
    public int no { get; set; }

    public string STID { get; set; }

    public string country { get; set; }

    public string town { get; set; }

    public string village { get; set; }

    public string status { get; set; }

    public string HOUR3 { get; set; }

    public string RT { get; set; }

    public string LRTI { get; set; }

    public string ELRTI { get; set; }

    public string HOUR2 { get; set; }

    public string HOUR1 { get; set; }

  }

  [Table("LRTIAlertHis")]
  public class LRTIAlertHis
  {
    //設定key
    [Key]
    //自動相加key的設定
    //[ExplicitKey]
    public int no { get; set; }

    public string STID { get; set; }

    public string country { get; set; }

    public string town { get; set; }

    public string village { get; set; }

    public string status { get; set; }

    public string RecTime { get; set; }

    public string HOUR3 { get; set; }

    public string RT { get; set; }

    public string LRTI { get; set; }

    public string ELRTI { get; set; }

    public string HOUR2 { get; set; }

    public string HOUR1 { get; set; }

  }
}
