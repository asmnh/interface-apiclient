using InterfaceApiClient.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InterfaceApiClient.Tests
{
    [TestClass]
    public class TestApiClientConfiguration
    {
        private interface INoGroup { };
        [ApiGroup("MyGroup")]
        private interface IHaveGroup { };

        [TestMethod]
        public void UseEndpoint_AddsEntryToConfiguration()
        {
            string groupUri = "https://group.uri/";
            ApiClientConfiguration subject = new ApiClientConfiguration().UseEndpoint<INoGroup>(groupUri);
            CollectionAssert.Contains(subject.Endpoints.ToList(), new KeyValuePair<string, string>(nameof(INoGroup), groupUri));
            
            subject = new ApiClientConfiguration().UseEndpoint<IHaveGroup>(groupUri);
            CollectionAssert.Contains(subject.Endpoints.ToList(), new KeyValuePair<string, string>("MyGroup", groupUri));
        }

        [TestMethod]
        public void Verify_ThrowsIfEndpointIsNotValidUriPrefix()
        {
            var subject = new ApiClientConfiguration().UseEndpoint("group", "invalid");
            Assert.ThrowsException<UriFormatException>(() => { subject.Verify(); });
        }

        [TestMethod]
        public void RemoveEndpoint_RemovesEndpointFromListOfEndpoints()
        {
            string groupUri = "https://group.uri/";
            ApiClientConfiguration subject = new ApiClientConfiguration().UseEndpoint<INoGroup>(groupUri);
            CollectionAssert.Contains(subject.Endpoints.ToList(), new KeyValuePair<string, string>(nameof(INoGroup), groupUri));
            subject.RemoveEndpoint(nameof(INoGroup));
            Assert.IsFalse(subject.Endpoints.Any(e => e.Key == nameof(INoGroup)));
            subject.UseEndpoint<INoGroup>(groupUri).RemoveEndpoint<INoGroup>();
            Assert.IsFalse(subject.Endpoints.Any(e => e.Key == nameof(INoGroup)));
        }
    }
}
