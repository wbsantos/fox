using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupDelete : IEndPoint
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

