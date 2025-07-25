﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OneID.Application.Interfaces.Keycloak;
using System.Text;

#nullable disable
namespace OneID.Application.Services.KeyCloakServices
{
    public class KeycloakUserCreator : IKeycloakUserCreator
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IKeycloakTokenService _tokenService;
        private readonly string _realm;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public KeycloakUserCreator(
            IHttpClientFactory httpClientFactory,
            IKeycloakTokenService tokenService,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;

            _realm = configuration["Keycloak:Realm"];
            _clientId = configuration["Keycloak:ClientId"];
            _clientSecret = configuration["Keycloak:ClientSecret"];
        }

        public async Task<Guid> CreateUserAsync(
            string username,
            string password,
            string email,
            string firstName,
            string lastName,
            CancellationToken ct)
        {
            var token = await _tokenService.GetAccessTokenAsync(ct);
            var client = _httpClientFactory.CreateClient("KeycloakAdmin");

            var payload = new
            {
                username,
                email,
                firstName,
                lastName,
                enabled = true,
                emailVerified = true,
                credentials = new[]
                {
                    new
                    {
                        type = "password",
                        value = password,
                        temporary = true
                    }
                }
            };

            var json = JsonConvert.SerializeObject(payload);

            var request = new HttpRequestMessage(HttpMethod.Post, $"/admin/realms/{_realm}/users")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                throw new InvalidOperationException($"Erro ao criar usuário no Keycloak: {response.StatusCode} - {content}");
            }

            var locationHeader = response.Headers.Location?.ToString();
            if (string.IsNullOrWhiteSpace(locationHeader))
                throw new InvalidOperationException("Não foi possível recuperar o ID do usuário do header 'Location'");

            var idSegment = locationHeader.Split('/').LastOrDefault();
            if (!Guid.TryParse(idSegment, out var parsedId))
                throw new InvalidOperationException($"ID do usuário do Keycloak não é um Guid válido: {idSegment}");

            return parsedId;
        }
    }

}
