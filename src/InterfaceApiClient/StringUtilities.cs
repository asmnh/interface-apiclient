using System;

namespace InterfaceApiClient
{
    internal static class StringUtilities
    {
        internal static string ToCamelCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException($"Can't camelcase a string that has only whitespace", nameof(input));
            if (char.IsUpper(input[0]))
                return char.ToLower(input[0]) + input[1..];
            return input;
        }
    }
}