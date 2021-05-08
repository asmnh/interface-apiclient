using System;
using System.Collections.Generic;

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
        public ApiClientConfiguration UseEndpoint(string groupName, string baseUrl)
        {
            Endpoints[groupName] = baseUrl;
            return this;
        }
    }
}
