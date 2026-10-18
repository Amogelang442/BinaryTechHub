using Microsoft.EntityFrameworkCore;
using BinaryTechHub.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

// 2. Database context
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

// 3. CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// CRITICAL: CORS must run first before DeveloperExceptionPage, Routing, or Controllers
app.UseCors();

if (app.Environment.IsDevelopment() || true) // Enable detailed errors on cloud during setup
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseCors(); // Kept for route endpoint matching

app.UseAuthorization();

app.MapControllers();

//AUTO-CREATE MISSING TABLES IN AIVEN MYSQL
using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();