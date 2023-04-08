using System;
using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using API.Fox;
using static System.Net.WebRequestMethods;
using System.Net.Http;
using System.Security.Cryptography;
using API.Fox.EndPoint;
using Microsoft.AspNetCore.Builder;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace API.Test.Fox;

public class E2EEndPointAuthorizeTest : IClassFixture<FoxApplicationFactory<Program>>
{
    private const string URL_SYSTEM_PERMISSION = "/management/systempermission";
    private readonly FoxApplicationFactory<Program> _factory;

    public E2EEndPointAuthorizeTest(FoxApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AuthorizeTest()
    {
        string token = await _factory.GetToken("admin", "123456");
        string userName = $"userauth{_factory.UniqueId()}";
        string userPassword = "123abc";
        (Guid userId, string userToken) = await _factory.AddUser(token,
                                                                 "email@email.com",
                                                                 userName,
                                                                 "User for testing endpoint authorization",
                                                                 userPassword);

        E2EPermissionManagementTest systemTest = new E2EPermissionManagementTest(_factory);
        E2EUserManagementTest userTest = new E2EUserManagementTest(_factory);
        E2EGroupManagementTest groupTest = new E2EGroupManagementTest(_factory);

        Guid groupId = await groupTest.AddGroup(token, $"Group for endpoint auth {_factory.UniqueId()}");
        await groupTest.AddUsersInGroup(token, groupId, new Guid[] { userId });
        await systemTest.DeletePermission(token, userId, "USER_SELF_MANAGEMENT");
        userToken = await _factory.GetToken(userName, userPassword);

        IEnumerable<IEndPoint> endPoints = GetEndPointList();
        foreach (var endPoint in endPoints)
        {
            await MakeRequest(userToken, endPoint.UrlPattern, endPoint.Verb, endPoint.PermissionClaim, true);

            await systemTest.AddPermission(token, userId, endPoint.PermissionClaim);
            userToken = await _factory.GetToken(userName, userPassword);
            await MakeRequest(userToken, endPoint.UrlPattern, endPoint.Verb, endPoint.PermissionClaim, false);

            await systemTest.DeletePermission(token, userId, endPoint.PermissionClaim);
            userToken = await _factory.GetToken(userName, userPassword);
            await MakeRequest(userToken, endPoint.UrlPattern, endPoint.Verb, endPoint.PermissionClaim, true);

            await systemTest.AddPermission(token, groupId, endPoint.PermissionClaim);
            userToken = await _factory.GetToken(userName, userPassword);
            await MakeRequest(userToken, endPoint.UrlPattern, endPoint.Verb, endPoint.PermissionClaim, false);
            await systemTest.DeletePermission(token, groupId, endPoint.PermissionClaim);
            userToken = await _factory.GetToken(userName, userPassword);
        }

        await userTest.DeleteUser(token, userId);
        await groupTest.DeleteGroup(token, groupId);
    }

    private async Task MakeRequest(string token, string url, EndPointVerb verb, string claim, bool expectForbidden)
    {
        var client = _factory.BuildClient(token);
        HttpResponseMessage response;
        switch (verb)
        {
            case EndPointVerb.DELETE:
                response = await client.DeleteAsync(url);
                break;
            case EndPointVerb.GET:
                response = await client.GetAsync(url);
                break;
            case EndPointVerb.POST:
                response = await client.PostAsJsonAsync(url, new { });
                break;
            case EndPointVerb.PUT:
                response = await client.PutAsJsonAsync(url, new { });
                break;
            default:
                return;
        }

        if(expectForbidden)
            Assert.True(HttpStatusCode.Forbidden == response.StatusCode, $"Expected forbbiden for '{url}'. Claim used: {claim}");
        else
            Assert.True(HttpStatusCode.Forbidden != response.StatusCode, $"Expected NOT forbbiden for '{url}'. Claim used: {claim}");
    }

    private IEnumerable<IEndPoint> GetEndPointList()
    {
        List<IEndPoint> list = new List<IEndPoint>();

        Type endpointInterface = typeof(IEndPoint);
        IEnumerable<Type> endPointsImplementation =
                ModuleReferences.GetAssemblies()
                         .SelectMany(a => a.GetTypes())
                         .Where(c => endpointInterface.IsAssignableFrom(c)
                                     && c.GetConstructor(Type.EmptyTypes) != null);

        foreach (var implementation in endPointsImplementation)
        {
            IEndPoint? implementationInstance = Activator.CreateInstance(implementation) as IEndPoint;
            if (implementationInstance != null)
            {
                if (implementationInstance.Verb == EndPointVerb.GET || implementationInstance.Verb == EndPointVerb.DELETE)
                {
                    var parameters = implementationInstance.Method.Method.GetParameters();
                    IEnumerable<string> pQuery = parameters.Where(p => p.ParameterType.IsValueType )
                                                           .Select(p => $"{p.Name}={Activator.CreateInstance(p.ParameterType)}" );
                    
                    string url = $"{implementationInstance.UrlPattern}?{string.Join("&", pQuery)}";
                    MockEndPoint mock = new MockEndPoint(implementationInstance.PermissionClaim, url, implementationInstance.Verb);
                    implementationInstance = mock;
                }
                list.Add(implementationInstance!);
            }
        }
        return list;
    }

    class MockEndPoint : IEndPoint
    {
        public string PermissionClaim { get; }
        public string UrlPattern { get; }
        public EndPointVerb Verb { get; }
        public Delegate Method => throw new NotImplementedException();

        public MockEndPoint(string claim, string url, EndPointVerb verb)
        {
            PermissionClaim = claim;
            UrlPattern = url;
            Verb = verb;
        }
    }
}


