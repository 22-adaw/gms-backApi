using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public class LessonChosedDTO
    {
        public int Id { get; set; }
        public string? LessonName { get; set; }
        public double? Price { get; set; }
        public int? MaxParticipants { get; set; }
    }
}
