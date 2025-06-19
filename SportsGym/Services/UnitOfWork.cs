using SportsGym.Models.Entities;
using SportsGym.Services.Interfaces;

namespace SportsGym.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PostgresConnection _db;

        public UnitOfWork(PostgresConnection db)
        {
            _db = db;
        }

        public IRepository<Admin> AdminRepository => new Repository<Admin>(_db);
        public IRepository<Booking> BookingRepository => new Repository<Booking>(_db);
        public IRepository<Client> ClientRepository => new Repository<Client>(_db);
        public IRepository<Gym> GymRepository => new Repository<Gym>(_db);
        public IRepository<Trainer> TrainerRepository => new Repository<Trainer>(_db);
        public IRepository<TrainingSchedule> TrainingsRepository => new Repository<TrainingSchedule>(_db);

        public void Commit()
        {
            using var transaction = _db.Database.BeginTransaction();
            try
            {
                _db.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        public async Task CommitAsync()
        {
            await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}