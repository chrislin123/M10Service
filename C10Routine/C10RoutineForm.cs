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
using System.Net.Http;

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


    //public async Task getLocalIp()
    //{
    //  //抓取localip對應的station 
    //  HttpClient client = new HttpClient();
    //  HttpResponseMessage response = await client.GetAsync("http://his.cmuh.org.tw/WebApi/PatientInfo/QueryInpPatient/GetStationByIP");
    //  response.EnsureSuccessStatusCode();
    //  string responseBody = await response.Content.ReadAsStringAsync();
    //  labStation.Text = "";
    //  labStation.Text = responseBody.Replace(@"""", "");
    //}

    public async Task<string> getLocalIp()
    {
      //抓取localip對應的station 
      HttpClient client = new HttpClient();
      
      HttpResponseMessage response = await client.GetAsync("http://140.116.38.211/C10Mvc/StockApi/getStockRealtime?stockcode=3313");
      response.EnsureSuccessStatusCode();
      string responseBody = await response.Content.ReadAsStringAsync();     

      return responseBody;
    }
    

    private async void C10RoutineForm_Load(object sender, EventArgs e)
    {
      string  intResult = await getLocalIp();
      this.Text = intResult;
      
      //this.Text = task.Result;


      //this.Text = task.Result;

      //Task<string> task = Task.Factory.StartNew<string>(async () =>
      //{
      //  HttpClient client = new HttpClient();

      //  HttpResponseMessage response = await client.GetAsync("");
      //  response.EnsureSuccessStatusCode();
      //  string responseBody = await response.Content.ReadAsStringAsync();

      //  return responseBody });

      //Task<string> task = Task.Run(async () => await getLocalIp());
      //task.Wait();
      //this.Text = task.Result;

      //MessageBox.Show(task.Result);
    }
  }
}
