using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using OneID.Application.Interfaces.Graph;
using OneID.Application.Messaging.Sagas.Contracts.Events;
using OneID.Domain.Results;

#nullable disable
namespace OneID.Application.Services.Graph
{
    public sealed class AzureGraphUserSyncService : IAzureGraphUserSyncService
    {
        private readonly IGraphServiceClientFactory _graphClientFactory;
        private readonly ILogger<AzureGraphUserSyncService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAccessPackageGroupService _groupService;

        public AzureGraphUserSyncService(
            ILogger<AzureGraphUserSyncService> logger,
            IConfiguration configuration,
            IGraphServiceClientFactory graphClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _graphClientFactory = graphClientFactory;
        }

        public async Task<Result> CreateUserAsync(AzureUserCreationRequested request, CancellationToken cancellationToken = default)
        {
            try
            {
                var graphClient = _graphClientFactory.GetGraphServiceClient();
                var domain = _configuration["AzureAd:Domain"];
                var userPrincipalName = $"{request.Login}@{domain}";

                var user = new User
                {
                    AccountEnabled = true,
                    DisplayName = $"{request.FirstName} {request.LastName}",
                    GivenName = request.FirstName,
                    Surname = request.LastName,
                    MailNickname = request.Login,
                    UserPrincipalName = userPrincipalName,
                    Mail = request.Email,
                    JobTitle = "Colaborador",
                    Department = "TI",
                    CompanyName = "OneID",
                    OnPremisesSamAccountName = request.Login,
                    PasswordProfile = new PasswordProfile
                    {
                        ForceChangePasswordNextSignIn = true,
                        Password = request.Password
                    }
                };

                await graphClient.Users.PostAsync(user, cancellationToken: cancellationToken);

                var createdUser = await graphClient.Users[request.Email].GetAsync(cancellationToken: cancellationToken);

                if (!string.IsNullOrEmpty(request.ManagerLogin))
                {
                    var managerUser = await graphClient.Users[request.ManagerLogin]
                        .GetAsync(cancellationToken: cancellationToken);

                    var referenceUpdate = new ReferenceUpdate
                    {
                        OdataId = $"https://graph.microsoft.com/v1.0/users/{managerUser.Id}"
                    };

                    await graphClient.Users[createdUser.Id].Manager.Ref
                        .PutAsync(referenceUpdate, cancellationToken: cancellationToken);
                }

                var groupNames = await _groupService.ResolveGroupsForUserAsync(request.CorrelationId, cancellationToken);

                if (groupNames.Any())
                {
                    var allGroups = await graphClient.Groups.GetAsync(cancellationToken: cancellationToken);

                    foreach (var name in groupNames)
                    {
                        var group = allGroups.Value.FirstOrDefault(g => g.DisplayName == name);
                        if (group is null) continue;

                        var referenceCreate = new ReferenceCreate
                        {
                            OdataId = $"https://graph.microsoft.com/v1.0/users/{createdUser.Id}"
                        };

                        await graphClient.Groups[group.Id].Members.Ref
                            .PostAsync(referenceCreate, cancellationToken: cancellationToken);
                    }
                }

                return Result.Success("Usuário criado com sucesso e gerente atribuído.");
            }
            catch (ODataError ex)
            {
                _logger.LogError(ex, "Erro do Graph API: {Message}", ex.Error?.Message);
                return Result.Failure("Erro do Graph API: " + ex.Error?.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar usuário");
                return Result.Failure("Erro inesperado ao criar usuário.");
            }

        }

    }

}
