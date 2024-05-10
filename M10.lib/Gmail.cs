using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace M10.lib
{
  public static class Gmail
  {

    /// <summary>
    /// 使用Gmail寄送郵件。
    /// //如果出現權限不足，可登入google帳號後，選擇下方網址，啟用低安全設定
    /// //https://www.google.com/settings/security/lesssecureapps
    /// </summary>
    /// <param name="SenderAddress">寄件者</param>
    /// <param name="SenderPass">寄件者密碼</param>
    /// <param name="MailContent">郵件內容</param>
    /// <param name="MailSubject">郵件標題</param>
    /// <param name="AddressList">收件者清單</param>
    /// <param name="Attachements">附件清單</param>
    /// <returns></returns>
    public static Boolean SendMailByGmail(string SenderAddress,string SenderPass, List<string> HtmlContentList, string MailSubject, List<string> AddressList, List<Attachment> Attachements)
    {
      Boolean bSendResult = true;

      string sAddressJoin = string.Join(",", AddressList);

      string MailContent = string.Join("", HtmlContentList);

      MailMessage message = new MailMessage(SenderAddress, sAddressJoin);//MailMessage(寄信者, 收信者)
      SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);//設定gmail的smtp
      try
      {
        message.IsBodyHtml = true;
        message.BodyEncoding = System.Text.Encoding.UTF8;//E-mail編碼
        message.SubjectEncoding = System.Text.Encoding.UTF8;//E-mail編碼
        message.Priority = MailPriority.Normal;//設定優先權
        message.Subject = MailSubject;//E-mail主旨
        message.Body = MailContent;//E-mail內容

        //Attachment attachment = new Attachment(@"C:\hsrv.txt");//<-這是附件部分~先用附件的物件把路徑指定進去~
        //message.Attachments.Add(attachment);//<-郵件訊息中加入附件
        foreach (Attachment item in Attachements)
        {
          message.Attachments.Add(item);
        }

        MySmtp.Credentials = new System.Net.NetworkCredential(SenderAddress, SenderPass);//gmail的帳號密碼System.Net.NetworkCredential(帳號,密碼)
        MySmtp.EnableSsl = true;//開啟ssl
        MySmtp.Send(message);
      }
      catch (Exception ex)
      {
        bSendResult = false;
      }
      finally
      {
        MySmtp = null;
        message.Dispose();
      }

      return bSendResult;
    }




  }
}
