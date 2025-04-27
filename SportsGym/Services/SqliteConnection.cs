using WebApplication1.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Services
{
    public class SqliteConnection : ADatabaseConnection
    {
        private const string _DATABASE_NAME = "./Gym.db";

        protected override string ReturnConnectionString()
        {
            return $"Data Source={_DATABASE_NAME}";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(this.ConnectionString);
        }
    }
}
