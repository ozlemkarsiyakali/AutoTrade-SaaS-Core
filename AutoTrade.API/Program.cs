using AutoTrade.API.Filters;
using AutoTrade.API.Middlewares;
using AutoTrade.Core.Interfaces;
using AutoTrade.Core.Validators;
using AutoTrade.Infrastructure.Persistence; 
using AutoTrade.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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

// 5. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program)));
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

app.Run();