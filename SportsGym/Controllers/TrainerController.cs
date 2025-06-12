using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGym.Models.DTO;
using SportsGym.Models.Entities;
using SportsGym.Services;
using System.ComponentModel;

namespace SportsGym.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainerController : ControllerBase
    {
        private readonly PostgresConnection _db;
        private readonly IMapper _mapper;

        public TrainerController(PostgresConnection db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<List<TrainerDTO>> GetTrainers() ///< Get all trainers
        {
            var trainers = await _db.Trainers.ToListAsync();
            return _mapper.Map<List<TrainerDTO>>(trainers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Trainer>> GetTrainer(int id) ///< Get trainer by id
        {
            Trainer trainer = await _db.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            return trainer;
        }

        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<Trainer>> GetTrainerByName(string name) ///< Get trainer by name
        {
            Trainer trainer = await _db.Trainers.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

            if (trainer == null)
            {
                return NotFound();
            }

            return trainer;
        }

        [HttpPost]
        public async Task<ActionResult<Trainer>> PostTrainer([FromBody] TrainerCreateDTO trainerDto) ///< Add new trainer
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(trainerDto.Password);

            var trainer = new Trainer
            {
                Name = trainerDto.Name,
                BirthDate = trainerDto.BirthDate,
                PhoneNumber = trainerDto.PhoneNumber,
                Email = trainerDto.Email,
                Gender = trainerDto.Gender,
                Status = trainerDto.Status,
                Specialization = trainerDto.Specialization,
                WorkingHours = trainerDto.WorkingHours,
                GymName = trainerDto.GymName,
                Login = trainerDto.Login,
                PasswordHash = passwordHash
            };

            _db.Trainers.Add(trainer);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTrainer), new { id = trainer.Id }, trainer);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainer(int id) ///< Delete a trainer
        {
            var trainer = await _db.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            _db.Trainers.Remove(trainer);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("by-gym/{gymId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Trainer>>> GetByGym(string gymName)
        {
            var trainers = await _db.Trainers
                .Where(t => t.GymName == gymName)
                .ToListAsync();

            return trainers;
        }

        [HttpPut("by-name/{name}")]
        public async Task<IActionResult> PutTrainerByName(string name, TrainerDTO trainerDTO)
        {
            var trainer = await _db.Trainers
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

            if (trainer == null)
            {
                return NotFound();
            }

            trainer.Name = trainerDTO.Name;
            trainer.PhoneNumber = trainerDTO.PhoneNumber;
            trainer.BirthDate = trainerDTO.BirthDate;
            trainer.Email = trainerDTO.Email;
            trainer.Gender = trainerDTO.Gender;
            trainer.Status = trainerDTO.Status;
            trainer.Specialization = trainerDTO.Specialization;
            trainer.WorkingHours = trainerDTO.WorkingHours;
            trainer.GymName = trainerDTO.GymName;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrainerExists(trainer.Id))
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

        private bool TrainerExists(int id)
        {
            return _db.Trainers.Any(e => e.Id == id);
        }
    }
}
