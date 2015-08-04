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
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Data.SqlClient;

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

                //ddlCOUNTY2.DataSource = oDal.DataTable();
                //ddlCOUNTY2.DataValueField = "COUNTY";
                //ddlCOUNTY2.DataTextField = "COUNTY";
                //ddlCOUNTY2.DataBind();

                for (int i = 0  ; i < 24; i++)
                {
                    ddlTimeCountryS.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    ddlTimeCountryE.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    ddlTimeRainS.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    ddlTimeRainE.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    ddlTimeShpS.Items.Add(new ListItem(i.ToString(), i.ToString()));                
                    ddlTimeShpE.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }

                bindRainStation();               

                //ddlCOUNTY.Items.Insert(0, new ListItem("全部", "全部"));

                //ViewState["sortExpression"] = "STNAME";
                //ViewState["sort"] = " ASC"; //or DESC
                //ViewState["sort"] = " DESC";
                //ViewState["sortExpression"] = "STNAME";
                //ViewState["sort"] = " ASC";

                //ddlCOUNTY2_SelectedIndexChanged(sender, new EventArgs());
            }
        }      

        private void bindRainStation()
        {
            ddlRainStation.Items.Clear();
            //return;
            ssql = @"   select * from StationData                         
                        order by county,stname
                    ";
            oDal.CommandText = ssql;

            foreach (DataRow dr in oDal.DataTable().Rows)
            {
                string sText = string.Format("{0}({1}-{2})", dr["STID"].ToString(), dr["county"].ToString(), dr["STNAME"].ToString());
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
                    //xpath_file.Name
                    // 將傳入的檔名以 FileInfo 來進行解析（只以字串無法做）
                    System.Web.HttpContext.Current.Response.Clear(); //清除buffer
                    System.Web.HttpContext.Current.Response.ClearHeaders(); //清除 buffer 表頭
                    System.Web.HttpContext.Current.Response.Buffer = false;
                    System.Web.HttpContext.Current.Response.ContentType = "application/octet-stream";
                    // 檔案類型還有下列幾種"application/pdf"、"application/vnd.ms-excel"、"text/xml"、"text/HTML"、"image/JPEG"、"image/GIF"
                    //System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(out_file, System.Text.Encoding.UTF8));
                    System.Web.HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + System.Web.HttpUtility.UrlEncode(xpath_file.Name, System.Text.Encoding.UTF8));
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
        
        private DateTime TransQueryTime(string sDate, string sTime)
        {
            //08/13/2015
            //0123456789
            //string sResult = string.Empty;

            DateTime dt = DateTime.Now;

            string syyyy, sMM, sdd, shh, smm, sss;
            syyyy = sDate.Substring(6, 4);
            sMM = sDate.Substring(0, 2);
            sdd = sDate.Substring(3, 2);
            shh = sTime;
            smm = "0";
            sss = "0";

            dt = new DateTime(Convert.ToInt32(syyyy), Convert.ToInt32(sMM), Convert.ToInt32(sdd), Convert.ToInt32(shh), 0, 0);


            return dt;
        }

        protected void btnExportCounty_Click(object sender, EventArgs e)
        {
            string sSaveFilePath = @"d:\temp\" +  "CountyData_" + Guid.NewGuid().ToString() + ".xlsx";

            string sCountry = ddlCOUNTY.SelectedValue;
            string sSDate = CountryDateS.Value;
            string sEDate = CountryDateE.Value;
            string sSTime = ddlTimeCountryS.SelectedValue;
            string sETime = ddlTimeCountryE.SelectedValue;

            DateTime dtS = TransQueryTime(sSDate, sSTime);
            DateTime dtE = TransQueryTime(sEDate, sETime);
            try
            {
                ssql = @" select * from RainStation 
                            where COUNTY = '{0}'   
                            and RTime between '{1}' and '{2}'
                            and datepart(mi,RTime) = 0 and datepart(ss,RTime) = 0
                            order by RTime
                        ";
                string sSqlStr = string.Format(ssql, sCountry, dtS.ToString("s"), dtE.ToString("s"));
                ExportBigDataToExcel(sSqlStr,oDal.objCon.ConString, sSaveFilePath);


                FileInfo file = new FileInfo(sSaveFilePath);
                if (file.Exists)
                {
                    xDownload(sSaveFilePath, sSaveFilePath);
                }
                else
                {
                    Response.Write("This file does not exist.");
                }            
            }
            catch (Exception ex)
            {   
                throw;
            }
        }

        protected void btnExportStation_Click(object sender, EventArgs e)
        {
            string sSaveFilePath = @"d:\temp\" + "RainStationData_" + Guid.NewGuid().ToString() + ".xlsx";

            string sRainStation = ddlRainStation.SelectedValue;
            string sSDate = RainDateS.Value;
            string sEDate = RainDateE.Value;
            string sSTime = ddlTimeRainS.SelectedValue;
            string sETime = ddlTimeRainE.SelectedValue;
            
            DateTime dtS = TransQueryTime(sSDate, sSTime);
            DateTime dtE = TransQueryTime(sEDate, sETime);
            try
            {
                ssql = @" select * from RainStation 
                            where STID = '{0}'   
                            and RTime between '{1}' and '{2}'
                            and datepart(mi,RTime) = 0 and datepart(ss,RTime) = 0
                            order by RTime
                        ";
                string sSqlStr = string.Format(ssql, sRainStation, dtS.ToString("s"), dtE.ToString("s"));
                ExportBigDataToExcel(sSqlStr, oDal.objCon.ConString, sSaveFilePath);


                FileInfo file = new FileInfo(sSaveFilePath);
                if (file.Exists)
                {
                    xDownload(sSaveFilePath, sSaveFilePath);
                }
                else
                {
                    Response.Write("This file does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected void btnExportSHP_Click(object sender, EventArgs e)
        {
            string sSaveFilePath = @"d:\temp\" + "RainShpData_" + Guid.NewGuid().ToString() + ".xlsx";

            
            string sSDate = DateShpS.Value;
            string sEDate = DateShpE.Value;
            string sSTime = ddlTimeShpS.SelectedValue;
            string sETime = ddlTimeShpE.SelectedValue;

            DateTime dtS = TransQueryTime(sSDate, sSTime);
            DateTime dtE = TransQueryTime(sEDate, sETime);
            try
            {
                ssql = @" select STID,STNAME,COUNTY,RAIN,LAT,LON,RTIME from RainStation 
                            where 1 = 1   
                            and RTime between '{0}' and '{1}'
                            and datepart(mi,RTime) = 0 and datepart(ss,RTime) = 0
                            order by RTime
                        ";
                string sSqlStr = string.Format(ssql, dtS.ToString("s"), dtE.ToString("s"));
                ExportBigDataToExcel(sSqlStr, oDal.objCon.ConString, sSaveFilePath);


                FileInfo file = new FileInfo(sSaveFilePath);
                if (file.Exists)
                {
                    xDownload(sSaveFilePath, sSaveFilePath);
                }
                else
                {
                    Response.Write("This file does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        static private int rowsPerSheet = 100000;
        static private DataTable ResultsData = new DataTable();

        private void ExportBigDataToExcel(string sSqlText,string sConnStr,string sfileName)
        {
            string queryString = sSqlText;
            ResultsData = new DataTable();

            using (var connection = new SqlConnection(sConnStr))
            {
                var command = new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                int c = 0;
                bool firstTime = true;

                //Get the Columns names, types, this will help when we need to format the cells in the excel sheet.
                DataTable dtSchema = reader.GetSchemaTable();
                var listCols = new List<DataColumn>();
                if (dtSchema != null)
                {
                    foreach (DataRow drow in dtSchema.Rows)
                    {
                        string columnName = Convert.ToString(drow["ColumnName"]);
                        var column = new DataColumn(columnName, (Type)(drow["DataType"]));
                        column.Unique = (bool)drow["IsUnique"];
                        column.AllowDBNull = (bool)drow["AllowDBNull"];
                        column.AutoIncrement = (bool)drow["IsAutoIncrement"];
                        listCols.Add(column);
                        ResultsData.Columns.Add(column);
                    }
                }

                // Call Read before accessing data. 
                while (reader.Read())
                {
                    DataRow dataRow = ResultsData.NewRow();
                    for (int i = 0; i < listCols.Count; i++)
                    {
                        dataRow[(listCols[i])] = reader[i];
                    }
                    ResultsData.Rows.Add(dataRow);
                    c++;
                    if (c == rowsPerSheet)
                    {
                        c = 0;
                        ExportToOxml(firstTime,sfileName);
                        ResultsData.Clear();
                        firstTime = false;
                    }
                }
                if (ResultsData.Rows.Count > 0)
                {
                    ExportToOxml(firstTime, sfileName);
                    ResultsData.Clear();
                }
                // Call Close when done reading.
                reader.Close();
            }
        }


        private static void ExportToOxml(bool firstTime, string fileName)
        {
            //const string fileName = @"C:\MyExcel.xlsx";

            //Delete the file if it exists. 
            if (firstTime && File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            uint sheetId = 1; //Start at the first sheet in the Excel workbook.

            if (firstTime)
            {
                //This is the first time of creating the excel file and the first sheet.
                // Create a spreadsheet document by supplying the filepath.
                // By default, AutoSave = true, Editable = true, and Type = xlsx.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
                    Create(fileName, SpreadsheetDocumentType.Workbook);

                // Add a WorkbookPart to the document.
                WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);


                var bold1 = new Bold();
                CellFormat cf = new CellFormat();


                // Add Sheets to the Workbook.
                Sheets sheets;
                sheets = spreadsheetDocument.WorkbookPart.Workbook.
                    AppendChild<Sheets>(new Sheets());

                // Append a new worksheet and associate it with the workbook.
                var sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.
                        GetIdOfPart(worksheetPart),
                    SheetId = sheetId,
                    Name = "Sheet" + sheetId
                };
                sheets.Append(sheet);

                //Add Header Row.
                var headerRow = new Row();
                foreach (DataColumn column in ResultsData.Columns)
                {
                    var cell = new Cell { DataType = CellValues.String, CellValue = new CellValue(column.ColumnName) };
                    headerRow.AppendChild(cell);
                }
                sheetData.AppendChild(headerRow);

                foreach (DataRow row in ResultsData.Rows)
                {
                    var newRow = new Row();
                    foreach (DataColumn col in ResultsData.Columns)
                    {
                        var cell = new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(row[col].ToString())
                        };
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }
                workbookpart.Workbook.Save();

                spreadsheetDocument.Close();
            }
            else
            {
                // Open the Excel file that we created before, and start to add sheets to it.
                var spreadsheetDocument = SpreadsheetDocument.Open(fileName, true);

                var workbookpart = spreadsheetDocument.WorkbookPart;
                if (workbookpart.Workbook == null)
                    workbookpart.Workbook = new Workbook();

                var worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);
                var sheets = spreadsheetDocument.WorkbookPart.Workbook.Sheets;

                if (sheets.Elements<Sheet>().Any())
                {
                    //Set the new sheet id
                    sheetId = sheets.Elements<Sheet>().Max(s => s.SheetId.Value) + 1;
                }
                else
                {
                    sheetId = 1;
                }

                // Append a new worksheet and associate it with the workbook.
                var sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.
                        GetIdOfPart(worksheetPart),
                    SheetId = sheetId,
                    Name = "Sheet" + sheetId
                };
                sheets.Append(sheet);

                //Add the header row here.
                var headerRow = new Row();

                foreach (DataColumn column in ResultsData.Columns)
                {
                    var cell = new Cell { DataType = CellValues.String, CellValue = new CellValue(column.ColumnName) };
                    headerRow.AppendChild(cell);
                }
                sheetData.AppendChild(headerRow);

                foreach (DataRow row in ResultsData.Rows)
                {
                    var newRow = new Row();

                    foreach (DataColumn col in ResultsData.Columns)
                    {
                        var cell = new Cell
                        {
                            DataType = CellValues.String,
                            CellValue = new CellValue(row[col].ToString())
                        };
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }

                workbookpart.Workbook.Save();

                // Close the document.
                spreadsheetDocument.Close();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("default.aspx", true);
        }

       
    }
}