using System;
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
      int i = 0;

      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          i = cn.Query(sql).Count();
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
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          i = cn.Query(sql, param).Count();
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }

      return i;
    }

    public int Execute(string sql, object param)
    {
      int i = 0;

      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          i = cn.Execute(sql, param);
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return i;
    }

    public int Execute(string sql)
    {
      int i = 0;

      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          i = cn.Execute(sql);
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return i;
    }

    public int Execute(List<SqlObject> SqlList)
    {
      int i = 0;

      try
      {
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
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          bResult = cn.Update<T>(entityToUpdate);
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
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          oo = cn.QuerySingleOrDefault<T>(sql);
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
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          oo = cn.QuerySingleOrDefault<T>(sql, param);
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
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          lResult = cn.Insert<T>(entityToInsert);
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }

      return lResult;
    }

    public object ExecuteScale(string sql, object param)
    {
      object oo = null;

      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          oo = cn.ExecuteScalar(sql, param);
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }

      return oo;
    }

    public object ExecuteScale(string sql)
    {
      object oo = null;

      try
      {
        using (var cn = new System.Data.SqlClient.SqlConnection(ConnStr))
        {
          oo = cn.ExecuteScalar(sql);
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }

      return oo;
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


    public DynamicParameters GetNewDynamicParameters()
    {
      DynamicParameters dp = new DynamicParameters();

      return dp;
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
}
