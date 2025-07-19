using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OneID.TotpFrontend.Tokens;
using System.Net.Http.Headers;
using System.Text.Json;


#nullable disable
public class StartModel : PageModel
{
    private readonly ITotpTokenGenerator _totpTokenGenerator;

    public StartModel(ITotpTokenGenerator totpTokenGenerator)
    {
        _totpTokenGenerator = totpTokenGenerator;
    }

    [BindProperty] public string FirstName { get; set; }
    [BindProperty] public string LastName { get; set; }
    [BindProperty] public string FullName { get; set; }
    [BindProperty] public string SocialName { get; set; }
    [BindProperty] public string Cpf { get; set; }
    [BindProperty] public DateTime BirthDate { get; set; }
    [BindProperty] public string Registry { get; set; }
    [BindProperty] public string MotherName { get; set; }
    [BindProperty] public string Company { get; set; }
    [BindProperty] public string TypeUserAccount { get; set; }
    [BindProperty] public string LoginManager { get; set; }
    [BindProperty] public string FiscalNumberIdentity { get; set; }
    [BindProperty] public DateTime StartDate { get; set; }
    [BindProperty] public DateTime? EndDate { get; set; }
    [BindProperty] public string ContractorCnpj { get; set; }
    [BindProperty] public string ContractorName { get; set; }
    [BindProperty] public string JobTitleId { get; set; }
    [BindProperty] public string JobTitleName { get; set; }
    [BindProperty] public string DepartmentId { get; set; }
    [BindProperty] public string DepartmentName { get; set; }
    [BindProperty] public string Login { get; set; }
    [BindProperty] public string PersonalEmail { get; set; }
    [BindProperty] public string CorporateEmail { get; set; }
    [BindProperty] public string PhoneNumber { get; set; }
    [BindProperty] public string Comments { get; set; }

    public async Task<IActionResult> OnPostSubmitAsync()
    {
        var token = Request.Cookies["access_token"];
        if (string.IsNullOrEmpty(token))
        {
            ModelState.AddModelError(string.Empty, "Token ausente. Refaça o login.");
            return Page();
        }

        var payload = new
        {
            firstName = FirstName,
            lastName = LastName,
            fullName = FullName,
            socialName = SocialName,
            cpf = Cpf,
            cpfHash = (string)null,
            birthDate = BirthDate.ToString("yyyy-MM-dd"),
            registry = Registry,
            motherName = MotherName,
            company = Company,
            typeUserAccount = TypeUserAccount,
            loginManager = LoginManager,
            fiscalNumberIdentity = FiscalNumberIdentity,
            fiscalNumberIdentityHash = (string)null,
            startDate = StartDate.ToString("yyyy-MM-dd"),
            endDate = EndDate?.ToString("yyyy-MM-dd"),
            contractorCnpj = ContractorCnpj,
            contractorCnpjHash = (string)null,
            contractorName = ContractorName,
            jobTitleId = JobTitleId,
            jobTitleName = JobTitleName,
            departmentId = DepartmentId,
            departmentName = DepartmentName,
            login = Login,
            loginHash = (string)null,
            personalEmail = PersonalEmail,
            personalEmailHash = (string)null,
            corporateEmail = CorporateEmail,
            corporateEmailHash = (string)null,
            phoneNumber = PhoneNumber,
            comments = Comments
        };

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("https://localhost:7200/v1/staging/start", payload);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Erro ao iniciar provisionamento: {error}");
            return Page();
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var correlationId = json.GetProperty("correlationId").GetString();

        Response.Cookies.Append("correlation_id", correlationId!, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(5)
        });

        var claims = new Dictionary<string, object>
        {
            { "sub", User.Identity?.Name ?? "unknown" },
            { "preferred_username", User.Identity?.Name ?? "unknown" },
            { "correlation_id", correlationId! }
        };

        var requestToken = _totpTokenGenerator.GenerateToken(claims, TimeSpan.FromMinutes(5));

        Response.Cookies.Append("request_token", requestToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(5)
        });

        return Redirect("/confirm-code");
    }

}
