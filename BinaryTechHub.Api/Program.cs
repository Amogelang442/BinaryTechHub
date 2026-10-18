using Microsoft.EntityFrameworkCore;
using BinaryTechHub.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Connection String (Falls back gracefully if null)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

// 2. MySQL DbContext via Pomelo
// We specify a fixed ServerVersion (e.g., MySQL 8.0) instead of AutoDetect to prevent cold-boot crashes on cloud hosts
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString, 
        new MySqlServerVersion(new Version(8, 0, 36)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

// 3. CORS Policy so both localhost and live Vercel domains can query the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// 4. Middleware Pipeline Execution Order
app.UseRouting();

// CORS must be executed right after routing and before Auth/Endpoints
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();