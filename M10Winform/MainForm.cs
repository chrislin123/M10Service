using System;
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
using System.Data.SqlClient;
using System.Transactions;
using CL.Data;
using System.Data.Common;

namespace M10Winform
{
    public partial class MainForm : Form
    {
        private Timer MyTimer;
        string folderName = @"D:\m10\temp\";
        string folderBack = @"D:\m10\back\";
        string ssql = string.Empty;

        //string sConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=m10;"; 

        //string sConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=False;Persist Security Info=True;User ID=sa;Password=sa;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=m10;Integrated Security=SSPI";

        //string sConnectionString = "Data Source=.;Integrated Security=False;User ID=sa;Password=sa;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=m10";

        string sConnectionString = Properties.Settings.Default.DBConnectionString;
        ODAL oDal = new ODAL(Properties.Settings.Default.DBConnectionString);

        public MainForm()
        {
            //SetEventLog("test");
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string sLog = "M10ServiceLog";
            string sSource = "M10ServiceLog";
            
            try
            {
                if (!System.Diagnostics.EventLog.SourceExists(sSource))
                {
                    System.Diagnostics.EventLog.CreateEventSource(sSource, sLog);
                }

                eventLog1.Source = sSource;

                //相關建立資料夾
                if (!Directory.Exists(folderBack)) Directory.CreateDirectory(folderBack);
                if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);
            }
            catch (Exception excep)
            {
                SetEventLog(excep.ToString());
                throw;
            }

            timer1.Enabled = true;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;


            try
            {
                ShowMessageToFront("轉檔啟動");
                SetEventLog("轉檔啟動");

                ProceStart();

                SetEventLog("轉檔完畢");
                ShowMessageToFront("轉檔完畢");
            }
            catch (Exception exc)
            {
                SetEventLog("轉檔錯誤:" + exc.ToString());
                throw;
            }



            System.Threading.Thread.Sleep(5000);
            
            this.Close();
        }


        private string getconfig()
        {
            string sResult = string.Empty;

            //取得Service 設定檔


            int recordCount = 10000;
            try
            {
                //ConfigurationManager.RefreshSection("appSettings");// 刷新命名节，在下次检索它时将从磁盘重新读取它。

                //ConfigurationSettings.AppSettings.Get("");

                //recordCount = Math.Abs(int.Parse(ConfigurationManager.AppSettings["recordcountUser"]));
                if (recordCount == 0)
                {
                    recordCount = 10000;
                }
            }
            catch
            {
                recordCount = 10000;
            }
            sResult = recordCount.ToString();

            return sResult;
        }


        private void ProceStart()
        {
            try
            {
                
                // FTP 下載資料到本機
                FtpDownload();


                // 取得資料夾內所有檔案
                foreach (string fname in System.IO.Directory.GetFiles(folderName))
                {
                    //ShowMessageToFront("轉檔啟動");
                    //轉檔到DB
                    TransToDB(fname);
                    //ShowMessageToFront("轉檔完畢");


                    //存至備份資料夾
                    FileInfo fi = new FileInfo(fname);

                    //記錄轉檔資料
                    FileTransLog(fi.Name);


                    if (File.Exists(folderBack + fi.Name))
                    {
                        fi.Delete();
                    }
                    else
                    {
                        fi.MoveTo(folderBack + fi.Name);
                    }
                }

            }
            catch (Exception )
            {
                //eventLog1.WriteEntry("ProceStart 錯誤:" + e.ToString());
                throw;
            }


        }

        private void FileTransLog(string pFileName)
        {
            ssql = " insert into FileTransLog "
                + "  ([FileTransName],[FileTransTime]) "
                + " VALUES "
                + " ( "
                + " '" + pFileName + "' "
                + " ,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' "                
                + " ) "
                ;
            oDal.CommandText = ssql;
            oDal.ExecuteSql();
        }

        private void FtpDownload()
        {
            //string sIP = "192.168.13.155";
            //string sUser = "iot";
            //string sPassword = "iot";

            string sIP = "140.116.38.195";
            string sUser = "yuchao2013";
            string sPassword = "yuchao2013";
            ftp ftpClient;
            try
            {
                ShowMessageToFront("Ftp連線");
                /* Create Object Instance */
                ftpClient = new ftp(@"ftp://" + sIP + "/", sUser, sPassword);

                /* Upload a File */
                //ftpClient.upload("test/test.txt", @"C:\test.txt");


                /* Download a File */
                //ftpClient.download("test/test.txt", @"d:\test111.txt");

                /* Delete a File */
                //ftpClient.delete("etc/test.txt");

                /* Rename a File */
                //ftpClient.rename("etc/test.txt", "test2.txt");

                /* Create a New Directory */
                //ftpClient.createDirectory("etc/test");

                /* Get the Date/Time a File was Created */
                //string fileDateTime = ftpClient.getFileCreatedDateTime("etc/test.txt");


                /* Get the Size of a File */
                //string fileSize = ftpClient.getFileSize("etc/test.txt");


                ShowMessageToFront("Ftp取得清單");
                /* Get Contents of a Directory (Names Only) */
                string[] simpleDirectoryListing = ftpClient.directoryListDetailed("14_FCU_raindata/M10");
                //for (int i = 0; i < simpleDirectoryListing.Count(); i++) { Console.WriteLine(simpleDirectoryListing[i]); }

                /* Get Contents of a Directory with Detailed File/Directory Info */
                //string[] detailDirectoryListing = ftpClient.directoryListDetailed("/test");
                //for (int i = 0; i < detailDirectoryListing.Count(); i++) { Console.WriteLine(detailDirectoryListing[i]); }


                //取得檔案名稱清單
                List<string> lstFileName = new List<string>();

                foreach (string sFullName in simpleDirectoryListing)
                {
                    
                    if (sFullName != "")
                    {
                        //字串空白
                        string[] sary = sFullName.Split(' ');

                        string sFileName = sary[sary.Length - 1];
                        lstFileName.Add(sFileName);
                    }
                }


                foreach (string sFileName in lstFileName)
                {
                    //判斷是否已轉檔
                    ssql = @" select * from FileTransLog  
                            where 1=1 and FileTransName = '" + sFileName + @"'
                        ";
                    oDal.CommandText = ssql;
                    //沒有下載紀錄則下載
                    if (oDal.DataTable().Rows.Count == 0)
                    {
                        
                        /* Download a File */
                        ftpClient.download("14_FCU_raindata/M10/" + sFileName, folderName + sFileName);
                        //預防下載時，後續檔案異動會造成Error
                        ShowMessageToFront("Ftp下載檔案到" +  folderName + sFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageToFront(ex.ToString());
                throw;
            }

            

            System.Threading.Thread.Sleep(5000);


            /* Release Resources */
            ftpClient = null;

            //throw new NotImplementedException();
        }


        private void TransToDB(string sFilePath)
        {
            try
            {
                ShowMessageToFront("轉檔:" + sFilePath);

                XmlDocument xd = new XmlDocument();

                xd.Load(sFilePath);

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


                DataTable dttemp = new DataTable();

                //轉入StationData
                foreach (DataRow dr in dt.Rows)
                {
                    
                    dttemp.Clear();
                    string sSTID = dr["STID"].ToString();

                    ssql = " select * from StationData "
                         + "  where STID = '" + sSTID + "' "
                         + "  "
                         + "  "
                         ;
                    oDal.CommandText = ssql;

                    //沒資料則寫入一筆新資料
                    if (oDal.DataTable().Rows.Count == 0)
                    {

                        ssql = " insert into StationData "
                          + "  ([STID],[STNAME],[COUNTY],[TOWN]) "
                          + " VALUES "
                          + " ( "
                          + " '" + dr["STID"].ToString() + "' "
                          + " ,'" + dr["STNAME"].ToString() + "' "
                          + " ,'" + dr["COUNTY"].ToString() + "' "
                          + " ,'" + dr["TOWN"].ToString() + "' "
                          + " ) "
                          ;
                        oDal.CommandText = ssql;
                        oDal.ExecuteSql();
                    }
                }                
                
                Boolean bUpdateRuntim = false;
                //取得目前資料時間比對即時資料時間
                if (dt.Rows.Count>0)
	            {
                    DateTime dtRunTimeRainData = DateTime.Now.AddYears(-100);
                    DateTime dtRTime = Convert.ToDateTime(dt.Rows[0]["RTime"].ToString());

                    ssql = " select top 1 * from RunTimeRainData ";
                    oDal.CommandText = ssql;
                    oDal.DataTable();
                    foreach (DataRow drRtime in oDal.DataTable().Rows)
                    {
                        dtRunTimeRainData = Convert.ToDateTime(drRtime["RTime"].ToString());
                    }

                    if (DateTime.Compare(dtRTime,dtRunTimeRainData) > 0)
                    {
                        bUpdateRuntim = true;
                    }
	            }

                if (bUpdateRuntim == true)
                {
                    //清空runtime 資料表
                    ssql = " delete RunTimeRainData ";
                    oDal.CommandText = ssql;
                    oDal.ExecuteSql();
                    
                    //寫入RuntimeRainData
                    foreach (DataRow dr in dt.Rows)
                    {
                        ssql = " insert into RunTimeRainData "
                                + "  ([STID] "
                                + " ,[STNAME]"
                                + " ,[STTIME]"
                                + " ,[LAT]"
                                + " ,[LON]"
                                + " ,[ELEV]"
                                + " ,[RAIN]"
                                + " ,[MIN10]"
                                + " ,[HOUR3]"
                                + " ,[HOUR6]"
                                + " ,[HOUR12]"
                                + " ,[HOUR24]"
                                + " ,[NOW]"
                                + " ,[COUNTY]"
                                + " ,[TOWN]"
                                + " ,[ATTRIBUTE]"
                                + " ,[diff]"
                                + " ,[STATUS]"
                                + " ,[TMX]"
                                + " ,[TMY]"
                                + " ,[RTime]"
                                + " ,[SWCBID]"
                                + " ,[DebrisRefStation]"
                                + " ,[EffectiveRainfall]"
                                + " ,[RT]"
                                + " ,[Cumulation]"
                                + " ,[Day1]"
                                + " ,[Day2]"
                                + " ,[Day3]"
                                + " ,[Hour2]"
                                + " ,[WGS84_lon]"
                                + " ,[WGS84_lat]) "
                                + " VALUES "
                                + " ( "
                                + "  '" + dr["STID"].ToString() + "' "
                                + " ,'" + dr["STNAME"].ToString() + "'"
                                + " ,'" + dr["STTIME"].ToString() + "'"
                                + " ,'" + dr["LAT"].ToString() + "'"
                                + " ,'" + dr["LON"].ToString() + "'"
                                + " ,'" + dr["ELEV"].ToString() + "'"
                                + " ,'" + RainDataValid(dr["RAIN"].ToString()) + "'"
                                + " ,'" + RainDataValid(dr["MIN10"].ToString()) + "'"
                                + " ,'" + RainDataValid(dr["HOUR3"].ToString()) + "'"
                                + " ,'" + RainDataValid(dr["HOUR6"].ToString()) + "'"
                                + " ,'" + RainDataValid(dr["HOUR12"].ToString()) + "'"
                                + " ,'" + RainDataValid(dr["HOUR24"].ToString()) + "'"
                                + " ,'" + RainDataValid(dr["NOW"].ToString()) + "'"
                                + " ,'" + dr["COUNTY"].ToString() + "'"
                                + " ,'" + dr["TOWN"].ToString() + "'"
                                + " ,'" + dr["ATTRIBUTE"].ToString() + "'"
                                + " ,'" + dr["diff"].ToString() + "'"
                                + " ,'" + dr["STATUS"].ToString() + "'"
                                + " ,'" + dr["TMX"].ToString() + "'"
                                + " ,'" + dr["TMY"].ToString() + "'"
                                + " ,'" + dr["RTime"].ToString() + "'"
                                + " ,'" + dr["SWCBID"].ToString() + "'"
                                + " ,'" + dr["DebrisRefStation"].ToString() + "'"
                                + " ,'" + dr["EffectiveRainfall"].ToString() + "'"
                                + " ,'" + dr["RT"].ToString() + "'"
                                + " ,'" + dr["Cumulation"].ToString() + "'"
                                + " ,'" + dr["Day1"].ToString() + "'"
                                + " ,'" + dr["Day2"].ToString() + "'"
                                + " ,'" + dr["Day3"].ToString() + "'"
                                + " ,'" + dr["Hour2"].ToString() + "'"
                                + " ,'" + dr["WGS84_lon"].ToString() + "'"
                                + " ,'" + dr["WGS84_lat"].ToString() + "' "
                              + " ) "
                              ;
                        oDal.CommandText = ssql;
                        oDal.ExecuteSql();
                    }
                }

                //資料寫入sql
                foreach (DataRow dr in dt.Rows)
                {
                    //判斷資料不存在才寫入
                    ssql = " select * from RainStation "
                         + " where 1=1 " 
                         + " and STID = '" + dr["STID"].ToString() + "' "
                         + " and RTime = '" + dr["RTime"].ToString() + "' " 
                         ;

                    oDal.CommandText = ssql;

                    if (oDal.DataTable().Rows.Count == 0)
                    {
                        ssql = " insert into RainStation "
                            + "  ([STID] "
                            + " ,[STNAME]"
                            + " ,[STTIME]"
                            + " ,[LAT]"
                            + " ,[LON]"
                            + " ,[ELEV]"
                            + " ,[RAIN]"
                            + " ,[MIN10]"
                            + " ,[HOUR3]"
                            + " ,[HOUR6]"
                            + " ,[HOUR12]"
                            + " ,[HOUR24]"
                            + " ,[NOW]"
                            + " ,[COUNTY]"
                            + " ,[TOWN]"
                            + " ,[ATTRIBUTE]"
                            + " ,[diff]"
                            + " ,[STATUS]"
                            + " ,[TMX]"
                            + " ,[TMY]"
                            + " ,[RTime]"
                            + " ,[SWCBID]"
                            + " ,[DebrisRefStation]"
                            + " ,[EffectiveRainfall]"
                            + " ,[RT]"
                            + " ,[Cumulation]"
                            + " ,[Day1]"
                            + " ,[Day2]"
                            + " ,[Day3]"
                            + " ,[Hour2]"
                            + " ,[WGS84_lon]"
                            + " ,[WGS84_lat]) "
                            + " VALUES "
                            + " ( "
                            + "  '" + dr["STID"].ToString() + "' "
                            + " ,'" + dr["STNAME"].ToString() + "'"
                            + " ,'" + dr["STTIME"].ToString() + "'"
                            + " ,'" + dr["LAT"].ToString() + "'"
                            + " ,'" + dr["LON"].ToString() + "'"
                            + " ,'" + dr["ELEV"].ToString() + "'"
                            + " ,'" + RainDataValid(dr["RAIN"].ToString()) + "'"
                            + " ,'" + RainDataValid(dr["MIN10"].ToString()) + "'"
                            + " ,'" + RainDataValid(dr["HOUR3"].ToString()) + "'"
                            + " ,'" + RainDataValid(dr["HOUR6"].ToString()) + "'"
                            + " ,'" + RainDataValid(dr["HOUR12"].ToString()) + "'"
                            + " ,'" + RainDataValid(dr["HOUR24"].ToString()) + "'"
                            + " ,'" + RainDataValid(dr["NOW"].ToString()) + "'"
                            + " ,'" + dr["COUNTY"].ToString() + "'"
                            + " ,'" + dr["TOWN"].ToString() + "'"
                            + " ,'" + dr["ATTRIBUTE"].ToString() + "'"
                            + " ,'" + dr["diff"].ToString() + "'"
                            + " ,'" + dr["STATUS"].ToString() + "'"
                            + " ,'" + dr["TMX"].ToString() + "'"
                            + " ,'" + dr["TMY"].ToString() + "'"
                            + " ,'" + dr["RTime"].ToString() + "'"
                            + " ,'" + dr["SWCBID"].ToString() + "'"
                            + " ,'" + dr["DebrisRefStation"].ToString() + "'"
                            + " ,'" + dr["EffectiveRainfall"].ToString() + "'"
                            + " ,'" + dr["RT"].ToString() + "'"
                            + " ,'" + dr["Cumulation"].ToString() + "'"
                            + " ,'" + dr["Day1"].ToString() + "'"
                            + " ,'" + dr["Day2"].ToString() + "'"
                            + " ,'" + dr["Day3"].ToString() + "'"
                            + " ,'" + dr["Hour2"].ToString() + "'"
                            + " ,'" + dr["WGS84_lon"].ToString() + "'"
                            + " ,'" + dr["WGS84_lat"].ToString() + "' "
                          + " ) "
                          ;
                        oDal.CommandText = ssql;
                        oDal.ExecuteSql();                     
   
                    }
                }



                string ssss = string.Empty;

                //加入2.0以後的交易,記得匯入System.Transactions.dll
                /*
                using (TransactionScope myScope = new TransactionScope())
                {
                    
                    //大量寫入
                    using (SqlConnection myConn = new SqlConnection(sConnectionString))
                    {
                        myConn.Open();

                        using (SqlBulkCopy mySbc = new SqlBulkCopy(myConn))
                        {
                            //設定

                            mySbc.BatchSize = 1000;
                            mySbc.BulkCopyTimeout = 60;

                            //處理完後丟出一個事件,或是說處理幾筆後就丟出事件
                            //mySbc.NotifyAfter = sourceDt.Rows.Count;
                            //mySbc.SqlRowsCopied += new SqlRowsCopiedEventHandler(mySbc_SqlRowsCopied);

                            //更新哪個資料庫
                            //mySbc.DestinationTableName = "dbo.m10.RainStation";
                            mySbc.DestinationTableName = "RainStation";

                            //column對應
                            foreach (DataColumn dc in dt.Columns)
                            {
                                mySbc.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                            }


                            //寫入d
                            mySbc.WriteToServer(dt);
                            //throw new Exception("error");

                            //完成交易
                            myScope.Complete();
                        }
                    }
                 * 
                }*/
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetEventLog(string sLogString){            

            //todo 加入換行符號
            eventLog1.WriteEntry(
                    string.Format("{0} [log] : {1} ", DateTime.Now.ToString(), sLogString)
            );
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

                if (double.TryParse(pRainData, out dResult))
                {
                    		 
                }

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
            richTextBox1.Refresh();
        }

    }
}
