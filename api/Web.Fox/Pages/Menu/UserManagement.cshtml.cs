using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fox.Access.Model;
using Fox.Access.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FoxUser = Fox.Access.Model.User;

namespace Web.Fox.Pages.Menu;

[Authorize(Policy = "USER_READ_ALL_MANAGEMENT")]
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

    private UserService _userService;
    public UserManagementModel(UserService userService)
    {
        _userService = userService;
    }

    public void OnGet()
    {
        Users = _userService.GetAllUsers();
    }

    public void OnPostEnterUpdateMode(Guid userId)
    {
        Users = _userService.GetAllUsers();
        AlterModeFor = userId;
    }

    public void OnPostDeleteUser(Guid userId)
    {
        try
        {
            HttpContext.HasPermission("USER_DELETION_MANAGEMENT");
            _userService.DeleteUser(userId);
        }
        catch (ArgumentException argEx)
        {
            Msg = argEx.Message;
        }
        Users = _userService.GetAllUsers();
    }

    public void OnPostUpdateUser(Guid userId)
    {
        try
        {
            HttpContext.HasPermission("USER_UPDATE_MANAGEMENT");
            _userService.UpdateUser(new FoxUser()
            {
                Id = userId,
                Login = AlterModeNewLogin,
                Name = AlterModeNewUserName,
                Email = AlterModeNewEmail
            });

            if (!string.IsNullOrEmpty(AlterModeNewPassword))
                _userService.UpdatePassword(userId, AlterModeNewPassword);
            AlterModeFor = null;
        }
        catch (Exception argEx)
        {
            Msg = argEx.Message;
        }
        Users = _userService.GetAllUsers();
    }

    public void OnPostUserGroups(Guid userId)
    {
        try
        {
            HttpContext.HasPermission("USER_MANAGEMENT_READ_GROUP");
            UserGroups = _userService.GetUserGroups(userId);
            GroupsFor = userId;
        }
        catch (Exception argEx)
        {
            Msg = argEx.Message;
        }
        Users = _userService.GetAllUsers();
    }

    public void OnPostCancelUpdate()
    {
        AlterModeFor = null;
        Users = _userService.GetAllUsers();
    }
}
