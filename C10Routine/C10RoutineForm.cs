using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using M10.lib.model;
using M10.lib;

namespace C10Routine
{
  public partial class C10RoutineForm :BaseForm
  {
    public C10RoutineForm()
    {
      InitializeComponent();
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      timer1.Enabled = false;
      try
      {
        ssql = " select * from stockset where settype = 'regconn' ";
        List<StockSet> SetList = dbDapper.Query<StockSet>(ssql);


        foreach (StockSet LoopSet in SetList)
        {
          using (var wc = Utils.getNewWebClient())
          {
            string text = wc.DownloadString(LoopSet.setvalue);
          }
        }
        
      }
      catch (Exception ex)
      {

        logger.Error(ex, "C10RoutineForm");
      }
      finally {
        this.Close();
      }
      
      
      

    }
  }
}
