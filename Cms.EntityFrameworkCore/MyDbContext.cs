using Gms.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gms.EntityFrameworkCore
{
    public class MyDbContext:DbContext
    {
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<RoleInfo> Roles { get; set; }
        public DbSet<PermissionInfo> PermissionInfos { get; set; }
        public DbSet<PermissionApi> PermissionApis { get; set; }
        public DbSet<PermissionMenu> PermissionMenus { get; set; }
        public DbSet<PermissionPoint> PermissionPoints { get; set; }
        public DbSet<Equipments> Equipments { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<VipInfo> VipInfos { get; set; }
        public DbSet<VipCard> VipCards { get; set; }
        public DbSet<VipCardType> VipCardTypes { get; set; }
        public DbSet<FinanceInfo> FinanceInfos { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<ImageInfo> ImageInfos { get; set; }
        public DbSet<PermissionDeleteApproval> PermissionDeleteApprovals { get; set; }
        public DbSet<PermissionDeleteApprovalStep> PermissionDeleteApprovalSteps { get; set; }
        public MyDbContext(DbContextOptions options):base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
           var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Gms.Entity.dll");
           Assembly assembly = Assembly.LoadFrom(path);
           var types = assembly.GetTypes();
            foreach (var type in types)
            {
                modelBuilder.ApplyConfigurationsFromAssembly(type.Assembly);
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
