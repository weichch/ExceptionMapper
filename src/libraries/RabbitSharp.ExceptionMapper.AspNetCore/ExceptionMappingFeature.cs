using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Implements <see cref="IExceptionMappingFeature"/>.
    /// </summary>
    class ExceptionMappingFeature : IExceptionMappingFeature, IExceptionMappingContextFeature
    {
        public ExceptionHandlingContext? Context { get; set; }
        public Endpoint? Endpoint { get; set; }
        public RouteValueDictionary? RouteValues { get; set; }
        public RequestDelegate RequestPipeline { get; set; } = null!;
        public PathString RequestPath { get; set; }
        public PathString RequestPathBase { get; set; }
    }
}
