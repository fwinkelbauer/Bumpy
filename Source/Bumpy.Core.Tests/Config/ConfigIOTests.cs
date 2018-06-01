using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bumpy.Core.Config;
using Xunit;

namespace Bumpy.Core.Tests.Config
{
    public class ConfigIOTests
    {
        [Fact]
        public void ReadConfigFile_ParseDefaultConfig()
        {
            // We also make sure that the default "new" template is valid
            var entries = ReadConfig(ConfigIO.NewConfigFile()).ToList();

            Assert.Equal(3, entries.Count);

            Assert.Equal("AssemblyInfo.cs", entries[0].Glob);
            Assert.Equal("assembly", entries[0].Profile);

            Assert.Equal("*.nuspec", entries[1].Glob);
            Assert.Equal("nuspec", entries[1].Profile);

            Assert.Equal("*.csproj", entries[2].Glob);
            Assert.Equal("nuspec", entries[2].Profile);
        }

        [Fact]
        public void ReadConfigFile_ParseCompleteEntry()
        {
            string configText = @"
[AssemblyInfo.cs | my_profile]
regex = some regex
encoding = utf-8

[*.rc]
encoding = 1200
";
            var entries = ReadConfig(configText).ToList();

            Assert.Equal(2, entries.Count);

            Assert.Equal("AssemblyInfo.cs", entries[0].Glob);
            Assert.Equal("my_profile", entries[0].Profile);
            Assert.Equal("Unicode (UTF-8)", entries[0].Encoding.EncodingName);
            Assert.Equal("some regex", entries[0].Regex);

            Assert.Equal(1200, entries[1].Encoding.CodePage);
        }

        [Fact]
        public void ReadConfigFile_InvalidSyntax()
        {
            string configText = @"
[AssemblyInfo.cs | my_profile]
regex
";
            Assert.Throws<ConfigException>(() => ReadConfig(configText));
        }

        [Fact]
        public void ReadConfigFile_UnrecognizedElement()
        {
            string configText = @"
[AssemblyInfo.cs | my_profile]
unknown_key = some value
";
            Assert.Throws<ConfigException>(() => ReadConfig(configText));
        }

        [Fact]
        public void ReadConfigFile_SameSectionTwice()
        {
            string configText = @"
[AssemblyInfo.cs | some_profile]

[AssemblyInfo.cs | some_profile]";

            Assert.Throws<ConfigException>(() => ReadConfig(configText));
        }

        private IEnumerable<BumpyConfigEntry> ReadConfig(string configText)
        {
            return ConfigIO.ReadConfigFile(GetLines(configText));
        }

        private IEnumerable<string> GetLines(string configText)
        {
            using (var stream = new StringReader(configText))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
