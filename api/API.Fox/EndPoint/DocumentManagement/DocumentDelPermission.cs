using System;
using Fox.Dox.Service;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Service;
using Fox.Access.Model;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentDelPermission : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_PERMISSION_REMOVAL";
    public string UrlPattern => "/document/permission";
    public EndPointVerb Verb => EndPointVerb.DELETE;
    public Delegate Method => (Guid documentId, Guid holderId, DocumentPermission permission, LoggedUser user, DocumentService docService) =>
    {
        docService.DelPermission(documentId, holderId, permission);
        return Results.Ok();
    };
}