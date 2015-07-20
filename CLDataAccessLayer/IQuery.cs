using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace CL.Data
{
    interface IQuery:IDisposable
    {
        /// <summary>
        /// 資料庫使用介面
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        object ExecuteSql(DbCommand cmd);

        System.Data.DataRow DataRow(DbCommand cmd);

        System.Data.DataTable DataTable(DbCommand cmd);

        object Value(DbCommand cmd);

        DataSet DataSet(DbCommand cmd);

        string ConvertTSQL(DbCommand cmd);
        void AddIndex(string FieldName);
        DbDataAdapter DataAdapter { get; set; }
       
    }
}
