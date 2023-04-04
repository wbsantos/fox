using System;
using Fox.Access.Repository;
using API.Fox.EndPoint;

namespace API.Fox.Modules.PermissionManagement;

public class PermissionReadEndPoint : IEndPoint
{
    public string PermissionClaim => "SYSTEM_PERMISSION_READ";
    public string UrlPattern => "/management/systempermission";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid holderId, PermissionRepository permissionRepo) =>
    {
        try
        {
            IEnumerable<string> permissions = permissionRepo.GetPermissions(holderId);
            return Results.Ok(new { permissions = permissions });
        }
        catch (ArgumentException argumentNull)
        {
            return Results.Problem(title: argumentNull.Message, statusCode: 400);
        }
        catch (Exception)
        {
            return Results.Problem();
        }
    };
}