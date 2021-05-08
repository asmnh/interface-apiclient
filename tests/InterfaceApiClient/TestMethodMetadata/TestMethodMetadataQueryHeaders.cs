using InterfaceApiClient.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceApiClient.Tests
{
    partial class TestMethodMetadata
    {
        public interface IQueryParams
        {
            [ApiEndpoint(HttpRequestMethod.GET, "/")]
            public Task<string> Plain();
            [ApiEndpoint(HttpRequestMethod.GET, "/")]
            public Task<string> Simple([InQuery] string queryParam);
            [ApiEndpoint(HttpRequestMethod.GET, "/")]
            public Task<string> Named([InQuery("param")] string queryParam);
            [ApiEndpoint(HttpRequestMethod.GET, "/")]
            public Task<string> Subpath([InQuery("name", Property = "Name")] SomeRecord record);
        }

        [TestMethod]
        public void BuildQuery_Parameterless_EmptyCollection()
        {
            MethodInfo method = typeof(IQueryParams).GetMethod(nameof(IQueryParams.Plain))!;
            var subject = new MethodMetadata(method);
            var result = subject.BuildQuery(new object[0]);
            Assert.IsTrue(result.Count == 0);
        }

        [TestMethod]
        public void BuildQuery_SimpleParam_ProvidesValue()
        {
            MethodInfo method = typeof(IQueryParams).GetMethod(nameof(IQueryParams.Simple))!;
            var subject = new MethodMetadata(method);
            var result = subject.BuildQuery(new object[] { "test" });
            Assert.IsTrue(result.ContainsKey("queryParam"));
            Assert.AreEqual("test", result["queryParam"]);
        }

        [TestMethod]
        public void BuildQuery_NamedParam_ProvidesValue()
        {
            MethodInfo method = typeof(IQueryParams).GetMethod(nameof(IQueryParams.Named))!;
            var subject = new MethodMetadata(method);
            var result = subject.BuildQuery(new object[] { "test" });
            Assert.IsTrue(result.ContainsKey("param"));
            Assert.AreEqual("test", result["param"]);
        }

        [TestMethod]
        public void BuildQuery_ParamPath_ProvidesValue()
        {
            MethodInfo method = typeof(IQueryParams).GetMethod(nameof(IQueryParams.Subpath))!;
            var subject = new MethodMetadata(method);
            var result = subject.BuildQuery(new object[] { new SomeRecord(5, "test") });
            Assert.IsTrue(result.ContainsKey("name"));
            Assert.AreEqual("test", result["name"]);
        }

        public interface IHeaderParams
        {
            [ApiEndpoint(HttpRequestMethod.GET, "/")]
            public Task<string> Plain();
            [ApiEndpoint(HttpRequestMethod.GET, "/")]
            public Task<string> Simple([InHeader("queryParam")] string queryParam);
            [ApiEndpoint(HttpRequestMethod.GET, "/")]
            public Task<string> Named([InHeader("param")] string queryParam);
            [ApiEndpoint(HttpRequestMethod.GET, "/")]
            public Task<string> Subpath([InHeader("name", Property = "Name")] SomeRecord record);
            [ApiEndpoint(HttpRequestMethod.GET, "/")]
            public Task<string> Casing([InHeader("X-App-Header")] string value);
        }

        [TestMethod]
        public void BuildHeaders_Parameterless_EmptyCollection()
        {
            MethodInfo method = typeof(IHeaderParams).GetMethod(nameof(IHeaderParams.Plain))!;
            var subject = new MethodMetadata(method);
            var result = subject.BuildHeaders(new object[0]);
            Assert.IsTrue(result.Count == 0);
        }

        [TestMethod]
        public void BuildHeaders_SimpleParam_ProvidesValue()
        {
            MethodInfo method = typeof(IHeaderParams).GetMethod(nameof(IHeaderParams.Simple))!;
            var subject = new MethodMetadata(method);
            var result = subject.BuildHeaders(new object[] { "test" });
            Assert.IsTrue(result.ContainsKey("queryParam"));
            Assert.AreEqual("test", result["queryParam"]);
        }

        [TestMethod]
        public void BuildHeaders_NamedParam_ProvidesValue()
        {
            MethodInfo method = typeof(IHeaderParams).GetMethod(nameof(IHeaderParams.Named))!;
            var subject = new MethodMetadata(method);
            var result = subject.BuildHeaders(new object[] { "test" });
            Assert.IsTrue(result.ContainsKey("param"));
            Assert.AreEqual("test", result["param"]);
        }

        [TestMethod]
        public void BuildHeaders_ParamPath_ProvidesValue()
        {
            MethodInfo method = typeof(IHeaderParams).GetMethod(nameof(IHeaderParams.Subpath))!;
            var subject = new MethodMetadata(method);
            var result = subject.BuildHeaders(new object[] { new SomeRecord(5, "test") });
            Assert.IsTrue(result.ContainsKey("name"));
            Assert.AreEqual("test", result["name"]);
        }

        [TestMethod]
        public void BuildHeaders_Casing_PreservesCasing()
        {
            string headerName = "X-App-Header";
            MethodInfo method = typeof(IHeaderParams).GetMethod(nameof(IHeaderParams.Casing))!;
            var subject = new MethodMetadata(method);
            var result = subject.BuildHeaders(new object[] { "test" });
            Assert.IsTrue(result.ContainsKey(headerName));
            Assert.AreEqual("test", result[headerName]);
        }
    }
}
