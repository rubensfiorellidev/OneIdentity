using Microsoft.Extensions.DependencyInjection;
using OneID.Application.Builders;
using OneID.Application.Interfaces;

namespace OneID.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<IApplicationUserBuilder, ApplicationUserBuilder>();

            return services;
        }
    }
}
