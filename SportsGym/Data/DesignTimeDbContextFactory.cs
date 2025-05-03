using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SportsGym.Services;

namespace SportsGym.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PostgresConnection>
    {
        public PostgresConnection CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PostgresConnection>();

            // Point to your Docker Postgres on localhost:5433
            builder.UseNpgsql(
                "Host=localhost;Port=5433;Database=SportsGym;" +
                "Username=artem;Password=123;" +
                "Pooling=true;SSL Mode=Prefer;Trust Server Certificate=true"
            );

            return new PostgresConnection(builder.Options);
        }
    }
}
