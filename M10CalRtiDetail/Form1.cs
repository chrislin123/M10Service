using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CL.Data;
using System.Data.Common;
using System.Globalization;


namespace M10CalRtiDetail
{
    public partial class MainForm : Form
    {
        string ssql = string.Empty;
        string sConnectionString = Properties.Settings.Default.DBConnectionString;
        ODAL oDal = new ODAL(Properties.Settings.Default.DBConnectionString);
        DataTable dt_rtidetail = new DataTable();


        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            try
            {
                //建立資料表基本資料
                buildtable();


                dataGridView1.DataSource = dt_rtidetail;
                dataGridView1.AutoGenerateColumns = true;



                DataTable dt = new DataTable();
                DataTable dt1 = new DataTable();

                //取得所有版本號
                ssql = " select distinct ver from RtiData ";
                oDal.CommandText = ssql;
                dt.Clear();
                dt = oDal.DataTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ddlver.Items.Add(dr["ver"].ToString());
                }


                //取得所有站號
                ssql = " select distinct station from RtiData"
                             + "  order by station  "
                             ;
                oDal.CommandText = ssql;
                dt.Clear();
                dt = oDal.DataTable();
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow newdr = dt_rtidetail.NewRow();

                    newdr["station"] = dr["station"].ToString();
                    dt_rtidetail.Rows.Add(newdr);
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }




        private void buildtable()
        {
            dt_rtidetail.Columns.Add("station");
            dt_rtidetail.Columns.Add("delaytime");
            dt_rtidetail.Columns.Add("totalcount");
            dt_rtidetail.Columns.Add("startdate");
            dt_rtidetail.Columns.Add("enddate");
            dt_rtidetail.Columns.Add("rti10");
            dt_rtidetail.Columns.Add("rti30");
            dt_rtidetail.Columns.Add("rti50");
            dt_rtidetail.Columns.Add("rti70");
            dt_rtidetail.Columns.Add("rti90");
        }

        private void btnstart_Click(object sender, EventArgs e)
        {
            try
            {
                int rowindex = 0;
                foreach (DataRow dr in dt_rtidetail.Rows)
                {
                    string sStation = dr["station"].ToString();

                    //移動資料行
                    dataGridView1.CurrentCell = dataGridView1.Rows[rowindex].Cells[0];
                    
                    //取得總筆數                
                    ssql = " select count(*) as total from RtiData "
                         + " where station = '" + sStation + "' ";
                    oDal.CommandText = ssql;
                    string sTotalcount = oDal.Value().ToString().Trim();
                    dr["totalcount"] = sTotalcount;

                    Application.DoEvents();

                    //取得開始時間
                    ssql = " select MIN(date)  from RtiData "
                         + " where station = '" + sStation + "' ";
                    oDal.CommandText = ssql;
                    string sStartDate = oDal.Value().ToString().Trim();
                    dr["startdate"] = sStartDate;

                    Application.DoEvents();


                    //取得結束時間
                    ssql = " select MAX(date)  from RtiData "
                         + " where station = '" + sStation + "' ";
                    oDal.CommandText = ssql;
                    string sEndDate = oDal.Value().ToString().Trim();
                    dr["enddate"] = sEndDate;

                    Application.DoEvents();



                    rowindex++;
                }

                



                
                




                




            }
            catch (Exception)
            {   
                throw;
            }
        }
    }
}
