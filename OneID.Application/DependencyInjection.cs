using Microsoft.Extensions.DependencyInjection;

namespace OneID.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Ex: services.AddMediatR(typeof(DependencyInjection).Assembly);
            // Ex: services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
