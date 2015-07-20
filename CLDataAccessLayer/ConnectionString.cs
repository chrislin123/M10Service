using System.Configuration;

namespace CL.Data
{
    /// <summary>
    /// 2010.9.14 新建置-資料庫連接字串使用類別
    /// 2011.6.13 改寫
    /// </summary>
    public class  ConnectionString
    {
        private ConnectionStringSettings objConnection ;
        /// <summary>
        /// 資料庫連線名稱ByString
        /// </summary>
        /// <param name="Connection">連結字串關鍵字</param>
        public ConnectionString(string Connection) 
        {
            objConnection = ConfigurationManager.ConnectionStrings[Connection];
        }
        /// <summary>
        /// 資料庫連線名稱By索引
        /// </summary>
        /// <param name="ConnectionIndex">連結字串索引</param>
        public ConnectionString(int ConnectionIndex)
        {
            objConnection = ConfigurationManager.ConnectionStrings[ConnectionIndex];
        }
        /// <summary>
        /// 取得資料庫連線元件
        /// </summary>
        /// <returns>資料庫連線元件</returns>
        public ConnectionStringSettings GetConnection()
        {
            return objConnection;
        }
       /// <summary>
        /// 取得資料庫連結字串
       /// </summary>
       /// <returns></returns>
        public string ConString
        {
            get
            {
                return objConnection.ConnectionString;
            }
        }
        /// <summary>
        /// 取得資料庫連結字串名稱
        /// </summary>
        public string ProviderName
        {
            get
            {
                
                return objConnection.ProviderName;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Debug
        {
            get
            {
                string str = ConfigurationManager.AppSettings["debug"];
                return (!string.IsNullOrEmpty(str) && (str == bool.TrueString));
            }
        }

    }
}
