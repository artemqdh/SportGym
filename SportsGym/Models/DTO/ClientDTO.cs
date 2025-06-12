using Newtonsoft.Json;

namespace SportsGym.Models.DTO
{
    public class ClientDTO
    {
        public int Id { get; set; }

        [JsonProperty("userName")]  // Map to frontend expectation
        public string Name { get; set; }

        //[JsonProperty("birthDate")]
        public string BirthDate { get; set; }

        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }  // Change to string

        public string Gender { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }
    }
}