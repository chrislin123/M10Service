using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using CL.Data;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet; //excel
using ClosedXML.Excel;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Configuration;
using M10.lib;
using M10.lib.model;
using M10AlertLRTI.Models;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;

namespace M10AlertLRTI
{
  public partial class M10kml : BaseForm
  {
    public M10kml()
    {
      InitializeComponent();

      //載入BaseForm資料
      base.InitForm();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      TransKML();
    }

    private void TransKML()
    {
      try
      {


        string sFilePath = @"c:\doc.kml";

        XDocument kml1 = XDocument.Load(sFilePath);

        var ns = XNamespace.Get("http://www.opengis.net/kml/2.2");
        var placemarks = kml1.Element(ns + "kml").Element(ns + "Document").Element(ns + "Folder").Elements(ns + "Placemark");

        foreach (var item in placemarks)
        {
          //取得名稱
          string Name = item.Element(ns + "name").Value;

          ssql = " select * from StationVillageLRTI where village = '{0}' ";
          StationVillageLRTI RelData = dbDapper.QuerySingleOrDefault<StationVillageLRTI>(string.Format(ssql, Name));

          if (RelData != null)
          {

            var LinearRing = item.Element(ns + "MultiGeometry").Element(ns + "Polygon").Element(ns + "outerBoundaryIs").Element(ns + "LinearRing");
            string sAllCoord = LinearRing.Element(ns + "coordinates").Value;

            //依照格式拆解
            string[] CoorDataList = sAllCoord.Replace(" ", "").Replace(",0", "|").Split('|');

            int idx = 1;
            foreach (string LoopItem in CoorDataList)
            {
              //資料空白去除
              if (LoopItem == "") continue;

              string[] aItem = LoopItem.Split(',');

              Coordinate insData = new Coordinate();
              insData.type = "stvillage";
              insData.relano = RelData.no;
              insData.lat = aItem[1];
              insData.lng = aItem[0];
              insData.pointseq = idx;

              dbDapper.Insert(insData);

              idx++;
            }
          }
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex, "");

      }


      MessageBox.Show("OK");
    }

    private void button2_Click(object sender, EventArgs e)
    {
      try
      {


        string sFilePath = @"c:\village.kml";

        XDocument kml1 = XDocument.Load(sFilePath);

        var ns = XNamespace.Get("http://www.opengis.net/kml/2.2");
        var placemarks = kml1.Element(ns + "kml").Element(ns + "Document").Element(ns + "Folder").Elements(ns + "Placemark");
        
        int iTownshipID = 1;
        foreach (var item in placemarks)
        {
          //取得名稱
          string Name = item.Element(ns + "name").Value;


          var LinearRing = item.Element(ns + "MultiGeometry").Element(ns + "Polygon").Element(ns + "outerBoundaryIs").Element(ns + "LinearRing");
          string sAllCoord = LinearRing.Element(ns + "coordinates").Value;

          //依照格式拆解
          string[] CoorDataList = sAllCoord.Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace(",0", "|").Split('|');
          int idx = 1;
          foreach (string LoopItem in CoorDataList)
          {
            //資料空白去除
            if (LoopItem == "") continue;

            string[] aItem = LoopItem.Split(',');

            Coordinate insData = new Coordinate();
            insData.type = "township";
            insData.relano = iTownshipID;
            insData.lat = aItem[1];
            insData.lng = aItem[0];
            insData.pointseq = idx;

            dbDapper.Insert(insData);

            idx++;
          }

          iTownshipID++;
          //ssql = " select * from StationVillageLRTI where village = '{0}' ";
          //StationVillageLRTI RelData = dbDapper.QuerySingleOrDefault<StationVillageLRTI>(string.Format(ssql, Name));

          //if (RelData != null)
          //{

          //  var LinearRing = item.Element(ns + "MultiGeometry").Element(ns + "Polygon").Element(ns + "outerBoundaryIs").Element(ns + "LinearRing");
          //  string sAllCoord = LinearRing.Element(ns + "coordinates").Value;

          //  //依照格式拆解
          //  string[] CoorDataList = sAllCoord.Replace(" ", "").Replace(",0", "|").Split('|');

          //  int idx = 1;
          //  foreach (string LoopItem in CoorDataList)
          //  {
          //    //資料空白去除
          //    if (LoopItem == "") continue;

          //    string[] aItem = LoopItem.Split(',');

          //    Coordinate insData = new Coordinate();
          //    insData.type = "stvillage";
          //    insData.relano = RelData.no;
          //    insData.lat = aItem[1];
          //    insData.lng = aItem[0];
          //    insData.pointseq = idx;

          //    dbDapper.Insert(insData);

          //    idx++;
          //  }
          //}
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex, "");

      }


      MessageBox.Show("OK");
    }

    private void btnCountry_Click(object sender, EventArgs e)
    {
      try
      {


        string sFilePath = @"c:\country.kml";

        XDocument kml1 = XDocument.Load(sFilePath);

        var ns = XNamespace.Get("http://www.opengis.net/kml/2.2");
        var placemarks = kml1.Element(ns + "kml").Element(ns + "Document").Element(ns + "Folder").Elements(ns + "Placemark");

        int iTownshipID = 1;
        foreach (var item in placemarks)
        {
          //取得名稱
          string Name = item.Element(ns + "name").Value;

          //if (Name != "高雄市") continue;

          //一個縣市有多個polygon

          var allPolygon = item.Element(ns + "MultiGeometry").Elements(ns + "Polygon");
          foreach (var Polygon in allPolygon)
          {
            var LinearRing = Polygon.Element(ns + "outerBoundaryIs").Element(ns + "LinearRing");
            string sAllCoord = LinearRing.Element(ns + "coordinates").Value;

            //依照格式拆解
            string[] CoorDataList = sAllCoord.Replace(" ", "").Replace("\n", "").Replace("\t", "").Replace(",0", "|").Split('|');
            int idx = 1;
            foreach (string LoopItem in CoorDataList)
            {
              //資料空白去除
              if (LoopItem == "") continue;

              string[] aItem = LoopItem.Split(',');

              Coordinate insData = new Coordinate();
              insData.type = "country";
              insData.relano = iTownshipID;
              insData.lat = aItem[1];
              insData.lng = aItem[0];
              insData.pointseq = idx;

              dbDapper.Insert(insData);

              idx++;
            }

            iTownshipID++;
          }          
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex, "");
      }

      MessageBox.Show("OK");
    }

    private void button1_Click_1(object sender, EventArgs e)
    {

      string sStartDate = "2016-04-08T00:00:01";
      string sEndDate = "2017-04-09T00:00:01";
      //string country = "桃園市";
      string country = "全部";

      string ssql = @" select * from RainStation where 1=1
                      and RTime between @sd and @ed
                      and datepart(mi,RTime) = 0 and datepart(ss,RTime) = 0
                        ";
      if (country != "全部") ssql += " and COUNTY = @COUNTY ";
      ssql += " order by RTime ";

      List<RainStation> DataList = new List<RainStation>();
      DataList = dbDapper.Query<RainStation>(ssql, new { COUNTY = country, sd = sStartDate, ed = sEndDate });
      
      string sSaveFilePath = @"c:\test1.csv";

      DataTable dt = Utils.ConvertToDataTable<RainStation>(DataList);


      DataExport de = new DataExport();
      

      de.ExportBigDataToCsv(sSaveFilePath, dt);

      MessageBox.Show("Test");
    }
  }

  public class polygon
  {
    public int relano { get; set; }

    public List<point> points { get; set; }
  }

  public class point
  {
    public string lat { get; set; }

    public string lng { get; set; }



  }
}
