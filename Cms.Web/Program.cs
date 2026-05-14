using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cms.EntityFrameworkCore;
using Cms.IRepository;
using Cms.IService;
using Cms.Repository;
using Cms.Service;
using Cms.Web.AutofaceDI;
using Cms.Web.Filters;
using Cms.Web.Profiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MyDbContext>(opt =>
{
   var connStr = builder.Configuration.GetConnectionString("StrConn");
    opt.UseSqlServer(connStr);
});
builder.Services.Configure<MvcOptions>(opt =>
{
    opt.Filters.Add<UnitOfWorkFilter>();
});
//builder.Services.AddScoped<IUserInfoService, UserInfoService>();
//builder.Services.AddScoped<IUserInfoRepository, UserInfoRepository>();

// 替换容器，初始化一个Autofac的示例。
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacModuleRegister());
});


builder.Services.AddAutoMapper(typeof(UserInfoProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
