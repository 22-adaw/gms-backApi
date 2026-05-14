using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class VipInfo:BaseEntity<int>
    {
       /// <summary>
       /// 会员姓名
       /// </summary>
        public string? VipName { get; set; }
        /// <summary>
        /// 性别 1表示男，0表示女
        /// </summary>
        public string? Gender { get; set; }
        /// <summary>
        /// 会员手机号
        /// </summary>
        public string? VipPhone { get; set; }
        /// <summary>
        /// 会员密码
        /// </summary>
        public string? VipPassword { get; set; } = "123456";
        /// <summary>
        /// 会员邮箱
        /// </summary>
        public string? VipEmail { get; set; }
        /// <summary>
        /// 会员卡号
        /// </summary>
        public VipCard? VipCard { get; set; }
        /// <summary>
        /// 外键字段，会员卡id
        /// </summary>
        public int? VipCardId { get; set; }
        /// <summary>
        /// 会员状态 0表示正常，1表示流失，2表示禁用
        /// </summary>
        public string? Status { get; set; }
        /// <summary>
        /// 课程
        /// </summary>
        public List<Lesson>? Lessons { get; set; } = new List<Lesson>();

    }
}
