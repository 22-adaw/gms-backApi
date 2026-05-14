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
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Gms.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentsController : ControllerBase
    {
        private readonly IEquipmentsService equipmentsService;
        private readonly IEquipmentTypeService equipmentTypeService;
        private readonly IFinanceInfoService financeInfoService;
        public EquipmentsController(IEquipmentsService equipmentsService, IEquipmentTypeService equipmentTypeService, IFinanceInfoService financeInfoService)
        {
            this.equipmentsService = equipmentsService;
            this.equipmentTypeService = equipmentTypeService;
            this.financeInfoService = financeInfoService;
        }
        /// <summary>
        /// 分页获取器材信息
        /// </summary>
        /// <param name="equipmentParams"></param>
        /// <returns></returns>
        [HttpGet("GetEquipmentsPages")]
        [Authorize]
        public async Task<IActionResult> GetEquipmentsPages([FromQuery]EquipmentParams equipmentParams)
        {
            EquipmentSearch equipmentSearch = new EquipmentSearch()
            {
                EquipmentName = equipmentParams.EquipmentName,
                EquipmentModel=equipmentParams.EquipmentModel,
                Order=equipmentParams.Order,
                PageIndex=equipmentParams.PageIndex,
                PageSize=equipmentParams.PageSize,
                TotalCount=0
            };
            var equipmentsList =await equipmentsService.LoadPageSearchEntities(equipmentSearch, false).Select(e => new { Id = e.Id, EquipmentName = e.EquipmentName, EquipmentCode = e.EquipmentCode, EquipmentState = e.EquipmentStatus==0?"正常使用" : e.EquipmentStatus == 1 ?"维修中" : e.EquipmentStatus == 2 ?"报废":"闲置", EquipmentTypeName = e.EquipmentTypes!=null? e.EquipmentTypes.EquipmentTypeName:"无类型" }).ToListAsync();

            return Ok(new ApiResult<object>() { Success = true, Message = "器材分页获取成功", Data = new { Rows = equipmentsList, Total = equipmentSearch.TotalCount } });

        }
        /// <summary>
        /// 添加器材
        /// </summary>
        /// <param name="equipmentsDTO"></param>
        /// <returns></returns>
        [HttpPost("CreateEquipment")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateEquipment([FromBody] EquipmentsDTO equipmentsDTO)
        {
            //查找对应的器材类型
            var equipmentType = await equipmentTypeService.LoadEntities(e => e.EquipmentTypeName == equipmentsDTO.EquipmentTypeName&&e.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            Equipments equipments = new Equipments()
            {
                EquipmentName = equipmentsDTO.EquipmentName,
                EquipmentCode = equipmentsDTO.EquipmentCode,
                EquipmentBrand = equipmentsDTO.EquipmentBrand,
                EquipmentModel = equipmentsDTO.EquipmentModel,
                PurchasePrice = equipmentsDTO.PurchasePrice,
                Location = equipmentsDTO.Location,
                Remark = equipmentsDTO.Remark,
                EquipmentTypes = equipmentType,
                EquipmentStatus = 3,
                PurchaseDate = DateTime.Now,
                CreateDate= DateTime.Now
            };
            //插入数据，并跟新到数据库
            await equipmentsService.AddEntity(equipments);
            int v = await equipmentsService.SaveChangesAsync();
            var equip= await equipmentsService.LoadEntities(e => e.EquipmentCode == equipments.EquipmentCode && e.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            var financeInfo = new FinanceInfo()
            {
                FinanceType = 1,
                TypeName = "器材支出",
                Amount = (double)equipments.PurchasePrice,
                FinanceCode = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999),
                RelatedCode = equip.Id,
                RelatedType = "equipment",
                Remark = equipmentsDTO.Remark,
                CreateDate = DateTime.Now
            };
            //财务信息插入
            bool v1 = await financeInfoService.AddEntity(financeInfo);
            if (v>0&&v1)
            {
                return Ok(new ApiResult<string>() { Success = true, Message = "新增器材成功", Data = null, Code = 200 });
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "新增器材失败", Data = null, Code = 400 });
            }
        }
        /// <summary>
        /// 根据id获取器材
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetEquipmentById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetEquipmentById([FromRoute]int id)
        {
            var equipment = await equipmentsService.LoadEntities(e => e.Id == id && e.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).Include(t => t.EquipmentTypes).Select(t =>new {t.Id, t.EquipmentCode, t.EquipmentName,t.EquipmentBrand,t.EquipmentModel,t.PurchaseDate,t.PurchasePrice,t.Location,t.EquipmentStatus,t.LastMaintenanceDate,t.Remark,t.EquipmentTypes}).FirstOrDefaultAsync();
            if(equipment!=null)
            {
                return Ok(new ApiResult<object>() { Success = true, Message = "器材获取成功", Data = equipment, Code = 200 });
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "器材获取失败，未找到器材", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 根据id软删除器材
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteEquipmentById/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> DeleteEquipmentById([FromRoute]int id)
        {
            var equipment= await equipmentsService.LoadEntities(e => e.Id == id && e.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(equipment!=null)
            {
                equipment.IsDeleted = true;
                bool v = await equipmentsService.UpdateEntity(equipment);
                if(v)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "删除器材成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "删除器材失败", Data = null, Code = 400 });
                }
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到要删除的器材", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 根据id改变器材的状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPatch("ChangeEquipmentStateById/{id}/{state}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> ChangeEquipmentStateById([FromRoute]int id, [FromRoute]int state)
        {
            var equipment= await equipmentsService.LoadEntities(e => e.Id == id && e.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if (equipment!=null)
            {
                //当修改的状态是Fixing（维修中）时，设置当前时间为最后维护日期
                if (state == Convert.ToInt32(EquipmentStatusEnum.Fixing))
                {
                    equipment.LastMaintenanceDate = DateTime.Now;
                    equipment.EditDate = DateTime.Now;
                }
                equipment.EquipmentStatus = state;
                bool v = await equipmentsService.UpdateEntity(equipment);
                
                if (v)
                {
                    return Ok(new ApiResult<string>() { Success = true, Message = "状态设置成功", Data = null, Code = 200 });
                }
                else
                {
                    return BadRequest(new ApiResult<string>() { Success = false, Message = "状态设置失败", Data = null, Code = 400 });
                }
            }
            else
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到要修改状态的器材", Data = null, Code = 404 });
            }
        }
        /// <summary>
        /// 根据Id编辑器材信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="equipmentsDTO"></param>
        /// <returns></returns>
        [HttpPatch("EditEquipmentById/{id}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> EditEquipmentById([FromRoute]int id, [FromBody]EquipmentsDTO equipmentsDTO)
        {
            var equipment= await equipmentsService.LoadEntities(e => e.Id == id && e.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            var equipmentType= await equipmentTypeService.LoadEntities(et => et.EquipmentTypeName == equipmentsDTO.EquipmentTypeName && et.IsDeleted==Convert.ToBoolean(DelFlagEnum.Nomal)).FirstOrDefaultAsync();
            if(equipment==null)
            {
                return NotFound(new ApiResult<string>() { Success = false, Message = "未找到器材", Data = null, Code = 404 });
            }
            equipment.EquipmentName = equipmentsDTO.EquipmentName;
            equipment.EquipmentCode = equipmentsDTO.EquipmentCode;
            equipment.EquipmentModel = equipmentsDTO.EquipmentModel;
            equipment.EquipmentBrand = equipmentsDTO.EquipmentBrand;
            equipment.EquipmentTypes = equipmentType;
            equipment.Location = equipmentsDTO.Location;
            equipment.PurchasePrice = equipmentsDTO.PurchasePrice;
            equipment.Remark = equipmentsDTO.Remark;
            equipment.EditDate = DateTime.Now;
            bool v = await equipmentsService.UpdateEntity(equipment);
            
            if(v)
            {
                return Ok(new ApiResult<string>() { Success = true, Message = "编辑成功", Data = null, Code = 200 });
            }
            else
            {
                return BadRequest(new ApiResult<string>() { Success = false, Message = "编辑失败", Data = null, Code = 400 });
            }
        }
    }
}
