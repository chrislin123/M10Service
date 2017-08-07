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
      string stype = "";
      string surl = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=2";

      HtmlWeb webClient = new HtmlWeb();
      //網頁特殊編碼
      webClient.OverrideEncoding = Encoding.GetEncoding(950);

      // 載入網頁資料 
      HtmlAgilityPack.HtmlDocument doc = webClient.Load(surl);

      // 裝載查詢結果 
      HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table[2]/tr");
      
      foreach (HtmlNode node in nodes)
      {

        HtmlNodeCollection tdnodes = node.SelectNodes("td");

        if (tdnodes.Count>0)
        {
          HtmlNode tdnode = tdnodes[0];
          string[] StockInfoSplit = tdnode.InnerText.Split('　');

          if (StockInfoSplit.Length != 2) continue;

          string sCode = StockInfoSplit[0];
          string sName = StockInfoSplit[1];

          //判斷代碼存在則更新，不存在新增
          ssql = " select * from stockinfo where stockcode = '{0}' ";
          StockInfo si = dbDapper.QuerySingleOrDefault<StockInfo>(string.Format(ssql, sCode));

          if (si == null) //不存在新增
          {
            si = new StockInfo();
            si.stockcode = sCode;
            si.stockname = sName;
            si.type = stype;
            dbDapper.Insert(si);
          }
          else
          {

          }
        }

      }

      
    }
  }
}
