using InterfaceApiClient.DataTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterfaceApiClient.Tests
{
    [TestClass]
    public class TestApiClientBuilder
    {
        public interface ISampleClient
        {
            [ApiEndpoint(HttpRequestMethod.GET, "/sample")]
            Task<string> Sample();
        }

        public interface IMalformedClient
        {
            [ApiEndpoint(HttpRequestMethod.POST, "/data/{id}")]
            Task<string> InvalidMethod(string identifier);
        }

        private Mock<IServiceCollection> _servicesMock = null!;
        private List<ServiceDescriptor> _servicesList = null!;

        [TestInitialize]
        public void Initialize()
        {
            _servicesList = new();
            _servicesMock = new();
            _servicesMock.Setup(mock => mock.GetEnumerator()).Returns(() => _servicesList.GetEnumerator());
        }

        [TestMethod]
        public void RegisterAllDependencies_AddsApiClientProxy()
        {
            var subject = new ApiClientBuilder(_servicesMock.Object);
            subject.Apply();
            _servicesMock.Verify(mock => mock.Add(It.Is<ServiceDescriptor>(desc => desc.ServiceType == typeof(IApiClientProxy) && desc.ImplementationType == typeof(ApiClientProxy) && desc.Lifetime == ServiceLifetime.Transient)), Times.Once);
        }

        [TestMethod]
        public void RegisterAllDependencies_SkipsApiClientProxyIfOneIsAlreadyRegistered()
        {
            _servicesList.Add(new ServiceDescriptor(typeof(IApiClientProxy), (IServiceProvider provider) => new object(), ServiceLifetime.Transient));
            var subject = new ApiClientBuilder(_servicesMock.Object);
            subject.Apply();
            _servicesMock.Verify(mock => mock.Add(It.Is<ServiceDescriptor>(desc => desc.ServiceType == typeof(IApiClientProxy))), Times.Never);
        }

        [TestMethod]
        public void RegisterAllDependencies_AddsConfigurationAsSingleton()
        {
            var subject = new ApiClientBuilder(_servicesMock.Object);
            subject.Apply();
            _servicesMock.Verify(mock => mock.Add(It.Is<ServiceDescriptor>(desc => desc.ServiceType == typeof(ApiClientConfiguration) && desc.Lifetime == ServiceLifetime.Singleton)), Times.Once);
        }

        [TestMethod]
        public void RegisterAllDependencies_AddsImplementationForRegisteredInterface()
        {
            var subject = new ApiClientBuilder(_servicesMock.Object);
            subject.WithTransientClient<ISampleClient>(config => { config.UseEndpoint<ISampleClient>("https://sample"); });
            subject.Apply();
            _servicesMock.Verify(mock => mock.Add(It.Is<ServiceDescriptor>(desc => desc.ServiceType == typeof(ISampleClient) && desc.Lifetime == ServiceLifetime.Transient && desc.ImplementationFactory != null)), Times.Once);
        }

        [TestMethod]
        public void RegisterAllDependencies_SkipsAlreadyDefinedImplementation()
        {
            _servicesList.Add(new ServiceDescriptor(typeof(ISampleClient), new object()));
            var subject = new ApiClientBuilder(_servicesMock.Object);
            subject.WithTransientClient<ISampleClient>(config => { config.UseEndpoint<ISampleClient>("https://sample"); });
            subject.Apply();
            _servicesMock.Verify(mock => mock.Add(It.Is<ServiceDescriptor>(desc => desc.ServiceType == typeof(ISampleClient))), Times.Never);
        }

        [TestMethod]
        public void RegisterAllDependencies_AbortsIfItCantBuildApiClient()
        {
            var subject = new ApiClientBuilder(_servicesMock.Object);
            subject.WithTransientClient<IMalformedClient>();
            Assert.ThrowsException<InvalidMetadataException>(() => { subject.Apply(); });
            _servicesMock.Verify(mock => mock.Add(It.IsAny<ServiceDescriptor>()), Times.Never);
        }

        [TestMethod]
        public void RegisterAllDependencies_ThrowsIfThereIsAGroupWithoutUriConfigured()
        {
            var subject = new ApiClientBuilder(_servicesMock.Object);
            subject.WithTransientClient<ISampleClient>();
            Assert.ThrowsException<KeyNotFoundException>(() => { subject.Apply(); }, "ISampleClient");
            _servicesMock.Verify(mock => mock.Add(It.IsAny<ServiceDescriptor>()), Times.Never);
        }

        [TestMethod]
        public void RegisterAllDependencies_ThrowsIfGroupHasMalformedUri()
        {
            var subject = new ApiClientBuilder(_servicesMock.Object);
            subject.WithTransientClient<ISampleClient>(config => { config.UseEndpoint<ISampleClient>("not a valid uri"); });
            Assert.ThrowsException<UriFormatException>(() => { subject.Apply(); }, "ISampleClient");
            _servicesMock.Verify(mock => mock.Add(It.IsAny<ServiceDescriptor>()), Times.Never);
        }
    }
}
