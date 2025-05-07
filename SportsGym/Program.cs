using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SportsGym.Services;
using Microsoft.AspNetCore.Identity;
using System.Text;
using SportsGym.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// 1) Load configuration
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// 2) Register CORS policy BEFORE calling Build()
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowReact", policy =>
        policy.WithOrigins("http://localhost:3001")   // ← React’s port
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
    );
});

// 3) Register EF Core, Identity, and JWT
builder.Services.AddDbContext<PostgresConnection>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsAssembly(typeof(Program).Assembly.FullName)
    )
);

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<PostgresConnection>()
    .AddDefaultTokenProviders();

var jwtKey = builder.Configuration["JWT:SigningKey"] ?? throw new InvalidOperationException("Missing JWT Signing Key");
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "SportsGym";

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// 4) Add Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5) Apply pending migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PostgresConnection>();
    db.Database.Migrate();

    var newAdmin = new Admin
    {
        Name = "Artem Admin",
        BirthDate = "2005-07-17",
        PhoneNumber = 0,
        Email = "",
        Gender = "Male",
        Status = "SysAdmin",
        Login = "artemadmin",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123") // hashed!
    };

    db.Admins.Add(newAdmin);
    db.SaveChanges();
}

// 6) Apply CORS and Auth middleware
app.UseCors("AllowReact");
app.UseAuthentication();
app.UseAuthorization();

// 7) Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
