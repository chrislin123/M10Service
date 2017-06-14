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
