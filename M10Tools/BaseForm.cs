using System.Windows.Forms;
using System.Configuration;
using M10.lib;
using NLog;
using System.Net;

namespace M10Tools
{
    public class BaseForm : Form
    {
        private string _ConnectionString;
        private string _ProviderName;
        private ConnectionStringSettings _ConnectionStringSettings;
        private DALDapper _dbDapper;
        public string ssql = string.Empty;
        public Logger logger;
        private StockHelper _stockhelper;

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

        public string ProviderName
        {
            get
            {
                if (string.IsNullOrEmpty(_ProviderName))
                {
                    _ProviderName = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["DBDefault"]].ProviderName;
                }

                return _ProviderName;
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


        

        public StockHelper Stockhelper
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

        public BaseForm()
        {

        }

        public void InitForm()
        {
            //_dbDapper = new DALDapper(ConnectionString);
            logger = NLog.LogManager.GetCurrentClassLogger();
        }

    }
}
