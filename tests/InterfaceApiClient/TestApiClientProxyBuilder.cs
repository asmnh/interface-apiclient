using InterfaceApiClient.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace InterfaceApiClient.Tests
{
    [TestClass]
    public class TestApiClientProxyBuilder
    {
        public interface IValidApi
        {
            [ApiEndpoint(HttpRequestMethod.GET, "request")]
            Task<string> Request();
        }

        private static TInterface CreateProxyInstance<TInterface>(Mock<IApiClientProxy> proxy)
        {
            var subject = new ApiClientProxyBuilder(new ApiClientConfiguration().UseEndpoint<TInterface>("https://valid.api/"));
            subject.AddInterface(typeof(TInterface));
            subject.Build();
            return (TInterface)subject.ImplementationFor(typeof(TInterface))(proxy.Object, new ProxyMetadata(typeof(TInterface)));
        }

        [TestMethod]
        public async Task BuildProxy_HasEndpoint_CallingWillPassToApiProxy()
        {
            Mock<IApiClientProxy> proxy = new Mock<IApiClientProxy>();
            IValidApi p = CreateProxyInstance<IValidApi>(proxy);
            proxy.Setup(mock => mock.Call<Task<string>>(It.IsAny<MethodInfo>(), It.IsAny<ProxyMetadata>(), It.IsAny<object?[]>())).Returns(() => Task.FromResult("world"));
            string response = await p.Request();
            Assert.AreEqual("world", response);
            proxy.Verify(mock => mock.Call<Task<string>>(It.Is<MethodInfo>(meth => meth.Name == nameof(IValidApi.Request)), It.IsAny<ProxyMetadata>(), It.Is<object[]>(args => args.Length == 0)), Times.Once);
        }
    }
}
