using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGym.Models.Entities;
using SportsGym.Services;

namespace SportsGym.Controllers
{
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
                return NotFound();
            return schedule;
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<TrainingSchedule>> PostSchedule(TrainingSchedule schedule) ///< Add new schedule
        {
            _db.Trainings.Add(schedule);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSchedule), new { id = schedule.Id }, schedule);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> PutSchedule(int id, TrainingSchedule updatedSchedule) ///< Update an existing schedule
        {
            if (id != updatedSchedule.Id)
                return BadRequest();

            _db.Entry(updatedSchedule).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Admin")]
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
