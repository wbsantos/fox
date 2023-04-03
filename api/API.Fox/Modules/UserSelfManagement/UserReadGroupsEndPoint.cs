using System;
using API.Fox.EndPoint;
using API.Fox.Modules.UserManagement;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.UserSelfManagement;

public class UserReadGroupsEndPoint : IEndPoint
{
    public string PermissionClaim => "USER_SELF_MANAGEMENT";
    public string UrlPattern => "/selfmanagement/user/group";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid userId, LoggedUser loggedUser, UserRepository userRepository) =>
    {
        try
        {
            if (userId != loggedUser.Id)
                return Results.Forbid();

            IEnumerable<Group> groups = userRepository.GetUserGroups(userId);
            if (groups.Count() == 0)
                return Results.NotFound();
            else
                return Results.Ok(new { groups = groups });
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