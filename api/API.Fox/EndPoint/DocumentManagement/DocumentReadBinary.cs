using System;
using Fox.Dox.Service;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Service;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentReadBinary : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_READ";
    public string UrlPattern => "/document/download";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid documentId, DocumentService docService) =>
    {
        DocumentInformation? info = docService.GetDocumentInformation(documentId);
        if (info == null)
            return Results.BadRequest();

        byte[] file = docService.GetDocumentBinary(documentId);
        return Results.File(file,
                            fileDownloadName: info.Name,
                            contentType: "application/octet-stream");
    };
}
