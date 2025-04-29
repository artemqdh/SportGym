using System.Dynamic;
using System.Runtime.CompilerServices;
using SportsGym.Models.Entities;
using SportsGym.Services.Interfaces;

namespace SportsGym.Services
{
    public class GymService
    {
        private ADatabaseConnection _db;

        public GymService(ADatabaseConnection db)
        { 
            this._db = db;

            SeedInitialData();
        }

        private void SeedInitialData()
        {
            if (!_db.Gyms.Any())
            {
                List<Gym> gyms = new List<Gym>
                {
                    new Gym { Location = "New York", AvailableTime = "23.04.2025;08:00-14:00" },
                    new Gym { Location = "Miami", AvailableTime = "25.04.2025;09:00-15:00" },
                    new Gym { Location = "Moscow", AvailableTime = "24.04.2025;10:00-18:00" }
                };

                _db.Gyms.AddRange(gyms);
                _db.SaveChanges();
            }
        }

        public List<Gym> GetGyms()
        {
            return _db.Gyms.OrderBy(g => g.Id).ToList();
        }
    }
}
