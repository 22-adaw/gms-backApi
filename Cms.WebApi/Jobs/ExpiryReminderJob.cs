using AutoMapper.Execution;
using Gms.Common;
using Gms.Entity;
using Gms.IService;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Gms.WebApi.Jobs
{
    /// <summary>
    /// 会员到期定时提醒任务
    /// </summary>
    [DisallowConcurrentExecution]  // 禁止并发执行，防止任务重叠
    public class ExpiryReminderJob : IJob
    {
        private readonly IVipInfoService vipInfoService;
        public ExpiryReminderJob(IVipInfoService vipInfoService)
        {
            this.vipInfoService = vipInfoService;
        }

        /// <summary>
        /// 查找将到期会员，发送邮件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"会员到期提醒任务开始执行，时间：{DateTime.Now}" );
            try
            {
                var targetDate = DateTime.Now.AddDays(10).Date;
                var expiringMembers = await vipInfoService.LoadEntities(v => v.IsDeleted == false && v.VipCard != null && v.VipCard.EndDate.HasValue && v.VipCard.EndDate.Value.Date == targetDate && v.VipCard.LastReminderSent == false).Include(v => v.VipCard).ToListAsync();
                Console.WriteLine($"[{DateTime.Now}] 找到 {expiringMembers.Count} 个即将到期的会员");

                foreach (var member in expiringMembers)
                {
                    // 发送提醒邮件
                    await SendReminderEmail(member);

                    // 标记已发送
                    member.VipCard.LastReminderSent = true;
                    member.VipCard.LastReminderTime = DateTime.Now;

                    Console.WriteLine($"[{DateTime.Now}] 已发送提醒邮件给：{member.VipEmail}");
                }

                await vipInfoService.SaveChangesAsync();
                Console.WriteLine($"[{DateTime.Now}] 会员到期提醒任务执行完成");
            }catch(Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] 任务执行失败：{ex.Message}");
            }
        }
        /// <summary>
        /// 发送提醒邮件具体实现
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private  async Task SendReminderEmail(VipInfo member)
        {
            var endDate = member.VipCard.EndDate.Value.ToString("yyyy年MM月dd日");
            var daysLeft = (member.VipCard.EndDate.Value - DateTime.Now).Days;

            var subject = "【会员到期提醒】您的会员即将到期";

            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>会员到期提醒</title>
</head>
<body style='font-family: Microsoft YaHei, Arial; padding: 20px;'>
    <div style='max-width: 600px; margin: 0 auto; background: #f8f9fa; border-radius: 12px; padding: 30px;'>
        <h2 style='color: #e67e22;'>⚠️ 会员到期提醒</h2>
        <p>尊敬的 <strong>{member.VipName}</strong> 用户：</p>
        <p>您的会员卡将于 <strong style='color: #e67e22;'>{endDate}</strong> 到期，</p>
        <p>距离到期还有 <strong style='color: #e67e22; font-size: 20px;'>{daysLeft}</strong> 天。</p>
        <p>为避免影响您的正常锻炼，请及时办理续费手续。</p>
        <hr style='margin: 20px 0; border-color: #ddd;'>
        <p style='color: #999; font-size: 12px;'>本邮件由系统自动发送，请勿回复。</p>
<hr style='border:none; border-top:1px solid #dee2e6; margin:35px 0;'>
<div style='text-align:right;'>
        <p style='margin:0 0 6px 0; font-weight:bold;'>健身房管理系统开发团队</p>
        <p style='margin:0; color:#6c757d; font-size:13px;'>{DateTime.Now.Year}年{DateTime.Now.Month}月{DateTime.Now.Day}日</p>
</div>
    </div>
</body>
</html>";

            var mailBox = new MailBox
            {
                To = new List<string> { member.VipEmail },
                Subject = subject,
                Body = body,
                IsHtml = true
            };

            MailQueueProvider.EnqueueMailBox(mailBox);
            await Task.CompletedTask;
        }
    }
}
