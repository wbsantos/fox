using System;
namespace API.Fox.EndPoint;

public interface IEndPoint
{
    public string UrlPattern { get; }
    public EndPointVerb Verb { get; }
    public Delegate Method { get; }
}

public enum EndPointVerb
{
    GET,
    POST,
    PUT,
    DELETE
}

