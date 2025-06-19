using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGym.Models.DTO;
using SportsGym.Models.Entities;
using SportsGym.Services.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class GymController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GymController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<List<GymDTO>> GetGyms()
    {
        var gyms = await _unitOfWork.GymRepository.GetAllAsync();
        return _mapper.Map<List<GymDTO>>(gyms);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Gym>> GetGym(int id)
    {
        var gym = await _unitOfWork.GymRepository.FindByIdAsync(id);
        return gym is not null ? gym : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Gym>> PostGym(Gym gym)
    {
        await _unitOfWork.GymRepository.AddAsync(gym);
        await _unitOfWork.CommitAsync();

        return CreatedAtAction(nameof(GetGym), new { id = gym.Id }, gym);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutGym(int id, Gym updatedGym)
    {
        if (id != updatedGym.Id) return BadRequest();

        _unitOfWork.GymRepository.Update(updatedGym);

        try
        {
            await _unitOfWork.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await _unitOfWork.GymRepository.FindByIdAsync(id) == null)
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGym(int id)
    {
        var gym = await _unitOfWork.GymRepository.FindByIdAsync(id);
        if (gym == null) return NotFound();

        _unitOfWork.GymRepository.Remove(gym);
        await _unitOfWork.CommitAsync();

        return NoContent();
    }
}
