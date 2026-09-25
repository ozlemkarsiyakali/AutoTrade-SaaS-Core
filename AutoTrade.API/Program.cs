using AutoTrade.API.Filters;
using AutoTrade.API.Handlers;
using AutoTrade.API.Middlewares;
using AutoTrade.API.Providers;
using AutoTrade.Core.Configuration;
using AutoTrade.Core.Interfaces;
using AutoTrade.Core.Services;
using AutoTrade.Core.Validators;
using AutoTrade.Infrastructure.Persistence;
using AutoTrade.Infrastructure.Seeds;
using AutoTrade.Infrastructure.Services;
using AutoTrade.Service.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Controller & Validation Filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
//JWT Option Binding
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOptions>() 
    ?? throw new InvalidOperationException("TokenOptions section is missing in appsettings.json");

builder.Services.Configure<CustomTokenOptions>(builder.Configuration.GetSection("TokenOptions"));

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


//Authentication & JwtBearer Service Registration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience?.FirstOrDefault(),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)),
        ClockSkew = TimeSpan.Zero
    };
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