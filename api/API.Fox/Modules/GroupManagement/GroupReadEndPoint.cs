using System;
using API.Fox.EndPoint;
using API.Fox.Modules.UserManagement;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.GroupManagement;

public class GroupReadEndPoint : IEndPoint
{
    public string PermissionClaim => "GROUP_READ_MANAGEMENT";
    public string UrlPattern => "/management/group";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid groupId, GroupRepository groupRepository) =>
    {
        try
        {
            Group? group = groupRepository.GetGroup(groupId);
            return Results.Ok(group);
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

