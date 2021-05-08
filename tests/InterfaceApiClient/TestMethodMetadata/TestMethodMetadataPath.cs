using InterfaceApiClient.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Threading.Tasks;

namespace InterfaceApiClient.Tests
{
    [TestClass]
    public partial class TestMethodMetadata
    {
        public record SomeRecord(int id, string Name);

        public interface IPathParams
        {
            [ApiEndpoint(HttpRequestMethod.GET, "plain")]
            Task<string> Plain();
            [ApiEndpoint(HttpRequestMethod.GET, "parametrized/{id}")]
            Task<string> Parametrized([InPath] int id);
            [ApiEndpoint(HttpRequestMethod.GET, "parametrized/{id}")]
            Task<string> NamedParameter([InPath(name:"id")] int objectId);
            [ApiEndpoint(HttpRequestMethod.GET, "camelcase/{id}")]
            Task<string> CamelCasedParameter([InPath(name: "Id")] int objectId);
            [ApiEndpoint(HttpRequestMethod.GET, "path/{id}")]
            Task<string> PathParameter([InPath(name: "id", Property = "id")] SomeRecord record);
        }

        [TestMethod]
        public void BuildPath_ParameterlessPath_KeepsPath()
        {
            MethodInfo method = typeof(IPathParams).GetMethod(nameof(IPathParams.Plain))!;
            var subject = new MethodMetadata(method);
            string path = subject.BuildPath(new object[0]);
            Assert.AreEqual("plain", path);
        }

        [TestMethod]
        public void BuildPath_ParametrizedPath_KeepsPath()
        {
            MethodInfo method = typeof(IPathParams).GetMethod(nameof(IPathParams.Parametrized))!;
            var subject = new MethodMetadata(method);
            string path = subject.BuildPath(new object[] { 1 });
            Assert.AreEqual("parametrized/1", path);
        }


        [TestMethod]
        public void BuildPath_NamedPath_KeepsPath()
        {
            MethodInfo method = typeof(IPathParams).GetMethod(nameof(IPathParams.NamedParameter))!;
            var subject = new MethodMetadata(method);
            string path = subject.BuildPath(new object[] { 1 });
            Assert.AreEqual("parametrized/1", path);
        }

        [TestMethod]
        public void BuildPath_CamelCasePath_KeepsPath()
        {
            MethodInfo method = typeof(IPathParams).GetMethod(nameof(IPathParams.CamelCasedParameter))!;
            var subject = new MethodMetadata(method);
            string path = subject.BuildPath(new object[] { 3 });
            Assert.AreEqual("camelcase/3", path);
        }

        [TestMethod]
        public void BuildPack_UnpackProperty_FindsValue()
        {
            MethodInfo method = typeof(IPathParams).GetMethod(nameof(IPathParams.PathParameter))!;
            var subject = new MethodMetadata(method);
            string path = subject.BuildPath(new object[] { new SomeRecord(5, "test") });
            Assert.AreEqual("path/5", path);
        }
    }
}
