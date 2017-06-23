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

namespace M10AlertLRTI
{
  public partial class L11HighRail : BaseForm
  {
    public L11HighRail()
    {
      InitializeComponent();

      //載入BaseForm資料
      base.InitForm();
    }


    private void button1_Click(object sender, EventArgs e)
    {
      DataTable dtresult = new DataTable();
      dtresult.Columns.Add("country");
      dtresult.Columns.Add("week");
      dtresult.Columns.Add("car");
      dtresult.Columns.Add("time");

      string sFilepath = @"c:\dn.csv";

      string sType = "";


      List<string> ll = new List<string>();

      using (StreamReader SR = new StreamReader(sFilepath))
      {
        string Line;
        while ((Line = SR.ReadLine()) != null)
        {
          Line = Line.Replace(" ", "");

          if (Line == "") continue;
          if (Line.Contains("=") == true) continue;
          if (Line.Contains("車次") == true) continue;
          if (Line.Contains("台灣高鐵列車時刻表") == true) continue;
          if (Line.Contains("南下") == true) {
            sType = "dn";
            continue;
          }
          if (Line.Contains("北上") == true)
          {
            sType = "up";
            continue;
          }
          
          if (Line.Contains("註:") == true) continue;
          if (Line.Contains("站名") == true) continue;
         
          string[] ReadLine_Array = Line.Split(',');
          if (ReadLine_Array[0] == "" && ReadLine_Array[1] == "" && ReadLine_Array[2] == "")
          {
            continue;
          }

          ll.Add(Line);       
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

      List<string> runday = new List<string>();
      List<string> car = new List<string>();

      foreach (DataRow LoopRow in dt.Rows)
      {
        if (LoopRow[0].ToString() == "行駛日")
        {
          for (int i = 0; i < dt.Columns.Count; i++)
          {
            runday.Add(LoopRow[i].ToString());
          }
        }
        if (LoopRow[0].ToString() == "")
        {
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


      List<HighRail> RailList = new List<HighRail>();


      foreach (DataRow LoopRow in dtresult.Rows)
      {
        HighRail NewRail = new HighRail();
        NewRail.type = sType;
        NewRail.country = LoopRow["country"].ToString();
        NewRail.week = LoopRow["week"].ToString();
        NewRail.car = LoopRow["car"].ToString();
        NewRail.time = LoopRow["time"].ToString();

        RailList.Add(NewRail);
      }


      dbDapper.Insert(RailList);



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

    private DataTable Transpose(DataTable dt)
    {
      DataTable dtNew = new DataTable();

      //adding columns    
      for (int i = 0; i <= dt.Rows.Count; i++)
      {
        dtNew.Columns.Add(i.ToString());
      }



      //Changing Column Captions: 
      dtNew.Columns[0].ColumnName = " ";

      for (int i = 0; i < dt.Rows.Count; i++)
      {
        //For dateTime columns use like below
        //dtNew.Columns[i + 1].ColumnName = Convert.ToDateTime(dt.Rows[i].ItemArray[0].ToString()).ToString("MM/dd/yyyy");
        //Else just assign the ItermArry[0] to the columnName prooperty
      }

      //Adding Row Data
      for (int k = 1; k < dt.Columns.Count; k++)
      {
        DataRow r = dtNew.NewRow();
        r[0] = dt.Columns[k].ToString();
        for (int j = 1; j <= dt.Rows.Count; j++)
          r[j] = dt.Rows[j - 1][k];
        dtNew.Rows.Add(r);
      }

      return dtNew;
    }
  }
}
