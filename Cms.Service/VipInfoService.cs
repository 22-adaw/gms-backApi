using Gms.Common;
using Gms.Entity;
using Gms.Entity.DTO;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.EntityFrameworkCore;
using Gms.IRepository;
using Gms.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Service
{
    public class VipInfoService:BaseService<VipInfo>,IVipInfoService
    {
        private readonly IVipInfoRepository vipInfoRepository;
        private readonly MyDbContext myDbContext;
        private readonly IRoleInfoRepository roleInfoRepository;
        private readonly IUserInfoRepository userInfoRepository;
        private readonly IMemoryCache memoryCache;
        private readonly IConfiguration configuration;

        public VipInfoService(IVipInfoRepository vipInfoRepository, MyDbContext myDbContext, IRoleInfoRepository roleInfoRepository, IUserInfoRepository userInfoRepository, IMemoryCache memoryCache, IConfiguration configuration)
        {
            base.Repository = vipInfoRepository;
            this.vipInfoRepository = vipInfoRepository;
            this.myDbContext = myDbContext;
            this.roleInfoRepository = roleInfoRepository;
            this.userInfoRepository = userInfoRepository;
            this.memoryCache = memoryCache;
            this.configuration = configuration;
        }
        /// <summary>
        /// 分页获取会员信息
        /// </summary>
        /// <param name="vipSearch"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public  IQueryable<VipInfo> LoadPagesEntities(VipSearch vipSearch, bool isDeleted)
        {
            var temp=  vipInfoRepository.LoadEntities(v => v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal));
            if(!string.IsNullOrEmpty(vipSearch.VipPhone))
            {
                temp = temp.Where(t => t.VipPhone.Contains(vipSearch.VipPhone));
            } 
            if(!string.IsNullOrEmpty(vipSearch.VipName))
            {
                temp = temp.Where(t => t.VipName.Contains(vipSearch.VipName));
            }
            if (!string.IsNullOrEmpty(vipSearch.VipEmail))
            {
                temp = temp.Where(t => t.VipEmail.Contains(vipSearch.VipEmail));
            }
                vipSearch.TotalCount = temp.Count();
            int skip = (vipSearch.PageIndex - 1) * vipSearch.PageSize;
            int take = vipSearch.PageSize;


            temp = vipSearch.Order
                    ? temp.OrderBy(a => a.Id).Include(u => u.VipCard).Skip(skip).Take(take)
                    : temp.OrderByDescending(a => a.Id).Include(u => u.VipCard).Skip(skip).Take(take);
            return temp;
           
        }
        //已废弃
        //public MyDbContext GetDbContext()
        //{
        //    return myDbContext; 
        //}


        /// <summary>
        /// 由管理人员会员开通账户邮件通知
        /// </summary>
        /// <param name="vipInformDTO"></param>
        /// <returns></returns>
        public Task<bool> HandleInform(VipInformDTO vipInformDTO)
        {
            //构建邮件内容
            StringBuilder sb = new StringBuilder();
            // 邮件HTML正文
            sb.Append(@"
<!DOCTYPE html>
<html lang='zh-CN'>
<head>
    <meta charset='UTF-8'>
    <title>智建中心健身房 账号开通通知</title>
    <style>
        body{font-family:微软雅黑,Arial;line-height:1.8;color:#333;margin:0;padding:20px;background:#f5f7fa;}
        .mail-box{max-width:600px;margin:0 auto;background:#fff;padding:30px;border-radius:10px;box-shadow:0 2px 12px rgba(0,0,0,0.08);}
        .title{font-size:22px;font-weight:bold;color:#2c3e50;border-bottom:1px solid #eee;padding-bottom:15px;margin-bottom:20px;text-align:center;}
        .info-item{margin:12px 0;padding:8px 0;}
        .label{font-weight:bold;color:#1d4ed8;}
        .tip-box{margin-top:25px;padding:15px;background:#f0f7ff;border-radius:6px;border-left:4px solid #165DFF;}
        .footer{margin-top:30px;padding-top:15px;border-top:1px solid #eee;color:#666;font-size:13px;text-align:right;}
    </style>
</head>
<body>
    <div class='mail-box'>
        <div class='title'>智建中心健身房 账号开通通知</div>
        <p>尊敬的会员您好：</p>
        <p>恭喜您！您的健身房会员账户已由管理员成功开通，以下为您的账号登录信息，请妥善保管：</p>

        <div class='info-item'>
            <span class='label'>会员用户名：</span>
");
            sb.Append(vipInformDTO.UserName);
            sb.Append(@"
        </div>
        <div class='info-item'>
            <span class='label'>初始登录密码：</span>
");
            sb.Append(vipInformDTO.Password);
            sb.Append(@"
        </div>
        <div class='info-item'>
            <span class='label'>登录账号：</span>
");
            sb.Append(vipInformDTO.Phone);
            sb.Append(@"
        </div>

        <div class='tip-box'>
            <p><strong>⚠️ 安全温馨提示</strong></p>
            <p>1. 首次登录系统后，请立即修改初始密码，保障账号安全；</p>
            <p>2. 请勿将账号密码泄露给他人，谨防账号盗用风险；</p>
            <p>3. 登录系统后可查看会员权益、课程预约、健身充值等全部服务；</p>
        </div>

        <div style='margin-top:25px;'>
            感谢您的信赖和选择，祝您健身愉快，收获健康体魄！
        </div>

        <div class='footer'>
            <div style='margin-bottom:5px;'>智建中心健身房管理系统开发团队</div>
            <div>");
            sb.Append($"{DateTime.Now.Year}年{DateTime.Now.Month}月{DateTime.Now.Day}日");
            sb.Append(@"
            </div>
            <div style='margin-top:8px; font-size:12px; color:#666;'>
                本邮件为系统官方自动发送邮件，请勿直接回复
            </div>
        </div>

    </div>
</body>
</html>
");
            MailBox mailBox = new MailBox()
            {
                Body = sb.ToString(),
                IsHtml = true,
                Subject = "账号开通",
                To = new List<string>() { vipInformDTO.Email }
            };
            MailQueueProvider.EnqueueMailBox(mailBox);
            return Task.FromResult(true);
        }
        /// <summary>
        /// 办卡成功邮件通知
        /// </summary>
        /// <param name="purchaseInformDTO"></param>
        /// <returns></returns>
        public Task<bool> HandlePurchaseInform(PurchaseInformDTO purchaseInformDTO)
        {
            StringBuilder sb = new StringBuilder();
            //邮件正文
            sb.Append(@"
<!DOCTYPE html>
<html lang='zh-CN'>
<head>
    <meta charset='UTF-8'>
    <title>智建中心健身房 会员购卡成功通知</title>
    <style>
        body{font-family:微软雅黑,Arial;line-height:1.8;color:#333;margin:0;padding:20px;background:#f5f7fa;}
        .mail-box{max-width:600px;margin:0 auto;background:#fff;padding:30px;border-radius:10px;box-shadow:0 2px 12px rgba(0,0,0,0.08);}
        .title{font-size:22px;font-weight:bold;color:#2c3e50;border-bottom:1px solid #eee;padding-bottom:15px;margin-bottom:20px;}
        .info-item{margin:12px 0;padding:8px 0;}
        .label{font-weight:bold;color:#1d4ed8;}
        .tip-box{margin-top:25px;padding:15px;background:#f0f7ff;border-radius:6px;border-left:4px solid #165DFF;}
        .footer{margin-top:30px;padding-top:15px;border-top:1px solid #eee;color:#666;font-size:13px;text-align:right;}
    </style>
</head>
<body>
    <div class='mail-box'>
        <div class='title'>智建中心健身房 会员购卡成功通知</div>
        <p>尊敬的会员您好：</p>
        <p>恭喜您！您已成功办理健身房会员套餐，以下为您的会员信息与权益：</p>

        <div class='info-item'>
            <span class='label'>会员卡类型：</span>
");
            sb.Append(purchaseInformDTO.CardTypeName);
            sb.Append(@"
        </div>
        <div class='info-item'>
            <span class='label'>会员卡号：</span>
");
            sb.Append(purchaseInformDTO.CardNum);
            sb.Append(@"
        </div>
        <div class='info-item'>
            <span class='label'>账户剩余余额：</span>
");
            sb.Append(purchaseInformDTO.LeftMoney);
            sb.Append(@" 元</div>
        <div class='info-item'>
            <span class='label'>会员有效期至：</span>
");
            sb.Append(purchaseInformDTO.EndDate.HasValue
     ? purchaseInformDTO.EndDate.Value.ToString("yyyy年MM月dd日")
     : "暂无");
            sb.Append(@"
        </div>
        <div class='info-item'>
            <span class='label'>剩余健身次数：</span>
");
            sb.Append(purchaseInformDTO.RemainTime);
            sb.Append(@" 次</div>
        <div class='info-item'>
            <span class='label'>会员折扣率：</span>
");
            sb.Append(purchaseInformDTO.DisCountRate);
            sb.Append(@"
        </div>

        <!-- 会员权益 -->
        <div class='info-item'>
            <span class='label'>会员权益：</span>
");
            sb.Append(purchaseInformDTO.Remark);
            sb.Append(@"
        </div>

        <div class='tip-box'>
            <p><strong>⚠️ 温馨提示</strong></p>
            <p>1. 请凭会员信息到店使用场馆设施与课程服务；</p>
            <p>2. 会员有效期内可正常享受对应折扣与权益；</p>
            <p>3. 请妥善保管账号信息，请勿转借他人；</p>
            <p>4. 如有疑问可联系场馆管理员进行咨询。</p>
        </div>

        <div style='margin-top:25px;'>
            感谢您的信赖与选择，祝您健身愉快，收获健康体魄！
        </div>

        <div class='footer'>
            <div style='margin-bottom:5px;'>智建中心健身房管理系统开发团队</div>
            <div>");
            sb.Append($"{DateTime.Now.Year}年{DateTime.Now.Month}月{DateTime.Now.Day}日");
            sb.Append(@"
            </div>
            <div style='margin-top:8px; font-size:12px; color:#666;'>
                本邮件为系统官方自动发送邮件，请勿直接回复
            </div>
        </div>

    </div>
</body>
</html>
");
            MailBox mailBox = new MailBox()
            {
                Body = sb.ToString(),
                IsHtml = true,
                Subject = "办卡成功",
                To = new List<string>() { purchaseInformDTO.Email }
            };
            MailQueueProvider.EnqueueMailBox(mailBox);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 充值成功邮件通知
        /// </summary>
        /// <param name="addInformDTO"></param>
        /// <returns></returns>
        public Task<bool> HandleAddInform(AddInformDTO addInformDTO)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
<!DOCTYPE html>
<html lang='zh-CN'>
<head>
    <meta charset='UTF-8'>
    <title>智建中心健身房 会员充值成功通知</title>
    <style>
        body{font-family:微软雅黑,Arial;line-height:1.8;color:#333;margin:0;padding:20px;background:#f5f7fa;}
        .mail-box{max-width:600px;margin:0 auto;background:#fff;padding:30px;border-radius:10px;box-shadow:0 2px 12px rgba(0,0,0,0.08);}
        .title{font-size:22px;font-weight:bold;color:#2c3e50;border-bottom:1px solid #eee;padding-bottom:15px;margin-bottom:20px;}
        .info-item{margin:12px 0;padding:8px 0;}
        .label{font-weight:bold;color:#1d4ed8;}
        .tip-box{margin-top:25px;padding:15px;background:#f0f7ff;border-radius:6px;border-left:4px solid #165DFF;}
        .footer{margin-top:30px;padding-top:15px;border-top:1px solid #eee;color:#666;font-size:13px;text-align:right;}
    </style>
</head>
<body>
    <div class='mail-box'>
        <div class='title'>智建中心健身房 会员充值成功通知</div>
        <p>尊敬的会员您好：</p>
        <p>恭喜您！您的会员账户已充值成功，以下为本次充值详情，请您查收：</p>

        <div class='info-item'>
            <span class='label'>会员卡号：</span>
");
            sb.Append(addInformDTO.CardNum);
            sb.Append(@"
        </div>
        <div class='info-item'>
            <span class='label'>充值金额：</span>
");
            sb.Append(addInformDTO.Amount);
            sb.Append(@" 元</div>
        <div class='info-item'>
            <span class='label'>剩余余额：</span>
");
            sb.Append(addInformDTO.LeftMoney);
            sb.Append(@" 元</div>

        <div class='tip-box'>
            <p><strong>⚠️ 温馨提示</strong></p>
            <p>1. 充值余额可用于场馆消费、课程预约、会员服务抵扣；</p>
            <p>2. 账户余额永久有效，可正常累计使用；</p>
            <p>3. 请妥善保管您的会员账号信息，请勿转借他人；</p>
            <p>4. 如有充值到账疑问，请及时联系健身房管理员核实处理。</p>
        </div>

        <div style='margin-top:25px;'>
            感谢您的信赖与选择，祝您健身愉快，收获健康体魄！
        </div>

        <div class='footer'>
            <div style='margin-bottom:5px;'>智建中心健身房管理系统开发团队</div>
            <div>");
            // 动态当前年月日
            sb.Append($"{DateTime.Now.Year}年{DateTime.Now.Month}月{DateTime.Now.Day}日");
            sb.Append(@"
            </div>
            <div style='margin-top:8px; font-size:12px; color:#666;'>
                本邮件为系统官方自动发送邮件，请勿直接回复
            </div>
        </div>

    </div>
</body>
</html>
");
            MailBox mailBox = new MailBox()
            {
                Body = sb.ToString(),
                IsHtml = true,
                Subject = "充值成功",
                To = new List<string>() { addInformDTO.Email }
            };
            MailQueueProvider.EnqueueMailBox(mailBox);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 管理员操作注册会员
        /// </summary>
        /// <param name="vipInfoDTO"></param>
        /// <returns></returns>
        public async Task<bool> AdminManifest(VipInfoDTO vipInfoDTO)
        {
            if (vipInfoDTO.IsAdminManifest)
            {
                var password = PasswordHelper.GeneratePassword();
                var vipInfo = new VipInfo()
                {
                    VipEmail = vipInfoDTO.VipEmail,
                    VipName = vipInfoDTO.VipName,
                    VipPhone = vipInfoDTO.VipPhone,
                    Gender = vipInfoDTO.Gender,
                    IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal),
                    Status = "0",
                    CreateDate = DateTime.Now,
                    VipPassword = PasswordHelper.HashPassword(password)
                };

                await vipInfoRepository.AddEntity(vipInfo);
                await vipInfoRepository.SaveChangesAsync();

                var role = await roleInfoRepository.LoadEntities(r => r.Id == 8 && r.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).ToListAsync();
                var user = new UserInfo()
                {
                    Id = vipInfo.Id,
                    UserName = vipInfoDTO.VipName,
                    UserPassword = vipInfo.VipPassword,
                    UserEmail = vipInfoDTO.VipEmail,
                    UserPhone = vipInfoDTO.VipPhone,
                    PhotoUrl = "images\\aa.jpg",
                    Gender = Convert.ToInt32(vipInfoDTO.Gender),
                    CreateDate = DateTime.Now,
                    EditDate = DateTime.Now,
                    RealName = vipInfoDTO.VipName,
                    RoleInfos = role,
                    IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal),
                    DepartmentId = 32,
                    IsClient = Convert.ToBoolean(ClientCheckEnum.Client)
                };

                await userInfoRepository.AddEntity(user);

                // 发送开通邮件
                VipInformDTO vipInformDTO = new VipInformDTO()
                {
                    UserName = vipInfoDTO.VipName,
                    Password = password,
                    Phone = vipInfoDTO.VipPhone,
                    Email = vipInfoDTO.VipEmail,
                };
                await HandleInform(vipInformDTO);

                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 用户注册发送激活邮件
        /// </summary>
        /// <param name="vipInfoDTO"></param>
        /// <returns></returns>

        public async Task<bool> ClientManifest(VipInfoDTO vipInfoDTO)
        {
            if (vipInfoDTO.IsAdminManifest)
            {
                return false;
            }

            // 检查是否已经在缓存中等待激活（防止重复发送）
            string cacheKey = $"register:{vipInfoDTO.VipPhone}";
            if (memoryCache.TryGetValue(cacheKey, out _))
            {
                return false;  //已存在等待激活的记录，不重复处理
            }

            //生成激活码
            string activeCode = Guid.NewGuid().ToString().Substring(0, 8);

            //将注册信息存入缓存（有效期10分钟）
            var registerData = new RegisterCacheData
            {
                VipPhone = vipInfoDTO.VipPhone,
                VipEmail = vipInfoDTO.VipEmail,
                VipName = vipInfoDTO.VipName,
                Gender = vipInfoDTO.Gender,
                VipPassword = vipInfoDTO.VipPassword,
                RealName = vipInfoDTO.VipName,
                ActiveCode = activeCode
            };
            memoryCache.Set(cacheKey, registerData, TimeSpan.FromMinutes(10));

            //发送激活邮件
            await SendActiveEmail(vipInfoDTO, activeCode);

            return true;
        }
        /// <summary>
        /// 发送激活邮件
        /// </summary>
        /// <param name="vipInfoDTO"></param>
        /// <param name="activeCode"></param>
        /// <returns></returns>
        private async Task SendActiveEmail(VipInfoDTO vipInfoDTO, string activeCode)
        {
            string backendUrl = configuration["AppSettings:BackendUrl"];
            string frontendUrl = configuration["AppSettings:FrontendUrl"];

            string activeLink = $"{backendUrl}/api/vipInfos/ActiveAccount?phone={vipInfoDTO.VipPhone}&email={vipInfoDTO.VipEmail}&activeCode={activeCode}";

            // 构建邮件内容
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'><title>账号激活</title></head>
<body>
    <div style='max-width:500px;margin:0 auto;padding:20px;'>
        <h2>欢迎注册智建中心健身房</h2>
        <p>尊敬的 ");
            sb.Append(vipInfoDTO.VipName);
            sb.Append(@" 您好：</p>
        <p>请点击以下链接激活您的账号（10分钟内有效）：</p>
        <p><a href='");
            sb.Append(activeLink);
            sb.Append(@"'>点击激活账号</a></p>
        <p>如果链接无法点击，请复制以下地址到浏览器打开：</p>
        <p style='color:#666;font-size:12px;'>");
            sb.Append(activeLink);
            sb.Append(@"</p>
        <hr>
        <p style='color:#999;font-size:12px;'>本邮件由系统自动发送，请勿回复</p>
    </div>
</body>
</html>");

            var mailBox = new MailBox()
            {
                Body = sb.ToString(),
                IsHtml = true,
                Subject = "账号激活",
                To = new List<string>() { vipInfoDTO.VipEmail }
            };
            MailQueueProvider.EnqueueMailBox(mailBox);
            await Task.CompletedTask;
        }
    }
}
