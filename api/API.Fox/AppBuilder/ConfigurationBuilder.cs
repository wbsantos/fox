using System;
using API.Fox.Settings;

namespace API.Fox.AppBuilder;

internal static class ConfigurationBuilder
{
	internal static (CorporateInfo corporateInfo, AppInfo appInfo, Security security) AddAppConfig(this WebApplicationBuilder builder)
    {
        CorporateInfo corporateInfo = new();
        AppInfo appInfo = new();
        Security security = new();
        builder.Configuration.GetSection("CorporateInfo").Bind(corporateInfo);
        builder.Configuration.GetSection("AppInfo").Bind(appInfo);
        builder.Configuration.GetSection("Security").Bind(security);
        builder.Services.AddSingleton<CorporateInfo>(corporateInfo);
        builder.Services.AddSingleton<AppInfo>(appInfo);
        builder.Services.AddSingleton<Security>(security);

        return (corporateInfo, appInfo, security);
    }
}

