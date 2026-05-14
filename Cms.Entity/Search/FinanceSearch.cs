using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.Search
{
    public class FinanceSearch:BaseSearch
    {
        public int? FinanceType { get; set; }
        public string? TypeName { get; set; }
        public int? RelatedCode { get; set; }
    }
}
