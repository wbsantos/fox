using System;
using Fox.Dox.Repository;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentReadPermission : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_PERMISSION_READ";
    public string UrlPattern => "/document/permission";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid documentId, DocumentRepository docRepo) =>
    {
        try
        {
            IEnumerable<DocumentHolder> permissions = docRepo.GetPermissionByDocument(documentId);
            return Results.Ok(new { permissions = permissions });
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
