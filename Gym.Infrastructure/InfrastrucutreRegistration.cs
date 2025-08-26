using DotNetEnv;
using Gym.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Infrastructure
{
    public static class InfrastrucutreRegistration
    {
        public static IServiceCollection InfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            Env.Load();
            string connStr = Environment.GetEnvironmentVariable("MambelaDatabase")
               ?? throw new InvalidOperationException("Connection string not found in environment variables.");

            services.AddDbContext<MambelaDbContext>((options) =>
            {
                options.UseSqlServer(connStr);
            });






            return services;
        }
    }
}
