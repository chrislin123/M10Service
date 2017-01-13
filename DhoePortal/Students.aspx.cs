using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CL.Data;
using System.Data;
using System.Text;

namespace DhoePortal
{
    public partial class Students : System.Web.UI.Page
    {
        ODAL oDal = new ODAL("DBConnectionString");
        string ssql = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                //取得在校生資料
                GetCurrentStudent();
                //取得在博士資料
                GetPHD();
                //取得碩士資料
                GetMasDeg();
            }
        }



        private void GetCurrentStudent()
        {
            string sDataType = "currst";

            ssql = string.Format(@" select distinct kind from students where datatype = '{0}' ", sDataType);

            oDal.CommandText = ssql;


            DataTable dt = new DataTable();
            dt = oDal.DataTable();

            //Building an HTML string.
            StringBuilder html = new StringBuilder();

            //Table start.
            html.Append("<table class='table'>");
            html.Append("<tbody>");

            //Building the Data rows.
            int iRow = 0;
            foreach (DataRow row in dt.Rows)
            {
                iRow++;

                if (iRow % 2 == 1)
                {
                    html.Append("<tr class='active'>");
                }
                else
                {
                    html.Append("<tr>");
                }

                //col1
                html.Append("<td style='width: 100px;'>");
                html.Append(row["kind"]);
                html.Append("</td>");

                //col2
                html.Append("<td>");

                //取得該類別學生資料
                ssql = string.Format(@" select * from students where datatype = '{0}' and kind = '{1}' ", sDataType, row["kind"]);

                oDal.CommandText = ssql;

                string sContent = string.Empty;

                DataTable dttemp = new DataTable();
                dttemp = oDal.DataTable();
                foreach (DataRow dRow in dttemp.Rows)
                {
                    sContent += dRow["name"].ToString() + "　";
                }


                html.Append(sContent);

                

                html.Append("</td>");

                html.Append("</tr>");
            }


            html.Append("</tbody>");

            //Table end.
            html.Append("</table>");

            //Append the HTML string to Placeholder.
            PlaceHolder1.Controls.Add(new Literal { Text = html.ToString() });

        }

        private void GetPHD()
        {
            string sDataType = "phd";

            ssql = string.Format(@" select distinct kind from students where datatype = '{0}'
                        order by kind desc
                    ", sDataType);

            oDal.CommandText = ssql;


            DataTable dt = new DataTable();
            dt = oDal.DataTable();

            //Building an HTML string.
            StringBuilder html = new StringBuilder();

            //Table start.
            html.Append("<table class='table'>");
            html.Append("<tbody>");

            //Building the Data rows.
            int iRow = 0;
            foreach (DataRow row in dt.Rows)
            {
                iRow++;

                if (iRow % 2 == 1)
                {
                    html.Append("<tr class='active'>");
                }
                else
                {
                    html.Append("<tr>");
                }

                //col1
                html.Append("<td style='width: 100px;' >");
                html.Append(row["kind"]);
                html.Append("</td>");

                //col2
                html.Append("<td>");

                //取得該類別學生資料
                ssql = string.Format(@" select * from students where datatype = '{0}' and kind = '{1}' ", sDataType, row["kind"]);

                oDal.CommandText = ssql;

                string sContent = string.Empty;

                DataTable dttemp = new DataTable();
                dttemp = oDal.DataTable();
                foreach (DataRow dRow in dttemp.Rows)
                {
                    string sName = dRow["name"].ToString();

                    string sData = string.Format("<a href='#' data-trigger='hover' rel='popover' data-html='true' title='Demo{0}' data-content=\"<iframe frameborder='0' scrolling='no' height='500' width='500' src='StudentsD.aspx'></iframe>\">{0}</a>", sName);
                    sContent += sData + "　";
                }


                html.Append(sContent);



                html.Append("</td>");

                html.Append("</tr>");
            }


            html.Append("</tbody>");

            //Table end.
            html.Append("</table>");

            //Append the HTML string to Placeholder.
            PlaceHolder2.Controls.Add(new Literal { Text = html.ToString() });
        }

        private void GetMasDeg()
        {
            string sDataType = "masdeg";

            ssql = string.Format(@" select distinct kind from students where datatype = '{0}'
                        order by kind desc
                    ", sDataType);

            oDal.CommandText = ssql;


            DataTable dt = new DataTable();
            dt = oDal.DataTable();

            //Building an HTML string.
            StringBuilder html = new StringBuilder();

            //Table start.
            html.Append("<table class='table'>");
            html.Append("<tbody>");

            //Building the Data rows.
            int iRow = 0;
            foreach (DataRow row in dt.Rows)
            {
                iRow++;

                if (iRow % 2 == 1)
                {
                    html.Append("<tr class='active'>");
                }
                else
                {
                    html.Append("<tr>");
                }

                //col1
                html.Append("<td style='width: 100px;'>");
                html.Append(row["kind"]);
                html.Append("</td>");

                //col2
                html.Append("<td>");

                //取得該類別學生資料
                ssql = string.Format(@" select * from students where datatype = '{0}' and kind = '{1}' ", sDataType, row["kind"]);

                oDal.CommandText = ssql;

                string sContent = string.Empty;

                DataTable dttemp = new DataTable();
                dttemp = oDal.DataTable();
                foreach (DataRow dRow in dttemp.Rows)
                {
                    sContent += dRow["name"].ToString() + "　";
                }


                html.Append(sContent);



                html.Append("</td>");

                html.Append("</tr>");
            }


            html.Append("</tbody>");

            //Table end.
            html.Append("</table>");

            //Append the HTML string to Placeholder.
            PlaceHolder3.Controls.Add(new Literal { Text = html.ToString() });
        }
    }
}