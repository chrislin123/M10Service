using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using System.Xml;
using System.Data.SqlClient;
using System.Transactions;
using System.IO;
using CL.Data;

namespace M10Service
{
    public partial class SrvTrans : ServiceBase
    {
        
        private Timer MyTimer;
        string folderName = @"D:\m10\temp\";
        string folderBack = @"D:\m10\back\";
        string ssql = string.Empty;

        //string sConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=m10;"; 

        //string sConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=False;Persist Security Info=True;User ID=sa;Password=sa;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=m10;Integrated Security=SSPI";

        //string sConnectionString = "Data Source=.;Integrated Security=False;User ID=sa;Password=sa;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=m10";

        string sConnectionString = Properties.Settings.Default.DBConnectionString;


        


        public SrvTrans()
        {
            InitializeComponent();

            //Service 偵錯的方法
            System.Diagnostics.Debugger.Launch();          
                       
            

            //string path = Environment.CurrentDirectory;

            //FtpDownload();

            


            //oDal.CommandText = "select * from StationData ";

            //DataTable dt = new DataTable();
            //dt = oDal.DataTable();
           
            

            

            //string sMachineName = Environment.MachineName;
            string sLog = "M10ServiceLog";
            string sSource = "M10ServiceLog";           

            try
            {
                ODAL oDal = new ODAL(Properties.Settings.Default.DBConnectionString);

                if (!System.Diagnostics.EventLog.SourceExists(sSource))
                {
                    System.Diagnostics.EventLog.CreateEventSource(sSource, sLog);
                }

                eventLog1.Source = sSource;

                eventLog1.WriteEntry("Start Service");

                //相關建立資料夾
                if (!Directory.Exists(folderBack)) Directory.CreateDirectory(folderBack);
                if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);
                
            }
            catch (Exception)
            {
                throw;
            }            
        }

        protected override void OnStart(string[] args)
        {
            //取得執行目錄
            //string path = Application.StartupPath;


            string path = Environment.SystemDirectory;

            eventLog1.WriteEntry(path);

            eventLog1.WriteEntry("Start Timer");

            MyTimer = new Timer();

            MyTimer.Elapsed += new ElapsedEventHandler(MyTimer_Elapsed);

            //預設20秒執行一次
            MyTimer.Interval = 20 * 1000;

            MyTimer.Start();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Stop Timer");

            MyTimer.Stop();

            MyTimer = null;
        }

        private void MyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //eventLog1.WriteEntry("Timer Ticked");

            //eventLog1.WriteEntry(Properties.Settings.Default.recordcountUser);


            //Service 偵錯的方法
            //System.Diagnostics.Debugger.Launch();


            eventLog1.WriteEntry("ProceStart 啟動");

            try
            {
                ProceStart();
            }
            catch (Exception)
            {                
                throw;
            }
            
            eventLog1.WriteEntry("ProceStart 完畢");


            //ConfigurationManager.RefreshSection("appSettings");
            //ConfigurationManager.RefreshSection("AppSettings");
            //eventLog1.WriteEntry(ConfigurationManager.AppSettings["test"]);


            //ConfigurationManager.RefreshSection("userSettings");
            //eventLog1.WriteEntry(Properties.Settings.Default.recordcountUser);

            //Properties.Settings.Default.re





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

                recordCount = Math.Abs(int.Parse(ConfigurationManager.AppSettings["recordcountUser"]));
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
                //FtpDownload();


                // 取得資料夾內所有檔案
                foreach (string fname in System.IO.Directory.GetFiles(folderName))
                {
                    //轉檔到DB
                    TransToDB(fname);

                    //存至備份資料夾
                    FileInfo fi = new FileInfo(fname);

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
            catch (Exception e)
            {
                eventLog1.WriteEntry("ProceStart 錯誤:" + e.ToString());
                throw;
            }
                   

        }

        private void FtpDownload()
        {
            string sIP = "192.168.13.155";
            string sUser = "iot";
            string sPassword = "iot";

            /* Create Object Instance */
            ftp ftpClient = new ftp(@"ftp://" + sIP + "/", sUser, sPassword);

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
            

            /* Get Contents of a Directory (Names Only) */
            string[] simpleDirectoryListing = ftpClient.directoryListDetailed("/test");
            //for (int i = 0; i < simpleDirectoryListing.Count(); i++) { Console.WriteLine(simpleDirectoryListing[i]); }

            /* Get Contents of a Directory with Detailed File/Directory Info */
            string[] detailDirectoryListing = ftpClient.directoryListDetailed("/test");
            //for (int i = 0; i < detailDirectoryListing.Count(); i++) { Console.WriteLine(detailDirectoryListing[i]); }

            


            /* Release Resources */
            ftpClient = null;

            //throw new NotImplementedException();
        }


        private void TransToDB(string sFilePath)
        {
            try
            {
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
                //foreach (DataRow dr in dt.Rows)
                //{
                //    dttemp.Clear();
                //    string sSTID = dr["STID"].ToString();

                //    ssql = " select * from StationData "
                //         + "  where STID = '" + sSTID + "' "
                //         + "  "
                //         + "  " 
                //         ;
                //    oDal.CommandText = ssql;                 

                //    //沒資料則寫入一筆新資料
                //    if (oDal.DataTable().Rows.Count == 0)
                //    {
                        
                //        ssql = " insert into StationData "
                //          + "  ([STID],[SName],[Country],[Town]) "
                //          + " VALUES "
                //          + " ( "
                //          + " '" + dr["STID"].ToString() + "' "
                //          + " ,'" + dr["STNAME"].ToString() + "' "
                //          + " ,'" + dr["COUNTY"].ToString() + "' "
                //          + " ,'" + dr["TOWN"].ToString() + "' "
                //          + " ) "
                //          ;
                //        oDal.CommandText = ssql;      

                //    }




                //}                











              




                string ssss = string.Empty;



                //加入2.0以後的交易,記得匯入System.Transactions.dll
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
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
