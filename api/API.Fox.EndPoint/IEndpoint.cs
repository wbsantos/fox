using System;
namespace API.Fox.EndPoint;

public interface IEndPointAnonymous
{
    public string UrlPattern { get; }
    public EndPointVerb Verb { get; }
    public Delegate Method { get; }
}

public interface IEndPoint : IEndPointAnonymous
{
    public string PermissionClaim { get; }
}

public enum EndPointVerb
{
    GET,
    POST,
    PUT,
    DELETE
}

