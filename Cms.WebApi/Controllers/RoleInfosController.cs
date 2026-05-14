using Gms.Entity;
using Gms.Entity.DTO;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.IService;
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
    public class RoleInfosController : ControllerBase
    {
        private readonly IRoleInfoService roleInfoService;

        public RoleInfosController(IRoleInfoService roleInfoService)
        {
            this.roleInfoService = roleInfoService;
        }
        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <param name="roleParams"></param>
        /// <returns></returns>
        [HttpGet("GetRoleInfo")]
        [Authorize]
        public  IActionResult GetRoleInfo([FromQuery]RoleParams roleParams)
        {
            RoleSearch roleSearch = new RoleSearch()
            {
                Order=roleParams.Order,
                PageIndex=roleParams.PageIndex,
                PageSize=roleParams.PageSize,
                RoleName=roleParams.RoleName,
                Remark=roleParams.Remark
            };
            var roles= roleInfoService.LoadPageSearchEntities(roleSearch, false).ToList();
            
            return Ok(new ApiResult<object>() { Success = true, Message = "获取角色成功", Data = new {rows=roles,total=roleSearch.TotalCount}, Code = 200 });
            
        }
        /// <summary>
        /// 获取角色具有的权限编号
        /// </summary>
        /// <param name="id">角色id</param>
        /// <returns></returns>
        [HttpGet("GetRolePermission/{id}")]
        [Authorize]
        public async Task<IActionResult> GetRolePermission([FromRoute]int id)
        {
            var role= await roleInfoService.LoadEntities(r => r.Id == id && r.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(role!=null)
            {
                var permissions = roleInfoService.LoadEntities(r => r.Id == id && r.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(r => r.PermissionInfos);
                var permissionIdList = await permissions.Select(r=>r.PermissionInfos.Select(p=>p.Id)).ToListAsync();
                return Ok(new ApiResult<List<IEnumerable<int>>>() { Success = true, Message = "获取角色权限成功", Data = permissionIdList, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未查询到该角色", Data = null,Code=404 }); 
            }
        }
        /// <summary>
        /// 为角色分配权限
        /// </summary>
        /// <param name="rolePerms">角色权限列表</param>
        /// <param name="id">角色id</param>
        /// <returns></returns>
        [HttpPost("SetPermission/{id}/{rolePerms}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> SetPermission([FromRoute] string rolePerms, [FromRoute] int id)
        {
            var roleInfo= await roleInfoService.LoadEntities(r => r.Id == id && r.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(roleInfo==null)
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到该角色", Data = null, Code = 400 });
            }
            var arr = rolePerms.Split(',');
            List<int> list = new List<int>();
            foreach (var rolePerm in arr)
            {
                list.Add(Convert.ToInt32(rolePerm));
            }
            bool isSuccess = await roleInfoService.SetRolePermission(roleInfo, list);
            if (isSuccess)
            {
                return Ok(new ApiResult<string>() { Success = true, Message = "权限分配成功", Data = null, Code = 200 });
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "权限分配失败", Data = null, Code = 400 });
            }
        }
        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="addRoleDTO"></param>
        /// <returns></returns>
        [HttpPost("CreateRole")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateRole([FromBody] AddRoleDTO addRoleDTO)
        {
            if(string.IsNullOrEmpty(addRoleDTO.RoleName))
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "角色名不能为空", Data = null, Success = false });
            }
            RoleInfo roleInfo = new RoleInfo()
            {
                Remark = addRoleDTO.Remark,
                RoleName = addRoleDTO.RoleName,
                CreateDate = DateTime.Now,
                IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal)
            };
            bool v = await roleInfoService.AddEntity(roleInfo);
            if(v)
            {
                return Ok(new ApiResult<string> { Code = 200, Message = "新增角色成功", Data = null, Success = true });
            }
            else
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "新增角色成功", Data = null, Success = false });
            }
        }
        /// <summary>
        /// 根据id获取角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet($"GetRoleInfoById")]
        public async Task<IActionResult> GetRoleInfoById([FromQuery] int id)
        {
            var role= await roleInfoService.LoadEntities(r => r.Id == id && r.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(role!=null)
            {
                return Ok(new ApiResult<RoleInfo> { Code = 200, Message = "获取角色信息成功", Data = role, Success = true });
            }
            else
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "获取角色信息失败,未找到角色信息", Data = null, Success = false });
            }
        }
        /// <summary>
        /// 根据id编辑角色
        /// </summary>
        /// <param name="addRoleDTO"></param>
        /// <returns></returns>
        [HttpPatch("EditRoleInfo")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> EditRoleInfo([FromBody] EditRoleDTO editRoleDTO)
        {
            if(string.IsNullOrEmpty(editRoleDTO.RoleName))
            {
                return BadRequest(new ApiResult<string> { Code = 400, Message = "角色名称不能为空", Data = null, Success = false });
            }
            var role= await roleInfoService.LoadEntities(r => r.Id == editRoleDTO.Id && r.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(role!=null)
            {
                role.Remark = editRoleDTO.Remark;
                role.RoleName = editRoleDTO.RoleName;
                bool v = await roleInfoService.UpdateEntity(role);
                if (v)
                {
                    return Ok(new ApiResult<string> { Code = 200, Message = "角色信息编辑成功", Data = null, Success = true });
                }
                else
                {
                    return BadRequest(new ApiResult<string> { Code = 400, Message = "角色信息编辑失败", Data = null, Success = false });
                }
            }
            else
            {
                return BadRequest(new ApiResult<string> { Code = 404, Message = "未找到要编辑的角色", Data = null, Success = false });
            }
        }
        [HttpDelete("DeleteRoleInfoById/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> DeleteRoleInfoById([FromRoute]int id)
        {
            //根据id查询出所要删除的角色
            var role= await roleInfoService.LoadEntities(r => r.Id == id && r.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            //修改角色状态为“已删除”
            if(role!=null)
            {
                role.IsDeleted = Convert.ToBoolean(DelFlagEnum.Deleted);
                bool v = await roleInfoService.UpdateEntity(role);
                if(v)
                {
                    return Ok(new ApiResult<string> { Code = 200, Message = "删除成功", Data = null ,Success=true});
                }
                else
                {
                    return BadRequest(new ApiResult<string> { Code = 400, Message = "删除失败", Data = null, Success = false });
                }
            }
            else
            {
                return NotFound(new ApiResult<string> { Code = 404, Message = "删除失败,未找到要删除的角色", Data = null, Success = false });
            }

        }
    }
}
