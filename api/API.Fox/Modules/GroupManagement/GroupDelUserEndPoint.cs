using System;
using API.Fox.EndPoint;
using API.Fox.Modules.UserManagement;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.GroupManagement;

public class GroupDelUserEndPoint : IEndPoint
{
    public string PermissionClaim => "GROUP_MANAGEMENT_DELUSER";
    public string UrlPattern => "/management/group/user";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (GroupDeletionData groupDeletion, GroupRepository groupRepository) =>
    {
        try
        {
            groupRepository.DelUserFromGroup(groupDeletion.GroupId, groupDeletion.UserIds);
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


record GroupDeletionData(Guid GroupId, Guid[] UserIds);