using Gms.Entity;
using Gms.IRepository;
using Gms.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Service
{
    public class PermissionMenuService:BaseService<PermissionMenu>,IPermissionMenuService
    {
        private readonly IPermissionMenuRepository permissionMenuRepository;

        public PermissionMenuService(IPermissionMenuRepository permissionMenuRepository)
        {
            base.Repository = permissionMenuRepository;
            this.permissionMenuRepository = permissionMenuRepository;
        }
    }
}
