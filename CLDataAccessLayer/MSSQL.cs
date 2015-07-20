using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

using System.Text;
using System.Text.RegularExpressions;


namespace CL.Data
{
    /// <summary>
    /// 2007.10.11 新建置-MSSQL資料庫使用類別
    ///	2010 9.14 重構類別	
    /// </summary>
    /// 
    internal class MSSQL : IQuery
    {
        // Fields
        /// <summary>
        /// 連線物件
        /// </summary>
        private SqlConnection Connection;
        private string _fieldName;
        private ConnectionString constring;
        private DbDataAdapter adapter;
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="ConnectString"></param>
        public MSSQL(ConnectionString ConnectString)
        {
            constring = ConnectString;
            Connection = new SqlConnection(ConnectString.ConString);
            adapter = new SqlDataAdapter();
        }


        #region 執行SQL 語法By交易機制
        /// <summary>
        ///  執行SQL 語法By交易機制
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public void ExecuteTransactionSql(List<DbCommand> cmd)
        {
            SqlConnection connection = Connection;
            connection.Open();
            SqlTransaction ST = connection.BeginTransaction();
            foreach (DbCommand strSql in cmd)
            {
                SqlCommand command = strSql as SqlCommand;

                command.Connection = connection;
                command.Transaction = ST;
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException exception)
                {
                    ST.Rollback();
                    throw exception;
                }

            }
            ST.Commit();
            connection.Close();

        }
        #endregion

        /// <summary>
        /// 從Db取得資料
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private DataSet ReturnSet(SqlCommand cmd)
        {

            SqlConnection selectConnection = Connection;
            cmd.Connection = selectConnection;
            try
            {
                adapter.SelectCommand = cmd;
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                adapter.Dispose();
                return dataSet;
            }
            catch (SqlException exception)
            {
                throw exception;
            }
        }

        private DataTable ReturnTable(SqlCommand cmd)
        {

            SqlConnection selectConnection = Connection;
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
            catch (SqlException exception)
            {

                throw exception;

            }

        }



        #region IQuery 成員

        /// <summary>
        /// 執行SQL語法
        /// </summary>
        public object ExecuteSql(DbCommand comSQL)
        {
            SqlCommand strSQL = comSQL as SqlCommand;
            SqlConnection connection = Connection;
            connection.Open();
            SqlTransaction ST = connection.BeginTransaction();
            strSQL.Connection = connection;
            strSQL.Transaction = ST;
            try
            {
                int rs=strSQL.ExecuteNonQuery();
                strSQL.Transaction.Commit();
                return rs;
            }
            catch (SqlException exception)
            {
                strSQL.Transaction.Rollback();
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
        public DataRow DataRow(DbCommand cmd)
        {

            DataTable table = DataTable(cmd as SqlCommand);
            if (table.Rows.Count > 0)
            {
                return table.Rows[0];
            }
            return null;
        }
        /// <summary>
        /// 回傳結果集至DataTable
        /// </summary>
        /// <param name="comSQL"></param>
        /// <returns></returns>
        public DataTable DataTable(DbCommand comSQL)
        {
            return ReturnTable(comSQL as SqlCommand);
        }

        /// <summary>
        ///  回傳結果集至DataSet
        /// </summary>
        /// <param name="comSQL"></param>
        /// <returns></returns>
        public object Value(DbCommand comSQL)
        {
            SqlConnection connection = Connection;
            SqlCommand strSQL = comSQL as SqlCommand;
            strSQL.Connection = connection;
            try
            {
                connection.Open();
                return (strSQL.ExecuteScalar());

            }
            catch (SqlException exception)
            {

                throw exception;
            }
            finally
            {
                connection.Close();
            }

        }
        /// <summary>
        /// Command轉成TSQL
        /// </summary>
        /// <param name="cmd">欲執行Command</param>
        /// <returns></returns>
        public DataSet DataSet(DbCommand cmd)
        {
            return ReturnSet(cmd as SqlCommand);
        }
        /// <summary>
        /// Command轉成TSQL
        /// </summary>
        /// <param name="cmd">欲執行Command</param>
        /// <returns></returns>
        public string ConvertTSQL(DbCommand cmd)
        {
            try
            {
                if (!string.IsNullOrEmpty(cmd.CommandText))//無內容則跳出
                {
                    string sb = (cmd.CommandText);
                    foreach (DbParameter dp in cmd.Parameters)
                    {
                        string pattern;
                        if (ConvertUtility.IsString(dp.DbType))//是否為文字
                        {
                            pattern = dp.ParameterName + @"\W";
                            Match mc = Regex.Match(sb, pattern, RegexOptions.Multiline);
                            if (!mc.Success)
                                break;
                            sb = Regex.Replace(sb.ToString(),
                                pattern,
                                 string.Format("'{0}'{1}",
                                                             dp.Value, mc.Value[mc.Length - 1]),
                                                             RegexOptions.None);

                        }
                        else
                        {
                            pattern = dp.ParameterName + @"\W";
                            Match mc = Regex.Match(sb, pattern, RegexOptions.Multiline);
                            if (!mc.Success)
                                break;
                            sb = Regex.Replace(sb.ToString(),
                                pattern,
                                 string.Format("{0}{1}",
                                                            dp.Value, mc.Value[mc.Length - 1]), RegexOptions.Multiline);
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
