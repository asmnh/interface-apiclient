using System;
using System.Collections.Generic;
using System.Linq;

namespace InterfaceApiClient
{
    public class ApiClientConfiguration
    {
        internal IDictionary<string, string> Endpoints { get; }

        public ApiClientConfiguration()
        {
            Endpoints = new Dictionary<string, string>();
        }

        /// <summary>
        /// Use endpoint for given interface.
        /// </summary>
        /// <typeparam name="T">Interface type - reads attribute or uses its interface name as group name.</typeparam>
        /// <param name="baseUrl">Base URL for endpoint calls.</param>
        public ApiClientConfiguration UseEndpoint<T>(string baseUrl) => UseEndpoint(AttributeReader.GetGroupName(typeof(T)), baseUrl);

        internal void ConfigureInterface(Type interfaceType)
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        /// Use endpoint for given group.
        /// </summary>
        /// <param name="groupName">Group name to set endpoint base URL for.</param>
        /// <param name="baseUrl">Base URL for group.</param>
        /// <returns>Self</returns>
        public ApiClientConfiguration UseEndpoint(string groupName, string baseUrl)
        {
            Endpoints[groupName] = baseUrl;
            return this;
        }

        /// <summary>
        /// Remove endpoint from list of registered groups.
        /// </summary>
        /// <param name="groupName">Group name for which endpoint should be removed</param>
        /// <returns>Self</returns>
        public ApiClientConfiguration RemoveEndpoint(string groupName)
        {
            Endpoints.Remove(groupName);
            return this;
        }

        /// <summary>
        /// Removes endpoint from list of registered groups.
        /// </summary>
        /// <typeparam name="TInterface">Interface for which group should be removed.</typeparam>
        /// <returns></returns>
        public ApiClientConfiguration RemoveEndpoint<TInterface>() => RemoveEndpoint(AttributeReader.GetGroupName(typeof(TInterface)));

        /// <summary>
        /// Verifies configuration.
        /// </summary>
        /// <exception cref="UriFormatException">One or more of groups has malformed URI</exception>
        public ApiClientConfiguration Verify()
        {
            VerifyGroupsUriFormat();
            return this;
        }

        private void VerifyGroupsUriFormat()
        {
            var malformedGroups = Endpoints
                            .Where(endpoint => !IsValidHttpUriString(endpoint.Value))
                            .ToArray();
            if (malformedGroups.Any())
                throw new UriFormatException($"Malformed URI strings in groups configuration: {string.Join(", ", FormatException(malformedGroups))}");
        }

        private static bool IsValidHttpUriString(string input)
        {
            return Uri.TryCreate(input, UriKind.Absolute, out Uri? uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        private static IEnumerable<string> FormatException(KeyValuePair<string, string>[]? values)
        {
            return values?.Select(kvp => $"{kvp.Key} => {kvp.Value}")?? Array.Empty<string>();
        }
    }
}
