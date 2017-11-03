using System;
using Bumpy.Version;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.UnitTests.Version
{
    [TestClass]
    public class VersionHelperTests
    {
        [TestMethod]
        public void ReplaceVersionInText_Replaced()
        {
            var version = VersionHelper.ParseVersionFromText("1.2.3");
            var text = VersionHelper.ReplaceVersionInText("a0.0.0a", @"a(?<version>\d\.\d\.\d)a", version);

            Assert.AreEqual("a1.2.3a", text);
        }

        [TestMethod]
        public void ReplaceVersionInText_NoChange()
        {
            var version = VersionHelper.ParseVersionFromText("1.2.3");
            var text = VersionHelper.ReplaceVersionInText("text", @"a(?<version>\d\.\d\.\d)a", version);

            Assert.AreEqual("text", text);
        }

        [TestMethod]
        public void ReplaceVersionInText_RegexWithoutNamedGroup()
        {
            var version = VersionHelper.ParseVersionFromText("1.2.3");
            var text = VersionHelper.ReplaceVersionInText("a0.0.0a", @"a\d\.\d\.\da", version);

            Assert.AreEqual("a0.0.0a", text);
        }

        [TestMethod]
        public void ReplaceVersionInText_InvalidRegexWithNamedGroup()
        {
            var version = VersionHelper.ParseVersionFromText("1.2.3");

            Assert.ThrowsException<ArgumentException>(() => VersionHelper.ReplaceVersionInText("a0.0.0a", @"(?<version>a\d\.\d\.\da)", version));
        }

        [DataTestMethod]
        [DataRow("1.2.3", "a0.0.0.0a", @"a(?<version>\d\.\d\.\d\.\d)a")]
        [DataRow("1.2.3a", "0.0.0.0", @"(?<version>\d\.\d\.\d)")]
        public void ReplaceVersionInText_NewVersionWouldMakeRegexUseless(string versionText, string text, string regex)
        {
            var version = VersionHelper.ParseVersionFromText(versionText);

            Assert.ThrowsException<InvalidOperationException>(() => VersionHelper.ReplaceVersionInText(text, regex, version));
        }

        [TestMethod]
        public void FindVersion_Found()
        {
            var version = VersionHelper.FindVersion("a1.2a", @"a(?<version>\d\.\d)a");

            Assert.AreEqual("1.2", version.ToString());
        }

        [TestMethod]
        public void FindVersion_RegexWithoutNamedGroup()
        {
            var version = VersionHelper.FindVersion("100", @"\d");

            Assert.IsNull(version);
        }

        [TestMethod]
        public void FindVersion_InvalidRegexWithNamedGroup()
        {
            Assert.ThrowsException<ArgumentException>(() => VersionHelper.FindVersion("1.0//", @"(?<version>1.0//)"));
        }

        [TestMethod]
        public void FindVersion_ValidRegexButNoData()
        {
            var version = VersionHelper.FindVersion("something", @"a(?<version>\d\.\d)a");

            Assert.IsNull(version);
        }

        [DataTestMethod]
        [DataRow("8")]
        [DataRow("100.5")]
        [DataRow("2020.100.143")]
        [DataRow("1.0.0.9")]
        [DataRow("1.0.0.9-foo")]
        [DataRow("1.100a")]
        [DataRow("18.5.3-beta03")]
        [DataRow("8.25.3-beta03+master0004")]
        [DataRow("8.5.43+bar")]
        public void ParseVersionFromText_ValidText(string versionText)
        {
            var version = VersionHelper.ParseVersionFromText(versionText);

            Assert.AreEqual(versionText, version.ToString());
        }

        [TestMethod]
        public void ParseVersionFromText_InvalidText()
        {
            Assert.ThrowsException<ArgumentException>(() => VersionHelper.ParseVersionFromText("a.b"));
        }
    }
}
