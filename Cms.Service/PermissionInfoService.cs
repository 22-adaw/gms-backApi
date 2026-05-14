using Gms.Common;
using Gms.Entity;
using Gms.Entity.DTO;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.EntityFrameworkCore;
using Gms.IRepository;
using Gms.IService;
using Gms.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Service
{
    public class PermissionInfoService : BaseService<PermissionInfo>, IPermissionInfoService
    {
        private readonly IPermissionInfoRepository permissionInfoRepository;
        private readonly IPermissionApiRepository permissionApiRepository;
        private readonly IPermissionMenuRepository permissionMenuRepository;
        private readonly IPermissionPointRepository permissionPointRepository;
        private readonly IPermissionDeleteApprovalRepository permissionDeleteApprovalRepository;
        private readonly IPermissionDeleteApprovalStepRepository permissionDeleteApprovalStepRepository;
        private readonly IUserInfoRepository userInfoRepository;
        private readonly MyDbContext myDbContext;
        private readonly IConfiguration configuration;
        //private readonly object _configLock = new object();
        private readonly SemaphoreSlim _configSemaphore = new SemaphoreSlim(1, 1);
        private readonly IServiceScopeFactory serviceScopeFactory;

        public PermissionInfoService(IPermissionInfoRepository permissionInfoRepository, IPermissionApiRepository permissionApiRepository, IPermissionMenuRepository permissionMenuRepository, IPermissionPointRepository permissionPointRepository, IPermissionDeleteApprovalRepository permissionDeleteApprovalRepository, IPermissionDeleteApprovalStepRepository permissionDeleteApprovalStepRepository, IUserInfoRepository userInfoRepository, MyDbContext myDbContext, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            base.Repository = permissionInfoRepository;
            this.permissionInfoRepository = permissionInfoRepository;
            this.permissionApiRepository = permissionApiRepository;
            this.permissionMenuRepository = permissionMenuRepository;
            this.permissionPointRepository = permissionPointRepository;
            this.permissionDeleteApprovalRepository = permissionDeleteApprovalRepository;
            this.permissionDeleteApprovalStepRepository = permissionDeleteApprovalStepRepository;
            this.userInfoRepository = userInfoRepository;
            this.myDbContext = myDbContext;
            this.configuration = configuration;
            this.serviceScopeFactory = serviceScopeFactory;
        }
        /// <summary>
        /// 新增权限具体实现
        /// </summary>
        /// <param name="permissionDTO"></param>
        /// <returns></returns>
        public Task<bool> AddPermission(PermissionDTO permissionDTO)
        {
            // 创建基础权限模型对象
            PermissionInfo permissionInfo = new PermissionInfo();
            permissionInfo.ParentId = permissionDTO.ParentId;
            permissionInfo.PermissionCode = permissionDTO.PermissionCode;
            permissionInfo.PermissionName = permissionDTO.PermissionName;
            permissionInfo.PermissionType = permissionDTO.PermissionType;
            permissionInfo.PermissionDescription = permissionDTO.PermissionDescription;
            permissionInfo.CreateDate = DateTime.Now;
            permissionInfo.EditDate = DateTime.Now;
            permissionInfo.IsDeleted = false;
            permissionInfoRepository.AddEntity(permissionInfo); // 注意：这里需要对PermissionInfo这个基础权限模型的信息进行保存
            bool result = false;
            switch (permissionInfo.PermissionType)
            {
                // 判断是否是菜单权限
                case (int)PermissionTypeEnum.PermissionMenu:
                    // 创建菜单权限模型对象
                    PermissionMenu permissionMenu = new PermissionMenu();
                    permissionMenu.MenuIcon = permissionDTO.MenuIcon;
                    permissionMenu.MenunOrder = permissionDTO.MenunOrder;
                    permissionMenu.CreateDate = DateTime.Now;
                    permissionMenu.EditDate = DateTime.Now;
                    permissionMenu.IsDeleted = false;
                    // 这里需要把基础permissionInfo模型对象赋值给permissionMenu中的PermissionInfo，才能完成一对一的操作
                    permissionMenu.PermissionInfo = permissionInfo;
                    // 把菜单权限模型对象添加到DbContext中
                    this.permissionMenuRepository.AddEntity(permissionMenu);
                    result = true;
                    break;
                // 功能权限的判断
                case (int)PermissionTypeEnum.PermissionPoint:
                    // 创建功能权限模型对象
                    PermissionPoint point = new PermissionPoint();
                    point.PointClass = permissionDTO.PointClass;
                    point.PointIcon = permissionDTO.PointIcon;
                    point.PointStatus = 1;
                    point.CreateDate = DateTime.Now;
                    point.EditDate = DateTime.Now;
                    point.IsDeleted = false;
                    point.PermissionInfo = permissionInfo; // 注意
                    this.permissionPointRepository.AddEntity(point); // 注意
                    result = true;
                    break;
                // 判断是否是Api权限
                case (int)PermissionTypeEnum.PermissionApi:
                    // 创建Api权限模型对象
                    PermissionApi permissionApi = new PermissionApi();
                    permissionApi.ApiMethod = permissionDTO.ApiMethod;
                    permissionApi.ApiUrl = permissionDTO.ApiUrl;
                    permissionApi.CreateDate = DateTime.Now;
                    permissionApi.EditDate = DateTime.Now;
                    permissionApi.IsDeleted = false;
                    permissionApi.PermissionInfo = permissionInfo;  // 注意
                    this.permissionApiRepository.AddEntity(permissionApi); // 注意
                    result = true;
                    break;
                default:

                    break;


            }
            return Task.FromResult(result);

        }
        /// <summary>
        /// 配置密码修改邮件发送
        /// </summary>
        /// <param name="sendDeletePwdDTO"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> HandleSendDeletePwd(SendDeletePwdDTO sendDeletePwdDTO)
        {
            StringBuilder sb = new StringBuilder();
            string subject = "";
            if (sendDeletePwdDTO.IsSuccess)
            {
                //重置成功邮件
                subject = "【系统安全通知】权限删除专用密码已重置";
                sb.Append("<p>管理员您好：</p>");
                sb.Append("<p>权限删除专用密码已自动重置。</p>");
                sb.Append($"<p><strong>新密码：{sendDeletePwdDTO.Password}</strong></p>");
                sb.Append("<p>此邮件为绝密文件，请勿将密码写在便利贴上贴在屏幕旁边，建议阅后请立即销毁！</p>");
                sb.Append("<p>此为系统自动发送邮件</p>");
            }
            else
            {
                //重置失败邮件 
                subject = "【系统异常通知】权限删除密码重置失败";
                sb.Append("<p>管理员您好：</p>");
                sb.Append("<p>权限删除专用密码重置失败。</p>");
                sb.Append("<p>请立即检查系统配置文件。</p>");
                sb.Append("<p>此为系统自动发送邮件</p>");
            }
            //获取所有管理员邮箱
            var emails = sendDeletePwdDTO.Users
                .Where(u => !string.IsNullOrEmpty(u.UserEmail))
                .Select(u => u.UserEmail)
                .ToList();
            MailBox mailBox = new MailBox()
            {
                Body = sb.ToString(),
                IsHtml = true,
                Subject = subject,
                To = emails
            };
            MailQueueProvider.EnqueueMailBox(mailBox);
            return Task.FromResult(true);

        }
        /// <summary>
        /// 触发审批，审批记录插入，审批细节初始化
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> HandleTriggerApproval(StartApproveDTO startApproveDTO)
        {
            //查出管理员个数
            var adminsList = await userInfoRepository.LoadEntities(u => u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(u => u.RoleInfos).Where(u => u.RoleInfos.Any(r => r.RoleName.Contains("管理员"))).ToListAsync();
            if (adminsList.Count == 0)
            {
                throw new Exception("未找到任何管理员");
            }

            //创建审批记录
            PermissionDeleteApproval permissionDeleteApproval = new PermissionDeleteApproval()
            {
                RequestId = GenerateRequestId(),
                RequesterUserId = startApproveDTO.Requester.Id,
                RequesterName = startApproveDTO.Requester.UserName,
                TargetPermissionId = startApproveDTO.RequestedPermission.Id,
                TargetPermissionName = startApproveDTO.RequestedPermission.PermissionName,
                ApprovalStatus = Convert.ToInt32(ApprovalStatusEnum.UnApproved),
                CurrentStep = 1,
                TotalSteps = adminsList.Count,
                ExpireTime = DateTime.Now.AddHours(24),
                IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal),
                CreateDate = DateTime.Now,
            };
            using (var transaction = await myDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await permissionDeleteApprovalRepository.AddEntity(permissionDeleteApproval);
                    int v1 = await permissionDeleteApprovalRepository.SaveChangesAsync();
                    if (v1 <= 0)
                    {
                        //保存失败，事务回滚
                        await transaction.RollbackAsync();
                        return false;
                    }

                    //随机打乱顺序
                    var randomAdmins = adminsList.OrderBy(x => Guid.NewGuid()).ToList();
                    //新建审批细节
                    for (int i = 0; i < randomAdmins.Count; i++)
                    {
                        string token = Guid.NewGuid().ToString("N");
                        PermissionDeleteApprovalStep step = new PermissionDeleteApprovalStep()
                        {
                            ApprovalId = permissionDeleteApproval.Id,
                            StepOrder = i + 1,
                            AdminUserId = randomAdmins[i].Id,
                            AdminName = randomAdmins[i].RealName,
                            AdminEmail = randomAdmins[i].UserEmail,
                            ApprovalToken = token,
                            TokenExpireTime = DateTime.Now.AddHours(24),
                            IsApproved = Convert.ToInt32(AdminApproveEnum.UnApprove),
                            IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal),
                        };
                        await permissionDeleteApprovalStepRepository.AddEntity(step);
                    }
                    int v2 = await permissionDeleteApprovalStepRepository.SaveChangesAsync();
                    if (v2 <= 0)
                    {
                        //保存失败，事务回滚
                        await transaction.RollbackAsync();
                        return false;
                    }
                    //事务提交
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    //发生异常，回滚事务
                    await transaction.RollbackAsync();
                    throw;
                }
            }

        }
        /// <summary>
        /// 生成业务编号
        /// </summary>
        /// <returns></returns>
        private string GenerateRequestId()
        {
            return $"DEL_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }
        /// <summary>
        /// 分页获取权限信息
        /// </summary>
        /// <param name="permissionSearch"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public IQueryable<PermissionInfo> LoadSearchPageEntities(PermissionSearch permissionSearch, bool isDeleted)
        {
            var temp = permissionInfoRepository.LoadEntities(p => p.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal));
            if (!string.IsNullOrEmpty(permissionSearch.PermissionName))
            {
                temp = temp.Where(t => t.PermissionName.Contains(permissionSearch.PermissionName));
            }
            if (!string.IsNullOrEmpty(permissionSearch.PermissionDescription))
            {
                temp = temp.Where(t => t.PermissionDescription.Contains(permissionSearch.PermissionDescription));
            }
            permissionSearch.TotalCount = temp.Count();
            int skip = (permissionSearch.PageIndex - 1) * permissionSearch.PageSize;
            int take = permissionSearch.PageSize;


            temp = !permissionSearch.Order
                ? temp.OrderBy(a => a.Id).ToList().Skip(skip).Take(take).AsQueryable()
                : temp.OrderByDescending(a => a.Id).ToList().Skip(skip).Take(take).AsQueryable();
            return temp;
        }
        /// <summary>
        /// 写入配置文件(.NET官方推荐的异步并发安全文件写入方案)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> UpdateConfig(string key, string value)
        {
            await _configSemaphore.WaitAsync();
            try
            {
                string configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

                // 异步文件操作
                string json = await File.ReadAllTextAsync(configPath);
                JObject jObj = JObject.Parse(json);

                var keys = key.Split(':');
                JToken current = jObj;

                for (int i = 0; i < keys.Length - 1; i++)
                {
                    if (current[keys[i]] == null)
                        current[keys[i]] = new JObject();
                    current = current[keys[i]];
                }

                current[keys.Last()] = value;
                string formattedJson = jObj.ToString(Formatting.Indented);

                await File.WriteAllTextAsync(configPath, formattedJson);
                return true;
            }
            catch (Exception ex)
            {
                // 日志记录
                return false;
            }
            finally
            {
                _configSemaphore.Release();
            }
        }
        /// <summary>
        /// 发送审批邮件具体实现
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task HandleSendEmail(PermissionDeleteApproval permissionDeleteApproval)
        {
            await Task.Run(async () =>
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        //从新作用域中获取仓储
                        var stepRepository = scope.ServiceProvider.GetRequiredService<IPermissionDeleteApprovalStepRepository>();

                        var firstStep = permissionDeleteApproval.Steps?.OrderBy(s => s.StepOrder).FirstOrDefault();
                        if (firstStep == null) return;

                        string baseUrl = configuration["AppSettings:BackendUrl"]!;
                        string approveLink = $"{baseUrl}/api/permissions/ProcessApproval?token={firstStep.ApprovalToken}&approved=true";
                        string rejectLink = $"{baseUrl}/api/permissions/ProcessApproval?token={firstStep.ApprovalToken}&approved=false";
                        string subject = $"【权限删除审批】第{firstStep.StepOrder}/{permissionDeleteApproval.TotalSteps}位审批人";
                        string body = BuildEmailBody(firstStep, permissionDeleteApproval, approveLink, rejectLink);

                        var mailBox = new MailBox
                        {
                            Body = body,
                            IsHtml = true,
                            Subject = subject,
                            To = new List<string> { firstStep.AdminEmail }
                        };
                        MailQueueProvider.EnqueueMailBox(mailBox);

                        // 更新通知状态
                        firstStep.NotificationSent = true;
                        firstStep.NotificationTime = DateTime.Now;
                        await stepRepository.UpdateEntity(firstStep);
                        await stepRepository.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"发送邮件失败: {ex.Message}");
                    }
                }
            });
        }
        /// <summary>
        /// 构建邮件正文
        /// </summary>
        /// <param name="step"></param>
        /// <param name="permissionDeleteApproval"></param>
        /// <param name="approveLink"></param>
        /// <param name="rejectLink"></param>
        /// <returns></returns>
        private string BuildEmailBody(PermissionDeleteApprovalStep step, PermissionDeleteApproval permissionDeleteApproval, string approveLink, string rejectLink)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<div style='font-family: Arial, sans-serif; padding: 20px; max-width: 600px; margin: 0 auto;'>");
            sb.AppendLine("  <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 20px; border-radius: 10px 10px 0 0;'>");
            sb.AppendLine("    <h2 style='color: white; margin: 0;'>权限删除审批请求</h2>");
            sb.AppendLine("  </div>");
            sb.AppendLine("  <div style='background: #f8f9fa; padding: 20px; border: 1px solid #dee2e6; border-top: none; border-radius: 0 0 10px 10px;'>");
            sb.AppendLine($"    <p style='font-size: 16px;'>您好 <strong>{step.AdminName}</strong>：</p>");
            sb.AppendLine("    <div style='background: white; padding: 15px; border-radius: 8px; margin: 15px 0; border-left: 4px solid #dc3545;'>");
            sb.AppendLine($"      <p><strong>申请人：</strong> {permissionDeleteApproval.RequesterName}</p>");
            sb.AppendLine($"      <p><strong>申请时间：</strong> {permissionDeleteApproval.CreateDate:yyyy-MM-dd HH:mm:ss}</p>");
            sb.AppendLine($"      <p><strong>要删除的权限：</strong> <span style='color: #dc3545; font-weight: bold;'>{permissionDeleteApproval.TargetPermissionName}</span></p>");
            sb.AppendLine($"      <p><strong>请求ID：</strong> {permissionDeleteApproval.RequestId}</p>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <div style='background: #e9ecef; padding: 10px; border-radius: 8px; margin: 15px 0;'>");
            sb.AppendLine($"      <p><strong>审批进度：</strong> 第 {step.StepOrder} / {permissionDeleteApproval.TotalSteps} 位审批人</p>");
            sb.AppendLine($"      <p><strong>超时时间：</strong> {step.TokenExpireTime:yyyy-MM-dd HH:mm:ss}</p>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <div style='margin: 30px 0; text-align: center;'>");
            sb.AppendLine($"      <a href='{approveLink}' style='background-color: #28a745; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin-right: 15px;'>✓ 同意删除</a>");
            sb.AppendLine($"      <a href='{rejectLink}' style='background-color: #dc3545; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px;'>✗ 驳回请求</a>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <hr />");
            sb.AppendLine("    <div style='color: #6c757d; font-size: 12px;'>");
            sb.AppendLine("      <p><strong>注意事项：</strong></p>");
            sb.AppendLine("      <ul>");
            sb.AppendLine("        <li>同意后，将自动通知下一位审批人</li>");
            sb.AppendLine("        <li>驳回后，整个审批流程将立即关闭</li>");
            sb.AppendLine("      </ul>");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }
        /////已废弃
        //public Task<bool> HandleSendEmailToRequester()
        //{
        //    throw new NotImplementedException();
        //}
        /// <summary>
        /// 处理审批，生成结果
        /// </summary>
        /// <param name="token"></param>
        /// <param name="approved"></param>
        /// <returns></returns>
        public async Task<ApprovalProcessResult> ProcessApproval(string token, bool approved)
        {
            //查找审批步骤
            var step = await permissionDeleteApprovalStepRepository.LoadEntities(s => s.ApprovalToken == token).Include(s => s.Approval).FirstOrDefaultAsync();

            if (step == null)
                return new ApprovalProcessResult { Success = false, Message = "无效的审批链接" };

            var approval = step.Approval;

            //各种校验
            if (step.IsApproved > 0)
                return new ApprovalProcessResult { Success = false, Message = "该链接已被使用" };

            if (step.StepOrder != approval.CurrentStep)
                return new ApprovalProcessResult { Success = false, Message = "还未轮到您审批" };

            if (DateTime.Now > step.TokenExpireTime)
                return new ApprovalProcessResult { Success = false, Message = "链接已过期" };

            using var transaction = await myDbContext.Database.BeginTransactionAsync();

            try
            {
                //记录当前人审批结果
                step.IsApproved = approved ?Convert.ToInt32(AdminApproveEnum.Approved): Convert.ToInt32(AdminApproveEnum.Rejected);
                step.ApprovalTime = DateTime.Now;
                await permissionDeleteApprovalStepRepository.UpdateEntity(step);

                if (!approved)
                {
                    // 驳回：流程关闭
                    approval.ApprovalStatus = (int)ApprovalStatusEnum.Rejected;
                    approval.CompleteTime = DateTime.Now;
                    var permission= await permissionInfoRepository.LoadEntities(p => p.Id == approval.TargetPermissionId && p.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal) && p.IsDeleteApproving == Convert.ToInt32(ApprovalStatusEnum.Approving)).FirstOrDefaultAsync();
                    permission.IsDeleteApproving = Convert.ToInt32(ApprovalStatusEnum.UnApproved);
                    await permissionInfoRepository.UpdateEntity(permission);
                    await permissionDeleteApprovalRepository.UpdateEntity(approval);
                    await myDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await SendResultEmailToRequester(approval, false);
                    return new ApprovalProcessResult { Success = false, Message = "审批已驳回" };
                }

                //同意：判断是否是最后一人
                if (step.StepOrder >= approval.TotalSteps)
                {
                    // 最后一人：执行删除
                    approval.ApprovalStatus = (int)ApprovalStatusEnum.Approved;
                    approval.CompleteTime = DateTime.Now;
                    await permissionDeleteApprovalRepository.UpdateEntity(approval);
                    var permission = await permissionInfoRepository
                        .LoadEntities(p => p.Id == approval.TargetPermissionId)
                        .FirstOrDefaultAsync();

                    if (permission != null)
                    {
                        permission.IsDeleted = Convert.ToBoolean(DelFlagEnum.Deleted);
                        permission.IsDeleteApproving = 2;
                        await permissionInfoRepository.UpdateEntity(permission);
                    }
                    await myDbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    //发送审批结果邮件给发起者
                    await SendResultEmailToRequester(approval, true);
                    return new ApprovalProcessResult { Success = true, Message = "权限已成功删除", IsCompleted = true };
                }

                //不是最后一人：流转到下一步
                approval.CurrentStep = step.StepOrder + 1;
                await permissionDeleteApprovalRepository.UpdateEntity(approval);
                await myDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                //异步发送邮件给下一个人
                SendNextApprovalEmail(approval.Id, step.StepOrder);

                return new ApprovalProcessResult
                {
                    Success = true,
                    Message = $"审批通过，已通知下一位审批人",
                    CurrentStep = approval.CurrentStep,
                    TotalSteps = approval.TotalSteps,
                    IsCompleted = false
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                return new ApprovalProcessResult { Success = false, Message = "系统异常" };
            }
        }
        /// <summary>
        /// 给下一位审批人发送邮件
        /// </summary>
        /// <param name="approvalId"></param>
        /// <param name="currentStepOrder"></param>
        private void SendNextApprovalEmail(int approvalId, int currentStepOrder)
        {
            Task.Run(async () =>
            {
                //新建局部作用域
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        //从新作用域中获取仓储（全新的DbContext）
                        var stepRepository = scope.ServiceProvider.GetRequiredService<IPermissionDeleteApprovalStepRepository>();
                        var approvalRepository = scope.ServiceProvider.GetRequiredService<IPermissionDeleteApprovalRepository>();

                        // 获取下一个审批步骤
                        var nextStep = await stepRepository
                            .LoadEntities(s => s.ApprovalId == approvalId && s.StepOrder == currentStepOrder + 1)
                            .FirstOrDefaultAsync();

                        if (nextStep == null) return;

                        var approval = await approvalRepository
                            .LoadEntities(a => a.Id == approvalId)
                            .FirstOrDefaultAsync();

                        if (approval == null) return;

                        // 重置Token过期时间
                        nextStep.TokenExpireTime = DateTime.Now.AddHours(24);
                        await stepRepository.UpdateEntity(nextStep);
                        await stepRepository.SaveChangesAsync();

                        string baseUrl = configuration["AppSettings:BackendUrl"]!;
                        string approveLink = $"{baseUrl}/api/permissions/ProcessApproval?token={nextStep.ApprovalToken}&approved=true";
                        string rejectLink = $"{baseUrl}/api/permissions/ProcessApproval?token={nextStep.ApprovalToken}&approved=false";

                        string body = $@"
                    <div style='font-family:Arial;'>
                        <h2>权限删除审批</h2>
                        <p>您好 {nextStep.AdminName}：</p>
                        <p>{approval.RequesterName} 请求删除权限：<strong>{approval.TargetPermissionName}</strong></p>
                        <p>当前进度：第 {nextStep.StepOrder}/{approval.TotalSteps} 位</p>
                        <p>
                            <a href='{approveLink}' style='background:green;color:white;padding:10px 20px;text-decoration:none;'>同意删除</a>
                            <a href='{rejectLink}' style='background:red;color:white;padding:10px 20px;text-decoration:none;'>驳回</a>
                        </p>
                    </div>";

                        var mailBox = new MailBox
                        {
                            Body = body,
                            IsHtml = true,
                            Subject = $"【权限删除审批】第{nextStep.StepOrder}/{approval.TotalSteps}位审批人",
                            To = new List<string> { nextStep.AdminEmail }
                        };

                        MailQueueProvider.EnqueueMailBox(mailBox);

                        // 更新通知状态
                        nextStep.NotificationSent = true;
                        nextStep.NotificationTime = DateTime.Now;
                        await stepRepository.UpdateEntity(nextStep);
                        await stepRepository.SaveChangesAsync();

                        Console.WriteLine($"✅ 已发送邮件给：{nextStep.AdminName}，第{nextStep.StepOrder}位");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"发送邮件失败: {ex.Message}");
                    }
                }
            });
        }
        /// <summary>
        /// 发送审批结果邮件给发起人
        /// </summary>
        /// <param name="approval"></param>
        /// <param name="isPassed"></param>
        /// <returns></returns>
        private async Task SendResultEmailToRequester(PermissionDeleteApproval approval, bool isPassed)
        {
            try
            {
                //获取申请人信息（根据 RequesterUserId）
                var requester = await userInfoRepository
                    .LoadEntities(u => u.Id == approval.RequesterUserId)
                    .FirstOrDefaultAsync();

                if (requester == null || string.IsNullOrEmpty(requester.UserEmail))
                    return;

                //邮件标题、内容
                string title = isPassed
                    ? $"【审批通过】权限删除申请已通过"
                    : $"【审批驳回】权限删除申请已驳回";

                string content = $@"
                    <p>您好 {requester.RealName}：</p>
                    <p>您提交的【权限删除申请】已处理完成。</p>
                    <p><strong>权限名称：</strong>{approval.TargetPermissionName}</p>
                    <p><strong>申请结果：</strong>{(isPassed ? "审批通过，权限已删除" : "审批驳回")}</p>
                    <p>系统自动发送，无需回复。</p>";

                //发送邮件
                MailBox mail = new MailBox
                {
                    To = new List<string> { requester.UserEmail },
                    Subject = title,
                    Body = content,
                    IsHtml = true
                };

                MailQueueProvider.EnqueueMailBox(mail);
            }
            catch
            {
                // 邮件异常不影响主流程
            }
        }
    }
}