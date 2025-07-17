using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using OneID.Application.Interfaces.Graph;

namespace OneID.Application.Services.Graph
{
    public sealed class GraphServiceClientFactory : IGraphServiceClientFactory
    {
        private readonly IConfiguration _configuration;
        public GraphServiceClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public GraphServiceClient GetGraphServiceClient()
        {
            var clientId = _configuration["AzureAd:ClientId"];
            var tenantId = _configuration["AzureAd:TenantId"];
            var clientSecret = _configuration["AzureAd:ClientSecret"];

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            var graphClient = new GraphServiceClient(clientSecretCredential, ["https://graph.microsoft.com/.default"]);

            return graphClient;
        }

    }

}
