using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class PermissionDeleteApproval:BaseEntity<int>
    {
        

        /// <summary>
        /// 当前请求编号
        /// </summary>
        public string RequestId { get; set; }
        /// <summary>
        /// 申请人id
        /// </summary>
        public int RequesterUserId { get; set; }
        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string RequesterName { get; set; }
        /// <summary>
        /// 申请删除权限id
        /// </summary>
        public int TargetPermissionId { get; set; }
        /// <summary>
        /// 申请删除权限名称
        /// </summary>
        public string TargetPermissionName { get; set; }

        /// <summary>
        /// 审批状态: 0:待审批 1:审批中 2:已通过 3:已驳回 4:已取消 5:已超时
        /// </summary>
        public int ApprovalStatus { get; set; }

        /// <summary>
        /// 当前审批步骤（第几步）
        /// </summary>
        public int CurrentStep { get; set; }

        /// <summary>
        /// 总审批步骤数
        /// </summary>
        public int TotalSteps { get; set; }


        /// <summary>
        /// 整体过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompleteTime { get; set; }

        /// <summary>
        /// 驳回步骤
        /// </summary>
        public int? RejectStep { get; set; }


        /// <summary>
        /// 驳回人ID
        /// </summary>
        public int? RejectByUserId { get; set; }

        /// <summary>
        /// 导航属性：审批步骤明细
        /// </summary>
        public virtual ICollection<PermissionDeleteApprovalStep> Steps { get; set; } = new List<PermissionDeleteApprovalStep>();
    }
}
