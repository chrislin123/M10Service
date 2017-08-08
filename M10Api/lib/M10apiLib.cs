using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M10Api.lib
{
  public class M10apiLib
  {

    public static System.Collections.Specialized.NameValueCollection ParseQueryString(string QueryString)
    { 
      System.Collections.Specialized.NameValueCollection result = new System.Collections.Specialized.NameValueCollection();
      result =  HttpUtility.ParseQueryString(QueryString);
      return result;
    }

  }
}