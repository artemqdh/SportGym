using Microsoft.AspNetCore.Mvc;
using SportsGym.Services;

namespace SportsGym.Controllers
{
    [ApiController]
    [Route("api")]
    public class SportCenterController : ControllerBase
    {
        private readonly GymService _gymService;

        public SportCenterController(GymService gymService)
        {
            _gymService = gymService;
        }

        [HttpGet]
        public IEnumerable<dynamic> GetGyms()
        {
            return _gymService.GetGyms();
        }
    }
}
