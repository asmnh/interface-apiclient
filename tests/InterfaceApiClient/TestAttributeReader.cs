using InterfaceApiClient.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace InterfaceApiClient.Tests
{
    [TestClass]
    public class TestAttributeReader
    {
        private interface INoGroup
        {
            Task DefaultGroup();
        };

        [ApiGroup("MyGroup")]
        private interface IHaveGroup
        {
            Task DefaultGroup();
        };

        [DataTestMethod]
        [DataRow(typeof(INoGroup), nameof(INoGroup.DefaultGroup), nameof(INoGroup))]
        [DataRow(typeof(IHaveGroup), nameof(IHaveGroup.DefaultGroup), "MyGroup")]
        public void GetGroupName_Method(Type interfaceType, string method, string expected)
        {
            string name = AttributeReader.GetGroupName(interfaceType.GetMethod(method)!);
            Assert.AreEqual(expected, name);
        }

        [DataTestMethod]
        [DataRow(typeof(INoGroup), nameof(INoGroup))]
        [DataRow(typeof(IHaveGroup), "MyGroup")]
        public void GetGroupName_Type(Type type, string expected)
        {
            string groupName = AttributeReader.GetGroupName(type);
            Assert.AreEqual(expected, groupName);
        }
    }
}
