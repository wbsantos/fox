using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fox.Access.Model;
using Fox.Access.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoxUser = Fox.Access.Model.User;

namespace Web.Fox.Pages.Menu;

public class UserManagementModel : PageModel, INavBarItem
{
    public string MenuDescription => "Users";
    public string PagePath => "/Menu/UserManagement";
    public string MenuCategory => "MANAGEMENT";

    public string Msg { get; set; } = string.Empty;
    private IEnumerable<FoxUser> _users = Array.Empty<FoxUser>();
    public IEnumerable<FoxUser> Users
    {
        get => _users.OrderBy(g => g.Login);
        set => _users = value;
    }

    public IEnumerable<Group> UserGroups { get; set; } = Array.Empty<Group>();

    [BindProperty]
    public Guid? GroupsFor { get; set; }
    [BindProperty]
    public Guid? AlterModeFor { get; set; }
    [BindProperty]
    public string AlterModeNewLogin { get; set; } = string.Empty;
    [BindProperty]
    public string AlterModeNewUserName { get; set; } = string.Empty;
    [BindProperty]
    public string AlterModeNewEmail { get; set; } = string.Empty;
    [BindProperty]
    public string AlterModeNewPassword { get; set; } = string.Empty;

    private UserRepository _userRepo;
    public UserManagementModel(UserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public void OnGet()
    {
        Users = _userRepo.GetAllUsers();
    }

    public void OnPostEnterUpdateMode(Guid userId)
    {
        Users = _userRepo.GetAllUsers();
        AlterModeFor = userId;
    }

    public void OnPostDeleteUser(Guid userId)
    {
        try
        {
            _userRepo.DeleteUser(userId);
        }
        catch (ArgumentException argEx)
        {
            Msg = argEx.Message;
        }
        Users = _userRepo.GetAllUsers();
    }

    public void OnPostUpdateUser(Guid userId)
    {
        try
        {
            _userRepo.UpdateUser(new FoxUser()
            {
                Id = userId,
                Login = AlterModeNewLogin,
                Name = AlterModeNewUserName,
                Email = AlterModeNewEmail
            });

            if (!string.IsNullOrEmpty(AlterModeNewPassword))
                _userRepo.UpdatePassword(userId, AlterModeNewPassword);
            AlterModeFor = null;
        }
        catch (Exception argEx)
        {
            Msg = argEx.Message;
        }
        Users = _userRepo.GetAllUsers();
    }

    public void OnPostUserGroups(Guid userId)
    {
        try
        {
            UserGroups = _userRepo.GetUserGroups(userId);
            GroupsFor = userId;
        }
        catch (Exception argEx)
        {
            Msg = argEx.Message;
        }
        Users = _userRepo.GetAllUsers();
    }

    public void OnPostCancelUpdate()
    {
        AlterModeFor = null;
        Users = _userRepo.GetAllUsers();
    }
}
