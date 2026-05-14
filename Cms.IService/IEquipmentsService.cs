using Gms.Entity;
using Gms.Entity.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.IService
{
    public interface IEquipmentsService:IBaseService<Equipments>
    {
        /// <summary>
        /// 分页获取器材信息
        /// </summary>
        /// <param name="equipments"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        IQueryable<Equipments> LoadPageSearchEntities(EquipmentSearch equipmentSearch, bool isDeleted);
    }
}
