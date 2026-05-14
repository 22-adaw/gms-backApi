using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public class VipInfoDTO
    {
        public string VipName { get; set; }
        public string? VipPhone { get; set; }
        public string? VipPassword { get; set; }
        public string? Gender { get; set; }
        public string? VipEmail { get; set; }
        public string? Status { get; set; }
        public bool IsAdminManifest { get; set; }
    }
}
