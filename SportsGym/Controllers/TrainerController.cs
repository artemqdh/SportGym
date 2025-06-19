using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGym.Models.DTO;
using SportsGym.Models.Entities;
using SportsGym.Services.Interfaces;

namespace SportsGym.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<List<TrainerDTO>> GetTrainers()
        {
            var trainers = await _unitOfWork.TrainerRepository.GetAllAsync();
            return _mapper.Map<List<TrainerDTO>>(trainers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Trainer>> GetTrainer(int id)
        {
            var trainer = await _unitOfWork.TrainerRepository.FindByIdAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            return trainer;
        }

        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<Trainer>> GetTrainerByName(string name)
        {
            var trainers = await _unitOfWork.TrainerRepository.FindAllAsync(t => t.Name.ToLower() == name.ToLower());
            var trainer = trainers.FirstOrDefault();

            if (trainer == null)
            {
                return NotFound();
            }

            return trainer;
        }

        [HttpPost]
        public async Task<ActionResult<Trainer>> PostTrainer([FromBody] TrainerCreateDTO trainerDto)
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

            await _unitOfWork.TrainerRepository.AddAsync(trainer);
            await _unitOfWork.CommitAsync();

            return CreatedAtAction(nameof(GetTrainer), new { id = trainer.Id }, trainer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainer(int id)
        {
            var trainer = await _unitOfWork.TrainerRepository.FindByIdAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            _unitOfWork.TrainerRepository.Remove(trainer);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }

        [HttpGet("by-gym/{gymName}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Trainer>>> GetByGym(string gymName)
        {
            var trainers = await _unitOfWork.TrainerRepository.FindAllAsync(t => t.GymName == gymName);
            return Ok(trainers.ToList());
        }

        [HttpPut("by-name/{name}")]
        public async Task<IActionResult> PutTrainerByName(string name, TrainerDTO trainerDTO)
        {
            var trainers = await _unitOfWork.TrainerRepository.FindAllAsync(t => t.Name.ToLower() == name.ToLower());
            var trainer = trainers.FirstOrDefault();

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
            trainer.Login = trainerDTO.Login;

            trainer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(trainerDTO.Password);

            _unitOfWork.TrainerRepository.Update(trainer);

            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = (await _unitOfWork.TrainerRepository.FindByIdAsync(trainer.Id)) != null;
                if (!exists)
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

        [HttpPut("by-name/trainer/{name}")]
        public async Task<IActionResult> PutTrainerByNameTrainer(string name, TrainerDTO trainerDTO)
        {
            var trainers = await _unitOfWork.TrainerRepository.FindAllAsync(t => t.Name.ToLower() == name.ToLower());
            var trainer = trainers.FirstOrDefault();

            if (trainer == null)
            {
                return NotFound();
            }

            trainer.Name = trainerDTO.Name;
            trainer.PhoneNumber = trainerDTO.PhoneNumber;
            trainer.BirthDate = trainerDTO.BirthDate;
            trainer.Email = trainerDTO.Email;
            trainer.Gender = trainerDTO.Gender;
            trainer.Login = trainerDTO.Login;

            trainer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(trainerDTO.Password);

            _unitOfWork.TrainerRepository.Update(trainer);

            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = (await _unitOfWork.TrainerRepository.FindByIdAsync(trainer.Id)) != null;
                if (!exists)
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
    }
}
