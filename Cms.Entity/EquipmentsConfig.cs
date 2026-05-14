using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class EquipmentsConfig : IEntityTypeConfiguration<Equipments>
    {
        public void Configure(EntityTypeBuilder<Equipments> builder)
        {
            builder.ToTable("Equipments");
            builder.Property(e => e.EquipmentName).HasMaxLength(50).IsRequired();
            builder.Property(e => e.EquipmentCode).HasMaxLength(50).IsRequired();
            builder.Property(e => e.EquipmentBrand).HasMaxLength(50);
            builder.Property(e => e.EquipmentModel).HasMaxLength(100);
            builder.Property(e => e.PurchaseDate).HasMaxLength(500).IsRequired();
            builder.Property(e => e.PurchasePrice).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Location).HasMaxLength(500);
            builder.Property(e => e.EquipmentStatus).HasMaxLength(10).IsRequired();
            builder.Property(e => e.LastMaintenanceDate).HasMaxLength(500);
            builder.Property(e => e.Remark).HasMaxLength(500);
            builder.HasOne<EquipmentType>(et => et.EquipmentTypes).WithMany( e=> e.Equipments).IsRequired();
        }
    }
}
