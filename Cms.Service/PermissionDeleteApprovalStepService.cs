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
    public class PermissionDeleteApprovalStepService:BaseService<PermissionDeleteApprovalStep>,IPermissionDeleteApprovalStepService
    {
        private readonly IPermissionDeleteApprovalStepRepository permissionDeleteApprovalStepRepository;

        public PermissionDeleteApprovalStepService(IPermissionDeleteApprovalStepRepository permissionDeleteApprovalStepRepository)
        {
            base.Repository = permissionDeleteApprovalStepRepository;
            this.permissionDeleteApprovalStepRepository = permissionDeleteApprovalStepRepository;
        }
    }
}
