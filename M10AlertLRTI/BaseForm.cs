
using System.Windows.Forms;
using System.Configuration;
using M10.lib;
using NLog;

namespace M10AlertLRTI
{
  public class BaseForm : Form
  {
    public string ssql = string.Empty;
    private string _ConnectionString;
    private DALDapper _dbDapper;
    public Logger logger;

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

    public BaseForm()
    {

    }
    
    public void InitForm()
    { 
      _dbDapper = new DALDapper(ConnectionString);
      logger = NLog.LogManager.GetCurrentClassLogger();
    }

  }
}
