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
            [ApiGroup("CustomNo")]
            Task CustomGroup();
        };

        [ApiGroup("MyGroup")]
        private interface IHaveGroup
        {
            Task DefaultGroup();
            [ApiGroup("CustomHave")]
            Task CustomGroup();
        };

        [DataTestMethod]
        [DataRow(typeof(INoGroup), nameof(INoGroup.DefaultGroup), nameof(INoGroup))]
        [DataRow(typeof(INoGroup), nameof(INoGroup.CustomGroup), "CustomNo")]
        [DataRow(typeof(IHaveGroup), nameof(IHaveGroup.DefaultGroup), "MyGroup")]
        [DataRow(typeof(IHaveGroup), nameof(IHaveGroup.CustomGroup), "CustomHave")]
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
