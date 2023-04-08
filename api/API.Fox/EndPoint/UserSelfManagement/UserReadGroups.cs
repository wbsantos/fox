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
        try
        {
            if (userId != loggedUser.Id)
                return Results.Unauthorized();

            IEnumerable<Group> groups = userRepository.GetUserGroups(userId);
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