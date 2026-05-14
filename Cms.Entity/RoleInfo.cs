using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class RoleInfo:BaseEntity<long>
    {
        public int Id { get; set; }
        public string? RoleName { get; set; }
        public string? Remark { get; set; }
        // 一个角色属于多个用户
        public List<UserInfo>? UserInfos { get; set; } = new List<UserInfo>();
        //一个角色有多个权限
        public List<PermissionInfo>? PermissionInfos { get; set; } = new List<PermissionInfo>();
    }
}
