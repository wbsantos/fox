using System;
using Fox.Access.Service;
using API.Fox.EndPoint;
using System.ComponentModel.DataAnnotations;

namespace API.Fox.EndPoint.PermissionManagement;

public class PermissionAdd : IEndPoint
{
    public string PermissionClaim => "SYSTEM_PERMISSION_ADDITION";
    public string UrlPattern => "/management/systempermission";
    public EndPointVerb Verb => EndPointVerb.POST;
    public Delegate Method => (PermissionAddData data, PermissionService permissionService) =>
    {
        permissionService.AddPermission(data.PermissionHolderId, data.Permission);
        return Results.Ok();
    };
}

public record PermissionAddData([Required] Guid PermissionHolderId, [Required] string Permission);