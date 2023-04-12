using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupReadAll : IEndPoint
{
    public string PermissionClaim => "GROUP_READ_ALL_MANAGEMENT";
    public string UrlPattern => "/management/group/all";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (GroupService groupService) =>
    {
        IEnumerable<Group> groups = groupService.GetAllGroups();
        return Results.Ok(new { groups = groups });
    };
}

