using InterfaceApiClient.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void UseEndpoint_Generic()
        {
            string groupUri = "https://group.uri/";
            ApiClientConfiguration subject = new ApiClientConfiguration().UseEndpoint<INoGroup>(groupUri);
            CollectionAssert.Contains(subject.Endpoints.ToList(), new KeyValuePair<string, string>(nameof(INoGroup), groupUri));
            
            subject = new ApiClientConfiguration().UseEndpoint<IHaveGroup>(groupUri);
            CollectionAssert.Contains(subject.Endpoints.ToList(), new KeyValuePair<string, string>("MyGroup", groupUri));
        }
    }
}
