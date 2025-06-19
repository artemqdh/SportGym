namespace SportsGym.Models.DTO
{
    public class TrainerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int PhoneNumber { get; set; }
        public string BirthDate { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string WorkingHours { get; set; } = string.Empty;
        public string GymName { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}