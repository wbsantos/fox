using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.UserManagement;

public class UserDelete : IEndPoint
{
    public string PermissionClaim => "USER_DELETION_MANAGEMENT";
    public string UrlPattern => "/management/user";
    public EndPointVerb Verb => EndPointVerb.DELETE;
    public Delegate Method => (Guid userId, UserRepository userRepo) =>
    {
        userRepo.DeleteUser(userId);
        return Results.Ok();
    };
}