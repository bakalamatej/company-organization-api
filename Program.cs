using Scalar.AspNetCore;
using firmyAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Missing connection string 'DefaultConnection' in appsettings.json.");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Controllers
builder.Services.AddControllers();

// db context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    }));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

// Scalar API Reference Middleware  
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                  
    app.MapScalarApiReference();       
}

app.Run();