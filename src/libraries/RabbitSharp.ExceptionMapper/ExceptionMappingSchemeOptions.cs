using System;
using Microsoft.Extensions.Options;

namespace RabbitSharp.Diagnostics
{
    /// <summary>
    /// Defines settings for mapping convention based exception mapping scheme.
    /// </summary>
    public class ExceptionMappingSchemeOptions
    {
        private string _name;

        /// <summary>
        /// Creates an instance of the options.
        /// </summary>
        public ExceptionMappingSchemeOptions()
        {
            Conventions = new ExceptionMappingConventionCollection();
            _name = Options.DefaultName;
        }

        /// <summary>
        /// Gets or sets name of the scheme using this options object.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets a collection of registered exception mapping conventions.
        /// </summary>
        public ExceptionMappingConventionCollection Conventions { get; }
    }
}
