using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Authorization;
using Web.Fox.AppBuilder;
using API.Fox;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

var (appInfo, security) = builder.AddAppConfig();
builder.AddAppAuth(security);
builder.AddAppInjections(security, appInfo);
var razorBuilder = builder.Services.AddRazorPages();
if (builder.Environment.IsDevelopment())
    razorBuilder.AddRazorRuntimeCompilation();

var app = builder.Build();
app.CreateAdminUser();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages(); //by default will map the razor pages in the Pages subfolder
app.MapGet("/", () => Results.Redirect("/Menu/DownloadDocument", true));

app.Run();