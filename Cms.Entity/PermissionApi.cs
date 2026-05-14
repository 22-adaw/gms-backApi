using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class PermissionApi:BaseEntity<int>
    {
        /// <summary>
        /// API的地址
        /// </summary>
        public string? ApiUrl { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        public string? ApiMethod { get; set; }
        //-----------------添加了PermissionInfo
        public PermissionInfo? PermissionInfo { get; set; }
        //---------------显式的指定外键
        public int? PermissionId { get; set; }
    }
}
