using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.UserSelfManagement;

public class UserReadGroups : IEndPoint
{
    public string PermissionClaim => "USER_SELF_MANAGEMENT";
    public string UrlPattern => "/selfmanagement/user/group";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid userId, LoggedUser loggedUser, UserRepository userRepository) =>
    {
        if (userId != loggedUser.Id)
            return Results.Unauthorized();

        IEnumerable<Group> groups = userRepository.GetUserGroups(userId);
        return Results.Ok(new { groups = groups });
    };
}