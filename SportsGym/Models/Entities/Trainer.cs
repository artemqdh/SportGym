namespace SportsGym.Models.Entities
{
    public class Trainer
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string BirthDate { get; set; } = string.Empty;

        public int PhoneNumber { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public string WorkingHours { get; set; } = string.Empty; // "Monday|09:00|18:00;Tuesday|08:00|17:30;Wednesday|10:00|18:00;..."

        public string Login { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public int GymId { get; set; }
    }
}
