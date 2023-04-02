using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.UserManagement;

public class UserUpdateEndPoint : IEndPoint
{
    public string PermissionClaim => "USER_UPDATE";

    public string UrlPattern => "/management/user";

    public EndPointVerb Verb => EndPointVerb.PUT;

    public Delegate Method => (User user, UserRepository userRepo) =>
    {
        try
        {
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

