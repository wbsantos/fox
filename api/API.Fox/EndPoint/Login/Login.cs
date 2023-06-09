﻿using System;
using API.Fox.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Fox.Access.Service;
using Fox.Access.Model;
using API.Fox.EndPoint;
using System.ComponentModel.DataAnnotations;

namespace API.Fox.EndPoint.Login;

public class Login : IEndPointAnonymous
{
    public string UrlPattern => "/security/token";
    public EndPointVerb Verb => EndPointVerb.POST;
    
    public Delegate Method => (LoginData user, UserService userService, Security security) =>
    {
        if (!(user.GrandType == "password" && userService.ValidateUserPassword(user.UserName, user.Password)))
            return Results.Unauthorized();

        var issuer = security.TokenIssuers.First();
        var audience = security.TokenAudiences.First();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(security.SymetricKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        User? userData = userService.GetUser(user.UserName);
        if (userData == null)
            return Results.Unauthorized();
        IEnumerable<string> permissions = userService.GetSystemPermissions(userData.Id);
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
        return Results.Ok(new { token = jwtToken, secondsToExpire = 15*60 } );
    }; 
}

record LoginData([Required] string UserName, [Required] string Password, [Required] string GrandType);
