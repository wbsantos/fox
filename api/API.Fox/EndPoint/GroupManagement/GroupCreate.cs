using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupCreate : IEndPoint
{
    public string PermissionClaim => "GROUP_CREATION_MANAGEMENT";
    public string UrlPattern => "/management/group";
    public EndPointVerb Verb => EndPointVerb.POST;
    public Delegate Method => (Group group, GroupService groupService) =>
    {
        var groupCreated = groupService.CreateGroup(group);
        return Results.Ok(groupCreated);
    };
}

