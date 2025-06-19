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

        public DbSet<Gym> Gyms { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<TrainingSchedule> Trainings { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Booking> Bookings { get; set; }


        public void BeginTransaction()
        {
            Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            Database.RollbackTransaction();
        }
    }
}
