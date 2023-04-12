using System;
using Fox.Access.Service;
using API.Fox.EndPoint;

namespace API.Fox.EndPoint.PermissionManagement;

public class PermissionRead : IEndPoint
{
    public string PermissionClaim => "SYSTEM_PERMISSION_READ";
    public string UrlPattern => "/management/systempermission";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid holderId, PermissionService permissionService) =>
    {
        IEnumerable<string> permissions = permissionService.GetPermissions(holderId);
        return Results.Ok(new { permissions = permissions });
    };
}