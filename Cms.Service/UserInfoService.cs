using Gms.Common;
using Gms.Entity;
using Gms.Entity.DTO;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.IRepository;
using Gms.IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Service
{
    public class UserInfoService :BaseService<UserInfo>,IUserInfoService
    {
        private readonly IUserInfoRepository userInfoRepository;
        private readonly IRoleInfoRepository roleInfoRepository;
        private readonly IPermissionInfoRepository permissionInfoRepository;
        private readonly IMemoryCache memoryCache;
        private readonly IConfiguration configuration;

        public UserInfoService(IUserInfoRepository userInfoRepository, IRoleInfoRepository roleInfoRepository, IPermissionInfoRepository permissionInfoRepository, IMemoryCache memoryCache, IConfiguration configuration) { 
         base.Repository = userInfoRepository;
            this.userInfoRepository = userInfoRepository;
            this.roleInfoRepository = roleInfoRepository;
            this.permissionInfoRepository = permissionInfoRepository;
            this.memoryCache = memoryCache;
            this.configuration = configuration;

        }
        /// <summary>
        /// 实现员工权限获取（权限编码）
        /// </summary>
        /// <param name="userInfo">员工信息</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Dictionary<string, List<string>>> GetUserPermission(UserInfo userInfo)
        {
            //执行完这一步后，该角色下的所有角色信息被查询出来放到了userInfo.RoleInfos List集合中
            await userInfoRepository.LoadEntities(u => u.Id == userInfo.Id).Include(u => u.RoleInfos).ToListAsync();
            
            var userRoles = userInfo.RoleInfos;//存储当前登录用户的角色信息
            List<string> menus = new List<string>();
            List<string> points = new List<string>();
            List<string> Apis = new List<string>();
            var permissionCodeDict = new Dictionary<string, List<string>>();
            //查询每个角色具有的权限编码信息
            foreach (var userRole in userRoles)
            {
                await roleInfoRepository.LoadEntities(r => r.Id == userRole.Id).Include(r => r.PermissionInfos).ToListAsync();
                var permissionInfos = userRole.PermissionInfos;
                foreach (var permissionInfo in permissionInfos)
                {
                    switch(permissionInfo.PermissionType)
                    {
                        case (int)PermissionTypeEnum.PermissionMenu:
                            //菜单权限
                            menus.Add(permissionInfo.PermissionCode!);
                            break;
                        case (int)PermissionTypeEnum.PermissionPoint:
                            //功能权限
                            points.Add(permissionInfo.PermissionCode!);
                            break;
                        case (int)PermissionTypeEnum.PermissionApi:
                            //Api权限
                            Apis.Add(permissionInfo.PermissionCode!);
                            break;
                        default:
                            break;
                    }
                }
                
            }
            permissionCodeDict.Add("menus", menus.Distinct().ToList());
            permissionCodeDict.Add("points", points.Distinct().ToList());
            permissionCodeDict.Add("Apis", Apis.Distinct().ToList());
            return permissionCodeDict;
        }
        /// <summary>
        /// 实现员工信息分页获取
        /// </summary>
        /// <param name="userSearch">分页条件</param>
        /// <param name="isDeleted">是否已被删除</param>
        /// <returns></returns>
        public IQueryable<UserInfo> LoadPageEntities(UserSearch userSearch, bool isDeleted)
        {
            var temp = userInfoRepository.LoadEntities(u => u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)&&u.IsClient==Convert.ToBoolean(ClientCheckEnum.Employee));
            if(!string.IsNullOrEmpty(userSearch.UserName))
            {
                temp = temp.Where(a => a.UserName.Contains(userSearch.UserName));
            }
            if(!string.IsNullOrEmpty(userSearch.RealName))
            {
                temp = temp.Where(a => a.RealName.Contains(userSearch.RealName));
            }
            if (!string.IsNullOrEmpty(userSearch.UserPhone))
            {
                temp = temp.Where(a => a.UserPhone.Contains(userSearch.UserPhone));
            }
            if(!string.IsNullOrEmpty(userSearch.DepartmentName))
            { 
                temp = temp.Where(a => a.Department.DepartmentName.Contains(userSearch.DepartmentName));
            }
            if(!string.IsNullOrEmpty(userSearch.UserEmail))
            {
                temp = temp.Where(a => a.UserEmail.Contains(userSearch.UserEmail));
            }
            userSearch.TotalCount = temp.Count();
           
            int skip = (userSearch.PageIndex - 1) * userSearch.PageSize;
            int take = userSearch.PageSize;


            temp = userSearch.Order
                 ? temp.OrderBy(a => a.Id).Include(u => u.Department).Skip(skip).Take(take)
                 : temp.OrderByDescending(a => a.Id).Include(u => u.Department).Skip(skip).Take(take);
            return temp;
        }
        /// <summary>
        /// 完成员工角色分配
        /// </summary>
        /// <param name="userInfo">要分配角色的员工</param>
        /// <param name="list">要分配的角色编号</param>
        /// <returns></returns>
        public async Task<bool> SetUserRoles(UserInfo userInfo, List<long> list)
        {
            //获取了员工具有的角色信息
            await userInfoRepository.LoadEntities(u => u.Id == userInfo.Id).Include(u => u.RoleInfos).ToListAsync();
            //清空集合
            userInfo.RoleInfos.Clear();
            if (list.Count == 1 && list[0]==0)
            {
                return true;
            }
            var roleInfos = await roleInfoRepository.LoadEntities(r =>list.Contains(r.Id) && r.IsDeleted == false).ToListAsync();
            //foreach (var roleId in list)
            //{
            //    var roleInfo = await roleInfoRepository.LoadEntities(r => r.Id == roleId && r.IsDeleted == false).FirstOrDefaultAsync();
            //    userInfo.RoleInfos.Add(roleInfo);
            //}
            foreach (var roleInfo in roleInfos)
            {
                userInfo.RoleInfos.Add(roleInfo);
            }
            return true;
        }
        #region 已废弃代码
        /// <summary>
        /// 用户激活账号邮件通知(已废弃)
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        //public Task<bool> HandleActive(UserInfo userInfo)
        //{
        //    string backendUrl = configuration["AppSettings:BackendUrl"];
        //    var activeCode = Guid.NewGuid().ToString().Substring(0, 8);
        //    memoryCache.Set("user" + userInfo.UserEmail, activeCode, absoluteExpiration: DateTime.Now.AddMinutes(10));
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("<!DOCTYPE html>");
        //    sb.Append("<html lang='zh-CN'>");
        //    sb.Append("<head>");
        //    sb.Append("<meta charset='UTF-8'>");
        //    sb.Append("<meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        //    sb.Append("<title>账号激活通知</title>");
        //    sb.Append("</head>");

        //    sb.Append("<body style='margin:0; padding:30px; font-family:Microsoft YaHei, SimSun, Arial;");
        //    sb.Append("background-color:#e9ecef; font-size:14px; color:#212529; line-height:1.8;'>");

        //    sb.Append("<div style='max-width:680px; margin:0 auto; background:#ffffff;");
        //    sb.Append("padding:35px; border-radius:6px; box-shadow:0 4px 12px rgba(0,0,0,0.07);'>");

        //    sb.Append("<div style='text-align:center; margin-bottom:30px; padding-bottom:20px; border-bottom:1px solid #dee2e6;'>");
        //    sb.Append("<div style='font-size:26px; font-weight:bold; color:#0F4C81;'> 健身房管理系统</div>");
        //    sb.Append("<p style='margin:6px 0 0; color:#6c757d; font-size:13px;'>官方账号安全通知邮件</p>");
        //    sb.Append("</div>");

        //    sb.Append($"<p><strong>尊敬的 {userInfo.RealName} 用户：</strong></p>");

        //    sb.Append("<p>您好！</p>");
        //    sb.Append("<p>恭喜您已完成健身房管理系统会员账号注册。</p>");
        //    sb.Append("<p>系统检测到本次为新设备、陌生环境登录操作，为保障您的账户信息安全，需要您完成邮箱激活验证后方可正常使用系统权限。</p>");

        //    sb.Append("<p style='text-align:center; margin:35px 0;'>");
        //    sb.Append($"<a href='{backendUrl}/api/vipInfos/activeAccount?email={userInfo.UserEmail}&activeCode={activeCode}' ");
        //    sb.Append("style='display:inline-block; padding:11px 32px; background:#0F4C81; color:#ffffff;");
        //    sb.Append("border-radius:4px; text-decoration:none; font-weight:500; font-size:15px;'>");
        //    sb.Append("确认激活账号");
        //    sb.Append("</a>");
        //    sb.Append("</p>");

        //    sb.Append("<p style='color:#495057;'><strong>⚠️ 时效提示：</strong>本次激活链接有效时长为 10 分钟，请您尽快完成激活，超时链接失效需重新申请。</p>");

        //    sb.Append("<p>为保护用户隐私安全，系统不会采集、存储您的真实地理位置信息，相关地域标识仅用于安全风控校验。</p>");

        //    sb.Append("<div style='background-color:#f8d7da; border-left:4px solid #842029; padding:14px 18px; margin:25px 0; border-radius:4px;'>");
        //    sb.Append("<p style='margin:0; color:#721c24;'><strong>安全风险预警</strong><br>");
        //    sb.Append("若本次注册、激活操作并非本人发起，说明您的邮箱及账户存在被盗用风险，请您立即修改邮箱密码，并联系系统运维人员核查处理。</p>");
        //    sb.Append("</div>");

        //    sb.Append("<p>本邮件由健身房管理系统后台自动生成发送，无需回复。</p>");
        //    sb.Append("<p>若您在账号使用、激活操作中存在疑问，请联系平台官方客服及运维人员咨询。</p>");

        //    sb.Append("<p style='margin-top:30px;'>感谢您的信任与使用，祝您愉快！</p>");

        //    sb.Append("<hr style='border:none; border-top:1px solid #dee2e6; margin:35px 0;'>");

        //    sb.Append("<div style='text-align:right;'>");
        //    sb.Append("<p style='margin:0 0 6px 0; font-weight:bold;'>健身房管理系统开发团队</p>");
        //    sb.Append($"<p style='margin:0; color:#6c757d; font-size:13px;'>{DateTime.Now.Year}年{DateTime.Now.Month}月{DateTime.Now.Day}日</p>");
        //    sb.Append("</div>");

        //    sb.Append("</div>");
        //    sb.Append("</body>");
        //    sb.Append("</html>");
        //    MailBox mailBox = new MailBox()
        //    {
        //        Body = sb.ToString(),
        //        IsHtml = true,
        //        Subject = "账号激活",
        //        To = new List<string>() { userInfo.UserEmail }
        //    };
        //    MailQueueProvider.EnqueueMailBox(mailBox);
        //    return Task.FromResult(true);
        //}
        #endregion
        /// <summary>
        /// 用户找回密码邮件通知发送
        /// </summary>
        /// <param name="userFindDTO"></param>
        /// <returns></returns>
        public Task<bool> HandleFind(UserFindDTO userFindDTO)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<!DOCTYPE html>");
            sb.Append("<html>");
            sb.Append("<head><meta charset='UTF-8'></head>");

            sb.Append("<body style='font-family:Microsoft Yahei,SimSun,Arial;line-height:1.8;color:#212529;font-size:14px;margin:0;padding:30px;background-color:#e9ecef;'>");

            sb.Append("<div style='max-width:680px;margin:0 auto;background:#ffffff;padding:40px;border-radius:6px;box-shadow:0 4px 12px rgba(0,0,0,0.06);'>");

            sb.Append("<h3 style='margin:0 0 25px 0;color:#0F4C81;font-size:18px;'>健身房管理系统 - 密码找回通知</h3>");

            sb.Append($"<p style='margin:0 0 20px 0;'><strong style='font-size:15px;'>尊敬的 {userFindDTO.RealName} 用户：</strong></p>");

            sb.Append("<p style='margin:0 0 12px 0;'>您好！</p>");
            sb.Append("<p style='margin:0 0 24px 0;'>系统已受理您本次 <strong style='color:#0F4C81;'>密码找回申请</strong>，现将您的账户登录凭证信息发送至您绑定的官方邮箱，请您妥善保管。</p>");

            sb.Append("<div style='background-color:#f8f9fa;border-left:4px solid #b91c1c;padding:18px;margin:28px 0;border-radius:4px;'>");
            sb.Append("<p style='margin:0 0 8px 0;color:#b91c1c;font-weight:bold;'>【账户登录密码】</p>");
            sb.Append($"<h2 style='margin:0;color:#b91c1c;font-size:22px;font-weight:bold;'>{userFindDTO.UserPassword}</h2>");
            sb.Append("</div>");

            sb.Append("<p style='margin:0 0 12px 0;font-weight:bold;color:#333;'>⚠️ 安全重要提醒</p>");
            sb.Append("<ul style='margin:0 0 28px 20px;padding:0;'>");
            sb.Append("<li><strong>此密码由系统随机生成</strong></li>");
            sb.Append("<li>请您登录系统后<strong>第一时间修改初始密码</strong>，请勿使用原始密码长期登录账户。</li>");
            sb.Append("<li>请勿向他人泄露本邮件内容、账户密码，避免账户被盗用、信息泄露风险。</li>");
            sb.Append("<li>本次密码找回操作全程留痕，系统已记录本次申请时间与设备信息。</li>");
            sb.Append("</ul>");

            sb.Append("<div style='background-color:#f8f9fa;border-left:4px solid #495057;padding:18px;margin:28px 0;border-radius:4px;'>");
            sb.Append("<p style='margin:0 0 8px 0;font-weight:bold;'>【非本人操作预警】</p>");
            sb.Append("<p style='margin:0;'>若本次密码找回申请<strong>并非您本人发起</strong>，说明您的账户存在被盗风险，请您立即登录系统修改密码，并联系平台管理员核查账户安全。</p>");
            sb.Append("</div>");

            sb.Append("<p style='margin:0 0 12px 0;'>本邮件由健身房管理系统后台<strong>自动发送</strong>，请勿回复。</p>");
            sb.Append("<p style='margin:0 0 35px 0;'>若您在账户使用、密码修改过程中存在疑问，请通过系统官方客服通道咨询工作人员。</p>");
            sb.Append("<p style='margin-top:30px;'>感谢您的信任与使用，祝您愉快！</p>");
            sb.Append("<hr style='border:none;border-top:1px solid #dee2e6;margin:35px 0;'>");

            sb.Append("<div style='text-align:right;'>");
            sb.Append("<p style='margin:0 0 6px 0;font-weight:bold;'>健身房管理系统开发团队</p>");
            sb.Append($"<p style='margin:0;color:#6c757d;font-size:13px;'>{DateTime.Now.Year}年{DateTime.Now.Month}月{DateTime.Now.Day}日</p>");
            sb.Append("</div>");

            sb.Append("</div>");
            sb.Append("</body>");
            sb.Append("</html>");
            MailBox mailBox = new MailBox()
            {
                Body = sb.ToString(),
                IsHtml = true,
                Subject = "找回密码",
                To = new List<string>() { userFindDTO.UserEmail }
            };
            MailQueueProvider.EnqueueMailBox(mailBox);
            return Task.FromResult(true);
        }
        /// <summary>
        /// 开通员工账号邮件通知
        /// </summary>
        /// <param name="userInDTO"></param>
        /// <returns></returns>
        public Task<bool> HandleCreate(UserInDTO userInDTO)
        {
            StringBuilder sb = new StringBuilder();
            string assert = userInDTO.Gender == 1 ? "先生" : userInDTO.Gender == 0 ? "女士" : "";
            sb.Append("<!DOCTYPE html>");
            sb.Append("<html>");
            sb.Append("<head><meta charset='UTF-8'></head>");

            sb.Append("<body style='font-family:Microsoft Yahei,SimSun,Arial;line-height:1.8;color:#212529;font-size:14px;margin:0;padding:30px;background-color:#e9ecef;'>");

            sb.Append("<div style='max-width:680px;margin:0 auto;background:#ffffff;padding:40px;border-radius:6px;box-shadow:0 4px 12px rgba(0,0,0,0.06);'>");

            sb.Append("<h3 style='margin:0 0 25px 0;color:#0F4C81;font-size:18px;border-bottom:1px solid #dee2e6;padding-bottom:15px;'>健身房管理系统 - 员工账号开通通知</h3>");

            sb.Append($"<p style='margin:0 0 20px 0;'><strong style='font-size:15px;'>尊敬的 {userInDTO.RealName} {assert}：</strong></p>");
            
            sb.Append("<p style='margin:0 0 12px 0;'>您好！</p>");
            sb.Append("<p style='margin:0 0 12px 0;'>恭喜您已正式加入智健中心健身房，成为团队一员。您的专属系统账号现已开通，账户信息如下：</p>");

            sb.Append("<div style='background:#f8f9fa;border-left:4px solid #0F4C81;padding:18px 20px;margin:20px 0;border-radius:4px;'>");
            sb.Append($"<p style='margin:0 0 8px 0;'><strong>用户名：</strong> {userInDTO.UserName}</p>");
            sb.Append($"<p style='margin:0 0 8px 0;'><strong>手机号：</strong> {userInDTO.UserPhone}</p>");
            sb.Append($"<p style='margin:0 0 8px 0;'><strong>初始密码：</strong> <span style='color:#dc2626; font-weight:bold;'>{userInDTO.UserPassword}</span></p>");
            sb.Append($"<p style='margin:0 0 8px 0;'><strong>所属部门：</strong> {userInDTO.DepartmentName}</p>");
            sb.Append("<p style='margin:0;'><strong>登录方式：</strong> 手机号 + 初始密码</p>");
            sb.Append("</div>");

            sb.Append("<p style='margin:0 0 12px 0;'><strong>⚠️ 重要安全提醒：</strong></p>");
            sb.Append("<p style='margin:0 0 12px 0;'>1. 此密码为系统自动生成，首次登录后请<strong style='color:#dc2626;'>立即修改密码</strong>。</p>");
            sb.Append("<p style='margin:0 0 12px 0;'>2. 请妥善保管账号信息，切勿向他人泄露。</p>");
            sb.Append("<p style='margin:0 0 12px 0;'>3. 如账号信息有误，请及时联系管理员处理。</p>");

            sb.Append("<p style='margin:30px 0 0 0;'>欢迎加入智健中心健身房，祝您工作顺利，前程似锦！</p>");

            sb.Append("<hr style='border:none;border-top:1px solid #dee2e6;margin:30px 0;'>");

            sb.Append("<div style='text-align:right;'>");
            sb.Append("<p style='margin:0 0 6px 0;font-weight:bold;'>健身房管理系统开发团队</p>");
            sb.Append($"<p style='margin:0;color:#6c757d;font-size:13px;'>{DateTime.Now.Year}年{DateTime.Now.Month}月{DateTime.Now.Day}日</p>");
            sb.Append("</div>");

            sb.Append("</div>");
            sb.Append("</body>");
            sb.Append("</html>");
            MailBox mailBox = new MailBox()
            {
                Body = sb.ToString(),
                IsHtml = true,
                Subject = "账号开通",
                To = new List<string>() { userInDTO.UserEmail }
            };
            MailQueueProvider.EnqueueMailBox(mailBox);
            return Task.FromResult(true);
        }
        /// <summary>
        /// 验证码邮件通知
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public Task<bool> HandleCode(UserInfo userInfo)
        {
            var code = CodeHelper.GenerateCode();
            memoryCache.Set("client" + userInfo.UserPhone, code, absoluteExpiration: DateTime.Now.AddMinutes(5));
            StringBuilder sb = new StringBuilder();

            sb.Append("<!DOCTYPE html>");
            sb.Append("<html>");
            sb.Append("<head><meta charset='UTF-8'></head>");

            sb.Append("<body style='font-family:Microsoft Yahei,SimSun,Arial;line-height:1.8;color:#212529;font-size:14px;margin:0;padding:30px;background-color:#e9ecef;'>");

            sb.Append("<div style='max-width:680px;margin:0 auto;background:#ffffff;padding:40px;border-radius:6px;box-shadow:0 4px 12px rgba(0,0,0,0.06);'>");

            sb.Append("<h3 style='margin:0 0 25px 0;color:#0F4C81;font-size:18px;border-bottom:1px solid #dee2e6;padding-bottom:15px;'>健身房管理系统 - 验证码通知</h3>");

            sb.Append($"<p style='margin:0 0 20px 0;'><strong style='font-size:15px;'>尊敬的用户：</strong></p>");

            sb.Append("<p style='margin:0 0 12px 0;'>您好！您正在进行【身份验证】操作，本次请求的验证码如下：</p>");

            sb.Append("<div style='background:#f8f9fa;border-left:4px solid #0F4C81;padding:18px 20px;margin:20px 0;border-radius:4px;text-align:center;'>");
            sb.Append($"<p style='margin:0;font-size:24px;font-weight:bold;color:#dc2626;letter-spacing:6px;'>{code}</p>");
            sb.Append("</div>");

            sb.Append("<p style='margin:0 0 12px 0;'><strong>⚠️ 验证码有效期为 5 分钟，请尽快完成验证。</strong></p>");
            sb.Append("<p style='margin:0 0 12px 0;'>1. 验证码仅限本人使用，请勿泄露给他人。</p>");
            sb.Append("<p style='margin:0 0 12px 0;'>2. 若非本人操作，请忽略本邮件，账号信息不会发生变化。</p>");
            sb.Append("<p style='margin:0 0 12px 0;'>3. 为保障账号安全，请勿将验证码转发给他人。</p>");

            sb.Append("<p style='margin:30px 0 0 0;'>感谢您使用健身房管理系统！</p>");

            sb.Append("<hr style='border:none;border-top:1px solid #dee2e6;margin:30px 0;'>");

            sb.Append("<div style='text-align:right;'>");
            sb.Append("<p style='margin:0 0 6px 0;font-weight:bold;'>健身房管理系统开发团队</p>");
            sb.Append($"<p style='margin:0;color:#6c757d;font-size:13px;'>{DateTime.Now.Year}年{DateTime.Now.Month}月{DateTime.Now.Day}日</p>");
            sb.Append("</div>");

            sb.Append("</div>");
            sb.Append("</body>");
            sb.Append("</html>");
            MailBox mailBox = new MailBox()
            {
                Body = sb.ToString(),
                IsHtml = true,
                Subject = "【健身房管理系统】登录临时凭证",
                To = new List<string>() { userInfo.UserEmail }
            };
            MailQueueProvider.EnqueueMailBox(mailBox);
            return Task.FromResult(true);
        }
        /// <summary>
        /// 发送提醒密码修改邮件
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public Task<bool> SendChangePassword(UserInfo userInfo)
        {
            StringBuilder sb = new StringBuilder();
            //邮件正文
            sb.Append("<html><body style='font-family: Arial, sans-serif;'>");
            sb.Append("<div style='max-width: 500px; padding: 20px;'>");
            sb.Append("<p>尊敬的 <strong>" + System.Web.HttpUtility.HtmlEncode(userInfo.RealName ?? userInfo.UserName) + "</strong>，您好！</p>");
            sb.Append("<p>系统检测到您的登录密码为早期明文存储格式，存在安全风险。请登录后尽快前往「个人中心」修改密码，以增强账号安全性。</p>");
            sb.Append("<p>感谢您的配合。</p>");

            sb.Append("<hr style='border:none;border-top:1px solid #dee2e6;margin:30px 0;'>");
            sb.Append("<div style='text-align:right;'>");
            sb.Append("<p style='margin:0 0 6px 0;font-weight:bold;'>健身房管理系统开发团队</p>");
            sb.Append($"<p style='margin:0;color:#6c757d;font-size:13px;'>{DateTime.Now.Year}年{DateTime.Now.Month}月{DateTime.Now.Day}日</p>");
            sb.Append("</div>");
            sb.Append("</div></body></html>");
            MailBox mailBox = new MailBox()
            {
                Body = sb.ToString(),
                IsHtml = true,
                Subject = "【健身房管理系统】密码安全提醒",
                To = new List<string>() { userInfo.UserEmail }
            };
            MailQueueProvider.EnqueueMailBox(mailBox);
            return Task.FromResult(true);
        }
        /// <summary>
        /// 发送密码已加密邮件
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public Task<bool> SendChangedPassword(UserInfo userInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><body style='font-family: Arial;'>");
            sb.Append("<p>尊敬的 <strong>" + System.Web.HttpUtility.HtmlEncode(userInfo.RealName ?? userInfo.UserName) + "</strong>，您好！</p>");
            sb.Append("<p>为了保障您的账号安全，系统已自动将您的登录密码升级为高强度加密存储，您无需进行任何操作。</p>");
            sb.Append("<p>感谢您的支持！</p>");
            sb.Append("<hr style='border:none;border-top:1px solid #dee2e6;margin:20px 0;'>");
            sb.Append("<div style='text-align:right;'>");
            sb.Append("<p style='margin:0;font-weight:bold;'>健身房管理系统开发团队</p>");
            sb.Append($"<p style='margin:0;color:#6c757d;font-size:12px;'>{DateTime.Now:yyyy年MM月dd日}</p>");
            sb.Append("</div></body></html>");
            MailBox mailBox = new MailBox()
            {
                Body = sb.ToString(),
                IsHtml = true,
                Subject = "【健身房管理系统】您的密码已自动升级",
                To = new List<string>() { userInfo.UserEmail }
            };
            MailQueueProvider.EnqueueMailBox(mailBox);
            return Task.FromResult(true);
        }
    }
}
