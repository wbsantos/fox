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

public class E2EPermissionManagementTest : IClassFixture<FoxApplicationFactory<Program>>
{
    private const string URL_SYSTEM_PERMISSION = "/management/systempermission";
    private readonly FoxApplicationFactory<Program> _factory;

    public E2EPermissionManagementTest(FoxApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ManagementTest()
    {
        var groupTest = new E2EGroupManagementTest(_factory);
        var userTest = new E2EUserManagementTest(_factory);
        string token = await _factory.GetToken("admin", "123456");

        Guid groupId = await groupTest.AddGroup(token, $"Group Permission Test {_factory.UniqueId()}");
        (Guid userId, _) = await userTest.AddUser(token);

        await AddPermission(token, groupId, "SYSTEM_PERMISSION_ADDITION");
        var groupPermissions = await GetPermissions(token, groupId);
        Assert.Contains(groupPermissions, p => p == "SYSTEM_PERMISSION_ADDITION");
        await DeletePermission(token, groupId, "SYSTEM_PERMISSION_ADDITION");
        groupPermissions = await GetPermissions(token, groupId);
        Assert.DoesNotContain(groupPermissions, p => p == "SYSTEM_PERMISSION_ADDITION");

        await AddPermission(token, userId, "USER_CREATION_MANAGEMENT");
        var userPermissions = await GetPermissions(token, userId);
        Assert.Contains(userPermissions, p => p == "USER_CREATION_MANAGEMENT");
        await DeletePermission(token, userId, "USER_CREATION_MANAGEMENT");
        groupPermissions = await GetPermissions(token, userId);
        Assert.DoesNotContain(groupPermissions, p => p == "USER_CREATION_MANAGEMENT");

        await userTest.DeleteUser(token, userId);
        await groupTest.DeleteGroup(token, groupId);
    }

    public async Task AddPermission(string token, Guid holderId, string permission)
    {
        var dataRequest = new { PermissionHolderId = holderId, Permission = permission };
        var client = _factory.BuildClient(token);
        var response = await client.PostAsJsonAsync(URL_SYSTEM_PERMISSION, dataRequest);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<string>> GetPermissions(string token, Guid holderId)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_SYSTEM_PERMISSION}?holderId={holderId}");
        response.EnsureSuccessStatusCode();
        var dataResponse = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                                new { permissions = Array.Empty<string>() });
        Assert.NotNull(dataResponse);
        return dataResponse!.permissions;
    }

    public async Task DeletePermission(string token, Guid holderId, string permission)
    {
        var client = _factory.BuildClient(token);
        var response = await client.DeleteAsync($"{URL_SYSTEM_PERMISSION}?holderId={holderId}&permission={permission}");
        response.EnsureSuccessStatusCode();
    }
}


