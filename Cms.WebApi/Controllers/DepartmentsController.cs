using Gms.Entity;
using Gms.Entity.Enum;
using Gms.IRepository;
using Gms.IService;
using Gms.WebApi.Attributes;
using Gms.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Gms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService departmentService;
        private readonly IUserInfoService userInfoService;

        public DepartmentsController(IDepartmentService departmentService, IUserInfoService userInfoService)
        {
            this.departmentService = departmentService;
            this.userInfoService = userInfoService;
        }
        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <returns></returns>
        [HttpGet] //get   api/department
        [Authorize]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await departmentService.LoadEntities(d => d.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)).ToListAsync();
            if(departments != null)
            {
                return Ok(new ApiResult<List<Departments>>() { Success = true, Code = 200, Message = "所有部门获取成功", Data = departments });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Code = 404, Message = "所有部门获取失败", Data = null });
            }
        }
        /// <summary>
        /// 删除部门信息
        /// </summary>
        /// <param name="departments"></param>
        /// <returns></returns>
        [HttpDelete("{id}")] //delete  api/departments/2
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> RemoveDepartment([FromRoute]int id)
        {
            var department= await departmentService.LoadEntities(d => d.Id == id && d.IsDeleted== Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            var user= await userInfoService.LoadEntities(u => u.Id == department.ManagerId && u.IsDeleted== Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(user != null)
            {
                user.DepartmentId = null;
                bool v = await userInfoService.UpdateEntity(user);

                if (department != null)
                {
                    department.IsDeleted = true;
                    department.EditDate = DateTime.Now;
                    await departmentService.UpdateEntity(department);
                }
                if(v)
                {
                    return Ok(new ApiResult<Departments>() { Success = true, Message = "部门删除成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<Departments>() { Success = false, Message = "部门删除失败", Data = null, Code = 400 });
                }
            }
            else
            {
                return NotFound(new ApiResult<Departments>() { Success = false, Message = "该部门不存在", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="departments"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateDepartment([FromBody]Departments departments)
        {
            var arr = departments.Manager.Split(":");
            var department = new Departments()
            {
                DepartmentName = departments.DepartmentName,
                DepartmentCode = departments.DepartmentCode,
                DepartmentDescription = departments.DepartmentDescription,
                ParentId = departments.ParentId,
                ManagerId = Convert.ToInt32(arr[0]),
                Manager = arr[1],
                CreateDate = DateTime.Now,
                IsDeleted = false,
                City = "北京"
            };
            var user= await userInfoService.LoadEntities(u => u.Id == department.ManagerId && department.IsDeleted== Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            await departmentService.AddEntity(department);
            int v = await departmentService.SaveChangesAsync();
            var dept= await departmentService.LoadEntities(d => d.DepartmentName == department.DepartmentName && d.IsDeleted== Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if (user != null&&dept!=null)
            {
                user.DepartmentId = dept.Id;
                bool v1 = await userInfoService.UpdateEntity(user);
                if (v > 0 && v1)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "添加部门成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "添加部门失败", Data = null, Code = 400 });
                }
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "添加部门失败", Data = null, Code = 404 });
            }
            
        }
        /// <summary>
        /// 根据id获取部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDepartmentById([FromRoute]int id)
        {
            var department= await departmentService.LoadEntities(d => d.Id == id && d.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(department!=null)
            {
                return Ok(new ApiResult<Departments>() { Success = true, Message = "获取部门成功", Data = department, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "获取部门失败", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 编辑部门
        /// </summary>
        /// <param name="departments"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> UpdateDepartment([FromBody]Departments departments)
        {
            int id = departments.Id;
            var arr = departments.Manager.Split(":");
            var department= await departmentService.LoadEntities(d => d.Id == id && d.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if (department!=null)
            {
                department.DepartmentName = departments.DepartmentName;
                department.DepartmentCode = departments.DepartmentCode;
                department.Manager = arr[1];
                department.EditDate = DateTime.Now;
                department.ManagerId = Convert.ToInt32(arr[0]);
                department.DepartmentDescription = departments.DepartmentDescription;
                bool v1 = await departmentService.UpdateEntity(department);
                var user= await userInfoService.LoadEntities(u => u.Id == department.ManagerId && u.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
                user!.DepartmentId = id;
                bool v = await userInfoService.UpdateEntity(user);
                if(v&&v1)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "更新部门成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "更新部门失败", Data = null ,Code=400});
                }
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到要更新的部门", Data = null, Code = 404 });
            }
        }
    }
}
