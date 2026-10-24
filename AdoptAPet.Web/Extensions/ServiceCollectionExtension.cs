namespace AdoptAPet.Web.Extensions
{
    using Microsoft.EntityFrameworkCore;

    using Data;
    using Data.Models;

    using Service.Contracts;
    using Service.Services;
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
            services.AddScoped<IAdoptionRequestService, AdoptionRequestService>();

            return services;
        }
    }
}
