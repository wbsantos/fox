using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.UserSelfManagement;

public class UserReadGroups : IEndPoint
{
    public string PermissionClaim => "USER_SELF_MANAGEMENT";
    public string UrlPattern => "/selfmanagement/user/group";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid userId, LoggedUser loggedUser, UserService userService) =>
    {
        if (userId != loggedUser.Id)
            return Results.Unauthorized();

        IEnumerable<Group> groups = userService.GetUserGroups(userId);
        return Results.Ok(new { groups = groups });
    };
}