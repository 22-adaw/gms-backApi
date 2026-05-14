using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class MailBox
    {
        /// <summary>
        /// 邮件正文
        /// </summary>
        public string? Body { get; set; }
        /// <summary>
        /// 邮件抄送给谁
        /// </summary>
        //public IEnumerable<string> Cc { get; set; }

        // 如果为true,表示有html内容
        public bool IsHtml { get; set; }
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string? Subject { get; set; }
        /// <summary>
        /// 发送给谁
        /// </summary>
        public IEnumerable<string>? To { get; set; }
    }
}
