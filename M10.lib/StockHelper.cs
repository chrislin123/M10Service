using M10.lib.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace M10.lib
{
  public class StockHelper : M10BaseClass
  {
    public bool GetStockAfterTse(DateTime GetDatetime)
    {
      //bool bResult = false;
      string sLineTrans = "";
      try
      {
        string sDate = Utils.getDataString(GetDatetime, M10Const.DataStringType.ADT1);
        string sUrl = string.Format(M10Const.StockAfterTseUrl, sDate);
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sUrl);
        req.Proxy = null;

        string sTemp = "";
        using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
        {
          //判斷http回應狀態(HttpStatusCode.OK=200)
          if (resp.StatusCode != HttpStatusCode.OK)
          {
            System.Threading.Thread.Sleep(3000);
            return false;
          }

          using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.GetEncoding(950)))
          {
            sTemp = SR.ReadToEnd();
          }
        }

        using (Stream s = GenerateStreamFromString(sTemp))
        {
          using (StreamReader sr = new StreamReader(s))
          {
            string Line;
            while ((Line = sr.ReadLine()) != null)
            {
              Line = Line.Replace(" ", "");
              Line = Line.Replace("\",\"", "|");
              Line = Line.Replace("\"", "");
              Line = Line.Replace(",", "");
              Line = Line.Replace("=", "");
              sLineTrans = Line;
              string[] aCol = Line.Split('|');

              if (aCol.Length == 16)
              {
                //檢核資料
                Decimal iCheck = -1;

                if (Decimal.TryParse(aCol[8], out iCheck) == false)
                {
                  continue;
                }

                ssql = " select * from stockafter  where stockdate = '{0}' and stockcode = '{1}'  ";
                Stockafter sa = dbDapper.QuerySingleOrDefault<Stockafter>(string.Format(ssql, sDate, aCol[0]));

                decimal dpricelastbuy = 0;
                decimal.TryParse(aCol[11], out dpricelastbuy);

                decimal dpricelastsell = 0;
                decimal.TryParse(aCol[13], out dpricelastsell);

                if (sa == null)
                {
                  sa = new Stockafter();
                  sa.stockdate = sDate;
                  sa.stocktype = M10Const.StockType.tse;
                  sa.stockcode = aCol[0];
                  sa.pricelast = Convert.ToDecimal(aCol[8]);
                  sa.updown = aCol[9];
                  sa.pricediff = aCol[10];
                  sa.priceopen = Convert.ToDecimal(aCol[5]);
                  sa.pricetop = Convert.ToDecimal(aCol[6]);
                  sa.pricelow = Convert.ToDecimal(aCol[7]);
                  sa.priceavg = 0;
                  sa.dealnum = Convert.ToInt64(aCol[2]);
                  sa.dealmoney = Convert.ToInt64(aCol[4]);
                  sa.dealamount = Convert.ToInt64(aCol[3]);
                  sa.pricelastbuy = dpricelastbuy;
                  sa.pricelastsell = dpricelastsell;
                  sa.publicnum = 0;
                  sa.pricenextday = Convert.ToDecimal(aCol[8]);
                  sa.pricenextlimittop = 0;
                  sa.pricenextlimitlow = 0;
                  sa.updatetime = Utils.getDatatimeString();
                  dbDapper.Insert(sa);
                }
                else
                {
                  sa.pricelast = Convert.ToDecimal(aCol[8]);
                  sa.updown = aCol[9];
                  sa.pricediff = aCol[10];
                  sa.priceopen = Convert.ToDecimal(aCol[5]);
                  sa.pricetop = Convert.ToDecimal(aCol[6]);
                  sa.pricelow = Convert.ToDecimal(aCol[7]);
                  sa.priceavg = 0;
                  sa.dealnum = Convert.ToInt64(aCol[2]);
                  sa.dealmoney = Convert.ToInt64(aCol[4]);
                  sa.dealamount = Convert.ToInt64(aCol[3]);
                  sa.pricelastbuy = dpricelastbuy;
                  sa.pricelastsell = dpricelastsell;
                  sa.publicnum = 0;
                  sa.pricenextday = Convert.ToDecimal(aCol[8]);
                  sa.pricenextlimittop = 0;
                  sa.pricenextlimitlow = 0;
                  sa.updatetime = Utils.getDatatimeString();
                  dbDapper.Update(sa);
                }
              }
            }
          }
        }


      }
      catch (Exception ex)
      {
        logger.Error(ex, "stock after:" + sLineTrans);
        //System.Threading.Thread.Sleep(10000);
        return false;
      }

      return true;
    }

    public bool GetStockAfterOtc(DateTime GetDatetime)
    {
      try
      {
        string sDate = Utils.getDataString(GetDatetime, M10Const.DataStringType.ChineseT2);
        string sUrl = string.Format(M10Const.StockAfterOtcUrl, sDate);
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sUrl);
        req.Proxy = null;

        //改為寫入資料庫格式
        sDate = Utils.getDataString(GetDatetime, M10Const.DataStringType.ADT1);

        string sTemp = "";
        using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
        {
          //判斷http回應狀態(HttpStatusCode.OK=200)
          if (resp.StatusCode != HttpStatusCode.OK)
          {
            System.Threading.Thread.Sleep(3000);
            return false;
          }

          using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.GetEncoding(950)))
          {
            sTemp = SR.ReadToEnd();
          }
        }


        using (Stream s = GenerateStreamFromString(sTemp))
        {

          using (StreamReader sr = new StreamReader(s))
          {

            string Line;
            while ((Line = sr.ReadLine()) != null)
            {
              Line = Line.Replace(" ", "");
              Line = Line.Replace("\",\"", "|");
              Line = Line.Replace("\"", "");
              Line = Line.Replace(",", "");
              Line = Line.Replace("=", "");

              string[] aCol = Line.Split('|');

              if (aCol.Length == 17)
              {
                //檢核資料
                Decimal iCheck = -1;

                if (Decimal.TryParse(aCol[2], out iCheck) == false)
                {
                  continue;
                }

                //資訊整理
                string sDiff = aCol[3];
                string sUpdown = "X";
                string sPricediff = "0.00";
                if (sDiff.Length > 0)
                {
                  string sCheck = sDiff.Substring(0, 1);
                  if (sCheck == "+")
                  {
                    sUpdown = "+";
                    sPricediff = sDiff.Replace("+", "");
                  }
                  if (sCheck == "-")
                  {
                    sUpdown = "-";
                    sPricediff = sDiff.Replace("-", "");
                  }
                  if (sCheck == "0")
                  {
                    sUpdown = "X";
                  }

                  if (sCheck != "+" && sCheck != "-" && sCheck != "0")
                  {
                    sUpdown = "X";
                  }
                }

                ssql = " select * from stockafter  where stockdate = '{0}' and stockcode = '{1}'  ";
                Stockafter sa = dbDapper.QuerySingleOrDefault<Stockafter>(string.Format(ssql, sDate, aCol[0]));

                if (aCol[0] == "3226")
                {
                  string aaaa = string.Empty;
                }
                if (sa == null)
                {
                  sa = new Stockafter();
                  sa.stockdate = sDate;
                  sa.stocktype = M10Const.StockType.otc;
                  sa.stockcode = aCol[0];
                  sa.pricelast = Convert.ToDecimal(aCol[2]);
                  sa.updown = sUpdown;
                  sa.pricediff = sPricediff;
                  sa.priceopen = Convert.ToDecimal(aCol[4]);
                  sa.pricetop = Convert.ToDecimal(aCol[5]);
                  sa.pricelow = Convert.ToDecimal(aCol[6]);
                  sa.priceavg = Convert.ToDecimal(aCol[7]);
                  sa.dealnum = Convert.ToInt64(aCol[8]);
                  sa.dealmoney = Convert.ToInt64(aCol[9]);
                  sa.dealamount = Convert.ToInt64(aCol[10]);
                  sa.pricelastbuy = Convert.ToDecimal(aCol[11]);
                  sa.pricelastsell = Convert.ToDecimal(aCol[12]);
                  sa.publicnum = Convert.ToInt64(aCol[13]);
                  sa.pricenextday = Convert.ToDecimal(aCol[14]);
                  sa.pricenextlimittop = Convert.ToDecimal(aCol[15]);
                  sa.pricenextlimitlow = Convert.ToDecimal(aCol[16]);
                  sa.updatetime = Utils.getDatatimeString();
                  dbDapper.Insert(sa);
                }
                else
                {
                  sa.pricelast = Convert.ToDecimal(aCol[2]);
                  sa.updown = sUpdown;
                  sa.pricediff = sPricediff;
                  sa.priceopen = Convert.ToDecimal(aCol[4]);
                  sa.pricetop = Convert.ToDecimal(aCol[5]);
                  sa.pricelow = Convert.ToDecimal(aCol[6]);
                  sa.priceavg = Convert.ToDecimal(aCol[7]);
                  sa.dealnum = Convert.ToInt64(aCol[8]);
                  sa.dealmoney = Convert.ToInt64(aCol[9]);
                  sa.dealamount = Convert.ToInt64(aCol[10]);
                  sa.pricelastbuy = Convert.ToDecimal(aCol[11]);
                  sa.pricelastsell = Convert.ToDecimal(aCol[12]);
                  sa.publicnum = Convert.ToInt64(aCol[13]);
                  sa.pricenextday = Convert.ToDecimal(aCol[14]);
                  sa.pricenextlimittop = Convert.ToDecimal(aCol[15]);
                  sa.pricenextlimitlow = Convert.ToDecimal(aCol[16]);
                  sa.updatetime = Utils.getDatatimeString();
                  dbDapper.Update(sa);
                }


              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        System.Threading.Thread.Sleep(10000);
        return false;
      }

      return true;
    }


    public bool GetStockThreeTradeTse(DateTime GetDatetime)
    {
      try
      {
        string sDate = Utils.getDataString(GetDatetime, M10Const.DataStringType.ADT1);
        string sUrl = string.Format(M10Const.StockThreeTradeOtc, sDate);

        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(string.Format(sUrl, sDate));
        req.Proxy = null;

        string sTemp = "";
        using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
        {
          //判斷http回應狀態(HttpStatusCode.OK=200)
          if (resp.StatusCode != HttpStatusCode.OK)
          {
            System.Threading.Thread.Sleep(3000);
            return false;
          }

          using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.GetEncoding(950)))
          {
            sTemp = SR.ReadToEnd();
          }
        }


        using (Stream s = GenerateStreamFromString(sTemp))
        {
          using (StreamReader sr = new StreamReader(s))
          {
            string Line;
            while ((Line = sr.ReadLine()) != null)
            {
              Line = Line.Replace(" ", "");
              Line = Line.Replace("\",\"", "|");
              Line = Line.Replace("\"", "");
              Line = Line.Replace(",", "");
              Line = Line.Replace("=", "");

              string[] aCol = Line.Split('|');

              if (aCol.Length == 16)
              {
                //檢核資料
                int iCheck = -1;

                if (int.TryParse(aCol[15], out iCheck) == false)
                {
                  continue;
                }

                ssql = " select * from Stockthreetrade where date = '{0}' and stockcode = '{1}' ";
                Stockthreetrade st = dbDapper.QuerySingleOrDefault<Stockthreetrade>(string.Format(ssql, sDate, aCol[0]));

                if (st == null)
                {
                  st = new Stockthreetrade();
                  st.stockcode = aCol[0];
                  st.date = sDate;
                  st.type = "tse";
                  st.foreigninv = Convert.ToInt32(aCol[4]);
                  st.trustinv = Convert.ToInt32(aCol[7]);
                  st.selfempinv = Convert.ToInt32(aCol[14]);
                  st.threeinv = Convert.ToInt32(aCol[15]);
                  st.updatetime = Utils.getDatatimeString();
                  dbDapper.Insert(st);
                }
                else
                {
                  st.stockcode = aCol[0];
                  st.date = sDate;
                  st.type = "tse";
                  st.foreigninv = Convert.ToInt32(aCol[4]);
                  st.trustinv = Convert.ToInt32(aCol[7]);
                  st.selfempinv = Convert.ToInt32(aCol[14]);
                  st.threeinv = Convert.ToInt32(aCol[15]);
                  st.updatetime = Utils.getDatatimeString();
                  dbDapper.Update(st);
                }
              }

              if (aCol.Length == 12)
              {
                //檢核資料
                int iCheck = -1;

                if (int.TryParse(aCol[11], out iCheck) == false)
                {
                  continue;
                }

                ssql = " select * from Stockthreetrade where date = '{0}' and stockcode = '{1}' ";
                Stockthreetrade st = dbDapper.QuerySingleOrDefault<Stockthreetrade>(string.Format(ssql, sDate, aCol[0]));

                if (st == null)
                {
                  st = new Stockthreetrade();
                  st.stockcode = aCol[0];
                  st.date = sDate;
                  st.type = "tse";
                  st.foreigninv = Convert.ToInt32(aCol[4]);
                  st.trustinv = Convert.ToInt32(aCol[7]);
                  st.selfempinv = Convert.ToInt32(aCol[10]);
                  st.threeinv = Convert.ToInt32(aCol[11]);
                  st.updatetime = Utils.getDatatimeString();
                  dbDapper.Insert(st);
                }
                else
                {
                  st.foreigninv = Convert.ToInt32(aCol[4]);
                  st.trustinv = Convert.ToInt32(aCol[7]);
                  st.selfempinv = Convert.ToInt32(aCol[10]);
                  st.threeinv = Convert.ToInt32(aCol[11]);
                  st.updatetime = Utils.getDatatimeString();
                  dbDapper.Update(st);
                }


              }
            }
          }
        }

      }
      catch (Exception ex)
      {
        logger.Error(ex);
        System.Threading.Thread.Sleep(10000);
        return false;
      }

      return true;
    }

    public bool GetStockThreeTradeOtc(DateTime GetDatetime)
    {
      try
      {
        string sDate = Utils.getDataString(GetDatetime, M10Const.DataStringType.ChineseT2);
        string sUrl = string.Format(M10Const.StockThreeTradeOtc, sDate);
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(string.Format(sUrl, sDate));
        req.Proxy = null;
        //改為寫入資料庫格式
        sDate = Utils.getDataString(GetDatetime, M10Const.DataStringType.ADT1);



        string sTemp = "";
        using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
        {
          //判斷http回應狀態(HttpStatusCode.OK=200)
          if (resp.StatusCode != HttpStatusCode.OK)
          {
            System.Threading.Thread.Sleep(3000);
            return false;
          }

          using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.GetEncoding(950)))
          {
            sTemp = SR.ReadToEnd();
          }
        }


        using (Stream s = GenerateStreamFromString(sTemp))
        {
          using (StreamReader sr = new StreamReader(s))
          {
            string Line;
            while ((Line = sr.ReadLine()) != null)
            {
              Line = Line.Replace(" ", "");
              Line = Line.Replace("\",\"", "|");
              Line = Line.Replace("\"", "");
              Line = Line.Replace(",", "");
              Line = Line.Replace("=", "");

              string[] aCol = Line.Split('|');

              if (aCol.Length == 16)
              {
                //檢核資料
                int iCheck = -1;

                if (int.TryParse(aCol[15], out iCheck) == false)
                {
                  continue;
                }

                ssql = " select * from Stockthreetrade where date = '{0}' and stockcode = '{1}' ";
                Stockthreetrade st = dbDapper.QuerySingleOrDefault<Stockthreetrade>(string.Format(ssql, sDate, aCol[0]));

                if (st == null)
                {
                  st = new Stockthreetrade();
                  st.stockcode = aCol[0];
                  st.date = sDate;
                  st.type = M10Const.StockType.otc;
                  st.foreigninv = Convert.ToInt32(aCol[4]);
                  st.trustinv = Convert.ToInt32(aCol[7]);
                  st.selfempinv = Convert.ToInt32(aCol[14]);
                  st.threeinv = Convert.ToInt32(aCol[15]);
                  st.updatetime = Utils.getDatatimeString();
                  dbDapper.Insert(st);
                }
                else
                {
                  st.foreigninv = Convert.ToInt32(aCol[4]);
                  st.trustinv = Convert.ToInt32(aCol[7]);
                  st.selfempinv = Convert.ToInt32(aCol[14]);
                  st.threeinv = Convert.ToInt32(aCol[15]);
                  st.updatetime = Utils.getDatatimeString();
                  dbDapper.Update(st);
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        logger.Error(ex);
        System.Threading.Thread.Sleep(10000);
        return false;
      }

      return true;
    }

    public static Stream GenerateStreamFromString(string s)
    {
      MemoryStream stream = new MemoryStream();
      StreamWriter writer = new StreamWriter(stream);
      writer.Write(s);
      writer.Flush();
      stream.Position = 0;
      return stream;
    }

    public void testlog()
    {


      logger.Info("baseclass test log......");


    }







  }
}
