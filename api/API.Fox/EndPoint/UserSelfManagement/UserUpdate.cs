using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.UserSelfManagement;

public class UserUpdate : IEndPoint
{
    public string PermissionClaim => "USER_SELF_MANAGEMENT";
    public string UrlPattern => "/selfmanagement/user";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (User user, LoggedUser loggedUser, UserRepository userRepo) =>
    {
        try
        {
            if (user.Id != loggedUser.Id)
                return Results.Unauthorized();

            userRepo.UpdateUser(user);
            return Results.Ok();
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

