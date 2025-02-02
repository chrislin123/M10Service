using HtmlAgilityPack;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M10Tools
{
    public partial class FormTest : Form
    {
        string URL = "https://www.wantgoo.com/stock/major-investors/broker-buy-sell-rank";
        public FormTest()
        {
            InitializeComponent();
            InitializeAsync(); // 務必實施的步驟

            //webBrowser1.Url = @"http://isin.twse.com.tw/isin/C_public.jsp?strMode=2";
        }


        async void InitializeAsync() // 注意紅字
        {
            await webView21.EnsureCoreWebView2Async(null); // 初始化需要時間
        }

        private void webView21_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            webView21.CoreWebView2.Navigate(URL); // 載入 URL 所指定的網頁

        }

        private async void webView21_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            // 網頁完全載入, 可以提取其中的 HTML, 但方法不太直觀
            string html = await
            webView21.ExecuteScriptAsync("document.documentElement.outerHTML;");

            // 取出的 HTML 包含 escape 碼, 需要處理
            html = Regex.Unescape(html);
            // 而且前後還用雙引號 (") 包起來, 也得拿掉
            html = html.Remove(0, 1);
            html = html.Remove(html.Length - 1, 1);
            // 現在才是原先由舊版 WebBrowser 直接可以取得並後續處理的 HTML


            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument(); 
            doc.LoadHtml(html);


            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//select[1]/option");
            

            foreach (HtmlNode node in nodes)
            {
                string sss = node.InnerText; ;


                ////目前成交價
                //if (idx == 2)
                //{
                //    sr.z = node.InnerText;
                //}

                ////yahoo漲跌
                //if (idx == 5)
                //{
                //    if (node.InnerText.Length > 0)
                //    {
                //        sr.xx = node.InnerText.Substring(0, 1);
                //    }

                //}

                ////昨收
                //if (idx == 7)
                //{
                //    sr.y = node.InnerText;
                //}

                //idx++;
            }



        }



        

        //// 必須等到這個 event 發生, 才可以開始操作 webView
        //private void webView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        //{
        //    webView21.CoreWebView2.Navigate(URL); // 載入 URL 所指定的網頁
        //}

        //private async void webView_NavigationCompleted(object sender,
        //                    CoreWebView2NavigationCompletedEventArgs e)
        //{
            
        //}

    }
}
