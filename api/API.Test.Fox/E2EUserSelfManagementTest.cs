using System;
using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using API.Fox;
using static System.Net.WebRequestMethods;
using System.Net.Http;

namespace API.Test.Fox;

public class E2EUserSelfManagementTest : IClassFixture<FoxApplicationFactory<Program>>
{
    private const string URL_USER_MANAGEMENT = "/management/user";
    private const string URL_USER_SELFMANAGEMENT = "/selfmanagement/user";
    private const string URL_USER_SELF_UPDATE_PASSWORD = "/selfmanagement/user/password";
    private const string URL_GROUP_USER_MANAGEMENT = "/management/group/user";
    private const string URL_USER_GROUP_SELF_MANAGEMENT = "/selfmanagement/user/group";

    private readonly FoxApplicationFactory<Program> _factory;

    public E2EUserSelfManagementTest(FoxApplicationFactory<Program> factory)
    {
        _factory = factory;
    }


    [Fact]
    public async Task SelfManagementTest()
    {
        string token = await _factory.GetToken("admin", "123456");

        (Guid userId, string userToken) = await _factory.AddUser(token,
                                                                "self@email.com",
                                                                $"selfuser{_factory.UniqueId()}",
                                                                "Self Management User Test",
                                                                "123XYZ");
        var userNewLogin = await SelfUpdateUser(userToken, userId);
        await SelfGetUserGroups(token, userToken, userId);
        await SelfUpdateUserPassword(userToken, userNewLogin, userId);
        await new E2EUserManagementTest(_factory).DeleteUser(token, userId);
    }

    public async Task<string> SelfUpdateUser(string token, Guid userId)
    {
        var credentials = new
        {
            Id = userId,
            Email = "userselfedit@email.com",
            Login = $"userstestselfedit{_factory.UniqueId()}",
            Name = "User Self Edit Testing"
        };

        var client = _factory.BuildClient(token);
        var response = await client.PutAsJsonAsync(URL_USER_SELFMANAGEMENT, credentials);
        response.EnsureSuccessStatusCode();

        var user = await SelfGetUser(token, userId, true);
        Assert.Equal(credentials.Email, user!.Email);
        Assert.Equal(credentials.Login, user!.Login);
        Assert.Equal(credentials.Name, user!.Name);

        var credentialsAdmin = new
        {
            Id = userId,
            Email = "admin@email.com",
            Login = $"admin",
            Name = "Update should fail"
        };
        response = await client.PutAsJsonAsync(URL_USER_SELFMANAGEMENT, credentialsAdmin);
        Assert.False(response.IsSuccessStatusCode);

        return credentials.Login;
    }

    public async Task<dynamic?> SelfGetUser(string token, Guid userId, bool expectExists)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_USER_SELFMANAGEMENT}?userId={userId}");
        response.EnsureSuccessStatusCode();

        var data = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                        new
                                                        {
                                                            Id = new Guid(),
                                                            Email = string.Empty,
                                                            Login = string.Empty,
                                                            Name = string.Empty
                                                        });
        if (expectExists)
            Assert.NotNull(data);
        else
            Assert.Null(data);
        return data;
    }

    public async Task SelfUpdateUserPassword(string token, string userName, Guid userId)
    {
        var credentials = new
        {
            Id = userId,
            Password = "123ABC"
        };

        var client = _factory.BuildClient(token);
        var response = await client.PutAsJsonAsync(URL_USER_SELF_UPDATE_PASSWORD, credentials);
        response.EnsureSuccessStatusCode();

        response = await client.PutAsJsonAsync(URL_USER_SELF_UPDATE_PASSWORD, new { Id = userId, Password = string.Empty });
        Assert.False(response.IsSuccessStatusCode);

        await _factory.GetToken(userName, credentials.Password);
    }

    public async Task SelfGetUserGroups(string adminToken, string userToken, Guid userId)
    {
        E2EGroupManagementTest groupTest = new E2EGroupManagementTest(_factory);
        Guid groupId = await groupTest.AddGroup(adminToken, $"Group Self User Test {_factory.UniqueId()}");
        await AddUsersInGroup(adminToken, groupId, userId);

        var client = _factory.BuildClient(userToken);
        var response = await client.GetAsync($"{URL_USER_GROUP_SELF_MANAGEMENT}?userId={userId}");
        response.EnsureSuccessStatusCode();
        var dataResponse = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                                new
                                                                {
                                                                    groups = new[] {
                                                                        new {
                                                                            Id = new Guid(),
                                                                            Name = string.Empty
                                                                        }
                                                                    }
                                                                });
        Assert.NotNull(dataResponse);
        Assert.Contains(dataResponse!.groups, g => g.Id == groupId);
        await groupTest.DeleteGroup(adminToken, groupId);
    }

    private async Task AddUsersInGroup(string token, Guid groupId, Guid userId)
    {
        var userIds = new Guid[] { userId };
        var client = _factory.BuildClient(token);
        var dataRequest = new { GroupId = groupId, UserIds = userIds };
        var response = await client.PostAsJsonAsync(URL_GROUP_USER_MANAGEMENT, dataRequest);
        response.EnsureSuccessStatusCode();
    }
}
