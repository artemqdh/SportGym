using Microsoft.EntityFrameworkCore;
using SportsGym.Services.Interfaces;

namespace SportsGym.Services
{
    public class PostgresConnection : ADatabaseConnection
    {
        public PostgresConnection(DbContextOptions<PostgresConnection> options)
            : base(options)
        {
        }

        public PostgresConnection() : base()
        {
        }

        protected override string ReturnConnectionString()
        {
            string host = Environment.GetEnvironmentVariable("DATABASE_SERVER_NAME");
            string port = Environment.GetEnvironmentVariable("DATABASE_PORT");
            string user = Environment.GetEnvironmentVariable("POSTGRES_USER");
            string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            string database = Environment.GetEnvironmentVariable("POSTGRES_DB");

            return $"Host={host};Port={port};Database={database};" +
                   $"Username={user};Password={password};" +
                   "Pooling=true;SSL Mode=Prefer;Trust Server Certificate=true";
        }
    }
}
