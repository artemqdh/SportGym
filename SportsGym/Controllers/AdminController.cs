using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGym.Models.DTO;
using SportsGym.Models.Entities;
using SportsGym.Services;

namespace SportsGym.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly PostgresConnection _db;
        private readonly IMapper _mapper;

        public AdminController(PostgresConnection db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<List<AdminDTO>> GetAdmins()
        {
            var admins = await _db.Admins.ToListAsync();
            return _mapper.Map<List<AdminDTO>>(admins);
        }

        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<AdminDTO>> GetAdminByName(string name)
        {
            var admin = await _db.Admins
                .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());

            if (admin == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AdminDTO>(admin));
        }

        [HttpPut("by-name/{name}")]
        public async Task<IActionResult> PutAdminByName(string name, AdminDTO adminDTO)
        {
            var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());

            if (admin == null)
            {
                return NotFound();
            }

            admin.Name = adminDTO.Name;
            admin.PhoneNumber = adminDTO.PhoneNumber;
            admin.BirthDate = adminDTO.BirthDate;
            admin.Email = adminDTO.Email;
            admin.Gender = adminDTO.Gender;
            admin.Status = adminDTO.Status;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch
            {
                if (!AdminExists(admin.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        private bool AdminExists(int id)
        {
            return _db.Admins.Any(e => e.Id == id);
        }
    }
}
