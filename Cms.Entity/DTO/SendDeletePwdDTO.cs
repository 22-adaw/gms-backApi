using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public class SendDeletePwdDTO
    {
        
            public IEnumerable<UserInfo> Users { get; set; }
        public string Password { get; set; }
        public bool IsSuccess {  get; set; }    
    }
}
