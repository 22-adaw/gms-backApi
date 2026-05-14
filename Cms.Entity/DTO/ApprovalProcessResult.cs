using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public class ApprovalProcessResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int? CurrentStep { get; set; }
        public int? TotalSteps { get; set; }
    }
}
