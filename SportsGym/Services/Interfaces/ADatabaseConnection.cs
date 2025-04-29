using SportsGym.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace SportsGym.Services.Interfaces
{
    public abstract class ADatabaseConnection : DbContext
    {
        public string ConnectionString { get; }

        // Constructor for DI
        protected ADatabaseConnection(DbContextOptions options) : base(options)
        {
            this.ConnectionString = this.ReturnConnectionString();
        }

        // Constructor for manual creation
        protected ADatabaseConnection() : base()
        {
            this.ConnectionString = this.ReturnConnectionString();
        }

        protected abstract string ReturnConnectionString();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(this.ConnectionString);
            }
        }

        public DbSet<Gym> Gyms => Set<Gym>();
        public DbSet<Trainer> Trainers => Set<Trainer>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<TrainingSchedule> Trainings => Set<TrainingSchedule>();
        public DbSet<Admin> Admins => Set<Admin>();

        
    }
}
