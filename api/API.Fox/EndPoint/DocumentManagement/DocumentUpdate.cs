using System;
using Fox.Dox.Service;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Service;
using System.ComponentModel.DataAnnotations;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentUpdate : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_UPDATE";
    public string UrlPattern => "/document";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (DocumentUpdateData document, DocumentService docService) =>
    {
        DocumentInformation documentModel = new DocumentInformation()
        {
            Id = document.Id,
            Name = document.Name
        };
        docService.UpdateDocument(documentModel);
        docService.DeleteMetadata(document.Id, document.MetadataToRemove);
        docService.AddMetadata(document.Id, document.MetadataToAdd);
        return Results.Ok();
    };
}

public class DocumentUpdateData
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> MetadataToAdd { get; set; } = new Dictionary<string, string>();
    public string[] MetadataToRemove { get; set; } = Array.Empty<string>();
}
