using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.UserManagement;

public class UserReadGroups : IEndPoint
{
    public string PermissionClaim => "USER_MANAGEMENT_READ_GROUP";
    public string UrlPattern => "/management/user/group";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid userId, UserRepository userRepository) =>
    {
        try
        {
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