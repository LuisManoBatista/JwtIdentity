using GameStore.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GameStore.Infrastructure.Stores.Identity;

internal class IdentityUserStore(IdentityDbContext context, IdentityErrorDescriber? describer = null) 
    : UserStore<ApplicationIdentityUser>(context, describer), IUserAuthenticationRefreshTokenStore
{
    const string JwtLoginProvider = "JWT";
    const string JwtRefreshTokenName = "RefreshToken";
    const string JwtRefreshTokenExpiryName = "RefreshTokenExpiry";

    public Task<ApplicationIdentityUser?> RefreshTokenFindUserAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);
        if (Context is not IdentityDbContext dbContext)
        {
            return Task.FromResult<ApplicationIdentityUser?>(null);
        }

        var tokenEntry = dbContext.UserTokens.FirstOrDefault(t => t.Value == refreshToken && t.LoginProvider == JwtLoginProvider && t.Name == JwtRefreshTokenName);
        if (tokenEntry is null)
        {
            return Task.FromResult<ApplicationIdentityUser?>(null);
        }

        return FindUserAsync(tokenEntry.UserId, cancellationToken);
    }

    public Task SetRefreshTokenAsync(ApplicationIdentityUser user, string? value, CancellationToken cancellationToken = default)
    {
        return base.SetTokenAsync(user, JwtLoginProvider, JwtRefreshTokenName, value, cancellationToken);
    }

    public Task RemoveRefreshTokenAsync(ApplicationIdentityUser user, CancellationToken cancellationToken = default)
    {
        return base.RemoveTokenAsync(user, JwtLoginProvider, JwtRefreshTokenName, cancellationToken);
    }

    public Task<string?> GetRefreshTokenAsync(ApplicationIdentityUser user, CancellationToken cancellationToken = default)
    {
        return base.GetTokenAsync(user, JwtLoginProvider, JwtRefreshTokenName, cancellationToken);
    }

    public Task<DateTime?> GetRefreshTokenExpiryAsync(ApplicationIdentityUser user, CancellationToken cancellationToken = default)
    {
        var expiryString = base.GetTokenAsync(user, JwtLoginProvider, JwtRefreshTokenExpiryName, cancellationToken).Result;
        if (DateTime.TryParse(expiryString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var expiry))
        {
            return Task.FromResult<DateTime?>(expiry);
        }
        return Task.FromResult<DateTime?>(null);
    }

    public Task SetRefreshTokenExpiryAsync(ApplicationIdentityUser user, DateTime? expiry, CancellationToken cancellationToken = default)
    {
        var expiryString = expiry?.ToString("O"); // Use ISO 8601 format
        return base.SetTokenAsync(user, JwtLoginProvider, JwtRefreshTokenExpiryName, expiryString, cancellationToken);
    }

    public Task RemoveRefreshTokenExpiryAsync(ApplicationIdentityUser user, CancellationToken cancellationToken = default)
    {
        return base.RemoveTokenAsync(user, JwtLoginProvider, JwtRefreshTokenExpiryName, cancellationToken);
    }
}