using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class Departments:BaseEntity<int>
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string? DepartmentName { get; set; }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string? DepartmentCode { get; set; }
        /// <summary>
        /// 部门描述
        /// </summary>

        public string? DepartmentDescription { get; set; }
        /// <summary>
        /// 父级部门ID
        /// </summary>
        public long? ParentId { get; set; }
        /// <summary>
        /// 部门所在城市
        /// </summary>

        public string? City { get; set; }
        /// <summary>
        /// 部门负责人
        /// </summary>
        public string? Manager { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool? IsDeleted { get; set; }
        /// <summary>
        /// 部分负责人编号
        /// </summary>
        public int? ManagerId { get; set; }
        /// <summary>
        /// 一个部门下面又很多员工
        /// </summary>

        public List<UserInfo>? UserInfos { get; set; } = new List<UserInfo>();
    }
}
