using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M10.lib
{
  public static class M10Const
  {
    public static class AlertStatus
    {
      public const string I = "發布黃色";
      public const string C = "黃升紅";
      public const string O = "紅降黃";
      public const string D = "解除黃色";
    }

    public static class StockType
    {
      public const string otc = "otc";
      public const string tse = "tse";
    }

  }
}
