using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class PermissionApiConfig : IEntityTypeConfiguration<PermissionApi>
    {
        public void Configure(EntityTypeBuilder<PermissionApi> builder)
        {
            builder.ToTable("PermissionApis");
            builder.Property(x => x.ApiUrl).HasMaxLength(200).IsRequired();
            builder.Property(x => x.ApiMethod).HasMaxLength(20).IsRequired();
            builder.HasOne(p => p.PermissionInfo)
                   .WithMany()
                   .HasForeignKey(p => p.PermissionId);
        }
    }
}
