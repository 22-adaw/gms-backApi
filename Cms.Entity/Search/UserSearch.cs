using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.Search
{
    public class UserSearch:BaseSearch
    {
        public string? UserName { get; set; }
        public string? RealName { get; set; }
        public string? UserPhone { get; set; }
        public string? DepartmentName { get; set; }
        public string? UserEmail { get; set; }
    }
}
