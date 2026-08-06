using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;


namespace M10.lib
{
    public class DALDapper
    {
        string _ConnStr = string.Empty;
        //public ConnectionString objCon;
        //預設使用MsSQL
        public string _ProviderName = "System.Data.SqlClient";
        public ConnectionStringSettings _objConColl;

        public dynamic SqlClient
        {
            get
            {
                if (_ProviderName == "System.Data.SqlClient")
                {
                    return new System.Data.SqlClient.SqlConnection(ConnStr);
                }
                else if (_ProviderName == "System.Data.OleDb")
                {
                    return new System.Data.SqlClient.SqlConnection(ConnStr);
                }
                else if (_ProviderName == "System.Data.OracleClient")
                {
                    return new System.Data.SqlClient.SqlConnection(ConnStr);
                }
                else if (_ProviderName == "MySql.Data.MySqlClient")
                {
                    return new MySql.Data.MySqlClient.MySqlConnection(ConnStr);
                }
                else
                {
                    //預設使用sSQL
                    return new System.Data.SqlClient.SqlConnection(ConnStr);
                }
            }
        }

        public DALDapper(string pConnStr)
        {
            _ConnStr = pConnStr;
        }

        public DALDapper(ConnectionStringSettings objConColl)
        {
            _objConColl = objConColl;
            _ConnStr = objConColl.ConnectionString;
            _ProviderName = objConColl.ProviderName;
        }

        public string ConnStr
        {
            get
            {
                return _ConnStr;
            }
        }

        public List<dynamic> Query(string sql)
        {
            //using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
            //{
            //    lResult = cn.Query(sql).ToList();
            //}

            List<dynamic> lResult = new List<dynamic>();

            if (_ProviderName == "System.Data.SqlClient")
            {
                using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                {
                    lResult = cn.Query(sql).ToList();
                }
            }
            else if (_ProviderName == "MySql.Data.MySqlClient")
            {
                using (MySql.Data.MySqlClient.MySqlConnection cn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr))
                {
                    lResult = cn.Query(sql).ToList();
                }
            }
            else
            {
                using (System.Data.SqlClient.SqlConnection cn = SqlClient)
                {
                    lResult = cn.Query(sql).ToList();
                }
            }

            return lResult;
        }

        public List<dynamic> Query(string sql, object param)
        {
            //using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
            //{
            //    return cn.Query(sql, param).ToList();
            //}

            List<dynamic> lResult = new List<dynamic>();

            if (_ProviderName == "System.Data.SqlClient")
            {
                using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                {
                    lResult = cn.Query(sql, param).ToList();
                }
            }
            else if (_ProviderName == "MySql.Data.MySqlClient")
            {
                using (MySql.Data.MySqlClient.MySqlConnection cn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr))
                {
                    lResult = cn.Query(sql, param).ToList();
                }
            }
            else
            {
                using (System.Data.SqlClient.SqlConnection cn = SqlClient)
                {
                    lResult = cn.Query(sql, param).ToList();
                }
            }

            return lResult;
        }

        public List<dynamic> Query(string sql, object param, int? commandTimeout = null)
        {
            List<dynamic> lResult = new List<dynamic>();

            if (_ProviderName == "System.Data.SqlClient")
            {
                using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                {
                    // 將 commandTimeout 往下傳遞給底層的原生 Dapper
                    lResult = cn.Query(sql, param, commandTimeout: commandTimeout).ToList();
                }
            }
            else if (_ProviderName == "MySql.Data.MySqlClient")
            {
                using (MySql.Data.MySqlClient.MySqlConnection cn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr))
                {
                    lResult = cn.Query(sql, param, commandTimeout: commandTimeout).ToList();
                }
            }
            else
            {
                using (System.Data.SqlClient.SqlConnection cn = SqlClient)
                {
                    lResult = cn.Query(sql, param, commandTimeout: commandTimeout).ToList();
                }
            }

            return lResult;
        }

        public List<T> Query<T>(string sql) where T : class
        {
            List<T> lResult = new List<T>();

            try
            {
                //using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                //{
                //    return cn.Query<T>(sql).ToList<T>();
                //}



                if (_ProviderName == "System.Data.SqlClient")
                {
                    using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                    {
                        lResult = cn.Query<T>(sql).ToList<T>();
                    }
                }
                else if (_ProviderName == "MySql.Data.MySqlClient")
                {
                    using (MySql.Data.MySqlClient.MySqlConnection cn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr))
                    {
                        lResult = cn.Query<T>(sql).ToList<T>();
                    }
                }
                else
                {
                    using (System.Data.SqlClient.SqlConnection cn = SqlClient)
                    {
                        lResult = cn.Query<T>(sql).ToList<T>();
                    }
                }


            }
            catch (Exception)
            {
                return null;
            }

            return lResult;
        }

        public List<T> Query<T>(string sql, object param) where T : class
        {
            List<T> lResult = new List<T>();

            try
            {
                //using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                //{
                //    return cn.Query<T>(sql, param).ToList<T>();
                //}

                if (_ProviderName == "System.Data.SqlClient")
                {
                    using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                    {
                        lResult = cn.Query<T>(sql, param).ToList<T>();
                    }
                }
                else if (_ProviderName == "MySql.Data.MySqlClient")
                {
                    using (MySql.Data.MySqlClient.MySqlConnection cn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr))
                    {
                        lResult = cn.Query<T>(sql, param).ToList<T>();
                    }
                }
                else
                {
                    using (System.Data.SqlClient.SqlConnection cn = SqlClient)
                    {
                        lResult = cn.Query<T>(sql, param).ToList<T>();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return lResult;
        }



        public int QueryTotalCount(string sql)
        {
            int i = 0;

            try
            {
                //using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                //{
                //    i = cn.Query(sql).Count();
                //}

                if (_ProviderName == "System.Data.SqlClient")
                {
                    using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                    {
                        i = cn.Query(sql).Count();
                    }
                }
                else if (_ProviderName == "MySql.Data.MySqlClient")
                {
                    using (MySql.Data.MySqlClient.MySqlConnection cn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr))
                    {
                        i = cn.Query(sql).Count();
                    }
                }
                else
                {
                    using (System.Data.SqlClient.SqlConnection cn = SqlClient)
                    {
                        i = cn.Query(sql).Count();
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return i;
        }

        public int QueryTotalCount(string sql, object param)
        {
            int i = 0;

            try
            {
                //using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                //{
                //    i = cn.Query(sql, param).Count();
                //}

                if (_ProviderName == "System.Data.SqlClient")
                {
                    using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                    {
                        i = cn.Query(sql, param).Count();
                    }
                }
                else if (_ProviderName == "MySql.Data.MySqlClient")
                {
                    using (MySql.Data.MySqlClient.MySqlConnection cn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr))
                    {
                        i = cn.Query(sql, param).Count();
                    }
                }
                else
                {
                    using (System.Data.SqlClient.SqlConnection cn = SqlClient)
                    {
                        i = cn.Query(sql, param).Count();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return i;
        }

        /// <summary>
        /// 執行 SQL 指令並回傳受影響筆數 (支援參數與逾時設定)
        /// </summary>
        public int Execute(string sql, object param, int? commandTimeout = null)
        {
            int i = 0;
            try
            {
                // 利用類別內定義好的 SqlClient 屬性自動取得正確的 Connection
                using (System.Data.IDbConnection cn = SqlClient)
                {
                    // 呼叫底層 Dapper 並傳遞 commandTimeout
                    i = cn.Execute(sql, param, commandTimeout: commandTimeout);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return i;
        }

        /// <summary>
        /// 執行 SQL 指令 (簡易版)
        /// </summary>
        public int Execute(string sql, int? commandTimeout = null)
        {
            // 直接導向帶有 param 的版本
            return Execute(sql, null, commandTimeout);
        }

        /// <summary>
        /// 執行多筆 SQL 指令 (含交易)
        /// </summary>
        public int Execute(List<SqlObject> SqlList, int? commandTimeout = null)
        {
            int i = 0;
            try
            {
                using (System.Data.IDbConnection cn = SqlClient)
                {
                    using (var transactionScope = new TransactionScope())
                    {
                        foreach (SqlObject d in SqlList)
                        {
                            i += cn.Execute(d.sql, d.param, commandTimeout: commandTimeout);
                        }
                        transactionScope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return i;
        }

        public bool Update<T>(T entityToUpdate) where T : class
        {
            bool bResult = false;

            try
            {
                //using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                //{
                //    bResult = cn.Update<T>(entityToUpdate);
                //}

                if (_ProviderName == "System.Data.SqlClient")
                {
                    using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                    {
                        bResult = cn.Update<T>(entityToUpdate);
                    }
                }
                else if (_ProviderName == "MySql.Data.MySqlClient")
                {
                    using (MySql.Data.MySqlClient.MySqlConnection cn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr))
                    {
                        bResult = cn.Update<T>(entityToUpdate);
                    }
                }
                else
                {
                    using (System.Data.SqlClient.SqlConnection cn = SqlClient)
                    {
                        bResult = cn.Update<T>(entityToUpdate);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bResult;
        }

        public bool Delete<T>(T entityToDelete) where T : class
        {
            bool bResult = false;

            try
            {
                //using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                //{   
                //    bResult = cn.Delete<T>(entityToDelete);
                //}

                if (_ProviderName == "System.Data.SqlClient")
                {
                    using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                    {
                        bResult = cn.Delete<T>(entityToDelete);
                    }
                }
                else if (_ProviderName == "MySql.Data.MySqlClient")
                {
                    using (MySql.Data.MySqlClient.MySqlConnection cn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr))
                    {
                        bResult = cn.Delete<T>(entityToDelete);
                    }
                }
                else
                {
                    using (System.Data.SqlClient.SqlConnection cn = SqlClient)
                    {
                        bResult = cn.Delete<T>(entityToDelete);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bResult;
        }

        public T QuerySingleOrDefault<T>(string sql) where T : class
        {
            object oo = new object();

            try
            {
                //using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                //{
                //    oo = cn.QuerySingleOrDefault<T>(sql);
                //}

                if (_ProviderName == "System.Data.SqlClient")
                {
                    using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                    {
                        oo = cn.QuerySingleOrDefault<T>(sql);
                    }
                }
                else if (_ProviderName == "MySql.Data.MySqlClient")
                {
                    using (MySql.Data.MySqlClient.MySqlConnection cn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr))
                    {
                        oo = cn.QuerySingleOrDefault<T>(sql);
                    }
                }
                else
                {
                    using (System.Data.SqlClient.SqlConnection cn = SqlClient)
                    {
                        oo = cn.QuerySingleOrDefault<T>(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return oo as T;
        }

        public T QuerySingleOrDefault<T>(string sql, object param) where T : class
        {
            object oo = null;

            try
            {
                //using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                //{
                //    oo = cn.QuerySingleOrDefault<T>(sql, param);
                //}

                if (_ProviderName == "System.Data.SqlClient")
                {
                    using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                    {
                        oo = cn.QuerySingleOrDefault<T>(sql, param);
                    }
                }
                else if (_ProviderName == "MySql.Data.MySqlClient")
                {
                    using (MySql.Data.MySqlClient.MySqlConnection cn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr))
                    {
                        oo = cn.QuerySingleOrDefault<T>(sql, param);
                    }
                }
                else
                {
                    using (System.Data.SqlClient.SqlConnection cn = SqlClient)
                    {
                        oo = cn.QuerySingleOrDefault<T>(sql, param);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return oo as T;
        }

        public long Insert<T>(T entityToInsert) where T : class
        {
            long lResult = 0;
            try
            {
                //using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                //{
                //    lResult = cn.Insert<T>(entityToInsert);
                //}

                if (_ProviderName == "System.Data.SqlClient")
                {
                    using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConnStr))
                    {
                        lResult = cn.Insert<T>(entityToInsert);
                    }
                }
                else if (_ProviderName == "MySql.Data.MySqlClient")
                {
                    using (MySql.Data.MySqlClient.MySqlConnection cn = new MySql.Data.MySqlClient.MySqlConnection(ConnStr))
                    {
                        lResult = cn.Insert<T>(entityToInsert);
                    }
                }
                else
                {
                    using (System.Data.SqlClient.SqlConnection cn = SqlClient)
                    {
                        lResult = cn.Insert<T>(entityToInsert);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lResult;
        }

        /// <summary>
        /// ExecuteScalar 執行一個SQL命令返回結果集的第一列的第一行。
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScale(string sql, object param, int? commandTimeout = null)
        {
            object oo = null;
            try
            {
                using (System.Data.IDbConnection cn = SqlClient)
                {
                    // 這裡要把參數傳給底層的 Dapper ExecuteScalar
                    oo = cn.ExecuteScalar(sql, param, commandTimeout: commandTimeout);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return oo;
        }

        /// <summary>
        /// ExecuteScalar 執行一個SQL命令返回結果集的第一列的第一行。
        /// </summary>
        public object ExecuteScale(string sql, int? commandTimeout = null)
        {
            // 直接呼叫上面那個帶 param 的版本，傳入 null 即可
            return ExecuteScale(sql, null, commandTimeout);
        }


        public DynamicParameters GetNewDynamicParameters()
        {
            DynamicParameters dp = new DynamicParameters();

            return dp;
        }

        /// <summary>
        /// 執行 SQL 指令並回傳結果集第一列的第一行 (泛型強型別版本)
        /// </summary>
        public T ExecuteScalar<T>(string sql, object param, int? commandTimeout = null)
        {
            try
            {
                using (System.Data.IDbConnection cn = SqlClient)
                {
                    // 呼叫原生 Dapper 的 ExecuteScalar<T>
                    return cn.ExecuteScalar<T>(sql, param, commandTimeout: commandTimeout);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 執行 SQL 指令並回傳結果集第一列的第一行 (泛型強型別 簡易版)
        /// </summary>
        public T ExecuteScalar<T>(string sql, int? commandTimeout = null)
        {
            return ExecuteScalar<T>(sql, null, commandTimeout);
        }

        /// <summary>
        /// 執行 SQL 指令並回傳結果集第一列的第一行 (非泛型、回傳 object 版本)
        /// </summary>
        public object ExecuteScalar(string sql, object param, int? commandTimeout = null)
        {
            try
            {
                using (System.Data.IDbConnection cn = SqlClient)
                {
                    // 呼叫原生 Dapper 的 ExecuteScalar (回傳 object)
                    return cn.ExecuteScalar(sql, param, commandTimeout: commandTimeout);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 執行 SQL 指令並回傳結果集第一列的第一行 (非泛型 簡易版)
        /// </summary>
        public object ExecuteScalar(string sql, int? commandTimeout = null)
        {
            return ExecuteScalar(sql, null, commandTimeout);
        }
    }


    public class SqlObject
    {
        public SqlObject() { }
        public SqlObject(string sql, object param)
        {
            this.sql = sql;
            this.param = param;
        }
        public SqlObject(string sql, DynamicParameters dp)
        {
            this.sql = sql;
            this.param = param;
        }
        public string sql { get; set; }
        public object param { get; set; }
    }

    /// <summary>
    /// 2010.9.14 新建置-資料庫連接字串使用類別
    /// 2011.6.13 改寫
    /// </summary>
    public class ConnectionString
    {
        private ConnectionStringSettings objConnection;
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
