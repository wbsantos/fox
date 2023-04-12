using System;
using Fox.Dox.Service;
using API.Fox.EndPoint;
using Fox.Access.Service;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentDelete : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_DELETION";
    public string UrlPattern => "/document";
    public EndPointVerb Verb => EndPointVerb.DELETE;
    public Delegate Method => (Guid documentId, DocumentService docService) =>
    {        
        if (docService.DeleteDocument(documentId))
            return Results.Ok();
        else
            return Results.Unauthorized();
    };
}

