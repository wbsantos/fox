using System;
using Fox.Access.Repository;
using API.Fox.EndPoint;

namespace API.Fox.EndPoint.PermissionManagement;

public class PermissionRead : IEndPoint
{
    public string PermissionClaim => "SYSTEM_PERMISSION_READ";
    public string UrlPattern => "/management/systempermission";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid holderId, PermissionRepository permissionRepo) =>
    {
        IEnumerable<string> permissions = permissionRepo.GetPermissions(holderId);
        return Results.Ok(new { permissions = permissions });
    };
}