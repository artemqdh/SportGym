using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDTO dto)
        {
            // 1) Ensure trainer exists and belongs to this gym
            var trainer = await _db.Trainers.FindAsync(dto.TrainerId);
            if (trainer == null || trainer.GymId != dto.GymId)
                return BadRequest("Trainer not available in this gym.");

            // 2) Parse date+times into DateTime
            if (!DateTime.TryParse($"{dto.Date}T{dto.StartTime}", out var start) ||
                !DateTime.TryParse($"{dto.Date}T{dto.EndTime}", out var end))
            {
                return BadRequest("Invalid date or time format.");
            }

            // 3) Optional: check for conflicting bookings
            bool conflict = _db.Bookings.Any(b =>
                b.TrainerId == dto.TrainerId &&
                b.StartTime == start
            );
            if (conflict)
                return Conflict("Trainer is already booked at that time.");

            // 4) Create entity and save
            var booking = new Booking
            {
                GymId = dto.GymId,
                TrainerId = dto.TrainerId,
                StartTime = start,
                EndTime = end
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            return Ok("Booking created successfully.");
        }
    }
}
