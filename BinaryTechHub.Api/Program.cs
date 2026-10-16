using Microsoft.EntityFrameworkCore;
using BinaryTechHub.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. MySQL DbContext via Pomelo
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// 3. CORS Policy so the frontend can query the API
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

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();