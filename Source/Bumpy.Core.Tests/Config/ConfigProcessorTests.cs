using Bumpy.Core.Config;
using Xunit;

namespace Bumpy.Core.Tests.Config
{
    public class ConfigProcessorTests
    {
        [Fact]
        public void Process_GlobCannotBeEmpty()
        {
            Assert.Throws<ConfigException>(() => ConfigProcessor.Process(new BumpyConfigEntry()));
            Assert.Throws<ConfigException>(() => ConfigProcessor.Process(new BumpyConfigEntry { Glob = string.Empty }));
            Assert.Throws<ConfigException>(() => ConfigProcessor.Process(new BumpyConfigEntry { Glob = null }));
            Assert.Throws<ConfigException>(() => ConfigProcessor.Process(new BumpyConfigEntry { Glob = " " }));
        }

        [Fact]
        public void Process_ApplyTemplate()
        {
            var entry = new BumpyConfigEntry { Glob = "*.csproj" };

            var newEntry = ConfigProcessor.Process(entry);

            Assert.False(string.IsNullOrWhiteSpace(newEntry.Regex));
        }

        [Fact]
        public void Process_NoTemplateFound()
        {
            var entry = new BumpyConfigEntry { Glob = "unsupported.foo" };

            Assert.Throws<ConfigException>(() => ConfigProcessor.Process(entry));
        }
    }
}
