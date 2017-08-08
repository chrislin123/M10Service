﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Web;

namespace M10.lib
{
  public class Utils
  {

    public static DataTable ConvertToDataTable<T>(IList<T> data)
    {
      PropertyDescriptorCollection properties =
         TypeDescriptor.GetProperties(typeof(T));
      DataTable table = new DataTable();
      foreach (PropertyDescriptor prop in properties)
        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
      foreach (T item in data)
      {
        DataRow row = table.NewRow();
        foreach (PropertyDescriptor prop in properties)
          row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
        table.Rows.Add(row);
      }
      return table;
    }

    public static IEnumerable<string> GetPropertyName<T>()
    {
      var prof = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

      return prof.Select(p => p.Name);
    }


    public static System.Collections.Specialized.NameValueCollection ParseQueryString(string QueryString)
    {
      System.Collections.Specialized.NameValueCollection result = new System.Collections.Specialized.NameValueCollection();
      result = HttpUtility.ParseQueryString(QueryString);
      return result;
    }



  }
}
