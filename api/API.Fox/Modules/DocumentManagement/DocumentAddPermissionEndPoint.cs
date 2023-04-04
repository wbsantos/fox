
using System;
using Fox.Dox.Repository;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Repository;
using Fox.Access.Model;

namespace API.Fox.Modules.DocumentManagement;

public class DocumentAddPermissionEndPoint : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_PERMISSION_ADDITION";
    public string UrlPattern => "/document/permission";
    public EndPointVerb Verb => EndPointVerb.POST;
    public Delegate Method => (DocumentAddPermissionData permissionData, DocumentRepository docRepo) =>
    {
        try
        {
            docRepo.AddPermission(permissionData.DocumentId, permissionData.HolderId, permissionData.Permission);
            return Results.Ok();
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

public class DocumentAddPermissionData
{
    public Guid DocumentId { get; set; }
    public Guid HolderId { get; set; }
    public DocumentPermission Permission { get; set; }
}
