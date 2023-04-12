using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupDelete : IEndPoint
{
    public string PermissionClaim => "GROUP_DELETION_MANAGEMENT";
    public string UrlPattern => "/management/group";
    public EndPointVerb Verb => EndPointVerb.DELETE;
    public Delegate Method => (Guid groupId, GroupService groupService) =>
    {
        groupService.DeleteGroup(groupId);
        return Results.Ok();
    };
}

