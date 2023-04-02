using Microsoft.AspNetCore.Authorization;
using API.Fox.AppBuilder;
using API.Fox;

var builder = WebApplication.CreateBuilder(args);
var (corpInfo, appInfo, security) = builder.AddAppConfig();
builder.AddAppSwagger(corpInfo, appInfo);
builder.AddAppAuth(security);
builder.AddAppRepositories(security, appInfo);

var app = builder.Build();
app.MapAppEndPoints();
app.MapGet("/", () => "Hello World!");
app.MapGet("/hello/get", [Authorize] () => "Hello World with Authorization!");

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
