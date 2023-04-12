using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupDelUser : IEndPoint
{
    public string PermissionClaim => "GROUP_MANAGEMENT_DELUSER";
    public string UrlPattern => "/management/group/user";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (GroupDeletionData groupDeletion, GroupService groupService) =>
    {
        groupService.DelUserFromGroup(groupDeletion.GroupId, groupDeletion.UserIds);
        return Results.Ok();
    };
}


record GroupDeletionData(Guid GroupId, Guid[] UserIds);