using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using M10.lib.model;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace M10.lib
{
    public class StockHelper : M10BaseClass
    {


        public static WebClient getNewWebClient()
        {
            var wc = new WebClient();
            //wc.Headers.Add("User-Agent", HttpHelper.GetRandomAgent());
            wc.Encoding = Encoding.UTF8;
            wc.Proxy = null;

            //預防基礎連接已關閉: 傳送時發生未預期的錯誤。
            //TLS 1.0 已被視為不安全，近期應會被各大網站陸續停用
            ServicePointManager.SecurityProtocol =
                        SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                        SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            return wc;
        }



        public bool GetStockAfterTse(DateTime GetDatetime)
        {
            //bool bResult = false;
            string sLineTrans = "";
            try
            {
                string sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1);
                string sUrl = string.Format(M10Const.StockAfterTseUrl, sDate);


                using (WebClient wc = getNewWebClient())
                {
                    wc.Encoding = Encoding.GetEncoding(950);
                    string text = wc.DownloadString(sUrl);


                    List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

                    foreach (string LoopItem in StringList)
                    {
                        string Line = LoopItem;
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

                            if (aCol[9] == "") aCol[9] = "X";

                            //計算昨收
                            Decimal dPriceYesterday = CalcPriceYesterday(aCol[8], aCol[9], aCol[10]);

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
                                sa.priceyesterday = dPriceYesterday;
                                sa.updatetime = Utils.getDatatimeString();

                                dbDapper.Insert(sa);
                            }
                            else
                            {
                                sa.stocktype = M10Const.StockType.tse;
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
                                sa.priceyesterday = dPriceYesterday;
                                dbDapper.Update(sa);
                            }
                        }

                    }
                }

                //刪除記錄檔
                ssql = @"
                    delete  stocklog where  logstatus = 'success' and logtype = '{0}' and logdate = '{1}'
                ";
                ssql = string.Format(ssql
                    , M10Const.StockLogType.StockAfterTse.ToString(), Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1));
                dbDapper.Execute(ssql);

                //新增記錄檔
                StockLog sl = new StockLog();
                sl.logdate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1);
                sl.logdatetime = Utils.getDatatimeString();
                sl.logstatus = M10Const.StockLogStatus.s200;
                sl.memo = "";
                sl.logtype = M10Const.StockLogType.StockAfterTse;
                dbDapper.Insert(sl);
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
                string sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ChineseT2);
                string sUrl = string.Format(M10Const.StockAfterOtcUrl, sDate);
                sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1);


                using (WebClient wc = getNewWebClient())
                {
                    wc.Encoding = Encoding.GetEncoding(950);
                    string text = wc.DownloadString(sUrl);


                    List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

                    foreach (string LoopItem in StringList)
                    {
                        string Line = LoopItem;
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

                            //計算昨收
                            Decimal dPriceYesterday = CalcPriceYesterday(aCol[2], sUpdown, sPricediff);


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
                                sa.priceyesterday = dPriceYesterday;
                                dbDapper.Insert(sa);
                            }
                            else
                            {
                                sa.stocktype = M10Const.StockType.otc;
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
                                sa.priceyesterday = dPriceYesterday;
                                dbDapper.Update(sa);
                            }


                        }


                    }

                }
                //刪除記錄檔
                ssql = @"
                    delete  stocklog where  logstatus = 'success' and logtype = '{0}' and logdate = '{1}'
                ";
                ssql = string.Format(ssql
                    , M10Const.StockLogType.StockAfterOtc.ToString(), Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1));
                dbDapper.Execute(ssql);

                //新增記錄檔
                StockLog sl = new StockLog();
                sl.logdate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1);
                sl.logdatetime = Utils.getDatatimeString();
                sl.logstatus = M10Const.StockLogStatus.s200;
                sl.memo = "";
                sl.logtype = M10Const.StockLogType.StockAfterOtc;
                dbDapper.Insert(sl);



            }
            catch (Exception ex)
            {
                logger.Error(ex);
                System.Threading.Thread.Sleep(10000);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 三大法人個股買賣盤後資料(TSE)
        /// </summary>
        /// <param name="GetDatetime"></param>
        /// <returns></returns>
        public bool GetStockThreeTradeTse(DateTime GetDatetime)
        {
            try
            {
                string sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1);
                string sUrl = string.Format(M10Const.StockThreeTradeTse, sDate);

                string text = string.Empty;
                using (WebClient wc = getNewWebClient())
                {
                    wc.Encoding = Encoding.GetEncoding(950);
                    text = wc.DownloadString(sUrl);
                }

                List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

                foreach (string LoopItem in StringList)
                {
                    string Line = LoopItem;
                    Line = Line.Replace(" ", "");
                    Line = Line.Replace("\",\"", "|");
                    Line = Line.Replace("\"", "");
                    Line = Line.Replace(",", "");
                    Line = Line.Replace("=", "");

                    string[] aCol = Line.Split('|');


                    //第三版格式
                    if (aCol.Length == 19)
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
                            st.type = M10Const.StockType.tse;
                            st.foreigninv = Convert.ToInt32(aCol[4]);
                            st.trustinv = Convert.ToInt32(aCol[7]);
                            st.selfempinv = Convert.ToInt32(aCol[14]);
                            st.threeinv = Convert.ToInt32(aCol[18]);
                            st.updatetime = Utils.getDatatimeString();
                            dbDapper.Insert(st);
                        }
                        else
                        {
                            st.stockcode = aCol[0];
                            st.date = sDate;
                            st.type = M10Const.StockType.tse;
                            st.foreigninv = Convert.ToInt32(aCol[4]);
                            st.trustinv = Convert.ToInt32(aCol[7]);
                            st.selfempinv = Convert.ToInt32(aCol[14]);
                            st.threeinv = Convert.ToInt32(aCol[18]);
                            st.updatetime = Utils.getDatatimeString();
                            dbDapper.Update(st);
                        }
                    }

                    //第二版格式
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
                            st.type = M10Const.StockType.tse;
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
                            st.type = M10Const.StockType.tse;
                            st.foreigninv = Convert.ToInt32(aCol[4]);
                            st.trustinv = Convert.ToInt32(aCol[7]);
                            st.selfempinv = Convert.ToInt32(aCol[14]);
                            st.threeinv = Convert.ToInt32(aCol[15]);
                            st.updatetime = Utils.getDatatimeString();
                            dbDapper.Update(st);
                        }
                    }

                    //第一版格式
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
                            st.type = M10Const.StockType.tse;
                            st.foreigninv = Convert.ToInt32(aCol[4]);
                            st.trustinv = Convert.ToInt32(aCol[7]);
                            st.selfempinv = Convert.ToInt32(aCol[10]);
                            st.threeinv = Convert.ToInt32(aCol[11]);
                            st.updatetime = Utils.getDatatimeString();
                            dbDapper.Insert(st);
                        }
                        else
                        {
                            st.type = M10Const.StockType.tse;
                            st.foreigninv = Convert.ToInt32(aCol[4]);
                            st.trustinv = Convert.ToInt32(aCol[7]);
                            st.selfempinv = Convert.ToInt32(aCol[10]);
                            st.threeinv = Convert.ToInt32(aCol[11]);
                            st.updatetime = Utils.getDatatimeString();
                            dbDapper.Update(st);
                        }


                    }
                }


                StockLog sl = new StockLog();
                sl.logdate = Utils.getDateString(DateTime.Now, M10Const.DateStringType.ADT1);
                sl.logdatetime = Utils.getDatatimeString();
                sl.logstatus = M10Const.StockLogStatus.s200;
                sl.memo = "";
                sl.logtype = M10Const.StockLogType.StockThreeTse;
                dbDapper.Insert(sl);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                System.Threading.Thread.Sleep(10000);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 三大法人個股買賣盤後資料(OTC)
        /// </summary>
        /// <param name="GetDatetime"></param>
        /// <returns></returns>
        public bool GetStockThreeTradeOtc(DateTime GetDatetime)
        {
            try
            {
                string sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ChineseT2);
                string sUrl = string.Format(M10Const.StockThreeTradeOtc, sDate);
                //改為寫入資料庫格式
                sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1);

                string text = "";
                using (WebClient wc = getNewWebClient())
                {
                    wc.Encoding = Encoding.GetEncoding(950);
                    text = wc.DownloadString(sUrl);
                }

                List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

                foreach (string LoopItem in StringList)
                {
                    string Line = LoopItem;
                    Line = Line.Replace(" ", "");
                    Line = Line.Replace("\",\"", "|");
                    Line = Line.Replace("\"", "");
                    Line = Line.Replace(",", "");
                    Line = Line.Replace("=", "");

                    string[] aCol = Line.Split('|');

                    //新版版格式
                    if (aCol.Length == 24)
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
                            st.trustinv = Convert.ToInt32(aCol[13]);
                            st.selfempinv = Convert.ToInt32(aCol[22]);
                            st.threeinv = Convert.ToInt32(aCol[23]);
                            st.updatetime = Utils.getDatatimeString();
                            dbDapper.Insert(st);
                        }
                        else
                        {
                            st.type = M10Const.StockType.otc;
                            st.foreigninv = Convert.ToInt32(aCol[4]);
                            st.trustinv = Convert.ToInt32(aCol[13]);
                            st.selfempinv = Convert.ToInt32(aCol[22]);
                            st.threeinv = Convert.ToInt32(aCol[23]);
                            st.updatetime = Utils.getDatatimeString();
                            dbDapper.Update(st);
                        }
                    }

                    //舊版格式
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
                            st.type = M10Const.StockType.otc;
                            st.foreigninv = Convert.ToInt32(aCol[4]);
                            st.trustinv = Convert.ToInt32(aCol[7]);
                            st.selfempinv = Convert.ToInt32(aCol[14]);
                            st.threeinv = Convert.ToInt32(aCol[15]);
                            st.updatetime = Utils.getDatatimeString();
                            dbDapper.Update(st);
                        }
                    }
                }

                StockLog sl = new StockLog();
                sl.logdate = Utils.getDateString(DateTime.Now, M10Const.DateStringType.ADT1);
                sl.logdatetime = Utils.getDatatimeString();
                sl.logstatus = M10Const.StockLogStatus.s200;
                sl.memo = "";
                sl.logtype = M10Const.StockLogType.StockThreeOtc;
                dbDapper.Insert(sl);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                System.Threading.Thread.Sleep(10000);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 盤後當沖資料(TSE)
        /// </summary>
        /// <param name="GetDatetime"></param>
        /// <returns></returns>
        public bool GetStockAfterRushTse(DateTime GetDatetime)
        {
            //bool bResult = false;
            string sLineTrans = "";
            try
            {
                string sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1);
                string sUrl = string.Format(M10Const.StockAfterRushTse, sDate);


                using (WebClient wc = getNewWebClient())
                {
                    wc.Encoding = Encoding.GetEncoding(950);
                    string text = wc.DownloadString(sUrl);


                    List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

                    foreach (string LoopItem in StringList)
                    {
                        string Line = LoopItem;
                        Line = Line.Replace(" ", "");
                        Line = Line.Replace("\",\"", "|");
                        Line = Line.Replace("\"", "");
                        Line = Line.Replace(",", "");
                        Line = Line.Replace("=", "");
                        sLineTrans = Line;
                        string[] aCol = Line.Split('|');

                        if (aCol.Length == 6)
                        {
                            //檢核資料
                            Decimal iCheck = -1;

                            if (Decimal.TryParse(aCol[1], out iCheck) == true)
                            {
                                continue;
                            }

                            if (Decimal.TryParse(aCol[3], out iCheck) == false)
                            {
                                continue;
                            }

                            string StockCode = aCol[0];
                            string StopRush = aCol[2];

                            ssql = " select * from stockafterrush  where stockdate = @stockdate and stockcode = @stockcode ";
                            var p = dbDapper.GetNewDynamicParameters();
                            p.Add("@stockdate", sDate);
                            p.Add("@stockcode", StockCode);
                            StockAfterRush sa = dbDapper.QuerySingleOrDefault<StockAfterRush>(ssql, p);

                            decimal dRushDealNum = 0;
                            decimal.TryParse(aCol[3], out dRushDealNum);

                            decimal dRushMoneyBuy = 0;
                            decimal.TryParse(aCol[4], out dRushMoneyBuy);

                            decimal dRushMoneySell = 0;
                            decimal.TryParse(aCol[5], out dRushMoneySell);

                            if (sa == null)
                            {
                                sa = new StockAfterRush();
                                sa.stockdate = sDate;
                                sa.type = M10Const.StockType.tse;
                                sa.stockcode = StockCode;
                                sa.stoprush = StopRush;
                                sa.rushdealnum = Convert.ToInt64(dRushDealNum);
                                sa.rushmoneybuy = Convert.ToInt64(dRushMoneyBuy);
                                sa.rushmoneysell = Convert.ToInt64(dRushMoneySell);
                                sa.createdatetime = Utils.getDatatimeString();
                                sa.updatetime = Utils.getDatatimeString();

                                dbDapper.Insert(sa);
                            }
                            else
                            {
                                sa.rushdealnum = Convert.ToInt64(dRushDealNum);
                                sa.rushmoneybuy = Convert.ToInt64(dRushMoneyBuy);
                                sa.rushmoneysell = Convert.ToInt64(dRushMoneySell);
                                sa.updatetime = Utils.getDatatimeString();

                                dbDapper.Update(sa);
                            }
                        }

                    }
                }

                StockLog sl = new StockLog();
                sl.logdate = Utils.getDateString(DateTime.Now, M10Const.DateStringType.ADT1);
                sl.logdatetime = Utils.getDatatimeString();
                sl.logstatus = M10Const.StockLogStatus.s200;
                sl.memo = "";
                sl.logtype = M10Const.StockLogType.StockAfterRushTseLog;
                dbDapper.Insert(sl);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "StockAfterRushTse:" + sLineTrans);
                //System.Threading.Thread.Sleep(10000);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 盤後當沖資料(OTC)
        /// </summary>
        /// <param name="GetDatetime"></param>
        /// <returns></returns>
        public bool GetStockAfterRushOtc(DateTime GetDatetime)
        {
            try
            {
                string sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ChineseT2);
                string sUrl = string.Format(M10Const.StockAfterRushOtc, sDate);
                sDate = Utils.getDateString(GetDatetime, M10Const.DateStringType.ADT1);


                using (WebClient wc = getNewWebClient())
                {
                    wc.Encoding = Encoding.GetEncoding(950);
                    string text = wc.DownloadString(sUrl);


                    List<string> StringList = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();

                    foreach (string LoopItem in StringList)
                    {
                        string Line = LoopItem;
                        Line = Line.Replace(" ", "");
                        Line = Line.Replace("\",\"", "|");
                        Line = Line.Replace("\"", "");
                        Line = Line.Replace(",", "");
                        Line = Line.Replace("=", "");

                        string[] aCol = Line.Split('|');



                        if (aCol.Length == 6)
                        {
                            //檢核資料
                            Decimal iCheck = -1;

                            if (Decimal.TryParse(aCol[1], out iCheck) == true)
                            {
                                continue;
                            }

                            if (Decimal.TryParse(aCol[3], out iCheck) == false)
                            {
                                continue;
                            }

                            string StockCode = aCol[0];
                            string StopRush = aCol[2] == "＊" ? "Y" : "";

                            ssql = " select * from stockafterrush  where stockdate = @stockdate and stockcode = @stockcode ";
                            var p = dbDapper.GetNewDynamicParameters();
                            p.Add("@stockdate", sDate);
                            p.Add("@stockcode", StockCode);
                            StockAfterRush sa = dbDapper.QuerySingleOrDefault<StockAfterRush>(ssql, p);

                            decimal dRushDealNum = 0;
                            decimal.TryParse(aCol[3], out dRushDealNum);

                            decimal dRushMoneyBuy = 0;
                            decimal.TryParse(aCol[4], out dRushMoneyBuy);

                            decimal dRushMoneySell = 0;
                            decimal.TryParse(aCol[5], out dRushMoneySell);

                            if (sa == null)
                            {
                                sa = new StockAfterRush();
                                sa.stockdate = sDate;
                                sa.type = M10Const.StockType.otc;
                                sa.stockcode = StockCode;
                                sa.stoprush = StopRush;
                                sa.rushdealnum = Convert.ToInt64(dRushDealNum);
                                sa.rushmoneybuy = Convert.ToInt64(dRushMoneyBuy);
                                sa.rushmoneysell = Convert.ToInt64(dRushMoneySell);
                                sa.createdatetime = Utils.getDatatimeString();
                                sa.updatetime = Utils.getDatatimeString();

                                dbDapper.Insert(sa);
                            }
                            else
                            {
                                sa.rushdealnum = Convert.ToInt64(dRushDealNum);
                                sa.rushmoneybuy = Convert.ToInt64(dRushMoneyBuy);
                                sa.rushmoneysell = Convert.ToInt64(dRushMoneySell);
                                sa.updatetime = Utils.getDatatimeString();

                                dbDapper.Update(sa);
                            }
                        }


                    }

                }

                StockLog sl = new StockLog();
                sl.logdate = Utils.getDateString(DateTime.Now, M10Const.DateStringType.ADT1);
                sl.logdatetime = Utils.getDatatimeString();
                sl.logstatus = M10Const.StockLogStatus.s200;
                sl.memo = "";
                sl.logtype = M10Const.StockLogType.StockAfterRushOtcLog;
                dbDapper.Insert(sl);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                System.Threading.Thread.Sleep(10000);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 更新個股基本資料
        /// </summary>
        /// <returns></returns>
        public bool GetStockInfo()
        {
            try
            {
                List<string> TypeList = new List<string>();
                TypeList.Add(M10Const.StockType.tse);
                TypeList.Add(M10Const.StockType.otc);
                TypeList.Add(M10Const.StockType.otc1);

                foreach (string sType in TypeList)
                {
                    GetStockInfoSub(sType);

                    //停止15秒再進行下一個更新
                    System.Threading.Thread.Sleep(30 * 1000);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }

            return true;
        }

        public void GetStockInfoSub(string sType)
        {
            string surl = "";

            if (sType == M10Const.StockType.tse) surl = M10Const.StockInfoTse;
            if (sType == M10Const.StockType.otc) surl = M10Const.StockInfoOtc;
            if (sType == M10Const.StockType.otc1) surl = M10Const.StockInfoOtc1;



            HtmlWeb webClient = new HtmlWeb();

            //預防基礎連接已關閉: 傳送時發生未預期的錯誤。
            //TLS 1.0 已被視為不安全，近期應會被各大網站陸續停用
            ServicePointManager.SecurityProtocol =
                        SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                        SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            //網頁特殊編碼
            webClient.OverrideEncoding = Encoding.GetEncoding(950);

            // 載入網頁資料 
            HtmlDocument doc = webClient.Load(surl);

            // 裝載查詢結果 
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table[2]/tr");

            if (nodes.Count > 0)
            {
                ssql = " update stockinfo set updstatus = 'N' where type = '{0}'  ";
                dbDapper.Execute(string.Format(ssql, sType));
            }

            foreach (HtmlNode node in nodes)
            {
                string sCode = "";
                string sName = "";

                HtmlNodeCollection tdnodes = node.SelectNodes("td");

                if (tdnodes.Count > 0)
                {
                    HtmlNode tdnode = tdnodes[0];
                    string[] StockInfoSplit = tdnode.InnerText.Split('　');

                    if (StockInfoSplit.Length != 2) continue;

                    sCode = StockInfoSplit[0];
                    sName = StockInfoSplit[1];

                    //判斷代碼存在則更新，不存在新增
                    ssql = " select * from stockinfo where stockcode = '{0}' ";
                    StockInfo StockInfoItem = dbDapper.QuerySingleOrDefault<StockInfo>(string.Format(ssql, sCode));

                    if (StockInfoItem == null) //不存在新增
                    {
                        StockInfoItem = new StockInfo();
                        StockInfoItem.stockcode = sCode;
                        StockInfoItem.stockname = sName;
                        StockInfoItem.type = sType;
                        StockInfoItem.updatetime = Utils.getDatatimeString();
                        StockInfoItem.updstatus = "Y";
                        StockInfoItem.status = "Y";

                        dbDapper.Insert(StockInfoItem);
                    }
                    else
                    {
                        StockInfoItem.type = sType;
                        StockInfoItem.updatetime = Utils.getDatatimeString();
                        StockInfoItem.updstatus = "Y";
                        StockInfoItem.status = "Y";

                        dbDapper.Update(StockInfoItem);
                    }
                }

            }

            ssql = "update stockinfo set status = 'N' where updstatus = 'N' ";
            dbDapper.Execute(ssql);

            //興櫃轉上櫃
            if (sType == M10Const.StockType.otc1)
            {
                ssql = "update stockinfo set type = 'otc' where type = 'otc1' ";
                dbDapper.Execute(ssql);
            }

            StockLog sl = new StockLog();
            sl.logdate = Utils.getDateString(DateTime.Now, M10Const.DateStringType.ADT1);
            sl.logdatetime = Utils.getDatatimeString();
            sl.logstatus = M10Const.StockLogStatus.s200;
            sl.memo = "";
            if (sType == M10Const.StockType.tse)
            {
                sl.logtype = M10Const.StockLogType.StockInfoTse;
            }
            if (sType == M10Const.StockType.otc)
            {
                sl.logtype = M10Const.StockLogType.StockInfoOtc;
            }
            if (sType == M10Const.StockType.otc1)
            {
                sl.logtype = M10Const.StockLogType.StockInfoOtc1;
            }
            dbDapper.Insert(sl);


        }

        public StockRuntime getStockRealtimeYahooWeb(string stockcode)
        {
            if (stockcode == "t00") stockcode = "0000";
            //if (stockcode == "t00") stockcode = "0000";

            StockRuntime sr = new StockRuntime();
            sr.z = "";
            sr.y = "";
            sr.u = "";
            sr.w = "";
            sr.n = "";
            sr.c = "";
            sr.xx = "";


            string surl = "https://tw.stock.yahoo.com/q/q?s=" + stockcode;
            //surl = "https://tw.stock.yahoo.com/s/tse.php";
            HtmlWeb webClient = new HtmlWeb();

            //預防基礎連接已關閉: 傳送時發生未預期的錯誤。
            //TLS 1.0 已被視為不安全，近期應會被各大網站陸續停用
            ServicePointManager.SecurityProtocol =
                        SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                        SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            //網頁特殊編碼
            webClient.OverrideEncoding = System.Text.Encoding.GetEncoding(950);

            // 載入網頁資料 
            HtmlAgilityPack.HtmlDocument doc = webClient.Load(surl);
            // 裝載查詢結果 
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table[2]/tr/td/table/tr[2]/td");

            int idx = 0;
            foreach (HtmlNode node in nodes)
            {
                //目前成交價
                if (idx == 2)
                {
                    sr.z = node.InnerText;
                }

                //yahoo漲跌
                if (idx == 5)
                {
                    if (node.InnerText.Length > 0)
                    {
                        sr.xx = node.InnerText.Substring(0, 1);
                    }

                }

                //昨收
                if (idx == 7)
                {
                    sr.y = node.InnerText;
                }

                idx++;
            }


            //使用api呼叫取得
            if (stockcode == "0000" || stockcode == "9999")
            {
                StockRuntime sr_0000 = getStockRealtimeYahooApi(stockcode);

                sr = sr_0000;
            }

            //取得資訊
            ssql = " select * from StockInfo where stockcode = '{0}' ";
            StockInfo si = dbDapper.QuerySingleOrDefault<StockInfo>(string.Format(ssql, stockcode));
            if (si != null)
            {
                //名稱
                sr.n = si.stockname;
                //代碼
                sr.c = si.stockcode;
            }

            return sr;
        }

        public StockRuntime getStockRealtimeYahooApi(string stockcode)
        {
            StockRuntime sr = new StockRuntime();
            sr.z = "";
            sr.y = "";
            sr.u = "";
            sr.w = "";
            sr.n = "";
            sr.c = "";
            sr.xx = "";
            sr.a = "";
            sr.TradeDay = "";
            sr.open = "";
            sr.YTop = "";
            sr.YLow = "";

            string sJson = "";
            try
            {
                string sUrl = "https://tw.quote.finance.yahoo.net/quote/q?type=tick&sym={0}";
                string sStockcodeUrl = "";
                sStockcodeUrl = stockcode;
                if (stockcode == "0000") sStockcodeUrl = "%23001";
                if (stockcode == "9999")
                {
                    sStockcodeUrl = "WTX%26";
                    sUrl = "https://tw.screener.finance.yahoo.net/future/q?type=tick&mkt=01&sym={0}";
                }

                using (WebClient wc = getNewWebClient())
                {
                    sUrl = string.Format(sUrl, sStockcodeUrl);
                    string text = wc.DownloadString(sUrl);



                    text = text.Replace("null(", "");
                    text = text.Replace(");", "");
                    text = text.Replace(",\"tick\":[]}", "");
                    text = text.Replace(text.Substring(0, text.IndexOf(",\"mem\":") + 7), "");
                    if (stockcode != "9999")
                    {
                        text = text.Replace(text.Substring(text.IndexOf(",\"tick\":")), "");
                    }

                    // 1090320 配合JSON格式異動，調整格式化
                    if (stockcode == "9999")
                    {
                        text = text.Replace(text.Substring(text.IndexOf(",\"tick\":")), "");
                        text = text.Replace("sections", @"""sections""");
                    }


                    if (text.Contains(",\"143\":"))
                    {
                        //1061219 修改為取代
                        //text = text.Insert(text.IndexOf(",\"143\":") + 7, "\"").Insert(text.IndexOf(",\"143\":") + 14, "\"");

                        int iStart = text.IndexOf(",\"143\":");
                        int iEnd = text.IndexOf(",", iStart + 1);
                        string sRep = text.Substring(iStart, iEnd - iStart);

                        text = text.Replace(sRep, "");
                    }


                    sJson = text;
                    JObject jobj = JObject.Parse(text);

                    //成交價(125)
                    if (jobj["125"] != null)
                        sr.z = jobj["125"].ToString();
                    //判斷是否早盤試搓期間(0830-0900)，則使用昨收(129)回報
                    TimeSpan tsS = new TimeSpan(8, 30, 0);
                    TimeSpan tsE = new TimeSpan(9, 0, 0);
                    TimeSpan tsNow = DateTime.Now.TimeOfDay;
                    if (tsNow >= tsS && tsNow <= tsE)
                    {
                        if (jobj["129"] != null)
                            sr.z = jobj["129"].ToString();
                    }

                    //昨收(129)
                    if (jobj["129"] != null)
                        sr.y = jobj["129"].ToString();
                    //成交量(404)
                    if (jobj["404"] != null)
                        sr.a = jobj["404"].ToString();

                    //日期
                    if (jobj["TradeDay"] != null)
                        sr.TradeDay = jobj["TradeDay"].ToString();

                    //開盤價(126)
                    if (jobj["126"] != null)
                        sr.open = jobj["126"].ToString();

                    //最高(130)
                    //if (jobj["130"] != null)
                    //  sr.u = jobj["130"].ToString();
                    //最低(131)
                    //if (jobj["131"] != null)
                    //  sr.w = jobj["131"].ToString();


                    ////昨高(Yahoo沒提供)
                    //if (jobj["106"] != null)
                    //    sr.YTop = jobj["106"].ToString();
                    ////昨低(Yahoo沒提供)
                    //if (jobj["107"] != null)
                    //    sr.YLow = jobj["107"].ToString();
                    //均價(471)



                    if (stockcode != "0000" && stockcode != "9999")
                    {
                        //LimitUp
                        if (jobj["132"] != null)
                            sr.u = jobj["132"].ToString();
                        //LimitDw
                        if (jobj["133"] != null)
                            sr.w = jobj["133"].ToString();
                    }
                }

                //大盤不顯示量
                if (stockcode == "0000") sr.a = "";

                //取得個股資訊
                ssql = " select * from StockInfo where stockcode = '{0}' ";
                StockInfo si = dbDapper.QuerySingleOrDefault<StockInfo>(string.Format(ssql, stockcode));
                if (si != null)
                {
                    //個股名稱
                    sr.n = si.stockname;
                    //個股代碼
                    sr.c = si.stockcode;
                }

                sr.status = M10Const.StockRuntimeStatus.YahooApi;

            }
            catch (Exception ex)
            {
                logger.Error(ex, sJson);
            }

            return sr;
        }

        public List<string> getStockTUMD(string sComm)
        {
            string sStockCode = sComm.Replace("Q", "");
            List<string> ResultList = new List<string>();
            string sTU = "";
            string sTM = "";
            string sTD = "";
            string sStockName = "";
            string sStockValue = "";

            string sJson = "";
            try
            {
                string sUrl = "https://cronjob.uanalyze.com.tw/fetch/IndividualStock_TUMD_Free/{0}";
                using (WebClient wc = getNewWebClient())
                {
                    sUrl = string.Format(sUrl, sStockCode);
                    string text = wc.DownloadString(sUrl);

                    sJson = text;

                    dynamic dd = JObject.Parse(text);

                    sTU = dd.data.display.ua80250_cp.Data;
                    sTM = dd.data.display.ua80251_cp.Data;
                    sTD = dd.data.display.ua80252_cp.Data;                    
                }

                //20211218 個股名稱改由呼叫即時股價時取得
                //ssql = " select * from StockInfo where stockcode = '{0}' ";
                //StockInfo si = dbDapper.QuerySingleOrDefault<StockInfo>(string.Format(ssql, sStockCode));
                ////個股名稱
                //if (si != null) sStockName = si.stockname;

                //20211218 毛哥交代取得現在的價格
                StockRuntime sr = getStockRealtimeYahooApi(sStockCode);
                sStockName = sr.n;
                sStockValue = sr.z;


                List<string> sValueList = new List<string>();
                sValueList.Add(sTU);
                sValueList.Add(sTM);
                sValueList.Add(sTD);
                sValueList.Add(sStockValue);

                //進階作法
                //直接排序現價
                List<double> ValueList = new List<double>();
                //ValueList.ConvertAll(s => Convert.ToDouble(s));

                foreach (string sValue in sValueList)
                {
                    double dTmp = 0;
                    if (double.TryParse(sValue, out dTmp) == true) ValueList.Add(dTmp);
                }

                //數列進行反向排列(價格高在上面) 
                // List<double> 直接使用Reverse沒有效果，先用Sort後，在執行Reverse才會正常
                ValueList.Sort();
                ValueList.Reverse();

                //ResultList.Add(string.Format("TUMD 日期：{0}", sStockDate));
                ResultList.Add("股泰免費版TUMD");
                ResultList.Add(string.Format("{0}({1})", sStockName, sStockCode));
                foreach (double value in ValueList)
                {
                    string sType = "";
                    if (sTU == value.ToString()) sType = "TU";
                    if (sTM == value.ToString()) sType = "TM";
                    if (sTD == value.ToString()) sType = "TD";
                    if (sStockValue == value.ToString()) sType = "現價";

                    ResultList.Add(string.Format("[{0}]：{1}", sType, value.ToString()));
                }
                //ResultList.Add(string.Format("[TU]：{0}", sTU));
                //ResultList.Add(string.Format("[TM]：{0}", sTM));
                //ResultList.Add(string.Format("[TD]：{0}", sTD));

            }
            catch (Exception ex)
            {
                logger.Error(ex, sJson);
            }

            return ResultList;
        }

        


        private string tempCheckObjectNull(Object o) 
        {
            string sResult = "";

            try
            {
                if (o != null) sResult = o.ToString();
            }
            catch (Exception ex)
            {
                //遇到錯誤，直接回傳空字串
                return sResult;
            }

            return sResult;
        }

        /// <summary>
        /// 取得資料後，完整格式化API資料
        /// </summary>
        /// <param name="stockcode"></param>
        /// <returns></returns>
        public StockYahooAPI getStockRealtimeYahooApi2(string stockcode)
        {
            StockYahooAPI sr = new StockYahooAPI();

            //sr.z = "";
            //sr.y = "";
            //sr.u = "";
            //sr.w = "";
            //sr.n = "";
            //sr.c = "";
            //sr.xx = "";
            //sr.a = "";
            //sr.TradeDay = "";
            //sr.open = "";
            //sr.YTop = "";
            //sr.YLow = "";

            //櫃檯指數
            //https://tw.quote.finance.yahoo.net/quote/q?type=tick&sym=%23026


            string sJson = "";
            try
            {
                string sUrl = "https://tw.quote.finance.yahoo.net/quote/q?type=tick&sym={0}";
                string sStockcodeUrl = "";
                sStockcodeUrl = stockcode;
                if (stockcode == "0000") sStockcodeUrl = "%23001";
                if (stockcode == "9999")
                {
                    sStockcodeUrl = "WTX%26";
                    sUrl = "https://tw.screener.finance.yahoo.net/future/q?type=tick&mkt=01&sym={0}";
                }

                using (WebClient wc = getNewWebClient())
                {
                    sUrl = string.Format(sUrl, sStockcodeUrl);
                    string sApiText = wc.DownloadString(sUrl);


                    //格式化
                    sApiText = sApiText.Replace("null(", "");
                    sApiText = sApiText.Replace(");", "");
                    //text = text.Replace(",\"tick\":[]}", "");
                    //text = text.Replace(text.Substring(0, text.IndexOf(",\"mem\":") + 7), "");
                    //if (stockcode != "9999")
                    //{
                    //    text = text.Replace(text.Substring(text.IndexOf(",\"tick\":")), "");
                    //}

                    //// 1090320 配合JSON格式異動，調整格式化
                    //if (stockcode == "9999")
                    //{
                    //    text = text.Replace(text.Substring(text.IndexOf(",\"tick\":")), "");
                    //    text = text.Replace("sections", @"""sections""");
                    //}


                    //if (text.Contains(",\"143\":"))
                    //{
                    //    //1061219 修改為取代
                    //    //text = text.Insert(text.IndexOf(",\"143\":") + 7, "\"").Insert(text.IndexOf(",\"143\":") + 14, "\"");

                    //    int iStart = text.IndexOf(",\"143\":");
                    //    int iEnd = text.IndexOf(",", iStart + 1);
                    //    string sRep = text.Substring(iStart, iEnd - iStart);

                    //    text = text.Replace(sRep, "");
                    //}


                    //sJson = sApiText;
                    JObject jobj = JObject.Parse(sApiText);

                    
                    if (jobj["mem"] != null) 
                    {
                        JObject jobMem = (JObject)jobj["mem"];

                        sr.id   = tempCheckObjectNull(jobMem["id"]);
                        sr.name = tempCheckObjectNull(jobMem["name"]);
                        sr.TradeDay = tempCheckObjectNull(jobMem["TradeDay"]);
                        sr.vol = tempCheckObjectNull(jobMem["404"]);
                        sr.open = tempCheckObjectNull(jobMem["126"]);
                        sr.close = tempCheckObjectNull(jobMem["125"]);
                        sr.high = tempCheckObjectNull(jobMem["130"]);
                        sr.low = tempCheckObjectNull(jobMem["131"]);
                    }

                    if (jobj["tick"] != null)
                    {
                        JArray jobTick = (JArray)jobj["tick"];

                        foreach (JToken item in jobTick)
                        {
                            StockYahooAPITick syat = new StockYahooAPITick();
                            syat.tick = tempCheckObjectNull(item["t"]);
                            syat.price = tempCheckObjectNull(item["p"]);
                            syat.vol = tempCheckObjectNull(item["v"]);

                            sr.Tick.Add(syat);
                        }


                    }




                    //if (jobj["id"] != null)
                    //    sr.id = jobj["id"].ToString();

                    //if (jobj["id"] != null)
                    //    sr.id = jobj["id"].ToString();



                    ////成交價(125)
                    //if (jobj["125"] != null)
                    //    sr.z = jobj["125"].ToString();
                    ////昨收(129)
                    //if (jobj["129"] != null)
                    //    sr.y = jobj["129"].ToString();
                    ////成交量(404)
                    //if (jobj["404"] != null)
                    //    sr.a = jobj["404"].ToString();

                    ////日期
                    //if (jobj["TradeDay"] != null)
                    //    sr.TradeDay = jobj["TradeDay"].ToString();

                    ////開盤價(126)
                    //if (jobj["126"] != null)
                    //    sr.open = jobj["126"].ToString();

                    //最高(130)
                    //if (jobj["130"] != null)
                    //  sr.u = jobj["130"].ToString();
                    //最低(131)
                    //if (jobj["131"] != null)
                    //  sr.w = jobj["131"].ToString();


                    ////昨高(Yahoo沒提供)
                    //if (jobj["106"] != null)
                    //    sr.YTop = jobj["106"].ToString();
                    ////昨低(Yahoo沒提供)
                    //if (jobj["107"] != null)
                    //    sr.YLow = jobj["107"].ToString();
                    //均價(471)



                    //if (stockcode != "0000" && stockcode != "9999")
                    //{
                    //    ////LimitUp
                    //    //if (jobj["132"] != null)
                    //    //    sr.u = jobj["132"].ToString();
                    //    ////LimitDw
                    //    //if (jobj["133"] != null)
                    //    //    sr.w = jobj["133"].ToString();
                    //}
                }

                ////大盤不顯示量
                //if (stockcode == "0000") sr.a = "";

                ////取得個股資訊
                //ssql = " select * from StockInfo where stockcode = '{0}' ";
                //StockInfo si = dbDapper.QuerySingleOrDefault<StockInfo>(string.Format(ssql, stockcode));
                //if (si != null)
                //{
                //    //個股名稱
                //    sr.n = si.stockname;
                //    //個股代碼
                //    sr.c = si.stockcode;
                //}

                //sr.status = M10Const.StockRuntimeStatus.YahooApi;

            }
            catch (Exception ex)
            {
                logger.Error(ex, sJson);
            }

            return sr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitType">UP , DOWN</param>
        /// <param name="sPriceClose"></param>
        /// <returns></returns>
        public decimal getPriceLimitUpOrDown(string limitType, string sPriceClose)
        {
            // reference: http://stock7.0123456789.tw/


            double price = 0;
            double.TryParse(sPriceClose, out price);

            //double price = dClose;
            //double limitUp = price * (PriceToday.Date >= new DateTime(2015, 6, 1) ? 1.10 : 1.07);
            //double limitDown = price * (PriceToday.Date >= new DateTime(2015, 6, 1) ? 0.90 : 0.93);
            double limitUp = price * 1.10;
            double limitDown = price * 0.90;
            double STOCKUP = 0, STOCKDW = 0;
            if (limitUp < 10 && limitDown < 10)
            {
                STOCKUP = ((Math.Floor((Math.Floor(limitUp * 100) * 100))) / 100) / 100;
                STOCKDW = ((Math.Floor((Math.Ceiling(limitDown * 100) * 100))) / 100) / 100;
            }
            else if (limitUp > 10 && limitDown < 10)
            {
                STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.05) * 0.05) * 100) * 100)) / 100) / 100;
                STOCKDW = ((Math.Floor((Math.Ceiling(limitDown * 100) * 100))) / 100) / 100;
            }
            else if (limitUp >= 10 && limitDown >= 10 && limitUp <= 50 && limitDown < 50)
            {
                STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.05) * 0.05) * 100) * 100)) / 100) / 100;
                STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 0.05) * 0.05) * 100) * 100)) / 100) / 100;
            }
            else if (limitUp >= 50 && limitDown >= 50 && limitUp < 100 && limitDown < 100)
            {
                STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.1) * 0.1) * 100) * 100)) / 100) / 100;
                STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 0.1) * 0.1) * 100) * 100)) / 100) / 100;
            }
            else if (limitUp >= 50 && limitDown < 50)
            {
                STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.1) * 0.1) * 100) * 100)) / 100) / 100;
                STOCKDW = (Math.Floor((Math.Ceiling(limitDown / 0.05) * 0.05) * 100)) / 100;
            }
            else if (limitUp >= 100 && limitDown >= 100 && limitUp < 1000 && limitDown < 1000)
            {
                STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.5) * 0.5) * 100) * 100)) / 100) / 100;
                STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 0.5) * 0.5) * 100) * 100)) / 100) / 100;
            }
            else if (limitUp >= 100 && limitDown < 100)
            {
                STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.5) * 0.5) * 100) * 100)) / 100) / 100;
                STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 0.1) * 0.1) * 100) * 100)) / 100) / 100;
            }
            else if (limitUp >= 1000 && limitDown <= 1000)
            {
                STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 5) * 5) * 100) * 100)) / 100) / 100;
                STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 5) * 5) * 100) * 100)) / 100) / 100;
            }
            else if (limitUp >= 1000 && limitDown >= 1000)
            {
                STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 5) * 5) * 100) * 100)) / 100) / 100;
                STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 5) * 5) * 100) * 100)) / 100) / 100;
            }

            //STOCKUP = ((Math.Floor(((Math.Floor(limitUp / 0.5) * 0.5) * 100) * 100)) / 100) / 100;
            //STOCKDW = ((Math.Floor(((Math.Ceiling(limitDown / 0.5) * 0.5) * 100) * 100)) / 100) / 100;

            if (limitType.ToUpper() == "UP")
            {
                return Convert.ToDecimal(STOCKUP);
            }
            else
            {
                return Convert.ToDecimal(STOCKDW);
            }
        }


        public decimal CalcPriceYesterday(string price, string updown, string pricediff)
        {
            Decimal dPrice = 0;
            Decimal dPricediff = 0;
            Decimal dResult = 0;

            try
            {
                //異常回傳0
                if (Decimal.TryParse(price, out dPrice) == false || Decimal.TryParse(pricediff, out dPricediff) == false)
                {
                    return 0;
                }

                //判斷平
                if (dPricediff == 0 || updown == "" || updown == "X")
                {
                    return dPrice;
                }

                if (updown == "+")
                {
                    dResult = dPrice - dPricediff;
                }

                if (updown == "-")
                {
                    dResult = dPrice + dPricediff;
                }

            }
            catch (Exception)
            {
                dResult = 0;

            }




            return dResult;
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



        protected void AppendParameter(StringBuilder sb, string name, string value)
        {
            string encodedValue = System.Web.HttpUtility.UrlEncode(value);
            sb.AppendFormat("{0}={1}&", name, encodedValue);
        }


        public bool GetStockBrokerBSTSE()
        {
            //string sStockCode = "2330";
            string sUrl = "http://bsr.twse.com.tw/bshtm/bsMenu.aspx";
            StringBuilder sbPostData = new StringBuilder();
            //AppendParameter(sbPostData, "stk_code", sStockCode);
            //AppendParameter(sbPostData, "topage", "1");

            byte[] byteArray = Encoding.UTF8.GetBytes(sbPostData.ToString());

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
            request.Proxy = null;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // 寫入 Post Body Message 資料流
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(byteArray, 0, byteArray.Length);
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {



                HtmlDocument HtmlDoc = new HtmlDocument();
                HtmlDoc.Load(response.GetResponseStream(), Encoding.UTF8);
                //HtmlNodeCollection nodes1 = HtmlDoc.DocumentNode.SelectNodes("//form[@name='brokerBS2']");

                //if (nodes1 != null)
                //{
                //  foreach (HtmlNode node in nodes1)
                //  {
                //    //取得共有幾頁
                //    HtmlNodeCollection tdnodes = node.SelectNodes("a");

                //    foreach (HtmlNode item in tdnodes)
                //    {
                //      //PageList.Add(item.InnerText);
                //    }
                //  }
                //}
            }
            return true;
        }

        /// <summary>
        /// 券商買賣證券日報表查詢系統(暫停使用，因為無法驗證google grecaptcha)
        /// </summary>
        /// <returns></returns>
        public bool GetStockBrokerBS()
        {

            return false;

            try
            {
                //網頁瀏覽停滯時間
                int iSec = 2;

                DateTime dtTread = DateTime.MinValue;

                //取得上櫃所有的股票代號
                ssql = " select * from stockinfo where type = 'otc' and status = 'Y' and len(stockcode) = 4 ";
                List<StockInfo> StockInfoList = dbDapper.Query<StockInfo>(ssql);

                foreach (StockInfo si in StockInfoList)
                {
                    string sStockCode = si.stockcode;

                    //未取交易日期，則取得目前提供資料的交易日期
                    if (dtTread == DateTime.MinValue)
                    {
                        dtTread = getStockBrokerBSNow(sStockCode);
                    }

                    //判斷是否已經轉檔成功
                    ssql = " select * from stocklog where logtype = '{0}' and logdate = '{1}' and  memo = '{2}'  ";
                    StockLog slchk = dbDapper.QuerySingleOrDefault<StockLog>(string.Format(
                      ssql, M10Const.StockLogType.StockBrokerBSOtc
                      , Utils.getDateString(dtTread, M10Const.DateStringType.ADT1), sStockCode));
                    if (slchk != null) continue;

                    System.Threading.Thread.Sleep(iSec * 1000);

                    List<string> PageList = new List<string>();
                    string sUrl = "http://www.tpex.org.tw/web/stock/aftertrading/broker_trading/brokerBS.php?l=zh-tw";

                    StringBuilder sbPostData = new StringBuilder();
                    AppendParameter(sbPostData, "stk_code", sStockCode);
                    AppendParameter(sbPostData, "topage", "1");

                    byte[] byteArray = Encoding.UTF8.GetBytes(sbPostData.ToString());

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
                    request.Proxy = null;
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";

                    // 寫入 Post Body Message 資料流
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(byteArray, 0, byteArray.Length);
                    }

                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        HtmlDocument HtmlDoc = new HtmlDocument();
                        HtmlDoc.Load(response.GetResponseStream(), Encoding.UTF8);
                        HtmlNodeCollection nodes1 = HtmlDoc.DocumentNode.SelectNodes("//form[@name='brokerBS2']");

                        if (nodes1 != null)
                        {
                            foreach (HtmlNode node in nodes1)
                            {
                                //取得共有幾頁
                                HtmlNodeCollection tdnodes = node.SelectNodes("a");

                                foreach (HtmlNode item in tdnodes)
                                {
                                    PageList.Add(item.InnerText);
                                }
                            }
                        }
                    }

                    //每個分頁進行資料取得
                    foreach (string sPage in PageList)
                    {
                        //每一個分頁，暫停十秒鐘
                        System.Threading.Thread.Sleep(iSec * 1000);

                        sbPostData.Clear();
                        AppendParameter(sbPostData, "stk_code", sStockCode);
                        AppendParameter(sbPostData, "topage", sPage);

                        byteArray = Encoding.UTF8.GetBytes(sbPostData.ToString());

                        request = (HttpWebRequest)WebRequest.Create(sUrl);
                        request.Proxy = null;
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";

                        // 寫入 Post Body Message 資料流
                        using (Stream requestStream = request.GetRequestStream())
                        {
                            requestStream.Write(byteArray, 0, byteArray.Length);
                        }

                        using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                        {
                            HtmlDocument HtmlDoc = new HtmlDocument();
                            HtmlDoc.Load(response.GetResponseStream(), Encoding.UTF8);
                            HtmlNodeCollection MainTableNodes = HtmlDoc.DocumentNode.SelectNodes("//table[@id='data_01']");

                            if (MainTableNodes == null) continue;

                            //取得共有幾頁
                            HtmlNodeCollection SubTableNodes = MainTableNodes[0].SelectNodes("//table");

                            foreach (HtmlNode SubTableNode in SubTableNodes)
                            {
                                if (SubTableNode.Id == "data_01") continue;


                                HtmlNodeCollection TrNodes = SubTableNode.SelectNodes("tr");

                                foreach (HtmlNode TrNode in TrNodes)
                                {
                                    HtmlNodeCollection TdNodes = TrNode.SelectNodes("td");

                                    //取得日期
                                    if (TdNodes.Count == 4)
                                    {
                                        string sTreadDate = TdNodes[1].InnerText;
                                        sTreadDate = sTreadDate.Replace("年", "").Replace("月", "").Replace("日", "");
                                        dtTread = new DateTime(int.Parse(sTreadDate.Substring(0, 3)) + 1911, int.Parse(sTreadDate.Substring(3, 2)), int.Parse(sTreadDate.Substring(5, 2)));
                                    }

                                    if (TdNodes.Count != 5) continue;

                                    if (TdNodes[1].InnerText == "券商") continue;

                                    string sVoucher = TdNodes[1].InnerText;
                                    string sVoucherCode = "";
                                    string sVoucherName = "";
                                    string[] aVoucher = sVoucher.Split(new string[] { "&nbsp;" }, StringSplitOptions.None);
                                    if (aVoucher.Length == 2)
                                    {
                                        sVoucherCode = aVoucher[0];
                                        sVoucherName = aVoucher[1];

                                        //判斷Voucher資料是否存在，如不存在則新增一筆
                                        ssql = " select * from stockvoucher where vouchercode = '{0}' ";
                                        StockVoucher oStockVoucher = dbDapper.QuerySingleOrDefault<StockVoucher>(string.Format(ssql, sVoucherCode));

                                        if (oStockVoucher == null)
                                        {
                                            //新增
                                            oStockVoucher = new StockVoucher();
                                            oStockVoucher.vouchercode = sVoucherCode;
                                            oStockVoucher.vouchername = sVoucherName;
                                            oStockVoucher.updatetime = Utils.getDatatimeString();
                                            dbDapper.Insert(oStockVoucher);
                                        }
                                        else
                                        {
                                            //名稱不一樣進行更新
                                            if (oStockVoucher.vouchername != sVoucherName)
                                            {
                                                oStockVoucher.vouchername = sVoucherName;
                                                dbDapper.Update(oStockVoucher);
                                            }
                                        }
                                    }

                                    if (aVoucher.Length == 1)
                                    {
                                        sVoucherCode = aVoucher[0];
                                    }

                                    //判斷資料是否存在，
                                    ssql = " select * from StockBrokerBS where stockcode = '{0}' and stockdate = '{1}' and bsno = '{2}' ";
                                    StockBrokerBS oStockBrokerBS = dbDapper.QuerySingleOrDefault<StockBrokerBS>(string.Format(ssql
                                      , sStockCode, Utils.getDateString(dtTread, M10Const.DateStringType.ADT1)
                                      , TdNodes[0].InnerText));

                                    if (oStockBrokerBS == null)
                                    {
                                        oStockBrokerBS = new StockBrokerBS();
                                        oStockBrokerBS.stockcode = sStockCode;
                                        oStockBrokerBS.stockdate = Utils.getDateString(dtTread, M10Const.DateStringType.ADT1);
                                        oStockBrokerBS.vouchercode = sVoucherCode;
                                        oStockBrokerBS.bsno = TdNodes[0].InnerText;
                                        oStockBrokerBS.bsprice = Convert.ToDecimal(TdNodes[2].InnerText);
                                        oStockBrokerBS.bvol = int.Parse(TdNodes[3].InnerText.Replace(",", ""));
                                        oStockBrokerBS.svol = int.Parse(TdNodes[4].InnerText.Replace(",", ""));
                                        oStockBrokerBS.updatetime = Utils.getDatatimeString();
                                        dbDapper.Insert(oStockBrokerBS);
                                    }
                                    else
                                    {
                                        oStockBrokerBS.vouchercode = sVoucherCode;
                                        oStockBrokerBS.bsprice = Convert.ToDecimal(TdNodes[2].InnerText);
                                        oStockBrokerBS.bvol = int.Parse(TdNodes[3].InnerText.Replace(",", ""));
                                        oStockBrokerBS.svol = int.Parse(TdNodes[4].InnerText.Replace(",", ""));
                                        oStockBrokerBS.updatetime = Utils.getDatatimeString();
                                        dbDapper.Update(oStockBrokerBS);
                                    }
                                }
                            }

                        }
                    }

                    //寫入log紀錄
                    StockLog sl = new StockLog();
                    sl.logdate = Utils.getDateString(dtTread, M10Const.DateStringType.ADT1);
                    sl.logdatetime = Utils.getDatatimeString();
                    sl.logstatus = M10Const.StockLogStatus.s200;
                    sl.memo = sStockCode;
                    sl.logtype = M10Const.StockLogType.StockBrokerBSOtc;
                    dbDapper.Insert(sl);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }

            return true;
        }



        //public Stream GetWebData(string Url, string PostData)
        //{
        //  byte[] myData = Encoding.Default.GetBytes(PostData);

        //  HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(Url);
        //  myRequest.Method = "POST";
        //  myRequest.Accept = "*/*";
        //  myRequest.ContentType = "application/x-www-form-urlencoded";
        //  myRequest.ContentLength = myData.Length;

        //  Stream myStream = myRequest.GetRequestStream();
        //  myStream.Write(myData, 0, myData.Length);
        //  myStream.Close();

        //  HttpWebResponse myResponse;
        //  try { myResponse = (HttpWebResponse)myRequest.GetResponse(); }
        //  catch (Exception) { return null; }

        //  ResponseStream = myResponse.GetResponseStream();

        //  return ResponseStream;
        //}

        public bool GetStockBrokerBStest()
        {
            try
            {

                string sUrl = "http://www.tpex.org.tw/web/stock/aftertrading/broker_trading/download_ALLCSV.php";


                string PostURL = sUrl;

                using (WebClient wc = Utils.getNewWebClient())
                {
                    wc.Proxy = null;
                    wc.Encoding = Encoding.UTF8;
                    //wc.Encoding = Encoding.GetEncoding.getencoding(950);

                    //wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    NameValueCollection nc = new NameValueCollection();
                    nc["stk_code"] = "6180";
                    //nc["charset"] = "UTF-8";
                    //string sResult = wc.UploadString(PostURL, "POST", "stk_code=6180");


                    byte[] bResult = wc.UploadValues(PostURL, "POST", nc);

                    //wc.DownloadString()
                    //string text = wc.DownloadString(surl1);

                    string resultXML = Encoding.UTF8.GetString(bResult);
                }

                //return true;

                //string targetUrl = "http://localhost:13772/TWebRequest/TargetHandler.ashx";
                //string parame = "stk_code=6180&charset=UTF-8";
                //byte[] postData = Encoding.UTF8.GetBytes(parame);

                //HttpWebRequest request1 = HttpWebRequest.Create(sUrl) as HttpWebRequest;
                //request1.Method = "POST";
                //request1.ContentType = "application/x-www-form-urlencoded";
                //request1.Timeout = 30000;
                //request1.ContentLength = postData.Length;
                //// 寫入 Post Body Message 資料流
                //using (Stream st = request1.GetRequestStream())
                //{
                //  st.Write(postData, 0, postData.Length);
                //}

                //string result = "";
                //// 取得回應資料
                //using (HttpWebResponse response = request1.GetResponse() as HttpWebResponse)
                //{
                //  using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                //  {
                //    result = sr.ReadToEnd();
                //  }
                //}





                List<string> PageList = new List<string>();

                StringBuilder sbPostData = new StringBuilder();

                AppendParameter(sbPostData, "stk_code", "6180");
                //AppendParameter(sbPostData, "charset", "UTF-8");

                byte[] byteArray = Encoding.UTF8.GetBytes(sbPostData.ToString());

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
                request.Proxy = null;
                request.Method = WebRequestMethods.Http.Post;
                request.ContentType = "application/x-www-form-urlencoded";

                // 寫入 Post Body Message 資料流
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(byteArray, 0, byteArray.Length);
                }


                //string postData = "stk_code=6180";
                //request.ContentLength = postData.Length;
                ////request.ContentType = "application/x-www-form-urlencoded";
                //// Write the post data to the HTTP request
                //StreamWriter requestWriter = new StreamWriter(
                //    request.GetRequestStream(),
                //    System.Text.Encoding.ASCII);
                //requestWriter.Write(postData);
                //requestWriter.Close();



                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        string results = sr.ReadToEnd();
                    }


                    //sr.Close();


                    //HtmlDocument HtmlDoc = new HtmlDocument();
                    //HtmlDoc.Load(response.GetResponseStream(), Encoding.UTF8);
                    //HtmlNodeCollection nodes1 = HtmlDoc.DocumentNode.SelectNodes("//form[@name='brokerBS2']");

                    //if (nodes1 != null)
                    //{
                    //  foreach (HtmlNode node in nodes1)
                    //  {
                    //    //取得共有幾頁
                    //    HtmlNodeCollection tdnodes = node.SelectNodes("a");

                    //    foreach (HtmlNode item in tdnodes)
                    //    {
                    //      PageList.Add(item.InnerText);
                    //    }
                    //  }
                    //}
                }





                //網頁瀏覽停滯時間
                int iSec = 2;




            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }

            return true;
        }





        protected DateTime getStockBrokerBSNow(string sStockCode)
        {
            DateTime dt = DateTime.MinValue;
            string sUrl = "http://www.tpex.org.tw/web/stock/aftertrading/broker_trading/brokerBS.php?l=zh-tw";

            StringBuilder sbPostData = new StringBuilder();
            AppendParameter(sbPostData, "stk_code", sStockCode);
            AppendParameter(sbPostData, "topage", "1");

            byte[] byteArray = Encoding.UTF8.GetBytes(sbPostData.ToString());

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
            request.Proxy = null;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // 寫入 Post Body Message 資料流
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(byteArray, 0, byteArray.Length);
            }

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                HtmlDocument HtmlDoc = new HtmlDocument();
                HtmlDoc.Load(response.GetResponseStream(), Encoding.UTF8);
                HtmlNodeCollection MainTableNodes = HtmlDoc.DocumentNode.SelectNodes("//table[@id='data_01']");

                //取得共有幾頁
                HtmlNodeCollection SubTableNodes = MainTableNodes[0].SelectNodes("//table");

                foreach (HtmlNode SubTableNode in SubTableNodes)
                {
                    if (SubTableNode.Id == "data_01") continue;


                    HtmlNodeCollection TrNodes = SubTableNode.SelectNodes("tr");

                    foreach (HtmlNode TrNode in TrNodes)
                    {
                        HtmlNodeCollection TdNodes = TrNode.SelectNodes("td");

                        //取得日期
                        if (TdNodes.Count == 4)
                        {
                            string sTreadDate = TdNodes[1].InnerText;
                            sTreadDate = sTreadDate.Replace("年", "").Replace("月", "").Replace("日", "");
                            dt = new DateTime(int.Parse(sTreadDate.Substring(0, 3)) + 1911, int.Parse(sTreadDate.Substring(3, 2)), int.Parse(sTreadDate.Substring(5, 2)));
                        }
                    }
                }
            }

            return dt;
        }

        public List<dynamic> getStockDataLvstg(string stockcode, string date)
        {
            List<dynamic> StockafterList = new List<dynamic>();

            ssql = @" select stockcode, stockdate,pricelast as price from stockafter where stockcode = '{0}' and stockdate >= '{1}' order by stockdate asc ";
            StockafterList = dbDapper.Query(string.Format(ssql, stockcode, date));

            return StockafterList;
        }

        //Api呼叫==取得巨量換手資料
        public List<dynamic> getStockGet(string StockDate)
        {
            List<dynamic> StockGetList = new List<dynamic>();

            ssql = @" select StockGet.stockcode,stockinfo.stockname from StockGet 
                inner join stockinfo on StockGet.stockcode = stockinfo.stockcode
                where stockdate = '{0}' order by StockGet.stockcode ";
            StockGetList = dbDapper.Query<dynamic>(string.Format(ssql, StockDate));

            return StockGetList;
        }


        /// <summary>
        /// 計算每日巨量換手
        /// </summary>
        public void GetHugeTurnover(string sStockCode, string sRunDate)
        {
            try
            {
                //刪除目前存在資料
                //dbDapper.Execute("delete stockget");

                //stockcode = "3162";

                //判斷盤後資料是否存在
                ssql = " select * from StockAfter where stockcode  = '{0}' and stockdate = '{1}'  ";
                ssql = string.Format(ssql, sStockCode, sRunDate);
                int iTotal = dbDapper.QueryTotalCount(ssql);
                if (iTotal == 0)
                {
                    return;
                }

                //判斷當天為180天最大量
                Boolean bCheck180Max = check180Max(sStockCode, sRunDate);
                if (bCheck180Max == false)
                {
                    return;
                }


                ssql = @" select top 3 * from stockafter 
                            where stockdate <= '{0}' and stockcode = '{1}' order by stockdate desc
                       ";
                ssql = string.Format(ssql, sRunDate, sStockCode);
                List<Stockafter> saList = dbDapper.Query<Stockafter>(ssql);

                if (saList.Count == 3)
                {
                    //to
                    Stockafter saToday = saList[0];
                    //yes
                    Stockafter saYes = saList[1];
                    //per
                    Stockafter saPer = saList[2];

                    //判斷前一天+9%
                    //Boolean bCheckUp9 = checkUp9(saYes, saPer);

                    //判斷兩天+15%
                    Boolean bCheckUp15 = checkUp15(saYes, saPer);

                    //今天量是昨天的兩倍
                    Boolean bCheckTodayOver2 = checkTodayOver2(saToday, saYes);


                    if (bCheckUp15 == true && bCheck180Max == true && bCheckTodayOver2 == true)
                    {
                        //確認資料是否已存在
                        ssql = " select * from StockGet where stockcode = '{0}' and stockdate = '{1}' and getdate  = '{2}' ";
                        ssql = string.Format(ssql, sStockCode, saToday.stockdate, sRunDate);
                        StockGet sgcheck = dbDapper.QuerySingleOrDefault<StockGet>(ssql);
                        if (sgcheck == null)
                        {
                            StockGet sg = new StockGet();
                            sg.getdate = sRunDate;
                            sg.stockcode = saToday.stockcode;
                            sg.stockdate = saToday.stockdate;

                            dbDapper.Insert(sg);
                        }

                    }


                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "巨量換手轉檔異常通報");
            }

        }

        /// <summary>
        /// 判斷前一天上漲>9%
        /// </summary>
        /// <param name="saYes"></param>
        /// <param name="saPer"></param>
        /// <returns></returns>
        public Boolean checkUp9(Stockafter saYes, Stockafter saPer)
        {
            Boolean bResult = false;


            if (saYes.updown == "+")
            {
                double aa = 0.09;
                decimal dd = 0;
                dd = (Convert.ToDecimal(saYes.pricelast) - Convert.ToDecimal(saPer.pricelast)) / Convert.ToDecimal(saPer.pricelast);
                if (dd > Convert.ToDecimal(aa))
                {
                    bResult = true;
                }
            }

            return bResult;
        }

        /// <summary>
        /// 判斷兩天上漲15%
        /// </summary>
        /// <param name="saYes"></param>
        /// <param name="saPer"></param>
        /// <returns></returns>
        public Boolean checkUp15(Stockafter saYes, Stockafter saPer)
        {
            Boolean bResult = false;

            double dTarget = 0.15;
            decimal dResult = 0;
            //昨天漲幅
            if (saYes.updown == "+")
            {
                dResult += Convert.ToDecimal(saYes.pricediff) / Convert.ToDecimal(saYes.priceyesterday);
            }
            //前天漲幅
            if (saPer.updown == "+")
            {
                dResult += Convert.ToDecimal(saPer.pricediff) / Convert.ToDecimal(saPer.priceyesterday);
            }

            //大於15%
            if (dResult > Convert.ToDecimal(dTarget))
            {
                bResult = true;
            }


            return bResult;
        }

        /// <summary>
        /// 當天是180根K棒 最大量
        /// </summary>
        /// <param name="saToday"></param>
        /// <returns></returns>
        public Boolean check180Max(Stockafter saToday)
        {
            Boolean bResult = false;


            ssql = @" select max(dealnum) from (
                      select top 180 * from stockafter where stockcode = '{1}' 
                      and stockdate <= '{0}'  order by stockdate desc
                      ) as dd 
                     ";
            ssql = string.Format(ssql, saToday.stockdate, saToday.stockcode);
            object sa = dbDapper.ExecuteScale(ssql);
            if (sa != null)
            {
                long lDealNum = Convert.ToInt64(sa.ToString());

                if (saToday.dealnum == lDealNum)
                {
                    bResult = true;
                }
            }


            return bResult;
        }

        /// <summary>
        /// 當天是180根K棒 最大量
        /// </summary>
        /// <param name="sStockCode"></param>
        /// <param name="sStockDate"></param>
        /// <returns></returns>
        public Boolean check180Max(string sStockCode, string sStockDate)
        {
            Boolean bResult = false;


            ssql = @" select * from stockafter where stockcode = '{1}' and stockdate = '{0}' ";
            ssql = string.Format(ssql, sStockDate, sStockCode);

            Stockafter oStockAfter = dbDapper.QuerySingleOrDefault<Stockafter>(ssql);

            bResult = check180Max(oStockAfter);


            return bResult;
        }




        /// <summary>
        /// 今天的量是昨天的兩倍
        /// </summary>
        /// <param name="saToday"></param>
        /// <param name="saYes"></param>
        /// <returns></returns>
        public Boolean checkTodayOver2(Stockafter saToday, Stockafter saYes)
        {
            Boolean bResult = false;

            if (saToday.dealnum > saYes.dealnum * 2)
            {
                bResult = true;
            }

            return bResult;
        }


        /// <summary>
        /// [暫緩]轉檔紀錄
        /// </summary>
        /// <param name="TransDateTime"></param>
        /// <param name="pType"></param>
        /// <param name="FinishDateTime"></param>
        public void LogStockTransRec(DateTime TransDateTime, M10Const.StockTransRecType pType, DateTime FinishDateTime)
        {
            string sTransDate = Utils.getDateString(TransDateTime, M10Const.DateStringType.ADT1);
            ssql = " select * from StockTransRec  where stockdate = @stockdate and type = @type ";
            var p = dbDapper.GetNewDynamicParameters();
            p.Add("@stockdate", sTransDate);
            p.Add("@type", pType.ToString());
            StockTransRec str = dbDapper.QuerySingleOrDefault<StockTransRec>(ssql, p);

            if (str == null)
            {
                str = new StockTransRec();
                str.stockdate = sTransDate;
                str.type = pType.ToString();
                str.stockcode = "";
                str.status = "";
                str.finish = "Y";
                str.finishtime = Utils.getDatatimeString();
                str.updatetime = Utils.getDatatimeString();

                dbDapper.Insert(str);
            }
            else
            {
                str.finish = "Y";
                str.finishtime = Utils.getDatatimeString();
                str.updatetime = Utils.getDatatimeString();

                dbDapper.Update(str);
            }
        }



        public void getThreeFuturesDailyData(DateTime dtTarget)
        {
            string sUrl = "https://www.taifex.com.tw/cht/3/futContractsDateView";

            sUrl = "https://www.taifex.com.tw/cht/3/futContractsDateDown?commodityId=MXF&queryEndDate=2019/06/25&queryStartDate=2019/06/25";


            using (WebClient wc = getNewWebClient())
            {
                wc.Encoding = Encoding.GetEncoding(950);
                string text = wc.DownloadString(sUrl);
            }




        }



        public List<string> GetCallComm(string sComm) 
        {
            List<string> ResultList = new List<string>();

            switch (sComm)
            {
               case "TUMD":
                    ResultList = GetCommTUMD();

                    break;
                default:

                    break;
            }

            if (sComm.Substring(0,1) == "Q")
            {
                ResultList = GetOnlineTUMD(sComm);
            }


            return ResultList;
        }

        private List<string> GetCommTUMD()
        {
            List<string> ResultList = new List<string>();


            string sStockDate = DateTime.Now.ToString("yyyyMMdd");
            ssql = " select * from stocktumd where stockdate = '{0}' ";
            StockTumd StockTumdItem = dbDapper.QuerySingleOrDefault<StockTumd>(string.Format(ssql, sStockDate));

            if (StockTumdItem != null)
            {
                ResultList.Add(string.Format("TUMD 日期：{0}", sStockDate));
                ResultList.Add("[加權]");
                ResultList.Add(string.Format("TU：{0}",StockTumdItem.TseTU));
                ResultList.Add(string.Format("TM：{0}", StockTumdItem.TseTM));
                ResultList.Add(string.Format("TD：{0}", StockTumdItem.TseTD));
                ResultList.Add(string.Format("TW：{0}", StockTumdItem.TseTW));
                ResultList.Add("[櫃買]");
                ResultList.Add(string.Format("TU：{0}", StockTumdItem.OtcTU));
                ResultList.Add(string.Format("TM：{0}", StockTumdItem.OtcTM));
                ResultList.Add(string.Format("TD：{0}", StockTumdItem.OtcTD));
                ResultList.Add(string.Format("TW：{0}", StockTumdItem.OtcTW));
                ResultList.Add("[台指]");
                ResultList.Add(string.Format("TU：{0}", StockTumdItem.TxfTU));
                ResultList.Add(string.Format("TM：{0}", StockTumdItem.TxfTM));
                ResultList.Add(string.Format("TD：{0}", StockTumdItem.TxfTD));
                ResultList.Add(string.Format("TW：{0}", StockTumdItem.TxfTW));
            }

            return ResultList;
        }

        public List<string> GetOnlineTUMD(string sComm)
        {
            List<string> ResultList = new List<string>();



            ResultList = stockhelper.getStockTUMD(sComm);

            //string sStockDate = DateTime.Now.ToString("yyyyMMdd");
            //ssql = " select * from stocktumd where stockdate = '{0}' ";
            //StockTumd StockTumdItem = dbDapper.QuerySingleOrDefault<StockTumd>(string.Format(ssql, sStockDate));

            //if (StockTumdItem != null)
            //{
            //    ResultList.Add(string.Format("TUMD 日期：{0}", sStockDate));
            //    ResultList.Add("[加權]");
            //    ResultList.Add(string.Format("TU：{0}", StockTumdItem.TseTU));
            //    ResultList.Add(string.Format("TM：{0}", StockTumdItem.TseTM));
            //    ResultList.Add(string.Format("TD：{0}", StockTumdItem.TseTD));
            //    ResultList.Add(string.Format("TW：{0}", StockTumdItem.TseTW));
            //    ResultList.Add("[櫃買]");
            //    ResultList.Add(string.Format("TU：{0}", StockTumdItem.OtcTU));
            //    ResultList.Add(string.Format("TM：{0}", StockTumdItem.OtcTM));
            //    ResultList.Add(string.Format("TD：{0}", StockTumdItem.OtcTD));
            //    ResultList.Add(string.Format("TW：{0}", StockTumdItem.OtcTW));
            //    ResultList.Add("[台指]");
            //    ResultList.Add(string.Format("TU：{0}", StockTumdItem.TxfTU));
            //    ResultList.Add(string.Format("TM：{0}", StockTumdItem.TxfTM));
            //    ResultList.Add(string.Format("TD：{0}", StockTumdItem.TxfTD));
            //    ResultList.Add(string.Format("TW：{0}", StockTumdItem.TxfTW));
            //}

            return ResultList;
        }

    }
}
