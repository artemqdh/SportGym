﻿namespace SportsGym.Models.Entities
{
    public class Client
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string BirthDate { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;
    }
}
