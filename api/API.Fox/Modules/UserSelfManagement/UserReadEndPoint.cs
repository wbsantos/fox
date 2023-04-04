using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.UserSelfManagement;

public class UserReadEndPoint : IEndPoint
{
    public string PermissionClaim => "USER_SELF_MANAGEMENT";
    public string UrlPattern => "/selfmanagement/user";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid userId, LoggedUser loggedUser, UserRepository userRepo) =>
    {
        try
        {
            if (userId != loggedUser.Id)
                return Results.Forbid();

            User? userData = userRepo.GetUser(userId);
            return Results.Ok(userData);
        }
        catch(ArgumentException argumentNull)
        {
            return Results.Problem(title: argumentNull.Message, statusCode: 400);
        }
        catch(Exception)
        {
            return Results.Problem();
        }
    };
}


