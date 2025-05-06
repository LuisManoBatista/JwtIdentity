using GameStore.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization.Metadata;

namespace GameStore.Api.Identity;

public class IdentitySignInManager(
    UserManager<ApplicationIdentityUser> userManager,
    IHttpContextAccessor contextAccessor,
    IUserClaimsPrincipalFactory<ApplicationIdentityUser> claimsFactory,
    IOptions<IdentityOptions> optionsAccessor,
    ILogger<SignInManager<ApplicationIdentityUser>> logger,
    IAuthenticationSchemeProvider schemes,
    IUserConfirmation<ApplicationIdentityUser> confirmation,
    IOptions<JwtSettings> jwtSettings) 
    : SignInManager<ApplicationIdentityUser>(
        userManager, 
        contextAccessor, 
        claimsFactory, 
        optionsAccessor, 
        logger, 
        schemes, 
        confirmation)
{

    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public override async Task SignInWithClaimsAsync(ApplicationIdentityUser user, AuthenticationProperties? authenticationProperties, IEnumerable<Claim> additionalClaims)
    {
        var userPrincipal = await CreateUserPrincipalAsync(user);
        foreach (var claim in additionalClaims)
        {
            userPrincipal.Identities.First().AddClaim(claim);
        }

        // Generate JWT token and create AccessTokenResponse
        var tokenResponse = await GenerateAccessTokenResponseAsync(userPrincipal);

        // Store it in HttpContext.Items for later retrieval
        Context.Items["AccessTokenResponse"] = tokenResponse;

        // Set the User property for the current request 
        Context.User = userPrincipal;
        await Context.Response.WriteAsJsonAsync(tokenResponse, ResolveAccessTokenJsonTypeInfo(Context));
    }

    public override Task SignOutAsync()
    {
        // JWT is stateless, so we don't need server-side sign-out
        return Task.CompletedTask;
    }

    // Override two-factor related methods if needed
    // For example:
    public override Task<bool> IsTwoFactorClientRememberedAsync(ApplicationIdentityUser user)
    {
        // Implement JWT-based two-factor client remember logic
        // This might involve checking a separate token or claim
        return Task.FromResult(false); // Default to always requiring 2FA
    }
    public async Task<AccessTokenResponse> GenerateAccessTokenResponseAsync(ClaimsPrincipal userPrincipal)
    {
        var jwtToken = await GenerateAccessTokenAsync(userPrincipal);
        string refreshToken = await GenerateRefreshTokenAsync(userPrincipal);

        return new AccessTokenResponse
        {
            AccessToken = jwtToken,
            ExpiresIn = (int)TimeSpan.FromMinutes(_jwtSettings.ExpiryMinutes).TotalSeconds,
            RefreshToken = refreshToken
        };
    }

    public async Task<AccessTokenResponse?> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return null;
        }

        var user = await (UserManager as IdentityUserManager)!.RefreshTokenFindUserAsync(refreshToken);
        if (user is null)
        {
            return null;
        }

        if (!await (UserManager as IdentityUserManager)!.ValidateRefreshTokenAsync(user, refreshToken))
        {
            return null;
        }
        
        var userPrincipal = await CreateUserPrincipalAsync(user);
        var tokenResponse = await GenerateAccessTokenResponseAsync(userPrincipal);
        return tokenResponse;
    }

    private static JsonTypeInfo<AccessTokenResponse> ResolveAccessTokenJsonTypeInfo(HttpContext httpContext)
    {
        // Attempt to resolve options from DI then fall back to static options
        var options = httpContext.RequestServices.GetRequiredService<IOptions<JsonOptions>>();
        var value = options.Value.SerializerOptions?.GetTypeInfo(typeof(AccessTokenResponse));
        if (value is not JsonTypeInfo<AccessTokenResponse> typeInfo )
        {
            throw new InvalidOperationException("Unable to resolve JsonTypeInfo for AccessTokenResponse.");
        }
        return typeInfo;
    }

    private Task<string> GenerateAccessTokenAsync(ClaimsPrincipal userPrincipal)
    {
        var claims = userPrincipal.Claims.ToList();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        return Task.FromResult(jwtToken);
    }

    private async Task<string> GenerateRefreshTokenAsync(ClaimsPrincipal userPrincipal)
    {
        // Generate a secure refresh token
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);

        // Save the refresh token with the user
        var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? userPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            var user = await UserManager.FindByIdAsync(userId);
            if (user is not null && UserManager is IdentityUserManager identityUserManager)
            {
                await identityUserManager.SetRefreshTokenAsync(user, refreshToken, DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpireDays));
            }
        }
        return refreshToken;
    }
}
