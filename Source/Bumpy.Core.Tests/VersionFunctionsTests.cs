using System;
using Xunit;

namespace Bumpy.Core.Tests
{
    public class VersionFunctionsTests
    {
        [Theory]
        [InlineData(1, "2.2.2.2", "3.0.0.0", true)]
        [InlineData(2, "2.2.2.2", "2.3.0.0", true)]
        [InlineData(3, "2.2.2.2", "2.2.3.0", true)]
        [InlineData(4, "2.2.2.2", "2.2.2.3", true)]
        [InlineData(4, "2.2.2.2+bar", "2.2.2.3+bar", true)]
        [InlineData(1, "2.2.2.2", "3.2.2.2", false)]
        [InlineData(2, "2.2.2.2", "2.3.2.2", false)]
        [InlineData(3, "2.2.2.2", "2.2.3.2", false)]
        [InlineData(4, "2.2.2.2", "2.2.2.3", false)]
        [InlineData(4, "2.2.2.2+bar", "2.2.2.3+bar", false)]
        [InlineData(4, "2.2.2.002", "2.2.2.003", false)]
        [InlineData(4, "2.2.2.09", "2.2.2.10", false)]
        [InlineData(3, "2.2.99.2", "2.2.100.2", false)]
        [InlineData(2, "2.2.99.2", "2.3.0.0", true)]
        [InlineData(1, "2,2,2,2", "3,0,0,0", true)]
        public void Increment_IncrementDifferentPositions(int position, string originalVersionText, string expectedVersionText, bool cascade)
        {
            var version = VersionFunctions.ParseVersion(originalVersionText);

            var newVersion = VersionFunctions.Increment(version, position, cascade);

            Assert.Equal(expectedVersionText, newVersion.ToString());
        }

        [Fact]
        public void Increment_InvalidArgument()
        {
            var version = VersionFunctions.ParseVersion("2.2.2.2");

            Assert.Throws<ArgumentOutOfRangeException>(() => VersionFunctions.Increment(version, -1, true));
        }

        [Theory]
        [InlineData(1, "1.1.1", "9.1.1", "9")]
        [InlineData(2, "1.1.1", "1.9.1", "9")]
        [InlineData(3, "1.1.1", "1.1.9", "9")]
        [InlineData(1, "1.1.1-foo", "9.1.1-foo", "9")]
        [InlineData(1, "1.1.1", "009.1.1", "009")]
        [InlineData(1, "1,1,1", "9,1,1", "9")]
        public void Assign_AssignPositions(int position, string originalVersionText, string expectedVersionText, string formattedNumber)
        {
            var version = VersionFunctions.ParseVersion(originalVersionText);

            var newVersion = VersionFunctions.Assign(version, position, formattedNumber);

            Assert.Equal(expectedVersionText, newVersion.ToString());
        }

        [Fact]
        public void Assign_InvalidArgument()
        {
            var version = VersionFunctions.ParseVersion("2.2.2");

            Assert.Throws<ArgumentOutOfRangeException>(() => VersionFunctions.Assign(version, 0, "-1"));
            Assert.Throws<ArgumentOutOfRangeException>(() => VersionFunctions.Assign(version, -1, "0"));
            Assert.Throws<ArgumentException>(() => VersionFunctions.Assign(version, 1, "not a number"));
        }

        [Theory]
        [InlineData("1.1.1", "1.1.1", "")]
        [InlineData("1.1.1", "1.1.1-beta", "-beta")]
        [InlineData("1.1.1-alpha", "1.1.1-beta", "-beta")]
        [InlineData("1.1.1-alpha", "1.1.1", "")]
        [InlineData("1,1,1", "1,1,1-beta", "-beta")]
        public void Label_ChangeLabel(string originalVersionText, string expectedVersionText, string versionLabel)
        {
            var version = VersionFunctions.ParseVersion(originalVersionText);

            var newVersion = VersionFunctions.Label(version, versionLabel);

            Assert.Equal(expectedVersionText, newVersion.ToString());
        }

        [Theory]
        [InlineData("8")]
        [InlineData("100.5")]
        [InlineData("2018.01.01")]
        [InlineData("1.0.0.9")]
        [InlineData("1.0.0.9-foo")]
        [InlineData("1.100aBc")]
        [InlineData("18.5.3-beta03")]
        [InlineData("8.25.3-beta03+master0004")]
        [InlineData("8.5.43+bar")]
        [InlineData("1.0.0_final2")]
        [InlineData("1,0,0,9")]
        public void ParseVersionFromText_ValidText(string versionText)
        {
            var version = VersionFunctions.ParseVersion(versionText);

            Assert.Equal(versionText, version.ToString());
        }

        [Fact]
        public void ParseVersionFromText_InvalidText()
        {
            Assert.Throws<ArgumentException>(() => VersionFunctions.ParseVersion("invalid.input"));
        }

        [Fact]
        public void TryParseVersionInText_Found()
        {
            var success = VersionFunctions.TryParseVersionInText("a1.2a", @"a(?<version>\d\.\d)a", out var version, out var marker);

            Assert.True(success);
            Assert.Equal("1.2", version.ToString());
            Assert.True(string.IsNullOrEmpty(marker));
        }

        [Fact]
        public void TryParseVersionInText_InvalidRegexWithoutNamedGroup()
        {
            var success = VersionFunctions.TryParseVersionInText("100", @"\d", out var version, out var marker);

            Assert.False(success);
            Assert.Null(version);
            Assert.True(string.IsNullOrEmpty(marker));
        }

        [Fact]
        public void TryParseVersionInText_InvalidRegexWithNamedGroup()
        {
            Assert.Throws<ArgumentException>(() => VersionFunctions.TryParseVersionInText("1.0//", @"(?<version>1.0//)", out _, out _));
        }

        [Fact]
        public void TryParseVersionInText_ValidRegexButTextIsNoVersion()
        {
            var success = VersionFunctions.TryParseVersionInText("something", @"a(?<version>\d\.\d)a", out var version, out var marker);

            Assert.False(success);
            Assert.Null(version);
            Assert.True(string.IsNullOrEmpty(marker));
        }

        [Fact]
        public void TryParseVersionInText_ValidMarker()
        {
            var success = VersionFunctions.TryParseVersionInText("foo 1.0", @"(?<marker>foo) (?<version>1.0)", out var _, out var marker);

            Assert.True(success);
            Assert.Equal("foo", marker);
        }

        [Fact]
        public void EnsureExpectedVersion_Ok()
        {
            var expectedText = "a1.2a";
            var version = VersionFunctions.ParseVersion("1.2");

            var actualText = VersionFunctions.EnsureExpectedVersion(expectedText, @"a(?<version>\d\.\d)a", version);

            Assert.Equal(expectedText, actualText);
        }

        [Fact]
        public void EnsureExpectedVersion_WrongText()
        {
            var version = VersionFunctions.ParseVersion("1.2");

            Assert.Throws<InvalidOperationException>(() => VersionFunctions.EnsureExpectedVersion("not a number", @"(?<version>\d\.\d)", version));
        }

        [Fact]
        public void EnsureExpectedVersion_WrongRegex()
        {
            var version = VersionFunctions.ParseVersion("1.2");

            Assert.Throws<InvalidOperationException>(() => VersionFunctions.EnsureExpectedVersion("1.2", "wrong regex", version));
        }
    }
}
