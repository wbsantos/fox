﻿using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.GroupManagement;

public class GroupUpdate : IEndPoint
{
    public string PermissionClaim => "GROUP_UPDATE_MANAGEMENT";
    public string UrlPattern => "/management/group";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (Group group, GroupRepository groupRepository) =>
    {
        try
        {
            groupRepository.UpdateGroup(group);
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
