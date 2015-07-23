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

                ViewState["sortExpression"] = "STNAME";
                ViewState["sort"] = " ASC"; //or DESC
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
            ssql = " SELECT * FROM RunTimeRainData where COUNTY = '" + ddlCOUNTY.SelectedValue + "' ";
            oDal.CommandText = ssql;
            DataTable dt = oDal.DataTable();


            if (dt.Rows.Count > 0)
            {
                lblDataTime.Text = "資料時間:" + dt.Rows[0]["RTime"].ToString();
            }

            DataView myview = dt.DefaultView;
            myview.Sort = ViewState["sortExpression"].ToString() + " " + ViewState["sort"].ToString();
            GridView1.DataSource = myview;
            //NodeGridView.DataKeyNames = new string[] { "node_id" };               //设置主键字段
            GridView1.DataBind();                                                  //绑定GridView控件  
        }

        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
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
    }
}