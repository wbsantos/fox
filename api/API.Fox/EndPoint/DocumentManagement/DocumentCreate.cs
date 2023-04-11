using System;
using Fox.Dox.Repository;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentCreate : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_CREATION";
    public string UrlPattern => "/document";
    public EndPointVerb Verb => EndPointVerb.POST;
    public Delegate Method => (DocumentCreateData document, DocumentRepository docRepo) =>
    {
        Document documentModel = new Document()
        {
            Name = document.Name,
            Metadata = document.Metadata,
            FileBinary = Convert.FromBase64String(document.FileBase64),
        };
        documentModel.FileSizeBytes = documentModel.FileBinary.Length;
        documentModel = docRepo.CreateDocument(documentModel);
        return Results.Ok(new DocumentInformation()
        {
            Id = documentModel.Id,
            Name = documentModel.Name,
            FileSizeBytes = documentModel.FileSizeBytes,
            Metadata = documentModel.Metadata
        });
    };
}

public class DocumentCreateData
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    public string FileBase64 { get; set; } = string.Empty;
}
