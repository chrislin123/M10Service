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
    public partial class RtiData : System.Web.UI.Page
    {
        ODAL oDal = new ODAL("DBConnectionString");
        string ssql = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {   
                //排序預設
                ViewState["sortExpression"] = "station";
                ViewState["sort"] = " DESC"; //or DESC              

                //預設選取第一個
                rDelaytime.SelectedIndex = 0;
                rDelaytime_SelectedIndexChanged(sender, e);               
            }
        }

        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {

            if (ViewState["SortExpression"] == null)
            {
                ViewState["sortExpression"] = e.SortExpression;

                if (ViewState["sort"].ToString() == " DESC")
                {
                    ViewState["sort"] = " ASC";
                    BindNodeInfo();
                }
                else
                {
                    ViewState["sort"] = " DESC";
                    BindNodeInfo();
                }
            }
        }

        public void BindNodeInfo()
        {
            //ssql = " SELECT * FROM RunTimeRainData where COUNTY = '" + ddlCOUNTY.SelectedValue + "' ";

            //取得更新最新時間
            //ssql = " select MAX(RTime) as RTime from RunTimeRainData ";
            //oDal.CommandText = ssql;
            //DataTable dttime = oDal.DataTable();

            //if (dttime.Rows.Count > 0)
            //{
            //    lblDataTime.Text = "資料時間:" + dttime.Rows[0]["RTime"].ToString();
            //}


            //預先轉型態，Gridview可直接排序   
//            ssql = @"   select CONVERT(float, MIN10) as MIN10
//                        ,CONVERT(float, RAIN) as RAIN
//                        ,CONVERT(float, HOUR3) as HOUR3
//                        ,CONVERT(float, HOUR6) as HOUR6
//                        ,CONVERT(float, HOUR12) as HOUR12
//                        ,CONVERT(float, HOUR24) as HOUR24
//                        ,CONVERT(float, NOW) as NOW
//                        ,ROUND(CONVERT(float, RT),2) as RT
//                        ,CONVERT(float, LRTI) as LRTI
//                        ,ROUND(CONVERT(float, b.ELRTI),2) as ELRTI
//                        ,* from RunTimeRainData a
//                        left join StationErrLRTI b on a.STID = b.STID 
//                    ";
            ssql = @"   select * from RtiDetail
                        where delaytime = 
                            
                        ";

            ssql = " select * from RtiDetail where delaytime = '{0}' ";
            ssql = string.Format(ssql, rDelaytime.SelectedValue);
            ssql += " order by station ";
          

            oDal.CommandText = ssql;
            DataTable dt = oDal.DataTable();




            //1040806 處理資料異常，所有數值改為0
            //foreach (DataRow dr in dt.Rows)
            //{
            //    if (dr["STATUS"].ToString() == "-99")
            //    {
            //        dr["MIN10"] = 0;
            //        dr["RAIN"] = 0;
            //        dr["HOUR3"] = 0;
            //        dr["HOUR6"] = 0;
            //        dr["HOUR12"] = 0;
            //        dr["HOUR24"] = 0;
            //        dr["NOW"] = 0;
            //        dr["RT"] = 0;
            //        dr["LRTI"] = 0;

            //    }
            //}

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


        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //if (e.Row.Cells[15].Text == "-99")
                //{
                //    e.Row.Cells[15].Text = "異常";              

                //    e.Row.ForeColor = System.Drawing.Color.LightGray;
                //}
                //else
                //{
                //    e.Row.Cells[15].Text = "";
                //}

                //警戒值超過要變色
                //if (e.Row.Cells[13].Text != "" && e.Row.Cells[13].Text != "&nbsp;")
                //{
                //    double dLRTI = 0;
                //    double dELRTI = 0;
                //    double.TryParse(e.Row.Cells[12].Text, out dLRTI);
                //    double.TryParse(e.Row.Cells[13].Text, out dELRTI);

                //    if (dLRTI > dELRTI)
                //    {
                //        e.Row.ForeColor = System.Drawing.Color.Red;
                //    }
                //}
            }



        }

        protected void rDelaytime_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindNodeInfo();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx", true);
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            BindNodeInfo();
        }
    }
}