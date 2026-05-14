using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class VipCard
    {
        /// <summary>
        /// 卡编号
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public int? CardNum { get; set; }
        /// <summary>
        /// 有效期开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 有效期结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 剩余次数（次卡使用）
        /// </summary>
        public int RemainTimes { get; set; }
        /// <summary>
        /// 冻结状态 0表示未冻结，1表示冻结
        /// </summary>
        public int FreezeStatus { get; set; }
        
        /// <summary>
        /// 卡主信息
        /// </summary>
        public VipInfo VipInfo { get; set; }
        /// <summary>
        /// 卡类型外键 关联 VipCardType 主键
        /// </summary>
        public int? VipCardTypeId { get; set; }
        /// <summary>
        /// 卡类型
        /// </summary>
        public VipCardType VipCardType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? EditTime { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public float LeftMoney { get; set; } = 0;
        /// <summary>
        /// 是否提醒到期
        /// </summary>
        public bool LastReminderSent { get; set; } = false;
        /// <summary>
        /// 提醒时间
        /// </summary>
        public DateTime? LastReminderTime { get; set; }
    }
}
