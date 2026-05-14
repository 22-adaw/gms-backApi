using AutoMapper;
using Gms.Common;
using Gms.Entity;
using Gms.Entity.DTO;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.IService;
using Gms.Service;
using Gms.WebApi.Attributes;
using Gms.WebApi.Models;
using Gms.WebApi.SearchParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Gms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionInfoService permissionInfoService;
        private readonly IMapper mapper;
        private readonly IRoleInfoService roleInfoService;
        private readonly IUserInfoService userInfoService;
        private readonly IConfiguration configuration;
        private readonly IPermissionDeleteApprovalService permissionDeleteApprovalService;
        public PermissionsController(IPermissionInfoService permissionInfoService, IMapper mapper, IRoleInfoService roleInfoService, IUserInfoService userInfoService, IConfiguration configuration, IPermissionDeleteApprovalService permissionDeleteApprovalService)
        {
            this.permissionInfoService = permissionInfoService;
            this.mapper = mapper;
            this.roleInfoService = roleInfoService;
            this.userInfoService = userInfoService;
            this.configuration = configuration;
            this.permissionDeleteApprovalService = permissionDeleteApprovalService;
        }
        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="permissionDTO">前端传过来的权限信息</param>
        /// <returns></returns>
        [HttpPost("CreatePermission/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreatePermission([FromBody]PermissionDTO permissionDTO, [FromRoute]int id)
        {
            if(string.IsNullOrEmpty(permissionDTO.PermissionName)||string.IsNullOrEmpty(permissionDTO.PermissionCode)||string.IsNullOrEmpty(permissionDTO.PermissionDescription))
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "表单请填写完整", Data = null, Code = 400 });
            }
            else
            {
                bool v = await permissionInfoService.AddPermission(permissionDTO);
                if (v)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "权限添加成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "权限添加失败", Data = null, Code = 400 });
                }
            }
        }
        /// <summary>
        /// 获取权限列表数据
        /// </summary>
        /// <param name="permissionParams"></param>
        /// <returns></returns>
        [HttpGet("GetPermission")]
        [Authorize]
        public IActionResult GetPermission([FromQuery]PermissionParams permissionParams)
        {
            var permissionSearch = new PermissionSearch()
            {
                PageIndex=permissionParams.PageIndex,
                PageSize=permissionParams.PageSize,
                PermissionDescription=permissionParams.PermissionDescription,
                PermissionName=permissionParams.PermissionName,
                Order=permissionParams.Order,
                TotalCount=0
                
            };
            var permissions = permissionInfoService.LoadSearchPageEntities(permissionSearch, false).Select(p => new { Id = p.Id, PermissionName = p.PermissionName, PermissionDescription = p.PermissionDescription, PermissionCode = p.PermissionCode, PermissionType = p.PermissionType, ParentId = p.ParentId, IsDeleteApproving=p.IsDeleteApproving}).ToList();
            if(permissions.Count>0)
            {
                return Ok(new ApiResult<object>() { Success = true, Message = "获取权限信息成功", Data = new { Permissions = permissions, Total = permissionSearch.TotalCount }, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到权限信息", Data =null, Code = 404 });
            }
        }
        /// <summary>
        /// 直接返回所有权限数据，不涉及分页
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllPermission")]
        [Authorize]
        public async Task<IActionResult> GetAllPermission()
        {
            var permissionList= await permissionInfoService.LoadEntities(p => p.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Select(p => new { Id = p.Id, PermissionName = p.PermissionName, PermissionDescription = p.PermissionDescription, PermissionCode = p.PermissionCode, PermissionType = p.PermissionType, ParentId = p.ParentId }).ToListAsync();
            if(permissionList!=null)
            {
                return Ok(new ApiResult<object>() { Success = true, Message = "获取所有权限信息成功",Data=permissionList, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到权限信息",Data=null, Code = 404 });
            }
        }
        /// <summary>
        /// 根据id获取当前权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getCurrentPermissionById")]
        [Authorize]
        public async Task<IActionResult> GetCurrentPermissionById([FromQuery]int id)
        {
            //获取当前权限
            var permission= await permissionInfoService.LoadEntities(p => p.Id == id && p.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Select(p=>new {Id=p.Id,PermissionName=p.PermissionName , PermissionCode=p.PermissionCode, PermissionDescription=p.PermissionDescription}).FirstOrDefaultAsync();
            if(permission==null)
            {
                return NotFound(new ApiResult<string> { Code = 404, Message = "未找到权限信息", Data = null, Success = false });
            }
            else
            {
                return Ok(new ApiResult<object> { Code = 200, Message = "获取权限信息成功", Data = permission, Success = true });
            }
        }
        /// <summary>
        /// 根据id编辑权限信息
        /// </summary>
        /// <param name="permissionEditDTO"></param>
        /// <returns></returns>
        [HttpPatch("EditPermissionById")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> EditPermissionById([FromBody] PermissionEditDTO permissionEditDTO )
        {
            //查询当前权限
            var permission= await permissionInfoService.LoadEntities(p => p.Id == permissionEditDTO.Id && p.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(permission==null)
            {
                return NotFound(new ApiResult<string> { Code = 404, Message = "未找到要编辑的权限", Data = null, Success = false });
            }
            //自动映射
            mapper.Map(permissionEditDTO, permission);
            //持久化到数据库
            bool v = await permissionInfoService.UpdateEntity(permission);
            if (v)
            {
                return Ok(new ApiResult<string> { Code = 200, Message = "编辑权限成功", Data = null, Success = true });
            }
            else
            {
                return Ok(new ApiResult<object> { Code = 400, Message = "编辑权限失败", Data = null, Success = true });
            }
        }
        #region 已废弃代码
        /// <summary>
        /// 根据id删除权限（已废弃）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpDelete("DeletePermissionById/{id}")]
        //[Authorize]
        //[UnitOfWork]
        //public async Task<IActionResult> DeletePermissionById([FromRoute]int id)
        //{
        //    var permission= await permissionInfoService.LoadEntities(p => p.Id == id && p.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
        //    if(permission==null)
        //    {
        //        return NotFound(new ApiResult<string> { Code = 404, Message = "未找到要删除的权限", Data = null, Success = false });
        //    }
        //    permission.IsDeleted = Convert.ToBoolean(DelFlagEnum.Deleted);
        //    bool v = await permissionInfoService.UpdateEntity(permission);
        //    if(v)
        //    {
        //        return Ok(new ApiResult<string> { Code = 200, Message = "删除全新成功", Data = null, Success = true });
        //    }
        //    else
        //    {
        //        return BadRequest(new ApiResult<string> { Code = 400, Message = "删除全新失败", Data = null, Success = false });
        //    }
        //}
        #endregion
        /// <summary>
        /// 重置配置文件权限删除密码
        /// </summary>
        /// <returns></returns>
        [HttpPost("ChangeApplicationPassword")]
        [Authorize]
        public async Task<IActionResult> ChangeApplicationPassword()
        {
            //直接生成密码
            string password = PasswordHelper.GenerateDeletePassword();
            //密码加密
            string PasswordHash = PasswordHelper.HashPassword(password);
            //写入配置文件
            bool isWriteSuccess =await permissionInfoService.UpdateConfig("PermissionDeleteSettings:PasswordHash", PasswordHash);
            if(!isWriteSuccess)
            {
                return BadRequest(new ApiResult<string>{ Code = 500, Message = "配置文件写入失败" ,Data=null,Success=false});
            }
            //写入成功获取管理员，发送邮件
            var adminsList= await userInfoService.LoadEntities(u => u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(u => u.RoleInfos).Where(u => u.RoleInfos.Any(r => r.RoleName.Contains("管理员"))).ToListAsync();
            //构建重置结果邮件通知
            SendDeletePwdDTO sendDeletePwdDTO = new SendDeletePwdDTO()
            {
                Password = password,
                Users = adminsList,
                IsSuccess = isWriteSuccess,
            };
            bool v = await permissionInfoService.HandleSendDeletePwd(sendDeletePwdDTO);
            if(v)
            {
                return Ok(new ApiResult<string> { Code = 200, Message = "重置结果已发送至所有管理员邮箱", Data = null, Success = true });
            }
            else
            {
                return Ok(new ApiResult<string> { Code = 200, Message = "重置邮件发送失败,请检查系统配置文件！", Data = null, Success = true });
            }
        }
        /// <summary>
        /// 触发审批流
        /// </summary>
        /// <param name="userLoginDTO"></param>
        /// <returns></returns>
        [HttpPost("SendToTriggerApproval")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> SendToTriggerApproval([FromBody] UserLoginDTO userLoginDTO )
        {
            if (string.IsNullOrEmpty(userLoginDTO.UserPassword))
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "密码不允许为空", Data = null, Success = false });
            }
            //读取配置文件
            string configHash= configuration["PermissionDeleteSettings:PasswordHash"]!;
            //校验密码
            bool v = PasswordHelper.VerifyPassword(userLoginDTO.UserPassword, configHash);
            if(v)
            {
                //查询当前权限
                var permission= await permissionInfoService.LoadEntities(p => p.PermissionCode == userLoginDTO.Code && p.IsDeleteApproving == Convert.ToInt32(ApprovalStatusEnum.UnApproved) && p.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
                if(permission == null)
                {
                    return BadRequest(new ApiResult<string> { Code = 404, Message = "未找到要审批的权限", Data = null, Success = false });
                }
                //设置为审批中
                permission.IsDeleteApproving = Convert.ToInt32(ApprovalStatusEnum.Approving);
                await permissionInfoService.UpdateEntity(permission);
                 //发起人
                var user= await userInfoService.LoadEntities(u => u.UserPhone == userLoginDTO.UserPhone && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
                StartApproveDTO startApproveDTO = new StartApproveDTO()
                {
                    RequestedPermission=permission,
                    Requester=user!
                };
                //触发审批流程,初始化审批记录和审批细节，在这一步由系统决定管理员审批顺序
                await permissionInfoService.HandleTriggerApproval(startApproveDTO);
                //获取当前审批主表信息
                var permissionDeleteApproval = await permissionDeleteApprovalService.LoadEntities(pda=>pda.TargetPermissionId== permission.Id&&pda.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)&&pda.ApprovalStatus==Convert.ToInt32(ApprovalStatusEnum.UnApproved)).FirstOrDefaultAsync();
                await permissionInfoService.HandleSendEmail(permissionDeleteApproval!);
                return Ok(new ApiResult<string> { Code = 200, Message = "验证通过，当前发起的请求已进入审批流程", Data = null, Success = true });
            }
            else
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "验证失败，密码错误", Data = null, Success = false });
            }
        }
        /// <summary>
        /// 邮件展示结果
        /// </summary>
        /// <param name="token"></param>
        /// <param name="approved"></param>
        /// <returns></returns>
        [HttpGet("ProcessApproval")]
        public async Task<IActionResult> ProcessApproval([FromQuery]string token, [FromQuery] bool approved)
        {
            var result = await permissionInfoService.ProcessApproval(token, approved);
            //获取配置文件中FrontendUrl的节点信息
            string frontUrl= configuration["AppSettings:FrontendUrl"]!;

            string html = result.Success
                ? $@"
        <html>
        <head><title>审批结果</title><meta charset='utf-8'></head>
        <body style='font-family: Arial;text-align:center;padding:50px;'>
            <div style='background:#d4edda;border:1px solid #c3e6cb;border-radius:10px;padding:30px;max-width:500px;margin:0 auto;'>
                <h1 style='color:#155724;'>✅ {result.Message}</h1>
                <p><a href='{frontUrl}/#/login' style='color:#007bff;'>返回首页</a></p>
            </div>
        </body>
        </html>"
                : $@"
        <html>
        <head><title>审批结果</title><meta charset='utf-8'></head>
        <body style='font-family: Arial;text-align:center;padding:50px;'>
            <div style='background:#f8d7da;border:1px solid #f5c6cb;border-radius:10px;padding:30px;max-width:500px;margin:0 auto;'>
                <h1 style='color:#721c24;'>❌ {result.Message}</h1>
                <p><a href='{frontUrl}/#/login' style='color:#007bff;'>返回首页</a></p>
            </div>
        </body>
        </html>";

            return Content(html, "text/html; charset=utf-8");
        }
    }
}
