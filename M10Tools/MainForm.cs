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
using HtmlAgilityPack;

namespace M10Tools
{
  public partial class MainForm : BaseForm
  {
    public MainForm()
    {
      InitializeComponent();
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
  }
}
