using System;
using Fox.Dox.Service;
using Fox.Dox.Model;
using API.Fox.EndPoint;
using Fox.Access.Service;

namespace API.Fox.EndPoint.DocumentManagement;

public class DocumentReadAll : IEndPoint
{
    public string PermissionClaim => "DOCUMENT_READ";
    public string UrlPattern => "/document/all";
    public EndPointVerb Verb => EndPointVerb.GET;
    public Delegate Method => (DocumentService docService) =>
    {
        IEnumerable<DocumentInformation> documents = docService.GetAllDocuments();
        return Results.Ok(new { documents = documents });
    };
}
