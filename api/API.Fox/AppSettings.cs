namespace API.Fox.Settings
{
    public class CorporateInfo
    {
        public string SupportName { get; set; } = "";
        public string SupportEmail { get; set; } = "";
        public string SupportUrl { get; set; } = "";
        public string LicenseName { get; set; } = "";
        public string LicenseUrl { get; set; } = "";
        public string TermsOfServiceUrl { get; set; } = "";
    }

    public class AppInfo
    {
        public string Version { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
    }
}