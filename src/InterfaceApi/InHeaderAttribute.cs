using System;

namespace InterfaceApiClient.DataTypes
{
    public class InHeaderAttribute : RequestAttribute
    {
        /// <summary>
        /// Mark attribute as being passed as header. Will add parameter using <paramref name="headerName"/> as header name and <c>ToString</c> value of parameter as value.
        /// </summary>
        /// <param name="headerName">Header name to be used.</param>
        public InHeaderAttribute(string headerName) : base(headerName) { }

        public string HeaderName { get { return base.Name!; } }

        public override string? Name
        {
            get { return base.Name; }
            init { if (value is null) throw new ArgumentNullException(nameof(Name)); base.Name = value; }
        }
    }
}
