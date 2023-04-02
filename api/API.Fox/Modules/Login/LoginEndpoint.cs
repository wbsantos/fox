using System;
using API.Fox.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Fox.Access.Repository;
using Fox.Access.Model;
using API.Fox.EndPoint;

namespace API.Fox.Modules.Login;

public class LoginEndpoint : IEndPointAnonymous
{
    public string UrlPattern => "/security/token/create";
    public EndPointVerb Verb => EndPointVerb.POST;
    
    public Delegate Method => (UserAuth user, UserRepository userRepo, Security security) =>
    {
        if (!(user.GrandType == "password" && userRepo.ValidateUserPassword(user.UserName, user.Password)))
            return Results.Unauthorized();

        var issuer = security.TokenIssuers.First();
        var audience = security.TokenAudiences.First();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(security.SymetricKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        User? userData = userRepo.GetUser(user.UserName);
        if (userData == null)
            return Results.Unauthorized();
        IEnumerable<string> permissions = userRepo.GetSystemPermissions(userData.Id);
        IEnumerable<Claim> permissionsClaims = permissions.Select(permissionKey => new Claim("SystemPermission", permissionKey));
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(nameof(User.Id), userData.Id.ToString()),
                            new Claim(nameof(User.Login), userData.Login),
                            new Claim(nameof(User.Email), userData.Email),
                            new Claim(nameof(User.Name), userData.Name)
                        }.Union(permissionsClaims)),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Audience = audience,
            Issuer = issuer,
            SigningCredentials = credentials
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);
        return Results.Ok(jwtToken);
    }; 
}

