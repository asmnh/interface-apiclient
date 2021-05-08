using System;

namespace InterfaceApiClient.DataTypes
{
    /// <summary>
    /// Marks API group.
    /// </summary>
    /// <remarks>
    /// API group is used to group multiple API endpoints sharing single endpoint destination.
    /// Group endpoint URL is configured during API client or API configuration injection.
    /// If not present, interface assembly name will be used as API group.
    /// 
    /// Setting group on an interface will apply to all methods present.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ApiGroupAttribute : Attribute
    {
        /// <summary>
        /// API group name - name will be used to resolve configuration with endpoint URI.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Marks API group.
        /// </summary>
        /// <param name="name">Group name - used to resolve endpoint URI from configuration.</param>
        public ApiGroupAttribute(string name)
        {
            Name = name;
        }
    }
}
