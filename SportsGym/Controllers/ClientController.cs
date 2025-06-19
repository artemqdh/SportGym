using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsGym.Models.DTO;
using SportsGym.Models.Entities;
using SportsGym.Services.Interfaces;

namespace SportsGym.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClientController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: /api/Client
        [HttpGet]
        public async Task<ActionResult<List<ClientDTO>>> GetClients()
        {
            var clients = await _unitOfWork.ClientRepository.GetAllAsync();
            var dtos = _mapper.Map<List<ClientDTO>>(clients);
            return Ok(dtos);
        }

        // GET: /api/Client/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDTO>> GetClient(int id)
        {
            var client = await _unitOfWork.ClientRepository.FindByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ClientDTO>(client));
        }

        // POST: /api/Client
        [HttpPost]
        public async Task<ActionResult<ClientDTO>> PostClient([FromBody] ClientCreateDTO dto)
        {
            var existingClient = await _unitOfWork.ClientRepository.FindByLoginAsync(dto.Login);
            if (existingClient != null)
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

            await _unitOfWork.ClientRepository.AddAsync(client);
            await _unitOfWork.CommitAsync();

            var clientDto = _mapper.Map<ClientDTO>(client);
            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, clientDto);
        }

        // PUT: /api/Client/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, [FromBody] ClientUpdateDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID in URL does not match ID in body");
            }

            var client = await _unitOfWork.ClientRepository.FindByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            client.Name = dto.Name;
            client.BirthDate = dto.BirthDate;
            client.Email = dto.Email;
            client.PhoneNumber = dto.PhoneNumber;
            client.Gender = dto.Gender;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                client.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            _unitOfWork.ClientRepository.Update(client);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }

        // DELETE: /api/Client/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _unitOfWork.ClientRepository.FindByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _unitOfWork.ClientRepository.Remove(client);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }

        // GET: /api/Client/profile/by-name/{name}
        [HttpGet("profile/by-name/{name}")]
        public async Task<ActionResult<ClientDTO>> GetProfileByName(string name)
        {
            try
            {
                var client = await _unitOfWork.ClientRepository.FindByNameAsync(name);
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

        // PUT: /api/Client/profile/by-name/{name}
        [HttpPut("profile/by-name/{name}")]
        public async Task<IActionResult> UpdateOwnProfile([FromBody] ClientDTO dto, string name)
        {
            try
            {
                if (!string.Equals(name, dto.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("Route name and body name must match");
                }

                var client = await _unitOfWork.ClientRepository.FindByNameAsync(name);
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

                _unitOfWork.ClientRepository.Update(client);
                await _unitOfWork.CommitAsync();

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
