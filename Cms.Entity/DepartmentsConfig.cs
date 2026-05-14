using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class DepartmentsConfig
    {
        public void Configure(EntityTypeBuilder<Departments> builder)
        {
            builder.ToTable("Departments");
            builder.Property(x => x.DepartmentName).HasMaxLength(20).IsRequired();
            builder.Property(x => x.DepartmentCode).HasMaxLength(20).IsRequired();
            builder.Property(x => x.DepartmentDescription).IsRequired();
            builder.Property(x => x.City).HasMaxLength(16).IsRequired();
            builder.Property(x => x.Manager).HasMaxLength(16).IsRequired();

        }
    }
}
