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
        public void ReadConfigFile_CanDeserializeConfig()
        {
            var entries = ConfigIO.ReadConfigFile(GetLines()).Queries.ToList();

            Assert.AreEqual(3, entries.Count);

            Assert.AreEqual("AssemblyInfo.cs", entries[0].Glob);
            Assert.AreEqual("assembly", entries[0].Profile);

            Assert.AreEqual("*.nuspec", entries[1].Glob);
            Assert.AreEqual("nuspec", entries[1].Profile);

            Assert.AreEqual("*.csproj", entries[2].Glob);
            Assert.AreEqual("nuspec", entries[2].Profile);
        }

        private IEnumerable<string> GetLines()
        {
            // We also make sure that the default "new" template is valid
            using (var stream = new StringReader(ConfigIO.NewConfigFile()))
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
