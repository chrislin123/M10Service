using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace M10.lib.modeldhoe
{
  class modeldhoe
  {
  }

  [Table("Newsm")]
  public class Newsm
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string date { get; set; }

    public string title { get; set; }

    public string link { get; set; }

    public string emp { get; set; }

    public string dttm { get; set; }

  }

  [Table("Connect")]
  public class Connect
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string date { get; set; }

    public string title { get; set; }

    public string link { get; set; }

    public string emp { get; set; }

    public string dttm { get; set; }

  }

  [Table("AlbumM")]
  public class AlbumM
  {
    //設定key
    [Key]
    public int intseq { get; set; }

    public string name { get; set; }

    public int? sort { get; set; }

    public string emp { get; set; }

    public string dttm { get; set; }

    public string cover { get; set; }

  }

  [Table("Books")]
  public class Books
  {
    //設定key
    [Key]
    public int inseq { get; set; }

    public string date { get; set; }

    public string content { get; set; }

    public string emp { get; set; }

    public string dttm { get; set; }

  }

  [Table("CenterMember")]
  public class CenterMember
  {
    //設定key
    [Key]
    public int inseq { get; set; }

    public string name { get; set; }

    public string content { get; set; }

    public string emp { get; set; }

    public string dttm { get; set; }

  }

  [Table("CenterProject")]
  public class CenterProject
  {
    //設定key
    [Key]
    public int inseq { get; set; }

    public string name { get; set; }

    public string content { get; set; }

    public string emp { get; set; }

    public string dttm { get; set; }

  }

  [Table("ConfPaper")]
  public class ConfPaper
  {
    //設定key
    [Key]
    public int inseq { get; set; }

    public string date { get; set; }

    public string content { get; set; }

    public string emp { get; set; }

    public string dttm { get; set; }

  }

  [Table("JournalPaper")]
  public class JournalPaper
  {
    //設定key
    [Key]
    public int intseq { get; set; }

    public string date { get; set; }

    public string content { get; set; }

    public string emp { get; set; }

    public string dttm { get; set; }

  }

  [Table("Publications")]
  public class Publications
  {
    //設定key
    [Key]
    public int inseq { get; set; }

    public string date { get; set; }

    public string content { get; set; }

    public string emp { get; set; }

    public string dttm { get; set; }

  }

  [Table("Students")]
  public class Students
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string kind { get; set; }

    public string name { get; set; }

    public string emp { get; set; }

    public string dttm { get; set; }

    public string datatype { get; set; }

  }

  [Table("StudentD")]
  public class StudentD
  {
    //設定key
    [Key]
    public int? no { get; set; }

    public int? studentno { get; set; }

    public string type { get; set; }

    public string value { get; set; }

  }

  [Table("Research")]
  public class Research
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string date { get; set; }

    public string content { get; set; }

    public string emp { get; set; }

    public string dttm { get; set; }

  }

  [Table("DhoeSet")]
  public class DhoeSet
  {
    //設定key
    [Key]
    public int no { get; set; }

    public string settype { get; set; }

    public string setkind { get; set; }

    public int setseq { get; set; }

    public string setvalue { get; set; }

  }


}
