namespace SportsGym.Models.DTO
{
    public class BookingDTO
    {
        public int GymId { get; set; }
        public int TrainerId { get; set; }
        public string Date { get; set; } = string.Empty;       // e.g. "2025-05-14"
        public string StartTime { get; set; } = string.Empty;  // e.g. "14:00"
        public string EndTime { get; set; } = string.Empty;    // e.g. "15:00"
    }
}
