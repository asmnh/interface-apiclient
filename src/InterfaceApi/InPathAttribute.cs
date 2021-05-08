using System;

namespace InterfaceApiClient.DataTypes
{
    /// <summary>
    /// Mark attribute as being passed using query path. Will replace token with {propertyname} with serialized attribute value.
    /// </summary>
    /// <remarks>
    /// Attribute serialization is culture-invariant, uses <c>ToString</c>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class InPathAttribute : RequestAttribute
    {
        /// <summary>
        /// Mark attribute as part of query path. Attribute name will replace token {attributename} with serialized attribute value.
        /// </summary>
        /// <param name="name">Attribute name override.</param>
        public InPathAttribute(string? name = null) : base(name) { }
    }
}
