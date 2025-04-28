namespace SportsGym.Models.Entities
{
    public class TrainingSchedule
    {
        public int Id { get; set; }

        public string Type { get; set; } = string.Empty; // "Individual" or "Group"

        public string TrainerName { get; set; } = string.Empty;

        public string ClientNames { get; set; } = string.Empty; // For group: "Client1, Client2, Client3" | For individual: just one client name

        public string Time { get; set; } = string.Empty; // "Monday|8:00|10:00"

        public string GymName { get; set; } = string.Empty;
    }
}
