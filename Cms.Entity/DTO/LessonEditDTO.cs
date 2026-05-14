using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity.DTO
{
    public class LessonEditDTO
    {
        public int Id { get; set; }
        public int CoachId { get; set; }
        public DateTime EndTime { get; set; }
        public string LessonDesc { get; set; }
        public string LessonName { get; set; }
        public int MaxParticipants { get; set; }
        public int Price { get; set; }
        public DateTime StartTime { get; set; }

    }
}
