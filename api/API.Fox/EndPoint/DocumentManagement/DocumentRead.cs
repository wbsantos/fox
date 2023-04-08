using System;
using Fox.Dox.Repository;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentRead : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_READ";
    public string UrlPattern => "/document";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid documentId, DocumentRepository docRepo) =>
    {
        try
        {
            DocumentInformation? document = docRepo.GetDocumentInformation(documentId);
            return Results.Ok(document);
        }
        catch (UnauthorizedAccessException)
        {
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
