using DotNetEnv;
using Gym.Core.Interfaces;
using Gym.Core.Interfaces.Services;
using Gym.Infrastructure.Data;
using Gym.Infrastructure.Repositores;
using Gym.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Infrastructure
{
    public static class InfrastrucutreRegistration
    {
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Try to load .env file from multiple possible locations (for development)
            var envPaths = new[]
            {
                ".env",
                "../.env",
                "../../.env",
                "../../../.env"
            };

            foreach (var path in envPaths)
            {
                if (File.Exists(path))
                {
                    Env.Load(path);
                    break;
                }
            }

            // Try multiple sources for connection string
            string connStr = Environment.GetEnvironmentVariable("MambelaDatabase") // Environment variable first
                ?? configuration.GetConnectionString("MambelaDatabase") // Then appsettings.json
                ?? "Server=(localdb)\\mssqllocaldb;Database=MambelaGymDB;Trusted_Connection=true;MultipleActiveResultSets=true;"; // Default fallback


            services.AddScoped<IUnitofWork, UnitofWork>();
            services.AddScoped<IReportService, ReportService>();

            services.AddDbContext<MambelaDbContext>((options) =>
            {
                options.UseSqlServer(connStr);
            });






            return services;
        }
    }
}
