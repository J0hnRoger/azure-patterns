using AzElevator.B2BWebApp.Services;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace AzElevator.B2BWebApp.Controllers;

[Authorize]
public class BusinessController(
    GraphServiceClient graphServiceClient,
    ClientSecretCredential clientSecretCredential,
    TenantRepository tenantRepository)
    : Controller
{
    public async Task<IActionResult> TestToken()
    {
        try
        {
            // ✅ Récupérer un token valide
            var token = await clientSecretCredential.GetTokenAsync(
                new TokenRequestContext(new[] {"https://graph.microsoft.com/.default"})
            );

            return Content($"Token récupéré : {token.Token}");
        }
        catch (Exception ex)
        {
            return Content($"Erreur : {ex.Message}");
        }
    }

    [Authorize(Roles = "CreateTenant.Resellers")]
    public IActionResult ProtectedByRolePermission()
    {
        return View();
    } 
}