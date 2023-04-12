using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.UserManagement;

public class UserUpdatePassword : IEndPoint
{
    public string PermissionClaim => "USER_UPDATE_MANAGEMENT";
    public string UrlPattern => "/management/user/password";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (UserUpdatePasswordData user, UserService userService) =>
    {
        userService.UpdatePassword(user.Id, user.Password);
        return Results.Ok();
    };
}

record UserUpdatePasswordData(Guid Id, string Password);
