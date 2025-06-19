using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SportsGym.Models.Dto;
using SportsGym.Models.Entities;
using SportsGym.Services.Interfaces;

namespace SportsGym.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public AuthController(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool loginExists = await _unitOfWork.ClientRepository.ExistsAsync(c => c.Login == dto.Login) ||
                               await _unitOfWork.TrainerRepository.ExistsAsync(t => t.Login == dto.Login) ||
                               await _unitOfWork.AdminRepository.ExistsAsync(a => a.Login == dto.Login);

            if (loginExists)
            {
                return BadRequest("Login already taken.");
            }

            var client = new Client
            {
                Name = dto.Name,
                Login = dto.Login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _unitOfWork.ClientRepository.Add(client);
            await _unitOfWork.CommitAsync();

            return Ok("Registration successful");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = await _unitOfWork.ClientRepository.FindAsync(c => c.Login == dto.Login);
            var trainer = await _unitOfWork.TrainerRepository.FindAsync(t => t.Login == dto.Login);
            var admin = await _unitOfWork.AdminRepository.FindAsync(a => a.Login == dto.Login);

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
