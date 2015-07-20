using System;
using System.Collections.Generic;
using System.Text;

namespace CL.Data
{
    /// <summary>
    /// 訊息傳輸元件
    /// </summary>
    [System.Obsolete("此類別已過期，不建議使用，並於下次改版移除")]
    public class Message
    {
        /// <summary>
        /// 建構子
        /// </summary>
        public Message()
        {

        }
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string Error{get;set;}
        /// <summary>
        /// 執行是否成功
        /// </summary>
        public bool IsCompelete{get;set;}
        /// <summary>
        /// 傳回值
        /// </summary>
        public object Value { get; set; }
    }
}
