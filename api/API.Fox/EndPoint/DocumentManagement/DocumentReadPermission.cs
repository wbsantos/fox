using System;
using Fox.Dox.Service;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Service;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentReadPermission : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_PERMISSION_READ";
    public string UrlPattern => "/document/permission";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (Guid documentId, DocumentService docService) =>
    {
        IEnumerable<DocumentHolder> permissions = docService.GetPermissionByDocument(documentId);
        return Results.Ok(new { permissions = permissions });
    };
}
