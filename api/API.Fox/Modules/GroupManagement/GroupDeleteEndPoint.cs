using System;
using API.Fox.EndPoint;
using API.Fox.Modules.UserManagement;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.GroupManagement;

public class GroupDeleteEndPoint : IEndPoint
{
    public string PermissionClaim => "GROUP_DELETION_MANAGEMENT";
    public string UrlPattern => "/management/group";
    public EndPointVerb Verb => EndPointVerb.DELETE;
    public Delegate Method => (Guid groupId, GroupRepository groupRepository) =>
    {
        try
        {
            groupRepository.DeleteGroup(groupId);
            return Results.Ok();
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

