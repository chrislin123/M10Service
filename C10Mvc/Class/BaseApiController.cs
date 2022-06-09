using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Configuration;
using M10.lib;
using NLog;

namespace C10Mvc.Class
{
    public class BaseApiController : ApiController
    {
        public string ssql = string.Empty;
        private DALDapper _dbDapper;
        private string _ConnectionString;
        private StockUtil _StockUtil;
        private StockHelper _stockhelper;
        private NLog.Logger _logger;
        private ConnectionStringSettings _ConnectionStringSettings;


        //public DALDapper dbDapper
        //{
        //    get
        //    {
        //        if (_dbDapper == null)
        //        {
        //            _dbDapper = new DALDapper(ConnectionString);
        //        }

        //        return _dbDapper;
        //    }
        //}
        public DALDapper dbDapper
        {
            get
            {
                if (_dbDapper == null)
                {
                    //_dbDapper = new DALDapper(ConnectionString);
                    _dbDapper = new DALDapper(ConnectionStringSettings);
                }

                return _dbDapper;
            }
        }

        public ConnectionStringSettings ConnectionStringSettings
        {
            get
            {
                if (_ConnectionStringSettings == null)
                {
                    _ConnectionStringSettings = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DBDefault"]];
                }

                return _ConnectionStringSettings;
            }
        }

        public string ConnectionString
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

        public StockUtil oStockUtil
        {
            get
            {
                if (_StockUtil == null)
                {
                    _StockUtil = new StockUtil();
                }

                return _StockUtil;
            }
        }

        public StockHelper stockhelper
        {
            get
            {
                if (_stockhelper == null)
                {
                    _stockhelper = new StockHelper();
                }
                return _stockhelper;
            }

        }

        protected Logger logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = NLog.LogManager.GetCurrentClassLogger();
                }

                return _logger;
            }
        }

    }
}