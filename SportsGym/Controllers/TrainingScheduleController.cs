using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGym.Models.DTO;
using SportsGym.Models.Entities;
using SportsGym.Services;

namespace SportsGym.Controllers
{
    //[Authorize(Roles = "Admin,Trainer")]
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingScheduleController : ControllerBase
    {
        private readonly PostgresConnection _db;

        public TrainingScheduleController(PostgresConnection db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<List<TrainingSchedule>> GetSchedules() ///< Get all training schedules
        {
            return await _db.Trainings.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TrainingSchedule>> GetSchedule(int id) ///< Get schedule by id
        {
            var schedule = await _db.Trainings.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            return schedule;
        }

        [HttpGet("trainer/{trainerName}")]
        public async Task<ActionResult<List<TrainingScheduleDTO>>> GetSchedulesByTrainer(string trainerName)
        {
            var schedules = await _db.Trainings
                .Where(schedule => schedule.TrainerName == trainerName)
                .Select(schedule => new TrainingScheduleDTO
                {
                    Id = schedule.Id,
                    Type = schedule.Type,
                    TrainerName = schedule.TrainerName,
                    ClientNames = schedule.ClientNames,
                    Date = schedule.Date,
                    Time = schedule.Time,
                    GymName = schedule.GymName
                }).ToListAsync();

            return Ok(schedules);
        }

        [HttpPost]
        //[Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<TrainingScheduleDTO>> PostSchedule(TrainingScheduleDTO scheduleDto) ///< Add new schedule
        {
            var schedule = new TrainingSchedule
            {
                Type = scheduleDto.Type,
                TrainerName = scheduleDto.TrainerName,
                ClientNames = scheduleDto.ClientNames,
                Date = scheduleDto.Date,
                Time = scheduleDto.Time,
                GymName = scheduleDto.GymName
            };

            _db.Trainings.Add(schedule);
            await _db.SaveChangesAsync();

            var createdScheduleDto = new TrainingScheduleDTO
            {
                Id = schedule.Id,
                Type = schedule.Type,
                TrainerName = schedule.TrainerName,
                ClientNames = schedule.ClientNames,
                Date = schedule.Date,
                Time = schedule.Time,
                GymName = schedule.GymName
            };

            return CreatedAtAction(nameof(GetSchedule), new { id = createdScheduleDto.Id }, createdScheduleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSchedule(int id, TrainingSchedule updatedSchedule)
        {
            if (id != updatedSchedule.Id)
            {
                return BadRequest();
            }

            var existingSchedule = await _db.Trainings.FindAsync(id);
            if (existingSchedule == null)
            {
                return NotFound();
            }

            existingSchedule.ClientNames = updatedSchedule.ClientNames;
            existingSchedule.GymName = updatedSchedule.GymName;
            existingSchedule.TrainerName = updatedSchedule.TrainerName;
            existingSchedule.Date = updatedSchedule.Date;
            existingSchedule.Time = updatedSchedule.Time;

            _db.Entry(existingSchedule).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> DeleteSchedule(int id) ///< Delete a schedule
        {
            var schedule = await _db.Trainings.FindAsync(id);
            if (schedule == null)
                return NotFound();

            _db.Trainings.Remove(schedule);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
