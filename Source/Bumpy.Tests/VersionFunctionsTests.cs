using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.Tests
{
    [TestClass]
    public class VersionFunctionsTests
    {
        [DataTestMethod]
        [DataRow(1, "2.2.2.2", "3.0.0.0", true)]
        [DataRow(2, "2.2.2.2", "2.3.0.0", true)]
        [DataRow(3, "2.2.2.2", "2.2.3.0", true)]
        [DataRow(4, "2.2.2.2", "2.2.2.3", true)]
        [DataRow(4, "2.2.2.2+bar", "2.2.2.3+bar", true)]
        [DataRow(1, "2.2.2.2", "3.2.2.2", false)]
        [DataRow(2, "2.2.2.2", "2.3.2.2", false)]
        [DataRow(3, "2.2.2.2", "2.2.3.2", false)]
        [DataRow(4, "2.2.2.2", "2.2.2.3", false)]
        [DataRow(4, "2.2.2.2+bar", "2.2.2.3+bar", false)]
        [DataRow(4, "2.2.2.002", "2.2.2.003", false)]
        [DataRow(4, "2.2.2.09", "2.2.2.10", false)]
        [DataRow(3, "2.2.99.2", "2.2.100.2", false)]
        [DataRow(2, "2.2.99.2", "2.3.0.0", true)]
        public void Increment_IncrementDifferentPositions(int position, string originalVersionText, string expectedVersionText, bool cascade)
        {
            var version = VersionFunctions.ParseVersion(originalVersionText);

            var newVersion = VersionFunctions.Increment(version, position, cascade);

            Assert.AreEqual(expectedVersionText, newVersion.ToString());
        }

        [TestMethod]
        public void Increment_InvalidArgument()
        {
            var version = VersionFunctions.ParseVersion("2.2.2.2");

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => VersionFunctions.Increment(version, -1, true));
        }

        [DataTestMethod]
        [DataRow(1, "1.1.1", "9.1.1", "9")]
        [DataRow(2, "1.1.1", "1.9.1", "9")]
        [DataRow(3, "1.1.1", "1.1.9", "9")]
        [DataRow(1, "1.1.1-foo", "9.1.1-foo", "9")]
        [DataRow(1, "1.1.1", "009.1.1", "009")]
        public void Assign_AssignPositions(int position, string originalVersionText, string expectedVersionText, string formattedNumber)
        {
            var version = VersionFunctions.ParseVersion(originalVersionText);

            var newVersion = VersionFunctions.Assign(version, position, formattedNumber);

            Assert.AreEqual(expectedVersionText, newVersion.ToString());
        }

        [TestMethod]
        public void Assign_InvalidArgument()
        {
            var version = VersionFunctions.ParseVersion("2.2.2");

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => VersionFunctions.Assign(version, 0, "-1"));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => VersionFunctions.Assign(version, -1, "0"));
            Assert.ThrowsException<ArgumentException>(() => VersionFunctions.Assign(version, 1, "not a number"));
        }

        [DataTestMethod]
        [DataRow("8")]
        [DataRow("100.5")]
        [DataRow("2018.01.01")]
        [DataRow("1.0.0.9")]
        [DataRow("1.0.0.9-foo")]
        [DataRow("1.100aBc")]
        [DataRow("18.5.3-beta03")]
        [DataRow("8.25.3-beta03+master0004")]
        [DataRow("8.5.43+bar")]
        [DataRow("1.0.0_final2")]
        public void ParseVersionFromText_ValidText(string versionText)
        {
            var version = VersionFunctions.ParseVersion(versionText);

            Assert.AreEqual(versionText, version.ToString());
        }

        [TestMethod]
        public void ParseVersionFromText_InvalidText()
        {
            Assert.ThrowsException<ArgumentException>(() => VersionFunctions.ParseVersion("invalid.input"));
        }

        [TestMethod]
        public void TryParseVersionInText_Found()
        {
            var success = VersionFunctions.TryParseVersionInText("a1.2a", @"a(?<version>\d\.\d)a", out var version);

            Assert.IsTrue(success);
            Assert.AreEqual("1.2", version.ToString());
        }

        [TestMethod]
        public void TryParseVersionInText_InvalidRegexWithoutNamedGroup()
        {
            var success = VersionFunctions.TryParseVersionInText("100", @"\d", out var version);

            Assert.IsFalse(success);
            Assert.IsNull(version);
        }

        [TestMethod]
        public void TryParseVersionInText_InvalidRegexWithNamedGroup()
        {
            Assert.ThrowsException<ArgumentException>(() => VersionFunctions.TryParseVersionInText("1.0//", @"(?<version>1.0//)", out _));
        }

        [TestMethod]
        public void TryParseVersionInText_ValidRegexButTextIsNoVersion()
        {
            var success = VersionFunctions.TryParseVersionInText("something", @"a(?<version>\d\.\d)a", out var version);

            Assert.IsFalse(success);
            Assert.IsNull(version);
        }

        [TestMethod]
        public void EnsureExpectedVersion_Ok()
        {
            var expectedText = "a1.2a";
            var version = VersionFunctions.ParseVersion("1.2");

            var actualText = VersionFunctions.EnsureExpectedVersion(expectedText, @"a(?<version>\d\.\d)a", version);

            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void EnsureExpectedVersion_WrongText()
        {
            var version = VersionFunctions.ParseVersion("1.2");

            Assert.ThrowsException<InvalidOperationException>(() => VersionFunctions.EnsureExpectedVersion("not a number", @"(?<version>\d\.\d)", version));
        }

        [TestMethod]
        public void EnsureExpectedVersion_WrongRegex()
        {
            var version = VersionFunctions.ParseVersion("1.2");

            Assert.ThrowsException<InvalidOperationException>(() => VersionFunctions.EnsureExpectedVersion("1.2", "wrong regex", version));
        }
    }
}
