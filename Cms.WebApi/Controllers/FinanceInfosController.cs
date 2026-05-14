using Gms.Entity;
using Gms.Entity.DTO;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.IRepository;
using Gms.IService;
using Gms.WebApi.Attributes;
using Gms.WebApi.Models;
using Gms.WebApi.SearchParams;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Gms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceInfosController : ControllerBase
    {
        private readonly IFinanceInfoService financeInfoService;
        public FinanceInfosController(IFinanceInfoService financeInfoService)
        {
            this.financeInfoService = financeInfoService;
        }
        /// <summary>
        /// 分页获取财务信息
        /// </summary>
        /// <param name="financeParams"></param>
        /// <returns></returns>
        [HttpGet("GetFinanceInfoPages")]
        [Authorize]
        public IActionResult GetFinanceInfoPages([FromQuery] FinanceParams financeParams)
        {
            var financeSearch = new FinanceSearch()
            {
                FinanceType = financeParams.FinanceType,
                TypeName = financeParams.TypeName,
                RelatedCode = financeParams.RelatedCode, 
                Order = financeParams.Order,
                PageIndex = financeParams.PageIndex,
                PageSize = financeParams.PageSize,
                TotalCount = 0
            };
            var financeInfoList = financeInfoService.LoadPagesEntities(financeSearch, false).Select(f => new { f.Id, f.FinanceType, f.TypeName, f.FinanceCode, f.Remark, f.Amount, f.RelatedCode}).ToList();
            if (financeInfoList.Count > 0)
            {
                return Ok(new ApiResult<object> { Success = true, Message = "财务信息分页获取成功", Data = new { Rows = financeInfoList, Total = financeSearch.TotalCount }, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到财务信息", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 新增财务信息
        /// </summary>
        /// <param name="financeInfoDTO"></param>
        /// <returns></returns>
        [HttpPost("CreateFinanceInfo")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateFinanceInfo([FromBody]FinanceInfoDTO financeInfoDTO)
        {
            var financeInfo = new FinanceInfo()
            {
                FinanceType = financeInfoDTO.FinanceType,
                TypeName = financeInfoDTO.TypeName == 0 ? "办卡收入" : financeInfoDTO.TypeName == 1 ? "工资支出" : financeInfoDTO.TypeName == 3 ? "房租水电支出" : financeInfoDTO.TypeName == 4 ? "设备维修支出" : financeInfoDTO.TypeName == 5 ? "其他支出" : "其他收入",
                Amount = financeInfoDTO.Amount,
                FinanceCode = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999),
                RelatedCode =null,
                RelatedType=null,
                Remark=financeInfoDTO.Remark,
                IsDeleted=Convert.ToBoolean(DelFlagEnum.Nomal),
                CreateDate=DateTime.Now
            };
            bool v = await financeInfoService.AddEntity(financeInfo);
            if(v)
            {
                return Ok(new ApiResult<string>() { Success = true, Message = "新增财务信息成功", Data = null, Code = 200 });
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "新增财务信息失败", Data = null, Code = 400 });
            }
        }
        /// <summary>
        /// 根据id查询单个财务信息详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetFinanceInfoById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetFinanceInfoById([FromRoute]int id)
        {
            var financeInfo= await financeInfoService.LoadEntities(f => f.Id == id && f.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(financeInfo!=null)
            {
                return Ok(new ApiResult<FinanceInfo>() { Success = true, Message = "获取财务信息成功", Data = financeInfo, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到财务信息", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 根据id软删除财务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteFinanceInfoById/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> DeleteFinanceInfoById([FromRoute]int id)
        {
            var financeInfo= await financeInfoService.LoadEntities(f => f.Id == id && f.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(financeInfo!=null)
            {
                financeInfo.IsDeleted = true;
                bool v = await financeInfoService.UpdateEntity(financeInfo);
                if(v)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "删除财务信息成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "删除财务信息失败", Data = null, Code = 400 });
                }
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "删除财务信息失败,未找到财务信息", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 根据id编辑财务信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="financeInfoDTO"></param>
        /// <returns></returns>
        [HttpPatch("EditFinanceInfopById/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> EditFinanceInfopById([FromRoute]int id, [FromBody]FinanceInfoDTO financeInfoDTO)
        {
            var financeInfo= await financeInfoService.LoadEntities(f => f.Id == id && f.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(financeInfo != null)
            {
                financeInfo.FinanceType = financeInfoDTO.FinanceType;
                financeInfo.TypeName = financeInfoDTO.TypeName == 0 ? "办卡收入" : financeInfoDTO.TypeName == 1 ? "工资支出" : financeInfoDTO.TypeName == 3 ? "房租水电支出" : financeInfoDTO.TypeName == 4 ? "设备维修支出" : financeInfoDTO.TypeName == 5 ? "其他支出" : "其他收入";
                financeInfo.Amount = financeInfoDTO.Amount;
                financeInfo.Remark = financeInfoDTO.Remark;
                financeInfo.EditDate = DateTime.Now;
                bool v = await financeInfoService.UpdateEntity(financeInfo);
                if(v)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "财务信息更新成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = true, Message = "财务信息更新失败", Data = null, Code = 400 });
                }
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到财务信息", Data = null, Code = 404 });
            }
        }
    }
}
