namespace AzElevator.B2BWebApp.Services;

public class TenantRepository
{
    private static List<string> _allowedTenants = new();

    public void Add(string tenantId)
    {
        _allowedTenants.Add(tenantId);
    }

    public bool IsAllowed(string tenantId)
    {
        return _allowedTenants.Contains(tenantId);
    } 
}