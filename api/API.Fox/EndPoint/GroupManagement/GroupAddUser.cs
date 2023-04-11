using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupAddUser : IEndPoint
{
    public string PermissionClaim => "GROUP_MANAGEMENT_ADDUSER";
    public string UrlPattern => "/management/group/user";
    public EndPointVerb Verb => EndPointVerb.POST;
    public Delegate Method => (GroupAdditionData groupAddition, GroupRepository groupRepository) =>
    {
        groupRepository.AddUserToGroup(groupAddition.GroupId, groupAddition.UserIds);
        return Results.Ok();
    };
}

record GroupAdditionData(Guid GroupId, Guid[] UserIds);