namespace SportsGym.Models.DTO
{
    public class BookingDTO
    {
        public string GymName { get; set; } = string.Empty;
        public string TrainerName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;       ///< "2025-05-14"
        public string StartTime { get; set; } = string.Empty;  ///< "14:00"
        public string EndTime { get; set; } = string.Empty;    ///< "15:00"
        public int TrainerId { get; set; }
    }
}
