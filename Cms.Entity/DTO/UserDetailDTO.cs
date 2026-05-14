using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public class UserDetailDTO:UserInfoDTO
    {
        public DateTime? CreateDate { get; set; }
        public DateTime? EditDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
