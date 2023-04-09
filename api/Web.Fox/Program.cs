using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Authorization;
using Web.Fox.AppBuilder;
using API.Fox;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
//builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(20));
//builder.Services.AddMemoryCache();

var (corpInfo, appInfo, security) = builder.AddAppConfig();
builder.AddAppAuth(security);
builder.AddAppRepositories(security, appInfo);
var razorBuilder = builder.Services.AddRazorPages();
if (builder.Environment.IsDevelopment())
    razorBuilder.AddRazorRuntimeCompilation();

var app = builder.Build();
app.UseStaticFiles();
//app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages(); //by default will map the razor pages in the Pages subfolder

app.Run();