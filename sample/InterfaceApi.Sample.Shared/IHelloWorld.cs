using InterfaceApiClient.DataTypes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace InterfaceApi.Sample.Shared
{
    /// <summary>
    /// Sample interop interface.
    /// </summary>
    /// <remarks>
    /// [ApiGroup] is not required here - will use interface name as group name.
    /// </remarks>
    public interface IHelloWorld
    {
        /// <summary>
        /// Takes name and returns welcome message.
        /// </summary>
        /// <param name="toWhom">To whom hello will be said.</param>
        /// <returns>Welcome message</returns>
        [ApiEndpoint(HttpRequestMethod.POST, "hello/{toWhom}")]
        [MapError(System.Net.HttpStatusCode.NotFound, typeof(KeyNotFoundException))]
        [MapError(System.Net.HttpStatusCode.Forbidden, typeof(ArgumentException))]
        Task<string> SayHello([InPath] string toWhom);

        /// <summary>
        /// Takes name and returns welcome message.
        /// </summary>
        /// <param name="toWhom">To whom hello will be said.</param>
        /// <param name="language">Language to be used.</param>
        /// <returns>Welcome message</returns>
        [ApiEndpoint(HttpRequestMethod.POST, "hello/{toWhom}")]
        [MapError(System.Net.HttpStatusCode.NotFound, typeof(KeyNotFoundException))]
        [MapError(System.Net.HttpStatusCode.Forbidden, typeof(ArgumentException))]
        Task<string> SayHello([InPath] string toWhom, [InQuery(Name = "lang")] string? language = null);

        [ApiEndpoint(HttpRequestMethod.POST, "hello2/{name}")]
        [MapError(System.Net.HttpStatusCode.NotFound, typeof(KeyNotFoundException))]
        [MapError(System.Net.HttpStatusCode.Forbidden, typeof(ArgumentException))]
        Task<string> SayHello([InPath(Property = "Name")][InQuery(Name = "lang", Property = "Language")] [AsBody("Name", "Language")] HelloRequest helloRequest);
    }

    /// <summary>
    /// Hello request record.
    /// </summary>
    public record HelloRequest(string Name, string Language, string? CustomSuffix = null);
}
