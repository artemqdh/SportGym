using System.ComponentModel.DataAnnotations;

namespace SportsGym.Models.Dto
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Login is required.")]
        public string Login { get; init; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; init; }
    }
}
