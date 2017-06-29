﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using System.Transactions;

namespace M10.lib
{
  public class DALDapper
  {
    public DALDapper(string pConnStr)
    {
      _ConnStr = pConnStr;
    }

    string _ConnStr = string.Empty;

    public string ConnStr
    {
      get
      {
        return _ConnStr;
      }
    }

    public List<dynamic> Query(string sql)
    {
      using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
      {
        return cn.Query(sql).ToList();
      }
    }

    public List<dynamic> Query(string sql, object param)
    {
      using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
      {
        return cn.Query(sql, param).ToList();
      }
    }

    public List<T> Query<T>(string sql) where T : class
    {
      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          return cn.Query<T>(sql).ToList<T>();
        }
      }
      catch (Exception)
      {
        return null;
      }
    }

    public List<T> Query<T>(string sql, object param) where T : class
    {
      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          return cn.Query<T>(sql, param).ToList<T>();
        }
      }
      catch (Exception)
      {
        return null;
      }
    }



    public int QueryTotalCount(string sql)
    {
      using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
      { 
        return cn.Query(sql).Count();
      }
    }

    public int QueryTotalCount(string sql, object param)
    {
      using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
      {
        return cn.Query(sql, param).Count();
      }
    }

    public int Execute(string sql, object param)
    {
      using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
      {
        return cn.Execute(sql, param);
      }
    }

    public int Execute(string sql)
    {
      using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
      {
        return cn.Execute(sql);
      }
    }

    public int Execute(List<SqlObject> SqlList)
    {
      int i = 0;
      using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
      {
        using (var transactionScope = new TransactionScope())
        {
          foreach (SqlObject d in SqlList)
          {
            i += cn.Execute(d.sql, d.param);
          }
          transactionScope.Complete();

        }
      }
      return i;
    }

    public bool Update<T>(T entityToUpdate) where T : class
    {
      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          return cn.Update<T>(entityToUpdate);
        }
      }
      catch (Exception)
      {
        return false;
      }
    }

    public T QuerySingleOrDefault<T>(string sql) where T : class
    {
      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        { 
          return cn.QuerySingleOrDefault<T>(sql);
        }
      }
      catch (Exception)
      {
        return null;
      }
    }
   


    

    public T QuerySingleOrDefault<T>(string sql,object param) where T : class
    {
      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          return cn.QuerySingleOrDefault<T>(sql, param);
        }
      }
      catch (Exception)
      {
        return null;
      }
    }

    public long Insert<T>(T entityToInsert) where T : class
    {
      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        { 
          
          return cn.Insert<T>(entityToInsert);
        }
      }
      catch (Exception ex)
      {
        return 0;
      }
    }




    //public int InsertMasterAndDetail(SqlObject firstSO, ArrayList secondSOs, string masterIdInDetailColumnName)
    //{
    //  int i = 0;
    //  using (var cn = new MySql.Data.MySqlClient.MySqlConnection(this.Database.Connection.ConnectionString))
    //  {
    //    using (var transactionScope = new TransactionScope())
    //    {
    //      int id = cn.Query<int>(firstSO.sql, firstSO.param).Single();

    //      foreach (SqlObject so in secondSOs)
    //      {
    //        DynamicParameters dp = (DynamicParameters)so.param;
    //        dp.Add(masterIdInDetailColumnName, id, System.Data.DbType.Int16);

    //        i += cn.Execute(so.sql, dp);
    //      }
    //      transactionScope.Complete();
    //    }
    //  }
    //  return i;
    //}

    //public int Insert(ArrayList alSqlObjectl)
    //{
    //  int i = 0;
    //  using (var cn = new MySql.Data.MySqlClient.MySqlConnection(this.Database.Connection.ConnectionString))
    //  {
    //    using (var transactionScope = new TransactionScope())
    //    {
    //      foreach (SqlObject so in alSqlObjectl)
    //      {
    //        DynamicParameters dp = (DynamicParameters)so.param;
    //        i += cn.Execute(so.sql, dp);
    //      }
    //      transactionScope.Complete();
    //    }
    //  }
    //  return i;
    //}


    public object ExecuteScale(string sql, object param)
    {
      using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
      {
        return cn.ExecuteScalar(sql, param);
      }
    }

    public object ExecuteScale(string sql)
    {
      using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
      {
        return cn.ExecuteScalar(sql);
      }
    }

    //public List<dynamic> Query(string sql, int currentPage, int recordsPerPage)
    //{
    //  using (var cn = new MySql.Data.MySqlClient.MySqlConnection(this.Database.Connection.ConnectionString))
    //  {

    //    string limit = " LIMIT {0},{1}";
    //    limit = string.Format(
    //        limit,
    //        ((currentPage - 1) < 0 ? 0 : (currentPage - 1) * recordsPerPage),
    //        recordsPerPage);
    //    return cn.Query(sql + limit).ToList();
    //  }
    //}
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
}
