using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public  class FinanceInfoDTO
    {
        public double Amount { get; set; }
        public int? FinanceType { get; set; }
        public string? Remark { get; set; }
        public int? TypeName { get; set; }

    }
}
