using Gms.Entity;
using Gms.EntityFrameworkCore;
using Gms.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Repository
{
    public class PermissionDeleteApprovalStepRepository : BaseRepository<PermissionDeleteApprovalStep>, IPermissionDeleteApprovalStepRepository
    {
        public PermissionDeleteApprovalStepRepository(MyDbContext myDbContext) : base(myDbContext)
        {
        }
    }
}
