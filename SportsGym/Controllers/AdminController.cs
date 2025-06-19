using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsGym.Models.DTO;
using SportsGym.Services.Interfaces;

namespace SportsGym.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AdminDTO>>> GetAdmins()
        {
            var admins = await _unitOfWork.AdminRepository.GetAllAsync();

            if (admins == null || !admins.Any())
                return NoContent();

            return Ok(_mapper.Map<List<AdminDTO>>(admins));
        }

        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<AdminDTO>> GetAdminByName(string name)
        {
            var admin = await _unitOfWork.AdminRepository
                .FindAsync(a => a.Name.ToLower() == name.ToLower());

            if (admin == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AdminDTO>(admin));
        }

        [HttpPut("by-name/{name}")]
        public async Task<IActionResult> PutAdminByName(string name, AdminDTO adminDTO)
        {
            var admin = await _unitOfWork.AdminRepository
                .FindAsync(a => a.Name.ToLower() == name.ToLower());

            if (admin == null)
            {
                return NotFound();
            }

            // Обновляем поля
            admin.Name = adminDTO.Name;
            admin.PhoneNumber = adminDTO.PhoneNumber;
            admin.BirthDate = adminDTO.BirthDate;
            admin.Email = adminDTO.Email;
            admin.Gender = adminDTO.Gender;
            admin.Status = adminDTO.Status;

            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                if (!await AdminExists(admin.Id))
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

        private async Task<bool> AdminExists(int id)
        {
            return await _unitOfWork.AdminRepository.ExistsAsync(e => e.Id == id);
        }
    }
}
