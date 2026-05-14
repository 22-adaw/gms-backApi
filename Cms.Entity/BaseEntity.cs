using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class BaseEntity<TKey>
    {
        public TKey Id { get; set; }
        public DateTime? CreateDate { get; set; }

        public DateTime? EditDate { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
