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
        groupRepository.DeleteGroup(groupId);
        return Results.Ok();
    };
}

