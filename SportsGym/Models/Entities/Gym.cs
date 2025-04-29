namespace SportsGym.Models.Entities
{
    public class Gym
    {
        public int Id { get; set; }

        public string Location { get; set; } = string.Empty;

        public string AvailableTime { get; set; } = string.Empty;
    }
}
