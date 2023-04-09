using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fox.Access.Repository;
using Fox.Access.Model;

namespace Web.Fox.Pages.Menu;

public class GroupManagementModel : PageModel, INavBarItem
{
    public string MenuDescription => "Groups";
    public string PagePath => "/Menu/GroupManagement";

    public string Msg { get; set; } = string.Empty;
    private IEnumerable<Group> _groups = Array.Empty<Group>();
    public IEnumerable<Group> Groups
    {
        get => _groups.OrderBy(g => g.Id);
        set => _groups = value;
    }

    [BindProperty]
    public Guid? AlterModeFor { get; set; }
    [BindProperty]
    public string AlterModeNewGroupName { get; set; } = string.Empty;
    
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
            _groupRepo.DeleteGroup(groupId);
        }
        catch (ArgumentException argEx)
        {
            Msg = argEx.Message;
        }
        Groups = _groupRepo.GetAllGroups();
    }

    public void OnPostUpdateGroup(Guid groupId)
    {
        try
        {
            _groupRepo.UpdateGroup(new Group()
            {
                Id = groupId,
                Name = AlterModeNewGroupName
            });
            AlterModeFor = null;
        }
        catch(Exception argEx)
        {
            Msg = argEx.Message;
        }
        Groups = _groupRepo.GetAllGroups();
    }

    public void OnPostCancelUpdate()
    {
        AlterModeFor = null;
        Groups = _groupRepo.GetAllGroups();
    }
}
