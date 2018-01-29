using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.Core.Tests
{
    [TestClass]
    public class TextFunctionsTests
    {
        [TestMethod]
        public void EnsureExpectedVersion_Ok()
        {
            var expectedText = "a1.2a";
            var version = VersionFunctions.ParseVersion("1.2");

            var actualText = TextFunctions.EnsureExpectedVersion(expectedText, @"a(?<version>\d\.\d)a", version);

            Assert.AreEqual(expectedText, actualText);
        }

        [TestMethod]
        public void EnsureExpectedVersion_WrongText()
        {
            var version = VersionFunctions.ParseVersion("1.2");

            Assert.ThrowsException<InvalidOperationException>(() => TextFunctions.EnsureExpectedVersion("not a number", @"(?<version>\d\.\d)", version));
        }

        [TestMethod]
        public void EnsureExpectedVersion_WrongRegex()
        {
            var version = VersionFunctions.ParseVersion("1.2");

            Assert.ThrowsException<InvalidOperationException>(() => TextFunctions.EnsureExpectedVersion("1.2", "wrong regex", version));
        }
    }
}
