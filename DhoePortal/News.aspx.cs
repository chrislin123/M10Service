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
    public partial class News : System.Web.UI.Page
    {
        ODAL oDal = new ODAL("DBConnectionString");
        string ssql = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                ssql = @"   select * from newsm
                        order by date desc
                    ";
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
                        html.Append("<tr style='width: 100px;' class='active'>");
                    }
                    else
                    {
                        html.Append("<tr style='width: 100px;'>");
                    }

                    //col1
                    html.Append("<td>");
                    html.Append(row["date"]);
                    html.Append("</td>");

                    //col2
                    html.Append("<td>");
                    
                    html.Append(string.Format("<a href='{0}'>", row["link"]));

                    html.Append(row["title"]);

                    html.Append("</a>");

                    html.Append("</td>");
               
                    html.Append("</tr>");
                }


                html.Append("</tbody>");

                //Table end.
                html.Append("</table>");

                //Append the HTML string to Placeholder.
                PlaceHolder1.Controls.Add(new Literal { Text = html.ToString() });

            }
        }
    }
}