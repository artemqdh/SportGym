using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGym.Models.Entities;
using SportsGym.Services;

[Route("api/[controller]")]
[ApiController]
public class GymController : ControllerBase
{
    private readonly PostgresConnection _db;
    public GymController(PostgresConnection db) => _db = db;

    // Allow anyone
    [HttpGet]
    [AllowAnonymous]
    public async Task<List<Gym>> GetGyms()
    { 
        return await _db.Gyms.ToListAsync();
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Gym>> GetGym(int id)
    {
        var gym = await _db.Gyms.FindAsync(id);
        return gym is not null ? gym : NotFound();
    }

    // Only Manager or Admin
    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<Gym>> PostGym(Gym gym)
    {
        _db.Gyms.Add(gym);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGym), new { id = gym.Id }, gym);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> PutGym(int id, Gym updatedGym)
    {
        if (id != updatedGym.Id) return BadRequest();
        _db.Entry(updatedGym).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> DeleteGym(int id)
    {
        var gym = await _db.Gyms.FindAsync(id);
        if (gym == null) return NotFound();
        _db.Gyms.Remove(gym);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
