using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class PermissionMenuConfig : IEntityTypeConfiguration<PermissionMenu>
    {
        public void Configure(EntityTypeBuilder<PermissionMenu> builder)
        {
            builder.ToTable("PermissionMenus");
            builder.Property(x => x.MenuIcon).HasMaxLength(200);
            builder.Property(x => x.MenunOrder).HasMaxLength(20);
            builder.HasOne(p => p.PermissionInfo)
                   .WithMany()
                   .HasForeignKey(p => p.PermissionId);
        }
    }
}
