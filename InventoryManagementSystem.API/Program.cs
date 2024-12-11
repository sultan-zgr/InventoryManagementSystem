using FluentValidation.AspNetCore;
using InventoryManagementSystem.Application.Validators;
using InventoryManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant�s�
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<MongoDbContext>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new MongoDbContext(
        config["MongoDB:ConnectionString"],
        config["MongoDB:DatabaseName"]);
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddControllers()
    .AddFluentValidation(config =>
    {
        config.RegisterValidatorsFromAssemblyContaining<LoginUserDTOValidator>();
        config.RegisterValidatorsFromAssemblyContaining<CreateProductDTOValidator>();
    });

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Konsola log yaz
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // G�nl�k dosya logu
    .MinimumLevel.Debug() // Log seviyesi (Debug ve �st�)
    .CreateLogger();

// Serilog'u Logging i�in ayarla
builder.Host.UseSerilog();

// Authentication ve Authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// Controller ve Swagger
builder.Services.AddControllers(); // Controller servisi eklendi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Inventory Management System API",
        Version = "v1"
    });
});

var app = builder.Build();

// Swagger middleware
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory Management System API v1");
        c.RoutePrefix = string.Empty; // Swagger'� ana sayfa olarak ayarlar
    });
}

// Middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers(); // Controller'lar� route'a ba�la

app.Run();
