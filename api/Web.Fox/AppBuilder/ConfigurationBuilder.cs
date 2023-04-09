using System;
using API.Fox.Settings;

namespace Web.Fox.AppBuilder;

internal static class ConfigurationBuilder
{
	internal static (AppInfo appInfo, Security security) AddAppConfig(this WebApplicationBuilder builder)
    {
        AppInfo appInfo = new();
        Security security = new();
        builder.Configuration.GetSection("AppInfo").Bind(appInfo);
        builder.Configuration.GetSection("Security").Bind(security);
        builder.Services.AddSingleton<AppInfo>(appInfo);
        builder.Services.AddSingleton<Security>(security);

        return (appInfo, security);
    }
}

