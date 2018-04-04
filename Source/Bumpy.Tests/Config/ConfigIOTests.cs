using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bumpy.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.Tests.Config
{
    [TestClass]
    public class ConfigIOTests
    {
        [TestMethod]
        public void ReadConfigFile_ParseDefaultConfig()
        {
            // We also make sure that the default "new" template is valid
            var entries = ReadConfig(ConfigIO.NewConfigFile()).ToList();

            Assert.AreEqual(3, entries.Count);

            Assert.AreEqual("AssemblyInfo.cs", entries[0].Glob);
            Assert.AreEqual("assembly", entries[0].Profile);

            Assert.AreEqual("*.nuspec", entries[1].Glob);
            Assert.AreEqual("nuspec", entries[1].Profile);

            Assert.AreEqual("*.csproj", entries[2].Glob);
            Assert.AreEqual("nuspec", entries[2].Profile);
        }

        [TestMethod]
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

            Assert.AreEqual(2, entries.Count);

            Assert.AreEqual("AssemblyInfo.cs", entries[0].Glob);
            Assert.AreEqual("my_profile", entries[0].Profile);
            Assert.AreEqual("Unicode (UTF-8)", entries[0].Encoding.EncodingName);
            Assert.AreEqual("some regex", entries[0].Regex);

            Assert.AreEqual(1200, entries[1].Encoding.CodePage);
        }

        [TestMethod]
        public void ReadConfigFile_InvalidSyntax()
        {
            string configText = @"
[AssemblyInfo.cs | my_profile]
regex
";
            Assert.ThrowsException<ConfigException>(() => ReadConfig(configText));
        }

        [TestMethod]
        public void ReadConfigFile_UnrecognizedElement()
        {
            string configText = @"
[AssemblyInfo.cs | my_profile]
unknown_key = some value
";
            Assert.ThrowsException<ConfigException>(() => ReadConfig(configText));
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
