using GameStore.Api.Identity;
using GameStore.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace GameStore.Api.Services;

public class UserService(IdentityUserManager userManager, IdentitySignInManager signInManager)
{
    public async Task<IdentityResult> RegisterUserAsync(RegisterRequest registration)
    {
        var user = new ApplicationIdentityUser();
        await userManager.SetUserNameAsync(user, registration.Email);
        await userManager.SetEmailAsync(user, registration.Email);
        return await userManager.CreateAsync(user, registration.Password);
    }

    public async Task<SignInResult> LoginUserAsync(LoginRequest login, bool isPersistent)
    {
        return await signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent, lockoutOnFailure: true);
    }

    public async Task<AccessTokenResponse?> RefreshTokenAsync(string refreshToken)
    {
        return await signInManager.RefreshTokenAsync(refreshToken);
    }

    public async Task<ApplicationIdentityUser?> GetUserByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }
}


