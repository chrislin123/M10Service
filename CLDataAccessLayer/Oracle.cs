using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OracleClient;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace CL.Data
{
    /// <summary>
    /// 2011.6.15 新建置-Oracle資料庫使用類別	
    /// </summary>
    /// 
    internal class Oracle : IQuery
    {
        // Fields
        /// <summary>
        /// 連線物件
        /// </summary>
        private OracleConnection Connection;
        private string _fieldName;
        private ConnectionString cs;
        private DbDataAdapter adapter;
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="ConnectString"></param>
        public Oracle(ConnectionString ConnectString)
        {
            cs = ConnectString;
            Connection = new OracleConnection(ConnectString.ConString);

        }
        /// <summary>
        /// 從Db取得資料
        /// </summary>
        private DataSet ReturnData(OracleCommand cmd)
        {
            
           OracleConnection selectConnection = Connection;
                cmd.Connection = selectConnection;
                try
                {
                    adapter.SelectCommand = cmd;
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    return dataSet;
                }
                catch (OracleException exception)
                {

                        throw exception;

                }
                finally
                {
                    selectConnection.Close();
                }
            
        }

        private DataTable ReturnTable(OracleCommand cmd)
        {
            OracleConnection selectConnection = Connection;
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
                    return dt;
                }
                catch (OracleException exception)
                {
                    throw exception;
                }
            
        }

        #region IQuery Members

        /// <summary>
        /// 執行SQL語法
        /// </summary>
        public object ExecuteSql(System.Data.Common.DbCommand cmd)
        {
            OracleCommand strSQL = cmd as OracleCommand;
            OracleConnection connection = Connection;
            connection.Open();
            OracleTransaction ST = connection.BeginTransaction();
            strSQL.Connection = connection;
            strSQL.Transaction = ST;
            try
            {
                int rs= strSQL.ExecuteNonQuery();
                ST.Commit();
                return rs;
            }
            catch (OracleException exception)
            {
                ST.Rollback();
                throw exception;
            }
            finally
            {
                connection.Close();
            }
            
        }

        /// <summary>
        /// 回傳結果集至DataRow
        /// </summary>
        public System.Data.DataRow DataRow(System.Data.Common.DbCommand cmd)
        {
            DataTable table = DataTable(cmd as OracleCommand);
            if (table.Rows.Count > 0)
            {
                return table.Rows[0];
            }
            return null;
        }

        /// <summary>
        /// 回傳結果集至DataTable
        /// </summary>
        public System.Data.DataTable DataTable(System.Data.Common.DbCommand cmd)
        {
            return ReturnTable(cmd as OracleCommand);
        }

        /// <summary>
        /// 回傳單一結果資料為物件型態
        /// </summary>
        public object Value(System.Data.Common.DbCommand cmd)
        {
            OracleConnection connection = Connection;
            OracleCommand strSQL = cmd as OracleCommand;
            strSQL.Connection = connection;
            try
            {
                connection.Open();
                return  (strSQL.ExecuteScalar());

            }
            catch (OracleException exception)
            {
                throw exception;
                
            }
            finally
            {
                connection.Close();
            }
          
        }

        /// <summary>
        /// 回傳結果集至DataSet
        /// </summary>
        public System.Data.DataSet DataSet(System.Data.Common.DbCommand cmd)
        {
            return ReturnData(cmd as OracleCommand);
        }

        /// <summary>
        /// Command轉成TSQL
        /// </summary>
        public string ConvertTSQL(System.Data.Common.DbCommand cmd)
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
        #endregion

        #region IDisposable 成員

        public void Dispose()
        {
            Connection.Dispose();
            adapter.Dispose();
        }

        #endregion
 
    }
}
