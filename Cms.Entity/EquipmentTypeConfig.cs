using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class EquipmentTypeConfig : IEntityTypeConfiguration<EquipmentType>
    {
        public void Configure(EntityTypeBuilder<EquipmentType> builder)
        {
            builder.ToTable("EquipmentTypes");
            builder.Property(t => t.EquipmentTypeName).HasMaxLength(50).IsRequired();
            builder.Property(t => t.EquipmentTypeCode).HasMaxLength(50).IsRequired();
            builder.Property(t => t.Description).HasMaxLength(500);
            
        }
    }
}
