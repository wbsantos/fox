using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.UserManagement;

//TODO: Control user access to other users data
public class UserDeleteEndPoint : IEndPoint
{
    public string PermissionClaim => "USER_DELETION_MANAGEMENT";
    public string UrlPattern => "/management/user";
    public EndPointVerb Verb => EndPointVerb.DELETE;
    public Delegate Method => (Guid userId, UserRepository userRepo) =>
    {
        try
        {
            userRepo.DeleteUser(userId);
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