using GameStore.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GameStore.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    private readonly UserService _userService;
    private readonly EmailService _emailService;

    public IdentityController(UserService userService, EmailService emailService)
    {
        _userService = userService;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registration)
    {
        var result = await _userService.RegisterUserAsync(registration);
        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        var user = await _userService.GetUserByEmailAsync(registration.Email);
        await _emailService.SendConfirmationEmailAsync(user!, registration.Email, HttpContext);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest login, [FromQuery] bool? useCookies, [FromQuery] bool? useSessionCookies)
    {
        var isPersistent = (useCookies == true) && (useSessionCookies != true);
        var result = await _userService.LoginUserAsync(login, isPersistent);

        if (!result.Succeeded)
        {
            return Unauthorized("Invalid login attempt.");
        }

        return new EmptyResult();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
    {
        var tokenResponse = await _userService.RefreshTokenAsync(refreshRequest.RefreshToken);
        if (tokenResponse == null)
        {
            return Unauthorized();
        }

        return Ok(tokenResponse);
    }

    [HttpGet("confirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string code, [FromQuery] string? changedEmail)
    {
        var result = await _emailService.ConfirmEmailAsync(userId, code, changedEmail);
        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        return Content("Thank you for confirming your email.");
    }

    private ActionResult CreateValidationProblem(IdentityResult result)
    {
        Debug.Assert(!result.Succeeded);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return ValidationProblem(new ValidationProblemDetails { Errors = errorDictionary });
    }
}
