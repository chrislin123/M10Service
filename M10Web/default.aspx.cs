using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CL.Data;
using System.Data;

namespace M10Web
{
    public partial class _default : System.Web.UI.Page
    {
        //string sConnectionString = Properties.Settings.Default.DBConnectionString;
        ODAL oDal = new ODAL("DBConnectionString");
        string ssql = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                ssql = @"   select distinct COUNTY from StationData 
                        order by COUNTY
                    ";
                oDal.CommandText = ssql;

                ddlCOUNTY.DataSource = oDal.DataTable();
                ddlCOUNTY.DataValueField = "COUNTY";
                ddlCOUNTY.DataTextField = "COUNTY";
                ddlCOUNTY.DataBind();

                ddlCOUNTY.Items.Insert(0, new ListItem("全部", "全部"));

                ViewState["sortExpression"] = "STNAME";
                ViewState["sort"] = " DESC"; //or DESC
                //ViewState["sort"] = " DESC";
                //ViewState["sortExpression"] = "STNAME";
                //ViewState["sort"] = " ASC";

                ddlCOUNTY_SelectedIndexChanged(sender, new EventArgs());
            }
        }

        protected void ddlCOUNTY_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnQuery_Click(sender, new EventArgs());
            return;
            ssql = @"   select * from StationData 
                        where COUNTY = '" + ddlCOUNTY.SelectedValue + @"'
                        order by TOWN
                    ";
            oDal.CommandText = ssql;

            foreach (DataRow dr in oDal.DataTable().Rows)
            {
                //ddlStation.Items.Add(new ListItem(dr["TOWN"].ToString() + dr["STNAME"].ToString(), dr["STID"].ToString()));
            }
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            BindNodeInfo();
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {

            if (ViewState["SortExpression"] == null)
            {
                ViewState["sortExpression"] = e.SortExpression;


                ViewState["sort"] = " DESC";
                BindNodeInfo();

                //if (ViewState["sort"].ToString() == " DESC")
                //{
                //    ViewState["sort"] = " ASC";
                //    BindNodeInfo();
                //}
                //else
                //{
                //    ViewState["sort"] = " DESC";
                //    BindNodeInfo();
                //}
            }
            

        }

        public void BindNodeInfo()
        {
            //ssql = " SELECT * FROM RunTimeRainData where COUNTY = '" + ddlCOUNTY.SelectedValue + "' ";

            //預先轉型態，Gridview可直接排序   
            ssql = @"   select CONVERT(float, MIN10) as MIN10
                        ,CONVERT(float, RAIN) as RAIN
                        ,CONVERT(float, HOUR3) as HOUR3
                        ,CONVERT(float, HOUR6) as HOUR6
                        ,CONVERT(float, HOUR12) as HOUR12
                        ,CONVERT(float, HOUR24) as HOUR24
                        ,CONVERT(float, NOW) as NOW
                        ,ROUND(CONVERT(float, RT),2) as RT
                        ,CONVERT(float, LRTI) as LRTI
                        ,ROUND(CONVERT(float, b.ELRTI),2) as ELRTI
                        ,* from RunTimeRainData a
                        left join StationErrLRTI b on a.STID = b.STID 
                    ";
            if (ddlCOUNTY.SelectedValue != "全部")
            {
                ssql += "where COUNTY = '" + ddlCOUNTY.SelectedValue + "' ";                
            }
            
            oDal.CommandText = ssql;
            DataTable dt = oDal.DataTable();


            if (dt.Rows.Count > 0)
            {
                lblDataTime.Text = "資料時間:" + dt.Rows[0]["RTime"].ToString();
            }

            //1040806 處理資料異常，所有數值改為0
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["STATUS"].ToString() == "-99")
                {
                    dr["MIN10"] = 0;
                    dr["RAIN"] = 0;
                    dr["HOUR3"] = 0;
                    dr["HOUR6"] = 0;
                    dr["HOUR12"] = 0;
                    dr["HOUR24"] = 0;
                    dr["NOW"] = 0;
                    dr["RT"] = 0;
                    dr["LRTI"] = 0;

                }
            }

            DataView myview = dt.DefaultView;
            myview.Sort = ViewState["sortExpression"].ToString() + " " + ViewState["sort"].ToString();
            GridView1.DataSource = myview;
            //NodeGridView.DataKeyNames = new string[] { "node_id" };               //设置主键字段
            GridView1.DataBind();                                                  //绑定GridView控件  
        }

        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //隱藏欄位
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {

                //e.Row.Cells[15].Visible = false;
                
            }
               
            

            if (e.Row.RowType == DataControlRowType.Header)
            {
                foreach (TableCell tc in e.Row.Cells)
                {
                    if (tc.HasControls() == true)
                    {
                        if (((LinkButton)tc.Controls[0]).Text == "Duration")
                        {
                            ((LinkButton)tc.Controls[0]).Text = GetLocalResourceObject("Duration").ToString();
                        }
                        else if (((LinkButton)tc.Controls[0]).Text == "RecordingName")
                        {
                            ((LinkButton)tc.Controls[0]).Text = GetLocalResourceObject("RecordingName").ToString();
                        }
                        else if (((LinkButton)tc.Controls[0]).Text == "FileSize")
                        {
                            ((LinkButton)tc.Controls[0]).Text = GetLocalResourceObject("FileSize").ToString();
                        }

                        if (((LinkButton)tc.Controls[0]).CommandArgument == ViewState["sort"].ToString())
                        {
                            if (ViewState["mySorting"].ToString() == "ASC")
                            {
                                tc.Controls.Add(new LiteralControl("↓"));
                            }
                            else
                            {
                                tc.Controls.Add(new LiteralControl("↑"));
                            }
                        }
                    }
                }
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            //Label1.Text = "目前時間: " + DateTime.Now.ToLongTimeString();

        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            Response.Redirect("RainDataExport.aspx", true);
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[15].Text == "-99")
                {                    
                    e.Row.Cells[15].Text = "異常";
                    //e.Row.Cells[4].Text = "0";
                    //e.Row.Cells[5].Text = "0";
                    //e.Row.Cells[6].Text = "0";
                    //e.Row.Cells[7].Text = "0";
                    //e.Row.Cells[8].Text = "0";
                    //e.Row.Cells[9].Text = "0";
                    //e.Row.Cells[10].Text = "0";
                    //e.Row.Cells[11].Text = "0";
                    //e.Row.Cells[12].Text = "0";

                    e.Row.ForeColor = System.Drawing.Color.LightGray;
                }
                else
                {
                    e.Row.Cells[15].Text = "";
                }
                
                //警戒值超過要變色
                if (e.Row.Cells[13].Text != "" && e.Row.Cells[13].Text != "&nbsp;")
                {
                    double dLRTI = 0;
                    double dELRTI = 0;
                    double.TryParse(e.Row.Cells[12].Text, out dLRTI);
                    double.TryParse(e.Row.Cells[13].Text, out dELRTI);

                    if (dLRTI > dELRTI)
                    {
                        e.Row.ForeColor = System.Drawing.Color.Red;
                    } 
                }
                

            } 



        }
    }
}