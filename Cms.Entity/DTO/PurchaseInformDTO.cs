using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public class PurchaseInformDTO
    {
        public string CardTypeName { get; set; }
        public string Email { get; set; }
        public int? CardNum { get; set; }
        public float LeftMoney { get; set; }
        public DateTime? EndDate { get; set; }
        public int RemainTime { get; set; }
        public string Remark { get; set; }
        public float DisCountRate { get; set; }
        public string VipName { get; set; }
    }
}
