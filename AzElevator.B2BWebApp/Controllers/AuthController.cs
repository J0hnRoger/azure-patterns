using AzElevator.B2BWebApp.Services;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace AzElevator.B2BWebApp.Controllers;

[Authorize]
public class AuthController(
    TenantRepository tenantRepository)
    : Controller
{

    [HttpGet]
    public Task<IActionResult> AdminConsent([FromQuery] string admin_consent, [FromQuery] string tenant)
    {
        if (admin_consent == "True" && !string.IsNullOrEmpty(tenant))
        {
            tenantRepository.Add(tenant);
            return Task.FromResult<IActionResult>(Ok($"Le tenant {tenant} a été enregistré avec succès !"));
        }

        return Task.FromResult<IActionResult>(BadRequest("Consentement administrateur manquant ou invalide."));
    }
}