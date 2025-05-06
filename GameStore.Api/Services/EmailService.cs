using GameStore.Api.Identity;
using GameStore.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace GameStore.Api.Services;

public class EmailService(
    IdentityUserManager userManager, 
    IEmailSender<ApplicationIdentityUser> emailSender, 
    LinkGenerator linkGenerator)
{
    public async Task SendConfirmationEmailAsync(ApplicationIdentityUser user, string email, HttpContext httpContext)
    {
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var userId = await userManager.GetUserIdAsync(user);
        var confirmEmailUrl = linkGenerator.GetUriByAction(
            httpContext,
            action: "ConfirmEmail",
            controller: "Identity",
            values: new { userId, code });

        await emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl!));
    }

    public async Task<IdentityResult> ConfirmEmailAsync(string userId, string code, string? changedEmail)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        if (string.IsNullOrEmpty(changedEmail))
        {
            return await userManager.ConfirmEmailAsync(user, code);
        }
        else
        {
            var result = await userManager.ChangeEmailAsync(user, changedEmail, code);
            if (result.Succeeded)
            {
                return await userManager.SetUserNameAsync(user, changedEmail);
            }
            return result;
        }
    }
}


