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
using Ionic.Zip;

namespace M10_XmlArrange
{
  public partial class MXAP010Form : Form
  { 
    string sFilePathXml = string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(),"XML");
    string sFilePathArrange = string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), "Arrange"); 
    string sFilePathProblem = string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), "Problem"); 
    string sZipTarDirectory = string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), "Arrange"); 
    string sZipDesDirectory = string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), "ArrangeZip");
    string sZipTempDirectory = string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), "ZipTemp");
    public MXAP010Form()
    {
      InitializeComponent();      
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      lblProcCount.Text = "";
      lblProc.Text = "";
      lblStatus.Text = "";      

      if (!Directory.Exists(sFilePathXml)) Directory.CreateDirectory(sFilePathXml);
      if (!Directory.Exists(sFilePathArrange)) Directory.CreateDirectory(sFilePathArrange);
      if (!Directory.Exists(sFilePathProblem)) Directory.CreateDirectory(sFilePathProblem);
      if (!Directory.Exists(sZipTarDirectory)) Directory.CreateDirectory(sZipTarDirectory);
      if (!Directory.Exists(sZipDesDirectory)) Directory.CreateDirectory(sZipDesDirectory);
      if (!Directory.Exists(sZipTempDirectory)) Directory.CreateDirectory(sZipTempDirectory);
    }

    private void btnStart_Click(object sender, EventArgs e)
    {

      try
      {
        //取得資料夾所有檔案名稱
        List<string> AllFileList = Directory.GetFiles(sFilePathArrange, "*", SearchOption.AllDirectories).ToList<string>();
        List<string> AllFileNameList = new List<string>();
        foreach (string item in AllFileList) AllFileNameList.Add(new FileInfo(item).Name);


        List<string> ListFile = Directory.GetFiles(sFilePathXml).ToList<string>();

        lblStatus.Text = "歸檔";
        lblStatus.Refresh();

        int iIndex = 0;
        //取得所有檔案
        foreach (string sFile in ListFile)
        {
          iIndex++;

          lblProcCount.Text = string.Format("{0}/{1}", iIndex.ToString(), ListFile.Count.ToString());
          lblProcCount.Refresh();

          string sSaveFolder = "";
          FileInfo fi = new FileInfo(sFile);

          //資料已經歸檔，則跳過
          if (AllFileNameList.Contains(fi.Name)) continue;


          //解析檔案年月日
          List<string> slist = fi.Name.Split('_').ToList<string>();

          if (slist.Count < 4) continue;

          //分類
          sSaveFolder = string.Format(@"{0}\{1}\{2}\{3}\", sFilePathArrange, slist[0], slist[1], slist[2]);
          if (!Directory.Exists(sSaveFolder)) Directory.CreateDirectory(sSaveFolder);

          //歸檔
          //檔案移至整理路徑
          lblProc.Text = string.Format("{0}：移動至{1}", fi.Name, sSaveFolder + fi.Name);
          lblProc.Refresh();
          Application.DoEvents();
          fi.CopyTo(sSaveFolder + fi.Name, true);
          //1060529 來源資料夾不刪除
          //fi.Delete();    
        }

        //壓縮非同步
        lblStatus.Text = "非同步壓縮";
        lblStatus.Refresh();

        if (backgroundWorker1.IsBusy == false)
        {
          backgroundWorker1.RunWorkerAsync("test");
        }


        DateTime dt_3 = DateTime.Now.AddDays(-3);
        //移除超過三天檔案
        foreach (string sFile in Directory.GetFiles(sZipDesDirectory))
        {
          FileInfo fi = new FileInfo(sFile);

          if (DateTime.Compare(fi.CreationTime, dt_3) > 0)
          {
            fi.Delete();
          }
        }
      }
      catch (Exception ex)
      {

        
      }     
    }

    private void ProcZip()
    {
      //壓縮
      using (ZipFile zip = new ZipFile(Encoding.Default))
      {
        zip.SaveProgress += zipProgress;

        zip.TempFileFolder = sZipTempDirectory;
        //大檔壓縮
        zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
        //檔案名稱
        string sZipName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        //設定密碼
        zip.Password = "pass@word1";

        //忽略重複檔案
        zip.IgnoreDuplicateFiles = true;
        zip.AddDirectory(sZipTarDirectory, "");
        zip.Save(string.Format(@"{0}\{1}.zip", sZipDesDirectory, sZipName));
      }
    }

    private void zipProgress(object sender, SaveProgressEventArgs e)
    {
      if (e.EventType == ZipProgressEventType.Saving_AfterWriteEntry)
      { 
        backgroundWorker1.ReportProgress(e.EntriesSaved * 100 / e.EntriesTotal);
        
        //this.progressBar1.Value = e.EntriesSaved * 100 / e.EntriesTotal;
        
      }
      //if (e.EventType == ZipProgressEventType.Saving_EntryBytesRead)
      //  this.progressBar1.Value = (int)((e.BytesTransferred * 100) / e.TotalBytesToTransfer);

      else if (e.EventType == ZipProgressEventType.Saving_Completed)
      {
        backgroundWorker1.ReportProgress(100);
        //this.progressBar1.Value = 100;
      }
        
    }


    private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
    {
      ProcZip();
      //btnTest_Click(sender, e);
    }

    private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      progressBar1.Value = e.ProgressPercentage;
      lblProgress.Text = string.Format("壓縮執行進度：{0}%", e.ProgressPercentage.ToString());
    }

    private void MXAP010Form_FormClosed(object sender, FormClosedEventArgs e)
    { 

      //if (backgroundWorker1.WorkerSupportsCancellation == true && backgroundWorker1.IsBusy)
      //{
      //  backgroundWorker1.CancelAsync();
      //}
    }

    private void MXAP010Form_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (backgroundWorker1.WorkerSupportsCancellation == true && backgroundWorker1.IsBusy)
      {
        backgroundWorker1.CancelAsync();
      }
    }

    private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      //清空temp 資料表
      List<string> ListFile = Directory.GetFiles(sZipTempDirectory).ToList<string>();
      foreach (string sFile in ListFile)
      {
        File.Delete(sFile);
      }
    }
  }
}
