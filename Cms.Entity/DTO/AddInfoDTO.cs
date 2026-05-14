using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public class AddInformDTO
    {
        public int? CardNum { get; set; }
        public float LeftMoney { get; set; }
        public float Amount { get; set; }
        public string Email { get; set; }
        public string VipName { get; set; }
    }
}
