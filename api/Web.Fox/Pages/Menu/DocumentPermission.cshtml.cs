using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fox.Access.Service;
using FoxUser = Fox.Access.Model.User;
using FoxGroup = Fox.Access.Model.Group;
using Fox.Dox.Service;
using Fox.Dox.Model;
using Microsoft.AspNetCore.Authorization;
using Fox.Access.Model;

namespace Web.Fox.Pages.Menu;

[Authorize(Policy = "DOCUMENT_PERMISSION_READ")]
public class DocumentPermissionModel : PageModel
{
    [BindProperty]
    public Guid DocumentId { get; set; }
    [BindProperty]
    public string DocumentName { get; set; } = string.Empty;
    [BindProperty]
    public string HolderType { get; set; } = string.Empty;

    [FromQuery(Name = "documentId")]
    public Guid DocumentIdQuery { get; set; }
    [FromQuery(Name = "documentName")]
    public string DocumentNameQuery { get; set; } = string.Empty;
    [FromQuery(Name = "holderType")]
    public string HolderTypeQuery { get; set; } = string.Empty;

    public string Msg { get; set; } = string.Empty;
    public IEnumerable<DocumentHolder> PermissionsGiven { get; set; } = Array.Empty<DocumentHolder>();
    public IEnumerable<DocumentHolder> PermissionsDenied { get; set; } = Array.Empty<DocumentHolder>();

    private DocumentService _docService;
    private GroupService _groupService;
    private UserService _userService;
    public DocumentPermissionModel(DocumentService docService, GroupService groupService, UserService userService)
    {
        _docService = docService;
        _groupService = groupService;
        _userService = userService;
    }

    private void FillPermissionsList()
    {
        PermissionsGiven = _docService.GetPermissionByDocument(DocumentId)
                                   .Where(p => p.HolderType.ToLower() == HolderType.ToLower())
                                   .OrderBy(p => p.Permission)
                                   .ThenBy(p => p.HolderName);
        if (HolderType.ToLower() == "user")
        {
            PermissionsDenied = _userService.GetAllUsers()
                                         .Select(user => new DocumentHolder()
                                         {
                                             HolderId = user.Id,
                                             HolderLogin = user.Login,
                                             HolderName = user.Name,
                                             HolderType = "user"
                                         });
        }
        else
        {
            PermissionsDenied = _groupService.GetAllGroups()
                                          .Select(group => new DocumentHolder()
                                          {
                                              HolderId = group.Id,
                                              HolderName = group.Name,
                                              HolderType = "group"
                                          });
        }
        PermissionsDenied = PermissionsDenied.Where(p => !PermissionsGiven.Any(g => g.HolderId == p.HolderId))
                                             .OrderBy(p => p.HolderName);
    }

    public void OnGet()
    {
        DocumentId = DocumentIdQuery;
        DocumentName = DocumentNameQuery;
        HolderType = HolderTypeQuery;
        FillPermissionsList();
    }

    public void OnPostAddPermission(Guid holderId, DocumentPermission permission)
    {
        try
        {
            HttpContext.HasPermission("DOCUMENT_PERMISSION_ADDITION");
            _docService.AddPermission(DocumentId, holderId, permission);
        }
        catch(Exception argEx)
        {
            Msg = argEx.Message;
        }
        FillPermissionsList();
    }

    public void OnPostDelPermission(Guid holderId, DocumentPermission permission)
    {
        try
        {
            HttpContext.HasPermission("DOCUMENT_PERMISSION_REMOVAL");
            _docService.DelPermission(DocumentId, holderId, permission);
        }
        catch (Exception argEx)
        {
            Msg = argEx.Message;
        }
        FillPermissionsList();
    }
}
