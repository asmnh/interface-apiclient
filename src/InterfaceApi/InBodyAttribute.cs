using System;

namespace InterfaceApiClient.DataTypes
{
    /// <summary>
    /// Mark attribute as being passed using request body. Will add attribute to body object, using parameter name as body element name.
    /// </summary>
    /// <remarks>
    /// Body will be serialized to JSON. Datatype may be lost.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class InBodyAttribute : RequestAttribute
    {
        /// <summary>
        /// Mark attribute as being passed as part of request body. Will add parameter to body object, using parameter name as body element name.
        /// </summary>
        /// <param name="name">Parameter name override - if not present, will use parameter name from interface.</param>
        /// <remarks>
        /// Body attribute will be serialized to JSON. Datatype may be lost.
        /// </remarks>
        public InBodyAttribute(string? name) : base(name) { }
    }
}
