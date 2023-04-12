using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.UserSelfManagement;

public class UserUpdatePassword : IEndPoint
{
    public string PermissionClaim => "USER_SELF_MANAGEMENT";
    public string UrlPattern => "/selfmanagement/user/password";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (UserSelfUpdatePasswordData user, LoggedUser loggedUser, UserService userService) =>
    {
        if (user.Id != loggedUser.Id)
            return Results.Unauthorized();

        userService.UpdatePassword(user.Id, user.Password);
        return Results.Ok();
    };
}

record UserSelfUpdatePasswordData(Guid Id, string Password);
