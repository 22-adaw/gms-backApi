using Gms.Entity;
using Gms.IRepository;
using Gms.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Service
{
    public class EquipmentTypeService:BaseService<EquipmentType>,IEquipmentTypeService
    {
        private readonly IEquipmentTypeRepository equipmentTypeRepository;

        public EquipmentTypeService(IEquipmentTypeRepository equipmentTypeRepository)
        {
            base.Repository = equipmentTypeRepository;
            this.equipmentTypeRepository = equipmentTypeRepository;
        }
    }
}
