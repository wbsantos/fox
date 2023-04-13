using System;
using Fox.Dox.Service;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Service;
using System.ComponentModel.DataAnnotations;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentCreate : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_CREATION";
    public string UrlPattern => "/document";
    public EndPointVerb Verb => EndPointVerb.POST;
    public Delegate Method => async (DocumentCreateData document, DocumentService docService) =>
    {
        DocumentInformation documentModel = new DocumentInformation()
        {
            Name = document.Name,
            Metadata = document.Metadata
        };

        using (var fileToCreate = new MemoryStream(Convert.FromBase64String(document.FileBase64)))
        {
            var documentCreated = await docService.CreateDocumentAsync(documentModel, fileToCreate);
            return Results.Ok(documentCreated);
        }
    };
}

public class DocumentCreateData
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    [Required]
    public string FileBase64 { get; set; } = string.Empty;
}
