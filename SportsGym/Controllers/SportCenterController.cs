using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.Entities;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api")]
    public class SportCenterController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<dynamic> GetGyms()
        {
            GymService initializeGymsInfo = new GymService();

            List<Gym> gyms = initializeGymsInfo.GetGyms();

            return gyms;
        }
    }
}
