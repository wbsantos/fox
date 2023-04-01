using API.Fox.Settings;
using Microsoft.OpenApi.Models;

namespace API.Fox.AppBuilder;

internal static class Swagger
{
    internal static WebApplicationBuilder AddAppSwagger(this WebApplicationBuilder builder, CorporateInfo corporateInfo, AppInfo appInfo)
    {
        var securityScheme = new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JSON Web Token base security",
        };

        var securityRequirement = new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        };

        var contact = new OpenApiContact()
        {
            Name = corporateInfo.SupportName,
            Email = corporateInfo.SupportEmail,
            Url = new Uri(corporateInfo.SupportUrl),
        };

        var license = new OpenApiLicense()
        {
            Name = corporateInfo.LicenseName,
            Url = new Uri(corporateInfo.LicenseUrl),
        };

        var info = new OpenApiInfo()
        {
            Version = appInfo.Version,
            Title = appInfo.Title,
            Description = appInfo.Description,
            TermsOfService = new Uri(corporateInfo.TermsOfServiceUrl),
            Contact = contact,
            License = license,
        };

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", info);
            o.AddSecurityDefinition(securityScheme.Scheme, securityScheme);
            o.AddSecurityRequirement(securityRequirement);
        });

        builder.Services.AddEndpointsApiExplorer();
        return builder;
    }
}
