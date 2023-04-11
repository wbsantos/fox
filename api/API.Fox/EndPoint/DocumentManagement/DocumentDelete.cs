using System;
using Fox.Dox.Repository;
using API.Fox.EndPoint;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentDelete : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_DELETION";
    public string UrlPattern => "/document";
    public EndPointVerb Verb => EndPointVerb.DELETE;
    public Delegate Method => (Guid documentId, DocumentRepository docRepo) =>
    {        
        if (docRepo.DeleteDocument(documentId))
            return Results.Ok();
        else
            return Results.Unauthorized();
    };
}

