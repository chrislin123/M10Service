using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DhoeMvc.Class;
using M10.lib.modeldhoe;
using System.Text;
using System.IO;

namespace DhoeMvc.Controllers
{
  public class PhotoController : BaseController
  {
    // GET: Photo
    public ActionResult Photo()
    {
      List<AlbumM> DataList = dbDapper.Query<AlbumM>(@"select * from AlbumM
                        order by sort desc");

      ViewData["DataList"] = DataList;


      StringBuilder html = new StringBuilder();

      int idx = 0;
      foreach (AlbumM LoopItem in DataList)
      {
        string sImgSrc = "";
        if (LoopItem.cover != null)
        {
          sImgSrc = string.Format(@"/photo/n/{0}/{1}", LoopItem.name, LoopItem.cover);
        }
        else
        {
          string PhotoRootPath = Server.MapPath("~\\photo\\N");

          string PhotoPath = PhotoRootPath + string.Format("\\{0}\\", LoopItem.name);

          //資料夾不存在，跳離
          if (Directory.Exists(PhotoPath) == false) continue;

          DirectoryInfo di = new DirectoryInfo(PhotoPath);

          FileInfo[] afi = di.GetFiles();

          if (afi.Length > 0)
          {
            sImgSrc = string.Format(@"/photo/n/{0}/{1}", LoopItem.name, afi[0].Name);
          }
        }

        //string sHref = string.Format("<a href='{0}' class='{1}'>{2}</a>", "#", "album_info_title_hy", LoopItem.name);
        string sHref = string.Format("/Photo/PhotoDF?name={0}", LoopItem.name);

        idx++;

        if (idx % 3 == 1)
        {
          html.Append("<div class='row'>");
        }
        string sHtmlTemp = @"
        <div class='col-md-4'>
          <div class='album_item inline-block'>
            <div class='fix_png'>
              <a href='{2}' > 
                <img src='{1}' class='img-thumbnail' style='width:100%' />
              </a>
            </div>
            <div class='album_info'>
              <p class='album_info_title'>
                <a href='{2}' class='album_info_title_hy'>{0}</a>
              </p>
            </div>
          </div>
        </div>
        ";

        html.Append(string.Format(sHtmlTemp, LoopItem.name, sImgSrc, sHref));


        if (idx % 3 == 0)
        {
          html.Append("</div>");
        }
      }

      ViewBag.HtmlPhoto = html.ToString();
      
      return View();
    }

    public ActionResult PhotoDF(string name)
    {

      ViewBag.test = name;
      List<AlbumM> DataList = dbDapper.Query<AlbumM>(@"select * from AlbumM
                        order by sort desc");

      ViewData["DataList"] = DataList;


      StringBuilder html = new StringBuilder();

      int idx = 0;
      foreach (AlbumM LoopItem in DataList)
      {
        string sImgSrc = "";
        if (LoopItem.cover != null)
        {
          sImgSrc = string.Format(@"/photo/n/{0}/{1}", LoopItem.name, LoopItem.cover);
        }
        else
        {
          string PhotoRootPath = Server.MapPath("~\\photo\\N");

          string PhotoPath = PhotoRootPath + string.Format("\\{0}\\", LoopItem.name);

          //資料夾不存在，跳離
          if (Directory.Exists(PhotoPath) == false) continue;

          DirectoryInfo di = new DirectoryInfo(PhotoPath);

          FileInfo[] afi = di.GetFiles();

          if (afi.Length > 0)
          {
            sImgSrc = string.Format(@"/photo/n/{0}/{1}", LoopItem.name, afi[0].Name);
          }
        }

        //string sHref = string.Format("<a href='{0}' class='{1}'>{2}</a>", "#", "album_info_title_hy", LoopItem.name);
        string sHref = string.Format("PhotoDF.aspx?name={0}", LoopItem.name);

        idx++;

        if (idx % 3 == 1)
        {
          html.Append("<div class='row'>");
        }
        string sHtmlTemp = @"
        <div class='col-md-4'>
          <div class='album_item inline-block'>
            <div class='fix_png'>
              <a href='{2}' > 
                <img src='{1}' class='img-thumbnail' style='width:100%' />
              </a>
            </div>
            <div class='album_info'>
              <p class='album_info_title'>
                <a href='{2}' class='album_info_title_hy'>{0}</a>
              </p>
            </div>
          </div>
        </div>
        ";

        html.Append(string.Format(sHtmlTemp, LoopItem.name, sImgSrc, sHref));


        if (idx % 3 == 0)
        {
          html.Append("</div>");
        }
      }

      ViewBag.test = html.ToString();

      return View();
    }
  }
}