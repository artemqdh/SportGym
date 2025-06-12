namespace SportsGym.Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public string GymName { get; set; } = string.Empty;
        public string TrainerName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int TrainerId { get; set; }
    }
}
