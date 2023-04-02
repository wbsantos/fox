using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.UserManagement;

public class UserReadAllEndPoint : IEndPoint
{
    public string PermissionClaim => "USER_READ_ALL_MANAGEMENT";
    public string UrlPattern => "/management/user/all";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (UserRepository userRepo) =>
    {
        try
        {
            IEnumerable<User> userData = userRepo.GetAllUsers();
            if (userData.Count() == 0)
                return Results.NotFound();
            else
                return Results.Ok(new { users = userData });
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


