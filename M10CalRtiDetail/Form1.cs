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
        DataTable dt_rti = new DataTable();
        DataTable dt_rti90 = new DataTable();


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

                //建立資料表基本資料
                buildtableRTI();
                


                dataGridView1.DataSource = dt_rtidetail;
                dataGridView1.AutoGenerateColumns = true;

                dataGridView2.DataSource = dt_rti;
                dataGridView2.AutoGenerateColumns = true;

                dataGridView3.DataSource = dt_rti90;
                dataGridView3.AutoGenerateColumns = true;



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

                ddlTimeDelay.SelectedIndex = 0;
                


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

        private void buildtableRTI()
        {
            dt_rti.Columns.Add("index");
            dt_rti.Columns.Add("station");
            dt_rti.Columns.Add("rti");
            dt_rti.Columns.Add("totalcount");
            dt_rti.Columns.Add("pas");


            dt_rti90.Columns.Add("index");
            dt_rti90.Columns.Add("station");
            dt_rti90.Columns.Add("rti");
            dt_rti90.Columns.Add("totalcount");
            dt_rti90.Columns.Add("pas");


            //dt_rti.Columns.Add("totalcount");
            //dt_rti.Columns.Add("startdate");
            //dt_rti.Columns.Add("enddate");
            //dt_rti.Columns.Add("rti10");
            //dt_rti.Columns.Add("rti30");
            //dt_rti.Columns.Add("rti50");
            //dt_rti.Columns.Add("rti70");
            //dt_rti.Columns.Add("rti90");
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

                    //工作延時(delaytime)
                    dr["totalcount"] = ddlTimeDelay.Text;                    
                    Application.DoEvents();

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

                    //計算RTI10


                    //計算RTI90


                    //計算RTI30


                    //計算RTI50


                    //計算RTI70



                    rowindex++;
                }

            }
            catch (Exception)
            {   
                throw;
            }
        }

        private void btnRTI10_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();



            //取得總筆數                
            ssql = " select count(*) as total from RtiData "
                 + " where station = 'C0A510' ";
            oDal.CommandText = ssql;
            string sTotalcount = oDal.Value().ToString().Trim();            

            Application.DoEvents();


            //取得所有站號
            ssql = " select * from RtiData"
                 + "  where station = 'C0A510'  "
                 + "  order by rti  ";

            oDal.CommandText = ssql;
            dt.Clear();
            dt = oDal.DataTable();
            int iIndex = 1;
            dt_rti.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                DataRow newdr = dt_rti.NewRow();

                newdr["index"] = iIndex.ToString();
                newdr["station"] = dr["station"].ToString();
                newdr["rti"] = dr["rti"].ToString();
                newdr["totalcount"] = sTotalcount;
                dt_rti.Rows.Add(newdr);


                
                double dtotalcount = double.Parse(sTotalcount);
                double dIndex = double.Parse(iIndex.ToString());

                //前三個小時平均 * RT值
                double dResult = dIndex / (dtotalcount + 1) * 100;
                string sResult = Math.Round(dResult, 2).ToString();

                newdr["pas"] = sResult;

                iIndex++;
            }


            //找出最接近10%的數值
            double dRTIA = 0;
            double dRTIB = 0;
            double dPASa = 0;
            double dPASb = 0;
            double dRTI10 = 0;
            double dRTI30 = 0;
            double dRTI50 = 0;
            double dRTI70 = 0;
            double dRTI90 = 0;
          
            bool bCopy = false;
            foreach (DataRow dr in dt_rti.Rows)
            {
                //暫存前一筆
                dPASa = dPASb;
                dRTIA = dRTIB;

                //取得目前這一筆
                dPASb = double.Parse(dr["pas"].ToString());
                dRTIB = double.Parse(dr["rti"].ToString());

                //判斷兩組資料是否為10%中間
                if (dPASa < 10 && dPASb > 10)
                {
                    bCopy = true;
                    //進行計算RTI10


                    dRTI10 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (10 - dPASa)) + dRTIA;

                    dRTI10 = Math.Round(dRTI10, 2);                    
                }


                //複製除了rti10之前的資料到dt_rti90
                if (bCopy)
                {
                    DataRow drn = dt_rti90.NewRow();
                    drn["index"] = dr["index"];
                    drn["station"] = dr["station"];
                    drn["rti"] = dr["rti"];
                    drn["totalcount"] = dr["totalcount"];
                    drn["pas"] = dr["pas"];

                    dt_rti90.Rows.Add(drn);
                }
            }

            //dt_rti90重整
            int irti90 = 1;
            foreach (DataRow dr in dt_rti90.Rows)
            {
                double dtotalcount = double.Parse(dt_rti90.Rows.Count.ToString());
                double dIndex = double.Parse(irti90.ToString());

                //前三個小時平均 * RT值
                double dResult = dIndex / (dtotalcount + 1) * 100;
                string sResult = Math.Round(dResult, 2).ToString();

                dr["index"] = irti90.ToString();
                dr["pas"] = sResult;

                irti90++;
            }

            dRTIA = 0;
            dRTIB = 0;
            dPASa = 0;
            dPASb = 0;

            foreach (DataRow dr in dt_rti90.Rows)
            {
                //暫存前一筆
                dPASa = dPASb;
                dRTIA = dRTIB;

                //取得目前這一筆
                dPASb = double.Parse(dr["pas"].ToString());
                dRTIB = double.Parse(dr["rti"].ToString());

                //判斷兩組資料是否為10%中間
                if (dPASa < 90 && dPASb > 90)
                {
                    //進行計算RTI90
                    dRTI90 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (90 - dPASa)) + dRTIA;

                    dRTI90 = Math.Round(dRTI90, 2);
                }

            }


            dRTI30 = ((dRTI90 - dRTI10) / 8 * 2) + dRTI10;
            dRTI50 = ((dRTI90 - dRTI10) / 8 * 4) + dRTI10;
            dRTI70 = ((dRTI90 - dRTI10) / 8 * 6) + dRTI10;

            
            lblrti10.Text = dRTI10.ToString();
            lblrti30.Text = dRTI30.ToString();
            lblrti50.Text = dRTI50.ToString();
            lblrti70.Text = dRTI70.ToString();
            lblrti90.Text = dRTI90.ToString();
        }
    }
}
