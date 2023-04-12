using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupRead : IEndPoint
{
    public string PermissionClaim => "GROUP_READ_MANAGEMENT";
    public string UrlPattern => "/management/group";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid groupId, GroupService groupService) =>
    {
        Group? group = groupService.GetGroup(groupId);
        return Results.Ok(group);
    };
}

