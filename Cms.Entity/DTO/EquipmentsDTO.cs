using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public class EquipmentsDTO
    {
        public string? EquipmentName { get; set; }
        public string? EquipmentCode { get; set; }
        public string? EquipmentBrand { get; set; }
        public string? EquipmentModel { get; set; }
        public double? PurchasePrice { get; set; }
        public string? Location { get; set; }
        public string? Remark { get; set; }
        public string? EquipmentTypeName { get; set; }
    }
}
