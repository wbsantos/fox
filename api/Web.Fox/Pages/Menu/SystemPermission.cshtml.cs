using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fox.Access.Service;
using Fox.Access.Model;
using FoxUser = Fox.Access.Model.User;
using Microsoft.AspNetCore.Authorization;

namespace Web.Fox.Pages.Menu;

[Authorize(Policy = "SYSTEM_PERMISSION_READ")]
public class SystemPermissionModel : PageModel
{
    [BindProperty]
    public Guid HolderId { get; set; }
    [BindProperty]
    public string HolderName { get; set; } = string.Empty;
    [BindProperty]
    public string HolderType { get; set; } = string.Empty;

    [FromQuery(Name = "holderId")]
    public Guid HolderIdQuery { get; set; }
    [FromQuery(Name = "holderName")]
    public string HolderNameQuery { get; set; } = string.Empty;
    [FromQuery(Name = "holderType")]
    public string HolderTypeQuery { get; set; } = string.Empty;

    public string Msg { get; set; } = string.Empty;
    public IEnumerable<string> PermissionsGiven { get; set; } = Array.Empty<string>();
    public IEnumerable<string> PermissionsDenied { get; set; } = Array.Empty<string>();

    private PermissionService _permissionService;
    public SystemPermissionModel(PermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    private void FillPermissionsList()
    {
        PermissionsGiven = _permissionService.GetPermissions(HolderId).OrderBy(p => p);
        PermissionsDenied = Web.Fox.AppBuilder.Auth.GetEndPointsPolicies()
                                                   .Where(pol => !PermissionsGiven.Any(pg => pg == pol))
                                                   .OrderBy(p => p);
    }

    public void OnGet()
    {
        HolderId = HolderIdQuery;
        HolderName = HolderNameQuery;
        HolderType = HolderTypeQuery;
        FillPermissionsList();
    }

    public void OnPostAddPermission(string permission)
    {
        try
        {
            HttpContext.HasPermission("SYSTEM_PERMISSION_ADDITION");
            _permissionService.AddPermission(HolderId, permission);
        }
        catch(Exception argEx)
        {
            Msg = argEx.Message;
        }
        FillPermissionsList();
    }

    public void OnPostDelPermission(string permission)
    {
        try
        {
            HttpContext.HasPermission("SYSTEM_PERMISSION_REMOVAL");
            _permissionService.DeletePermission(HolderId, permission);
        }
        catch (Exception argEx)
        {
            Msg = argEx.Message;
        }
        FillPermissionsList();
    }
}
