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
    public partial class Photo : System.Web.UI.Page
    {

        ODAL oDal = new ODAL("DBConnectionString");
        string ssql = string.Empty;



        protected void Page_Load(object sender, EventArgs e)
        {

            if (this.IsPostBack == false)
            {
                ssql = @"   select * from AlbumM
                        order by sort desc
                    ";
                oDal.CommandText = ssql;


                DataTable dt = new DataTable();
                dt = oDal.DataTable();


                DataList1.DataSource = dt;
                DataList1.DataBind();





            }



        }

        protected void DataList1_ItemDataBound(object sender, DataListItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataListItem item = e.Item;

                DataRowView dr = (DataRowView)e.Item.DataItem;

                string sName = dr["name"].ToString();

                

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
                string sHref = string.Format("<a href='{0}' class='{1}'>{2}</a>", "//photo.xuite.net/j73025448/19904265", "album_info_title_hy", sName);

                PlaceHolder pht = (PlaceHolder)item.FindControl("phtitle");
                if (pht != null)
                {
                    pht.Controls.Add(new Literal { Text = sHref });
                }


                //照片內容
                //Building an HTML string.
                StringBuilder html = new StringBuilder();
                string sImgSrc = "";

                FileInfo[] afi = di.GetFiles();

                if (afi.Length > 0)
                {
                    sImgSrc = afi[0].Name;
                }


                sImgSrc = string.Format(@"photo/n/{0}/{1}", sName, sImgSrc);
               
                html.Append("<a href='//photo.xuite.net/j73025448/19904265'>");

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