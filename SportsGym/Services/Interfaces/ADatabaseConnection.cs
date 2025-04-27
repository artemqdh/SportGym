using WebApplication1.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Services.Interfaces
{
    public abstract class ADatabaseConnection : DbContext
    {
        protected abstract string ReturnConnectionString();

        protected string ConnectionString { get; private set; }

        public DbSet<Gym> Gyms => Set<Gym>();
        public DbSet<Trainer> Trainers => Set<Trainer>();
        public DbSet<Client> Clients => Set<Client>();

        public ADatabaseConnection()
        {
            this.ConnectionString = this.ReturnConnectionString();
            this.Database.EnsureCreated();
        }
    }
}
