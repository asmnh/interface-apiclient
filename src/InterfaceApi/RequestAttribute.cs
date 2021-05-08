using System;

namespace InterfaceApiClient.DataTypes
{
    /// <summary>
    /// Generic request attribute - points out attribute data to be passed in specific way to API.
    /// </summary>
    public abstract class RequestAttribute : Attribute
    {
        /// <summary>
        /// Attribute name override - overrides parameter name if present.
        /// </summary>
        public virtual string? Name { get; init; }

        /// <summary>
        /// Optional property path - dot-separated path to property being used as attribute value instead of using whole attribute.
        /// </summary>
        public string? Property { get; init; }

        /// <summary>
        /// Base constructor for <see cref="RequestAttribute"/> - all attributes require names, regardless of how they will be finally passed.
        /// </summary>
        /// <param name="name">Attribute name - overrides parameter name.</param>
        protected RequestAttribute(string? name = null)
        {
            Name = name;
        }
    }
}
