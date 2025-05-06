using GameStore.Infrastructure.Models.Identity;
using GameStore.Infrastructure.Stores.Abstractions;
using GameStore.Infrastructure.Stores.Application;
using GameStore.Infrastructure.Stores.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameStore.Infrastructure;

public static class DataExtensions
{
    public static IServiceCollection ConfigureInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("GameStoreContext");
        services.AddSqlServer<ApplicationDbContext>(connectionString, options => options.MigrationsAssembly("GameStore.Api"));
        services.AddDbContext<IdentityDbContext>(
         options => options.UseSqlServer(connectionString, o => o.MigrationsAssembly("GameStore.Api")));

        services.AddScoped<IGamesQueryStore, GamesQueryStore>()
            .AddScoped<IGamesCommandStore, GamesCommandStore>()
            .AddScoped<IGamesStore, GamesStore>();

        services.AddScoped<IUserStore<ApplicationIdentityUser>, IdentityUserStore>();

        return services;
    }
}