using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using API.Fox.AppBuilder;
using API.Fox.Settings;

//TODO: better organize the URL mapping using the modules folder

var builder = WebApplication.CreateBuilder(args);
var (corpInfo, appInfo, security) = builder.AddAppConfig();
builder.AddAppSwagger(corpInfo, appInfo);
builder.AddAppAuth(security);

var app = builder.Build();
app.MapPost("/security/token/create", [AllowAnonymous] (UserAuth user) =>
{
    //TODO the validation should be done against a database
    //TODO the JWT creation should use config file or environment variable
    if(!(user.UserName == "admin" && user.Password == "123456"))
        return Results.Unauthorized();
    
    var issuer = "fox.app";
    var audience = "fox.app";
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("wbsantosabcdefghijklmnopq"));
    //TODO use a certificate to create signingcredentials
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
    var jwtTokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes("wbsantosabcdefghijklmnopq");
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity (new[]
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
});

app.MapGet("/", [AllowAnonymous] () => "Hello World!");
app.MapGet("/hello/get", [Authorize] () => "Hello World with Authorization!");

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
