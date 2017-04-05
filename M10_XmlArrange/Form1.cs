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
  public partial class Form1 : Form
  {
    const string sBakFilePath = @"C:\m10\bak";
    const string sFilePathArrange = @"C:\m10\Arrange";
    const string sFilePathProblem = @"C:\m10\Problem";
    string sZipTarDirectory = @"c:\m10\Arrange";
    string sZipDesDirectory = @"c:\m10\ArrangeZip"; 
    public Form1()
    {
      InitializeComponent();


      
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      lblProcCount.Text = "";
      lblProc.Text = "";

      if (!Directory.Exists(sBakFilePath)) Directory.CreateDirectory(sBakFilePath);
      if (!Directory.Exists(sFilePathArrange)) Directory.CreateDirectory(sFilePathArrange);
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
      List<string> ListFile = Directory.GetFiles(sBakFilePath).ToList<string>();

      int iIndex = 0;
      //取得所有檔案
      foreach (string sFile in ListFile)
      {
        iIndex++;

        lblProcCount.Text = string.Format("{0}/{1}", iIndex.ToString(), ListFile.Count.ToString());
        lblProcCount.Refresh();

        string sSaveFolder = "";
        FileInfo fi = new FileInfo(sFile);

        //解析檔案年月日
        string sYear = "";
        string sMonth = "";
        string sDay = "";

        List<string> slist = fi.Name.Split('_').ToList<string>();

        if (slist.Count < 4) continue;

        //分類
        sSaveFolder = string.Format(@"{0}\{1}\{2}\{3}\", sFilePathArrange, slist[0], slist[1], slist[2]);
        if (!Directory.Exists(sSaveFolder)) Directory.CreateDirectory(sSaveFolder);

        //歸檔
        //檔案移至整理路徑
        lblProc.Text = string.Format("{0}：移動至{1}", fi.Name, sSaveFolder + fi.Name);
        lblProc.Refresh();
        File.Move(fi.FullName, sSaveFolder + fi.Name);

      }

      //壓縮
      //using (ZipFile zip = new ZipFile(@"c:\test.zip"))
      //{
      //  zip.AddFile("");


      //  zip.Save();

      //}



    }

    private void btnTest_Click(object sender, EventArgs e)
    {
      //壓縮
      using (ZipFile zip = new ZipFile(Encoding.Default))
      { 
        zip.SaveProgress += zipProgress;
        //大檔壓縮
        zip.UseZip64WhenSaving = Zip64Option.AsNecessary;       

        //檔案名稱
        string sZipName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");
        //忽略重複檔案
        zip.IgnoreDuplicateFiles = true;
        zip.AddDirectory(sZipTarDirectory,"123");
        zip.Save(string.Format(@"{0}\{1}.zip",sZipDesDirectory, sZipName));
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

    private void btnBack_Click(object sender, EventArgs e)
    {
      backgroundWorker1.RunWorkerAsync();
    }

    private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
    {
      btnTest_Click(sender, e);
    }

    private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      progressBar1.Value = e.ProgressPercentage;
      lblProgress.Text = string.Format("壓縮執行進度：{0}%", e.ProgressPercentage.ToString());
    }
  }
}
