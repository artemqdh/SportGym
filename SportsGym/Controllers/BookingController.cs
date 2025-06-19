using Microsoft.AspNetCore.Mvc;
using SportsGym.Models.DTO;
using SportsGym.Models.Entities;
using SportsGym.Services.Interfaces;

namespace SportsGym.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{trainerName}")]
        public async Task<IActionResult> GetTrainerBookings(string trainerName)
        {
            try
            {
                var bookings = await _unitOfWork.BookingRepository
                    .FindAllAsync(b => b.TrainerName == trainerName);

                var bookingDtos = bookings.Select(b => new BookingDTO
                {
                    GymName = b.GymName,
                    TrainerName = b.TrainerName,
                    ClientName = b.ClientName,
                    Date = b.Date,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime
                }).ToList();

                return Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var trainer = await _unitOfWork.TrainerRepository
                    .FirstOrDefaultAsync(t => t.Name == dto.TrainerName && t.GymName == dto.GymName);

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
                    EndTime = dto.EndTime,
                    TrainerId = dto.TrainerId
                };

                _unitOfWork.BookingRepository.Add(booking);
                await _unitOfWork.CommitAsync();

                return Ok("Booking created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
