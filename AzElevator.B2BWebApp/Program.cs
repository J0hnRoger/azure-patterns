using System.Security.Claims;
using AzElevator.B2BWebApp.Middlewares;
using AzElevator.B2BWebApp.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Graph;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = context =>
            {
                if (context.Principal.Identity is ClaimsIdentity identity)
                {
                    // Récupère les claims 'roles' du token brut
                    var tokenRoleClaims = context.SecurityToken?.Claims
                        .Where(c => c.Type == "roles") 
                        ?? Array.Empty<Claim>();

                    // On re-crée les claims sous la forme RoleClaimType
                    var identityRoleClaims = tokenRoleClaims
                        .Select(c => new Claim(identity.RoleClaimType, c.Value));

                    identity.AddClaims(identityRoleClaims);
                }

                return Task.CompletedTask;
            }
        };
    });

var tenantId = builder.Configuration["AzureAd:TenantId"];
var clientId = builder.Configuration["AzureAd:ClientId"];
var clientSecret = builder.Configuration["AzureAd:ClientSecret"];

var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

builder.Services.AddSingleton(clientSecretCredential);
builder.Services.AddSingleton(new GraphServiceClient(clientSecretCredential,
    new[] {"https://graph.microsoft.com/.default"}));

builder.Services.AddSingleton<TenantRepository>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Resellers.Invite",
        policy => policy.RequireRole("Resellers.Invite"));
});

builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    
    options.Filters.Add(new AuthorizeFilter(policy));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TenantValidationMiddleware>();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();