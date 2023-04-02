using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.Modules.UserManagement;

public class UserCreateEndPoint : IEndPoint
{
    public string PermissionClaim => "USER_CREATION_MANAGEMENT";
    public string UrlPattern => "/management/user";
    public EndPointVerb Verb => EndPointVerb.POST;
    public Delegate Method => (UserCreationData user, UserRepository userRepo) =>
    {
        try
        {
            var userCreated = userRepo.CreateUser(user, user.Password ?? string.Empty);
            user.Password = null; //do not return the password
            return Results.Ok(userCreated);
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

public class UserCreationData : User
{
    public string? Password { get; set; } = string.Empty;
}

