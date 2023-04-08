using Microsoft.AspNetCore.Authorization;
using Web.Fox.AppBuilder;
using API.Fox;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

var (corpInfo, appInfo, security) = builder.AddAppConfig();
builder.AddAppAuth(security);
builder.AddAppRepositories(security, appInfo);
builder.Services.AddRazorPages();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages(); //by default will map the razor pages in the Pages subfolder

app.Run();