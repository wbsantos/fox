using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fox.Access.Repository;
using Fox.Access.Model;
using Microsoft.AspNetCore.Authorization;

namespace Web.Fox.Pages.Menu;

[Authorize(Policy = "GROUP_READ_ALL_MANAGEMENT")]
public class GroupManagementModel : PageModel, INavBarItem
{
    public string MenuDescription => "Groups";
    public string PagePath => "/Menu/GroupManagement";
    public string MenuCategory => "MANAGEMENT";

    public bool SuccessOnPost { get; set; } = true;
    public string Msg { get; set; } = string.Empty;
    private IEnumerable<Group> _groups = Array.Empty<Group>();
    public IEnumerable<Group> Groups
    {
        get => _groups.OrderBy(g => g.Name);
        set => _groups = value;
    }

    [BindProperty]
    public Guid? AlterModeFor { get; set; }
    [BindProperty]
    public string AlterModeNewGroupName { get; set; } = string.Empty;
    [BindProperty]
    public string CreateNewGroupName { get; set; } = string.Empty;

    private GroupRepository _groupRepo;
    public GroupManagementModel(GroupRepository groupRepo)
    {
        _groupRepo = groupRepo;
    }

    public void OnGet()
    {
        Groups = _groupRepo.GetAllGroups();
    }

    public void OnPostEnterUpdateMode(Guid groupId)
    {
        Groups = _groupRepo.GetAllGroups();
        AlterModeFor = groupId;
    }

    public void OnPostDeleteGroup(Guid groupId)
    {
        try
        {
            HttpContext.HasPermission("GROUP_DELETION_MANAGEMENT");
            _groupRepo.DeleteGroup(groupId);
            SuccessOnPost = true;
        }
        catch (ArgumentException argEx)
        {
            Msg = argEx.Message;
            SuccessOnPost = false;
        }
        Groups = _groupRepo.GetAllGroups();
    }

    public void OnPostUpdateGroup(Guid groupId)
    {
        try
        {
            HttpContext.HasPermission("GROUP_UPDATE_MANAGEMENT");
            _groupRepo.UpdateGroup(new Group()
            {
                Id = groupId,
                Name = AlterModeNewGroupName
            });
            AlterModeFor = null;
            SuccessOnPost = true;
        }
        catch(Exception argEx)
        {
            Msg = argEx.Message;
            SuccessOnPost = false;
        }
        Groups = _groupRepo.GetAllGroups();
    }

    public IActionResult OnPostCreateGroup()
    {
        try
        {
            HttpContext.HasPermission("GROUP_CREATION_MANAGEMENT");
            _groupRepo.CreateGroup(new Group()
            {
                Name = CreateNewGroupName
            });
            Msg = "Group created!";
            CreateNewGroupName = string.Empty;
            SuccessOnPost = true;
        }
        catch (Exception argEx)
        {
            Msg = argEx.Message;
            SuccessOnPost = false;
        }
        Groups = _groupRepo.GetAllGroups();
        return Page();
    }

    public void OnPostCancelUpdate()
    {
        AlterModeFor = null;
        Groups = _groupRepo.GetAllGroups();
    }
}
