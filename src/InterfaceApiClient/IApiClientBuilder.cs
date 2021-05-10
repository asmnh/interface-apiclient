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
        /// Use this method to finalize service registration for your module or application.
        /// </summary>
        /// <returns>Service collection on which builder was originally created</returns>
        /// <remarks>
        /// Service collection stays untouched until after builder verified all registered services and is able to build a proxy for everything.
        /// If any error occurs during proxy building, whole process is being aborted and an exception will be thrown.
        /// If you're able to fix proxy configuration at runtime, you can attempt to re-apply configuration.
        /// </remarks>
        /// <exception cref="InvalidMetadataException">One or more of registered proxies has invalid metadata that makes it impossible to build API client.</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">One or more required API groups were not configured</exception>
        /// <exception cref="UriFormatException">One or more groups have invalid URI</exception>
        // TODO: include exceptions that ApiClientConfiguration can throw.
        IServiceCollection Apply();
    }
}
