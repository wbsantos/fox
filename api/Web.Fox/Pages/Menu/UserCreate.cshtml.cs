using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fox.Access.Repository;
using Fox.Access.Model;
using FoxUser = Fox.Access.Model.User;

namespace Web.Fox.Pages.Menu;

public class UserCreateModel : PageModel
{
    UserRepository _userRepo;
    public UserCreateModel(UserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public bool SuccessOnPost { get; set; } = true;
    public string MsgInformation { get; set; } = string.Empty;
    public string MsgPassword { get; set; } = string.Empty;
    [BindProperty]
    public UserModel UserData { get; set; } = new UserModel();

    public void OnGet()
    {
        
    }

    public IActionResult OnPostUserInformation()
    {
        try
        {
            if (UserData.Password != UserData.PasswordConfirmation)
                throw new ArgumentException("The password confirmation do not match");
            _userRepo.CreateUser(new FoxUser()
            {
                Login = UserData.Login,
                Name = UserData.Name,
                Email = UserData.Email
            }, UserData.Password);
            SuccessOnPost = true;
            MsgInformation = "Information updated!";

            return RedirectToPage("UserManagement");
        }
        catch(Exception argEx)
        {
            MsgInformation = argEx.Message;
            SuccessOnPost = false;
        }
        return Page();
    }
}