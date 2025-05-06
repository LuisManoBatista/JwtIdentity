using GameStore.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Api.Identity;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureIdentityManagers(this IServiceCollection services)
    {
        services.AddScoped<UserManager<ApplicationIdentityUser>, IdentityUserManager>();
        services.AddScoped<SignInManager<ApplicationIdentityUser>, IdentitySignInManager>();
        return services;
    }
}