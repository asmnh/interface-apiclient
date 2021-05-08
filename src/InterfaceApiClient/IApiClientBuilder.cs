using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace InterfaceApiClient
{
    /// <summary>
    /// Builds Interface Api Client configuration
    /// </summary>
    public interface IApiClientBuilder
    {
        /// <summary>
        /// Adds all interfaces that have endpoint registration from assembly to be resolved as api clients.
        /// </summary>
        /// <param name="assembly">Assembly to be scanned for interface types.</param>
        /// <param name="configure">Configuration callback.</param>
        /// <returns>Self</returns>
        IApiClientBuilder WithTransientAssemblyTypes(Assembly assembly, Action<ApiClientConfiguration>? configure = null);
        /// <summary>
        /// Adds an interface to be registered as api client.
        /// </summary>
        /// <typeparam name="TInterface">Interface to be registered.</typeparam>
        /// <param name="configure">Configuration callback.</param>
        /// <returns>Self</returns>
        IApiClientBuilder WithTransientClient<TInterface>(Action<ApiClientConfiguration>? configure = null);
        /// <summary>
        /// Allows for configuration without adding any clients.
        /// </summary>
        /// <param name="configure">Configuration callback.</param>
        /// <returns>Self</returns>
        IApiClientBuilder WithConfiguration(Action<ApiClientConfiguration>? configure);
        /// <summary>
        /// Builds list of types to be registered from configuration and adds them to service collection.
        /// </summary>
        /// <returns>Service collection on which builder was originally created</returns>
        IServiceCollection RegisterAllDependencies();
    }
}
