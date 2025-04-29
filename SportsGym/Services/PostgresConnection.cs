using Microsoft.EntityFrameworkCore;
using SportsGym.Services.Interfaces;

namespace SportsGym.Services
{
    public class PostgresConnection : ADatabaseConnection
    {
        // Constructor for DI
        public PostgresConnection(DbContextOptions<PostgresConnection> options)
            : base(options)
        {
        }

        // Constructor for manual creation
        public PostgresConnection() : base()
        {
        }

        protected override string ReturnConnectionString()
        {
            string host = Environment.GetEnvironmentVariable("DATABASE_SERVER_NAME") ?? "postgres";
            string port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";
            string user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
            string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "your_password";
            string database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "SportsGym";

            return $"Host={host};Port={port};Database={database};" +
                   $"Username={user};Password={password};" +
                   "Pooling=true;SSL Mode=Prefer;Trust Server Certificate=true";
        }
    }
}
