using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SportsGym.Services;
using SportsGym.Services.Interfaces;
using System.Text;
using DotNetEnv;
using SportsGym.Models;
using SportsGym.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]);

Env.Load();
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader()
    );
});

builder.Services.AddDbContext<PostgresConnection>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsAssembly(typeof(Program).Assembly.FullName)
    )
);

builder.Services.AddScoped<ADatabaseConnection, PostgresConnection>();

//var jwtKey = builder.Configuration["JWT:SigningKey"] ?? throw new InvalidOperationException("Missing JWT:SigningKey");
//var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "SportsGym";
//var jwtAudience = builder.Configuration["JWT:Audience"] ?? "SportsGym";

//Console.WriteLine($"SIGNING KEY: {jwtKey}");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AppMappingProfile));

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["JWT:Issuer"],
//        ValidAudience = builder.Configuration["JWT:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]))
//    };
//});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PostgresConnection>();
    db.Database.Migrate();

    if (!db.Gyms.Any())
    {
        db.Gyms.AddRange(
            new Gym
            {
                Name = "GymElite",
                Location = "Moscow",
                AvailableTime = "Monday-Sunday|10:00|23:00"
            },
            new Gym
            {
                Name = "McFit",
                Location = "London",
                AvailableTime = "Monday-Sunday|10:00|23:00"
            },
            new Gym
            {
                Name = "GymNation",
                Location = "Dubai",
                AvailableTime = "Monday-Sunday|10:00|23:00"
            },
            new Gym
            {
                Name = "FitnessLA",
                Location = "Los Angeles",
                AvailableTime = "Monday-Sunday|10:00|23:00"
            }
        );
        db.SaveChanges();
    }

    if (!db.Trainers.Any())
    {
        var gymElite = db.Gyms.First(g => g.Name == "GymElite");
        var mcFit = db.Gyms.First(g => g.Name == "McFit");
        var gymNation = db.Gyms.First(g => g.Name == "GymNation");
        var fitnessLA = db.Gyms.First(g => g.Name == "FitnessLA");

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
                WorkingHours = "Monday|10:00|18:00;Tuesday|10:00|18:00;Friday|10:00|18:00;",
                Login = "alexj",
                PasswordHash = Hash("trainer123"),
                GymName = gymElite.Name
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
                WorkingHours = "Wednesday|10:00|16:00;Thursday|10:00|18:00;Saturday|10:00|19:00",
                Login = "miap",
                PasswordHash = Hash("trainer123"),
                GymName = mcFit.Name
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
                WorkingHours = "Friday|10:00|17:00;Saturday|10:00|14:00;Sunday|10:00|19:00",
                Login = "sofiam",
                PasswordHash = Hash("trainer123"),
                GymName = gymNation.Name
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
                WorkingHours = "Monday|10:00|18:00;Wednesday|10:00|17:00;Friday|10:00|20:00",
                Login = "liamc",
                PasswordHash = Hash("trainer123"),
                GymName = fitnessLA.Name
            }
        );
        db.SaveChanges();
    }
}

app.UseRouting();

app.UseCors("AllowAll");

//app.UseAuthentication();
//app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();