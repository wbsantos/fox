using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Fox.Settings;
using Fox.Access.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoxUser = Fox.Access.Model.User;

namespace Web.Fox.Pages;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private UserService _userService;

    [BindProperty(Name = "ReturnUrl", SupportsGet = true)]
    public string ReturnUrl { get; set; } = string.Empty;
    [BindProperty()]
    public string UserLogin { get; set; } = string.Empty;
    [BindProperty()]
    public string UserPassword { get; set; } = string.Empty;
    public string Msg { get; set; } = string.Empty;

    public LoginModel(UserService userService)
    {
        _userService = userService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    { 
        if (!_userService.ValidateUserPassword(UserLogin, UserPassword))
            return InvalidCredentials();

        FoxUser? userData = _userService.GetUser(UserLogin);
        if (userData == null)
            return InvalidCredentials();

        IEnumerable<string> permissions = _userService.GetSystemPermissions(userData.Id);
        IEnumerable<Claim> permissionsClaims = permissions.Select(permissionKey => new Claim("SystemPermission", permissionKey));

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(new[]
                                {
                                    new Claim(nameof(FoxUser.Id), userData.Id.ToString()),
                                    new Claim(nameof(FoxUser.Login), userData.Login),
                                    new Claim(nameof(FoxUser.Email), userData.Email),
                                    new Claim(nameof(FoxUser.Name), userData.Name)
                                }.Union(permissionsClaims),
                                CookieAuthenticationDefaults.AuthenticationScheme)),
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(1)
            }
        );

        if(string.IsNullOrEmpty(ReturnUrl))
            return RedirectToPage("Menu/Profile");
        else
            return Redirect(ReturnUrl);
    }

    private IActionResult InvalidCredentials()
    {
        Msg = "User or password invalid";
        return Page();
    }
}
