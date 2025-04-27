using WebApplication1.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Services.Interfaces
{
    public abstract class ADatabaseConnection : DbContext
    {
        protected abstract string ReturnConnectionString();

        protected string ConnectionString { get; private set; }

        public DbSet<Gym> Gyms => Set<Gym>();

        public ADatabaseConnection()
        {
            this.ConnectionString = this.ReturnConnectionString();
            this.Database.EnsureCreated();
        }
    }
}
