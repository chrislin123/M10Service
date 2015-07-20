using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.OleDb;

namespace CL.Data
{
    /// <summary>
    /// 轉換函式
    /// </summary>
    public class ConvertUtility
    {
       /// <summary>
       /// 型別轉換ByType
       /// </summary>
       /// <param name="type">Type型別</param>
       /// <returns></returns>
        public static DbType ConvertDbType(Type type)
        {
            switch (type.Name)
            {
                case "Int64":
                    return DbType.Int64;
                case "Int32":
                    return DbType.Int32;
                case "Byte[]":
                    return DbType.Byte;
                case "Boolean":
                    return DbType.Boolean;
                case "String":
                    return DbType.String;
                case "DateTime":
                    return DbType.DateTime;
                case "Float":
                case "Double":
                    return DbType.Double;
                case "Decimal":
                    return DbType.Decimal;
                default:
                    return DbType.Object;

            }
        }
       /// <summary>
       ///  型別轉換ByDbType(文字)
       /// </summary>
       /// <param name="type"></param>
       /// <returns></returns>
        public static DbType ConvertDbType(string type)
        {

            if (type == "nvarchar" || type == "nchar" || type == "text")
            {
                return DbType.String;
            }
            else if (type == "varchar" || type == "char")
            {
                return DbType.AnsiString;
            }
            else if (type == "int")
            {
                return DbType.Int32;
            }
            else if (type == "binary")
            {
                return DbType.Byte;
            }
            else if (type == "datetime")
            {
                return DbType.DateTime;
            }
            else if (type == "float")
            {
                return DbType.Double;
            }
            else if (type == "decimal")
            {
                return DbType.Decimal;
            }
            else
            {
                return DbType.Object;
            }
           
        }
       /// <summary>
       /// 判斷是否為文字型別
       /// </summary>
       /// <param name="dt"></param>
       /// <returns></returns>
        public static bool IsString(DbType dt)
        {
            switch (dt)
            {
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.Single:
                case DbType.Double:
                case DbType.SByte:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    return false;
                
            }
            return true;
        }
       /// <summary>
       /// 取得參數化前置字元
       /// </summary>
       /// <param name="objCon">資料庫連線物件</param>
       /// <returns></returns>
        public static string GetParameterChar(ConnectionString objCon)
        {

            switch (objCon.ProviderName)
            {
                case "System.Data.SqlClient":
                    return "@";
                case "System.Data.OleDb":
                case "System.Data.MySql":
                    return "?";
                case "Oracle.DataAccess.Client":
                    return ":";
                default:
                    return string.Empty;
            }
           
        }
    }
}
