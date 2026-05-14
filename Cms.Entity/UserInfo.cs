using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class UserInfo:BaseEntity<int>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string? UserPassword { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? UserEmail { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string? RealName { get; set; }
        /// <summary>
        /// -----------------一个员工只能属于一个部门
        /// </summary>
        public Departments? Department { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string? UserPhone { get; set; }

        /// <summary>
        /// 性别：1表示男，2表示女
        /// </summary>
        public int? Gender { get; set; }
        /// <summary>
        /// 用户头像，存储头像路径
        /// </summary>
        public string? PhotoUrl { get; set; }
        /// <summary>
        /// 部门ID（外键）
        /// </summary>
        public int? DepartmentId { get; set; } // 显式定义外键字段


        /// <summary>
        /// 一个员工有多个角色
        /// </summary>
        public List<RoleInfo>? RoleInfos { get; set; } = new List<RoleInfo>();
        /// <summary>
        /// 标识是否是客户
        /// </summary>
        public bool IsClient { get; set; }
    }
}
