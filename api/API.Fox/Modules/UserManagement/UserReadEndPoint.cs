using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.UserManagement;

public class UserReadEndPoint : IEndPoint
{
    public string PermissionClaim => "USER_READ_MANAGEMENT";
    public string UrlPattern => "/management/user";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid userId, UserRepository userRepo) =>
    {
        try
        {
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


