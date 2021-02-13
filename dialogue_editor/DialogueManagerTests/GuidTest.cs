using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DialogueManagerTests
{
    /// <summary>
    /// Summary description for GuidTest
    /// </summary>
    [TestClass]
    public class GuidTest
    {
        private string _validGuidStringA = "F964FB23-022B-48CD-99C4-52EAC595B9B0";
        private string _validGuidStringB = "9440F8C9-192F-4197-A361-D6E0FA2A3562";

        [TestMethod]
        public void DefaultConstructorCreatesInvalidGuid()
        {
            var newGuid = new floofy.Guid();
            Assert.IsFalse(newGuid.IsValid());
        }

        [TestMethod]
        public void StringConstructorCreatesValidGuidWithValidString()
        {
            var newGuid = new floofy.Guid(_validGuidStringA);
            Assert.IsTrue(newGuid.IsValid());
        }

        [TestMethod]
        public void StringConstructorCreatesInValidGuidWithEmptyString()
        {
            string emptyString = "";
            var newGuid = new floofy.Guid(emptyString);
            Assert.IsFalse(newGuid.IsValid());
        }

        [TestMethod]
        public void ToStringMethodReturnsIdenticalString()
        {
            var newGuid = new floofy.Guid(_validGuidStringA);
            Assert.AreEqual(newGuid.ToString(), _validGuidStringA);
        }

        [TestMethod]
        public void ComparingIdenticalGuidsReturnsTrue()
        {
            var guidA = new floofy.Guid(_validGuidStringA);
            var guidB = new floofy.Guid(_validGuidStringA);
            Assert.AreEqual(guidA, guidB);
        }

        [TestMethod]
        public void ComparingDifferentGuidsReturnsFalse()
        {
            var guidA = new floofy.Guid(_validGuidStringA);
            var guidB = new floofy.Guid(_validGuidStringB);
            Assert.AreNotEqual(guidA, guidB);
        }
    }
}