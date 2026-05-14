using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class PermissionPointConfig : IEntityTypeConfiguration<PermissionPoint>
    {
        public void Configure(EntityTypeBuilder<PermissionPoint> builder)
        {
            builder.ToTable("PermissionPoints");
            builder.Property(x => x.PointClass).HasMaxLength(200);
            builder.Property(x => x.PointIcon).HasMaxLength(20);
            builder.HasOne(p => p.PermissionInfo)
                   .WithMany()
                   .HasForeignKey(p => p.PermissionId);
        }
    }
}
