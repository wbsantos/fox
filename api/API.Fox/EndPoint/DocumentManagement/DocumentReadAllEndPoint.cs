using System;
using Fox.Dox.Repository;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentReadAll : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_READ";
    public string UrlPattern => "/document/all";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (DocumentRepository docRepo) =>
    {
        try
        {
            IEnumerable<DocumentInformation> documents = docRepo.GetAllDocuments();
            return Results.Ok(new { documents = documents });
        }
        catch (UnauthorizedAccessException)
        {
            return Results.Forbid();
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
