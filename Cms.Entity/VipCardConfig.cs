using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class VipCardConfig : IEntityTypeConfiguration<VipCard>
    {
        public void Configure(EntityTypeBuilder<VipCard> builder)
        {
            builder.ToTable("VipCards");
            builder.HasKey(vc => vc.Id);
            builder.Property(vc => vc.StartDate).HasMaxLength(500);
            builder.Property(vc => vc.EndDate).HasMaxLength(500);
            builder.Property(vc => vc.RemainTimes).HasMaxLength(500);
            builder.Property(vc => vc.FreezeStatus).HasMaxLength(5).IsRequired();
            builder.Property(vc=>vc.CreateTime).HasMaxLength(500).IsRequired();
            builder.Property(vc=>vc.EditTime).HasMaxLength(500);
            builder.Property(vc => vc.IsDeleted).IsRequired();
            builder.Property(vc => vc.CardNum).HasMaxLength(50).IsRequired();
            builder.Property(vc => vc.LeftMoney).HasPrecision(18, 2).HasDefaultValue(0f);
            builder.HasOne<VipCardType>(vc => vc.VipCardType).WithMany(vct => vct.VipCards).HasForeignKey(vc=>vc.VipCardTypeId).IsRequired();
        }
    }
}
