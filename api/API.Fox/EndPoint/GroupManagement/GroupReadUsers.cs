using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupReadUser : IEndPoint
{
    public string PermissionClaim => "GROUP_MANAGEMENT_READ_USER";
    public string UrlPattern => "/management/group/user";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid groupId, GroupService groupService) =>
    {
        IEnumerable<User> users = groupService.GetUsersFromGroup(groupId);
        return Results.Ok(new { users = users } );
    };
}