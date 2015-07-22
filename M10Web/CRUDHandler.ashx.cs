using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Web;
using System.Text;


namespace M10Web
{
    /// <summary>
    /// CRUDHandler 的摘要描述
    /// </summary>
    public class CRUDHandler : IHttpHandler
    {

        //public void ProcessRequest(HttpContext context)
        //{
        //    context.Response.ContentType = "text/plain";
        //    context.Response.Write("Hello World");
        //}


        public void ProcessRequest(HttpContext context)
        {
            if (context.Request["mode"] != null)
            {
                string mode = context.Request["mode"].ToString();
                switch (mode)
                {
                    case "Qry":
                        QueryData(context);
                        break;
                    //case "INS":
                    //    InsertData(context);
                    //    break;
                    //case "DEL":
                    //    DeleteData(context);
                    //    break;
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        //查詢資料
        //public void QueryData(HttpContext context)
        //{
        //    //資料庫分頁取得資料方法
        //    string page = context.Request["page"];
        //    string rows = context.Request["rows"];
        //    List<users> li = new List<users>();
        //    DataSet ds = GetData(int.Parse(rows), int.Parse(page), context);
        //    foreach (DataRow dr in ds.Tables[0].Rows)
        //    {
        //        li.Add(new users()
        //        {
        //            id = dr["id"].ToString(),
        //            name = dr["name"].ToString(),
        //            age = dr["age"].ToString(),
        //            address = dr["address"].ToString()
        //        });
        //    }
        //    ReturnDate rd = new ReturnDate();
        //    rd.total = ds.Tables[1].Rows[0]["TotalCount"].ToString();
        //    rd.rows = li;
        //    DataContractJsonSerializer json = new DataContractJsonSerializer(rd.GetType());
        //    json.WriteObject(context.Response.OutputStream, rd);
        //}

        private void QueryData(HttpContext context)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append(" \"total\":239,");
            sb.Append(" \"rows\":[");
            sb.Append("    {\"id\":\"001\",\"name\":\"Name 1\",\"age\":\"12\",\"address\":\"Address 11\"},");
            sb.Append("    {\"id\":\"002\",\"name\":\"Name 2\",\"age\":\"18\",\"address\":\"Address 13\"},");
            sb.Append("    {\"id\":\"003\",\"name\":\"Name 3\",\"age\":\"12\",\"address\":\"Address 87\"},");
            sb.Append("    {\"id\":\"004\",\"name\":\"Name 4\",\"age\":\"13\",\"address\":\"Address 63\"},");
            sb.Append("    {\"id\":\"005\",\"name\":\"Name 5\",\"age\":\"52\",\"address\":\"Address 45\"},");
            sb.Append("    {\"id\":\"006\",\"name\":\"Name 6\",\"age\":\"72\",\"address\":\"Address 16\"},");
            sb.Append("    {\"id\":\"007\",\"name\":\"Name 7\",\"age\":\"34\",\"address\":\"Address 27\"},");
            sb.Append("    {\"id\":\"008\",\"name\":\"Name 8\",\"age\":\"22\",\"address\":\"Address 81\"},");
            sb.Append("    {\"id\":\"009\",\"name\":\"Name 9\",\"age\":\"18\",\"address\":\"Address 69\"},");
            sb.Append("    {\"id\":\"010\",\"name\":\"Name 10\",\"age\":\"28\",\"address\":\"Address 19\"}");
            sb.Append("]}");
            context.Response.Write(sb.ToString());


        }

        public class ReturnDate
        {
            public string total { get; set; }
            public List<users> rows { get; set; }
        }

        public class users
        {
            public string id { get; set; }
            public string name { get; set; }
            public string age { get; set; }
            public string address { get; set; }
        }
    }
}