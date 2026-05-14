using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class PermissionInfoConfig : IEntityTypeConfiguration<PermissionInfo>
    {
        public void Configure(EntityTypeBuilder<PermissionInfo> builder)
        {
            builder.ToTable("PermissionInfos");
            builder.Property(x => x.PermissionName).HasMaxLength(20).IsRequired();
            builder.Property(x => x.PermissionCode).HasMaxLength(20).IsRequired();
            builder.Property(x => x.PermissionDescription).HasMaxLength(500).IsRequired();
            // 中间表是T_RoleInfos_Permissions
            builder.HasMany<RoleInfo>(s => s.RoleInfos).WithMany(s => s.PermissionInfos).UsingEntity(a => a.ToTable("PermissionInfoRoleInfo"));

            
        }
    }
}
