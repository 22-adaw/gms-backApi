using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class Equipments:BaseEntity<int>
    {
        /// <summary>
        /// 器材名称
        /// </summary>
        public string? EquipmentName { get; set; }
        /// <summary>
        /// 器材编号
        /// </summary>
        public string? EquipmentCode { get; set; }
        /// <summary>
        /// 器材类型
        /// </summary>
        public EquipmentType? EquipmentTypes { get; set; }
        /// <summary>
        /// 器材品牌
        /// </summary>
        public string? EquipmentBrand { get; set; }
        /// <summary>
        /// 器材型号
        /// </summary>
        public string? EquipmentModel { get; set; }
        /// <summary>
        /// 购买时间
        /// </summary>
        public DateTime? PurchaseDate { get; set; }
        /// <summary>
        /// 价格（元）
        /// </summary>
        public double? PurchasePrice { get; set; }
        /// <summary>
        /// 器材位置
        /// </summary>
        public string? Location { get; set; }
        /// <summary>
        /// 器材状态 0-正常使用 | 1-维修中 | 2-报废 | 3-闲置
        /// </summary>
        public int? EquipmentStatus { get; set; } = 0;
        /// <summary>
        /// 最后维护日期
        /// </summary>
        public DateTime? LastMaintenanceDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

    }
}
