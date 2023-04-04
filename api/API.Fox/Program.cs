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
app.MapGet("/", () => Results.Redirect("/swagger", true));

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
