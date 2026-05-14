using Gms.Entity;
using Gms.Entity.Search;
using Gms.IRepository;
using Gms.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Service
{
    public class RoleInfoService:BaseService<RoleInfo>,IRoleInfoService
    {
        private readonly IRoleInfoRepository roleInfoRepository;
        private readonly IPermissionInfoService permissionInfoService;

        public RoleInfoService(IRoleInfoRepository roleInfoRepository, IPermissionInfoService permissionInfoService)
        {
            base.Repository = roleInfoRepository;
            this.roleInfoRepository = roleInfoRepository;
            this.permissionInfoService = permissionInfoService;
        }
        /// <summary>
        /// 分页获取角色信息
        /// </summary>
        /// <param name="roleSearch"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public IQueryable<RoleInfo> LoadPageSearchEntities(RoleSearch roleSearch, bool isDeleted)
        {
            var temp= roleInfoRepository.LoadEntities(r => r.IsDeleted == false);
            if(!string.IsNullOrEmpty(roleSearch.RoleName))
            {
                temp = temp.Where(a => a.RoleName.Contains(roleSearch.RoleName));
            }
            if(!string.IsNullOrEmpty(roleSearch.Remark))
            {
                temp = temp.Where(a => a.Remark.Contains(roleSearch.Remark));
            }
            roleSearch.TotalCount = temp.Count();
            int skip = (roleSearch.PageIndex - 1) * roleSearch.PageSize;
            int take = roleSearch.PageSize;


            temp = !roleSearch.Order
                ? temp.OrderBy(a => a.Id).ToList().Skip(skip).Take(take).AsQueryable()
                : temp.OrderByDescending(a => a.Id).ToList().Skip(skip).Take(take).AsQueryable();
            return temp;
        }
        /// <summary>
        /// 分配权限具体实现
        /// </summary>
        /// <param name="roleInfo"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<bool> SetRolePermission(RoleInfo roleInfo, List<int> list)
        {
            await roleInfoRepository.LoadEntities(r => r.Id == roleInfo.Id).Include(r=>r.PermissionInfos).FirstOrDefaultAsync();
            //清空集合
            roleInfo.PermissionInfos.Clear();
            if (list.Count == 1 && list[0] == 0)
            {
                return true;
            }
            var permissiomInfos = await permissionInfoService.LoadEntities(p => list.Contains(p.Id) && p.IsDeleted == false).ToListAsync();
            foreach (var permissiomInfo in permissiomInfos)
            {
                roleInfo.PermissionInfos.Add(permissiomInfo);
            }
            return true;
        }
    }
}
