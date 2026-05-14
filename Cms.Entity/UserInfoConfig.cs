using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class UserInfoConfig : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {
            builder.ToTable("Users");
            builder.Property(u=>u.UserName).HasMaxLength(20).IsRequired();
            builder.Property(u => u.UserPassword).HasMaxLength(256).IsRequired();
            builder.Property(u=>u.UserEmail).HasMaxLength(100).IsRequired();
            builder.Property(u=>u.UserPhone).HasMaxLength(11).IsRequired();
            builder.Property(u => u.PhotoUrl).HasMaxLength(100).HasDefaultValue("images/default.jpg");
            builder.Property(u => u.Gender).HasDefaultValue(0);
            builder.Property(x => x.PhotoUrl).HasMaxLength(100);
            // 添加了一对多的关系
            builder.HasOne<Departments>(d => d.Department).WithMany(u => u.UserInfos).IsRequired();
            builder.Property(u => u.RealName).HasMaxLength(50);
        }
    }
}
