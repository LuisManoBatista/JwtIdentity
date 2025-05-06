namespace GameStore.Api.Services;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<EmailService>();
        services.AddScoped<UserService>();
        return services;
    }
}