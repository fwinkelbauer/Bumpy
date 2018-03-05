using Bumpy.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bumpy.Tests.Config
{
    [TestClass]
    public class ConfigProcessorTests
    {
        [TestMethod]
        public void Process_GlobCannotBeEmpty()
        {
            Assert.ThrowsException<ConfigException>(() => ConfigProcessor.Process(new BumpyConfigEntry { }));
            Assert.ThrowsException<ConfigException>(() => ConfigProcessor.Process(new BumpyConfigEntry { Glob = string.Empty }));
            Assert.ThrowsException<ConfigException>(() => ConfigProcessor.Process(new BumpyConfigEntry { Glob = null }));
            Assert.ThrowsException<ConfigException>(() => ConfigProcessor.Process(new BumpyConfigEntry { Glob = " " }));
        }

        [TestMethod]
        public void Process_ApplyTemplate()
        {
            var entry = new BumpyConfigEntry { Glob = "*.csproj" };

            var newEntry = ConfigProcessor.Process(entry);

            Assert.IsFalse(string.IsNullOrWhiteSpace(newEntry.Regex));
        }

        [TestMethod]
        public void Process_NoTemplateFound()
        {
            var entry = new BumpyConfigEntry { Glob = "unsupported.foo" };

            Assert.ThrowsException<ConfigException>(() => ConfigProcessor.Process(entry));
        }
    }
}
