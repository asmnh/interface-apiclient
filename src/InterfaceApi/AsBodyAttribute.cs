using System;

namespace InterfaceApiClient.DataTypes
{
    /// <summary>
    /// Mark parameter as being the request body. Will add all properties except ones marked as skipped, body will be sent as JSON object.
    /// If multiple attributes are being marked using this attribute, resulting body will be merger of all attributes, older ones overwriting newer ones.
    /// </summary>
    /// <remarks>
    /// If any IEnumerable is marked using this attribute, body will be an array containing all elements.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class AsBodyAttribute : Attribute
    {
        /// <summary>
        /// Properties skipped when adding parameter to body.
        /// </summary>
        /// <remarks>
        /// If any IEnumerable is marked using this attribute, property should point out to element property that is being skipped.
        /// </remarks>
        public string[] SkipProperties { get; private set; }

        /// <summary>
        /// Mark parameter as being the request body. Will add all properties except ones marked as skipped, body will be sent as JSON object.
        /// If multiple attributes are being marked using this attribute, resulting body will be merger of all attributes, older ones overwriting newer ones.
        /// </summary>
        /// <param name="skipProperties">Property names to be skipped. If marking an IEnumerable, this should point to properties in each member.</param>
        public AsBodyAttribute(params string[] skipProperties)
        {
            SkipProperties = skipProperties;
        }
    }
}
