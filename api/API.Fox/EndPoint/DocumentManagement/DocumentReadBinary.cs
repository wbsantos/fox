using System;
using Fox.Dox.Repository;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Repository;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentReadBinary : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_READ";
    public string UrlPattern => "/document/download";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid documentId, DocumentRepository docRepo) =>
    {
        try
        {
            DocumentInformation? info = docRepo.GetDocumentInformation(documentId);
            if (info == null)
                return Results.BadRequest();

            byte[] file = docRepo.GetDocumentBinary(documentId);
            return Results.File(file,
                                fileDownloadName: info.Name,
                                contentType: "application/octet-stream");
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
