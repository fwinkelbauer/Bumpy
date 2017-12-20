using System;
using Bumpy.Version;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.UnitTests.Version
{
    [TestClass]
    public class BumpyVersionTests
    {
        [DataTestMethod]
        [DataRow("3.0.0.0")]
        [DataRow("100.52.89.1024")]
        [DataRow("5.17.0-foo+bar")]
        public void ToString_ReturnsVersionString(string versionText)
        {
            var version = VersionHelper.ParseVersionFromText(versionText);

            Assert.AreEqual(versionText, version.ToString());
        }

        [TestMethod]
        public void Constructor_ValidateInput()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BumpyVersion(null, string.Empty));
            Assert.ThrowsException<ArgumentException>(() => new BumpyVersion(new int[] { }, string.Empty));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BumpyVersion(new[] { 2, 1, -3 }, string.Empty));
            Assert.ThrowsException<ArgumentNullException>(() => new BumpyVersion(new[] { 1 }, null));
        }

        [DataTestMethod]
        [DataRow(1, "2.2.2.2", "3.0.0.0")]
        [DataRow(2, "2.2.2.2", "2.3.0.0")]
        [DataRow(3, "2.2.2.2", "2.2.3.0")]
        [DataRow(4, "2.2.2.2", "2.2.2.3")]
        [DataRow(4, "2.2.2.2+bar", "2.2.2.3+bar")]
        [DataRow(3, "2.2.2.2+bar", "2.2.3.0+bar")]
        public void Increment_IncrementDifferentPositions(int position, string originalVersionText, string expectedVersionText)
        {
            var version = VersionHelper.ParseVersionFromText(originalVersionText);

            var inc = version.Increment(position);

            Assert.AreEqual(expectedVersionText, inc.ToString());
        }

        [TestMethod]
        public void Increment_InvalidArgument()
        {
            var version = VersionHelper.ParseVersionFromText("2.2.2.2");

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => version.Increment(-1));
        }

        [DataTestMethod]
        [DataRow(1, "2.2.2.2", "3.2.2.2")]
        [DataRow(2, "2.2.2.2", "2.3.2.2")]
        [DataRow(3, "2.2.2.2", "2.2.3.2")]
        [DataRow(4, "2.2.2.2", "2.2.2.3")]
        [DataRow(4, "2.2.2.2+bar", "2.2.2.3+bar")]
        [DataRow(3, "2.2.2.2+bar", "2.2.3.2+bar")]
        public void IncrementOnly_IncrementDifferentPositions(int position, string originalVersionText, string expectedVersionText)
        {
            var version = VersionHelper.ParseVersionFromText(originalVersionText);

            var inc = version.Increment(position, false);

            Assert.AreEqual(expectedVersionText, inc.ToString());
        }

        [DataTestMethod]
        [DataRow(1, "1.1.1", "9.1.1")]
        [DataRow(2, "1.1.1", "1.9.1")]
        [DataRow(3, "1.1.1", "1.1.9")]
        [DataRow(1, "1.1.1-foo", "9.1.1-foo")]
        public void Assign_AssignPositions(int position, string originalVersionText, string expectedVersionText)
        {
            var version = VersionHelper.ParseVersionFromText(originalVersionText);

            var inc = version.Assign(position, 9);

            Assert.AreEqual(expectedVersionText, inc.ToString());
        }

        [TestMethod]
        public void Assign_InvalidArgument()
        {
            var version = VersionHelper.ParseVersionFromText("2.2.2");

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => version.Assign(0, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => version.Assign(-1, 0));
        }

        [DataTestMethod]
        [DataRow("9.1.1")]
        [DataRow("1.9.1")]
        [DataRow("1.1.9")]
        [DataRow("1.1.1-foo")]
        public void Equals_DifferentState(string versionText)
        {
            var version = VersionHelper.ParseVersionFromText("1.1.1");
            var otherVersion = VersionHelper.ParseVersionFromText(versionText);

            Assert.IsFalse(version.Equals(otherVersion));
            Assert.IsFalse(otherVersion.Equals(version));
            Assert.AreNotEqual(version.GetHashCode(), otherVersion.GetHashCode());
        }

        [DataTestMethod]
        [DataRow("1.8.5")]
        [DataRow("1.8.5-foo")]
        public void Equals_SameState(string versionText)
        {
            var version = VersionHelper.ParseVersionFromText(versionText);
            var otherVersion = VersionHelper.ParseVersionFromText(versionText);

            Assert.IsTrue(version.Equals(otherVersion));
            Assert.IsTrue(otherVersion.Equals(version));
            Assert.AreEqual(version.GetHashCode(), otherVersion.GetHashCode());
        }

        [TestMethod]
        public void Equals_WrongType()
        {
            var version = VersionHelper.ParseVersionFromText("1.1.1");

            Assert.IsFalse(version.Equals(null));
            Assert.IsFalse(version.Equals(string.Empty));
        }
    }
}
