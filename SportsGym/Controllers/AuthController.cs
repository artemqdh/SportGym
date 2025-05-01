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

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            var client = _db.Clients.FirstOrDefault(c => c.Login == dto.Login);
            var trainer = _db.Trainers.FirstOrDefault(t => t.Login == dto.Login);
            var admin = _db.Admins.FirstOrDefault(a => a.Login == dto.Login);

            
            if (client == null && trainer == null && admin == null) ///< No user found?
            {
                return Unauthorized("Invalid login or password.");
            }

            string storedHash;
            string role;
            int userId;
            string userName;

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
            else // admin != null
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

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role)
            };

            ///< Sign and issue the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
