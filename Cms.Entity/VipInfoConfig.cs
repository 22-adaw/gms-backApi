using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class VipInfoConfig : IEntityTypeConfiguration<VipInfo>
    {
        public void Configure(EntityTypeBuilder<VipInfo> builder)
        {
            builder.ToTable("VipInfos");
            builder.Property(v => v.VipName).HasMaxLength(50).IsRequired();
            builder.Property(v => v.Gender).HasMaxLength(5).IsRequired();
            builder.Property(v => v.VipPhone).HasMaxLength(50);
            builder.Property(v => v.VipEmail).HasMaxLength(500);
            builder.Property(v => v.VipPassword).HasMaxLength(16);
            builder.Property(v => v.Status).HasMaxLength(5).IsRequired().HasDefaultValue("0");
            builder.HasOne(v => v.VipCard).WithOne(vc => vc.VipInfo).HasForeignKey<VipInfo>(v=>v.VipCardId);
        }
    }
}
