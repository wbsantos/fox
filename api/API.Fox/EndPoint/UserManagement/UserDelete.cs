using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.UserManagement;

public class UserDelete : IEndPoint
{
    public string PermissionClaim => "USER_DELETION_MANAGEMENT";
    public string UrlPattern => "/management/user";
    public EndPointVerb Verb => EndPointVerb.DELETE;
    public Delegate Method => (Guid userId, UserService userService) =>
    {
        userService.DeleteUser(userId);
        return Results.Ok();
    };
}