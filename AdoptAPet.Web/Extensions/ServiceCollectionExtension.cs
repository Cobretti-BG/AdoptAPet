using AdoptAPet.Data;
using AdoptAPet.Data.Models;
using Microsoft.EntityFrameworkCore;

using AdoptAPet.Service.Contracts;
using AdoptAPet.Service.Services;

namespace AdoptAPet.Web.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationDbContext(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddApplicationIdentity(
            this IServiceCollection services)
        {
            services.AddDefaultIdentity<ApplicationUser>(options =>
                    options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
        {
            services.AddScoped<IPetService, PetService>();

            return services;
        }
    }
}
