using Microsoft.EntityFrameworkCore;
using SportsGym.Services;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from .env/appsettings.json
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<PostgresConnection>();

// Add other services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware pipeline
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    .SetIsOriginAllowed(origin => true));

// Auto-create DB when app starts 
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<PostgresConnection>().Database.EnsureCreated();
}

// Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();