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
        try
        {
            if (docRepo.DeleteDocument(documentId))
                return Results.Ok();
            else
                return Results.Unauthorized();
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

