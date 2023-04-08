using Microsoft.AspNetCore.Authorization;
using Web.Fox.AppBuilder;
using API.Fox;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

var (corpInfo, appInfo, security) = builder.AddAppConfig();
builder.AddAppAuth(security);
builder.AddAppRepositories(security, appInfo);

var app = builder.Build();
app.MapGet("/", () => Results.Redirect("/swagger", true));
app.MapRazorPages();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.Run();


public partial class Program { } //expose Program as public so it can run in the API.Test.Fox project