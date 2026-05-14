using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class LessonConfig : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.ToTable("Lessons");
            builder.Property(l=>l.LessonName).HasMaxLength(100).IsRequired();
            builder.Property(l => l.LessonDesc).HasMaxLength(500);
            builder.Property(l => l.Price).HasDefaultValue(0);
            builder.Property(l => l.Status).HasDefaultValue(0);
            builder.HasOne(l => l.Couch)
       .WithMany() // 反向配置，教练用户表不需要反向集合就填空
       .HasForeignKey(l => l.CoachId);
            builder.HasMany(l => l.VipInfos).WithMany(v => v.Lessons).UsingEntity(j => j.ToTable("LessonVipInfo"));
        }
    }
}
