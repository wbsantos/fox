using System;
using API.Fox.EndPoint;
using API.Fox.Modules.UserManagement;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.GroupManagement;

public class GroupReadAllEndPoint : IEndPoint
{
    public string PermissionClaim => "GROUP_READ_ALL_MANAGEMENT";
    public string UrlPattern => "/management/group/all";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (GroupRepository groupRepository) =>
    {
        try
        {
            IEnumerable<Group> groups = groupRepository.GetAllGroups();
            return Results.Ok(new { groups = groups });
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

