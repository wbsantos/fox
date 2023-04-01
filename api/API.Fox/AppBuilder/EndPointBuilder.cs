using System;
using API.Fox.Modules;
using API.Fox.Settings;
using API.Fox.EndPoint;

namespace API.Fox.AppBuilder;

public static class EndPointBuilder
{
    internal static WebApplication MapAppEndPoints(this WebApplication app)
    {
        Type endpointInterface = typeof(IEndPointAnonymous);

        IEnumerable<Type> endPointsImplementation =
                ModuleReferences.GetAssemblies()
                         .SelectMany(a => a.GetTypes())
                         .Where(c => endpointInterface.IsAssignableFrom(c)
                                     && c.GetConstructor(Type.EmptyTypes) != null);

        foreach (var implementation in endPointsImplementation)
        {
            IEndPointAnonymous? implementationInstance = Activator.CreateInstance(implementation) as IEndPointAnonymous;
            RouteHandlerBuilder? handler = null;
            switch (implementationInstance?.Verb)
            {
                case EndPoint.EndPointVerb.GET:
                    handler = app.MapGet(implementationInstance.UrlPattern, implementationInstance.Method);
                    break;
                case EndPoint.EndPointVerb.POST:
                    handler = app.MapPost(implementationInstance.UrlPattern, implementationInstance.Method);
                    break;
                case EndPoint.EndPointVerb.PUT:
                    handler = app.MapPut(implementationInstance.UrlPattern, implementationInstance.Method);
                    break;
                case EndPoint.EndPointVerb.DELETE:
                    handler = app.MapDelete(implementationInstance.UrlPattern, implementationInstance.Method);
                    break;
            }
            if(handler != null && implementationInstance is IEndPoint endPointAuthorize)
                handler.RequireAuthorization(endPointAuthorize.PermissionClaim);
        }
        return app;
	}
}

