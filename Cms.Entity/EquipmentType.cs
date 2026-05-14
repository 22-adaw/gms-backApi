using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class EquipmentType:BaseEntity<int>
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string? EquipmentTypeName { get; set; }
        /// <summary>
        /// 类型编号
        /// </summary>
        public string? EquipmentTypeCode { get; set; }
        /// <summary>
        /// 类型描述
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 一个类型有多个器材
        /// </summary>
        public List<Equipments> Equipments { get; set; } = new List<Equipments>();
    }
}
