using GameStore.Infrastructure.Models.Identity;
using GameStore.Infrastructure.Stores.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GameStore.Api.Identity;

public class IdentityUserManager(
    IUserStore<ApplicationIdentityUser> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<ApplicationIdentityUser> passwordHasher,
    IEnumerable<IUserValidator<ApplicationIdentityUser>> userValidators,
    IEnumerable<IPasswordValidator<ApplicationIdentityUser>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<IdentityUserManager> logger) : UserManager<ApplicationIdentityUser>(  
        store, 
        optionsAccessor, 
        passwordHasher, 
        userValidators, 
        passwordValidators, 
        keyNormalizer, 
        errors, 
        services, 
        logger)
{
    public async Task<ApplicationIdentityUser?> RefreshTokenFindUserAsync(string refreshToken)
    {
        ThrowIfDisposed();
        var refreshTokenStore = GetAuthenticationRefreshTokenStore();
        return await refreshTokenStore.RefreshTokenFindUserAsync(refreshToken).ConfigureAwait(false);
    }

    public async Task SetRefreshTokenAsync(ApplicationIdentityUser user, string refreshToken, DateTime expiryDate)
    {
        ThrowIfDisposed();
        var refreshTokenStore = GetAuthenticationRefreshTokenStore();

        // Remove existing refresh token and expiry
        await refreshTokenStore.RemoveRefreshTokenAsync(user).ConfigureAwait(false);
        await refreshTokenStore.RemoveRefreshTokenExpiryAsync(user).ConfigureAwait(false);

        // Set new refresh token and expiry
        await refreshTokenStore.SetRefreshTokenAsync(user, refreshToken).ConfigureAwait(false);
        await refreshTokenStore.SetRefreshTokenExpiryAsync(user, expiryDate).ConfigureAwait(false);
    }

    public async Task<bool> ValidateRefreshTokenAsync(ApplicationIdentityUser user, string refreshToken)
    {
        ThrowIfDisposed();
        var refreshTokenStore = GetAuthenticationRefreshTokenStore();

        // Get stored refresh token and expiry
        var storedRefreshToken = await refreshTokenStore.GetRefreshTokenAsync(user).ConfigureAwait(false);
        var expiryDate = await refreshTokenStore.GetRefreshTokenExpiryAsync(user).ConfigureAwait(false);

        // Validate token and expiry
        if (string.IsNullOrWhiteSpace(storedRefreshToken) || storedRefreshToken != refreshToken)
        {
            return false;
        }

        if (!expiryDate.HasValue || expiryDate.Value <= DateTime.UtcNow)
        {
            return false; // Token has expired
        }

        return true;
    }

    private IUserAuthenticationRefreshTokenStore GetAuthenticationRefreshTokenStore()
    {
        if (Store is not IUserAuthenticationRefreshTokenStore cast)
        {
            throw new NotSupportedException("Store is not IUserAuthenticationRefreshTokenStore");
        }
        return cast;
    }
}
