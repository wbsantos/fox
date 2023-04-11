using System;
using Fox.Access.Repository;
using API.Fox.EndPoint;

namespace API.Fox.EndPoint.PermissionManagement;

public class PermissionAdd : IEndPoint
{
    public string PermissionClaim => "SYSTEM_PERMISSION_ADDITION";
    public string UrlPattern => "/management/systempermission";
    public EndPointVerb Verb => EndPointVerb.POST;
    public Delegate Method => (PermissionAddData data, PermissionRepository permissionRepo) =>
    {
        permissionRepo.AddPermission(data.PermissionHolderId, data.Permission);
        return Results.Ok();
    };
}

public record PermissionAddData(Guid PermissionHolderId, string Permission);