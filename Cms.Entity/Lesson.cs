using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class Lesson:BaseEntity<int>
    {
        /// <summary>
        /// 课程名
        /// </summary>
        public string? LessonName { get; set; }
        /// <summary>
        /// 课程描述
        /// </summary>
        public string? LessonDesc { get; set; }
        /// <summary>
        /// 教练
        /// </summary>
        public UserInfo Couch { get; set; }
        /// <summary>
        /// 最大参与人数
        /// </summary>
        public int? MaxParticipants { get; set; }
        /// <summary>
        /// 参与会员
        /// </summary>
        public List<VipInfo>? VipInfos { get; set; } = new List<VipInfo>();
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; } 
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; } 
        /// <summary>
        /// 价格
        /// </summary>
        public float Price { get; set; }
        /// <summary>
        /// 状态（0-未开始，1-进行中，2-已结束）
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// 教练id外键
        /// </summary>
        public int CoachId { get; set; }
        /// <summary>
        /// 已参与人数
        /// </summary>
        public int EnrolledCount { get; set; }
    }
}
