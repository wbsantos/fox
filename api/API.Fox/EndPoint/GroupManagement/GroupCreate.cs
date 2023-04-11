using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupCreate : IEndPoint
{
    public string PermissionClaim => "GROUP_CREATION_MANAGEMENT";
    public string UrlPattern => "/management/group";
    public EndPointVerb Verb => EndPointVerb.POST;
    public Delegate Method => (Group group, GroupRepository groupRepository) =>
    {
        var groupCreated = groupRepository.CreateGroup(group);
        return Results.Ok(groupCreated);
    };
}

