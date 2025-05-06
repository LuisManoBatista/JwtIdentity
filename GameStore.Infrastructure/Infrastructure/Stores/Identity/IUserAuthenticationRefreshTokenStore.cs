using GameStore.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Infrastructure.Stores.Identity;

public interface IUserAuthenticationRefreshTokenStore : IUserAuthenticationTokenStore<ApplicationIdentityUser>
{
    Task<ApplicationIdentityUser?> RefreshTokenFindUserAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<string?> GetRefreshTokenAsync(ApplicationIdentityUser user, CancellationToken cancellationToken = default);

    Task SetRefreshTokenAsync(ApplicationIdentityUser user, string? value, CancellationToken cancellationToken = default);

    Task RemoveRefreshTokenAsync(ApplicationIdentityUser user, CancellationToken cancellationToken = default);

    Task<DateTime?> GetRefreshTokenExpiryAsync(ApplicationIdentityUser user, CancellationToken cancellationToken = default);

    Task SetRefreshTokenExpiryAsync(ApplicationIdentityUser user, DateTime? expiry, CancellationToken cancellationToken = default);

    Task RemoveRefreshTokenExpiryAsync(ApplicationIdentityUser user, CancellationToken cancellationToken = default);
}
