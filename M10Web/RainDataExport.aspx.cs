using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CL.Data;
using System.Data;
using ClosedXML;
using ClosedXML.Excel;
using System.IO;

namespace M10Web
{
    public partial class RainDataExport : System.Web.UI.Page
    {
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

                ddlCOUNTY2.DataSource = oDal.DataTable();
                ddlCOUNTY2.DataValueField = "COUNTY";
                ddlCOUNTY2.DataTextField = "COUNTY";
                ddlCOUNTY2.DataBind();
                

                //ddlCOUNTY.Items.Insert(0, new ListItem("全部", "全部"));

                ViewState["sortExpression"] = "STNAME";
                ViewState["sort"] = " ASC"; //or DESC
                //ViewState["sort"] = " DESC";
                //ViewState["sortExpression"] = "STNAME";
                //ViewState["sort"] = " ASC";

                ddlCOUNTY2_SelectedIndexChanged(sender, new EventArgs());
            }
        }


        protected void ddlCOUNTY2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlRainStation.Items.Clear();
            //return;
            ssql = @"   select * from StationData 
                        where COUNTY = '" + ddlCOUNTY2.SelectedValue + @"'
                        order by TOWN
                    ";
            oDal.CommandText = ssql;

            foreach (DataRow dr in oDal.DataTable().Rows)
            {
                string sText = string.Format("{0}({1}{2})",dr["STID"].ToString() , dr["TOWN"].ToString() , dr["STNAME"].ToString());
                ddlRainStation.Items.Add(new ListItem(sText, dr["STID"].ToString()));
            }
        }


        public bool xDownload(string xFile, string out_file)
        //xFile 路徑+檔案, 設定另存的檔名
        {
            if (File.Exists(xFile))
            {
                try
                {
                    FileInfo xpath_file = new FileInfo(xFile);  //要 using System.IO;
                    // 將傳入的檔名以 FileInfo 來進行解析（只以字串無法做）
                    System.Web.HttpContext.Current.Response.Clear(); //清除buffer
                    System.Web.HttpContext.Current.Response.ClearHeaders(); //清除 buffer 表頭
                    System.Web.HttpContext.Current.Response.Buffer = false;
                    System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                    // 檔案類型還有下列幾種"application/pdf"、"application/vnd.ms-excel"、"text/xml"、"text/HTML"、"image/JPEG"、"image/GIF"
                    System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(out_file, System.Text.Encoding.UTF8));
                    // 考慮 utf-8 檔名問題，以 out_file 設定另存的檔名
                    System.Web.HttpContext.Current.Response.AppendHeader("Content-Length", xpath_file.Length.ToString()); //表頭加入檔案大小
                    System.Web.HttpContext.Current.Response.WriteFile(xpath_file.FullName);

                    // 將檔案輸出
                    System.Web.HttpContext.Current.Response.Flush();
                    // 強制 Flush buffer 內容
                    System.Web.HttpContext.Current.Response.End();
                    return true;

                }
                catch (Exception)
                { return false; }

            }
            else
                return false;
        } // EOS xDownload(string xFile, string out_file)

        protected void btnExport_Click(object sender, EventArgs e)
        {

            var workbook = new XLWorkbook();

            //var worksheet = workbook.Worksheets.Add("Sample Sheet");




            ssql = "select * from RunTimeRainData";
            oDal.CommandText = ssql;
            DataTable dt = oDal.DataTable();

            workbook.AddWorksheet(dt, "testsheet");

            //XLWorkbook workbook = new XLWorkbook();
            //workbook.AddWorksheet(dt, "Sheet1");
            //workbook.SaveAs(fileName);
            //workbook.Dispose();
            //worksheet.Cell("A1").Value = "Hello World!";


            workbook.SaveAs(@"d:\temp\closedXml.xlsx");
            string PathToExcelFile = @"d:\temp\closedXml.xlsx";

            FileInfo file = new FileInfo(PathToExcelFile);
            if (file.Exists)
            {
                
                Response.Clear();
                Response.ClearHeaders();
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=" + file.Name);
                Response.AddHeader("Content-Type", "application/Excel");
                Response.ContentType = "application/vnd.xls";
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.WriteFile(file.FullName);
                Response.End();
            }
            else
            {
                Response.Write("This file does not exist.");
            }


            xDownload(@"d:\temp\closedXml.xlsx", @"d:\temp\closedXmltest.xlsx");

            //var wb = new XLWorkbook();
            //var ws = wb.Worksheets.First();
            //ws.Cells("A1").Value = "已修改";
            //ws.Protect("LetMeEdit");
            //wb.SaveAs(@"d:\temp\closedXml.xlsx");



        }
    }
}