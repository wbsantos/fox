using System;
using API.Fox.Settings;

namespace API.Fox.AppBuilder;

internal static class ConfigurationBuilder
{
	internal static (CorporateInfo corporateInfo, AppInfo appInfo) AddFoxConfig(this WebApplicationBuilder builder)
    {
        CorporateInfo corporateInfo = new();
        AppInfo appInfo = new();
        builder.Configuration.GetSection("CorporateInfo").Bind(corporateInfo);
        builder.Configuration.GetSection("AppInfo").Bind(appInfo);
        builder.Services.AddSingleton<CorporateInfo>(corporateInfo);
        builder.Services.AddSingleton<AppInfo>(appInfo);
        
        return (corporateInfo, appInfo);
    }
}

