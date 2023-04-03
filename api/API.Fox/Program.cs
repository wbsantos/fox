using Microsoft.AspNetCore.Authorization;
using API.Fox.AppBuilder;
using API.Fox;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

var (corpInfo, appInfo, security) = builder.AddAppConfig();
builder.AddAppSwagger(corpInfo, appInfo);
builder.AddAppAuth(security);
builder.AddAppRepositories(security, appInfo);

var app = builder.Build();
app.MapAppEndPoints();
//TODO: remove the URL's bellow and redirect the root to swagger page
app.MapGet("/", () => "Hello World!");
app.MapGet("/hello/get", [Authorize] () => "Hello World with Authorization!");

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
