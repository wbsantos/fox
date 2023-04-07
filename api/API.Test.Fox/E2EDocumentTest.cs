using System;
using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using API.Fox;
using static System.Net.WebRequestMethods;
using System.Net.Http;
using System.Security.Cryptography;
using System.Reflection;

namespace API.Test.Fox;

public class E2EDocumentTest : IClassFixture<FoxApplicationFactory<Program>>
{
    private const string URL_DOCUMENT = "/document";
    private const string URL_DOCUMENT_ALL = "/document/all";
    private const string URL_DOCUMENT_DOWNLOAD = "/document/download";
    private const string URL_DOCUMENT_PERMISSION = "/document/permission";

    private string _pdfTestPath;
    private readonly FoxApplicationFactory<Program> _factory;

    public E2EDocumentTest(FoxApplicationFactory<Program> factory)
    {
        _factory = factory;
        _pdfTestPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                                    "Backend_engineer_interview_project.pdf");
    }

    [Fact]
    public async Task DocumentTest()
    {
        var groupTest = new E2EGroupManagementTest(_factory);
        var userTest = new E2EUserManagementTest(_factory);
        string token = await _factory.GetToken("admin", "123456");

        var document = await UploadDocument(token);
        IEnumerable<dynamic>  allDocuments = await GetAllDocuments(token);
        Assert.Contains(allDocuments, d => d.Id == document!.Id);
        await UpdateDocument(token, document!.Id);

        Guid groupId = await groupTest.AddGroup(token, $"Group test for doc permission {DateTime.Now.Ticks}");
        (Guid userId, _) = await userTest.AddUser(token);

        IEnumerable<dynamic> permissions = await GetDocumentPermissions(token, document!.Id);
        Assert.Contains(permissions, p => p.HolderLogin == "admin" && p.Permission == DocumentPermission.Manage);
        Assert.DoesNotContain(permissions, p => p.HolderId == groupId);
        Assert.DoesNotContain(permissions, p => p.HolderId == userId);

        await AddDocumentPermission(token, document!.Id, groupId, DocumentPermission.Download);
        await AddDocumentPermission(token, document!.Id, userId, DocumentPermission.Manage);
        permissions = await GetDocumentPermissions(token, document!.Id);
        Assert.Contains(permissions, p => p.HolderId == groupId && p.Permission == DocumentPermission.Download);
        Assert.Contains(permissions, p => p.HolderId == userId && p.Permission == DocumentPermission.Manage);

        await DelDocumentPermission(token, document!.Id, userId, DocumentPermission.Manage);
        permissions = await GetDocumentPermissions(token, document!.Id);
        Assert.Contains(permissions, p => p.HolderLogin == "admin" && p.Permission == DocumentPermission.Manage);
        Assert.Contains(permissions, p => p.HolderId == groupId && p.Permission == DocumentPermission.Download);
        Assert.DoesNotContain(permissions, p => p.HolderId == userId && p.Permission == DocumentPermission.Manage);

        await DeleteDocument(token, document!.Id);
        await GetDocumentInformation(token, document!.Id, false);

        await groupTest.DeleteGroup(token, groupId);
        await userTest.DeleteUser(token, userId);
    }

    [Fact]
    public async Task PermissionTest()
    {
        var groupTest = new E2EGroupManagementTest(_factory);
        var userTest = new E2EUserManagementTest(_factory);
        var systemTest = new E2EPermissionManagementTest(_factory);
        string token = await _factory.GetToken("admin", "123456");
        string userName = $"userdocpermission{DateTime.Now.Ticks}";
        string userPassword = "123456789";

        var document = await UploadDocument(token);
        Guid groupId = await groupTest.AddGroup(token, $"Group test for doc permission {DateTime.Now.Ticks}");
        (Guid userId, string userToken) = await _factory.AddUser(token,
                                                                "email@email.com",
                                                                userName,
                                                                "User for testing document permission",
                                                                userPassword);
        await systemTest.AddPermission(token, userId, "DOCUMENT_READ");
        await systemTest.AddPermission(token, userId, "DOCUMENT_DELETION");
        await systemTest.AddPermission(token, userId, "DOCUMENT_UPDATE");
        await systemTest.AddPermission(token, userId, "DOCUMENT_PERMISSION_ADDITION");
        await systemTest.AddPermission(token, userId, "DOCUMENT_PERMISSION_REMOVAL");
        await systemTest.AddPermission(token, userId, "DOCUMENT_PERMISSION_READ");
        userToken = await _factory.GetToken(userName, userPassword);

        await GetDocumentInformation(userToken, document!.Id, true, expectsForbidden: true);
        await GetDocumentBinary(userToken, document!.Id, expectsForbidden: true);
        await AddDocumentPermission(userToken, document!.Id, userId, DocumentPermission.Download, expectsForbidden: true);
        await DelDocumentPermission(userToken, document!.Id, userId, DocumentPermission.Download, expectsForbidden: true);
        await GetDocumentPermissions(userToken, document!.Id, expectsForbidden: true);

        await AddDocumentPermission(token, document!.Id, userId, DocumentPermission.Download);
        await GetDocumentInformation(userToken, document!.Id, true, expectsForbidden: false);
        await GetDocumentBinary(userToken, document!.Id, expectsForbidden: false);
        await AddDocumentPermission(userToken, document!.Id, userId, DocumentPermission.Download, expectsForbidden: true);
        await DelDocumentPermission(userToken, document!.Id, userId, DocumentPermission.Download, expectsForbidden: true);
        await GetDocumentPermissions(userToken, document!.Id, false);

        await DelDocumentPermission(token, document!.Id, userId, DocumentPermission.Download);
        await GetDocumentInformation(userToken, document!.Id, true, expectsForbidden: true);

        await groupTest.AddUsersInGroup(token, groupId, new Guid[] { userId });
        await AddDocumentPermission(token, document!.Id, groupId, DocumentPermission.Download);
        await GetDocumentInformation(userToken, document!.Id, true, expectsForbidden: false);
        await AddDocumentPermission(userToken, document!.Id, userId, DocumentPermission.Download, expectsForbidden: true);
        await DelDocumentPermission(userToken, document!.Id, userId, DocumentPermission.Download, expectsForbidden: true);
        await GetDocumentPermissions(userToken, document!.Id, expectsForbidden: false);

        await UpdateDocument(userToken, document!.Id, expectsForbidden: true);
        await DeleteDocument(userToken, document!.Id, expectsForbidden: true);

        await AddDocumentPermission(token, document!.Id, groupId, DocumentPermission.Manage);
        await UpdateDocument(userToken, document!.Id, expectsForbidden: false);
        await AddDocumentPermission(userToken, document!.Id, userId, DocumentPermission.Download, expectsForbidden: false);
        await DelDocumentPermission(userToken, document!.Id, userId, DocumentPermission.Download, expectsForbidden: false);
        await GetDocumentPermissions(userToken, document!.Id, false);
        await DeleteDocument(userToken, document!.Id, expectsForbidden: false);

        await groupTest.DeleteGroup(token, groupId);
        await userTest.DeleteUser(token, userId);
    }

    public async Task<dynamic?> UploadDocument(string token)
    {
        var fileBytes = await System.IO.File.ReadAllBytesAsync(_pdfTestPath);
        var fileBase64 = Convert.ToBase64String(fileBytes);

        var dataRequest = new {
            Name = "Interview Project.pdf",
            FileBase64 = fileBase64,
            Metadata = new Dictionary<string, string>()
            {
                { "CATEGORY", "Test" },
                { "Just a Metadata", "Test X" },
                { "EXTENSION", "PDF" },
            }
        };

        var client = _factory.BuildClient(token);
        var response = await client.PostAsJsonAsync(URL_DOCUMENT, dataRequest);
        response.EnsureSuccessStatusCode();

        var documentCreated = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                                   new {
                                                                       Id = new Guid(),
                                                                       Name = string.Empty,
                                                                       FileSizeBytes = 0,
                                                                       Metadatada = new Dictionary<string, string>()
                                                                   });
        Assert.NotNull(documentCreated);

        var documentToTest = await GetDocumentInformation(token, documentCreated!.Id, true);
        Assert.Equal(dataRequest.Name, documentToTest!.Name);
        Assert.Equal(fileBytes.Length, documentToTest!.FileSizeBytes);
        Assert.Equal(dataRequest.Metadata["CATEGORY"], documentToTest!.Metadata["CATEGORY"]);
        Assert.Equal(dataRequest.Metadata["Just a Metadata"], documentToTest!.Metadata["Just a Metadata"]);
        Assert.Equal(dataRequest.Metadata["EXTENSION"], documentToTest!.Metadata["EXTENSION"]);

        byte[] binaryToTest = await GetDocumentBinary(token, documentCreated!.Id);
        Assert.True(fileBytes.SequenceEqual(binaryToTest));
        return documentCreated;
    }

    public async Task<dynamic?> GetDocumentInformation(string token, Guid documentId, bool expectExists, bool expectsForbidden = false)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_DOCUMENT}?documentId={documentId}");
        if(expectsForbidden)
        {
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            return null;

        }
        response.EnsureSuccessStatusCode();
        var dataResponse = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                                new {
                                                                    Id = new Guid(),
                                                                    Name = string.Empty,
                                                                    Metadata = new Dictionary<string, string>(),
                                                                    FileSizeBytes = 0
                                                                });
        if (expectExists)
            Assert.NotNull(dataResponse);
        else
            Assert.Null(dataResponse);

        return dataResponse;
    }

    public async Task<byte[]> GetDocumentBinary(string token, Guid documentId, bool expectsForbidden = false)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_DOCUMENT_DOWNLOAD}?documentId={documentId}");
        if (expectsForbidden)
        {
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            return Array.Empty<byte>();

        }
        response.EnsureSuccessStatusCode();
        var dataResponse = await response.Content.ReadAsByteArrayAsync();
        Assert.NotNull(dataResponse);
        return dataResponse;
    }

    public async Task UpdateDocument(string token, Guid documentId, bool expectsForbidden = false)
    {
        var dataRequest = new
        {
            Id = documentId,
            Name = "Interview Project Updated.pdf",
            MetadataToRemove = new string[]
            {
                "Just a Metadata"
            },
            MetadataToAdd = new Dictionary<string, string>()
            {
                { "CATEGORY", "Test 2" },
                { "Just another Metadata", "Test X" },
                { "EXTENSION", "PDF" },
            }
        };

        var client = _factory.BuildClient(token);
        var response = await client.PutAsJsonAsync(URL_DOCUMENT, dataRequest);
        if(expectsForbidden)
        {
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            return;
        }
        response.EnsureSuccessStatusCode();

        var documentToTest = await GetDocumentInformation(token, documentId, true);
        Assert.Equal(dataRequest.Name, documentToTest!.Name);
        Assert.Equal(dataRequest.MetadataToAdd["CATEGORY"], documentToTest!.Metadata["CATEGORY"]);
        Assert.Equal(dataRequest.MetadataToAdd["Just another Metadata"], documentToTest!.Metadata["Just another Metadata"]);
        Assert.Equal(dataRequest.MetadataToAdd["EXTENSION"], documentToTest!.Metadata["EXTENSION"]);
        dataRequest.MetadataToRemove.ToList().ForEach(r => Assert.DoesNotContain(r, documentToTest!.Metadata.Keys));
    }

    public async Task DeleteDocument(string token, Guid documentId, bool expectsForbidden = false)
    {
        var client = _factory.BuildClient(token);
        var response = await client.DeleteAsync($"{URL_DOCUMENT}?documentId={documentId}");
        if(expectsForbidden)
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        else
            response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<dynamic>> GetAllDocuments(string token)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_DOCUMENT_ALL}");
        response.EnsureSuccessStatusCode();
        var dataResponse = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                                new {
                                                                    documents = new[] {
                                                                    new {
                                                                        Id = new Guid(),
                                                                        Name = string.Empty,
                                                                        Metadata = new Dictionary<string, string>(),
                                                                        FileSizeBytes = 0
                                                                    }}
                                                                });
        Assert.NotNull(dataResponse);
        return dataResponse!.documents;
    }

    public async Task<IEnumerable<dynamic>> GetDocumentPermissions(string token, Guid documentId, bool expectsForbidden = false)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_DOCUMENT_PERMISSION}?documentId={documentId}");
        if (expectsForbidden)
        {
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            return Array.Empty<object>();
        }
        response.EnsureSuccessStatusCode();
        var dataResponse = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
                                                                new
                                                                {
                                                                    permissions = new[] {
                                                                    new {
                                                                        HolderId = new Guid(),
                                                                        HolderLogin = string.Empty,
                                                                        HolderName = string.Empty,
                                                                        Permission = DocumentPermission.Download,
                                                                        HolderType = string.Empty
                                                                    }}
                                                                });
        Assert.NotNull(dataResponse);
        return dataResponse!.permissions;
    }

    public async Task AddDocumentPermission(string token, Guid documentId, Guid holderId, DocumentPermission permission, bool expectsForbidden = false)
    {
        var dataRequest = new { documentId = documentId, holderId = holderId, permission = permission };
        var client = _factory.BuildClient(token);
        var response = await client.PostAsJsonAsync(URL_DOCUMENT_PERMISSION, dataRequest);
        if (expectsForbidden)
        {
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            return;
        }
        response.EnsureSuccessStatusCode();
    }

    public async Task DelDocumentPermission(string token, Guid documentId, Guid holderId, DocumentPermission permission, bool expectsForbidden = false)
    {
        var client = _factory.BuildClient(token);
        var response = await client.DeleteAsync($"{URL_DOCUMENT_PERMISSION}?documentId={documentId}&holderId={holderId}&permission={permission}");
        if (expectsForbidden)
        {
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            return;
        }
        response.EnsureSuccessStatusCode();
    }

    public enum DocumentPermission
    {
        Download,
        Manage
    }
}


