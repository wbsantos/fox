using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.UserManagement;

public class UserRead : IEndPoint
{
    public string PermissionClaim => "USER_READ_MANAGEMENT";
    public string UrlPattern => "/management/user";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid userId, UserRepository userRepo) =>
    {
        User? userData = userRepo.GetUser(userId);
        return Results.Ok(userData);
    };
}


