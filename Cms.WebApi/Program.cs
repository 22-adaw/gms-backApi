using Autofac;
using Autofac.Extensions.DependencyInjection;
using Gms.Common;
using Gms.EntityFrameworkCore;
using Gms.WebApi.AutofaceDI;
using Gms.WebApi.Filters;
using Gms.WebApi.Jobs;
using Gms.WebApi.profiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using System.Reflection;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         // 禁用小驼峰命名策略，保持属性名原样
         options.JsonSerializerOptions.PropertyNamingPolicy = null;
     });
// 注入JWT服务认证（这里采用默认认证）
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    // 配置文件中存储的密钥
    var secretByte = Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]!);
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        //验证token的发布者
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        // 验证token的持有者
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Authentication:Audience"],
        // 验证token是否过期
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero, // ------------------------这里将ClockSkew属性的值设置为了0时，表示token有效时间到期后，立马生效。
        // 使用密钥
        IssuerSigningKey = new SymmetricSecurityKey(secretByte)

    };
    // --------------------------------------jwt 过期后触发该事件。
    options.Events = new JwtBearerEvents
    {
        // 授权失败
        OnAuthenticationFailed = context =>
        {
            // 错误的类型是token过期
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                /* context.Response.Headers.Add();*/

                context.Response.Headers.Add("Access-Control-Expose-Headers", "act");
                context.HttpContext.Response.Headers.Add("act", "expired");
                context.Response.Headers.AccessControlAllowOrigin = "*"; // 防止出现跨域的问题
            }
            return Task.CompletedTask;
        }
    };
});
//builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton<MailQueueManager>(); //---------------单例生命周期
builder.Services.AddMemoryCache();//添加内存缓存服务
//已废弃
//builder.Services.AddAutoMapper(typeof(LessonProfile));
//builder.Services.AddAutoMapper(typeof(PermissionProfile));
//builder.Services.AddAutoMapper(g =>
//{
//    g.AddMaps(new[] { Gms.WebApi });
//})
//注入所有AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<MyDbContext>(opt =>
{
    var connStr = builder.Configuration.GetConnectionString("StrConn");
    opt.UseSqlServer(connStr);
});
builder.Services.Configure<MvcOptions>(opt =>
{
    opt.Filters.Add<UnitOfWorkFilter>();
    //opt.Filters.Add<ApiPermissionCheckFilter>();//未来迭代实现，暂时废弃
});
//已废弃
//builder.Services.AddScoped<IUserInfoService, UserInfoService>();
//builder.Services.AddScoped<IUserInfoRepository, UserInfoRepository>();

//替换容器，初始化一个Autofac实例
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()).ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule(new AutofacModuleRegister());
});

//跨域
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                      });

});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ExpiryReminderJob>();//注入job
// 配置Quartz定时任务
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("ExpiryReminderJob", "ReminderGroup");

    q.AddJob<ExpiryReminderJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ExpiryReminderTrigger", "ReminderGroup")
        .WithCronSchedule("0 0 10 * * ?"));  // 每天上午10:00执行
});

// Quartz托管服务（应用启动时自动运行）
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});
var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);

app.Services.GetService<MailQueueManager>()!.Run();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{ 
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();// 处理静态文件的中间件，头像
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
