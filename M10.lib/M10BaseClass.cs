using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using NLog;

namespace M10.lib
{
    public class M10BaseClass
    {
        public string ssql = string.Empty;
        private DALDapper _dbDapper;
        private string _ConnectionString;
        public Logger _logger;
        private StockHelper _stockhelper;

        

        public DALDapper dbDapper
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

        public Logger logger
        {
            get
            {
                _logger = NLog.LogManager.GetCurrentClassLogger();

                return _logger;
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
    }
}
