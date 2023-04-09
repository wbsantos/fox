﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Fox.Access.Repository;
using Fox.Access.Model;
using FoxUser = Fox.Access.Model.User;

namespace Web.Fox.Pages.Menu;

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

    private PermissionRepository _permissionRepo;
    public SystemPermissionModel(PermissionRepository permissionRepo)
    {
        _permissionRepo = permissionRepo;
    }

    private void FillPermissionsList()
    {
        PermissionsGiven = _permissionRepo.GetPermissions(HolderId).OrderBy(p => p);
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
            _permissionRepo.AddPermission(HolderId, permission);
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
            _permissionRepo.DeletePermission(HolderId, permission);
        }
        catch (Exception argEx)
        {
            Msg = argEx.Message;
        }
        FillPermissionsList();
    }
}