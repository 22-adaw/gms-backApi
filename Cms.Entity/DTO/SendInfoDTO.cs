using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public class SendInfoDTO
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? DepartmentName { get; set; }
        public string? Manager { get; set; }
        public string? RealName { get; set; }
        public string? PhotoUrl { get; set; }
        public List<string>? RoleName { get; set; }
        public bool IsClient { get; set; }
        public DateTime? EndDate { get; set; }
        public int FreezeStatus { get; set; }
        public int? CardNum { get; set; }
        public float LeftMoney { get; set; }
        public int RemainTimes { get; set; }
    }
}
