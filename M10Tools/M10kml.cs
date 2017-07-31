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
using System.IO;
using System.Configuration;
using M10.lib;
using M10.lib.model;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;

namespace M10Tools
{
  public partial class M10kml : BaseForm
  {
    public M10kml()
    {
      InitializeComponent();

      //載入BaseForm資料
      base.InitForm();
    }


    private void btnVillage_Click(object sender, EventArgs e)
    {
      try
      {
        string sFilePath = @"c:\village.kml";
        
        XDocument kml1 = XDocument.Load(sFilePath);

        var ns = XNamespace.Get("http://www.opengis.net/kml/2.2");
        var placemarks = kml1.Element(ns + "kml").Element(ns + "Document").Element(ns + "Folder").Elements(ns + "Placemark");


        foreach (var item in placemarks)
        {
          //取得名稱
          string Name = item.Element(ns + "name").Value;

          string sSTID = "";
          string sVillsageID = "";

          //延伸資料，拆解取得相關資料VillageID
          string extenddata = item.Element(ns + "ExtendedData").Value;
          var ExtendDataList = item.Element(ns + "ExtendedData").Element(ns + "SchemaData").Elements(ns + "SimpleData");
          foreach (var LoopData in ExtendDataList)
          {
            if (LoopData.Attribute("name").Value == "雨量站編號")
            {
              sSTID = LoopData.Value;
            }

            if (LoopData.Attribute("name").Value == "VILLAGE_ID")
            {
              sVillsageID = LoopData.Value;
            }

          }
          

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

            MapVillage MapData = new MapVillage();
          
            MapData.Type = "stvillage";
            MapData.VillageID = sVillsageID;
            MapData.Lat = aItem[1];
            MapData.Lng = aItem[0];
            MapData.Pointseq = idx;
            MapData.STID = sSTID;
            dbDapper.Insert(MapData);


            toolStripStatusLabel1.Text = string.Format("{0}：進度({1}/{2})", Name,idx,CoorDataList.Length);
            Application.DoEvents();
            idx++;
          }

          

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
    
       
    private void button2_Click(object sender, EventArgs e)
    {
      try
      {


        string sFilePath = @"c:\township.kml";

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
      ssql = "select * from areacode";

      List<AreaCode> AreaCodeList = dbDapper.Query<AreaCode>(ssql);



      foreach (AreaCode item in AreaCodeList)
      {
        item.CountryID = item.CountryID.Trim();
        item.TownID = item.TownID.Trim();
        item.VillageID = item.VillageID.Trim();

        dbDapper.Update(item);
      }
      MessageBox.Show("Test");
    }

    private void RarButton_Click(object sender, EventArgs e)
    {
      Recursion rr = new Recursion();

      rr.InputSet = rr.MakeCharArray("pass@wo");

      rr.CalcPermutation(0);


      List<string> tt = rr.AllList;



    }

    
  }

  /// <summary>
  /// 排列組合 遞迴
  /// Algorithm Source: A. Bogomolny, Counting And Listing 
  /// All Permutations from Interactive Mathematics Miscellany and Puzzles
  /// http://www.cut-the-knot.org/do_you_know/AllPerm.shtml, Accessed 11 June 2009
  /// </summary>
  class Recursion
  {
    private int elementLevel = -1;
    private int numberOfElements;
    private int[] permutationValue = new int[0];
    public List<string> AllList = new List<string>();

    private char[] inputSet;
    public char[] InputSet
    {
      get { return inputSet; }
      set { inputSet = value; }
    }

    private int permutationCount = 0;
    public int PermutationCount
    {
      get { return permutationCount; }
      set { permutationCount = value; }
    }

    public char[] MakeCharArray(string InputString)
    {
      char[] charString = InputString.ToCharArray();
      Array.Resize(ref permutationValue, charString.Length);
      numberOfElements = charString.Length;
      return charString;
    }

    public void CalcPermutation(int k)
    {
      elementLevel++;
      permutationValue.SetValue(elementLevel, k);

      if (elementLevel == numberOfElements)
      {
        OutputPermutation(permutationValue);
      }
      else
      {
        for (int i = 0; i < numberOfElements; i++)
        {
          if (permutationValue[i] == 0)
          {
            CalcPermutation(i);
          }
        }
      }
      elementLevel--;
      permutationValue.SetValue(0, k);
    }

    private void OutputPermutation(int[] value)
    {
      string result = string.Empty;
      foreach (int i in value)
      {
        result += inputSet.GetValue(i - 1);


        Console.Write(inputSet.GetValue(i - 1));
      }

      AllList.Add(result);
      //MessageBox.Show("Test");
      //Console.WriteLine();
      PermutationCount++;
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
