using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace RabbitSharp.Diagnostics.Builder
{
    /// <summary>
    /// Provides extension methods to <see cref="ExceptionMappingBuilder"/>.
    /// </summary>
    public static class ExceptionMappingBuilderExtensions
    {
        /// <summary>
        /// Updates lifetime of <see cref="IExceptionMapper"/> service.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="lifetime">The new lifetime of <see cref="IExceptionMapper"/> service.</param>
        public static ExceptionMappingBuilder SetMapperLifetime(
            this ExceptionMappingBuilder builder,
            ServiceLifetime lifetime)
        {
            var serviceDescriptor = builder.Services.FirstOrDefault(
                sd => sd.ServiceType == typeof(IExceptionMapper));
            if (serviceDescriptor == null)
            {
                throw new InvalidOperationException("No registered 'IExceptionMapper' found.");
            }

            if (serviceDescriptor.Lifetime == lifetime
                || serviceDescriptor.ImplementationInstance != null)
            {
                return builder;
            }

            ServiceDescriptor? newServiceDescriptor = null;
            if (serviceDescriptor.ImplementationType != null)
            {
                newServiceDescriptor = new ServiceDescriptor(typeof(IExceptionMapper),
                    serviceDescriptor.ImplementationType, lifetime);
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                newServiceDescriptor = new ServiceDescriptor(typeof(IExceptionMapper),
                    serviceDescriptor.ImplementationFactory, lifetime);
            }

            if (newServiceDescriptor != null)
            {
                builder.Services.Replace(newServiceDescriptor);
            }

            return builder;
        }

        /// <summary>
        /// Adds an exception mapping scheme of <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of the scheme implementer.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="name">The name of the scheme to be added.</param>
        public static ExceptionMappingBuilder AddScheme<THandler>(
            this ExceptionMappingBuilder builder,
            string name)
            where THandler : IExceptionHandler
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            builder.AddScheme(new ExceptionMappingSchemeRegistration(
                name, typeof(THandler),
                serviceProvider => ActivatorUtilities.GetServiceOrCreateInstance<THandler>(serviceProvider)));

            return builder;
        }

        /// <summary>
        /// Adds an exception mapping scheme which uses strongly-typed settings of <typeparamref name="TOptions"/>
        /// and a set of mapping conventions of <typeparamref name="TConventionService"/> to handle exception.
        /// </summary>
        /// <typeparam name="TOptions">The type of the settings consumed by the scheme.</typeparam>
        /// <typeparam name="TConventionService">The mapping convention service type.</typeparam>
        /// <typeparam name="THandler">The type of the scheme implementer.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="name">The name of the scheme to be added.</param>
        public static ExceptionMappingBuilder AddScheme<TOptions, TConventionService, THandler>(
            this ExceptionMappingBuilder builder,
            string name)
            where TOptions : ExceptionMappingSchemeOptions, new()
            where TConventionService : class
            where THandler : ExceptionHandler<TOptions, TConventionService>
            => builder.AddScheme<TOptions, TConventionService, THandler>(name, _ => { });

        /// <summary>
        /// Adds an exception mapping scheme which uses strongly-typed settings of <typeparamref name="TOptions"/>
        /// and a set of mapping conventions of <typeparamref name="TConventionService"/> to handle exception.
        /// </summary>
        /// <typeparam name="TOptions">The type of the settings consumed by the scheme.</typeparam>
        /// <typeparam name="TConventionService">The mapping convention service type.</typeparam>
        /// <typeparam name="THandler">The type of the scheme implementer.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="name">The name of the scheme to be added.</param>
        /// <param name="configure">The action to configure scheme options.</param>
        public static ExceptionMappingBuilder AddScheme<TOptions, TConventionService, THandler>(
            this ExceptionMappingBuilder builder,
            string name,
            Action<TOptions> configure)
            where TOptions : ExceptionMappingSchemeOptions, new()
            where TConventionService : class
            where THandler : ExceptionHandler<TOptions, TConventionService>
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.Services.Configure<TOptions>(name, opt => opt.SchemeName = name);
            builder.AddScheme<THandler>(name);
            builder.Services.Configure(name, configure);

            return builder;
        }

        /// <summary>
        /// Adds an exception mapping scheme of <typeparamref name="THandler"/> and passes custom parameters to the
        /// constructor of <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="THandler">The type of the scheme implementer.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="name">The name of the scheme to be added.</param>
        /// <param name="parameters">The custom parameters to pass into the handler.</param>
        public static ExceptionMappingBuilder AddParameterizedScheme<THandler>(
            this ExceptionMappingBuilder builder,
            string name,
            params object[] parameters)
            where THandler : IExceptionHandler
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            builder.AddScheme(new ExceptionMappingSchemeRegistration(
                name, typeof(THandler),
                serviceProvider => ActivatorUtilities.CreateInstance<THandler>(serviceProvider, parameters)));

            return builder;
        }

        /// <summary>
        /// Adds an exception mapping scheme which uses strongly-typed settings of <typeparamref name="TOptions"/>
        /// and a set of mapping conventions of <typeparamref name="TConventionService"/> to handle exception. This
        /// method passes custom parameters to the constructor of <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TOptions">The type of the settings consumed by the scheme.</typeparam>
        /// <typeparam name="TConventionService">The mapping convention service type.</typeparam>
        /// <typeparam name="THandler">The type of the scheme implementer.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="name">The name of the scheme to be added.</param>
        /// <param name="parameters">The custom parameters to pass into the handler.</param>
        public static ExceptionMappingBuilder AddParameterizedScheme<TOptions, TConventionService, THandler>(
            this ExceptionMappingBuilder builder,
            string name,
            params object[] parameters)
            where TOptions : ExceptionMappingSchemeOptions, new()
            where TConventionService : class
            where THandler : ExceptionHandler<TOptions, TConventionService>
            => builder.AddParameterizedScheme<TOptions, TConventionService, THandler>(name, _ => { }, parameters);

        /// <summary>
        /// Adds an exception mapping scheme which uses strongly-typed settings of <typeparamref name="TOptions"/>
        /// and a set of mapping conventions of <typeparamref name="TConventionService"/> to handle exception. This
        /// method passes custom parameters to the constructor of <typeparamref name="THandler"/>.
        /// </summary>
        /// <typeparam name="TOptions">The type of the settings consumed by the scheme.</typeparam>
        /// <typeparam name="TConventionService">The mapping convention service type.</typeparam>
        /// <typeparam name="THandler">The type of the scheme implementer.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="name">The name of the scheme to be added.</param>
        /// <param name="configure">The action to configure scheme options.</param>
        /// <param name="parameters">The custom parameters to pass into the handler.</param>
        public static ExceptionMappingBuilder AddParameterizedScheme<TOptions, TConventionService, THandler>(
            this ExceptionMappingBuilder builder,
            string name,
            Action<TOptions> configure,
            params object[] parameters)
            where TOptions : ExceptionMappingSchemeOptions, new()
            where TConventionService : class
            where THandler : ExceptionHandler<TOptions, TConventionService>
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            builder.Services.Configure<TOptions>(name, opt => opt.SchemeName = name);
            builder.AddParameterizedScheme<THandler>(name, parameters);
            builder.Services.Configure(name, configure);

            return builder;
        }
    }
}
