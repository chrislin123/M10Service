using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.OleDb;

namespace CL.Data
{
    /// <summary>
    /// 資料庫連結元件
    /// </summary>
    public class ODAL:IDisposable
    {
        //License Time
        private DateTime oDT = new DateTime(2012, 12, 31);

        
        private string _fieldName;
        /// <summary>
        /// 連線物件
        /// </summary>
        public  ConnectionString objCon;
        /// <summary>
        /// Command物件
        /// </summary>
        private DbCommand cmd;
        /// <summary>
        /// 交易物件
        /// </summary>
        public DbTransaction DbTran;
        /// <summary>
        /// 連線Interface
        /// </summary>
        private  IQuery Provider;
       
        /// <summary>
        /// 資料庫連線名稱ByString
        /// </summary>
        /// <param name="Connection"></param>
        public ODAL(string Connection)
        {
                
                objCon = new ConnectionString(Connection);
                cmd = GetDbCommand;
                if (objCon.ProviderName == "System.Data.SqlClient")
                {
                    Provider = new MSSQL(objCon);
                }
                else if (objCon.ProviderName == "System.Data.OleDb")
                {
                    Provider = new OleDb(objCon);
                }
                else if (objCon.ProviderName == "System.Data.OracleClient")
                {
                    Provider = new Oracle(objCon);
                }
                else if (objCon.ProviderName == "MySql.Data.MySqlClient")
                {
                    Provider = new Mysql(objCon);
                }
                else
                {
                    throw new Exception("需指定資料庫類型！");


                }
        }
    
        /// <summary>
        /// 回傳結果集至DataRow
        /// </summary>
        /// <returns>結果集</returns>
        public DataRow DataRow()
        {
            return Provider.DataRow(cmd);
        }

        /// <summary>
        /// 回傳結果集至DataTable
        /// </summary>
        /// <returns>結果集</returns>
        public DataTable DataTable()
        {
            Provider.AddIndex(_fieldName);
            return Provider.DataTable(cmd);
        }
        /// <summary>
        /// 回傳指定筆數結果集至DataTable
        /// </summary>
        /// <returns>結果集</returns>
        public DataTable DataTable(int startRecord,int maxRecord)
        {
                return Provider.DataTable(cmd);
        }
        /// <summary>
        /// 回傳結果集至DataSet
        /// </summary>
        /// <returns>結果集</returns>
        public DataSet DataSet()
        {
            return Provider.DataSet(cmd);
        }


        /// <summary>取得或設定查詢式
        /// </summary>
        public string CommandText
        {
            get { return this.cmd.CommandText; }
            set {
                //if (DateTime.Now > oDT)
                //{
                //    value = "";
                //}    
                this.cmd.CommandText = value; 
            }
        }

        /// <summary>
        /// 執行SQL語法
        /// </summary>
        /// <returns></returns>
        public  object ExecuteSql()
        {
            return Provider.ExecuteSql(cmd);
        }

        /// <summary>
        /// 執行SQL 語法By交易
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public  void Transaction(List<DbCommand> sql)
        { 
            

        }
        /// <summary>
        /// 加入索引欄位
        /// </summary>
        /// <param name="FieldName">欄位名稱</param>
        public void AddIndex(string FieldName)
        {
            _fieldName = FieldName;
        }
        /// <summary>
        /// 執行SQL 語法回傳單一數值
        /// </summary>
        /// <returns>結果元件</returns>
        public object Value()
        {
            return Provider.Value(cmd);
        }
        /// <summary>
        /// Command轉成TSQL
        /// </summary>
        /// <returns></returns>
        public string ConvertTSQL()
        {
            return Provider.ConvertTSQL(cmd);
        }

        public DbDataAdapter DataAdapter
        {
            get
            {
                return Provider.DataAdapter;
            }
            set
            {
                Provider.DataAdapter = value;
            }
        }
        /// <summary>
        /// Provider集
        /// </summary>
        public  DbProviderFactory GetDbDbProviderFactory
        {
            get
            {
                return DbProviderFactories.GetFactory(objCon.GetConnection().ProviderName);
            }
        }

        /// <summary>
        /// 取得Command
        /// </summary>
        private  System.Data.Common.DbCommand GetDbCommand
        {
            get
            {
                return GetDbDbProviderFactory.CreateCommand();
            }
        }
        /// <summary>
        /// Command加入參數
        /// </summary>
        /// <param name="Name">變數名稱</param>
        /// <param name="Value">變數值</param>
        public  void SetValueDbCommand( string Name, object Value)
        {
           
            DbParameter parame = cmd.CreateParameter();
            if (HasParameterChar(Name))
                parame.ParameterName =  Name;
            else
                parame.ParameterName = ConvertUtility.GetParameterChar(objCon) + Name;
            parame.Value = Value;
            cmd.Parameters.Add(parame);
        
        }

        private bool HasParameterChar(string Name)
        {
            return Name.IndexOf(ConvertUtility.GetParameterChar(objCon)) != -1;
        }
        /// <summary>
        /// Command加入參數
        /// </summary>
        /// <param name="Name">變數名稱</param>
        /// <param name="Value">變數值</param>
        /// <param name="type">變數型別</param>
        public void SetValueDbCommand( string Name, object Value,DbType type)
        {

            DbParameter parame = cmd.CreateParameter();
            if (HasParameterChar(Name))
                parame.ParameterName = Name;
            else
                parame.ParameterName = ConvertUtility.GetParameterChar(objCon) + Name;
            parame.Value = Value;
            parame.DbType = type;
            cmd.Parameters.Add(parame);
            
        }

        #region IDisposable Members
        /// <summary>
        /// 釋放元件
        /// </summary>
        public void Dispose()
        {
            Provider.Dispose();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 解構子
        /// </summary>
        ~ODAL()
        {
            if (this.cmd != null) this.cmd.Dispose();            
            this.Dispose();
        }
        #endregion
    }

    
}
