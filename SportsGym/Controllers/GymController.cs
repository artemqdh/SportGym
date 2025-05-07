using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGym.Models.Entities;
using SportsGym.Services;

namespace SportsGym.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class GymController : ControllerBase
    {
        private readonly PostgresConnection _db;

        public GymController(PostgresConnection db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<List<Gym>> GetGyms() ///< Get all gyms
        {
            return await _db.Gyms.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Gym>> GetGym(int id) ///< Get gym by id
        {
            var gym = await _db.Gyms.FindAsync(id);
            if (gym == null)
                return NotFound();
            return gym;
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<Gym>> PostGym(Gym gym) ///< Add new gym
        {
            _db.Gyms.Add(gym);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGym), new { id = gym.Id }, gym);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> PutGym(int id, Gym updatedGym) ///< Update an existing gym
        {
            if (id != updatedGym.Id)
                return BadRequest();

            _db.Entry(updatedGym).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> DeleteGym(int id) ///< Delete a gym
        {
            var gym = await _db.Gyms.FindAsync(id);
            if (gym == null)
                return NotFound();

            _db.Gyms.Remove(gym);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
