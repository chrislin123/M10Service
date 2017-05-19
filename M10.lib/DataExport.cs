using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Collections;


namespace M10.lib
{
  public class DataExport
  {
    private int rowsPerSheet = 100000;

    //List<string> HeadNameList = new List<string>();
    //List<List<string>> RowDataList = new List<List<string>>();
    

    private DataTable _SrcData = new DataTable();
    private DataTable ResultsData = new DataTable();

    public int RowsPerSheet
    {
      get
      {
        return rowsPerSheet;
      }

      set
      {
        rowsPerSheet = value;
      }
    }


    private Boolean ExportBigDataToExcel(string sfileName, List<string> HeadNameList, List<List<string>> RowDataList)
    {
      Boolean bSuccess = true;

      int c = 0;
      bool firstTime = true;
      try
      {
        //建立結構 from SrcData

        foreach (string sLoop in HeadNameList)
        {
          _SrcData.Columns.Add(sLoop);

          //DataColumn NewColumn = new DataColumn(sLoop);
          //ResultsData.Columns.Add(NewColumn);
          
        }


        foreach (List<string> LoopList in RowDataList)
        {
          foreach (string sLoopCell in LoopList)
          {

          }
        }



        foreach (DataRow dr in _SrcData.Rows)
        {
          DataRow NewRow = ResultsData.NewRow();
          NewRow.ItemArray = dr.ItemArray;
          ResultsData.Rows.Add(NewRow);

          c++;
          //批次筆數
          if (c == rowsPerSheet)
          {
            c = 0;
            ExportToOxml(firstTime, sfileName);
            ResultsData.Clear();
            firstTime = false;
          }
        }

        //最後一次寫入
        if (ResultsData.Rows.Count > 0)
        {
          ExportToOxml(firstTime, sfileName);
          ResultsData.Clear();
        }
      }
      catch (Exception)
      {
        bSuccess = false;
      }

      return bSuccess;
    }

    /// <summary>
    /// 大量資料匯出至Excel，依照設定資料行進行自動分頁(sheet)
    /// </summary>
    /// <param name="SaveFilePath">存檔路徑</param>
    /// <param name="SoureDataTable">來源資料</param>
    /// <returns></returns>
    public Boolean ExportBigDataToExcel(string SaveFilePath, DataTable SoureDataTable)
    {       
      Boolean bSuccess = true;

      _SrcData.Clear();
      _SrcData = SoureDataTable;

      int c = 0;
      bool firstTime = true;
      try
      {
        //建立結構 from SrcData
        foreach (DataColumn LoopColumn in _SrcData.Columns)
        {
          DataColumn NewColumn = new DataColumn(LoopColumn.ColumnName);
          ResultsData.Columns.Add(NewColumn);
        }

        foreach (DataRow dr in _SrcData.Rows)
        {
          DataRow NewRow = ResultsData.NewRow();
          NewRow.ItemArray = dr.ItemArray;
          ResultsData.Rows.Add(NewRow);

          c++;
          //批次筆數
          if (c == rowsPerSheet)
          {
            c = 0;
            ExportToOxml(firstTime, SaveFilePath);
            ResultsData.Clear();
            firstTime = false;
          }
        }

        //最後一次寫入
        if (ResultsData.Rows.Count > 0)
        {
          ExportToOxml(firstTime, SaveFilePath);
          ResultsData.Clear();
        }
      }
      catch (Exception )
      {
        bSuccess = false;
      }

      return bSuccess;
    }

    private void ExportToOxml(bool firstTime, string fileName)
    {
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

    


  }
}
