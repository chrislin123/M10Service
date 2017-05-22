using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace M10.lib.model
{
  class model
  {
  }


  public class LRTIAlert
  {
    public int no { get; set; }

    public string STID { get; set; }

    public string country { get; set; }

    public string town { get; set; }

    public string village { get; set; }

    public string status { get; set; }

    public string HOUR3 { get; set; }

    public string RT { get; set; }

    public string LRTI { get; set; }

    public string ELRTI { get; set; }

    public string HOUR2 { get; set; }

    public string HOUR1 { get; set; }

  }

  [Table("LRTIAlertHis")]
  public class LRTIAlertHis
  {
    //設定key
    [Key]
    //自動相加key的設定
    //[ExplicitKey]
    public int no { get; set; }

    public string STID { get; set; }

    public string country { get; set; }

    public string town { get; set; }

    public string village { get; set; }

    public string status { get; set; }

    public string RecTime { get; set; }

    public string HOUR3 { get; set; }

    public string RT { get; set; }

    public string LRTI { get; set; }

    public string ELRTI { get; set; }

    public string HOUR2 { get; set; }

    public string HOUR1 { get; set; }

  }

  public class LRTIAlertMail
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string type { get; set; }

    public string value { get; set; }

  }






}
