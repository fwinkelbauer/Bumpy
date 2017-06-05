using System;
using Bumpy.Version;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.UnitTests.Version
{
    [TestClass]
    public class BumpyVersionTests
    {
        [TestMethod]
        public void ToString_ReturnsVersionString()
        {
            var version = CreateVersion(1, 2, 3, 4);

            Assert.AreEqual("1.2.3.4", version.ToString());
        }

        [TestMethod]
        public void Constructor_ValidateInput()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BumpyVersion(null));
            Assert.ThrowsException<ArgumentException>(() => new BumpyVersion(new int[] { }));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => CreateVersion(5, 3, -1));
        }

        [DataTestMethod]
        [DataRow(1, "3.0.0.0")]
        [DataRow(2, "2.3.0.0")]
        [DataRow(3, "2.2.3.0")]
        [DataRow(4, "2.2.2.3")]
        public void Increment_IncrementDifferentPositions(int position, string expectedVersion)
        {
            var version = CreateVersion(2, 2, 2, 2);

            var inc = version.Increment(position);

            Assert.AreEqual(expectedVersion, inc.ToString());
        }

        [TestMethod]
        public void Increment_InvalidArgument()
        {
            var version = CreateVersion(2, 2, 2, 2);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => version.Increment(-1));
        }

        [DataTestMethod]
        [DataRow(1, "9.1.1.1")]
        [DataRow(2, "1.9.1.1")]
        [DataRow(3, "1.1.9.1")]
        [DataRow(4, "1.1.1.9")]
        public void Assign_AssignPositions(int position, string expectedVersion)
        {
            var version = CreateVersion(1, 1, 1, 1);

            var inc = version.Assign(position, 9);

            Assert.AreEqual(expectedVersion, inc.ToString());
        }

        [TestMethod]
        public void Assign_InvalidArgument()
        {
            var version = CreateVersion(2, 2, 2, 2);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => version.Assign(0, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => version.Assign(-1, 0));
        }

        [DataTestMethod]
        [DataRow("9.1.1.1")]
        [DataRow("1.9.1.1")]
        [DataRow("1.1.9.1")]
        [DataRow("1.1.1.9")]
        public void Equals_DifferentState(string versionText)
        {
            var version = CreateVersion(1, 1, 1, 1);
            var otherVersion = VersionHelper.ParseVersionFromText(versionText);

            Assert.IsFalse(version.Equals(otherVersion));
            Assert.IsFalse(otherVersion.Equals(version));
            Assert.AreNotEqual(version.GetHashCode(), otherVersion.GetHashCode());
        }

        [TestMethod]
        public void Equals_SameState()
        {
            var version = CreateVersion(1, 2, 3, 4);
            var otherVersion = CreateVersion(1, 2, 3, 4);

            Assert.IsTrue(version.Equals(otherVersion));
            Assert.IsTrue(otherVersion.Equals(version));
            Assert.AreEqual(version.GetHashCode(), otherVersion.GetHashCode());
        }

        [TestMethod]
        public void Equals_WrongType()
        {
            var version = CreateVersion(1, 2, 3, 4);

            Assert.IsFalse(version.Equals(null));
            Assert.IsFalse(version.Equals(string.Empty));
        }

        private BumpyVersion CreateVersion(params int[] parts)
        {
            return new BumpyVersion(parts);
        }
    }
}
