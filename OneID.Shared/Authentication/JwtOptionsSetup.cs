using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace OneID.Shared.Authentication
{
    public sealed class JwtOptionsSetup : IConfigureOptions<JwtOptions>
    {
        private const string _sectionName = "JwtOptions";
        private readonly IConfiguration _configuration;

        public JwtOptionsSetup(IConfiguration configuration) => _configuration = configuration;

        public void Configure(JwtOptions options)
        {
            _configuration.GetSection(_sectionName).Bind(options);
        }
    }
}
