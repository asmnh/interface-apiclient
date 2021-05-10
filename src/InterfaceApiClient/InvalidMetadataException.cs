using System;

namespace InterfaceApiClient
{
    public class InvalidMetadataException : Exception
    {
        public Type InvalidType { get; }
        public string Endpoint { get; }

        public InvalidMetadataException(Type type, string endpoint, string? message) : base(message)
        {
            InvalidType = type;
            Endpoint = endpoint;
        }

        public InvalidMetadataException(Type type, string endpoint) : this(type, endpoint, $"Endpoint {endpoint} of type {type} could not be build") { }
    }
}
