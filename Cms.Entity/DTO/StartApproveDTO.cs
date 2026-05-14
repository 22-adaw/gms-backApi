using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public class StartApproveDTO
    {
        public UserInfo Requester { get; set; }
        public PermissionInfo RequestedPermission { get; set; }
    }
}
