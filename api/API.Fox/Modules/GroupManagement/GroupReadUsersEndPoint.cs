using System;
using API.Fox.EndPoint;
using API.Fox.Modules.UserManagement;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.GroupManagement;

public class GroupReadUserEndPoint : IEndPoint
{
    public string PermissionClaim => "GROUP_MANAGEMENT_READ_USER";
    public string UrlPattern => "/management/group/user";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid groupId, GroupRepository groupRepository) =>
    {
        try
        {
            IEnumerable<User> users = groupRepository.GetUsersFromGroup(groupId);
            if (users.Count() == 0)
                return Results.NotFound();
            else
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