using System;
using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using API.Fox;
using static System.Net.WebRequestMethods;

namespace API.Test.Fox;

public class E2ETest : IClassFixture<FoxApplicationFactory<Program>>
{
    private const string URL_TOKEN = "/security/token";

    private readonly FoxApplicationFactory<Program> _factory;

    public E2ETest(FoxApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Test1()
    {
        string token = await GetAdminToken();
        Assert.True(true);
    }

    public async Task<string> GetAdminToken()
    {
        var credentials = new { UserName = "admin", Password = "123456", GrandType = "password" };

        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync(URL_TOKEN, credentials);
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        var data = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                        new { token = string.Empty });
        Assert.NotNull(data);
        Assert.False(string.IsNullOrWhiteSpace(data?.token));
        return data!.token;
    }

    [Fact]
    public async void TryAdminWrongPassword()
    {
        var credentials = new { UserName = "admin", Password = "1111", GrandType = "password" };

        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync(URL_TOKEN, credentials);
        HttpStatusCode statusCode = response.StatusCode;

        Assert.True(statusCode == HttpStatusCode.Unauthorized);
        string data = await response.Content.ReadAsStringAsync();
        Assert.True(string.IsNullOrEmpty(data));
    }

    [Theory]
    [InlineData("/management/user")]
    //[InlineData("/management/group")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("Hello World!", await response.Content.ReadAsStringAsync());
    }


    /*
    // Post example
    [Fact]
    public async Task CreatePerson()
    {
        await using var application = new FoxApplicationFactory<Program>();
        var client = application.CreateClient();

        var result = await client.PostAsJsonAsync("/people", new Person
        {
            FirstName = "Maarten",
            LastName = "Balliauw",
            Email = "maarten@jetbrains.com"
        });

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("\"Maarten Balliauw created.\"", await result.Content.ReadAsStringAsync());
    }
    */
}
