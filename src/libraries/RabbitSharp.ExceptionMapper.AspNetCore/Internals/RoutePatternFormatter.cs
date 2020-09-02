using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing.Template;

namespace RabbitSharp.Diagnostics.AspNetCore.Internals
{
    /// <summary>
    /// Implements <see cref="IRoutePatternFormatter"/>.
    /// </summary>
    class RoutePatternFormatter : IRoutePatternFormatter
    {
        private static readonly RouteValueDictionary EmptyRouteValues = new RouteValueDictionary();
        private readonly TemplateBinderFactory _templateBinderFactory;

        public RoutePatternFormatter(TemplateBinderFactory templateBinderFactory)
        {
            _templateBinderFactory = templateBinderFactory;
        }

        public string? Format(HttpContext httpContext, RoutePattern pattern, object? routeValues)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            // Refer to
            // https://github.com/dotnet/aspnetcore/blob/master/src/Http/Routing/src/DefaultLinkGenerator.cs#L291
            var binder = _templateBinderFactory.Create(pattern);
            // Route values captured from previous endpoint
            var ambientValues = httpContext.Features.Get<IExceptionMappingFeature>().RouteValues;

            var valuesResult = binder.GetValues(ambientValues, routeValues == null
                ? EmptyRouteValues
                : new RouteValueDictionary(routeValues));
            if (valuesResult == null)
            {
                return null;
            }

            if (!binder.TryProcessConstraints(httpContext, valuesResult.CombinedValues, out var _, out var _))
            {
                return null;
            }

            return binder.BindValues(valuesResult.AcceptedValues);
        }
    }
}
