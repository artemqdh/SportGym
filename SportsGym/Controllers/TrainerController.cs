using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public TrainerController(PostgresConnection db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<List<Trainer>> GetTrainers() ///< Get all trainers
        {
            return await _db.Trainers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Trainer>> GetTrainer(int id) ///< Get trainer by id
        {
            Trainer trainer = await _db.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            return trainer;
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<Trainer>> PostTrainer(Trainer trainer) ///< Add new trainer
        {
            _db.Trainers.Add(trainer);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTrainer), new { id = trainer.Id }, trainer);
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Admin")]
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
        public async Task<ActionResult<IEnumerable<Trainer>>> GetByGym(int gymId)
        {
            var trainers = await _db.Trainers
                .Where(t => t.GymId == gymId)
                .ToListAsync();

            return trainers;
        }

    }
}
