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
    public Delegate Method => async (Guid documentId, DocumentService docService) =>
    {
        DocumentInformation? info = docService.GetDocumentInformation(documentId);
        if (info == null)
            return Results.BadRequest();

        var fileStream = await docService.GetDocumentBinaryAsync(documentId);
        return Results.File(fileStream,
                            fileDownloadName: info.Name,
                            contentType: "application/octet-stream");
    };
}
