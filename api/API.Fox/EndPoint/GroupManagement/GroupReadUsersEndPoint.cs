using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupReadUser : IEndPoint
{
    public string PermissionClaim => "GROUP_MANAGEMENT_READ_USER";
    public string UrlPattern => "/management/group/user";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid groupId, GroupRepository groupRepository) =>
    {
        try
        {
            IEnumerable<User> users = groupRepository.GetUsersFromGroup(groupId);
            return Results.Ok(new { users = users } );
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