using Gms.Entity;
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
using System.Threading.Tasks;

namespace Gms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VipCardTypesController : ControllerBase
    {
        private readonly IVipCardTypeService vipCardTypeService;
        public VipCardTypesController(IVipCardTypeService vipCardTypeService)
        {
            this.vipCardTypeService = vipCardTypeService;
        }
        /// <summary>
        /// 分页获取会员卡类别信息
        /// </summary>
        /// <param name="vipCardTypeParams"></param>
        /// <returns></returns>
        [HttpGet("GetVipCardTypePages")]
        [Authorize]
        public IActionResult GetVipCardTypePages([FromQuery]VipCardTypeParams vipCardTypeParams)
        {
            var vipCardTypeSearch = new VipCardTypeSearch()
            {
                Order = vipCardTypeParams.Order,
                TotalCount = 0,
                PageIndex = vipCardTypeParams.PageIndex,
                PageSize = vipCardTypeParams.PageSize,
                VipCardTypeCode = vipCardTypeParams.VipCardTYpeCode,
                VipCardTypeName = vipCardTypeParams.VipCardTypeName
            };
            var vipCardTypeList= vipCardTypeService.LoadPagesEntities(vipCardTypeSearch, false).Select(vct => new { Id = vct.Id, VipCardTypeName = vct.VipCardTypeName, VipCardTypeCode = vct.VipCardTypeCode, DiscountRate = vct.DiscountRate, Price = vct.Price, UseDays = vct.UseDays, UseTimes = vct.UseTimes, CreateDate = vct.CreateDate, Remark = vct.Remark }).ToList();
            if(vipCardTypeList.Count>0)
            {
                return Ok(new ApiResult<object>() { Success = true, Message = "会员卡类型分页获取成功", Data = new { Rows = vipCardTypeList, Total = vipCardTypeSearch.TotalCount }, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "会员卡类型分页获取失败,未找到会员卡类型信息", Data = null, Code = 400 });
            }
        }
        /// <summary>
        /// 新增会员卡
        /// </summary>
        /// <param name="vipCardType"></param>
        /// <returns></returns>
        [HttpPost("CreateVipCardType")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateVipCardType([FromBody]VipCardType vipCardType)
        {
            vipCardType.CreateDate = DateTime.Now;
            vipCardType.IsDeleted = Convert.ToBoolean(DelFlagEnum.Nomal);
            bool v = await vipCardTypeService.AddEntity(vipCardType);
            if(v)
            {
                return Ok(new ApiResult<string>() { Success = true, Message = "卡类别新增成功", Data = null, Code = 200 });
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "卡类别新增失败", Data = null, Code = 400 });
            }
        }
        /// <summary>
        /// 获取所有会员卡类别信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetVipCardTypes")]
        [Authorize]
        public async Task<IActionResult> GetVipCardTypes()
        {
            var vipCardTypes= await vipCardTypeService.LoadEntities(vct => vct.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).ToListAsync();
            if(vipCardTypes.Count>0)
            {
                return Ok(new ApiResult<List<VipCardType>>() { Success=true,Message="获取会员卡类别成功", Data = vipCardTypes, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "获取会员卡类别失败，未找到会员卡类别信息", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 根据名称获取单个会员卡类型
        /// </summary>
        /// <param name="vipCardTypeName"></param>
        /// <returns></returns>
        [HttpGet("GetVipCardTypeByName")]
        [Authorize]
        public async Task<IActionResult> GetVipCardTypeByName([FromQuery]string vipCardTypeName)
        {
            var vipCardType= await vipCardTypeService.LoadEntities(vct => vct.VipCardTypeName == vipCardTypeName && vct.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(vipCardType!=null)
            {
                return Ok(new ApiResult<VipCardType>() { Success = true, Message = "获取单个会员卡类型信息成功", Data = vipCardType, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "获取单个会员卡类型信息失败，未找到会员卡类型信息", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 根据id删除会员卡类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteCardTypeById/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> DeleteCardTypeById([FromRoute]int id)
        {
            var vipCardType= await vipCardTypeService.LoadEntities(vct => vct.Id == id && vct.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(vipCardType!=null)
            {
                vipCardType.IsDeleted = true;
                bool v = await vipCardTypeService.UpdateEntity(vipCardType);
                if(v)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "会员卡类型删除成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "会员卡类型删除失败", Data = null, Code = 400 });
                }
            } 
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "会员卡类型删除失败,未找到会员卡类型", Data = null, Code =  404 });
            }
        }
        /// <summary>
        /// 通过id编辑会员卡类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vipCardType"></param>
        /// <returns></returns>
        [HttpPatch("EditCardTypeById/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> EditCardTypeById([FromRoute]int id, [FromBody]VipCardType vipCardType)
        {
            var cardType= await vipCardTypeService.LoadEntities(vct => vct.Id == id && vct.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(cardType!=null)
            {
                cardType.VipCardTypeName = vipCardType.VipCardTypeName;
                cardType.VipCardTypeCode = vipCardType.VipCardTypeCode;
                cardType.DiscountRate = vipCardType.DiscountRate;
                cardType.Price = vipCardType.Price;
                cardType.Remark = vipCardType.Remark;
                cardType.UseDays = vipCardType.UseDays;
                cardType.UseTimes = vipCardType.UseTimes;
                cardType.EditDate = DateTime.Now;
                bool v = await vipCardTypeService.UpdateEntity(cardType);
                if(v)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "会员卡类型更新成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "会员卡类型更新失败", Data = null, Code = 400 });
                }
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "会员卡类型更新失败，未找到会员卡类型", Data = null, Code = 404 });
            }
        }
    }
}
