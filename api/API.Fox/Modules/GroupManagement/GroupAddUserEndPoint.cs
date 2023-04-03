using System;
using API.Fox.EndPoint;
using API.Fox.Modules.UserManagement;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.GroupManagement;

public class GroupAddUserEndPoint : IEndPoint
{
    public string PermissionClaim => "GROUP_MANAGEMENT_ADDUSER";
    public string UrlPattern => "/management/group/user";
    public EndPointVerb Verb => EndPointVerb.POST;
    public Delegate Method => (GroupAdditionData groupAddition, GroupRepository groupRepository) =>
    {
        try
        {
            groupRepository.AddUserToGroup(groupAddition.GroupId, groupAddition.UserIds);
            return Results.Ok();
        }
        catch (ArgumentException argumentNull)
        {
            return Results.Problem(title: argumentNull.Message, statusCode: 400);
        }
        catch (Exception)
        {
            return Results.Problem();
        }
    };
}

record GroupAdditionData(Guid GroupId, Guid[] UserIds);