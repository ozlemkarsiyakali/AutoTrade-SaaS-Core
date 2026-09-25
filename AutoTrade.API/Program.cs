using AutoTrade.API.Filters;
using AutoTrade.API.Handlers;
using AutoTrade.API.Middlewares;
using AutoTrade.API.Providers;
using AutoTrade.Core.Interfaces;
using AutoTrade.Core.Services;
using AutoTrade.Core.Validators;
using AutoTrade.Infrastructure.Persistence;
using AutoTrade.Infrastructure.Seeds;
using AutoTrade.Infrastructure.Services;
using AutoTrade.Service.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;

var builder = WebApplication.CreateBuilder(args);

// 1. Controller & Validation Filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// 2. FluentValidation Kaydı
builder.Services.AddValidatorsFromAssemblyContaining<CreateVehicleBrandDtoValidator>();

// 3. AppDbContext & PostgreSQL Kaydı (EKSİK OLAN KISIM)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 4. Repository, Service & UnitOfWork Kayıtları
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRoleService, RoleService>();

// Custom Permission Policy DI Kayıtları
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

// 5. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program)));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.InstanceName = "AutoTrade_";
});
// ==========================================
var app = builder.Build();
// ==========================================

app.UseCustomException();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


//SuperAdmin Role Setted
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await AppDbContextSeed.SeedAsync(context);
}
app.Run();