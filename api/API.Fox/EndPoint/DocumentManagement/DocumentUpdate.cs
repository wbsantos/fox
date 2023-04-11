using System;
using Fox.Dox.Repository;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentUpdate : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_UPDATE";
    public string UrlPattern => "/document";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (DocumentUpdateData document, DocumentRepository docRepo) =>
    {
        DocumentInformation documentModel = new DocumentInformation()
        {
            Id = document.Id,
            Name = document.Name
        };
        docRepo.UpdateDocument(documentModel);
        docRepo.DeleteMetadata(document.Id, document.MetadataToRemove);
        docRepo.AddMetadata(document.Id, document.MetadataToAdd);
        return Results.Ok();
    };
}

public class DocumentUpdateData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> MetadataToAdd { get; set; } = new Dictionary<string, string>();
    public string[] MetadataToRemove { get; set; } = Array.Empty<string>();
}
