using System;
using API.Fox.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fox.Access.Repository;
using API.Fox.EndPoint;

namespace API.Fox.Modules.Login;

public class LoginEndpoint : IEndPointAnonymous
{
    public string UrlPattern => "/security/token/create";
    public EndPointVerb Verb => EndPointVerb.POST;
    
    public Delegate Method =>
        [AllowAnonymous] (UserAuth user, UserRepository userRepo, Security security) =>
    {
        //TODO: the validation should be done against a database
        if (!(user.UserName == "admin" && user.Password == "123456" && user.GrandType == "password"))
            return Results.Unauthorized();

        var issuer = security.TokenIssuers.First();
        var audience = security.TokenAudiences.First();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(security.SymetricKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", "1"),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
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

