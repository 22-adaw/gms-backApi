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
    public class PermissionPointService:BaseService<PermissionPoint>,IPermissionPointService
    {
        private readonly IPermissionPointRepository permissionPointRepository;

        public PermissionPointService(IPermissionPointRepository permissionPointRepository)
        {
            base.Repository = permissionPointRepository;
            this.permissionPointRepository = permissionPointRepository;
        }
    }
}
