using Gms.Entity;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Common
{
    public class MailQueueManager
    {
        private readonly SmtpClient smtpClient;
        private readonly IOptions<EmailOptions> emailOptions;
        public MailQueueManager(IOptions<EmailOptions> emailOptions)
        {
            smtpClient = new SmtpClient();
            this.emailOptions = emailOptions;
        }
        public void Run()
        {
            var thread = new Thread(StartSendMail);
            thread.IsBackground = true;//关闭程序后台线程自动回收
            thread.Start();
        }
        public void StartSendMail()
        {
            while (true)
            {
                if (MailQueueProvider.MailQueue.Count <= 0)
                {
                    Thread.Sleep(5000);
                    continue;
                }
                if (MailQueueProvider.DequeueMailBox(out MailBox mailBox))
                {
                    SendMail(mailBox);
                }
            }
        }
        /// <summary>
        /// 开始构建具体的邮件内容
        /// </summary>
        /// <param name="box"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private void SendMail(MailBox box)
        {
            if (box == null)
            {
                throw new ArgumentNullException(nameof(box));
            }

            try
            {
                MimeMessage message = ConvertToMimeMessage(box);
                SendUserMail(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString(), "发送邮件发生异常主题：{0},收件人：{1}", box.Subject, box.To!.First());
            }

        }
        /// <summary>
        /// 邮件具体的内容转换成MimeMessage
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private MimeMessage ConvertToMimeMessage(MailBox box)
        {
            var config = emailOptions.Value;
            var message = new MimeMessage();
            //MailboxAddress(发送者名称，发送者账号)
            message.From.Add(new MailboxAddress(config.FromName, config.FromEmail));

            //下面构建的是接收者账号
            var mailboxAddressList = new List<MailboxAddress>();
            box.To!.ToList().ForEach(f =>
            {

                mailboxAddressList.Add(new MailboxAddress(box.Subject, f));
            });
            message.To.AddRange(mailboxAddressList);

            message.Subject = box.Subject;// 邮件主题

            // 下面构建邮件内容
            var builder = new BodyBuilder();
            if (box.IsHtml)
            {
                builder.HtmlBody = box.Body;
            }
            else
            {
                builder.TextBody = box.Body;
            }
            message.Body = builder.ToMessageBody();
            return message;
        }
        /// <summary>
        /// 邮件的具体发送
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private void SendUserMail(MimeMessage message)
        {
            var config = emailOptions.Value;
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            try
            {
                // 指定smtp服务器地址，以及端口号
                smtpClient.Connect(config.SmtpHost, config.SmtpPort, false);
                // Note: only needed if the SMTP server requires authentication
                if (!smtpClient.IsAuthenticated)
                {
                    // 指定发送者邮箱的地址以及授权码
                    smtpClient.Authenticate(System.Text.Encoding.UTF8, config.FromEmail, config.AuthCode);
                }
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }

            finally
            {
                smtpClient.Disconnect(false);
            }
        }

    }
}
