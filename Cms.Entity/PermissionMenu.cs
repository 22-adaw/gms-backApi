using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class PermissionMenu:BaseEntity<int>
    {
        // 菜单图标
        public string? MenuIcon { get; set; }

        //排序
        public string? MenunOrder { get; set; }
        //-----------------添加了PermissionInfo
        public PermissionInfo? PermissionInfo { get; set; }
        //---------------显式的指定外键
        public int? PermissionId { get; set; }
    }
}
