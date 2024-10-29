using Microsoft.EntityFrameworkCore;
using ProjectIpInformation.Entities;
using ProjectIpInformation.Repositories.Interfaces;
using ProjectIpInformation.Repositories;
using ProjectIpInformation.Services;
using ProjectIpInformation.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddHttpClient<IIp2CService, Ip2CService>();

builder.Services.AddScoped<IIpRepository, IpRepository>();           
builder.Services.AddScoped<IIpInfoService, IpInfoService>();        
builder.Services.AddScoped<ICacheService, CacheService>();           
builder.Services.AddSingleton<IHostedService, IpInfoUpdateService>(); 
builder.Services.AddSingleton<ICacheService, CacheService>();


builder.Services.AddHostedService<IpInfoUpdateService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
