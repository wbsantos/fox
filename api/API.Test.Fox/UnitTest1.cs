using System;
using System.Net;
using System.Net.Http.Json;
using API.Fox;
using Microsoft.AspNetCore.Mvc.Testing;
using static System.Net.WebRequestMethods;

namespace API.Test.Fox;

public class UnitTest1 : IClassFixture<FoxApplicationFactory<Program>>
{
    private readonly FoxApplicationFactory<Program> _factory;

    public UnitTest1(FoxApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Test1()
    {
        Assert.True(true);
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
