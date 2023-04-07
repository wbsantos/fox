using System;
using Fox.Dox.Repository;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Repository;
using Fox.Access.Model;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentDelPermission : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_PERMISSION_REMOVAL";
    public string UrlPattern => "/document/permission";
    public EndPointVerb Verb => EndPointVerb.DELETE;
    public Delegate Method => (Guid documentId, Guid holderId, DocumentPermission permission, LoggedUser user, DocumentRepository docRepo) =>
    {
        try
        {
            docRepo.DelPermission(documentId, holderId, permission);
            return Results.Ok();
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