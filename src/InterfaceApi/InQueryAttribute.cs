using System;

namespace InterfaceApiClient.DataTypes
{
    /// <summary>
    /// Mark parameter as being passed using query string. Will add attribute with property name to query string, using serialized attribute value as value.
    /// </summary>
    /// <remarks>
    /// Parameter serialization is culture invariant. Serialization uses <c>ToString</c>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class InQueryAttribute : RequestAttribute
    {
        /// <summary>
        /// Mark attribute as being passed using query string. Will add ?propertyname=propertyvalue to query string, using stringified parameter value as propertyvalue.
        /// </summary>
        /// <param name="name">Property naem override - if not present, parameter name will be used.</param>
        public InQueryAttribute(string? name = null) : base(name) { }
    }
}
