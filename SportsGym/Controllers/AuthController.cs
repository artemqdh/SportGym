using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SportsGym.Models.Dto;
using SportsGym.Models.Entities;
using SportsGym.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace SportsGym.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly PostgresConnection _db;
        private readonly IConfiguration _config;

        public AuthController(PostgresConnection db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            // 1) Check if login already exists across all roles
            if (_db.Clients.Any(c => c.Login == dto.Login) ||
                _db.Trainers.Any(t => t.Login == dto.Login) ||
                _db.Admins.Any(a => a.Login == dto.Login))
            {
                return BadRequest("Login already taken.");
            }

            // 2) Create a Client by default (you can adjust role logic later)
            var client = new Client
            {
                Name = dto.Name,
                Login = dto.Login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Clients.Add(client);
            await _db.SaveChangesAsync();

            return Ok("Registration successful");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.Login == dto.Login);
            var trainer = await _db.Trainers.FirstOrDefaultAsync(t => t.Login == dto.Login);
            var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Login == dto.Login);

            if (client == null && trainer == null && admin == null)
            {
                return Unauthorized("Invalid login or password.");
            }

            string storedHash, role, userName;
            int userId;

            if (client != null)
            {
                storedHash = client.PasswordHash;
                role = "Client";
                userId = client.Id;
                userName = client.Name;
            }
            else if (trainer != null)
            {
                storedHash = trainer.PasswordHash;
                role = "Trainer";
                userId = trainer.Id;
                userName = trainer.Name;
            }
            else
            {
                storedHash = admin.PasswordHash;
                role = "Admin";
                userId = admin.Id;
                userName = admin.Name;
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, storedHash))
            {
                return Unauthorized("Invalid login or password.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JWT:SigningKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                data = new
                {
                    token = tokenString,
                    userName,
                    role
                },
                message = "Login successful."
            });
        }

    }
}
