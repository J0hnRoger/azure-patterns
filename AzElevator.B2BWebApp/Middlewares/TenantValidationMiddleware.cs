using AzElevator.B2BWebApp.Services;

namespace AzElevator.B2BWebApp.Middlewares;

public class TenantValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TenantRepository _tenantRepository;

    public TenantValidationMiddleware(RequestDelegate next, TenantRepository tenantRepository)
    {
        _next = next;
        _tenantRepository = tenantRepository;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (path != null && path.StartsWith("/auth/adminconsent", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var tenantId = context.User.Claims
            .FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

        if (!string.IsNullOrEmpty(tenantId))
        {
            var isApproved = _tenantRepository.IsAllowed(tenantId);
            if (!isApproved)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Accès refusé : votre organisation n'a pas été approuvée.");
                return;
            }
        }

        await _next(context);
    }
}