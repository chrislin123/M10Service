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
    public partial class RtiControl : System.Web.UI.Page
    {
        ODAL oDal = new ODAL("DBConnectionString");
        string ssql = string.Empty;
        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                ssql = " select value from LRTIAlertMail where type = 'isal' and value = 'Y' ";
                oDal.CommandText = ssql;
                DataTable dt = oDal.DataTable();

                if (dt.Rows.Count > 0)
                {
                    rRtiControl.SelectedIndex = 0;
                }
                else
                {
                    rRtiControl.SelectedIndex = 1;
                }

                

            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx", true);
        }

        protected void btnsave_Click(object sender, EventArgs e)
        {

            if (chkPassword(txtPassword.Text) == false)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "msg", "alert('密碼錯誤!')", true);
                return;               
            }


            string sRtiControl = "Y";
            if (rRtiControl.SelectedIndex == 0)
            {
                sRtiControl = "Y";
            }
            else
            {
                sRtiControl = "N";
            }


            ssql = @" UPDATE LRTIAlertMail
                        SET value = '{0}'
                        WHERE type = 'isal' ";
            oDal.CommandText = string.Format(ssql, sRtiControl);
            oDal.ExecuteSql();

            Response.Redirect("default.aspx", true);
        }

        private Boolean chkPassword(string sPsw)
        {
            Boolean bResult = false;

            List<string> lPassword = new List<string>();
            //取得密碼
            ssql = " select value from LRTIAlertMail where type in ('usp1','usp2') ";
            oDal.CommandText = ssql;
           
            DataTable dt = oDal.DataTable();
            foreach (DataRow dr in dt.Rows)
            {
                if (string.Compare(sPsw.ToUpper(), dr["value"].ToString(), true) == 0)
                {
                    bResult = true;
                }
               
            }
            
            return bResult;
        }
    }
}