using System;
using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using API.Fox;
using static System.Net.WebRequestMethods;
using System.Net.Http;

namespace API.Test.Fox;

public class E2EUserManagementTest : IClassFixture<FoxApplicationFactory<Program>>
{
    private const string URL_USER_MANAGEMENT = "/management/user";
    private const string URL_USER_GET_ALL = "/management/user/all";
    private const string URL_USER_UPDATE_PASSWORD = "/management/user/password";
    private const string URL_GROUP_USER_MANAGEMENT = "/management/group/user";
    private const string URL_USER_GROUP_MANAGEMENT = "/management/user/group";

    private readonly FoxApplicationFactory<Program> _factory;

    public E2EUserManagementTest(FoxApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ManagementTest()
    {
        string token = await _factory.GetToken("admin", "123456");

        (Guid userId, string userToken) = await AddUser(token);
        var userNewLogin = await UpdateUser(token, userId);
        await UpdateUserPassword(token, userNewLogin, userId);
        
        var allUsers = await GetAllUsers(token);
        Assert.Contains(allUsers, u => u.Id == userId);

        await GetUserGroups(token, userId);

        await DeleteUser(token, userId);
        await GetUser(token, userId, false);
    }

    public async Task<(Guid userId, string userToken)> AddUser(string token)
    {
        string login = $"usertest{DateTime.Now.Ticks}";
        string password = "ABCDEF";
        (Guid userId, string userToken) = await _factory.AddUser(token,
                                                                "user@email.com",
                                                                login,
                                                                "User for Testing",
                                                                password);

        var user = await GetUser(token, userId, true);
        Assert.Equal("user@email.com", user!.Email);
        Assert.Equal(login, user!.Login);
        Assert.Equal("User for Testing", user!.Name);


        await Assert.ThrowsAsync<HttpRequestException>(async () => await _factory.AddUser(token,
                                                                                          "user@email.com",
                                                                                          login,
                                                                                          "User for Testing",
                                                                                          password));
        return (userId, userToken);
    }

    public async Task<string> UpdateUser(string token, Guid userId)
    {
        var credentials = new
        {
            Id = userId,
            Email = "useredit@email.com",
            Login = $"usertestedit{DateTime.Now.Ticks}",
            Name = "User Edit for Testing"
        };

        var client = _factory.BuildClient(token);
        var response = await client.PutAsJsonAsync(URL_USER_MANAGEMENT, credentials);
        response.EnsureSuccessStatusCode();

        var user = await GetUser(token, userId, true);
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
        response = await client.PutAsJsonAsync(URL_USER_MANAGEMENT, credentialsAdmin);
        Assert.False(response.IsSuccessStatusCode);

        return credentials.Login;
    }

    public async Task DeleteUser(string token, Guid userId)
    {
        var client = _factory.BuildClient(token);
        var response = await client.DeleteAsync($"{URL_USER_MANAGEMENT}?userId={userId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateUserPassword(string token, string userName, Guid userId)
    {
        var credentials = new
        {
            Id = userId,
            Password = "123456A"
        };

        var client = _factory.BuildClient(token);
        var response = await client.PutAsJsonAsync(URL_USER_UPDATE_PASSWORD, credentials);
        response.EnsureSuccessStatusCode();

        response = await client.PutAsJsonAsync(URL_USER_UPDATE_PASSWORD, new { Id = userId, Password = string.Empty });
        Assert.False(response.IsSuccessStatusCode);

        await _factory.GetToken(userName, credentials.Password);
    }

    public async Task<dynamic?> GetUser(string token, Guid userId, bool expectExists)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_USER_MANAGEMENT}?userId={userId}");
        response.EnsureSuccessStatusCode();

        var data = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                        new
                                                        {
                                                            Id = new Guid(),
                                                            Email = string.Empty,
                                                            Login = string.Empty,
                                                            Name = string.Empty
                                                        });
        if(expectExists)
            Assert.NotNull(data);
        else
            Assert.Null(data);
        return data;
    }

    public async Task<IEnumerable<dynamic>> GetAllUsers(string token)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_USER_GET_ALL}");
        response.EnsureSuccessStatusCode();

        var data = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                        new
                                                        {
                                                            users = new[] {
                                                                new {
                                                                    Id = new Guid(),
                                                                    Email = string.Empty,
                                                                    Login = string.Empty,
                                                                    Name = string.Empty
                                                                }
                                                            }
                                                        });
        Assert.NotNull(data);
        return data!.users;
    }

    public async Task GetUserGroups(string token, Guid userId)
    {
        E2EGroupManagementTest groupTest = new E2EGroupManagementTest(_factory);
        Guid groupId = await groupTest.AddGroup(token, $"Group User Management Test {DateTime.Now.Ticks}");
        await AddUsersInGroup(token, groupId, userId);

        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_USER_GROUP_MANAGEMENT}?userId={userId}");
        response.EnsureSuccessStatusCode();
        var dataResponse = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                                new {
                                                                    groups = new[] {
                                                                        new {
                                                                            Id = new Guid(),
                                                                            Name = string.Empty
                                                                        }
                                                                    }
                                                                });
        Assert.NotNull(dataResponse);
        Assert.Contains(dataResponse!.groups, g => g.Id == groupId);
        await groupTest.DeleteGroup(token, groupId);
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
