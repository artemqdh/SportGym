namespace SportsGym.Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int GymId { get; set; }
        public int TrainerId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
