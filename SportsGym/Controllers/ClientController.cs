using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGym.Models.DTO;
using SportsGym.Models.Entities;
using SportsGym.Services;
using System.Security.Claims;

namespace SportsGym.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly PostgresConnection _db;
        private readonly IMapper _mapper;

        public ClientController(PostgresConnection db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // GET: /api/Client
        [HttpGet]
        public async Task<List<ClientDTO>> GetClients()
        {
            var clients = await _db.Clients.ToListAsync();
            return _mapper.Map<List<ClientDTO>>(clients);
        }

        // GET: /api/Client/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _db.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            return client;
        }

        [HttpPost]
        public async Task<ActionResult<Client>> PostClient([FromBody] ClientCreateDTO dto)
        {
            if (_db.Clients.Any(c => c.Login == dto.Login))
            {
                return BadRequest("Login already taken.");
            }

            var client = new Client
            {
                Name = dto.Name,
                BirthDate = dto.BirthDate,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Gender = dto.Gender,
                Login = dto.Login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Clients.Add(client);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, [FromBody] ClientUpdateDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID in URL does not match ID in body");
            }

            var client = await _db.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            client.Name = dto.Name;
            client.BirthDate = dto.BirthDate;
            client.Email = dto.Email;
            client.PhoneNumber = dto.PhoneNumber;
            client.Gender = dto.Gender;
            client.Login = dto.Login;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                client.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _db.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _db.Clients.FindAsync(id);
            if (client == null)
                return NotFound();

            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("profile/by-name/{name}")]
        public async Task<ActionResult<ClientDTO>> GetProfileByName(string name)
        {
            try
            {
                var client = await _db.Clients.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

                if (client == null)
                {
                    return NotFound($"Client '{name}' not found");
                }

                return Ok(_mapper.Map<ClientDTO>(client));
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("profile/by-name/{name}")]
        public async Task<IActionResult> UpdateOwnProfile([FromBody] ClientDTO dto, string name)
        {
            try
            {
                if (!string.Equals(name, dto.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("Route name and body name must match");
                }

                var client = await _db.Clients
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

                if (client == null)
                {
                    return NotFound($"Client '{name}' not found");
                }

                client.Email = dto.Email;
                client.PhoneNumber = dto.PhoneNumber;
                client.Gender = dto.Gender;
                client.BirthDate = dto.BirthDate;

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    client.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                }

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = $"Profile for '{name}' updated successfully",
                    updatedProfile = _mapper.Map<ClientDTO>(client)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating profile: {ex.Message}");
            }
        }
    }
}
