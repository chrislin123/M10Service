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
                ddltype.SelectedIndex = 0;

                //建立資料表基本資料
                buildtable();
                
                dataGridView1.DataSource = dt_rtidetail;
                dataGridView1.AutoGenerateColumns = true;
                
                DataTable dt = new DataTable();

                //取得所有版本號
                ssql = " select distinct ver from RtiData ";
                oDal.CommandText = ssql;
                dt.Clear();
                dt = oDal.DataTable();
                foreach (DataRow dr in dt.Rows)
                {
                    ddlver.Items.Add(dr["ver"].ToString());
                }

                ddlver.SelectedIndex = dt.Rows.Count - 1;
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
                    ddlstation.Items.Add(dr["station"].ToString());
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

                    //工作延時(delaytime)
                    dr["delaytime"] = ddlTimeDelay.Text;                    
                    Application.DoEvents();

                    //移動資料行
                    dataGridView1.CurrentCell = dataGridView1.Rows[rowindex].Cells[0];
                    
                    //取得總筆數                
                    ssql = " select count(*) as total from RtiData "
                         + " where station = '" + sStation + "' "
                         + " and ver = '" + ddlver.Text + "' "
                         ;
                    if (ddlTimeDelay.Text != "0")
                    {
                        ssql += " and raindelay > " + ddlTimeDelay.Text + " ";
                    }
                    oDal.CommandText = ssql;
                    string sTotalcount = oDal.Value().ToString().Trim();
                    dr["totalcount"] = sTotalcount;

                    Application.DoEvents();

                    //取得開始時間
                    ssql = " select MIN(date)  from RtiData "
                         + " where station = '" + sStation + "' "
                         + " and ver = '" + ddlver.Text + "' "
                         ;
                    if (ddlTimeDelay.Text != "0")
                    {
                        ssql += " and raindelay > " + ddlTimeDelay.Text + " ";
                    }
                    oDal.CommandText = ssql;
                    string sStartDate = oDal.Value().ToString().Trim();
                    dr["startdate"] = sStartDate;

                    Application.DoEvents();


                    //取得結束時間
                    ssql = " select MAX(date)  from RtiData "
                         + " where station = '" + sStation + "' "
                         + " and ver = '" + ddlver.Text + "' "
                         ;
                    if (ddlTimeDelay.Text != "0")
                    {
                        ssql += " and raindelay > " + ddlTimeDelay.Text + " ";
                    }
                    oDal.CommandText = ssql;
                    string sEndDate = oDal.Value().ToString().Trim();
                    dr["enddate"] = sEndDate;

                    Application.DoEvents();


                    RtiProc oRtiProc = new RtiProc(ddlver.Text, sStation, ddlTimeDelay.Text, sConnectionString,ddltype.Text);
                    //進行計算
                    oRtiProc.RtiCal();

                    //計算RTI10
                    dr["rti10"] = oRtiProc.dRTI10.ToString();
                    Application.DoEvents();
                    
                    //計算RTI30
                    dr["rti30"] = oRtiProc.dRTI30.ToString();
                    Application.DoEvents();

                    //計算RTI50
                    dr["rti50"] = oRtiProc.dRTI50.ToString();
                    Application.DoEvents();

                    //計算RTI70
                    dr["rti70"] = oRtiProc.dRTI70.ToString();
                    Application.DoEvents();

                    //計算RTI90
                    dr["rti90"] = oRtiProc.dRTI90.ToString();
                    Application.DoEvents();

                    //寫入資料庫
                    InsertData(dr);

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

            try
            {
                //string sStation = "C0A530";
                string sStation = ddlstation.Text;

                RtiProc oRtiProc = new RtiProc(ddlver.Text, sStation, ddlTimeDelay.Text, sConnectionString,ddltype.Text);
                //進行計算
                oRtiProc.RtiCal();


                dataGridView2.DataSource = oRtiProc.dt_rti;
                dataGridView2.AutoGenerateColumns = true;

                dataGridView3.DataSource = oRtiProc.dt_rti90;
                dataGridView3.AutoGenerateColumns = true;


                lblrti10.Text = oRtiProc.dRTI10.ToString();
                lblrti30.Text = oRtiProc.dRTI30.ToString();
                lblrti50.Text = oRtiProc.dRTI50.ToString();
                lblrti70.Text = oRtiProc.dRTI70.ToString();
                lblrti90.Text = oRtiProc.dRTI90.ToString();

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            
        }



        private void InsertData(DataRow dr)
        {
            try
            {
                if (ddltype.Text == "RTI")
                {
                    //刪除資料
                    ssql = " delete from RtiDetail "
                         + " where station = '" + dr["station"].ToString() + "' "
                         + " and delaytime = '" + dr["delaytime"].ToString() + "' "
                         ;
                    oDal.CommandText = ssql;
                    oDal.ExecuteSql();

                    //寫入資料
                    ssql = " INSERT INTO RtiDetail "
                            + " ([station]"
                        + " ,[delaytime]"
                        + " ,[totalcount]"
                        + " ,[startdate]"
                        + " ,[enddate]"
                        + " ,[rti10]"
                        + " ,[rti30]"
                        + " ,[rti50]"
                        + " ,[rti70]"
                        + " ,[rti90])"
                        + " VALUES "
                        + " ( "
                        + " '" + dr["station"].ToString() + "' "
                        + " ,'" + dr["delaytime"].ToString() + "' "
                        + " ,'" + dr["totalcount"].ToString() + "' "
                        + " ,'" + dr["startdate"].ToString() + "' "
                        + " ,'" + dr["enddate"].ToString() + "' "
                        + " ,'" + dr["rti10"].ToString() + "' "
                        + " ,'" + dr["rti30"].ToString() + "' "
                        + " ,'" + dr["rti50"].ToString() + "' "
                        + " ,'" + dr["rti70"].ToString() + "' "
                        + " ,'" + dr["rti90"].ToString() + "' "
                            + " ) "
                            ;

                    oDal.CommandText = ssql;
                    oDal.ExecuteSql();

                }

                if (ddltype.Text == "RTI3")
                {
                    //刪除資料
                    ssql = " delete from Rti3Detail "
                         + " where station = '" + dr["station"].ToString() + "' "
                         + " and delaytime = '" + dr["delaytime"].ToString() + "' "
                         ;
                    oDal.CommandText = ssql;
                    oDal.ExecuteSql();

                    //寫入資料
                    ssql = " INSERT INTO Rti3Detail "
                            + " ([station]"
                        + " ,[delaytime]"
                        + " ,[totalcount]"
                        + " ,[startdate]"
                        + " ,[enddate]"
                        + " ,[rti10]"
                        + " ,[rti30]"
                        + " ,[rti50]"
                        + " ,[rti70]"
                        + " ,[rti90])"
                        + " VALUES "
                        + " ( "
                        + " '" + dr["station"].ToString() + "' "
                        + " ,'" + dr["delaytime"].ToString() + "' "
                        + " ,'" + dr["totalcount"].ToString() + "' "
                        + " ,'" + dr["startdate"].ToString() + "' "
                        + " ,'" + dr["enddate"].ToString() + "' "
                        + " ,'" + dr["rti10"].ToString() + "' "
                        + " ,'" + dr["rti30"].ToString() + "' "
                        + " ,'" + dr["rti50"].ToString() + "' "
                        + " ,'" + dr["rti70"].ToString() + "' "
                        + " ,'" + dr["rti90"].ToString() + "' "
                            + " ) "
                            ;

                    oDal.CommandText = ssql;
                    oDal.ExecuteSql();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            //全部轉檔rti
            for (int i = 0; i < ddlTimeDelay.Items.Count; i++)
            {
                ddlTimeDelay.SelectedIndex = i;
                Application.DoEvents();

                btnstart_Click(sender, e);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }

    public class RtiProc
    {
        string ssql = string.Empty;
        string sver = string.Empty;
        string sstation = string.Empty;
        string sdelaytime = string.Empty;
        string stype = string.Empty;
        //string sConnectionString = Properties.Settings.Default.DBConnectionString;
        ODAL oDal;        
        public DataTable dt_rti = new DataTable();
        public DataTable dt_rti90 = new DataTable();

        public double dRTI10 = 0;
        public double dRTI30 = 0;
        public double dRTI50 = 0;
        public double dRTI70 = 0;
        public double dRTI90 = 0;



        public RtiProc(string ver, string station, string delaytime, string sConnectionString,string type)
        {
            sver = ver;
            sstation = station;
            sdelaytime = delaytime;
            stype = type;

            oDal = new ODAL(sConnectionString);
            buildtableRTI();
        }


        public void RtiCal(){
            try
            {
                DataTable dt = new DataTable();

                //取得總筆數                
                ssql = " select count(*) as total from RtiData "
                     + " where station = '" + sstation + "' "
                     + " and ver = '" + sver + "' "
                     ;

                if (sdelaytime != "0")
                {
                    ssql += " and raindelay > " + sdelaytime + " ";
                }

                oDal.CommandText = ssql;
                string sTotalcount = oDal.Value().ToString().Trim();


                //取得所有站號
                ssql = " select * from RtiData"
                     + "  where station = '" + sstation + "'  "
                     + " and ver = '" + sver + "' "
                     ;

                if (sdelaytime != "0")
                {
                    ssql += " and raindelay > " + sdelaytime + " ";
                }

                if (stype == "RTI") ssql += " order by rti  ";
                if (stype == "RTI3") ssql += " order by rti3  ";
                

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
                    if (stype == "RTI") newdr["rti"] = dr["rti"].ToString();
                    if (stype == "RTI3") newdr["rti"] = dr["rti3"].ToString();
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


                //bool bCopy = false;
                foreach (DataRow dr in dt_rti.Rows)
                {
                    //暫存前一筆
                    dPASa = dPASb;
                    dRTIA = dRTIB;

                    //取得目前這一筆
                    dPASb = double.Parse(dr["pas"].ToString());
                    dRTIB = double.Parse(dr["rti"].ToString());

                    //數值剛好等於10，則該數值為rti10
                    if (dPASb == 10)
                    {   
                        dRTI10 = dRTIB;
                    }
                    else
                    {
                        //判斷兩組資料是否為10%中間
                        if (dPASa < 10 && dPASb > 10)
                        {   
                            //進行計算RTI10
                            dRTI10 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (10 - dPASa)) + dRTIA;
                            dRTI10 = Math.Round(dRTI10, 2);
                        }
                    }                   

                    //複製除了rti10之前的資料到dt_rti90
                    if (dPASb > 10)
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


                    if (dPASb == 90)
                    {
                        dRTI90 = dRTIB;
                    }
                    else
                    {
                        //判斷兩組資料是否為90%中間
                        if (dPASa < 90 && dPASb > 90)
                        {
                            //進行計算RTI90
                            dRTI90 = ((dRTIB - dRTIA) / (dPASb - dPASa) * (90 - dPASa)) + dRTIA;
                            dRTI90 = Math.Round(dRTI90, 2);
                        }
                    }          



                }


                dRTI30 = ((dRTI90 - dRTI10) / 8 * 2) + dRTI10;
                dRTI50 = ((dRTI90 - dRTI10) / 8 * 4) + dRTI10;
                dRTI70 = ((dRTI90 - dRTI10) / 8 * 6) + dRTI10;

                //小數點以下兩位進位
                dRTI30 = Math.Round(dRTI30, 2);
                dRTI50 = Math.Round(dRTI50, 2);
                dRTI70 = Math.Round(dRTI70, 2);

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
        }
    }
}
