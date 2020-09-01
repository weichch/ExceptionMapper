using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Represents a collection of registered exception mapping conventions.
    /// </summary>
    public class ExceptionMappingConventionCollection : Collection<ExceptionMappingConventionRegistration>
    {
        /// <summary>
        /// Adds a mapping convention of <typeparamref name="TConvention"/>.
        /// </summary>
        /// <typeparam name="TConvention">The type of the convention.</typeparam>
        public void Add<TConvention>() where TConvention : class
            => Add<TConvention>(null, 0);

        /// <summary>
        /// Adds a mapping convention of <typeparamref name="TConvention"/>.
        /// </summary>
        /// <typeparam name="TConvention">The type of the convention.</typeparam>
        /// <param name="tags">The user-defined tags on the convention.</param>
        public void Add<TConvention>(IEnumerable<string>? tags) where TConvention : class
            => Add<TConvention>(tags, 0);

        /// <summary>
        /// Adds a mapping convention of <typeparamref name="TConvention"/>.
        /// </summary>
        /// <typeparam name="TConvention">The type of the convention.</typeparam>
        /// <param name="order">The order of the convention.</param>
        public void Add<TConvention>(int order) where TConvention : class
            => Add<TConvention>(null, order);

        /// <summary>
        /// Adds a mapping convention of <typeparamref name="TConvention"/>.
        /// </summary>
        /// <typeparam name="TConvention">The type of the convention.</typeparam>
        /// <param name="tags">The user-defined tags on the convention.</param>
        /// <param name="order">The order of the convention.</param>
        public void Add<TConvention>(IEnumerable<string>? tags, int order) where TConvention : class
        {
            var registration = new ExceptionMappingConventionRegistration(
                typeof(TConvention),
                ActivatorUtilities.GetServiceOrCreateInstance<TConvention>,
                tags)
            {
                Order = order
            };

            Add(registration);
        }

        /// <summary>
        /// Adds a mapping convention instance.
        /// </summary>
        /// <param name="convention">The convention instance.</param>
        public void AddConvention(object convention)
            => AddConvention(convention, null, 0);

        /// <summary>
        /// Adds a mapping convention instance.
        /// </summary>
        /// <param name="convention">The convention instance.</param>
        /// <param name="tags">The user-defined tags on the convention.</param>
        public void AddConvention(object convention, IEnumerable<string>? tags)
            => AddConvention(convention, tags, 0);

        /// <summary>
        /// Adds a mapping convention instance.
        /// </summary>
        /// <param name="convention">The convention instance.</param>
        /// <param name="order">The order of the convention.</param>
        public void AddConvention(object convention, int order)
            => AddConvention(convention, null, order);

        /// <summary>
        /// Adds a mapping convention instance.
        /// </summary>
        /// <param name="convention">The convention instance.</param>
        /// <param name="tags">The user-defined tags on the convention.</param>
        /// <param name="order">The order of the convention.</param>
        public void AddConvention(object convention, IEnumerable<string>? tags, int order)
        {
            if (convention == null)
            {
                throw new ArgumentNullException(nameof(convention));
            }

            var registration = new ExceptionMappingConventionRegistration(
                convention.GetType(),
                _ => convention,
                tags)
            {
                Order = order
            };

            Add(registration);
        }

        /// <summary>
        /// Adds a mapping convention of <typeparamref name="TConvention"/> and passes custom parameters
        /// to the constructor of <typeparamref name="TConvention"/>.
        /// </summary>
        /// <typeparam name="TConvention">The type of the convention.</typeparam>
        /// <param name="parameters">The user-defined parameters.</param>
        public void AddParameterized<TConvention>(params object[] parameters)
            where TConvention : class
            => AddParameterized<TConvention>(null, 0, parameters);

        /// <summary>
        /// Adds a mapping convention of <typeparamref name="TConvention"/> and passes custom parameters
        /// to the constructor of <typeparamref name="TConvention"/>.
        /// </summary>
        /// <typeparam name="TConvention">The type of the convention.</typeparam>
        /// <param name="order">The order of the convention.</param>
        /// <param name="parameters">The user-defined parameters.</param>
        public void AddParameterized<TConvention>(int order, params object[] parameters)
            where TConvention : class
            => AddParameterized<TConvention>(null, order, parameters);

        /// <summary>
        /// Adds a mapping convention of <typeparamref name="TConvention"/> and passes custom parameters
        /// to the constructor of <typeparamref name="TConvention"/>.
        /// </summary>
        /// <typeparam name="TConvention">The type of the convention.</typeparam>
        /// <param name="tags">The user-defined tags on the convention.</param>
        /// <param name="parameters">The user-defined parameters.</param>
        public void AddParameterized<TConvention>(IEnumerable<string>? tags, params object[] parameters)
            where TConvention : class
            => AddParameterized<TConvention>(tags, 0, parameters);

        /// <summary>
        /// Adds a mapping convention of <typeparamref name="TConvention"/> and passes custom parameters
        /// to the constructor of <typeparamref name="TConvention"/>.
        /// </summary>
        /// <typeparam name="TConvention">The type of the convention.</typeparam>
        /// <param name="tags">The user-defined tags on the convention.</param>
        /// <param name="order">The order of the convention.</param>
        /// <param name="parameters">The user-defined parameters.</param>
        public void AddParameterized<TConvention>(IEnumerable<string>? tags, int order, params object[] parameters)
            where TConvention : class
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var registration = new ExceptionMappingConventionRegistration(
                typeof(TConvention),
                serviceProvider => ActivatorUtilities.CreateInstance<TConvention>(serviceProvider, parameters),
                tags)
            {
                Order = order
            };

            Add(registration);
        }

        /// <summary>
        /// Stops inserting <c>null</c>.
        /// </summary>
        protected override void InsertItem(int index, ExceptionMappingConventionRegistration item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Stops inserting <c>null</c>.
        /// </summary>
        protected override void SetItem(int index, ExceptionMappingConventionRegistration item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            base.SetItem(index, item);
        }
    }
}
