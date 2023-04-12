using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Service;

namespace API.Fox.EndPoint.UserManagement;

public class UserCreate : IEndPoint
{
    public string PermissionClaim => "USER_CREATION_MANAGEMENT";
    public string UrlPattern => "/management/user";
    public EndPointVerb Verb => EndPointVerb.POST;
    public Delegate Method => (UserCreationData user, UserService userService) =>
    {
        var userCreated = userService.CreateUser(user, user.Password ?? string.Empty);
        user.Password = null; //do not return the password
        return Results.Ok(userCreated);
    };
}

public class UserCreationData : User
{
    public string? Password { get; set; } = string.Empty;
}

