using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class PermissionPoint:BaseEntity<int>
    {
        /// <summary>
        /// 按钮的样式
        /// </summary>
        public string? PointClass { get; set; }
        /// <summary>
        /// 按钮的图标
        /// </summary>
        public string? PointIcon { get; set; }

        /// <summary>
        /// 按钮的状态
        /// </summary>
        public int? PointStatus { get; set; }
        //-----------------添加了PermissionInfo
        public PermissionInfo? PermissionInfo { get; set; }
        //---------------显式的指定外键
        public int? PermissionId { get; set; }
    }
}
