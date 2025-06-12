namespace SportsGym.Models.DTO
{
    public class AdminDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string BirthDate { get; set; } = string.Empty;

        public int PhoneNumber { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty; // "Manager", "Chief Manager",...
    }
}
