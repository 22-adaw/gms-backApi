using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class RoleInfoConfig : IEntityTypeConfiguration<RoleInfo>
    {
        public void Configure(EntityTypeBuilder<RoleInfo> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(x => x.Id);//配置主键
            builder.Property(x => x.Id).ValueGeneratedOnAdd();//主键自增
            builder.Property(x => x.RoleName).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Remark).IsRequired().HasMaxLength(500);
            // 配置了多对多的关系 
            //中间表是T_UserInfos_RoleInfos
            builder.HasMany<UserInfo>(s => s.UserInfos).WithMany(s => s.RoleInfos).UsingEntity(a => a.ToTable("RoleInfoUserInfo"));//中间表名
        }
    }
}
