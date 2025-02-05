using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AzElevator.B2BWebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace AzElevator.B2BWebApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var user = User.Identity.Name;
        var tenantId = User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value; // Récupérer le Tenant ID
        var claims = String.Join('\n', User.Claims);
        var rawRoles = User.Claims
                    .Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                    .Select(c => c.Value).ToList();
        bool inRole = User.IsInRole("Members.Resellers");
        return View(model: $"Bienvenue {user}, votre tenant ID est : {tenantId} - vos Claims: {claims} <br/>\n\n Roles: {string.Join('\n', rawRoles)} - IsRole - Members: {inRole}");
    }

    public IActionResult Logout()
    {
        return SignOut("Cookies", "OpenIdConnect");
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}