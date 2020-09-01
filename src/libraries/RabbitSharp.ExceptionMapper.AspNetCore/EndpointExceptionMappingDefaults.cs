namespace RabbitSharp.Diagnostics.AspNetCore
{
    /// <summary>
    /// Provides static/constant default values for endpoint exception mapping in ASP.NET Core application.
    /// </summary>
    public static class EndpointExceptionMappingDefaults
    {
        /// <summary>
        /// Gets the name of the exception mapping scheme for endpoints in ASP.NET Core application.
        /// </summary>
        public static readonly string EndpointScheme = "AspNetCoreEndpoint";
    }
}
