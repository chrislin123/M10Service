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
            ssql = @"   select distinct COUNTY from StationData 
                        order by COUNTY
                    ";
            oDal.CommandText = ssql;

            ddlCOUNTY.DataSource = oDal.DataTable();
            ddlCOUNTY.DataValueField = "COUNTY";
            ddlCOUNTY.DataTextField = "COUNTY";
            ddlCOUNTY.DataBind();
        }

        protected void ddlCOUNTY_SelectedIndexChanged(object sender, EventArgs e)
        {
            ssql = @"   select * from StationData 
                        where COUNTY = '" + ddlCOUNTY.SelectedValue + @"'
                        order by TOWN
                    ";
            oDal.CommandText = ssql;

            foreach (DataRow dr in oDal.DataTable().Rows)
            {                
                ddlStation.Items.Add(new ListItem(dr["TOWN"].ToString() + dr["STNAME"].ToString(), dr["STID"].ToString()));
            }
        }
    }
}