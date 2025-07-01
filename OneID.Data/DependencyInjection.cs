using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OneID.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            // Ex: services.AddDbContext<MyDbContext>(options => ...);
            // Ex: services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
