using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;


namespace CL.Data
{
    /// <summary>
    /// 2008.1.21 新建置-MySQL資料庫使用類別
    /// 2010 9.14 重構類別
    /// 2011.08.04建置主線-1
    /// </summary>
   internal  class Mysql:IQuery
    {
        // Field
        /// <summary>
        /// 連線物件
        /// </summary>
        private MySqlConnection Connection;
        private string _fieldName;
        private ConnectionString constring;
        private DbDataAdapter adapter;
        /// <summary>
 /// 建構子
 /// </summary>
 /// <param name="ConnectString"></param>
        public Mysql(ConnectionString ConnectString)
        {
            constring = ConnectString;
            Connection = new MySqlConnection(ConnectString.ConString);
            adapter =new MySqlDataAdapter();
        }

        /// <summary>
        /// 從Db取得資料
        /// </summary>
        private DataSet ReturnData(MySqlCommand cmd)
        {
        
            MySqlConnection selectConnection = Connection;
            cmd.Connection = selectConnection;
            DataSet dataSet;
            try
            {
                selectConnection.Open();
                adapter.SelectCommand = cmd;
                dataSet = new DataSet();
                adapter.Fill(dataSet);

            }
            catch (MySqlException exception)
            {
                    throw exception;

            }
            finally
            {
                selectConnection.Close();
            }
            return dataSet;
        }
        private DataTable ReturnTable(MySqlCommand cmd)
        {

            using (MySqlConnection selectConnection = Connection)
            {
                cmd.Connection = selectConnection;
                try
                {
                    adapter.SelectCommand=cmd;
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
                catch (MySqlException exception)
                {

                    throw exception;

                }
            }
        }
        /// <summary>
        /// 回傳結果集至DataSet
        /// </summary>
        public DataSet DataSet(DbCommand cmd)
        {
            return ReturnData(cmd as MySqlCommand);
        }
         #region IQuery 成員

        /// <summary>
        /// 執行SQL語法
        /// </summary>
        public object ExecuteSql(DbCommand cmd)
         {
             MySqlConnection connection = Connection;
             MySqlCommand command = cmd as MySqlCommand;

             try
             {
                 connection.Open();
                return  command.ExecuteNonQuery();
             }
             catch (MySqlException exception)
             {
                  
                  throw exception;
             }
             finally
             {
                 command.Dispose();
                 connection.Close();
             }
            
         }

         /// <summary>
         /// 回傳結果集至DataRow
         /// </summary>
        public DataRow DataRow(DbCommand cmd)
         {
             DataTable table = DataTable(cmd as MySqlCommand);
             if (table.Rows.Count > 0)
             {
                 return table.Rows[0];
             }
             return null;
         }
         /// <summary>
         /// 回傳結果集至DataTable
         /// </summary>
         /// <param name="comSQL">欲執行Dbcommand</param>
         /// <returns>結果集</returns>
         public DataTable DataTable(DbCommand comSQL)
         {
             return ReturnTable(comSQL as MySqlCommand);
         }
         /// <summary>
         /// 回傳數值
         /// </summary>
         /// <param name="cmd">欲執行Dbcommand</param>
         /// <returns>結果集</returns>
         public object Value(DbCommand cmd)
         {
             Connection.ConnectionString = constring.ConString;
             MySqlConnection connection = Connection;
             MySqlCommand strSQL = cmd as MySqlCommand;
             strSQL.Connection = connection;
             try
             {
                 
                 connection.Open();
                 return strSQL.ExecuteScalar();

             }
             catch (MySqlException exception)
             {
                     throw exception;
             }
             finally
             {
                 strSQL.Dispose();
                 connection.Close();

             }
             
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
                             Match mc = Regex.Match(sb, pattern);
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
                             Match mc = Regex.Match(sb, pattern);
                             if (!mc.Success)
                                 break;
                             sb = Regex.Replace(sb.ToString(),
                                 pattern,
                                  string.Format("{0}{1}",
                                                             dp.Value, mc.Value[mc.Length - 1]),
                                                             RegexOptions.None);
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
