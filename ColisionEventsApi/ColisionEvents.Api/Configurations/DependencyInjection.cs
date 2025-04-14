using CollisionEvents.Application.CollisionWarnings;
using CollisionEvents.Application.UserRequest;
using CollisionEvents.Infrastructure.Repositories;

namespace CollisionEvents.Api.Configurations
{
    /// <summary>
    /// Dependency injection configurations
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Add application services dependecy injection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<IUserRequestService, UserRequestService>();

            return services.Scan(scan =>
            scan.FromAssemblyOf<CollisionWarningsService>()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        }

        /// <summary>
        /// Add infrastructure services dependecy injection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services.Scan(scan =>
            scan.FromAssemblyOf<AppDbContext>()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        }
    }
}
