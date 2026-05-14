using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class FinanceInfoConfig : IEntityTypeConfiguration<FinanceInfo>
    {
        public void Configure(EntityTypeBuilder<FinanceInfo> builder)
        {
            builder.ToTable("FinanceInfos");
            builder.Property(f => f.FinanceType).HasMaxLength(50).IsRequired();
            builder.Property(f => f.TypeName).HasMaxLength(50);
            builder.Property(f => f.Amount).HasMaxLength(500).IsRequired();
            builder.Property(f => f.FinanceCode).HasMaxLength(50).IsRequired();
            builder.Property(f => f.RelatedCode).HasMaxLength(100);
            builder.Property(f => f.RelatedType).HasMaxLength(100);
            builder.Property(f => f.Remark).HasMaxLength(200);
        }
    }
}
