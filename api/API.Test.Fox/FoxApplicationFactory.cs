using System;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace API.Test.Fox;

public class FoxApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private const string URL_TOKEN = "/security/token";
    private const string URL_USER_MANAGEMENT = "/management/user";
        
    public FoxApplicationFactory()
	{
        
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
    }

    public HttpClient BuildClient(string token)
    {
        var client = this.CreateClient();
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        return client;
    }

    public async Task<string> GetToken(string userName, string password)
    {
        var credentials = new { UserName = userName, Password = password, GrandType = "password" };

        var client = BuildClient(string.Empty);
        var response = await client.PostAsJsonAsync(URL_TOKEN, credentials);
        response.EnsureSuccessStatusCode();

        var data = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                        new { token = string.Empty });
        Assert.NotNull(data);
        Assert.False(string.IsNullOrWhiteSpace(data?.token));

        await TryWrongPassword(userName, password + "123");
        return data!.token;
    }

    public async Task TryWrongPassword(string userName, string password)
    {
        var credentials = new { UserName = userName, Password = password, GrandType = "password" };

        var client = BuildClient(string.Empty);
        var response = await client.PostAsJsonAsync(URL_TOKEN, credentials);
        HttpStatusCode statusCode = response.StatusCode;

        Assert.True(statusCode == HttpStatusCode.Unauthorized);
        string data = await response.Content.ReadAsStringAsync();
        Assert.True(string.IsNullOrEmpty(data));
    }

    public async Task<(Guid userId, string userToken)> AddUser(string token, string email, string login, string name, string password)
    {
        var credentials = new
        {
            Email = email,
            Login = login,
            Name = name,
            Password = password
        };

        var client = BuildClient(token);
        var response = await client.PostAsJsonAsync(URL_USER_MANAGEMENT, credentials);
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        var data = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                        new { id = new Guid() });
        Assert.NotNull(data);
        Assert.True(data!.id != new Guid());

        string userToken = await GetToken(credentials.Login, credentials.Password);
        return (data!.id, userToken);
    }

    public string UniqueId()
    {
        return $"{DateTime.Now.Ticks}";
    }
}