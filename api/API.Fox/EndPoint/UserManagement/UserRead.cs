using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.UserManagement;

public class UserRead : IEndPoint
{
    public string PermissionClaim => "USER_READ_MANAGEMENT";
    public string UrlPattern => "/management/user";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid userId, UserService userService) =>
    {
        User? userData = userService.GetUser(userId);
        return Results.Ok(userData);
    };
}


