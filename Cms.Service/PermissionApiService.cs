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
    internal class PermissionApiService:BaseService<PermissionApi>,IPermissionApiService
    {
        private readonly IPermissionApiRepository permissionApiRepository;

        public PermissionApiService(IPermissionApiRepository permissionApiRepository)
        {
            base.Repository = permissionApiRepository;
            this.permissionApiRepository = permissionApiRepository;
        }
    }
}
