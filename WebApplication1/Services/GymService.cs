using System.Dynamic;
using System.Runtime.CompilerServices;
using WebApplication1.Models.Entities;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services
{
    public class GymService
    {
        private ADatabaseConnection _db = new SqliteConnection();

        public GymService()
        { 
            if (this._db.Gyms.FirstOrDefault() == null)
            {
                Gym gym1 = new Gym();
                gym1.Location = $"New York";
                gym1.AvailableTime = "23.04.2025;08:00-14:00";

                Gym gym2 = new Gym();
                gym2.Location = $"Miami";
                gym2.AvailableTime = "25.04.2025;09:00-15:00";

                Gym gym3 = new Gym();
                gym3.Location = $"Moscow";
                gym3.AvailableTime = "24.04.2025;10:00-18:00";

                this._db.Gyms.Add(gym1);
                this._db.Gyms.Add(gym2);
                this._db.Gyms.Add(gym3);

                this._db.SaveChanges();
            }
        }

        public List<Gym> GetGyms()
        {
            List<Gym> gyms = new List<Gym>();

            for (int i = 1; i < this._db.Gyms.Count() + 1; i++)
            {
                Gym gym = this._db.Gyms.Where(a => a.Id == i).FirstOrDefault();
                gyms.Add(gym);
            }

            return gyms;
        }
    }
}
