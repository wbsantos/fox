using System;
using API.Fox.Settings;
using API.Fox.EndPoint;

namespace API.Fox.AppBuilder;

public static class EndPointBuilder
{
    internal static WebApplication MapAppEndPoints(this WebApplication app)
    {
        Type endpointInterface = typeof(IEndPoint);

        IEnumerable<Type> endPointsImplementation =
                AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(a => a.GetTypes())
                         .Where(c => endpointInterface.IsAssignableFrom(c)
                                     && c.GetConstructor(Type.EmptyTypes) != null);

        foreach (var implementation in endPointsImplementation)
        {
            IEndPoint? implementationInstance = Activator.CreateInstance(implementation) as IEndPoint;
            
            switch (implementationInstance?.Verb)
            {
                case EndPoint.EndPointVerb.GET:
                    app.MapGet(implementationInstance.UrlPattern, implementationInstance.Method);
                    break;
                case EndPoint.EndPointVerb.POST:
                    app.MapPost(implementationInstance.UrlPattern, implementationInstance.Method);
                    break;
                case EndPoint.EndPointVerb.PUT:
                    app.MapPut(implementationInstance.UrlPattern, implementationInstance.Method);
                    break;
                case EndPoint.EndPointVerb.DELETE:
                    app.MapDelete(implementationInstance.UrlPattern, implementationInstance.Method);
                    break;
            }
        }
        return app;
	}
}

