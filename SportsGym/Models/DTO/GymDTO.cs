namespace SportsGym.Models.DTO
{
    public class GymDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string AvailableTime { get; set; } = string.Empty;
    }
}
