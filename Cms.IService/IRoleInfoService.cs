using Gms.Entity;
using Gms.Entity.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.IService
{
    public interface IRoleInfoService:IBaseService<RoleInfo>
    {
        /// <summary>
        /// 分页获取角色信息
        /// </summary>
        /// <param name="roleSearch"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        IQueryable<RoleInfo> LoadPageSearchEntities(RoleSearch roleSearch,bool isDeleted);
        /// <summary>
        /// 为角色分配权限
        /// </summary>
        /// <param name="roleInfo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<bool> SetRolePermission(RoleInfo roleInfo, List<int> list);
    }
}
