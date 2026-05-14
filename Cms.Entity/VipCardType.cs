using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class VipCardType:BaseEntity<int>
    {
        /// <summary>
        /// 卡类型名称
        /// </summary>
        public string? VipCardTypeName { get; set; }
        /// <summary>
        /// 卡类型编号 0表示时长卡，1表示次卡
        /// </summary>
        public int? VipCardTypeCode { get; set; }
        /// <summary>
        /// 折扣比例
        /// </summary>
        public float DiscountRate { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public float Price { get; set; }
        /// <summary>
        /// 可使用的天数
        /// </summary>
        public int? UseDays { get; set; } = 0;
        /// <summary>
        /// 可使用的次数
        /// </summary>
        public int UseTimes { get; set; } = 0;
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 一个类型下有多张卡
        /// </summary>
        public List<VipCard>? VipCards { get; set; } = new List<VipCard>();
    }
}
