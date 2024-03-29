﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using C10Mvc.Class;
using M10.lib.model;
using M10.lib;



namespace C10Mvc.Controllers
{
    [RoutePrefix("StockApi")]
    public class StockController : BaseApiController
    {
        /// <summary>
        /// LineBot接收通用指令後，在主機端處理後，回傳結果給LineBot
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getCallComm")]
        public dynamic getCallComm(string MessageText)
        {
            //先統一轉大寫判斷
            MessageText = MessageText.ToUpper();

            dynamic CommResult;

            List<string> CommList = new List<string>();
            CommList.Add("TUMD");


            try
            {
                //判斷指令是否符合，不符合則不執行
                if (MessageText.Substring(0,1) != "Q") //股泰軟體TUMD查詢
                {
                    if (CommList.Contains(MessageText) == false)
                    {
                        CommResult = new { status = "Y", datas = new List<string>() };

                        return CommResult;
                    }
                }
                
                List<string> Datas = stockhelper.GetCallComm(MessageText);
                CommResult = new { status = "Y", datas = Datas };                

            }
            catch (Exception ex)
            {
                CommResult = new { status = "N", datas = new List<string>() };
            }

            return CommResult;

        }

        [HttpGet]
        [Route("getStockType")]
        public StockInfo getStockType()
        {
            if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return null;

            NameValueCollection Params = M10.lib.Utils.ParseQueryString(ActionContext.Request.RequestUri.Query);


            string ssql = " select * from stockinfo where 1=1 and status = 'Y' ";
            if (Params["StockCode"] != null)
            {
                ssql += string.Format(" and stockcode = '{0}' ", Params["StockCode"]);
            }


            StockInfo StockInfoItem = dbDapper.QuerySingleOrDefault<StockInfo>(ssql);

            return StockInfoItem;
        }


        [HttpGet]
        [Route("getStockThreeTrade")]
        public dynamic getStockThreeTrade()
        {
            //if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return null;

            //NameValueCollection Params = M10.lib.Utils.ParseQueryString(ActionContext.Request.RequestUri.Query);

            string sMaxDate = "";
            string ssql = " select max(date) from Stockthreetrade ";

            var maxdate = dbDapper.ExecuteScale(ssql);
            if (maxdate != null)
            {
                sMaxDate = maxdate.ToString();
            }

            ssql = @"select top 30 * from Stockthreetrade a
                left join stockinfo b on a.stockcode = b.stockcode
                where threeinv > 0 
                and date = '{0}'  
                order by threeinv desc
                ";

            List<dynamic> ThreeTradeList = dbDapper.Query<dynamic>(string.Format(ssql, sMaxDate));

            List<dynamic> ListB = ThreeTradeList
                        .Select(t => new
                        {
                            stockcode = t.stockcode
                          ,
                            threeinv = t.threeinv
                                          ,
                            stockname = t.stockname
                        })
                        .ToList<dynamic>();

            ssql = @"select top 30 * from Stockthreetrade a
                left join stockinfo b on a.stockcode = b.stockcode
                where threeinv < 0
                and date = '{0}'  
                order by threeinv 
                ";

            ThreeTradeList = dbDapper.Query<dynamic>(string.Format(ssql, sMaxDate));

            List<dynamic> ListS = ThreeTradeList
                        .Select(t => new
                        {
                            stockcode = t.stockcode
                          ,
                            threeinv = t.threeinv
                                          ,
                            stockname = t.stockname
                        })
                        .ToList<dynamic>();

            dynamic dd = new { date = sMaxDate, s = ListS, b = ListB };

            return dd;
        }

        [HttpGet]
        [Route("getStockThreeTrade3d")]
        public dynamic getStockThreeTrade3d()
        {
            ////if (string.IsNullOrWhiteSpace(ActionContext.Request.RequestUri.Query) == true) return null;

            ////NameValueCollection Params = M10.lib.Utils.ParseQueryString(ActionContext.Request.RequestUri.Query);

            string sMaxDate = "";
            string ssql = " select max(date) from Stockthreetrade ";

            var maxdate = dbDapper.ExecuteScale(ssql);
            if (maxdate != null)
            {
                sMaxDate = maxdate.ToString();
            }

            ssql = @"
            select top 30 * from (
              select stockcode,count(*) as tt , sum(threeinv) as threeinv  from (
                select * from Stockthreetrade
                where date in (
                  select top 3 * from (
                    select distinct date from Stockthreetrade 
                  ) as a order by a.date desc
                ) and threeinv > 0
              ) b group by stockcode
            ) as c 
            left join stockinfo d on c.stockcode = d.stockcode
            where c.tt =3 order by threeinv desc
            ";

            List<dynamic> ThreeTradeList = dbDapper.Query<dynamic>(ssql);

            List<dynamic> ListB3d = ThreeTradeList
                        .Select(t => new
                        {
                            stockcode = t.stockcode
                          ,
                            threeinv = t.threeinv
                                          ,
                            stockname = t.stockname
                        })
                        .ToList<dynamic>();

            ssql = @"
            select top 30 * from (
              select stockcode,count(*) as tt , sum(threeinv) as threeinv  from (
                select * from Stockthreetrade
                where date in (
                  select top 3 * from (
                    select distinct date from Stockthreetrade 
                  ) as a order by a.date desc
                ) and threeinv < 0
              ) b group by stockcode
            ) as c 
            left join stockinfo d on c.stockcode = d.stockcode
            where c.tt =3 order by threeinv asc
                ";

            ThreeTradeList = dbDapper.Query<dynamic>(string.Format(ssql, sMaxDate));

            List<dynamic> ListS3d = ThreeTradeList
                        .Select(t => new
                        {
                            stockcode = t.stockcode
                          ,
                            threeinv = t.threeinv
                                          ,
                            stockname = t.stockname
                        })
                        .ToList<dynamic>();

            dynamic dd = new { date = sMaxDate, s = ListS3d, b = ListB3d };

            return dd;
        }



        [HttpGet]
        [Route("getStockRealtime")]
        public dynamic getStockRealtime(string stockcode)
        {
            //get from yahoo web
            //StockRuntime sr = oStockUtil.getStockRealtimeYahooWeb(stockcode);

            //get from yahoo api
            StockRuntime sr = stockhelper.getStockRealtimeYahooApi(stockcode);

            //盤前沒資料改取得歷史資料
            //if (sr.z == "")
            //{
            //  ssql = " select top 1 * from stockafter where stockcode = '{0}' order by stockdate desc ";
            //  ssql = string.Format(ssql, stockcode);
            //  Stockafter sa = dbDapper.QuerySingleOrDefault<Stockafter>(ssql);
            //  if (sa != null)
            //  {
            //    sr.status = M10Const.StockRuntimeStatus.Histroy;
            //    sr.z = sa.pricelast.ToString();
            //    sr.y = sa.priceyesterday.ToString();
            //  }
            //}

            return sr;
        }

        [HttpGet]
        [Route("getStockDataLvstg")]
        public dynamic getStockDataLvstg(string stockcode, string date)
        {
            //取得資料
            List<dynamic> TempList = stockhelper.getStockDataLvstg(stockcode, date);







            //盤前沒資料改取得歷史資料
            //if (sr.z == "")
            //{
            //  ssql = " select top 1 * from stockafter where stockcode = '{0}' order by stockdate desc ";
            //  ssql = string.Format(ssql, stockcode);
            //  Stockafter sa = dbDapper.QuerySingleOrDefault<Stockafter>(ssql);
            //  if (sa != null)
            //  {
            //    sr.status = M10Const.StockRuntimeStatus.Histroy;
            //    sr.z = sa.pricelast.ToString();
            //    sr.y = sa.priceyesterday.ToString();
            //  }
            //}

            return TempList;
        }



        [HttpGet]
        [Route("getStockHugeTurnover")]
        public List<dynamic> getStockHugeTurnover(string date)
        {
            //取得資料
            //List<StockGet> TempList = stockhelper.getStockGet(date);

            List<dynamic> TempList = stockhelper.getStockGet(date);

            return TempList;
        }

        [HttpGet]
        [Route("DoStockAfter")]
        public void DoStockAfter(string date)
        {
            
            logger.Info("START DoStockAfter()");
            DateTime dt = Utils.getStringToDateTime(date);

            for (DateTime dtTemp = dt; dtTemp >= dt.AddDays(-3); dtTemp = dtTemp.AddDays(-1))
            {
                logger.Info(string.Format("{0}=={1}", "DoStockAfter()", Utils.getDatatimeString(dtTemp)));
                #region tse-StockAfter
                stockhelper.GetStockAfterTse(dtTemp);
                #endregion

                #region otc-StockAfter
                stockhelper.GetStockAfterOtc(dtTemp);
                #endregion
            }

            logger.Info("END DoStockAfter()");
        }


        [HttpPost]
        [Route("PostTest")]
        public List<dynamic> PostTest(Player palyer)
        {
            //取得資料
            //List<StockGet> TempList = stockhelper.getStockGet(date);

            //List<dynamic> TempList = stockhelper.getStockGet(date);
            List<dynamic> TempList = new List<dynamic>();
            return TempList;
        }

        //宣告Model類別承接前端傳入資料
        public class Player
        {
            //public int Id;
            public string Name;
            //public DateTime RegDate;
            //public int Score;
        }
    }
}
