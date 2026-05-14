using Gms.Entity;
using Gms.Entity.Enum;
using Gms.IService;
using Gms.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Gms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentTypesController : ControllerBase
    {
        private readonly IEquipmentTypeService equipmentTypeService;
        public EquipmentTypesController(IEquipmentTypeService equipmentTypeService)
        {
            this.equipmentTypeService = equipmentTypeService;
        }
        /// <summary>
        /// 获取器材类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetEquipmentType")]
        [Authorize]
        public async Task<IActionResult> GetEquipmentType()
        {
            var equipmentTypeList= await equipmentTypeService.LoadEntities(e => e.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).ToListAsync();
            if(equipmentTypeList.Count==0)
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到器材类型", Data = null, Code = 404 });
            }
            else
            {
                return Ok(new ApiResult<List<EquipmentType>>() { Success = true, Message = "获取器材类型成功", Data = equipmentTypeList, Code = 200 });
            }
        }
    }
}
