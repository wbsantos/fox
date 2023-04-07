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
    private const string URL_DOCUMENT_DOWNLOAD = "/document/download";

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

    public async Task<dynamic?> GetDocumentInformation(string token, Guid documentId, bool expectExists)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_DOCUMENT}?documentId={documentId}");
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

    public async Task<byte[]> GetDocumentBinary(string token, Guid documentId)
    {
        var client = _factory.BuildClient(token);
        var response = await client.GetAsync($"{URL_DOCUMENT_DOWNLOAD}?documentId={documentId}");
        response.EnsureSuccessStatusCode();
        var dataResponse = await response.Content.ReadAsByteArrayAsync();
        Assert.NotNull(dataResponse);
        return dataResponse;
    }
}


