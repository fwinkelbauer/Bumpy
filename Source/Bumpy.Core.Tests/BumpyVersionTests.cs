using System;
using Xunit;

namespace Bumpy.Core.Tests
{
    public class BumpyVersionTests
    {
        [Theory]
        [InlineData("3.0.0.0")]
        [InlineData("2018.01.01")]
        [InlineData("100.52.89.1024")]
        [InlineData("5.17.0-foo+bar")]
        [InlineData("5.17.0_final")]
        [InlineData("3,0,0,0")]
        public void ToString_ReturnsVersionString(string versionText)
        {
            var version = VersionFunctions.ParseVersion(versionText);

            Assert.Equal(versionText, version.ToString());
        }

        [Fact]
        public void Constructor_ValidateInput()
        {
            Assert.Throws<ArgumentException>(() => new BumpyVersion(new int[] { }, new int[] { }, string.Empty, '.'));
            Assert.Throws<ArgumentException>(() => new BumpyVersion(new int[] { 2, 3 }, new int[] { 1 }, string.Empty, '.'));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BumpyVersion(new int[] { 2, -3 }, new int[] { 1, 1 }, string.Empty, '.'));
            Assert.Throws<ArgumentOutOfRangeException>(() => new BumpyVersion(new int[] { 2, 3 }, new int[] { 1, -1 }, string.Empty, '.'));
        }

        [Theory]
        [InlineData("9.1.1")]
        [InlineData("1.9.1")]
        [InlineData("1.1.9")]
        [InlineData("01.1.1")]
        [InlineData("1.01.1")]
        [InlineData("1.1.01")]
        [InlineData("1.1.1-foo")]
        [InlineData("1,1,1")]
        public void Equals_DifferentState(string versionText)
        {
            var version = VersionFunctions.ParseVersion("1.1.1");
            var otherVersion = VersionFunctions.ParseVersion(versionText);

            Assert.False(version.Equals(otherVersion));
            Assert.False(otherVersion.Equals(version));
            Assert.NotEqual(version.GetHashCode(), otherVersion.GetHashCode());
        }

        [Theory]
        [InlineData("1.8.5")]
        [InlineData("0001.008.0005")]
        [InlineData("1.8.5-foo")]
        [InlineData("1,8,5")]
        public void Equals_SameState(string versionText)
        {
            var version = VersionFunctions.ParseVersion(versionText);
            var otherVersion = VersionFunctions.ParseVersion(versionText);

            Assert.True(version.Equals(otherVersion));
            Assert.True(otherVersion.Equals(version));
            Assert.Equal(version.GetHashCode(), otherVersion.GetHashCode());
        }

        [Fact]
        public void Equals_WrongType()
        {
            var version = VersionFunctions.ParseVersion("1.1.1");

            Assert.False(version.Equals(null));
        }
    }
}
