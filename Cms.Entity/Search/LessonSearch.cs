using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.Search
{
    public class LessonSearch:BaseSearch
    {
        public string? LessonName { get; set; }
        public int? Status { get; set; }
    }
}
