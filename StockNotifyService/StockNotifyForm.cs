using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using isRock.LineBot;
using M10;
using M10.lib.model;


namespace StockNotifyService
{
    public partial class StockNotifyForm : Form
    {

        string CHANNEL_ACCESS_TOKEN = "5IA4CnSwKujUKPwG1F+Fc8rAFHj/8b+g4TctWhBTqi0kLu0pxXTzOhk+IKQ5d+mfvfijHhqUYpwtyjebWtwqupVHYVeIzgu2dhrf2StNzoEr8UF7dVjRwgA1+2FDWSBJBxNpoLAwAOY3vQbUJ7GITAdB04t89/1O/w1cDnyilFU=";
        //老巴UserID
        string UserID_General = "U29fe27cab3c5147be0bbc825911afef1";
        //賺爆群ID
        //string UserID_General = "Cd65c1b8e4ded0a6c587d69e18d559e0b";



        private isRock.LineBot.Bot _bot = null;

        public Bot Bot
        {
            get
            {
                if (_bot == null)
                {
                    _bot = new isRock.LineBot.Bot(CHANNEL_ACCESS_TOKEN);
                }

                return _bot;
            }
            set { _bot = value; }
        }


        public StockNotifyForm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bot.PushMessage(UserID_General, textBox2.Text.Trim());
        }

        //新的執行緒
        Thread othread;

        private void button1_Click(object sender, EventArgs e)
        {
            //將這個WinForm上的TextBox指定給SysHelper._textbox這個參數
            SysHelper._textbox = textBox1;
            //告訴這個執行緒，該去執行這個方法
            othread = new Thread(SysHelper.Run);

            //開始
            othread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //停止執行緒的運作!!!
            othread.Abort();
        }

        private BackgroundWorker GenWorker(string sid)
        {
            //Bot.PushMessage(UserID_General, textBox2.Text.Trim());
            BackgroundWorker bw = new BackgroundWorker();

            //回報進程
            bw.WorkerReportsProgress = true;

            bw.WorkerSupportsCancellation = true;
            //加入DoWork
            bw.DoWork += new DoWorkEventHandler(Proc_DoWorkGetSrt);
            //加入ProgressChanged
            bw.ProgressChanged += new ProgressChangedEventHandler(Proc_ProgressChanged);
            //加入RunWorkerCompleted
            //bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Proc_RunWorkerCompleted);
            //傳遞參數
            //object i = new object();

            //執行程序    
            bw.RunWorkerAsync(sid);

            return bw;
        }
        

        List<BackgroundWorker> bwList = new List<BackgroundWorker>();
        private void button4_Click(object sender, EventArgs e)
        {
            //TXF-9999
            //OTC-
            //TSE-0000

            bwList.Add(GenWorker("0000"));
            bwList.Add(GenWorker("9999"));
            bwList.Add(GenWorker("%23026"));
        }

        static string PrintDateTime()
        {
            return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ");
            //Console.Write(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss "));
        }


        private void Proc_DoWorkGetSrt(object sender, DoWorkEventArgs e)
        {
            //ReportInfo ri = new ReportInfo();
            BackgroundWorker bw = sender as BackgroundWorker;
            string sid = e.Argument as string;

            
            while (true)
            {
                if ((bw.CancellationPending == true))
                {
                    e.Cancel = true;
                    return;
                }


                M10.lib.StockHelper stHelper = new M10.lib.StockHelper();
                StockYahooAPI sr = stHelper.getStockRealtimeYahooApi2(sid);

                //lblid.Text = sr.id;
                //lbltxfname.Text = sr.name;
                //lblprice.Text = sr.close;
                //lblvol.Text = sr.vol;

                string ss = string.Format("[{0}]{1}({2})-{3}-{4}", PrintDateTime(), sr.name,sr.id,sr.close,sr.vol);
                bw.ReportProgress(1, ss);

                Thread.Sleep(1000);
            }

        }

        private void ShowMessageToFront(string pMsg)
        {

            if (richTextBox1.Lines.Length > 14)
            {
                richTextBox1.Clear();
            }

            richTextBox1.AppendText(pMsg + "\r\n");


            //this.Refresh();
            this.Update();
            //richTextBox1.Refresh();
            Application.DoEvents();
        }

        private void Proc_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ShowMessageToFront(e.UserState.ToString());
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (BackgroundWorker item in bwList)
            {
                item.CancelAsync();
                return;
            }
            //bw.CancelAsync();
        }
    }



    //我們創一個類別讓它一直算數一直算數一直算數，當他可以被1000整除時進行回報
    class SysHelper
    {
        public static TextBox _textbox;
        //要呼叫別的執行緒就必須透過委派!!
        delegate void PrintHandler(TextBox tb, string text);
        public static void Run()
        {
            //這是一個麻煩的事情，他會無窮迴圈的一直算數
            int x = 0;
            while (true)
            {
                if (x % 1000 == 0)
                {
                    //當數字會被1000整除時，我要通報別的執行緒上的控制項進行顯示
                    Print(_textbox, x.ToString());
                }
                x++;
            }
        }

        public static void Print(TextBox tb, string text)
        {
            //判斷這個TextBox的物件是否在同一個執行緒上
            if (tb.InvokeRequired)
            {
                //當InvokeRequired為true時，表示在不同的執行緒上，所以進行委派的動作!!
                PrintHandler ph = new PrintHandler(Print);
                tb.Invoke(ph, tb, text);
            }
            else
            {
                //表示在同一個執行緒上了，所以可以正常的呼叫到這個TextBox物件
                //tb.Text = tb.Text + text + Environment.NewLine;
                tb.Text = text;
            }
        }
    }
}
