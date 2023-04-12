
using System;
using Fox.Dox.Service;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Service;
using Fox.Access.Model;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentAddPermission : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_PERMISSION_ADDITION";
    public string UrlPattern => "/document/permission";
    public EndPointVerb Verb => EndPointVerb.POST;
    public Delegate Method => (DocumentAddPermissionData permissionData, DocumentService docService) =>
    {
        docService.AddPermission(permissionData.DocumentId, permissionData.HolderId, permissionData.Permission);
        return Results.Ok();
    };
}

public class DocumentAddPermissionData
{
    public Guid DocumentId { get; set; }
    public Guid HolderId { get; set; }
    public DocumentPermission Permission { get; set; }
}
