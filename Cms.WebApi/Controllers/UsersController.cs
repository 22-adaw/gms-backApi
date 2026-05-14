using Gms.Common;
using Gms.Entity;
using Gms.Entity.DTO;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.IService;
using Gms.Repository;
using Gms.Service;
using Gms.WebApi.Attributes;
using Gms.WebApi.Models;
using Gms.WebApi.SearchParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Threading.Tasks;
namespace Gms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserInfoService  userInfoService;
        private readonly IDepartmentService departmentService;
        private readonly IRoleInfoService roleInfoService;
        private readonly IVipInfoService vipInfoService;
        
        public UsersController(IUserInfoService userInfoService, IDepartmentService departmentService, IRoleInfoService roleInfoService, IVipInfoService vipInfoService)
        {
            this.userInfoService = userInfoService;
            this.departmentService = departmentService;
            this.roleInfoService = roleInfoService;
            this.vipInfoService = vipInfoService;
        }
        /// <summary>
        /// 根据手机号码获取登录用户信息
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUser([FromQuery]string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
            {
                return BadRequest("手机号码不能为空");
            }
          var user = await  userInfoService.LoadEntities(u=>u.UserPhone==mobile).Include(u=>u.RoleInfos).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound(new ApiResult<string>() {Success=false,Message="没有找到用户",Data= null });
            }
            //查询用户具有的角色权限信息
            var userPermCodeDict = await userInfoService.GetUserPermission(user);
            return Ok(new ApiResult<object>() { Success = true, Message = "获取用户成功", Data = new { User = new {Id=user.Id,UserName=user.UserName,UserPhone=user.UserPhone, PhotoUrl=user.PhotoUrl, IsClient = user.IsClient ,RealName=user.RealName, rId = user.RoleInfos!.Select(r => r.Id) },UserPermCodeDict= userPermCodeDict },Code=200 }); 
        }
        /// <summary>
        /// 员工注册
        /// </summary>
        /// <param name="userInfoDTO"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> Register([FromBody]UserInfoDTO userInfoDTO)
        {
            try
            {
                //校验手机号是否已被注册
                var user = await userInfoService.LoadEntities(u => (u.UserPhone == userInfoDTO.UserPhone||u.UserEmail==userInfoDTO.UserEmail)&& u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
                if(user!=null)
                {
                    return Ok(new ApiResult<string>() { Success = false, Message = "该账号已存在" ,Data=null});
                }
                var department= await departmentService.LoadEntities(d => d.DepartmentName == userInfoDTO.DepartmentName && d.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
                userInfoDTO.UserPassword = PasswordHelper.GeneratePassword();
                //构建用户实体
                var userRegister = new UserInfo()
                {
                    Id = new Random().Next(100000, 999999),
                    UserName = userInfoDTO.UserName,
                    RealName = userInfoDTO.RealName,
                    UserPassword = PasswordHelper.HashPassword(userInfoDTO.UserPassword),
                    UserEmail = userInfoDTO.UserEmail,
                    UserPhone = userInfoDTO.UserPhone,
                    Gender = userInfoDTO.Gender,
                    PhotoUrl = "images\\12.jpg",
                    CreateDate = DateTime.Now,
                    DepartmentId = department!.Id,
                    EditDate=DateTime.Now,
                    Department=department,
                    IsDeleted=Convert.ToBoolean(DelFlagEnum.Nomal)


                };
                //保存到数据库
                bool v = await userInfoService.AddEntity(userRegister);
                //构建发送邮件数据
                UserInDTO userInDTO = new UserInDTO()
                {
                    UserEmail = userInfoDTO.UserEmail,
                    RealName = userInfoDTO.RealName,
                    UserName = userInfoDTO.UserName,
                    UserPassword = userInfoDTO.UserPassword,
                    DepartmentName = department.DepartmentName,
                    UserPhone= userInfoDTO.UserPhone
                };
                if(v)
                {
                    bool v1 = await userInfoService.HandleCreate(userInDTO);
                    if(v1)
                    {
                        return Ok(new ApiResult<string>() { Success = true, Message = $"操作成功,消息已发送至{userInDTO.RealName}的注册邮箱", Data = null, Code = 200 });
                    }
                    else
                    {
                        return BadRequest(new ApiResult<string>() { Success = false, Message = "操作失败", Data = null, Code = 400 });
                    }
                }
                else
                {
                    return BadRequest(new ApiResult<string> { Code = 400, Message = "操作失败", Data = null, Success = false });
                }
                

            }
            catch
            {
                return Ok(new ApiResult<string>() { Success=false,Message="操作失败",Data=null});
            }
        }
        /// <summary>
        /// 获取所有员工信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUsers")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var userInfos = await userInfoService.LoadEntities(u => u.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)&& u.IsClient == Convert.ToBoolean(ClientCheckEnum.Employee)).ToListAsync();
            if(userInfos!=null)
            {
                return Ok(new ApiResult<List<UserInfo>>() { Success = true, Message = "获取用户信息成功", Data = userInfos, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "获取用户信息失败", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 分页获取员工信息
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        [HttpGet("GetUsersPage")]
        [Authorize]
        public IActionResult GetUsersPage([FromQuery]UserParams userParams)
        { 
            //判断userParams中pageIndex的取值范围
            UserSearch userSearch = new UserSearch()
            {
                Order = userParams.Order,
                PageIndex=userParams.PageIndex,
                PageSize=userParams.PageSize,
                UserName=userParams.UserName,
                RealName=userParams.RealName,
                UserPhone=userParams.UserPhone,
                DepartmentName=userParams.DepartmentName,
                TotalCount=0
            };
            var users = userInfoService.LoadPageEntities(userSearch,false).Select(u=>new {Id=u.Id,UserPhone=u.UserPhone,RealName= u.RealName,CreateDate=u.CreateDate ,DepartmentName=u.Department!=null?u.Department.DepartmentName:"无部门",PhotoUrl=u.PhotoUrl,IsClient=u.IsClient}).ToList();
            users = users.Where(u => u.IsClient==Convert.ToBoolean(ClientCheckEnum.Employee)).ToList();
            return Ok(new ApiResult<object>() { Success = true, Message = "获取成功", Data =new {Rows=users,Total=userSearch.TotalCount }, Code = 200 });
        }
        /// <summary>
        /// 根据id获取单个员工信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetUserById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById([FromRoute]int id)
        {
            var user= await userInfoService.LoadEntities(u => u.Id == id && u.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(user==null)
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到该用户", Data = null, Code = 404 });
            }
            var department = await departmentService.LoadEntities(d => d.Id == user.DepartmentId && d.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            var userDetail = new UserDetailDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                RealName = user.RealName,
                UserEmail = user.UserEmail,
                UserPhone = user.UserPhone,
                Gender = (int)user.Gender!,
                DepartmentName = department?.DepartmentName,
                CreateDate = user.CreateDate,
                EditDate = user.EditDate,
                IsDeleted = user.IsDeleted,
                PhotoUrl = user.PhotoUrl
            };
            return Ok(new ApiResult<UserDetailDTO>() { Success = true, Message = "获取单个员工成功", Data = userDetail, Code = 200 });
        }
        /// <summary>
        /// 根据id删除单个员工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteEmployeeById/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> DeleteEmployeeById([FromRoute] int id)
        {
            var user = await userInfoService.LoadEntities(u => u.Id == id && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "未找到该员工", Data = null, Code = 400 });
            }
            var department= await departmentService.LoadEntities(d => d.ManagerId == id && d.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(department!=null)
            {
                department.Manager = null;
                department.ManagerId = 0;
                await departmentService.UpdateEntity(department);
            }
            user.IsDeleted = true;
            user.EditDate = DateTime.Now;
            bool v = await userInfoService.UpdateEntity(user);
            if(v)
            {
                return Ok(new ApiResult<string>() { Success = true, Message = "删除员工成功", Data = null, Code = 200 });
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "删除员工失败", Data = null, Code = 400 });
            }
        }
        /// <summary>
        /// 编辑员工
        /// </summary>
        /// <param name="userInfoDTO"></param>
        /// <returns></returns>
        [HttpPatch("EditEmployee")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> EditEmployee([FromBody]UserInfoDTO userInfoDTO)
        {
            var user= await userInfoService.LoadEntities(u => u.Id == userInfoDTO.Id && u.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            bool v1 = false;
            var department= await departmentService.LoadEntities(d => d.DepartmentName == userInfoDTO.DepartmentName && d.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            var dept = await departmentService.LoadEntities(d => d.ManagerId == userInfoDTO.Id && d.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(dept!=null)
            {
                //说明是部门负责人
                if (userInfoDTO.DepartmentName != dept.DepartmentName)
                {
                    //说明部门发生了改变
                    dept.ManagerId = 0;
                    dept.Manager = null;
                }
                else
                {
                    dept.Manager = userInfoDTO.RealName;
                }
                 v1 = await departmentService.UpdateEntity(dept);
            }
            //不是负责人就不修改Department表
            if(user!=null)
            {
                user.RealName = userInfoDTO.RealName;
                user.UserName = userInfoDTO.UserName;
                user.UserPhone = userInfoDTO.UserPhone;
                user.UserEmail = userInfoDTO.UserEmail;
                user.DepartmentId = department!.Id;
                user.Gender = userInfoDTO.Gender;
                user.EditDate = DateTime.Now;
                bool v = await userInfoService.UpdateEntity(user);
                if(v)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "编辑员工成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "编辑员工失败", Data = null, Code = 400 });
                }
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到该员工", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 获取所有角色以及获取该员工具有的所有角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/GetUserRole")]
        [Authorize]
        public async Task<IActionResult> GetUserRole([FromRoute]int userId)
        {
            var user = await userInfoService.LoadEntities(u => u.Id == userId && u.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(user==null)
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到用户", Data = null, Code = 404 });
            }
            //获取了所有角色
            var roles= await roleInfoService.LoadEntities(d => d.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)&&d.Id!=8).ToListAsync();
            //获取员工具有的角色编号
            var userRole =  userInfoService.LoadEntities(u => u.Id == userId&&u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(u => u.RoleInfos);
            var userRoleIdList= await userRole.Select(u => u.RoleInfos.Select(r => r.Id)).ToListAsync();

            return Ok(new ApiResult<object>() { Success = true, Message = "获取成功", Data = new { Roles = roles, UserRoleIdList = userRoleIdList }, Code = 200 });
        }
        /// <summary>
        /// 给员工分配角色
        /// </summary>
        /// <param name="roleIds"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("SetUserRole/{userId}/{roleIds}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> SetUserRole([FromRoute]string roleIds,[FromRoute]int userId)
        {
            //查询当前用户
            var user= await userInfoService.LoadEntities(u => u.Id == userId && u.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(user==null)
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到用户", Data = null, Code = 404 });
            }
            var arr = roleIds.Split(',');
            List<long> list = new List<long>();
            foreach (var roleId in arr)
            {
                list.Add(Convert.ToInt64(roleId));
            }
            var isSuccess=await userInfoService.SetUserRoles(user, list);
            if(isSuccess)
            {
                await userInfoService.UpdateEntity(user);
                return Ok(new ApiResult<string>() { Success = true, Message = "分配角色成功", Data = null, Code = 200 });
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "分配角色失败", Data = null, Code = 400 });
            }
            
        }
        /// <summary>
        /// 根据手机号查询用户信息
        /// </summary>
        /// <param name="UserPhone"></param>
        /// <returns></returns>
        [HttpGet("GetUserByPhone")]
        [Authorize]
        public async Task<IActionResult> GetUserByPhone([FromQuery]string UserPhone)
        {
            var user = await userInfoService.LoadEntities(u => u.UserPhone == UserPhone && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(u => u.Department).Select(a => new {Id=a.Id, UserName = a.UserName ,UserEmail=a.UserEmail, UserPhone=a.UserPhone, DepartmentName=a.Department.DepartmentName, Manager=a.Department.Manager, RealName=a.RealName, PhotoUrl=a.PhotoUrl, RoleName = a.RoleInfos.Select(b=>b.RoleName), IsClient=a.IsClient}).FirstOrDefaultAsync();
            if(user==null)
            {
                return NotFound(new ApiResult<string> { Code = 404, Message = "未查询到用户信息", Success = false, Data = null });
            }
            var vip = await vipInfoService.LoadEntities(v => v.VipPhone == user.UserPhone && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(v => v.VipCard).FirstOrDefaultAsync();

            if (vip==null|| vip.VipCard==null)
            {
                return Ok(new ApiResult<object> { Code = 200, Message = "获取员工信息成功", Data = user, Success = true });
            }
            SendInfoDTO sendInfoDTO = new SendInfoDTO()
            {
                Id = user.Id,
                UserName = user.UserName,
                UserEmail = user.UserEmail,
                UserPhone = user.UserPhone,
                DepartmentName = user.DepartmentName,
                Manager = user.Manager==null? user.Manager:"null",
                RealName = user.RealName,
                PhotoUrl = user.PhotoUrl,
                RoleName = user.RoleName.ToList() ?? new List<string>(),
                IsClient = user.IsClient,
                EndDate = vip.VipCard.EndDate,
                FreezeStatus = vip.VipCard.FreezeStatus,
                RemainTimes = vip.VipCard.RemainTimes,
                CardNum = vip.VipCard.CardNum,
                LeftMoney = vip.VipCard.LeftMoney
            };
            return Ok(new ApiResult<object> { Data = sendInfoDTO, Code = 200, Message = "获取用户信息成功", Success = true });

        }
        /// <summary>
        /// 获取用户编辑信息
        /// </summary>
        /// <param name="UserPhone"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetEditUserByPhone")]
        public async Task<IActionResult> GetEditUserByPhone([FromQuery] string UserPhone)
        {
            var user= await userInfoService.LoadEntities(u => u.UserPhone == UserPhone && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Select(u=>new { RealName=u.RealName, UserName=u.UserName, UserPhone=u.UserPhone, UserEmail=u.UserEmail, Gender=u.Gender, PhotoUrl=u.PhotoUrl, IsClient=u.IsClient}).FirstOrDefaultAsync();
            if (user != null)
            {
                return Ok(new ApiResult<object> { Code = 200, Message = "获取编辑用户信息成功", Data = user, Success = true });
            }
            else
            {
                return Ok(new ApiResult<object> { Code = 400, Message = "获取编辑用户信息失败，未找到要编辑的用户信息", Data = null, Success = false });
            }
        }
        /// <summary>
        /// 在首页编辑自己
        /// </summary>
        /// <param name="userEditDTO"></param>
        /// <returns></returns>
        [HttpPost("EditUserInfoAtMainPage")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> EditUserInfoAtMainPage([FromBody] UserEditDTO userEditDTO)
        {
            //根据userphone查询当前用户
            var user = await userInfoService.LoadEntities(u => u.UserPhone == userEditDTO.UserPhone && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest(new ApiResult<string> { Code = 404, Message = "该用户不存在", Data = null, Success = false });
            }
            user.PhotoUrl = userEditDTO.PhotoUrl;
            user.RealName = userEditDTO.RealName;
            user.UserName = userEditDTO.UserName;
            user.UserEmail = userEditDTO.UserEmail;
            user.Gender = Convert.ToInt32(userEditDTO.Gender);
            user.EditDate = DateTime.Now;
            if (string.IsNullOrEmpty(userEditDTO.UserPwd) && string.IsNullOrEmpty(userEditDTO.ConfirmPwd))
            {
                //不校验密码
                bool v = await userInfoService.UpdateEntity(user);
                if (userEditDTO.IsClient || userEditDTO.UserPhone == "13767565260")
                {
                    var vip= await vipInfoService.LoadEntities(v => v.VipPhone == user.UserPhone && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
                        if(vip==null)
                    {
                        return BadRequest(new ApiResult<string> { Code = 400, Message = "未找到该会员", Data = null, Success = false });
                    }
                    vip.VipName = userEditDTO.RealName;
                    vip.Gender = userEditDTO.Gender;
                    vip.VipEmail = userEditDTO.UserEmail;
                    vip.EditDate = DateTime.Now;

                    bool v1 = await vipInfoService.UpdateEntity(vip);
                    if(v1&&v)
                    {
                        return Ok(new ApiResult<string> { Code = 200, Message = "编辑成功", Data = null, Success = true });
                    }
                    else
                    {
                        return BadRequest(new ApiResult<string> { Code = 400, Message = "编辑失败", Data = null, Success = false });
                    }
                }
                if(v)
                {
                    return Ok(new ApiResult<string> { Code = 200, Message = "编辑成功", Data = null, Success = true });
                }
                else
                {
                    return BadRequest(new ApiResult<string> { Code = 400, Message = "编辑失败", Data = null, Success = false });
                }
            }
            else
            {
                //修改了密码，校验密码
                if (string.IsNullOrEmpty(userEditDTO.UserPwd))
                {
                    return BadRequest(new ApiResult<string> { Code = 400, Message = "密码不允许为空", Data = null, Success = false });
                }
                else if(userEditDTO.UserPwd.Length<6|| userEditDTO.UserPwd.Length>15)
                {
                    return BadRequest(new ApiResult<string> { Code = 400, Message = "密码必须不小于6位和不大于15位", Data = null, Success = false });
                }
                else if (string.IsNullOrEmpty(userEditDTO.ConfirmPwd))
                {
                    return BadRequest(new ApiResult<string> { Code = 400, Message = "确认密码不允许为空", Data = null, Success = false });
                }
                else if (userEditDTO.UserPwd != userEditDTO.ConfirmPwd)
                {
                    return BadRequest(new ApiResult<string> { Code = 400, Message = "两次密码不一致", Data = null, Success = false });
                }
                else if (string.IsNullOrEmpty(userEditDTO.OldPwd))
                {
                    return BadRequest(new ApiResult<string> { Code = 400, Message = "原密码不允许为空", Data = null, Success = false });
                }
                else if (!PasswordHelper.VerifyPassword(userEditDTO.OldPwd, user.UserPassword!))
                {
                    return BadRequest(new ApiResult<string> { Code = 400, Message = "原密码错误", Data = null, Success = false });

                }
                else if (PasswordHelper.VerifyPassword(userEditDTO.UserPwd, user.UserPassword!))
                {
                    return BadRequest(new ApiResult<string> { Code = 400, Message = "新密码不能与原密码相同", Data = null, Success = false });
                }
                else
                {
                    //密码正确
                    
                    user.UserPassword = PasswordHelper.HashPassword(userEditDTO.UserPwd);
                    bool v = await userInfoService.UpdateEntity(user);
                    if (userEditDTO.IsClient || userEditDTO.UserPhone == "13767565260")
                    {
                        var vip = await vipInfoService.LoadEntities(v => v.VipPhone == user.UserPhone && v.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
                        vip.VipName = userEditDTO.RealName;
                        vip.Gender = userEditDTO.Gender;
                        vip.VipEmail = userEditDTO.UserEmail;
                        vip.EditDate = DateTime.Now;
                        vip.VipPassword = PasswordHelper.HashPassword(userEditDTO.UserPwd);

                        bool v1 = await vipInfoService.UpdateEntity(vip);
                        if (v1 && v)
                        {
                            return Ok(new ApiResult<string> { Code = 200, Message = "编辑成功", Data = null, Success = true });
                        }
                        else
                        {
                            return BadRequest(new ApiResult<string> { Code = 400, Message = "编辑失败", Data = null, Success = false });
                        }
                    }
                    if (v)
                    {
                        return Ok(new ApiResult<string> { Code = 200, Message = "编辑成功", Data = null, Success = true });
                    }
                    else
                    {
                        return BadRequest(new ApiResult<string> { Code = 400, Message = "编辑失败", Data = null, Success = false });
                    }
                }
            }
        }
    }
}
