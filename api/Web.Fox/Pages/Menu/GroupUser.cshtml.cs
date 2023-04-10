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

[Authorize(Policy = "GROUP_MANAGEMENT_READ_USER")]
public class GroupUserModel : PageModel
{
    [BindProperty]
    public Guid GroupId { get; set; }
    [BindProperty]
    public string GroupName { get; set; } = string.Empty;

    [FromQuery(Name = "groupId")]
    public Guid GroupIdQuery { get; set; }
    [FromQuery(Name = "groupName")]
    public string GroupNameQuery { get; set; } = string.Empty;

    public string Msg { get; set; } = string.Empty;
    public IEnumerable<FoxUser> UsersInGroup { get; set; } = Array.Empty<FoxUser>();
    public IEnumerable<FoxUser> UsersNotInGroup { get; set; } = Array.Empty<FoxUser>();

    private UserRepository _userRepo;
    private GroupRepository _groupRepo;
    public GroupUserModel(GroupRepository groupRepo, UserRepository userRepo)
    {
        _groupRepo = groupRepo;
        _userRepo = userRepo;
    }

    private void FillUsersList()
    {
        UsersInGroup = _groupRepo.GetUsersFromGroup(GroupId).OrderBy(u => u.Name);
        UsersNotInGroup = _userRepo.GetAllUsers()
                                   .Where(u => !UsersInGroup.Any(ug => ug.Id == u.Id))
                                   .OrderBy(u => u.Name);
    }

    public void OnGet()
    {
        GroupId = GroupIdQuery;
        GroupName = GroupNameQuery;
        FillUsersList();
    }

    public void OnPostAddToGroup(Guid userId)
    {
        try
        {
            HttpContext.HasPermission("GROUP_MANAGEMENT_ADDUSER");
            _groupRepo.AddUserToGroup(GroupId, new Guid[] { userId });
        }
        catch(Exception argEx)
        {
            Msg = argEx.Message;
        }
        FillUsersList();
    }

    public void OnPostDelFromGroup(Guid userId)
    {
        try
        {
            HttpContext.HasPermission("GROUP_MANAGEMENT_DELUSER");
            _groupRepo.DelUserFromGroup(GroupId, new Guid[] { userId });
        }
        catch (Exception argEx)
        {
            Msg = argEx.Message;
        }
        FillUsersList();
    }
}
