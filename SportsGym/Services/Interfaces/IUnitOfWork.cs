using SportsGym.Models.Entities;

namespace SportsGym.Services.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Admin> AdminRepository { get; }
        IRepository<Booking> BookingRepository { get; }
        IRepository<Client> ClientRepository { get; }
        IRepository<Gym> GymRepository { get; }
        IRepository<Trainer> TrainerRepository { get; }
        IRepository<TrainingSchedule> TrainingsRepository { get; }

        void Commit();
        Task CommitAsync();
    }
}