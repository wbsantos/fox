using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.UserSelfManagement;

public class UserUpdate : IEndPoint
{
    public string PermissionClaim => "USER_SELF_MANAGEMENT";
    public string UrlPattern => "/selfmanagement/user";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (User user, LoggedUser loggedUser, UserService userService) =>
    {
        if (user.Id != loggedUser.Id)
            return Results.Unauthorized();

        userService.UpdateUser(user);
        return Results.Ok();
    };
}

