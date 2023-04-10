using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fox.Access.Repository;
using Fox.Access.Model;
using FoxUser = Fox.Access.Model.User;
using Microsoft.AspNetCore.Authorization;

namespace Web.Fox.Pages.Menu;

[Authorize(Policy = "USER_SELF_MANAGEMENT")]
public class ProfileModel : PageModel, INavBarItem
{
    public string MenuDescription => "Profile";
    public string PagePath => "/Menu/Profile";
    public LoggedUser LoggedUser { get; set; }
    public string MenuCategory => "SELF_MANAGEMENT";

    UserRepository _userRepo;
    public ProfileModel(UserRepository userRepo, LoggedUser loggedUser)
    {
        _userRepo = userRepo;
        LoggedUser = loggedUser;
    }

    public bool SuccessOnPost { get; set; } = true;
    public string MsgInformation { get; set; } = string.Empty;
    public string MsgPassword { get; set; } = string.Empty;
    [BindProperty]
    public UserModel UserData { get; set; } = new UserModel();

    public void OnGet()
    {
        var currentUserInformation = _userRepo.GetUser(LoggedUser.Id);
        if (currentUserInformation == null)
            return;
        UserData.Login = currentUserInformation.Login;
        UserData.Email = currentUserInformation.Email;
        UserData.Name = currentUserInformation.Name;
    }

    public void OnPostUserInformation()
    {
        try
        {
            _userRepo.UpdateUser(new FoxUser()
            {
                Id = LoggedUser.Id,
                Login = UserData.Login,
                Name = UserData.Name,
                Email = UserData.Email
            });
            SuccessOnPost = true;
            MsgInformation = "Information updated!";
        }
        catch(Exception argEx)
        {
            MsgInformation = argEx.Message;
            SuccessOnPost = false;
        }
    }

    public void OnPostChangePassword()
    {
        try
        {
            if(UserData.Password != UserData.PasswordConfirmation)
            {
                throw new ArgumentException("The password confirmation do not match");
            }
            _userRepo.UpdatePassword(LoggedUser.Id, UserData.Password);
            MsgPassword = "Password changed!";
            SuccessOnPost = true;
        }
        catch (Exception argEx)
        {
            MsgPassword = argEx.Message;
            SuccessOnPost = false;
        }
    }
}

public class UserModel
{
    public string Login { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordConfirmation { get; set; } = string.Empty;
}
