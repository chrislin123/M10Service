using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CL.Data;
using System.Text;
using System.IO;

namespace DhoePortal
{
    public partial class PhotoD : System.Web.UI.Page
    {
        ODAL oDal = new ODAL("DBConnectionString");
        string ssql = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                //取得相簿分類
                if (this.Request.Params["name"] == null)
                {
                    return;   
                }

                string sName  = this.Request.Params["name"].ToString();

                              
                DataTable dt = new DataTable();
                dt = getPhotoList(sName);

                DataList1.DataSource = dt;
                DataList1.DataBind();

                //取得相簿分類階層
                SetAlbumsLevel(sName);


            }
        }

        private void SetAlbumsLevel(string pName)
        {
            //Building an HTML string.
            StringBuilder html = new StringBuilder();


            //<ol class="breadcrumb">
            //    <li><a href="#">Home</a></li>
            //    <li><a href="#">2013</a></li>
            //    <li class="active">十一月</li>
            //</ol>

            //Table start.
            html.Append("<ol class='breadcrumb'>");
            html.Append("<li><a href='Photo.aspx'>Albums</a></li>");
            html.Append(string.Format("<li>{0}</li>", pName));         
            html.Append("</ol>");
           
            
            //Append the HTML string to Placeholder.
            phBread.Controls.Add(new Literal { Text = html.ToString() });




        }



        /// <summary>
        /// 取得照片明細清單
        /// </summary>
        /// <param name="pPara"></param>
        /// <returns></returns>
        private DataTable getPhotoList(string pPara)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("name");
            dt.Columns.Add("photoname");



            string PhotoRootPath = Server.MapPath("~\\photo\\N");

            string PhotoPath = PhotoRootPath + string.Format("\\{0}\\", pPara);

            DirectoryInfo di = new DirectoryInfo(PhotoPath);

            //資料夾不存在，跳離
            if (Directory.Exists(PhotoPath) == false) return dt;

            //資料夾沒照片 資料夾不存在，跳離
            if (di.GetFiles().Length == 0) return dt;


            string sImgSrc = "";
            FileInfo[] afi = di.GetFiles();

            foreach (FileInfo item in afi)
            {
                DataRow dr = dt.NewRow();
                dr["name"] = pPara;
                dr["photoname"] = item.Name;

                dt.Rows.Add(dr);
            }


            if (afi.Length > 0)
            {
                sImgSrc = afi[0].Name;
            }

            return dt;
        }


        protected void DataList1_ItemDataBound(object sender, DataListItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataListItem item = e.Item;

                DataRowView dr = (DataRowView)e.Item.DataItem;

                string sName = dr["name"].ToString();
                string sImgSrc = dr["photoname"].ToString();


                //判斷資料夾是否有照片
                //照片存檔根目錄
                string PhotoRootPath = Server.MapPath("~\\photo\\N");

                string PhotoPath = PhotoRootPath + string.Format("\\{0}\\", sName);

                DirectoryInfo di = new DirectoryInfo(PhotoPath);

                //資料夾不存在，跳離
                if (Directory.Exists(PhotoPath) == false) return;

                //資料夾沒照片 資料夾不存在，跳離
                if (di.GetFiles().Length == 0) return;



                //照片表頭
                //string sHref = string.Format("<a href='{0}' class='{1}'>{2}</a>", "//photo.xuite.net/j73025448/19904265", "album_info_title_hy", sName);
                //string sHref = string.Format("<a href='{0}' class='{1}'>{2}</a>", "#", "album_info_title_hy", sImgSrc);

                string sHref = string.Format("<span class='{1}'>{2}</span>", "#", "album_info_title_hy", sImgSrc);

                PlaceHolder pht = (PlaceHolder)item.FindControl("phtitle");
                if (pht != null)
                {
                    pht.Controls.Add(new Literal { Text = sHref });
                }


                //照片內容
                //Building an HTML string.
                StringBuilder html = new StringBuilder();
                //string sImgSrc = "";

                //FileInfo[] afi = di.GetFiles();

                //if (afi.Length > 0)
                //{
                //    sImgSrc = afi[0].Name;
                //}


                sImgSrc = string.Format(@"photo/n/{0}/{1}", sName, sImgSrc);

                //html.Append("<a href='//photo.xuite.net/j73025448/19904265'>");
                //html.Append("<a href='#'>");
                html.Append(string.Format("<a class='example-image-link' href='{0}' data-lightbox='example-1'>",sImgSrc));

                html.Append(string.Format("<img src='{0}'  border='0' class='img-thumbnail' style='width: 100%;'>", sImgSrc));
                //html.Append("<img src='images/logo.jpg'  border='0' class='img-thumbnail' style='width: 100%;'>");

                html.Append("</a>");


                PlaceHolder php = (PlaceHolder)item.FindControl("phphoto");
                if (php != null)
                {
                    php.Controls.Add(new Literal { Text = html.ToString() });
                }
            }


        }
    }
}