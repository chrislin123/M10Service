﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using System.Text;
using Quartz;
using M10.lib;
using NLog;
using M10.lib.model;
using HtmlAgilityPack;
using System.IO;

namespace C10Mvc.Controllers
{
  public class JobController : ApiController
  {
  }

  public class BaseJob 
  {
    private NLog.Logger _logger ;
    protected string ssql = string.Empty;
    private string _ConnectionString;
    private DALDapper _dbDapper;

    protected Logger logger {
      get
      {
        if (_logger == null)
        {
          _logger =  NLog.LogManager.GetCurrentClassLogger();
        }

        return _logger;
      }
    }

    protected DALDapper dbDapper
    {
      get
      {
        if (_dbDapper == null)
        {
          _dbDapper = new DALDapper(ConnectionString);
        }

        return _dbDapper;
      }
    }

    protected string ConnectionString
    {
      get
      {
        if (string.IsNullOrEmpty(_ConnectionString))
        {
          _ConnectionString = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DBDefault"]].ConnectionString;
        }

        return _ConnectionString;
      }
    }
  }

  public class SendMailTask : BaseJob, IJob
  {
    private void Log(string msg)
    { 
      System.IO.File.AppendAllText(@"C:\Temp\log.txt", msg + Environment.NewLine);
    }

    public void DoSendMail()
    {
      Log("Entering DoSendMail() at " + DateTime.Now.ToString());

      int.Parse("tt1");
      // 發送 email。這裡只固定輸出一筆文字訊息至 log 檔案，方便觀察測試。
      // 每發送一封 email 就檢查一次 IntervalTask.Current.SuttingDown 以配合外部的終止事件。
      string msg = String.Format("DoSendMail() at {0:yyyy/MM/dd HH:mm:ss}", DateTime.Now);
      Log(msg);
      //Thread.Sleep(2000);

    }

    public void  Execute(IJobExecutionContext context)
    { 
      try
      {
        DoSendMail();
      }
      catch (Exception ex)
      {
        logger.Log(NLog.LogLevel.Error, ex.Message);
      }
    }
  }

  //一次只執行一個體
  [DisallowConcurrentExecutionAttribute]
  public class StockTransTask : BaseJob, IJob
  {
    /// <summary>
    /// 寫入執行log
    /// </summary>
    /// <param name="msg"></param>
    private void Log(string msg)
    {
      System.IO.File.AppendAllText(@"C:\Temp\log.txt", msg + Environment.NewLine);
    }

    public void DoStockTrans()
    {
      //Log("START DoStockTrans() at " + DateTime.Now.ToString());
      logger.Info("START DoStockTrans()");
      try
      {
        List<string> TypeList = new List<string>();
        TypeList.Add("tse");
        TypeList.Add("otc");

        foreach (string sType in TypeList)
        {
          string surl = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=2";

          if (sType == "tse") surl = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=2";
          if (sType == "otc") surl = "http://isin.twse.com.tw/isin/C_public.jsp?strMode=4";


          HtmlWeb webClient = new HtmlWeb();
          //網頁特殊編碼
          webClient.OverrideEncoding = Encoding.GetEncoding(950);

          // 載入網頁資料 
          HtmlAgilityPack.HtmlDocument doc = webClient.Load(surl);

          // 裝載查詢結果 
          HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//table[2]/tr");

          if (nodes.Count > 0)
          {
            ssql = " update stockinfo set updstatus = 'N' where type = '{0}'  ";
            dbDapper.Execute(string.Format(ssql,sType));
          }
          int idx = 1;
          foreach (HtmlNode node in nodes)
          {
            string sCode = "";
            string sName = "";
            string sStatus = "";
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
                sStatus = "新增";
                StockInfoItem = new StockInfo();
                StockInfoItem.stockcode = sCode;
                StockInfoItem.stockname = sName;
                StockInfoItem.type = sType;
                StockInfoItem.updatetime = Utils.getDatatimeString();
                StockInfoItem.updstatus = "Y";
                StockInfoItem.status = "Y";
                //StockInfoItem.updatetime =

                dbDapper.Insert(StockInfoItem);
              }
              else
              {
                sStatus = "更新";
                StockInfoItem.type = sType;
                StockInfoItem.updatetime = Utils.getDatatimeString();
                StockInfoItem.updstatus = "Y";
                StockInfoItem.status = "Y";
                dbDapper.Update(StockInfoItem);
                
              }
            }

            //Log(string.Format("{0}進度({1}/{2})=>[{3}]{4} 狀態：{5}"
            //  , sType, idx, nodes.Count, sCode, sName, sStatus));
            

            idx++;

          }

          ssql = "update stockinfo set status = 'N' where updstatus = 'N' ";
          dbDapper.Execute(ssql);

        }

      }
      catch (Exception)
      {

        
      }


      logger.Info("END DoStockTrans()");
      //MessageBox.Show("Finish");



      // 發送 email。這裡只固定輸出一筆文字訊息至 log 檔案，方便觀察測試。
      // 每發送一封 email 就檢查一次 IntervalTask.Current.SuttingDown 以配合外部的終止事件。
      //Log("Finish DoStockTrans() at " + DateTime.Now.ToString());

      //Thread.Sleep(2000);

    }

    public void Execute(IJobExecutionContext context)
    {
      try
      {
        DoStockTrans();
      }
      catch (Exception ex)
      {
        logger.Log(NLog.LogLevel.Error, ex.Message);
        
      }
    }
  }


  /// <summary>
  ///
  /// </summary>
  //一次只執行一個體
  [DisallowConcurrentExecutionAttribute]  
  public class StockThreeTradeTask : BaseJob, IJob
  {
    
    public void DoStockThreeTrade()
    {

      logger.Info("START DoStockThreeTrade()");

      //全部
      //string sUrl = "http://www.tse.com.tw/fund/T86?response=csv&date={0}&selectType=ALL";
      //全部(不含權證、牛熊證、可展延牛熊證)
      string sUrl = "http://www.tse.com.tw/fund/T86?response=csv&date={0}&selectType=ALLBUT0999";
      string sDate = DateTime.Now.ToString("yyyyMMdd");
      
      HttpWebRequest req = (HttpWebRequest)WebRequest.Create(string.Format(sUrl, sDate));
      HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

      using (StreamReader SR = new StreamReader(resp.GetResponseStream(), System.Text.Encoding.GetEncoding(950)))
      {
        string Line;
        while ((Line = SR.ReadLine()) != null)
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
            Stockthreetrade st = dbDapper.QuerySingleOrDefault<Stockthreetrade>(string.Format(ssql,sDate, aCol[0]));

            if (st == null)
            {
              st = new Stockthreetrade();
              st.stockcode = aCol[0];
              st.date = sDate;
              st.type = "0";
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
              st.type = "0";
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

      logger.Info("END DoStockThreeTrade()");
    }

    

    public void Execute(IJobExecutionContext context)
    {
      try
      {
        DoStockThreeTrade();
      }
      catch (Exception ex)
      {
        logger.Log(NLog.LogLevel.Error, ex.Message);
      }
    }
  }


}