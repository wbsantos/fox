﻿using System;
using Fox.Access.Repository;
using API.Fox.EndPoint;

namespace API.Fox.EndPoint.PermissionManagement;

public class PermissionDel : IEndPoint
{
    public string PermissionClaim => "SYSTEM_PERMISSION_REMOVAL";
    public string UrlPattern => "/management/systempermission";
    public EndPointVerb Verb => EndPointVerb.DELETE;
    public Delegate Method => (Guid holderId, string permission, PermissionRepository permissionRepo) =>
    {
        permissionRepo.DeletePermission(holderId, permission);
        return Results.Ok();
    };
}
