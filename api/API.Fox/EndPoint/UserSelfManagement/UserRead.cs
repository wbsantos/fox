using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.UserSelfManagement;

public class UserRead : IEndPoint
{
    public string PermissionClaim => "USER_SELF_MANAGEMENT";
    public string UrlPattern => "/selfmanagement/user";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid userId, LoggedUser loggedUser, UserRepository userRepo) =>
    {
        if (userId != loggedUser.Id)
            return Results.Unauthorized();

        User? userData = userRepo.GetUser(userId);
        return Results.Ok(userData);
    };
}


