using InterfaceApiClient.DataTypes;
using System;
using System.Linq;
using System.Reflection;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("InterfaceApiClient.Tests")]
namespace InterfaceApiClient
{
    internal class AttributeReader
    {
        internal static string GetGroupName(Type type) => 
            type.GetCustomAttribute<ApiGroupAttribute>()?.Name 
                ?? type.Name;

        internal static string GetGroupName(MethodInfo method) => 
            method.GetCustomAttribute<ApiGroupAttribute>()?.Name 
                ?? GetGroupName(method.DeclaringType!);
    }
}