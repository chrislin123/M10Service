using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Web;
using System.Net;

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

        public static string getDatatimeString(DateTime dt)
        {
            return dt.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public static string getDatatimeString()
        {
            DateTime dt = DateTime.Now;
            return getDatatimeString(dt);
        }


        public static DateTime getStringToDateTime(string sDatetime)
        {
            DateTime dt = DateTime.MinValue;


            string[] DateTimeList = {
                            "yyyy/M/d tt hh:mm:ss",
                            "yyyy/MM/dd tt hh:mm:ss",
                            "yyyy/MM/dd HH:mm:ss",
                            "yyyy/M/d HH:mm:ss",
                            "yyyy/M/d",
                            "yyyyMMdd"
                        };

            dt = DateTime.ParseExact(sDatetime,
                                              DateTimeList,
                                              System.Globalization.CultureInfo.InvariantCulture,
                                              System.Globalization.DateTimeStyles.AllowWhiteSpaces
                                              );

            //if (sDatetime.Length == 8)
            //{
                
            //}
            //dt = Convert.ToDateTime("20190802");


            return dt;
        }


        public static string getDateString(DateTime dt, M10Const.DateStringType dsType)
        {
            if (dsType == M10Const.DateStringType.ChineseT1)
            {
                return string.Format("{0}{1}{2}", Convert.ToString(dt.Year - 1911), dt.ToString("MM"), dt.ToString("dd"));
            }

            if (dsType == M10Const.DateStringType.ChineseT2)
            {
                return string.Format("{0}/{1}/{2}", Convert.ToString(dt.Year - 1911), dt.ToString("MM"), dt.ToString("dd"));
            }

            if (dsType == M10Const.DateStringType.ADT1)
            {
                return dt.ToString("yyyyMMdd");
            }

            if (dsType == M10Const.DateStringType.ADT2)
            {
                return dt.ToString("yyyy/MM/dd");
            }

            return "";
        }


        public static string getDatatimeString(DateTime dt, M10Const.DatetimeStringType dsType)
        {
            //if (dsType == M10Const.DateStringType.ChineseT1)
            //{
            //  return string.Format("{0}{1}{2}", Convert.ToString(dt.Year - 1911), dt.ToString("MM"), dt.ToString("dd"));
            //}

            //if (dsType == M10Const.DateStringType.ChineseT2)
            //{
            //  return string.Format("{0}/{1}/{2}", Convert.ToString(dt.Year - 1911), dt.ToString("MM"), dt.ToString("dd"));
            //}

            if (dsType == M10Const.DatetimeStringType.ADDT1)
            {
                return dt.ToString("yyyyMMddTHHmmss");
            }

            if (dsType == M10Const.DatetimeStringType.ADDT2)
            {
                return dt.ToString("yyyy-MM-ddTHH:mm:ss");
            }

            return "";
        }


        public static WebClient getNewWebClient()
        {
            var wc = new WebClient();
            //wc.Headers.Add("User-Agent", HttpHelper.GetRandomAgent());
            wc.Encoding = Encoding.UTF8;
            wc.Proxy = null;
            return wc;
        }

        /// <summary>
        /// 建立資料夾副本
        /// </summary>
        /// <param name="SourceDir">來源資料夾</param>
        /// <param name="TargetDir">目標資料夾</param>
        public static void CreateDirByCopy(string SourceDir, string TargetDir)
        {
            DirectoryInfo di = new DirectoryInfo(SourceDir);

            string sParentPath = Path.Combine(TargetDir, di.Name);

            if (Directory.Exists(sParentPath) == false)
            {
                Directory.CreateDirectory(sParentPath);
            }

            foreach (DirectoryInfo tempid in di.GetDirectories())
            {
                CreateDirByCopy(tempid.FullName, sParentPath);
            }

        }

    }
}
