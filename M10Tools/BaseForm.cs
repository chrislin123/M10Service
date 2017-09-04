using System.Windows.Forms;
using System.Configuration;
using M10.lib;
using NLog;

namespace M10Tools
{
  public class BaseForm : Form
  { 
    private string _ConnectionString;
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
