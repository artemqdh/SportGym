using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SportsGym.Services;
using SportsGym.Models.Entities;
using SportsGym.Services.Interfaces;    // <-- for ADatabaseConnection
using System.Text;
using DotNetEnv;  // only if you still need to load .env

var builder = WebApplication.CreateBuilder(args);

// 1) Load .env (if you need it) and configuration
Env.Load();  // optional: only if you rely on DotNetEnv
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// 2) Register permissive CORS (dev only)
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader()
    );
});

// 3) Register EF Core DbContext
builder.Services.AddDbContext<PostgresConnection>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsAssembly(typeof(Program).Assembly.FullName)
    )
);

// 3b) Make ADatabaseConnection injectable
builder.Services.AddScoped<ADatabaseConnection, PostgresConnection>();

var jwtKey = Environment.GetEnvironmentVariable("JWT__SigningKey")
    ?? throw new InvalidOperationException("Missing JWT:SigningKey");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT__Issuer") ?? "SportsGym";

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

// 5) MVC controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6) Apply EF migrations & seed initial data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PostgresConnection>();
    db.Database.Migrate();

    if (!db.Gyms.Any())
    {
        db.Gyms.AddRange(
            new Gym
            {
                Name = "Downtown Fitness Center",
                Location = "Downtown",
                AvailableTime = "Monday|08:00|20:00;Tuesday|08:00|20:00"
            },
            new Gym
            {
                Name = "Eastside Gym",
                Location = "Eastside",
                AvailableTime = "Wednesday|09:00|18:00;Thursday|09:00|18:00"
            },
            new Gym
            {
                Name = "Uptown Health Club",
                Location = "Uptown",
                AvailableTime = "Monday|10:00|18:00;Wednesday|09:00|17:00"
            },
            new Gym
            {
                Name = "Eastside Yoga Studio",
                Location = "Eastside",
                AvailableTime = "Friday|09:00|17:00;Saturday|08:00|14:00"
            }
        );
        db.SaveChanges();
    }

    if (!db.Trainers.Any())
    {
        var downtown = db.Gyms.First(g => g.Name == "Downtown Fitness Center");
        var eastside = db.Gyms.First(g => g.Name == "Eastside Gym");
        var uptown = db.Gyms.First(g => g.Name == "Uptown Health Club");
        var yoga = db.Gyms.First(g => g.Name == "Eastside Yoga Studio");

        string Hash(string pw) => BCrypt.Net.BCrypt.HashPassword(pw);

        db.Trainers.AddRange(
            new Trainer
            {
                Name = "Alex Johnson",
                BirthDate = "1990-03-15",
                PhoneNumber = 1234567890,
                Email = "alex.johnson@example.com",
                Gender = "Male",
                Status = "Active",
                Specialization = "Weightlifting",
                WorkingHours = "Monday|09:00|18:00;Tuesday|09:00|18:00",
                Login = "alexj",
                PasswordHash = Hash("trainer123"),
                GymId = downtown.Id
            },
            new Trainer
            {
                Name = "Mia Patel",
                BirthDate = "1988-07-21",
                PhoneNumber = 234567890,
                Email = "mia.patel@example.com",
                Gender = "Female",
                Status = "Active",
                Specialization = "Cardio",
                WorkingHours = "Wednesday|08:00|16:00;Thursday|10:00|18:00",
                Login = "miap",
                PasswordHash = Hash("trainer123"),
                GymId = eastside.Id
            },
            new Trainer
            {
                Name = "Sofia Martinez",
                BirthDate = "1992-11-10",
                PhoneNumber = 345678901,
                Email = "sofia.martinez@example.com",
                Gender = "Female",
                Status = "Active",
                Specialization = "Yoga",
                WorkingHours = "Friday|09:00|17:00;Saturday|08:00|14:00",
                Login = "sofiam",
                PasswordHash = Hash("trainer123"),
                GymId = yoga.Id
            },
            new Trainer
            {
                Name = "Liam Chen",
                BirthDate = "1985-05-30",
                PhoneNumber = 456789012,
                Email = "liam.chen@example.com",
                Gender = "Male",
                Status = "Active",
                Specialization = "CrossFit",
                WorkingHours = "Monday|10:00|18:00;Wednesday|09:00|17:00",
                Login = "liamc",
                PasswordHash = Hash("trainer123"),
                GymId = uptown.Id
            }
        );
        db.SaveChanges();
    }
}

// 7) Enable CORS + Authentication + Authorization middleware
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// 8) Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
