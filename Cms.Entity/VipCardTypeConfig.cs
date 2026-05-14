using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class VipCardTypeConfig : IEntityTypeConfiguration<VipCardType>
    {
        public void Configure(EntityTypeBuilder<VipCardType> builder)
        {
            builder.ToTable("VipCardTypes");
            builder.Property(vct => vct.VipCardTypeName).HasMaxLength(50).IsRequired();
            builder.Property(vct => vct.VipCardTypeCode).HasMaxLength(50).IsRequired();
            builder.Property(vct => vct.DiscountRate).HasMaxLength(50).IsRequired();
            builder.Property(vct => vct.Remark).HasMaxLength(500);
            builder.Property(vct => vct.Price).IsRequired();
            builder.Property(vct => vct.UseDays).HasMaxLength(500);
            builder.Property(vct => vct.UseTimes).HasMaxLength(500);

        }
    }
}
