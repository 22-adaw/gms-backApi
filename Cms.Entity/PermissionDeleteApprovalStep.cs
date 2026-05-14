using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class PermissionDeleteApprovalStep:BaseEntity<int>
    {
        
        /// <summary>
        /// 审批编号
        /// </summary>
        public int ApprovalId { get; set; }

        /// <summary>
        /// 审批顺序：1,2,3...
        /// </summary>
        public int StepOrder { get; set; }
        /// <summary>
        /// 管理员id
        /// </summary>
        public int AdminUserId { get; set; }
        /// <summary>
        /// 管理员姓名
        /// </summary>
        public string AdminName { get; set; }
        /// <summary>
        /// 管理员邮箱
        /// </summary>
        public string AdminEmail { get; set; }

        /// <summary>
        /// 审批唯一令牌（安全凭证）
        /// </summary>
        
        public string ApprovalToken { get; set; }

        /// <summary>
        /// 单个步骤超时时间
        /// </summary>
        public DateTime TokenExpireTime { get; set; }

        /// <summary>
        /// 审批结果: NULL:未处理  true:通过  false:驳回
        /// </summary>
        public int IsApproved { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? ApprovalTime { get; set; }

        
        /// <summary>
        /// 通知是否已发送
        /// </summary>
        public bool NotificationSent { get; set; }

        /// <summary>
        /// 通知发送时间
        /// </summary>
        public DateTime? NotificationTime { get; set; }

        /// <summary>
        /// 导航属性：关联的审批主表
        /// </summary>
        public virtual PermissionDeleteApproval Approval { get; set; }
    }
}
