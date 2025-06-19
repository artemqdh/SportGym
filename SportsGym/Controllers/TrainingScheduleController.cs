using Microsoft.AspNetCore.Mvc;
using SportsGym.Models.DTO;
using SportsGym.Models.Entities;
using SportsGym.Services.Interfaces;
using AutoMapper;

namespace SportsGym.Controllers
{
    //[Authorize(Roles = "Admin,Trainer")]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingScheduleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainingScheduleController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TrainingScheduleDTO>>> GetSchedules()
        {
            var schedules = await _unitOfWork.TrainingsRepository.GetAllAsync();
            var scheduleDtos = _mapper.Map<List<TrainingScheduleDTO>>(schedules);
            return Ok(scheduleDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TrainingScheduleDTO>> GetSchedule(int id)
        {
            var schedule = await _unitOfWork.TrainingsRepository.FindByIdAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            var scheduleDto = _mapper.Map<TrainingScheduleDTO>(schedule);
            return Ok(scheduleDto);
        }

        [HttpGet("trainer/{trainerName}")]
        public async Task<ActionResult<List<TrainingScheduleDTO>>> GetSchedulesByTrainer(string trainerName)
        {
            var schedules = await _unitOfWork.TrainingsRepository.FindAllAsync(s => s.TrainerName == trainerName);
            var scheduleDtos = _mapper.Map<List<TrainingScheduleDTO>>(schedules);
            return Ok(scheduleDtos);
        }

        [HttpPost]
        public async Task<ActionResult<TrainingScheduleDTO>> PostSchedule(TrainingScheduleDTO scheduleDto)
        {
            var timeParts = scheduleDto.Time.Split('-');
            if (timeParts.Length != 2)
            {
                return BadRequest("Invalid time format. Expected format: 'HH:mm-HH:mm'.");
            }

            if (!TimeSpan.TryParse(timeParts[0], out var newStart))
            {
                return BadRequest("Invalid start time format.");
            }
            if (!TimeSpan.TryParse(timeParts[1], out var newEnd))
            {
                return BadRequest("Invalid end time format.");
            }

            if (newStart >= newEnd)
            {
                return BadRequest("Start time must be earlier than end time.");
            }

            var existingTrainings = await _unitOfWork.TrainingsRepository.FindAllAsync(
                t => t.TrainerName == scheduleDto.TrainerName && t.Date == scheduleDto.Date);

            foreach (var training in existingTrainings)
            {
                if (training.GymName != scheduleDto.GymName)
                {
                    continue;
                }

                var existingTimeParts = training.Time.Split('-');
                if (existingTimeParts.Length != 2)
                {
                    continue;
                }

                if (!TimeSpan.TryParse(existingTimeParts[0], out var existStart))
                {
                    continue;
                }
                if (!TimeSpan.TryParse(existingTimeParts[1], out var existEnd))
                {
                    continue;
                }

                if (newStart < existEnd && existStart < newEnd)
                {
                    return Conflict("Training overlaps with an existing session.");
                }
            }

            var schedule = _mapper.Map<TrainingSchedule>(scheduleDto);

            await _unitOfWork.TrainingsRepository.AddAsync(schedule);
            await _unitOfWork.CommitAsync();

            var createdScheduleDto = _mapper.Map<TrainingScheduleDTO>(schedule);

            return CreatedAtAction(nameof(GetSchedule), new { id = createdScheduleDto.Id }, createdScheduleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSchedule(int id, TrainingScheduleDTO updatedScheduleDto)
        {
            if (id != updatedScheduleDto.Id)
            {
                return BadRequest();
            }

            var existingSchedule = await _unitOfWork.TrainingsRepository.FindByIdAsync(id);
            if (existingSchedule == null)
            {
                return NotFound();
            }
            
            _mapper.Map(updatedScheduleDto, existingSchedule);

            _unitOfWork.TrainingsRepository.Update(existingSchedule);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _unitOfWork.TrainingsRepository.FindByIdAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            _unitOfWork.TrainingsRepository.Remove(schedule);
            await _unitOfWork.CommitAsync();

            return NoContent();
        }
    }
}
