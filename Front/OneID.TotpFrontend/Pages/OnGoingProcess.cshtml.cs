using Microsoft.AspNetCore.Mvc.RazorPages;

public class OngoingProcessesModel : PageModel
{
    public List<AdmissionProcessViewModel> Processes { get; set; } = [];

    public async Task OnGetAsync()
    {
        var token = Request.Cookies["access_token"];
        if (string.IsNullOrEmpty(token))
            return;

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var response = await client.GetAsync("https://localhost:7200/v1/admissions/ongoing");
        if (!response.IsSuccessStatusCode)
            return;

        var result = await response.Content.ReadFromJsonAsync<List<AdmissionProcessViewModel>>();
        if (result is not null)
            Processes = result;
    }

    public class AdmissionProcessViewModel
    {
        public string Id { get; set; } = default!;
        public string CorrelationId { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Login { get; set; } = default!;
        public string CurrentState { get; set; } = default!;
        public string EventName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime ProvisioningDate { get; set; }
    }
}
