using Microsoft.AspNetCore.WebUtilities;
using OneID.WebApp.Interfaces;
using OneID.WebApp.ViewModels;
using OneID.WebApp.Wrappers;
using System.Text.Json;

#nullable disable
namespace OneID.WebApp.Services.ActiveUsers
{
    public class OneIdUserService : IOneIdUserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public OneIdUserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<IReadOnlyList<AllUserViewModel>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync("/v1/users", HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var users = new List<AllUserViewModel>(capacity: 128);

            await foreach (var user in JsonSerializer.DeserializeAsyncEnumerable<AllUserViewModel>(
                await response.Content.ReadAsStreamAsync(cancellationToken),
                _jsonOptions,
                cancellationToken))
            {
                if (user is not null)
                    users.Add(user);
            }

            return [.. users];
        }

        public async Task<PaginatedUsersViewModel> GetUsersAsync(
            int page,
            int pageSize,
            string search,
            string sortBy,
            bool descending,
            CancellationToken cancellationToken)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["page"] = page.ToString(),
                ["pageSize"] = pageSize.ToString(),
                ["search"] = search ?? "",
                ["sortBy"] = sortBy ?? "",
                ["descending"] = descending.ToString().ToLowerInvariant()
            };

            var url = QueryHelpers.AddQueryString("/v1/users", queryParams);

            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            var wrapper = await JsonSerializer.DeserializeAsync<ApiResponse<List<AllUserViewModel>>>(
                stream, _jsonOptions, cancellationToken);

            var users = wrapper?.Data ?? [];

            return new PaginatedUsersViewModel(users, users.Count);
        }


    }

}
