# ExceptionMapper

[![Build status](https://ci.appveyor.com/api/projects/status/g7bnt6o9s2vb6hwq?svg=true)](https://ci.appveyor.com/project/weichch/exceptionmapper)

ExceptionMapper is an extension of _Microsoft.Extensions.DependencyInjection_ which provides ability to build exception handler with strongly-typed settings and conventions.

ExceptionMapper is licensed under the MIT license, so you can feel free to use it in your projects.

## Installation

You can install the latest NuGet packages from [nuget.org](https://www.nuget.org/packages/RabbitSharp.ExceptionMapper/). For ASP.NET Core applications, install the [latest ASP.NET Core package](https://www.nuget.org/packages/RabbitSharp.ExceptionMapper.AspNetCore/).

Also available via console commands:

```
> dotnet add package RabbitSharp.ExceptionMapper
> dotnet add package RabbitSharp.ExceptionMapper.AspNetCore
```

## Basic Usage

Add exception mapping and mapping schemes (exception handlers) to your dependency injection container, and then use `IExceptionMapper` in your services.

### Add to Container

```csharp
serviceCollection
    // Add core services
    .AddExceptionMapping()
    // Add ASP.NET Core handler
    .AddEndpointResponse();
```

### Add Mapping Conventions

Mapping conventions are configured per scheme. For example, for ASP.NET Core scheme, you can add conventions using fluent convention builder:

```csharp
services.AddExceptionMapping()
    .AddEndpointResponse(scheme =>
    {
        scheme.MapEndpointExceptions(conventions =>
        {
            // Map exception to scheme.DefaultConvention
            conventions.MapException(_ => true);

            // Map exception to HTTP response with status code
            conventions.MapException<InvalidOperationException>()
                .ToStatusCode(StatusCodes.Status402PaymentRequired);

            // Map exception to custom request handler
            conventions.MapException<InvalidOperationException>()
                .ToRequestHandler(async httpContext => { });

            // Map exception to an endpoint at path, and format
            // path with route values
            conventions.MapException<InvalidOperationException>()
                .ToEndpoint("/error/{value}/{custom}", new {custom = "xyz"});

            // Map exception to HTTP response and writes ProblemDetails
            // to the response body
            conventions.MapException<InvalidOperationException>()
                .ToProblemDetails(ctx => ctx.Factory.CreateProblemDetails(
                    ctx.HttpContext, StatusCodes.Status400BadRequest));
        });
    });
```

### Filter Conventions

You can use custom tags to filter conventions. For example, for ASP.NET Core scheme, you can filter conventions using `MapExceptionAttribute`:

```csharp
services.AddExceptionMapping()
    .AddEndpointResponse(scheme =>
    {
        scheme.MapEndpointExceptions(conventions =>
        {
            // Add tags to a convention
            conventions.MapException(_ => true).UseTags("default");
        });
    });
```

In your controller:

```csharp
class MyController : Controller
{
    [MapException]
    public IActionResult UseAnyConventions() { }

    [MapException("default")]
    public IActionResult UseDefaultConvention() { }

    [ExcludeFromExceptionMapping]
    public IActionResult DoesNotUseAnyConvention() { }
}
```

### Use `IExceptionMapper`

```csharp
// Inject IExceptionMapper into controller and handle exception manually
class MyController : Controller
{
    private readonly IExceptionMapper _exceptionMapper;

    public MyController(IExceptionMapper exceptionMapper)
    {
        _exceptionMapper = exceptionMapper;
    }

    public async Task<IActionResult> Get()
    {
        try
        {
            throw new Exception();
        }
        catch (Exception ex)
        {
            var result = await _exceptionMapper.MapAsync(ex);
            throw;
        }
    }
}
```

### Use ASP.NET Core Middleware

```csharp
void Configure(IApplicationBuilder app)
{
    // Add exception mapping middleware
    app.UseEndpointExceptionMapping();

    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

## Roadmap

### Features

- Map to named route
- Map to named request pipeline
- Define mapping via metadata attributes
- Exception filter attributes using `IExceptionMapper`

### Project

- Tidy up this `README.md`
- Add advanced documentation
- Add more tests
- Set up CI/CD
- Set up contribution documentation