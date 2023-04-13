using System;
using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using API.Fox;
using static System.Net.WebRequestMethods;
using System.Net.Http;
using System.Security.Cryptography;

namespace API.Test.Fox;

public class E2EGroupManagementTest : IClassFixture<FoxApplicationFactory<Program>>
{
    private const string URL_GROUP_MANAGEMENT = "/management/group";
    private const string URL_GROUP_USER_MANAGEMENT = "/management/group/user";
    private const string URL_GROUP_GET_ALL = "/management/group/all";
        
    private readonly FoxApplicationFactory<Program> _factory;

    public E2EGroupManagementTest(FoxApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ManagementTest()
    {
        string token = await _factory.GetToken("admin", "123456");

        Guid groupId = await AddGroup(token, $"Group Test {_factory.UniqueId()}");
        var userNewLogin = await UpdateGroup(token, groupId);
        
        var allGroups = await GetAllGroups(token);
        Assert.Contains(allGroups, g => g.Id == groupId);

        var usersInGroup = await AddUsersInGroup(token, groupId);
        await DeleteUserFromGroup(token, groupId, usersInGroup);
        var userTest = new E2EUserManagementTest(_factory);
        foreach(var ug in usersInGroup)
            await userTest.DeleteUser(token, ug);

        await DeleteGroup(token, groupId);
        await GetGroup(token, groupId, false);
    }

    public async Task<Guid> AddGroup(string token, string groupName)
    {
        var dataRequest = new { Name = groupName };
        var client = _factory.BuildClient(token);
        var response = await client.PostAsJsonAsync(URL_GROUP_MANAGEMENT, dataRequest);
        response.EnsureSuccessStatusCode();

        var dataResponse = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                                new { Name = string.Empty, Id = new Guid() });
        Assert.NotNull(dataResponse);
        Assert.True(dataResponse!.Id != new Guid());

        var group = await GetGroup(token, dataResponse!.Id, true);
        Assert.Equal(dataRequest.Name, group!.Name);

        response = await client.PostAsJsonAsync(URL_GROUP_MANAGEMENT, dataRequest);
        Assert.False(response.IsSuccessStatusCode);
        return dataResponse!.Id;
    }

    public async Task<string> UpdateGroup(string token, Guid groupId)
    {
        var dataRequest = new
        {
            Id = groupId,
            Name = $"Group Edit Test {_factory.UniqueId()}"
        };

        var client = _factory.BuildClient(token);
        var response = await client.PutAsJsonAsync(URL_GROUP_MANAGEMENT, dataRequest);
        response.EnsureSuccessStatusCode();

        var group = await GetGroup(token, groupId, true);
        Assert.Equal(dataRequest.Name, group!.Name);


        string tempGroupName = $"Temp Group {_factory.UniqueId()}";
        Guid tempGroup = await AddGroup(token, tempGroupName);
        response = await client.PutAsJsonAsync(URL_GROUP_MANAGEMENT, new { Id = groupId, Name = tempGroupName });
        Assert.False(response.IsSuccessStatusCode);
        await DeleteGroup(token, tempGroup);

        return group!.Name;
    }

    public async Task DeleteGroup(string token, Guid groupId)
    {
        var client = _factory.BuildClient(token);
        var response = await client.DeleteAsync($"{URL_GROUP_MANAGEMENT}?groupId={groupId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<dynamic?> GetGroup(string token, Guid groupId, bool expectExists)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_GROUP_MANAGEMENT}?groupId={groupId}");
        response.EnsureSuccessStatusCode();

        var data = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                        new
                                                        {
                                                            Id = new Guid(),
                                                            Name = string.Empty
                                                        });
        if(expectExists)
            Assert.NotNull(data);
        else
            Assert.Null(data);
        return data;
    }

    public async Task<IEnumerable<dynamic>> GetAllGroups(string token)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_GROUP_GET_ALL}");
        response.EnsureSuccessStatusCode();

        var data = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                        new
                                                        {
                                                            groups = new[] {
                                                                new {
                                                                    Id = new Guid(),
                                                                    Name = string.Empty
                                                                }
                                                            }
                                                        });
        Assert.NotNull(data);
        return data!.groups;
    }

    public async Task<Guid[]> AddUsersInGroup(string token, Guid groupId)
    {
        string userLogin1 = $"usergroup1{_factory.UniqueId()}";
        string userLogin2 = $"usergroup2{_factory.UniqueId()}";
        (Guid userId1, _) = await _factory.AddUser(token,
                                                   "usergroup1@email.com",
                                                    userLogin1,
                                                    "User 1 for Testing Group Addition",
                                                    "123456");

        (Guid userId2, _) = await _factory.AddUser(token,
                                                   "usergroup2@email.com",
                                                    userLogin2,
                                                    "User 2 for Testing Group Addition",
                                                    "123456");
        var userIds = new Guid[] { userId1, userId2 };
        var client = _factory.BuildClient(token);
        var dataRequest = new { GroupId = groupId, UserIds = userIds };
        var response = await client.PostAsJsonAsync(URL_GROUP_USER_MANAGEMENT, dataRequest);
        response.EnsureSuccessStatusCode();

        var usersInGroup = await GetUsersInGroup(token, groupId);
        Assert.Contains(usersInGroup, ug => ug.Id == userId1
                                            && ug.Login == userLogin1
                                            && ug.Name == "User 1 for Testing Group Addition"
                                            && ug.Email == "usergroup1@email.com");

        Assert.Contains(usersInGroup, ug => ug.Id == userId2
                                            && ug.Login == userLogin2
                                            && ug.Name == "User 2 for Testing Group Addition"
                                            && ug.Email == "usergroup2@email.com");
        return userIds;
    }

    public async Task<Guid[]> AddUsersInGroup(string token, Guid groupId, Guid[] userIds)
    {
        var client = _factory.BuildClient(token);
        var dataRequest = new { GroupId = groupId, UserIds = userIds };
        var response = await client.PostAsJsonAsync(URL_GROUP_USER_MANAGEMENT, dataRequest);
        response.EnsureSuccessStatusCode();
        return userIds;
    }

    public async Task<IEnumerable<dynamic>> GetUsersInGroup(string token, Guid groupId)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_GROUP_USER_MANAGEMENT}?groupId={groupId}");
        response.EnsureSuccessStatusCode();

        var dataResponse = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                                new {
                                                                    Users = new[] {
                                                                        new {
                                                                            Id = new Guid(),
                                                                            Email = string.Empty,
                                                                            Name = string.Empty,
                                                                            Login = string.Empty
                                                                        }
                                                                    }
                                                                });
        Assert.NotNull(dataResponse);
        return dataResponse!.Users;
    }

    public async Task DeleteUserFromGroup(string token, Guid groupId, IEnumerable<Guid> users)
    {
        Assert.True(users.Count() > 1);
        var dataRequest = new { GroupId = groupId, UserIds = new Guid[] { users.First() } };

        var client = _factory.BuildClient(token);
        var response = await client.PutAsJsonAsync($"{URL_GROUP_USER_MANAGEMENT}", dataRequest);
        response.EnsureSuccessStatusCode();

        var usersInGroup = await GetUsersInGroup(token, groupId);
        Assert.DoesNotContain(usersInGroup, ug => ug.Id == users.First());
        Assert.Contains(usersInGroup, ug => ug.Id == users.Last());
    }
}


