using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupRead : IEndPoint
{
    public string PermissionClaim => "GROUP_READ_MANAGEMENT";
    public string UrlPattern => "/management/group";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid groupId, GroupRepository groupRepository) =>
    {
        Group? group = groupRepository.GetGroup(groupId);
        return Results.Ok(group);
    };
}

