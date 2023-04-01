namespace API.Fox.Settings;

public class CorporateInfo
{
    public string SupportName { get; set; } = string.Empty;
    public string SupportEmail { get; set; } = string.Empty;
    public string SupportUrl { get; set; } = string.Empty;
    public string LicenseName { get; set; } = string.Empty;
    public string LicenseUrl { get; set; } = string.Empty;
    public string TermsOfServiceUrl { get; set; } = string.Empty;
}

public class AppInfo
{
    public string Version { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class Security
{
    public string[] TokenIssuers { get; set; } = Array.Empty<string>();
    public string[] TokenAudiences { get; set; } = Array.Empty<string>();
    public string SymetricKey { get; set; } = string.Empty;
}