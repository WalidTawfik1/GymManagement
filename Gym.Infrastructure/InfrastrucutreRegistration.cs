using DotNetEnv;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Data;
using Gym.Infrastructure.Repositores;
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
            // Try to load .env file from multiple possible locations
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

            string connStr = Environment.GetEnvironmentVariable("MambelaDatabase")
               ?? throw new InvalidOperationException("Connection string not found in environment variables.");


            services.AddScoped<IUnitofWork, UnitofWork>();


            services.AddDbContext<MambelaDbContext>((options) =>
            {
                options.UseSqlServer(connStr);
            });






            return services;
        }
    }
}
