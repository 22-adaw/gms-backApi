using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.Enum
{
    public enum ApprovalStatusEnum
    {
        /// 审批状态: 0:待审批 1:审批中 2:已通过 3:已驳回 4:已取消 5:已超时
        /// <summary>
        /// 0：未审批，可删除
        /// </summary>
        UnApproved = 0,

        /// <summary>
        /// 1：审批中，禁用删除
        /// </summary>
        Approving = 1,

        /// <summary>
        /// 2：审批通过，可执行删除
        /// </summary>
        Approved = 2,
        /// <summary>
        /// 已驳回
        /// </summary>
            Rejected=3,
            /// <summary>
            /// 已取消
            /// </summary>
            Canceled=4,
        /// <summary>
        /// 已超时
        /// </summary>
        Expired= 5
    }
}
