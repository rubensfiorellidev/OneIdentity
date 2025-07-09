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
            if (settings == null)
                return NotFound("Configurações de alerta não encontradas.");

            var response = new AlertSettingsResponse
            {
                Id = settings.Id,
                CriticalRecipients = settings.CriticalRecipients,
                WarningRecipients = settings.WarningRecipients,
                InfoRecipients = settings.InfoRecipients
            };

            return ResponseOk("Configurações carregadas com sucesso.", response);
        }

        [HttpPost]
        [Route("", Name = nameof(CreateAsync))]
        public async Task<IActionResult> CreateAsync([FromBody] AlertSettingsRequest request, CancellationToken ct)
        {
            var exists = await _alertSettingsRepository.ExistsAsync(ct);
            if (exists)
                return Conflict("Configurações já existem. Use PUT para atualizar.");

            var entity = new AlertSettings
            {
                CriticalRecipients = request.CriticalRecipients,
                WarningRecipients = request.WarningRecipients,
                InfoRecipients = request.InfoRecipients
            };

            await _alertSettingsRepository.AddAsync(entity, ct);

            var response = new AlertSettingsResponse
            {
                Id = entity.Id,
                CriticalRecipients = entity.CriticalRecipients,
                WarningRecipients = entity.WarningRecipients,
                InfoRecipients = entity.InfoRecipients
            };

            return ResponseCreated(response);

        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] AlertSettingsRequest request, CancellationToken ct)
        {
            var existing = await _alertSettingsRepository.GetAsync(ct);
            if (existing == null)
                return NotFound("Nenhuma configuração encontrada para atualizar.");

            existing.CriticalRecipients = request.CriticalRecipients;
            existing.WarningRecipients = request.WarningRecipients;
            existing.InfoRecipients = request.InfoRecipients;

            await _alertSettingsRepository.UpdateAsync(existing, ct);

            return NoContent();
        }
    }
}
