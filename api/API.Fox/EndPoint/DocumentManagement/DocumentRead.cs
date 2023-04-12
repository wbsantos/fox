using System;
using Fox.Dox.Service;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Service;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentRead : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_READ";
    public string UrlPattern => "/document";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid documentId, DocumentService docService) =>
    {
        DocumentInformation? document = docService.GetDocumentInformation(documentId);
        return Results.Ok(document);
    };
}
