using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Text;
using System.Text.RegularExpressions;

namespace CL.Data
{

    /// <summary>
    /// 2008.3.7 新建置-Access or Excel資料庫使用類別
    /// 2010 9.14 重構類別
    ///		
    /// </summary>
    /// 

    internal class OleDb : IQuery
    {
        // Fields
        /// <summary>
        /// 連線物件
        /// </summary>
        private OleDbConnection Connection;
        private DbDataAdapter adapter;
        private string _fieldName;
        private ConnectionString constring;
        /// <summary>
        /// Excel 或Access 檔存放位置(虛擬路徑)
        /// </summary>
        /// <param name="ConnectString">連結字串</param>
        public OleDb(ConnectionString ConnectString)
        {
            constring = ConnectString;
            string connstr =
                string.Format("{0}{1}",
                                         "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=",
                                         System.Web.HttpContext.Current.Server.MapPath(ConnectString.ConString));
            Connection = new OleDbConnection(connstr);
            adapter = new OleDbDataAdapter();

        }

        /// <summary>
        /// 回傳結果集至DataRow
        /// </summary>
        /// <returns>結果集</returns>
        public DataRow DataRow(DbCommand sql)
        {

            DataTable table = DataTable(sql as OleDbCommand);
            if (table.Rows.Count > 0)
            {
                return table.Rows[0];
            }
            return null;


        }

        /// <summary>
        /// 回傳結果集至DataTable
        /// </summary>
        /// <param name="cmd">欲執行Dbcommand</param>
        /// <returns>結果集</returns>
        public DataTable DataTable(DbCommand cmd)
        {
            return ReturnTable(cmd as OleDbCommand);

        }
        /// <summary>
        /// 回傳單一數值
        /// </summary>
        /// <param name="cmd">欲執行Dbcommand</param>
        /// <returns>結果集</returns>
        public object Value(DbCommand cmd)
        {
            OleDbConnection connection = Connection;
            object  msg ;
            OleDbCommand strSQL = cmd as OleDbCommand;
            strSQL.Connection = connection;
            try
            {

                connection.Open();
                msg = strSQL.ExecuteScalar();

            }
            catch (OleDbException exception)
            {
                throw exception;
            }
            finally
            {
                strSQL.Dispose();
                connection.Close();

            }
            return msg;
        }
        /// <summary>
        /// 回傳結果集至DataSet
        /// </summary>
        /// <param name="cmd">欲執行Dbcommand</param>
        /// <returns>結果集</returns>
        public DataSet DataSet(DbCommand cmd)
        {
            return ReturnData(cmd as OleDbCommand);
        }

        /// <summary>
        /// Command轉成TSQL
        /// </summary>
        /// <param name="cmd">欲執行Command</param>
        /// <returns>結果集</returns>
        public string ConvertTSQL(DbCommand cmd)
        {
            try
            {
                if (!string.IsNullOrEmpty(cmd.CommandText))//無內容則跳出
                {

                    StringBuilder sb = new StringBuilder(cmd.CommandText);
                    foreach (DbParameter dp in cmd.Parameters)
                    {
                        string pattern;
                        if (ConvertUtility.IsString(dp.DbType))//是否為文字
                        {
                            pattern = dp.ParameterName + "\\W";
                            sb = new StringBuilder(Regex.Replace(sb.ToString(),
                                pattern.Remove(pattern.Length - 1),
                                 string.Format("'{0}'", dp.Value), RegexOptions.IgnoreCase));

                        }
                        else
                        {
                            pattern = dp.ParameterName + "\\W";
                            sb = new StringBuilder(Regex.Replace(sb.ToString(),
                                pattern.Remove(pattern.Length - 1),
                                 string.Format("{0}", dp.Value), RegexOptions.IgnoreCase));
                        }

                    }

                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return string.Empty;
        }


        /// <summary>
        /// 執行SQL 語法
        /// </summary>
        public object ExecuteSql(DbCommand cmd)
        {
            OleDbConnection connection = Connection;
            cmd.Connection = connection;
            try
            {
                connection.Open();
               return cmd.ExecuteNonQuery();

            }
            catch (OleDbException exception)
            {
                throw exception;
            }
            finally
            {
                connection.Close();
                cmd.Dispose();


            }
          
        }

        /// <summary>
        /// 從Db取得資料
        /// </summary>
        private DataSet ReturnData(OleDbCommand cmd)
        {
            DataSet dataSet;
            OleDbConnection selectConnection = Connection;
            cmd.Connection = selectConnection;
            try
            {
                selectConnection.Open();
                adapter.SelectCommand = cmd;
                dataSet = new DataSet();
                adapter.Fill(dataSet);

            }
            catch (OleDbException exception)
            {
                throw exception;
            }
            finally
            {
                selectConnection.Close();
            }
            return dataSet;
        }
        private DataTable ReturnTable(OleDbCommand cmd)
        {
            using (OleDbConnection selectConnection = Connection)
            {
                cmd.Connection = selectConnection;
                try
                {

                    adapter.SelectCommand = cmd;
                    DataTable dt = new DataTable();
                    if (!string.IsNullOrEmpty(_fieldName))
                    {
                        DataColumn dc = new DataColumn(_fieldName);
                        dc.AutoIncrement = true;
                        dc.AutoIncrementSeed = 1;
                        dt.Columns.Add(dc);
                    }
                    adapter.Fill(dt);
                    adapter.Dispose();
                    return dt;
                }
                catch (OleDbException exception)
                {

                    throw exception;

                }
            }
        }
        /// <summary>
        /// 加入自動編號欄位
        /// </summary>
        /// <param name="FieldName">欄位名稱</param>
        public void AddIndex(string FieldName)
        {
            _fieldName = FieldName;
        }
        public DbDataAdapter DataAdapter
        {
            get
            {
                return adapter;
            }
            set
            {
                adapter = value;
            }
        }

        #region IDisposable 成員

        public void Dispose()
        {
            Connection.Dispose();
            adapter.Dispose();
        }

        #endregion

    }
}
