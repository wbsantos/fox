using System;
using Fox.Dox.Repository;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Repository;

namespace API.Fox.Modules.DocumentManagement;

public class DocumentUpdateEndPoint : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_UPDATE";
    public string UrlPattern => "/document";
    public EndPointVerb Verb => EndPointVerb.PUT;
    public Delegate Method => (DocumentUpdateData document, DocumentRepository docRepo) =>
    {
        try
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
        }
        catch (ArgumentException argumentNull)
        {
            return Results.Problem(title: argumentNull.Message, statusCode: 400);
        }
        catch (Exception)
        {
            return Results.Problem();
        }
    };
}

public class DocumentUpdateData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> MetadataToAdd { get; set; } = new Dictionary<string, string>();
    public string[] MetadataToRemove = Array.Empty<string>();
}
