namespace SportsGym.Models.Entities
{
    public class Admin
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string BirthDate { get; set; } = string.Empty;

        public int PhoneNumber { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty; // "Manager", "Chief Manager",...

        public string Login { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
