
using System;
using Fox.Dox.Service;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Service;
using Fox.Access.Model;
using System.ComponentModel.DataAnnotations;

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
    [Required]
    public Guid DocumentId { get; set; }
    [Required]
    public Guid HolderId { get; set; }
    [Required]
    public DocumentPermission Permission { get; set; }
}
