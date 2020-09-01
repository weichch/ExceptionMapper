# ExceptionMapper

_I'm still working on the docs, but here is some examples:_

```csharp
private void ConfigureServices(IServiceCollection services)
{
    services.AddExceptionMapping()
        .AddEndpointResponse(scheme =>
        {
            // Map InvalidOperationException to HTTP 402 without response body
            scheme.MapToStatusCode<InvalidOperationException>(StatusCodes.Status402PaymentRequired);

            // Map InvalidOperationException to another endpoint at "/error",
            // with custom tags defined on the convention
            scheme.MapToPath<InvalidOperationException>("/error", tags: new[] {"content"});

            // Map InvalidOperationException to a request delegate
            scheme.MapToRequestDelegate<InvalidOperationException>(
                async httpContext =>
                {

                });
        });
}
```

```csharp
private void Configure(IApplicationBuilder app, string expectedMessage)
{
    // Add exception mapping middleware
    // Exception mapping uses built-in exception handler middleware internally
    app.UseEndpointExceptionMapping();

    // Routing is required by built-in exception handler middleware
    app.UseRouting();

    // Add some dummy endpoints
    app.UseEndpoints(endpoints =>
    {
        // Add an endpoint which throws InvalidOperationException
        // Specifies when mapping exception from this endpoint,
        // only use conventions with "content" tag defined
        endpoints.MapGet("/throw", async httpContext =>
        {
            await Task.Yield();
            throw new InvalidOperationException(expectedMessage);
        }).MapException("content");

        // Add an endpoint to re-execute the errored request
        // Also exclude this endpoint from exception mapping
        endpoints.MapGet("/error", async httpContext =>
        {
            // Data context for exception mapper is available
            // in IExceptionMappingContextFeature
            var feature = httpContext.Features.Get<IExceptionMappingContextFeature>();
            httpContext.Response.StatusCode = StatusCodes.Status402PaymentRequired;
            await httpContext.Response.WriteAsync(feature.Context!.Exception.Message);
        }).ExcludeFromExceptionMapping();
    });
}
```