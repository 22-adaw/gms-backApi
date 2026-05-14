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
    public class PermissionDeleteApprovalService:BaseService<PermissionDeleteApproval>,IPermissionDeleteApprovalService
    {
        private readonly IPermissionDeleteApprovalRepository permissionDeleteApprovalRepository;

        public PermissionDeleteApprovalService(IPermissionDeleteApprovalRepository permissionDeleteApprovalRepository)
        {
            base.Repository = permissionDeleteApprovalRepository;
            this.permissionDeleteApprovalRepository = permissionDeleteApprovalRepository;
        }
    }
}
