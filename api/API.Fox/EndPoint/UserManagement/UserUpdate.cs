using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.UserManagement;

public class UserUpdate : IEndPoint
{
    public string PermissionClaim => "USER_UPDATE_MANAGEMENT";
    public string UrlPattern => "/management/user";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (User user, UserRepository userRepo) =>
    {
        userRepo.UpdateUser(user);
        return Results.Ok();
    };
}

