using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.Core.Tests
{
    [TestClass]
    public class BumpyVersionTests
    {
        [DataTestMethod]
        [DataRow("3.0.0.0")]
        [DataRow("100.52.89.1024")]
        [DataRow("5.17.0-foo+bar")]
        [DataRow("5.17.0_final")]
        public void ToString_ReturnsVersionString(string versionText)
        {
            var version = VersionFunctions.ParseVersion(versionText);

            Assert.AreEqual(versionText, version.ToString());
        }

        [TestMethod]
        public void Constructor_ValidateInput()
        {
            Assert.ThrowsException<ArgumentException>(() => new BumpyVersion(new int[] { }, string.Empty));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BumpyVersion(new[] { 2, 1, -3 }, string.Empty));
        }

        [DataTestMethod]
        [DataRow("9.1.1")]
        [DataRow("1.9.1")]
        [DataRow("1.1.9")]
        [DataRow("1.1.1-foo")]
        public void Equals_DifferentState(string versionText)
        {
            var version = VersionFunctions.ParseVersion("1.1.1");
            var otherVersion = VersionFunctions.ParseVersion(versionText);

            Assert.IsFalse(version.Equals(otherVersion));
            Assert.IsFalse(otherVersion.Equals(version));
            Assert.AreNotEqual(version.GetHashCode(), otherVersion.GetHashCode());
        }

        [DataTestMethod]
        [DataRow("1.8.5")]
        [DataRow("1.8.5-foo")]
        public void Equals_SameState(string versionText)
        {
            var version = VersionFunctions.ParseVersion(versionText);
            var otherVersion = VersionFunctions.ParseVersion(versionText);

            Assert.IsTrue(version.Equals(otherVersion));
            Assert.IsTrue(otherVersion.Equals(version));
            Assert.AreEqual(version.GetHashCode(), otherVersion.GetHashCode());
        }

        [TestMethod]
        public void Equals_WrongType()
        {
            var version = VersionFunctions.ParseVersion("1.1.1");

            Assert.IsFalse(version.Equals(null));
            Assert.IsFalse(version.Equals(string.Empty));
        }
    }
}
