using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class PermissionInfo:BaseEntity<int>
    {
        /// <summary>
        /// 权限名称,如果是菜单权限，表示的就是菜单名称, 如果是按钮，表示的就是按钮的名称，如果是API,则表示API的名称
        /// </summary>
        public string? PermissionName { get; set; }
        /// <summary>
        /// 权限类型:1为菜单，2为功能(按钮),3为API
        /// </summary>
        public int? PermissionType { get; set; }

        /// <summary>
        /// 权限编码，有响应的编码，就表示拥有该权限，这里主要用于前端路由权限的判断
        /// </summary>
        public string? PermissionCode { get; set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string? PermissionDescription { get; set; }
        /// <summary>
        /// 权限是有父子关系的，例如当单击某个菜单，呈现一个页面，页面中有按钮，单击按钮的时候会访问某个API
        /// 这样按钮，API,这些权限都是菜单的子权限。
        /// </summary>

        public int? ParentId { get; set; }
        /// <summary>
        /// 一个权限有多个角色
        /// </summary>
        public List<RoleInfo> RoleInfos { get; set; } = new List<RoleInfo>();
        /// <summary>
        /// 是否审批中,删除审批状态：0=未审批, 1=审批中, 2=审批通过
        /// </summary>
        public int IsDeleteApproving { get; set; }
    }
}
