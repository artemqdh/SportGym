using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGym.Models.Dto;
using SportsGym.Models.DTO;
using SportsGym.Models.Entities;
using SportsGym.Services;

namespace SportsGym.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly PostgresConnection _db;

        public BookingController(PostgresConnection db)
        {
            _db = db;
        }

        [HttpGet("{trainerName}")]
        public async Task<IActionResult> GetTrainerBookings(string trainerName)
        {
            try
            {
                var bookings = await _db.Bookings
                    .Where(b => b.TrainerName == trainerName)
                    .Select(b => new BookingDTO
                    {
                        GymName = b.GymName,
                        TrainerName = trainerName,
                        ClientName = b.ClientName,
                        Date = b.Date,
                        StartTime = b.StartTime,
                        EndTime = b.EndTime
                    })
                    .ToListAsync();

                if (bookings == null || !bookings.Any())
                {
                    return NotFound("No bookings found.");
                }

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDTO dto)
        {
            try
            {
                Trainer trainer = await _db.Trainers.FirstOrDefaultAsync(t => t.Name == dto.TrainerName && t.GymName == dto.GymName);

                if (trainer == null)
                {
                    return BadRequest("Trainer not found in this gym.");
                }

                var booking = new Booking
                {
                    GymName = dto.GymName,
                    TrainerName = trainer.Name,
                    ClientName = dto.ClientName,
                    Date = dto.Date,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime
                };

                _db.Bookings.Add(booking);
                await _db.SaveChangesAsync();

                return Ok("Booking created successfully.");
            }
            catch (Exception ex)
            {
                // Log the error (optional)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
