using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.UserManagement;

public class UserReadAll : IEndPoint
{
    public string PermissionClaim => "USER_READ_ALL_MANAGEMENT";
    public string UrlPattern => "/management/user/all";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (UserRepository userRepo) =>
    {
        IEnumerable<User> userData = userRepo.GetAllUsers();
        return Results.Ok(new { users = userData });
    };
}


