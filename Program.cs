using Scalar.AspNetCore;
using firmyAPI.Data;
using Microsoft.EntityFrameworkCore;
using firmyAPI.Validation;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Missing connection string 'DefaultConnection' in appsettings.json.");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// db context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    }));

// Validators
builder.Services.AddScoped<IEntityValidator, EntityValidator>();
// Controllers
builder.Services.AddControllers();

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