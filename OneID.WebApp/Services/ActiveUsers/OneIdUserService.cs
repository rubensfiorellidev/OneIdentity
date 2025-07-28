using Newtonsoft.Json;
using OneID.WebApp.Interfaces;
using static OneID.WebApp.Components.Pages.ActiveUsers;

namespace OneID.WebApp.Services.ActiveUsers
{
    public class OneIdUserService : IOneIdUserService
    {
        private readonly HttpClient _httpClient;

        public OneIdUserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<ActiveUserViewModel>> GetActiveUsersAsync()
        {
            var response = await _httpClient.GetAsync("/v1/users/all");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<ActiveUserViewModel>>(json)!;
        }

    }

}
