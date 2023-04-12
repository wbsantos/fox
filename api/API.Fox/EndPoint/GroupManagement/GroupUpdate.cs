using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupUpdate : IEndPoint
{
    public string PermissionClaim => "GROUP_UPDATE_MANAGEMENT";
    public string UrlPattern => "/management/group";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (Group group, GroupService groupService) =>
    {
        groupService.UpdateGroup(group);
        return Results.Ok();        
    };
}

