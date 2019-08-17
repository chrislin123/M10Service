using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Mail;
//using CL.Data;
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

        static private List<dynamic> AlertIList = new List<dynamic>();
        static private List<dynamic> AlertCList = new List<dynamic>();
        static private List<dynamic> AlertOList = new List<dynamic>();
        static private List<dynamic> AlertDList = new List<dynamic>();


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
            try
            {



                //int iii = Convert.ToInt32("");

                //DateTime dd = Convert.ToDateTime("2017-05-16T10:31:14");

                //UpdateLRTIAlertRainData("");

                //紀錄資料更新時間(2017-05-16T10:31:14)
                LRTIAlertUpdateTime();

                //Alert LRTI資料處理
                //LRTIAlertProc();
                LRTIAlertProc_108();

                //AlertLRTI寫入歷史資料
                LRTIAlertRecToHis();

                //取得警戒通知資料
                //getLRTIAlertData();
                GetLRTIAlertData_108();

                //1050615 判斷有資料才進行警戒提醒
                if (AlertIList.Count == 0
                  && AlertCList.Count == 0
                  && AlertOList.Count == 0
                  && AlertDList.Count == 0) return;

                //文件產生      
                string sAttachFileName = LRTIAlertReport();

                //1060519 判斷是否啟動發報功能，修改使用dapper
                var chkMailFlag = dbDapper.ExecuteScale("select value from LRTIAlertMail where type = 'isal' and value = 'Y'");
                if (string.IsNullOrEmpty(chkMailFlag as string) == false)
                {
                    //Alert LRTI寄送發布mail
                    LRTIAlertSendMail(sAttachFileName);
                }



                //開啟excel
                //OpenExcel(sAttachFileName);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "M10AlertLRTI-btnStart_Click");
            }
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
                        oHis.HOUR6 = item.HOUR6;
                        oHis.LRTI = item.LRTI;
                        oHis.RT = item.RT;
                        oHis.status = item.status;
                        oHis.STID = item.STID;
                        oHis.town = item.town;
                        oHis.village = item.village;
                        oHis.RecTime = sDt;
                        oHis.nowwarm = item.nowwarm;
                        oHis.statustime = item.statustime;
                        oHis.villageid = item.villageid;
                        oHis.ReleaseRTime = item.ReleaseRTime;

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
            ssql = " select * from LRTIAlertMail where type = 'altm' ";
            LRTIAlertMail item = dbDapper.QuerySingleOrDefault<LRTIAlertMail>(ssql);
            if (item != null)
            {
                item.value = Utils.getDatatimeString(dtNow, M10Const.DatetimeStringType.ADDT2);
                dbDapper.Update(item);
            }

        }

        private void getLRTIAlertData()
        {
            try
            {
                ssql = " select country,town,village,HOUR1,HOUR2,HOUR3,RT,LRTI,ELRTI,status,statustime from LRTIAlert "
                     + " where status in ('{0}') "
                     + " order by country,town ";


                AlertIList = dbDapper.Query(string.Format(ssql, "I"));
                AlertCList = dbDapper.Query(string.Format(ssql, "C"));
                AlertOList = dbDapper.Query(string.Format(ssql, "O"));
                AlertDList = dbDapper.Query(string.Format(ssql, "D"));
                foreach (var item in AlertIList)
                {
                    if (item.status == "I") item.status = M10Const.AlertStatus.I;
                    if (item.status == "C") item.status = M10Const.AlertStatus.C;
                    if (item.status == "O") item.status = M10Const.AlertStatus.O;
                    if (item.status == "D") item.status = M10Const.AlertStatus.D;
                }
                foreach (var item in AlertCList)
                {
                    if (item.status == "I") item.status = M10Const.AlertStatus.I;
                    if (item.status == "C") item.status = M10Const.AlertStatus.C;
                    if (item.status == "O") item.status = M10Const.AlertStatus.O;
                    if (item.status == "D") item.status = M10Const.AlertStatus.D;
                }
                foreach (var item in AlertOList)
                {
                    if (item.status == "I") item.status = M10Const.AlertStatus.I;
                    if (item.status == "C") item.status = M10Const.AlertStatus.C;
                    if (item.status == "O") item.status = M10Const.AlertStatus.O;
                    if (item.status == "D") item.status = M10Const.AlertStatus.D;
                }
                foreach (var item in AlertDList)
                {
                    if (item.status == "I") item.status = M10Const.AlertStatus.I;
                    if (item.status == "C") item.status = M10Const.AlertStatus.C;
                    if (item.status == "O") item.status = M10Const.AlertStatus.O;
                    if (item.status == "D") item.status = M10Const.AlertStatus.D;
                }


                //oDal.CommandText = string.Format(ssql,"I");
                //LrtiAlertI.Clear();
                //LrtiAlertI = oDal.DataTable();       

                //oDal.CommandText = string.Format(ssql, "C");
                //LrtiAlertC.Clear();
                //LrtiAlertC = oDal.DataTable();

                //oDal.CommandText = string.Format(ssql, "O");
                //LrtiAlertO.Clear();
                //LrtiAlertO = oDal.DataTable();

                //oDal.CommandText = string.Format(ssql, "D");
                //LrtiAlertD.Clear();
                //LrtiAlertD = oDal.DataTable();


                //foreach (DataRow dr in LrtiAlertI.Rows)
                //{
                //  if (dr["status"].ToString() == "I") dr["status"] = Constants.AlertStatus.I;
                //  if (dr["status"].ToString() == "C") dr["status"] = Constants.AlertStatus.C;
                //  if (dr["status"].ToString() == "O") dr["status"] = Constants.AlertStatus.O;
                //  if (dr["status"].ToString() == "D") dr["status"] = Constants.AlertStatus.D;
                //}
                //foreach (DataRow dr in LrtiAlertC.Rows)
                //{
                //  if (dr["status"].ToString() == "I") dr["status"] = Constants.AlertStatus.I;
                //  if (dr["status"].ToString() == "C") dr["status"] = Constants.AlertStatus.C;
                //  if (dr["status"].ToString() == "O") dr["status"] = Constants.AlertStatus.O;
                //  if (dr["status"].ToString() == "D") dr["status"] = Constants.AlertStatus.D;
                //}
                //foreach (DataRow dr in LrtiAlertO.Rows)
                //{
                //  if (dr["status"].ToString() == "I") dr["status"] = Constants.AlertStatus.I;
                //  if (dr["status"].ToString() == "C") dr["status"] = Constants.AlertStatus.C;
                //  if (dr["status"].ToString() == "O") dr["status"] = Constants.AlertStatus.O;
                //  if (dr["status"].ToString() == "D") dr["status"] = Constants.AlertStatus.D;
                //}
                //foreach (DataRow dr in LrtiAlertD.Rows)
                //{
                //  if (dr["status"].ToString() == "I") dr["status"] = Constants.AlertStatus.I;
                //  if (dr["status"].ToString() == "C") dr["status"] = Constants.AlertStatus.C;
                //  if (dr["status"].ToString() == "O") dr["status"] = Constants.AlertStatus.O;
                //  if (dr["status"].ToString() == "D") dr["status"] = Constants.AlertStatus.D;
                //}

            }
            catch (Exception)
            {

            }


            //1060614 異動狀態
            //try
            //{
            //  ssql = " select country,town,village,HOUR1,HOUR2,HOUR3,RT,LRTI,ELRTI from LRTIAlert "
            //       + " where status in ('C','I') "
            //       + " order by country,town ";
            //  oDal.CommandText = ssql;
            //  LrtiAlertAll.Clear();
            //  LrtiAlertAll = oDal.DataTable();
            //  ssql = " select country,town,village,HOUR1,HOUR2,HOUR3,RT,LRTI,ELRTI from LRTIAlert "
            //           + " where status = 'I' "
            //           + " order by country,town ";
            //  oDal.CommandText = ssql;
            //  LrtiAlertNew.Clear();
            //  LrtiAlertNew = oDal.DataTable();
            //  ssql = " select country,town,village,HOUR1,HOUR2,HOUR3,RT,LRTI,ELRTI from LRTIAlert "
            //           + " where status = 'D' "
            //           + " order by country,town ";
            //  oDal.CommandText = ssql;
            //  LrtiAlertDel.Clear();
            //  LrtiAlertDel = oDal.DataTable();
            //}
            //catch (Exception)
            //{

            //}
        }

        private void GetLRTIAlertData_108()
        {
            try
            {
                ssql = " select country,town,village,HOUR1,HOUR2,HOUR3,RT,LRTI,ELRTI,status,statustime from LRTIAlert "
                     + " where status in ('{0}') "
                     + " order by country,town ";


                AlertIList = dbDapper.Query(string.Format(ssql, "A1"));
                AlertCList = dbDapper.Query(string.Format(ssql, "A2"));
                AlertOList = dbDapper.Query(string.Format(ssql, "A3"));
                AlertDList = dbDapper.Query(string.Format(ssql, "AD"));
                foreach (var item in AlertIList)
                {
                    item.status = M10Const.AlertStatus.A1;
                    //if (item.status == "I") 
                    //if (item.status == "C") item.status = M10Const.AlertStatus.C;
                    //if (item.status == "O") item.status = M10Const.AlertStatus.O;
                    //if (item.status == "D") item.status = M10Const.AlertStatus.D;
                }
                foreach (var item in AlertCList)
                {
                    item.status = M10Const.AlertStatus.A2;
                    //if (item.status == "I") item.status = M10Const.AlertStatus.I;
                    //if (item.status == "C") item.status = M10Const.AlertStatus.C;
                    //if (item.status == "O") item.status = M10Const.AlertStatus.O;
                    //if (item.status == "D") item.status = M10Const.AlertStatus.D;
                }
                foreach (var item in AlertOList)
                {
                    item.status = M10Const.AlertStatus.A3;
                    //if (item.status == "I") item.status = M10Const.AlertStatus.I;
                    //if (item.status == "C") item.status = M10Const.AlertStatus.C;
                    //if (item.status == "O") item.status = M10Const.AlertStatus.O;
                    //if (item.status == "D") item.status = M10Const.AlertStatus.D;
                }
                foreach (var item in AlertDList)
                {
                    item.status = M10Const.AlertStatus.AD;
                    //if (item.status == "I") item.status = M10Const.AlertStatus.I;
                    //if (item.status == "C") item.status = M10Const.AlertStatus.C;
                    //if (item.status == "O") item.status = M10Const.AlertStatus.O;
                    //if (item.status == "D") item.status = M10Const.AlertStatus.D;
                }

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
            lHead.Add("警戒發布狀態");
            lHead.Add("警戒發布時間");

            workSheet.Cell(iRowIndex, 1).Value = sLrtiTime;
            workSheet.Range(iRowIndex, 1, iRowIndex, 1).Style.Font.Bold = true;

            //預警明細
            if (AlertIList.Count > 0)
            {
                iRowIndex++;
                workSheet.Cell(iRowIndex, 1).Value = "崩塌警戒區域 黃色 明細";
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

                foreach (var Item in AlertIList)
                {
                    iRowIndex++;
                    for (int i = 0; i < lHead.Count; i++)
                    {
                        workSheet.Cell(iRowIndex, i + 1).SetValue<string>(getCellValue(lHead[i], Item));
                    }
                }
            }

            //警戒明細
            if (AlertCList.Count > 0)
            {
                iRowIndex++;
                workSheet.Cell(iRowIndex, 1).Value = "崩塌警戒區域 橙色 明細";
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

                foreach (var Item in AlertCList)
                {
                    iRowIndex++;
                    for (int i = 0; i < lHead.Count; i++)
                    {
                        workSheet.Cell(iRowIndex, i + 1).SetValue<string>(getCellValue(lHead[i], Item));
                    }
                }
            }

            //警戒調降明細
            if (AlertOList.Count > 0)
            {
                iRowIndex++;
                workSheet.Cell(iRowIndex, 1).Value = "崩塌警戒區域 紅色 明細";
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

                foreach (var Item in AlertOList)
                {
                    iRowIndex++;
                    for (int i = 0; i < lHead.Count; i++)
                    {
                        workSheet.Cell(iRowIndex, i + 1).SetValue<string>(getCellValue(lHead[i], Item));
                    }
                }
            }

            //解除警戒明細
            if (AlertDList.Count > 0)
            {
                iRowIndex++;
                workSheet.Cell(iRowIndex, 1).Value = "崩塌警戒區域 解除警戒 明細";
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

                foreach (var Item in AlertDList)
                {
                    iRowIndex++;
                    for (int i = 0; i < lHead.Count; i++)
                    {
                        workSheet.Cell(iRowIndex, i + 1).SetValue<string>(getCellValue(lHead[i], Item));
                    }
                }
            }

            //寬度調整至資料大小
            workSheet.Columns().AdjustToContents();

            //存檔
            workBook.SaveAs(fileName);

            return fileName;
        }

        private string getCellValue(string sHead, dynamic Item)
        {
            string sResult = "";

            try
            {
                if (sHead == "縣市") sResult = Item.country;
                if (sHead == "鄉鎮區") sResult = Item.town;
                if (sHead == "警戒區範圍") sResult = Item.village;
                if (sHead == "1hr") sResult = Item.HOUR1;
                if (sHead == "2hr") sResult = Item.HOUR2;
                if (sHead == "3hr") sResult = Item.HOUR3;
                if (sHead == "Rt") sResult = Item.RT;
                if (sHead == "LRTI") sResult = Item.LRTI;
                if (sHead == "警戒LRTI") sResult = Item.ELRTI;
                if (sHead == "警戒發布狀態") sResult = Item.status;
                if (sHead == "警戒發布時間") sResult = Item.statustime;
            }
            catch (Exception)
            {
                sResult = "";
            }

            return sResult;
        }

        /// <summary>
        /// 106版本的崩塌警戒
        /// </summary>
        public void LRTIAlertProc()
        {
            //1060614
            //I(預警)=實際雨量已達崩塌警戒門檻
            //(無警戒狀態中)LRTI 大於 警戒LRTI為發布黃色
            //新增
            //C(警戒)=實際降雨已達崩塌警戒門檻，且連續3小時
            //(黃色狀態中)LRTI 連續大於 警戒LRTI三小時，第四小時為黃升紅(發布紅色)
            //判斷歷史已存在兩筆
            //O(警戒調降)=實際降雨低於崩塌警戒門檻，且連續3小時
            //(紅色狀態中)LRTI 連續小於 警戒LRTI三小時，第四小時為紅降黃(解除紅色)
            //判斷歷史資料三小時內沒警戒
            //D(解除警戒)=實際降雨低於崩塌警戒門檻，且連續6小時
            //(黃色狀態中)LRTI 連續大於 警戒LRTI三小時，第四小時為解除黃色
            //判斷歷史資料六小時內沒警戒

            string sDt = dtNow.ToString("yyyy-MM-ddTHH:mm:ss");
            string sDtd35 = dtNow.AddHours(-3.5).ToString("yyyy-MM-ddTHH:mm:ss");
            string sDtd65 = dtNow.AddHours(-6.5).ToString("yyyy-MM-ddTHH:mm:ss");

            string mark = "start";
            try
            {
                //刪除已解除註記資料
                dbDapper.Execute(" delete LRTIAlert where status = 'D' ");

                //取消狀態註記
                dbDapper.Execute(" update LRTIAlert set statuscheck = 'N' ");

                //判斷狀態的前置處理             
                ssql = " select * from LRTIAlert ";
                List<LRTIAlert> AlertList = dbDapper.Query<LRTIAlert>(ssql);
                foreach (LRTIAlert AlertItem in AlertList)
                {
                    string sSTID = AlertItem.STID;

                    ssql = " select * from RunTimeRainData where STID = '" + sSTID + "' ";

                    mark = "start1";
                    RunTimeRainData RuntimeData = dbDapper.QuerySingleOrDefault<RunTimeRainData>(ssql);
                    if (RuntimeData != null)
                    {
                        if (RuntimeData.STATUS == "-99")
                        {
                            AlertItem.HOUR1 = "異常";
                            AlertItem.HOUR2 = "異常";
                            AlertItem.HOUR3 = "異常";
                            AlertItem.RT = "異常";
                            AlertItem.LRTI = "異常";
                        }
                        else
                        {
                            AlertItem.HOUR1 = RuntimeData.RAIN;
                            AlertItem.HOUR2 = RuntimeData.HOUR2;
                            AlertItem.HOUR3 = RuntimeData.HOUR3;
                            AlertItem.RT = RuntimeData.RT;
                            AlertItem.LRTI = RuntimeData.LRTI;
                        }
                    }

                    //更新目前警戒中的警戒狀態
                    ssql = @" select * from RunTimeRainData a 
                    left join (select distinct stid,elrti from lrtibasic where type = 'now') b on a.STID = b.STID 
                    where CAST(a.LRTI AS decimal(8, 2))  > CAST(b.ELRTI AS decimal(8, 2)) 
                    and a.STID = '{0}' ";
                    int iCount = dbDapper.QueryTotalCount(string.Format(ssql, sSTID));
                    if (iCount == 0) //低於警戒
                    {
                        AlertItem.nowwarm = "N";
                    }
                    else //高於警戒
                    {
                        AlertItem.nowwarm = "Y";
                    }

                    //更新
                    dbDapper.Update<LRTIAlert>(AlertItem);
                }

                mark = "start2";
                string sqlQuery = " select * from LRTIAlert where status = '{0}' and statuscheck = 'N' ";
                string sqlCheck = @" select distinct RecTime from LRTIAlertHis 
                      where nowwarm = 'Y' and stid = '{0}' and RecTime > '{1}' ";
                //處理狀態(I)
                AlertList.Clear();
                AlertList = dbDapper.Query<LRTIAlert>(string.Format(sqlQuery, "I"));
                foreach (LRTIAlert AlertItem in AlertList)
                {
                    string sStstus = "";

                    if (AlertItem.nowwarm == "Y") //現在高於警戒
                    {
                        //判斷歷史資料三小時(用3.5小時切)是否超過兩筆
                        int iTemp = dbDapper.QueryTotalCount(string.Format(sqlCheck, AlertItem.STID, sDtd35));
                        if (iTemp >= 3) //變更狀態(I預警=>C警戒)
                        {
                            sStstus = "C";
                        }
                    }

                    if (AlertItem.nowwarm == "N") //現在低於警戒
                    {
                        //判斷歷史資料三小時(用3.5小時切)皆沒警戒資料
                        int iTemp = dbDapper.QueryTotalCount(string.Format(sqlCheck, AlertItem.STID, sDtd35));
                        if (iTemp == 0) //變更狀態(I預警=>D解除警戒)
                        {
                            sStstus = "D";
                        }
                    }

                    AlertItem.statuscheck = "Y";
                    if (sStstus != "")
                    {
                        AlertItem.status = sStstus;
                        AlertItem.statustime = sDt;
                    }

                    //更新
                    dbDapper.Update<LRTIAlert>(AlertItem);
                }

                mark = "start3";
                //處理狀態(C)
                AlertList.Clear();
                AlertList = dbDapper.Query<LRTIAlert>(string.Format(sqlQuery, "C"));
                foreach (LRTIAlert AlertItem in AlertList)
                {
                    string sStstus = "";

                    if (AlertItem.nowwarm == "N") //現在低於警戒
                    {
                        //判斷歷史資料三小時(用3.5小時切)皆沒警戒資料
                        int iTemp = dbDapper.QueryTotalCount(string.Format(sqlCheck, AlertItem.STID, sDtd35));
                        if (iTemp == 0) //變更狀態(C預警=>O警戒調降)
                        {
                            sStstus = "O";
                        }
                    }

                    AlertItem.statuscheck = "Y";
                    if (sStstus != "")
                    {
                        AlertItem.status = sStstus;
                        AlertItem.statustime = sDt;
                    }

                    //更新
                    dbDapper.Update<LRTIAlert>(AlertItem);
                }

                mark = "start4";
                //處理狀態(O)
                AlertList.Clear();
                AlertList = dbDapper.Query<LRTIAlert>(string.Format(sqlQuery, "O"));
                foreach (LRTIAlert AlertItem in AlertList)
                {
                    string sStstus = "";

                    if (AlertItem.nowwarm == "Y") //現在高於警戒
                    {
                        //判斷歷史資料三小時(用3.5小時切)是否超過兩筆
                        int iTemp = dbDapper.QueryTotalCount(string.Format(sqlCheck, AlertItem.STID, sDtd35));
                        if (iTemp >= 3) //變更狀態(O警戒調降=>C警戒)
                        {
                            sStstus = "C";
                        }
                    }

                    if (AlertItem.nowwarm == "N") //現在低於警戒
                    {
                        //判斷歷史資料三小時(用6.5小時切)皆沒警戒資料
                        int iTemp = dbDapper.QueryTotalCount(string.Format(sqlCheck, AlertItem.STID, sDtd65));
                        if (iTemp == 0) //變更狀態(O警戒調降=>D解除警戒)
                        {
                            sStstus = "D";
                        }
                    }

                    AlertItem.statuscheck = "Y";
                    if (sStstus != "")
                    {
                        AlertItem.status = sStstus;
                        AlertItem.statustime = sDt;
                    }

                    //更新
                    dbDapper.Update<LRTIAlert>(AlertItem);
                }

                mark = "start5";
                //處理狀態(新案)
                //取得目前超過警戒值的雨量站
                ssql = @" select * from RunTimeRainData a 
                  left join (select distinct stid,elrti from lrtibasic where type = 'now') b on a.STID = b.STID 
                  where CAST(a.LRTI AS decimal(8, 2))  > CAST(b.ELRTI AS decimal(8, 2)) ";
                List<dynamic> RuntimeList = dbDapper.Query(ssql);

                foreach (var RumtimeItem in RuntimeList)
                {
                    if (RumtimeItem.STATUS == "-99") continue;

                    mark = "start6";
                    //判斷資料是否已存在
                    ssql = " select STID from  LRTIAlert where STID = '{0}' ";
                    int iCount = dbDapper.QueryTotalCount(string.Format(ssql, RumtimeItem.STID));
                    if (iCount > 0) continue;

                    decimal dELRTI = 0;
                    decimal.TryParse(RumtimeItem.elrti, out dELRTI);
                    dELRTI = Math.Round(dELRTI, 2);

                    mark = "start7";
                    ssql = " select * from lrtibasic where STID = '{0}' and type = 'now' ";
                    List<LrtiBasic> LrtiBasicList = dbDapper.Query<LrtiBasic>(string.Format(ssql, RumtimeItem.STID));
                    foreach (LrtiBasic LrtiBasicItem in LrtiBasicList)
                    {
                        ssql = " select * from areacode where villageid = '{0}' ";
                        AreaCode AreaCodeItem =
                          dbDapper.QuerySingleOrDefault<AreaCode>(string.Format(ssql, LrtiBasicItem.villageid));

                        if (AreaCodeItem == null) AreaCodeItem = new AreaCode();

                        LRTIAlert AlertItem = new LRTIAlert();
                        AlertItem.STID = RumtimeItem.STID;
                        AlertItem.country = AreaCodeItem.CountryName;
                        AlertItem.town = AreaCodeItem.TownName;
                        AlertItem.village = AreaCodeItem.VillageName;
                        AlertItem.status = "I";
                        AlertItem.HOUR3 = RumtimeItem.HOUR3;
                        AlertItem.RT = RumtimeItem.RT;
                        AlertItem.LRTI = RumtimeItem.LRTI;
                        AlertItem.ELRTI = dELRTI.ToString();
                        AlertItem.HOUR2 = RumtimeItem.Hour2;
                        AlertItem.HOUR1 = RumtimeItem.RAIN;
                        AlertItem.nowwarm = "Y";
                        AlertItem.statustime = sDt;
                        AlertItem.statuscheck = "Y";
                        AlertItem.villageid = LrtiBasicItem.villageid;

                        mark = "start8";
                        dbDapper.Insert(AlertItem);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, mark);
            }
        }

        /// <summary>
        /// 108版本的崩塌警戒
        /// </summary>
        public void LRTIAlertProc_108()
        {
            //106 狀態 I C O D

            //1080612 狀態另外建立，避免與106版本重複，在歷史資料可以做出區隔
            //警戒4種狀態:黃色A1、橙色A2、紅色A3、解除AD
            //1.警戒發布門檻異動(詳附檔「全臺崩塌警戒基準值1080603_系統用」)
            //a.土石流紅色警戒發布門檻：詳附檔G欄位
            //若欄位為”該村里無潛勢溪流”表示該村里無需發布土石流警戒
            //b.淺層崩塌警戒發布門檻：詳附檔H、I、J、K欄位
            //淺層黃色警戒發布門檻：附檔K(3小時累積雨量) & J(有效累積雨量RT)欄位
            //  淺層橙色警戒發布門檻：附檔I(3小時累積雨量) & H(有效累積雨量RT)欄位
            //2.淺層黃色警戒發布條件
            //降雨時K(3小時累積雨量)欄位 & J(有效累積雨量RT)欄位同時達到門檻
            //3.淺層橙色警戒發布條件
            //降雨時I(3小時累積雨量)欄為 & H(有效累積雨量RT)欄位同時達到門檻
            //4.土石流紅色警戒發布條件
            //降雨時3小時累積雨量 > 30mm，且有效累積雨量RT > G欄位
            //5.不同村里警戒發布差異
            //a.有崩塌及土石流存在村里，可能3種顏色都會發布到
            //b.僅有崩塌村里，通常只會發布黃色、橙色
            //6.警戒發布狀態修正
            //發布記錄裡發部狀態簡化為只要呈現黃色、橙色、紅色、解除警戒4種狀態，不需要像之前有解除黃色、提升紅色等字樣。
            //7.警戒發布層級說明
            //紅色 > 橙色 > 黃色，當某區域發布紅色警戒後，要往下降至黃色警戒，則需要參考以下解除標準。
            //三、	警戒解除作業
            //發布警戒可能有3階段，但是調降警戒採用2階段
            //1.警戒調降 / 解除標準
            //6小時累積雨量≦24mm，且這6小時內的所有小時雨量≦10mm。
            //2.橙色及紅色警戒調降至黃色
            //降雨條件符合前述1的標準時，紅色警戒 => 黃色警戒；橙色警戒 => 黃色警戒。
            //3.黃色警戒解除
            //降雨條件符合前述1的標準時，黃色警戒 => 解除警戒。
            //4.警戒解除之狀態異動頻率為3小時
            //警戒狀態判斷頻率雖然仍可維持每小時判斷，但警戒狀態調整需至少間隔3小時，如06: 00警戒狀態由紅色降為黃色，下次狀態要調整為解除警戒則至少要致09: 00。


            string sDt = dtNow.ToString("yyyy-MM-ddTHH:mm:ss");
            string sDtd35 = dtNow.AddHours(-3.5).ToString("yyyy-MM-ddTHH:mm:ss");
            string sDtd65 = dtNow.AddHours(-6.5).ToString("yyyy-MM-ddTHH:mm:ss");


            try
            {
                ////刪除已解除註記資料
                dbDapper.Execute(" delete LRTIAlert where status = 'AD' ");

                ////取消狀態註記
                dbDapper.Execute(" update LRTIAlert set statuscheck = 'N' ");

                ////判斷狀態前，更新即時資料
                ssql = " select * from LRTIAlert ";
                List<LRTIAlert> AlertList = dbDapper.Query<LRTIAlert>(ssql);
                foreach (LRTIAlert AlertItem in AlertList)
                {

                    LRTIAlert LRTIAlertItem = AlertItem;
                    //1080614 有狀態異動才改變數值
                    string sSTID = AlertItem.STID;

                    ssql = " select * from RunTimeRainData where STID = '" + sSTID + "' ";
                    RunTimeRainData RuntimeData = dbDapper.QuerySingleOrDefault<RunTimeRainData>(ssql);
                    if (RuntimeData != null)
                    {
                        if (RuntimeData.STATUS == "-99")
                        {
                            AlertItem.HOUR1 = "異常";
                            AlertItem.HOUR2 = "異常";
                            AlertItem.HOUR3 = "異常";
                            AlertItem.HOUR6 = "異常";
                            AlertItem.RT = "異常";
                            AlertItem.LRTI = "異常";
                        }
                        else
                        {
                            AlertItem.HOUR1 = RuntimeData.RAIN;
                            AlertItem.HOUR2 = RuntimeData.HOUR2;
                            AlertItem.HOUR3 = RuntimeData.HOUR3;
                            AlertItem.HOUR6 = RuntimeData.HOUR6;
                            AlertItem.RT = RuntimeData.RT;
                            AlertItem.LRTI = RuntimeData.LRTI;
                        }
                    }


                    //判斷註記解除及調升
                    string sLRTIAlertStatus = CheckLRTIAlertStatus_108(LRTIAlertItem.villageid);
                    if (sLRTIAlertStatus == "") //解除註記(還沒正式解除警戒)
                    {
                        LRTIAlertItem.nowwarm = "Y";
                        //更新
                        dbDapper.Update<LRTIAlert>(LRTIAlertItem);
                    }
                    else //持續警戒
                    {
                        //判斷黃色(A1)調升為橙色(A2)或紅色(A3)
                        if (LRTIAlertItem.status == "A1" && sLRTIAlertStatus != "A1")
                        {
                            LRTIAlertItem.status = sLRTIAlertStatus;
                            //UpdateLRTIAlertRainData(ref LRTIAlertItem);
                        }

                        //判斷橙色(A2)調升為紅色(A3)
                        if (LRTIAlertItem.status == "A2" && sLRTIAlertStatus == "A3")
                        {
                            LRTIAlertItem.status = sLRTIAlertStatus;
                            //UpdateLRTIAlertRainData(ref LRTIAlertItem);
                        }

                        LRTIAlertItem.nowwarm = "Y";
                        //更新
                        dbDapper.Update<LRTIAlert>(LRTIAlertItem);
                    }
                }


                string sqlQuery = " select * from LRTIAlert where status = '{0}' and statuscheck = 'N' ";
                string sqlCheck = @" select distinct RecTime from LRTIAlertHis 
                      where nowwarm = 'Y' and villageID = '{0}' and RecTime > '{1}' and status = '{2}' ";
                //處理狀態黃色(A1)
                AlertList.Clear();
                AlertList = dbDapper.Query<LRTIAlert>(string.Format(sqlQuery, "A1"));
                foreach (LRTIAlert AlertItem in AlertList)
                {
                    LRTIAlert LRTIAlertItem = AlertItem;
                    string sStstus = "";

                    //同一狀態持續三個小時以上發布
                    if (LRTIAlertItem.nowwarm == "Y")
                    {
                        //判斷歷史資料三小時(用3.5小時切)同一個狀態維持三個小時以上
                        int iTemp = dbDapper.QueryTotalCount(string.Format(sqlCheck, LRTIAlertItem.villageid, sDtd35, "A1"));
                        if (iTemp >= 3)
                        {
                            //判斷是否符合調降規則，如果符合變更狀態(黃色(A1)=>解除警戒(AD))
                            if (CheckLRTIAlertStatusDowngrade_108(LRTIAlertItem.villageid) == true)
                            {
                                sStstus = "AD";
                            }
                        }
                    }

                    LRTIAlertItem.statuscheck = "Y";
                    if (sStstus != "")
                    {
                        LRTIAlertItem.status = sStstus;
                        LRTIAlertItem.nowwarm = "N";
                        LRTIAlertItem.statustime = sDt;
                        //UpdateLRTIAlertRainData(ref LRTIAlertItem);
                    }

                    //更新
                    dbDapper.Update<LRTIAlert>(LRTIAlertItem);
                }


                //處理狀態澄色(A2)
                AlertList.Clear();
                AlertList = dbDapper.Query<LRTIAlert>(string.Format(sqlQuery, "A2"));
                foreach (LRTIAlert AlertItem in AlertList)
                {
                    LRTIAlert LRTIAlertItem = AlertItem;
                    string sStstus = "";

                    //同一狀態持續三個小時以上發布
                    if (LRTIAlertItem.nowwarm == "Y")
                    {
                        //判斷歷史資料三小時(用3.5小時切)同一個狀態維持三個小時以上
                        int iTemp = dbDapper.QueryTotalCount(string.Format(sqlCheck, LRTIAlertItem.villageid, sDtd35, "A2"));
                        if (iTemp >= 3)
                        {
                            //判斷是否符合調降規則，如果符合變更狀態(澄色(A2)=>黃色(A1))
                            if (CheckLRTIAlertStatusDowngrade_108(LRTIAlertItem.villageid) == true)
                            {
                                sStstus = "A1";
                            }
                        }
                    }

                    LRTIAlertItem.statuscheck = "Y";
                    if (sStstus != "")
                    {
                        LRTIAlertItem.status = sStstus;
                        LRTIAlertItem.statustime = sDt;
                        //UpdateLRTIAlertRainData(ref LRTIAlertItem);
                    }

                    //更新
                    dbDapper.Update<LRTIAlert>(LRTIAlertItem);
                }

                //處理狀態紅色(A3)
                AlertList.Clear();
                AlertList = dbDapper.Query<LRTIAlert>(string.Format(sqlQuery, "A3"));
                foreach (LRTIAlert AlertItem in AlertList)
                {
                    string sStstus = "";
                    LRTIAlert LRTIAlertItem = AlertItem;

                    //同一狀態持續三個小時以上發布
                    if (LRTIAlertItem.nowwarm == "Y")
                    {
                        //判斷歷史資料三小時(用3.5小時切)同一個狀態維持三個小時以上
                        int iTemp = dbDapper.QueryTotalCount(string.Format(sqlCheck, LRTIAlertItem.villageid, sDtd35, "A3"));
                        if (iTemp >= 3)
                        {
                            //判斷是否符合調降規則，如果符合變更狀態(紅色(A3)=>黃色(A1))
                            if (CheckLRTIAlertStatusDowngrade_108(LRTIAlertItem.villageid) == true)
                            {
                                sStstus = "A1";
                            }
                        }
                    }

                    LRTIAlertItem.statuscheck = "Y";
                    if (sStstus != "")
                    {
                        LRTIAlertItem.status = sStstus;
                        LRTIAlertItem.statustime = sDt;
                        //UpdateLRTIAlertRainData(ref LRTIAlertItem);
                    }

                    //更新
                    dbDapper.Update<LRTIAlert>(LRTIAlertItem);
                }

                //處理狀態(新案)
                //黃色A1、橙色A2、紅色A3
                //依照條件進行發布
                //使用成大提供[全臺崩塌警戒基準值_系統]用村里做主Key串接即時雨量資料進行發布
                ssql = @" 
                        select * from  LRTIAlertRefData   
                        left join RunTimeRainData on LRTIAlertRefData.STID = RunTimeRainData.STID
                        where LRTIAlertRefData.ver = 'now'
                        ";
                List<dynamic> RuntimeList = dbDapper.Query(ssql);

                foreach (var RumtimeItem in RuntimeList)
                {
                    //測試
                    if (RumtimeItem.STID == "01A200")
                    {
                        string ssss = string.Empty;
                    }

                    //表示資料異常不進行判斷
                    if (RumtimeItem.STATUS == "-99") continue;


                    //判斷該村里是否已經在發布狀態
                    ssql = " select villageID from  LRTIAlert where villageID = '{0}' ";
                    int iCount = dbDapper.QueryTotalCount(string.Format(ssql, RumtimeItem.villageID));
                    if (iCount > 0) continue;

                    string sLRTIAlertStatus = CheckLRTIAlertStatus_108(RumtimeItem.villageID);

                    if (sLRTIAlertStatus != "")
                    {
                        LRTIAlert AlertItem = new LRTIAlert();
                        AlertItem.STID = RumtimeItem.STID;
                        AlertItem.country = RumtimeItem.country;
                        AlertItem.town = RumtimeItem.town;
                        AlertItem.village = RumtimeItem.village;
                        AlertItem.status = sLRTIAlertStatus;
                        AlertItem.HOUR1 = RumtimeItem.RAIN;
                        AlertItem.HOUR2 = RumtimeItem.Hour2;
                        AlertItem.HOUR3 = RumtimeItem.HOUR3;
                        AlertItem.HOUR6 = RumtimeItem.HOUR6;
                        AlertItem.RT = RumtimeItem.RT;
                        AlertItem.LRTI = RumtimeItem.LRTI;
                        AlertItem.ELRTI = ""; //確認是否需要存在                        
                        AlertItem.nowwarm = "Y";
                        AlertItem.statustime = sDt;
                        AlertItem.statuscheck = "Y";
                        AlertItem.villageid = RumtimeItem.villageID;
                        AlertItem.ReleaseRTime = RumtimeItem.RTime;

                        dbDapper.Insert(AlertItem);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, "");
            }
        }


        /// <summary>
        /// 更新警戒的雨量資料
        /// </summary>
        /// <param name="villageID"></param>
        private void UpdateLRTIAlertRainData(ref LRTIAlert LRTIAlertItem)
        {
            
            ssql = @"
                select * from  LRTIAlert where villageid = '{0}'           
                ";
            ssql = string.Format(ssql, LRTIAlertItem.villageid);
            LRTIAlert AlertItem = dbDapper.QuerySingleOrDefault<LRTIAlert>(ssql);

            if (AlertItem != null)
            {
                ssql = " select * from RunTimeRainData where STID = '{0}' ";
                ssql = string.Format(ssql, AlertItem.STID);
                RunTimeRainData RuntimeData = dbDapper.QuerySingleOrDefault<RunTimeRainData>(ssql);
                if (RuntimeData != null)
                {
                    if (RuntimeData.STATUS == "-99")
                    {
                        LRTIAlertItem.HOUR1 = "異常";
                        LRTIAlertItem.HOUR2 = "異常";
                        LRTIAlertItem.HOUR3 = "異常";
                        LRTIAlertItem.HOUR6 = "異常";
                        LRTIAlertItem.RT = "異常";
                        LRTIAlertItem.LRTI = "異常";
                    }
                    else
                    {
                        LRTIAlertItem.HOUR1 = RuntimeData.RAIN;
                        LRTIAlertItem.HOUR2 = RuntimeData.HOUR2;
                        LRTIAlertItem.HOUR3 = RuntimeData.HOUR3;
                        LRTIAlertItem.HOUR6 = RuntimeData.HOUR6;
                        LRTIAlertItem.RT = RuntimeData.RT;
                        LRTIAlertItem.LRTI = RuntimeData.LRTI;
                        LRTIAlertItem.ReleaseRTime = RuntimeData.RTime;
                    }
                }

                dbDapper.Update(AlertItem);

            }


        }

        private string CheckLRTIAlertStatus_108(string villageID)
        {
            string sStatus = "";

            ssql = @" 
                    select * from  LRTIAlertRefData   
                    inner join RunTimeRainData on LRTIAlertRefData.STID = RunTimeRainData.STID
                    where LRTIAlertRefData.ver = 'now' and LRTIAlertRefData.villageID = '{0}'
                    ";
            ssql = string.Format(ssql, villageID);
            List<dynamic> RuntimeList = dbDapper.Query(ssql);

            foreach (var RumtimeItem in RuntimeList)
            {
                double dRT = 0;    //有效累積雨量
                double dHOUR3 = 0; //3小時累積雨量
                Double.TryParse(RumtimeItem.RT, out dRT);
                Double.TryParse(RumtimeItem.HOUR3, out dHOUR3);

                //依照規則判斷發布狀態
                //發布順位1，紅色
                //土石流紅色警戒發布條件 降雨時3小時累積雨量 > 30mm，且有效累積雨量RT > G欄位
                double dFlowWarning = double.MaxValue;
                Double.TryParse(RumtimeItem.FlowWarning, out dFlowWarning);
                if (dFlowWarning == 0) dFlowWarning = double.MaxValue; //無法轉型，預設最大值

                if (dHOUR3 > 30 && dRT > dFlowWarning)
                {
                    sStatus = "A3";
                    break;
                }

                //發布順位2，澄色
                //降雨時I(3小時累積雨量)欄為 & H(有效累積雨量RT)欄位同時達到門檻
                double dRt_70 = 0;
                double dR3_70 = 0;
                dRt_70 = RumtimeItem.Rt_70;
                dR3_70 = RumtimeItem.R3_70;
                if (dHOUR3 > dR3_70 && dRT > dRt_70)
                {
                    sStatus = "A2";
                    break;
                }

                //發布順位3，黃色
                //降雨時K(3小時累積雨量)欄位 & J(有效累積雨量RT)欄位同時達到門檻
                double dRt_50 = 0;
                double dR3_50 = 0;
                dRt_50 = RumtimeItem.Rt_50;
                dR3_50 = RumtimeItem.R3_50;
                if (dHOUR3 > dR3_50 && dRT > dRt_50)
                {
                    sStatus = "A1";
                    break;
                }

            }

            return sStatus;
        }

        /// <summary>
        /// 判斷調降條件，是否需要調降
        /// </summary>
        /// <param name="villageID"></param>
        /// <returns></returns>
        private Boolean CheckLRTIAlertStatusDowngrade_108(string villageID)
        {
            Boolean sResult = false;

            //調降規則
            //1.	警戒調降/解除標準
            // 6小時累積雨量≦24mm，且這6小時內的所有小時雨量≦10mm。


            ssql = @" 
                    select * from  LRTIAlert 
                    inner join RunTimeRainData on LRTIAlert.STID = RunTimeRainData.STID
                    where LRTIAlert.villageID = '{0}'
                    ";
            ssql = string.Format(ssql, villageID);
            List<dynamic> RuntimeList = dbDapper.Query(ssql);

            foreach (var RumtimeItem in RuntimeList)
            {
                //6小時累積雨量≦24mm
                double dHOUR6 = 0; //6小時累積雨量                
                Double.TryParse(RumtimeItem.HOUR6, out dHOUR6);

                //6小時內的所有小時雨量≦10mm
                //取得目前的RTime
                string sRTime = RumtimeItem.RTime;
                DateTime dtRuntime = Convert.ToDateTime(sRTime);
                string sRTime_1 = dtRuntime.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ss");
                string sRTime_2 = dtRuntime.AddHours(-2).ToString("yyyy-MM-ddTHH:mm:ss");
                string sRTime_3 = dtRuntime.AddHours(-3).ToString("yyyy-MM-ddTHH:mm:ss");
                string sRTime_4 = dtRuntime.AddHours(-4).ToString("yyyy-MM-ddTHH:mm:ss");
                string sRTime_5 = dtRuntime.AddHours(-5).ToString("yyyy-MM-ddTHH:mm:ss");
                ssql = @"
                        select RTime from RainStation 
                        where STID = '{0}' and RTime in ('{1}','{2}','{3}','{4}','{5}','{6}')
                        and CAST(RAIN AS FLOAT) <= 10
                        ";
                ssql = string.Format(ssql, RumtimeItem.STID, sRTime, sRTime_1, sRTime_2, sRTime_3, sRTime_4, sRTime_5);
                int iCount = dbDapper.QueryTotalCount(ssql);

                //1.	警戒調降/解除標準
                // 6小時累積雨量≦24mm，且這6小時內的所有小時雨量≦10mm。
                if (dHOUR6 <= 24 && iCount == 6)
                {
                    sResult = true;
                }

            }

            return sResult;
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
        }


    }

}
