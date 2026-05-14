using Gms.Entity;
using Gms.Entity.Enum;
using Gms.Entity.Search;
using Gms.IRepository;
using Gms.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Service
{
    public class EquipmentsService:BaseService<Equipments>,IEquipmentsService
    {
        private readonly IEquipmentsRepository equipmentsRepository;

        public EquipmentsService(IEquipmentsRepository equipmentsRepository)
        {
            base.Repository = equipmentsRepository;
            this.equipmentsRepository = equipmentsRepository;
        }
        /// <summary>
        /// 分页获取器材信息
        /// </summary>
        /// <param name="equipmentSearch"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public IQueryable<Equipments> LoadPageSearchEntities(EquipmentSearch equipmentSearch, bool isDeleted)
        {
            var temp = equipmentsRepository.LoadEntities(e => e.IsDeleted == Convert.ToBoolean(DelFlagEnum.Nomal));
            if(!string.IsNullOrEmpty(equipmentSearch.EquipmentName))
            {
                temp = temp.Where(e => e.EquipmentName.Contains(equipmentSearch.EquipmentName));
            }
            if(!string.IsNullOrEmpty(equipmentSearch.EquipmentModel))
            {
                temp = temp.Where(e => e.EquipmentModel.Contains(equipmentSearch.EquipmentModel));
            }
            equipmentSearch.TotalCount = temp.Count();
            int skip = (equipmentSearch.PageIndex - 1) * equipmentSearch.PageSize;
            int take = equipmentSearch.PageSize;


            temp = !equipmentSearch.Order
                    ? temp.OrderBy(a => a.Id).Include(e => e.EquipmentTypes).Skip(skip).Take(take)
                    : temp.OrderByDescending(a => a.Id).Include(e => e.EquipmentTypes).Skip(skip).Take(take);
            return temp;
        }
    }
}
