using AutoMapper;
using Lims.WebAPI.Context.Repository;
using Lims.WebAPI.Context.UnitOfWork;
using Lims.WebAPI.Hubs;
using Lims.WebAPI.Profiles;
using Lims.WebAPI.Service;
using Lims.WebAPI.Service.Interface;
using Lims.WebAPI.Singleton;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration.Json;
using SqlSugar;
using SqlSugar.IOC;

var builder = WebApplication.CreateBuilder(args);



var conStr = AppConfigurtaionServices.Configuration.GetSection("ConnectionStrings:POSTGRESQL").Value;

//if (builder.Environment.IsDevelopment())
//var    conStr = AppConfigurtaionServices.Configuration.GetSection("ConnectionStrings:DevelopConection").Value;

//注册上下文：AOP里面可以获取IOC对象，如果有现成框架比如Furion可以不写这一行
//builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ISqlSugarClient>(s =>
{
    return new SqlSugarClient(new ConnectionConfig
    {
        ConnectionString = conStr,
        DbType = SqlSugar.DbType.PostgreSQL,
        IsAutoCloseConnection = true,
        MoreSettings = new ConnMoreSettings
        {
            PgSqlIsAutoToLower = false, //不自动转换为小写
            PgSqlIsAutoToLowerCodeFirst = false //代码优先不自动转换为小写
        }
    });
});   
  
//添加AutoMapper
var automapperConfog = new MapperConfiguration(config =>
{
    config.AddProfile(new AutoMapperProFile());
    //config.CreateMap<DateTimeOffset, DateTimeOffset>().ConvertUsing<UtcToLocalConverter>();
});
builder.Services.AddSingleton(automapperConfog.CreateMapper());

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();

//1.添加SignalR服务
builder.Services.AddSignalR();

builder.Services.AddScoped<TaskCount>();

//builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISampleRepository, SampleRepository>();
builder.Services.AddTransient<ISampleService, SampleService>();

builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddTransient<IItemService, ItemService>();

builder.Services.AddScoped<ISubItemRepository, SubItemRepository>();
builder.Services.AddTransient<ISubItemService, SubItemService>();

builder.Services.AddScoped<ISubItemStandardRepository, SubItemStandardRepository>();
builder.Services.AddTransient<ISubItemStandardService, SubItemStandardService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddScoped<ILoggerRepository, LoggerRepository>();
builder.Services.AddTransient<ILoggerService, LoggerService>();

builder.Services.AddScoped<IProductStandardRepository, ProductStandardRepository>();
builder.Services.AddTransient<IProductStandardService, ProductStandardService>();

builder.Services.AddScoped<IMethodStandardRepository, MethodStandardRepository>();
builder.Services.AddTransient<IMethodStandardService, MethodStandardService>();

builder.Services.AddScoped<IReagentRepository, ReagentRepository>();
builder.Services.AddTransient<IReagentService, ReagentService>();




builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});



var app = builder.Build();

app.UseRouting();

//启用https重定向
//app.UseHttpsRedirection();



//Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//在Use中注册单例实例
/*
app.Use(async (context, next) =>
{
    var hubContext = context.RequestServices
                            .GetRequiredService<IHubContext<ChatHub>>();
    TaskCount.Register(hubContext);//调用静态方法注册

    if (next != null)
    {
        await next.Invoke();
    }
});
*/
app.MapControllers();

//2.映射路由
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/TaskCount");
});


app.Run();
public class AppConfigurtaionServices
{
    public static IConfiguration Configuration { get; set; }
    static AppConfigurtaionServices()
    {
        //ReloadOnChange = true 当appsettings.json被修改时重新加载            
        Configuration = new ConfigurationBuilder()
        //.SetBasePath(Directory.GetCurrentDirectory())
        //AppDomain.CurrentDomain.BaseDirectory是程序集基目录，所以appsettings.json,需要复制一份放在程序集目录下，
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
        .Build();
    }


}
/*
public class UtcToLocalConverter : AutoMapper.ITypeConverter<DateTime, DateTime>
{
    public DateTime Convert(DateTime source, DateTime destination, ResolutionContext context)
    {
        var inputDate = source;
        if (inputDate.Kind == DateTimeKind.Utc)
        {
            return inputDate.ToLocalTime();
        }
        return inputDate;
    }

}
*/