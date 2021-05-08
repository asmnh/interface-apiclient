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
        public record BodyRecord(int Id, string Name, string Data, object stuff);

        public interface IBody
        {
            [ApiEndpoint(HttpRequestMethod.GET, "/")]
            Task<int> None();
            [ApiEndpoint(HttpRequestMethod.POST, "/")]
            Task<int> InBodySimple([InBody("value")] string value);
            [ApiEndpoint(HttpRequestMethod.POST, "/")]
            Task<int> InBodyMultiple([InBody("value")] string value, [InBody("id")] int id);
            [ApiEndpoint(HttpRequestMethod.POST, "/")]
            Task<int> AsBodySimple([AsBody] SomeRecord value);
            [ApiEndpoint(HttpRequestMethod.POST, "/")]
            Task<int> AsBodyWithExclude([AsBody(nameof(SomeRecord.id))] SomeRecord record);
            // TODO: multiple AsBody, mixed AsBody and InBody, override rules
        }

        [TestMethod]
        public void BuildBody_NoParameters_Null()
        {
            MethodInfo method = typeof(IBody).GetMethod(nameof(IBody.None))!;
            var subject = new MethodMetadata(method);
            var body = subject.BuildBody(new object[0]);
            Assert.IsNull(body);
        }

        [TestMethod]
        public void BuildBody_SimpleInBody_ReturnsValue()
        {
            MethodInfo method = typeof(IBody).GetMethod(nameof(IBody.InBodySimple))!;
            var subject = new MethodMetadata(method);
            var body = subject.BuildBody(new object[] { "123" }) as IDictionary<string, object?>;
            Assert.IsNotNull(body);
            Assert.IsTrue(body!.ContainsKey("value"));
            Assert.AreEqual("123", body["value"]);
        }

        [TestMethod]
        public void BuildBody_MultipleInBody_ReturnsValue()
        {
            MethodInfo method = typeof(IBody).GetMethod(nameof(IBody.InBodyMultiple))!;
            var subject = new MethodMetadata(method);
            var body = subject.BuildBody(new object[] { "123", 5 }) as IDictionary<string, object?>;
            Assert.IsNotNull(body);
            Assert.IsTrue(body!.ContainsKey("value"));
            Assert.AreEqual("123", body["value"]);
            Assert.IsTrue(body!.ContainsKey("id"));
            Assert.AreEqual(5, (int)body["id"]!);
        }

        [TestMethod]
        public void BuildBody_AsBodySimpleType_ReturnsOriginalValue()
        {
            MethodInfo method = typeof(IBody).GetMethod(nameof(IBody.AsBodySimple))!;
            var subject = new MethodMetadata(method);
            var input = new SomeRecord(5, "123");
            var body = subject.BuildBody(new object[] { input }) as SomeRecord;
            Assert.IsNotNull(body);
            Assert.AreEqual(input, body);
        }

        [TestMethod]
        public void BuildBody_AsBodyWithExclude_ReturnsDictionary()
        {
            MethodInfo method = typeof(IBody).GetMethod(nameof(IBody.AsBodyWithExclude))!;
            var subject = new MethodMetadata(method);
            var input = new SomeRecord(5, "123");
            var body = subject.BuildBody(new object[] { input }) as IDictionary<string, object?>;
            Assert.IsNotNull(body);
            Assert.AreEqual(1, body!.Count);
            Assert.IsTrue(body!.ContainsKey("name"));
            Assert.AreEqual("123", body["name"]);
            Assert.IsFalse(body!.ContainsKey("id"));
            Assert.IsFalse(body!.ContainsKey("Id"));
        }
    }
}
