using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGym.Models.DTO;
using SportsGym.Models.Entities;
using SportsGym.Services;

[Route("api/[controller]")]
[ApiController]
public class GymController : ControllerBase
{
    private readonly PostgresConnection _db;
    private readonly IMapper _mapper;

    public GymController(PostgresConnection db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // Allow anyone
    [HttpGet]
    [AllowAnonymous]
    public async Task<List<GymDTO>> GetGyms()
    {
        var gyms = await _db.Gyms.ToListAsync();
        return _mapper.Map<List<GymDTO>>(gyms);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Gym>> GetGym(int id)
    {
        var gym = await _db.Gyms.FindAsync(id);
        return gym is not null ? gym : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Gym>> PostGym(Gym gym)
    {
        _db.Gyms.Add(gym);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGym), new { id = gym.Id }, gym);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutGym(int id, Gym updatedGym)
    {
        if (id != updatedGym.Id) return BadRequest();
        _db.Entry(updatedGym).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGym(int id)
    {
        var gym = await _db.Gyms.FindAsync(id);
        if (gym == null) return NotFound();
        _db.Gyms.Remove(gym);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
