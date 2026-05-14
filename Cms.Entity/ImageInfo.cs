using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    [Table("ImageInfo")]
    public class ImageInfo:BaseEntity<int>
    {
        public string ImageUrl { get; set; }
        public string FileHash { get; set; }
    }
}
