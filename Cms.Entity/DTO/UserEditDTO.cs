using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public class UserEditDTO
    {
        public string? PhotoUrl { get; set; }
        public string? RealName { get; set; }
        public string? UserName { get; set; }
        public string? UserPhone { get; set; }
        public string? UserEmail { get; set; }
        public string? Gender { get; set; }
        public string? UserPwd { get; set; }
        public string? ConfirmPwd { get; set; }
        public string? OldPwd { get; set; }
        public bool IsClient { get; set; }
    }
}
