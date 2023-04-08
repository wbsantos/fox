using System;
using API.Fox.EndPoint;
using Fox.Access.Model;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.UserSelfManagement;

public class UserUpdatePassword : IEndPoint
{
    public string PermissionClaim => "USER_SELF_MANAGEMENT";
    public string UrlPattern => "/selfmanagement/user/password";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (UserSelfUpdatePasswordData user, LoggedUser loggedUser, UserRepository userRepo) =>
    {
        try
        {
            if (user.Id != loggedUser.Id)
                return Results.Unauthorized();

            userRepo.UpdatePassword(user.Id, user.Password);
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

record UserSelfUpdatePasswordData(Guid Id, string Password);
