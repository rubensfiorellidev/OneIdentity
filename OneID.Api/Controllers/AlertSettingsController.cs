using MediatR;
using Microsoft.AspNetCore.Mvc;
using OneID.Application.DTOs.Alerts;
using OneID.Application.Interfaces.Repositories;
using OneID.Domain.Entities.AlertsContext;

namespace OneID.Api.Controllers
{
    [Route("v1/alert-settings")]
    public class AlertSettingsController : MainController
    {
        private readonly IAlertSettingsRepository _alertSettingsRepository;
        public AlertSettingsController(ISender sender, IAlertSettingsRepository alertSettingsRepository) : base(sender)
        {
            _alertSettingsRepository = alertSettingsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(CancellationToken ct)
        {
            var settings = await _alertSettingsRepository.GetAsync(ct);
            return Ok(settings);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateAlertSettingsRequest request, CancellationToken ct)
        {
            var updated = new AlertSettings
            {
                CriticalRecipients = request.CriticalRecipients,
                WarningRecipients = request.WarningRecipients,
                InfoRecipients = request.InfoRecipients
            };

            await _alertSettingsRepository.UpdateAsync(updated, ct);

            return NoContent();
        }
    }
}
